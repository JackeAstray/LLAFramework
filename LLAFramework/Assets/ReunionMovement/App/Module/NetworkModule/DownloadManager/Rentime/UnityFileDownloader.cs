using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace LLAFramework.Download
{
    /// <summary>
    /// Unity文件下载器
    /// </summary>
    public class UnityFileDownloader : IDownloader
    {
        public UnityFileDownloader() { }

        public UnityFileDownloader(
            string downloadPath,
            bool downloadToRoot,
            int maxConcurrency,
            bool abandonOnFailure,
            bool continueAfterFailure,
            bool tryMultipartDownload,
            List<string> uris
        )
        {
            DownloadPath = downloadPath;
            DownloadToRoot = downloadToRoot;
            MaxConcurrency = maxConcurrency;
            AbandonOnFailure = abandonOnFailure;
            ContinueAfterFailure = continueAfterFailure;
            this.tryMultipartDownload = tryMultipartDownload;
            Uris = uris?.ToArray() ?? Array.Empty<string>();
        }

        internal readonly static SemaphoreLocker Locker = new SemaphoreLocker();

        internal int InitialCount;
        internal int timeout = 6;
        internal int maxConcurrency = 4;
        internal bool abandonOnFailure = true;
        internal bool continueAfterFailure = false;
        internal bool downloading = false;
        internal bool paused = false;
        internal bool didError = false;
        internal int numFilesRemaining = 0;
        internal int startTime = 0, endTime = 0;
        internal string downloadPath;
        internal bool downloadToRoot;
        internal string[] pendingUris = null;

        #region Events/Actions
        public event Action OnDownloadsSuccess;
        public event Action OnDownloadInvoked;
        public event Action OnCancelInvoked;
        public event Action<string> OnCancelIndividual;
        public event Action<string> OnDownloadIndividualInvoked;
        public event Action OnCancel;
        #endregion

        public override string iDownloadExecutorClassName => "UWRExecutor";

        public override int StartTime => startTime;
        public override int EndTime => endTime;

        /// <summary>
        /// Calculates the amount of files-per-second this downloder processed. 
        /// </summary>
        /// <value></value>
        public override float NumFilesPerSecond
        {
            get
            {
                if (ElapsedTime == 0 || DownloadedUris == null || DownloadedUris.Length == 0)
                {
                    return 0f;
                }
                return (DownloadedUris.Length * 1000f) / ElapsedTime;
            }
        }

        public float MegabytesDownloadedPerSecond => BytesDownloadedPerSecond / 1000f;

        public float BytesDownloadedPerSecond
        {
            get
            {
                float totalBytes = 0;
                float totalElapsed = 0;
                foreach (var idf in executors.Concat(executorsOld))
                {
                    totalBytes += idf.BytesDownloaded;
                    totalElapsed += idf.ElapsedTime;
                }
                return totalElapsed > 0 ? totalBytes / totalElapsed : 0;
            }
        }

        public override int Timeout
        {
            get => timeout;
            set => timeout = value;
        }

        public override int MaxConcurrency
        {
            get => maxConcurrency;
            set => maxConcurrency = value;
        }

        public override string DownloadPath
        {
            get => downloadPath;
            set => downloadPath = value;
        }

        public override bool DownloadToRoot
        {
            get => downloadToRoot;
            set => downloadToRoot = value;
        }

        public override bool AbandonOnFailure
        {
            get => abandonOnFailure;
            set => abandonOnFailure = value;
        }

        public override bool ContinueAfterFailure
        {
            get => continueAfterFailure;
            set => continueAfterFailure = value;
        }

        public override bool Downloading => downloading;

        public override bool DidError
        {
            get => didError;
            protected set => didError = value;
        }

        public override bool Paused => paused;

        public override int NumFilesRemaining => numFilesRemaining;

        public override string[] PendingURIS => pendingUris;

        public override List<string> IncompletedURIS => incompletedUris;

        public int NumThreads => n;
        internal int n = 0;

        /// <summary>
        /// 下载器的URI
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> Download()
        {
            if (Downloading || Uris == null || Uris.Length == 0)
            {
                Log.Error($"{GetType().FullName}.Download() 不能在属性Uris设置为null或empty的情况下调用");
                return false;
            }
            OnDownloadInvoked?.Invoke();
            pendingUris = Uris.ToArray();
            numFilesRemaining = Uris.Length;
            startTime = DateTime.Now.Millisecond;
            downloading = true;

            InitialCount = Uris.Length;
            int threadCount = Math.Min(MaxConcurrency, numFilesRemaining);
            if (threadCount <= 0)
            {
                Log.Error($"{GetType().FullName}.下载要求MaxConcurrency为非负整数。");
                return false;
            }
            var tasks = new List<Task<bool>>(threadCount);
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Dispatch());
            }

            await Task.WhenAll(tasks);

            while (Downloading && !(DidError && !ContinueAfterFailure))
            {
                await Task.Delay(75);
            }

            return true;
        }

        /// <summary>
        /// 下载单个URI
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public override async Task<bool> Download(string uri)
        {
            if (!Downloading)
            {
                downloading = true;
                startTime = DateTime.Now.Millisecond;
                InitialCount = 1;
            }

            if (!executors.Any(idf => idf.Uri == uri) && !executorsOld.Any(idf => idf.Uri == uri))
            {
                var idf = DownloadExecutorFactory.CreateFromClassName(iDownloadExecutorClassName);
                idf.Uri = uri;
                idf.DownloadPath = DownloadPath;
                idf.DownloadToRoot = DownloadToRoot;
                idf.AbandonOnFailure = AbandonOnFailure;
                idf.Timeout = Timeout;
                pendingUris = pendingUris.Append(uri).ToArray();
                executors = executors.Append(idf).ToArray();
                numFilesRemaining++;
            }
            else if (executorsOld.Any(idf => idf.Uri == uri))
            {
                var idf = executorsOld.First(idf => idf.Uri == uri);
                executorsOld = executorsOld.Where(x => x != idf).ToArray();
                pendingUris = pendingUris.Append(uri).ToArray();
                executors = executors.Append(idf).ToArray();
            }

            OnDownloadIndividualInvoked?.Invoke(uri);
            return true;
        }

        /// <summary>
        /// 返回false的异步方法
        /// </summary>
        /// <returns></returns>
        internal Task<bool> ReturnFalseAsync() => Task.FromResult(false);

        /// <summary>
        /// 派遣下载器
        /// </summary>
        internal Task<bool> Dispatch()
        {
            if (pendingUris == null || pendingUris.Length == 0)
            {
                return ReturnFalseAsync();
            }

            string uri = pendingUris[0];
            IDownloadExecutor idf = executors[0];
            pendingUris = pendingUris.Skip(1).ToArray();
            executors = executors.Skip(1).ToArray();
            executorsOld = executorsOld.Append(idf).ToArray();

            if (idf.CompletedMultipartDownload)
            {
                return ReturnFalseAsync();
            }

            if (!idf.DidHeadReq && idf.TryMultipartDownload)
            {
                var treq = ((UWRExecutor)idf).HeadRequest();
                n++;
                treq.completed += (obj) =>
                {
                    var rv = idf.Download();
                    rv.completed += resp =>
                    {
                        n--;
                        _ = DispatchCompletion(idf);
                    };
                };
                return ReturnFalseAsync();
            }

            var req = idf.Download();
            if (req == null)
            {
                _ = DispatchCompletion();
                return ReturnFalseAsync();
            }
            n++;
            req.completed += resp =>
            {
                n--;
                _ = DispatchCompletion(idf);
            };
            return ReturnFalseAsync();
        }

        /// <summary>
        /// Dispatches a given IDF, for multi-part downloads.
        /// </summary>
        /// <param name="idf"></param>
        /// <returns></returns>
        internal Task<bool> Dispatch(IDownloadExecutor idf)
        {
            var req = idf.Download();
            if (req == null)
            {
                _ = DispatchCompletion();
                return ReturnFalseAsync();
            }
            n++;
            req.completed += resp =>
            {
                n--;
                _ = DispatchCompletion(idf);
            };
            return ReturnFalseAsync();
        }

        internal async Task DispatchCompletion()
        {
            await Locker.LockAsync(async () =>
            {
                if (!Downloading)
                {
                    return;
                }

                if (pendingUris.Length > 0)
                {
                    _ = Dispatch();
                }

                else if (NumThreads == 0)
                {
                    OnDownloadsSuccess?.Invoke();
                    endTime = DateTime.Now.Millisecond;
                    downloading = false;
                }
            });
        }

        /// <summary>
        /// 以同步方式处理调度完成（允许等待）
        /// </summary>
        /// <param name="idf"></param>
        /// <returns></returns>
        internal async Task DispatchCompletion(IDownloadExecutor idf)
        {
            await Locker.LockAsync(async () =>
            {
                if (!Downloading)
                {
                    idf.Cancel();
                    return;
                }

                if (idf.DidError)
                {
                    if (ContinueAfterFailure && pendingUris.Length > 0)
                    {
                        _ = Dispatch();
                    }
                    else
                    {
                        await Cancel();
                    }
                }
                else
                {
                    if (!idf.CompletedMultipartDownload && idf.MultipartDownload)
                    {
                        _ = Dispatch(idf);
                        return;
                    }
                    if (pendingUris.Length > 0)
                    {
                        _ = Dispatch();
                    }
                    else if (NumThreads == 0)
                    {
                        OnDownloadsSuccess?.Invoke();
                        endTime = DateTime.Now.Millisecond;
                        downloading = false;
                    }
                }
            });
        }

        /// <summary>
        /// 获取对应的执行器
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IDownloadExecutor GetExecutor(string uri)
        {
            var exec = executors.FirstOrDefault(idf => idf.Uri == uri);
            if (exec != null)
            {
                return exec;
            }
            return executorsOld.FirstOrDefault(idf => idf.Uri == uri);
        }

        /// <summary>
        /// 取消所有下载
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> Cancel()
        {
            downloading = false;
            OnCancel?.Invoke();
            endTime = DateTime.Now.Millisecond;
            HandleAbandonOnFailure();
            return true;
        }

        /// <summary>
        /// 取消单个下载
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public override async Task<bool> Cancel(string uri)
        {
            OnCancelIndividual?.Invoke(uri);

            var exec = executors.FirstOrDefault(idf => idf.Uri == uri);
            if (exec != null)
            {
                exec.Cancel();
                executors = executors.Where(x => x != exec).ToArray();
                executorsOld = executorsOld.Append(exec).ToArray();
            }
            else if (!executorsOld.Any(idf => idf.Uri == uri))
            {
                Log.Error($"对从未调用过的URI调用取消 {uri}");
                return false;
            }
            else
            {
                Log.Error("对已完成的URI调用取消");
            }
            return true;
        }

        /// <summary>
        /// 如果失败时放弃为真，则删除所有文件。
        /// </summary>
        internal void HandleAbandonOnFailure()
        {
            if (AbandonOnFailure)
            {
                foreach (var idf in executors.Concat(executorsOld))
                {
                    idf.Cancel();
                }
            }
        }

        /// <summary>
        /// 重置与此下载器相关的所有属性
        /// </summary>
        public override void Reset()
        {
            if (Downloading)
            {
                Log.Error("下载时无法调用重置，需要先取消下载。");
                return;
            }
            downloading = false;
            timeout = 6;
            maxConcurrency = 4;
            abandonOnFailure = true;
            continueAfterFailure = false;
            paused = false;
            didError = false;
            numFilesRemaining = 0;
            startTime = 0;
            endTime = 0;
            pendingUris = Array.Empty<string>();
            downloadedUris = new List<string>();
            incompletedUris = new List<string>();
            executors = Array.Empty<IDownloadExecutor>();
            executorsOld = Array.Empty<IDownloadExecutor>();
            Uris = Array.Empty<string>();
        }
    }
}
