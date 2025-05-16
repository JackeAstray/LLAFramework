using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLAFramework.Download
{
    /// <summary>
    /// 下载器接口
    /// </summary>
    public abstract class IDownloader
    {
        /// <summary>
        /// URI列表
        /// </summary>
        protected string[] uris = Array.Empty<string>();
        /// <summary>
        /// 下载器的URI列表
        /// </summary>
        protected List<string> downloadedUris = new List<string>();
        /// <summary>
        /// 不完整的URI列表
        /// </summary>
        protected List<string> incompletedUris = new List<string>();
        /// <summary>
        /// 是否使用分块下载
        /// </summary>
        public Dictionary<string, string> requestHeaders = null;
        /// <summary>
        /// 是否使用分块下载
        /// </summary>
        public bool tryMultipartDownload = true;
        /// <summary>
        /// 下载执行器
        /// </summary>
        protected IDownloadExecutor[] executors = Array.Empty<IDownloadExecutor>();
        /// <summary>
        /// 旧的下载执行器
        /// </summary>
        protected IDownloadExecutor[] executorsOld = Array.Empty<IDownloadExecutor>();
        /// <summary>
        /// 下载执行器类名
        /// </summary>
        public virtual string iDownloadExecutorClassName => "UWRExecutor";
        /// <summary>
        /// 下载错误事件
        /// </summary>
        public event Action<string, int, string> OnDownloadError;
        /// <summary>
        /// 下载成功事件
        /// </summary>
        public event Action<string> OnDownloadSuccess;
        public event Action<string> OnDownloadChunkedSucces;

        public abstract Task<bool> Download();
        public abstract Task<bool> Download(string uri);
        public abstract Task<bool> Cancel(string uri);
        public abstract Task<bool> Cancel();
        public abstract void Reset();

        public float Progress
        {
            get
            {
                int total = executors.Length + executorsOld.Length;
                if (total == 0)
                {
                    return 0f;
                }

                float prog = executors.Sum(e => e.Progress) + executorsOld.Sum(e => e.Progress);

                return prog / total;
            }
        }

        public int NumFilesTotal => executors.Length;
        public bool Completed => Progress == 1.0f;
        public int MultipartChunkSize = 200000;

        public abstract string DownloadPath { get; set; }
        public abstract bool DownloadToRoot { get; set; }

        public float GetProgress(string uri)
        {
            if (string.IsNullOrEmpty(uri) || uris == null || uris.Length == 0)
            {
                return 0f;
            }

            foreach (var exec in executors)
            {
                if (exec.Uri == uri) return exec.Progress;
            }
            foreach (var exec in executorsOld)
            {
                if (exec.Uri == uri) return exec.Progress;
            }

            return 0f;
        }

        public string[] Uris
        {
            get => uris;
            set
            {
                if (value == null)
                {
                    return;
                }

                var validUris = new List<string>();
                var newExecutors = new List<IDownloadExecutor>();

                foreach (var str in value)
                {
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        continue;
                    }

                    if (!Uri.TryCreate(str, UriKind.Absolute, out _))
                    {
                        Log.Error($"URI {str} cannot be fed into {GetType().Name}.Uris");
                        continue;
                    }

                    var idf = DownloadExecutorFactory.CreateFromClassName(iDownloadExecutorClassName);
                    idf.Uri = str;
                    idf.DownloadPath = DownloadPath;
                    idf.DownloadToRoot = DownloadToRoot;
                    idf.AbandonOnFailure = AbandonOnFailure;
                    idf.Timeout = Timeout;
                    idf.RequestHeaders = requestHeaders;
                    idf.TryMultipartDownload = tryMultipartDownload;
                    idf.IntitialChunkSize = MultipartChunkSize;
                    newExecutors.Add(idf);
                    PendingURIS?.Add(str);

                    if (idf is UWRExecutor uwr)
                    {
                        uwr.OnDownloadChunkedSucces += () => OnDownloadChunkedSucces?.Invoke(idf.Uri);
                        uwr.OnDownloadError += (errorCode, errorMsg) =>
                        {
                            DidError = true;
                            OnDownloadError?.Invoke(idf.Uri, errorCode, errorMsg);
                            incompletedUris.Add(idf.Uri);
                        };
                        uwr.OnDownloadSuccess += () =>
                        {
                            downloadedUris.Add(idf.Uri);
                            OnDownloadSuccess?.Invoke(idf.Uri);
                        };
                    }

                    validUris.Add(str);
                }

                uris = validUris.ToArray();
                executors = newExecutors.ToArray();
            }
        }

        public abstract int Timeout { get; set; }
        public abstract int MaxConcurrency { get; set; }
        public abstract float NumFilesPerSecond { get; }
        public abstract bool AbandonOnFailure { get; set; }
        public abstract bool ContinueAfterFailure { get; set; }
        public abstract bool Downloading { get; }
        public abstract bool Paused { get; }
        public abstract bool DidError { get; protected set; }
        public abstract int NumFilesRemaining { get; }
        public abstract int StartTime { get; }
        public abstract int EndTime { get; }

        public int ElapsedTime
        {
            get
            {
                if (StartTime == 0)
                {
                    return 0;
                }

                if (EndTime == 0)
                {
                    return Math.Abs(Environment.TickCount - StartTime);
                }

                return Math.Abs(EndTime - StartTime);
            }
        }

        public abstract string[] PendingURIS { get; }
        public string[] DownloadedUris => downloadedUris.ToArray();
        public abstract List<string> IncompletedURIS { get; }
    }
}
