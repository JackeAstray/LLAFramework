using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Download
{
    public abstract class IDownloader
    {
        /// <summary>
        /// 使用给定的 URI 构造一个 IDownloader子对象
        /// </summary>
        /// <param name="uris"></param>
        public IDownloader(params string[] uris)
        {
            Uris = uris;
        }

        /// <summary>
        /// 用于存储将要下载的 URI
        /// </summary>
        protected string[] uris;

        /// <summary>
        /// 存储下载成功的URI
        /// </summary>
        protected string[] downloadedUris;

        /// <summary>
        /// 存储下载失败的URI
        /// </summary>
        protected string[] incompletedUris;

        /// <summary>
        /// 用于任何已分派请求的标头
        /// </summary>
        public Dictionary<string, string> requestHeaders = null;

        public bool tryMultipartDownload = true;

        /// <summary>
        /// 存储与每个要下载的文件关联的实际 IDownloadFulfiller
        /// </summary>
        protected IDownloadFulfiller[] fulfillers = new IDownloadFulfiller[0];

        protected IDownloadFulfiller[] fulfillersOld = new IDownloadFulfiller[0];

        /// <summary>
        /// 使用‘IDownloadFulfillerFactory’，该字符串将被传递到‘CreateFromClassName’函数中，以在‘Uris’上构造‘Fulfillers’进行分配。
        /// </summary>
        public virtual string IDownloadFulfillerClassName => "UWRFulfiller";

        /// <summary>
        /// 发生错误时调用，其中第一个参数是 uri，第二个参数是错误代码，第三个参数是错误消息。
        /// </summary>
        public event Action<string, int, string> OnDownloadError;

        /// <summary>
        /// 当文件已下载时调用，参数名称为 uri
        /// </summary>
        public event Action<string> OnDownloadSuccess, OnDownloadChunkedSucces;

        /// <summary>
        /// Initiates a download from 'Uris'.
        /// </summary>
        public abstract Task<bool> Download();

        /// <summary>
        /// 启动特定 URI 的下载
        /// 这用于定位其他 URI 或取消正在下载的特定 URI
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> Download(string uri);

        /// <summary>
        /// 如果适用，暂停特定 URI 的下载器
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public abstract Task<bool> Cancel(string uri);

        /// <summary>
        /// Cancels this downloader, if applicable.
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> Cancel();



        public abstract void Reset();

        /// <summary>
        /// Calculates progress based on 'Fulfillers' from 0f to 1f.
        /// </summary>
        /// <value></value>
        public float Progress
        {
            get
            {
                // progress = (allProg) / numFiles
                float prog = 0;
                float num = NumFilesTotal;
                foreach (var idf in fulfillers) prog += idf.Progress;
                return prog;
            }
        }

        /// <summary>
        /// Calculates the total amount of files to download based on 'Fulfillers'.
        /// </summary>
        /// <value></value>
        public int NumFilesTotal => fulfillers.Length;

        /// <summary>
        /// Returns true when this downloader has completed.
        /// </summary>
        public bool Completed => Progress == 1.0f;

        public int multipartChunkSize = 200000;

        public abstract string DownloadPath
        {
            get; set;
        }

        /// <summary>
        /// Gets progress associated with a specific URI, if it exists.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public float GetProgress(string uri)
        {
            if (!uris.ToList().Contains(uri)) return 0f;
            return fulfillers.ToList().Where(idf => idf.Uri == uri).ToArray().Length == 1 ? fulfillers.ToList().Find(idf => idf.Uri == uri).Progress : fulfillersOld.ToList().Find(idf => idf.Uri == uri).Progress;
        }

        /// <summary>
        /// Set upon construction or manually, but a 'Download' invokation must follow.
        /// Checks URI for basic parsing before allowing.
        /// </summary>
        public string[] Uris
        {
            get => uris;
            set
            {
                List<string> list = new List<string>();
                //foreach (var idf in _Fulfillers) idf.Dispose();
                this.fulfillers = null;
                List<IDownloadFulfiller> fulfillers = new List<IDownloadFulfiller>();
                foreach (var str in value)
                {
                    try
                    {
                        Uri uri = new Uri(str); // dummy way of checking for valid URIs
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"URI {str} cannot be fed into {GetType().Name}.Uris");
                        return;
                    }
                    IDownloadFulfiller idf = DownloadFulfillerFactory.CreateFromClassName(IDownloadFulfillerClassName);
                    idf.Uri = str;
                    idf.DownloadPath = DownloadPath;
                    idf.AbandonOnFailure = AbandonOnFailure;
                    idf.Timeout = Timeout;
                    idf.RequestHeaders = requestHeaders;
                    idf.TryMultipartDownload = tryMultipartDownload;
                    idf.intitialChunkSize = multipartChunkSize;
                    fulfillers.Add(idf);
                    PendingURIS.Add(str);

                    /// <summary>
                    /// Invok action on parent
                    /// </summary>
                    /// <returns></returns>
                    ((UWRFulfiller)idf).OnDownloadChunkedSucces += () =>
                    {
                        OnDownloadChunkedSucces?.Invoke(idf.Uri);
                    };

                    /// <summary>
                    /// 处理 AbandonOnFailure, updating 'DidError' and dispatching 'OnError', and IncompleteUris
                    /// </summary>
                    /// <param name="errorCode"></param>
                    /// <param name="errorMsg"></param>
                    /// <returns></returns>
                    ((UWRFulfiller)idf).OnDownloadError += async (int errorCode, string errorMsg) =>
                    {
                        DidError = true;
                        OnDownloadError?.Invoke(idf.Uri, errorCode, errorMsg);
                        incompletedUris.Add(new string[] { idf.Uri });
                    };

                    /// <summary>
                    /// 异步更新“DownloadedURIS”
                    /// TODO:通过将事件保留在父类中来避免强制转换
                    /// </summary>
                    /// <returns></returns>
                    ((UWRFulfiller)idf).OnDownloadSuccess += () =>
                    {
                        downloadedUris.Add(new string[] { idf.Uri });
                        OnDownloadSuccess?.Invoke(idf.Uri);
                    };
                    list.Add(str);
                }
                uris = list.ToArray();
                this.fulfillers = fulfillers.ToArray();
            }
        }

        /// <summary>
        /// 超时
        /// </summary>
        public abstract int Timeout
        {
            get; set;
        }

        /// <summary>
        /// 最大并发数
        /// </summary>
        public abstract int MaxConcurrency
        {
            get; set;
        }

        /// <summary>
        /// 返回此下载程序每秒处理的文件数量，仅计算下载启动后和下载开始之前的时间。
        /// </summary>
        /// <value></value>
        public abstract float NumFilesPerSecond
        {
            get;
        }

        /// <summary>
        /// 如果为真，那么该下载程序将删除所有部分下载的文件。
        /// </summary>
        /// <value></value>
        public abstract bool AbandonOnFailure
        {
            get; set;
        }

        /// <summary>
        /// 如果为真，则此下载程序将在错误发生后继续尝试下载文件。
        /// </summary>
        /// <value></value>
        public abstract bool ContinueAfterFailure
        {
            get; set;
        }

        /// <summary>
        /// 下载中
        /// </summary>
        public abstract bool Downloading
        {
            get;
        }

        /// <summary>
        /// 已暂停
        /// </summary>
        public abstract bool Paused
        {
            get;
        }

        /// <summary>
        /// 有错误
        /// </summary>
        public abstract bool DidError
        {
            get; protected set;
        }

        /// <summary>
        /// 剩余文件数
        /// </summary>
        public abstract int NumFilesRemaining
        {
            get;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public abstract int StartTime
        {
            get;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public abstract int EndTime
        {
            get;
        }

        /// <summary>
        /// 已用时间
        /// </summary>
        public int ElapsedTime => Math.Abs(StartTime == 0 ? 0 : (EndTime == 0 ? DateTime.Now.Millisecond - StartTime : EndTime - StartTime));

        /// <summary>
        /// 待处理URIS
        /// </summary>
        public abstract string[] PendingURIS
        {
            get;
        }

        /// <summary>
        /// 下载的Uris
        /// </summary>
        public string[] DownloadedUris => downloadedUris;

        /// <summary>
        /// 未完成的 URIS
        /// </summary>
        public abstract string[] IncompletedURIS
        {
            get;
        }
    }
}
