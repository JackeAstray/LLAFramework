using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic.Download
{
    public class DownloadFile
    {
        /// <summary>
        /// 主线程
        /// </summary>
        private SynchronizationContext mainThreadSynContext;

        /// <summary>
        /// 下载网址
        /// </summary>
        public string Url;

        public event Action<Exception> OnError;

        private static object errorlock = new object();

        /// <summary>
        /// 主要用于关闭线程
        /// </summary>
        private bool isDownload = false;
        public DownloadFile(string url)
        {
            // 主线程赋值
            mainThreadSynContext = SynchronizationContext.Current;
            // 突破Http协议的并发连接数限制
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;

            Url = url;
        }

        /// <summary>
        /// 查询文件大小
        /// </summary>
        /// <returns></returns>
        public long GetFileSize()
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(new Uri(Url));
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // 获得文件长度
                    long contentLength = response.ContentLength;
                    return contentLength;
                }
            }
            catch (Exception ex)
            {
                OnErrorEX(ex);
                return -1;
            }
        }

        /// <summary>
        /// 异步查询文件大小
        /// </summary>
        /// <param name="onTrigger"></param>
        public void GetFileSizeAsyn(Action<long> onTrigger = null)
        {
            Task.Run(() =>
            {
                PostMainThreadAction<long>(onTrigger, GetFileSize());
            });
        }

        /// <summary>
        /// 多线程下载文件至本地
        /// </summary>
        /// <param name="threadCount">线程总数</param>
        /// <param name="filePath">保存文件路径</param>
        /// <param name="onDownloading">下载过程回调（已下载文件大小、总文件大小）</param>
        /// <param name="onTrigger">下载完毕回调（下载文件数据）</param>
        public void DownloadToFile(int threadCount, string filePath, Action<long, long> onDownloading = null, Action<byte[]> onTrigger = null)
        {
            isDownload = true;

            long csize = 0; //已下载大小
            int ocnt = 0;   //完成线程数


            // 下载逻辑
            GetFileSizeAsyn((size) =>
            {
                if (size == -1) return;
                // 准备工作
                var tempFilePaths = new string[threadCount];
                var tempFileFileStreams = new FileStream[threadCount];
                var dirPath = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileName(filePath);
                // 下载根目录不存在则创建
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                // 查看下载临时文件是否可以继续断点续传
                var fileInfos = new DirectoryInfo(dirPath).GetFiles(fileName + "*.temp");
                if (fileInfos.Length != threadCount)
                {
                    // 下载临时文件数量不相同，则清理
                    foreach (var info in fileInfos)
                    {
                        info.Delete();
                    }
                }
                // 创建下载临时文件，并创建文件流
                for (int i = 0; i < threadCount; i++)
                {
                    tempFilePaths[i] = filePath + i + ".temp";
                    if (!File.Exists(tempFilePaths[i]))
                    {
                        File.Create(tempFilePaths[i]).Dispose();
                    }
                    tempFileFileStreams[i] = File.OpenWrite(tempFilePaths[i]);
                    tempFileFileStreams[i].Seek(tempFileFileStreams[i].Length, System.IO.SeekOrigin.Current);

                    csize += tempFileFileStreams[i].Length;
                }
                // 单线程下载过程回调函数
                Action<int, long, byte[], byte[]> t_onDownloading = (index, rsize, rbytes, data) =>
                {
                    csize += rsize;
                    tempFileFileStreams[index].Write(rbytes, 0, (int)rsize);
                    PostMainThreadAction<long, long>(onDownloading, csize, size);
                };
                // 单线程下载完毕回调函数
                Action<int, byte[]> t_onTrigger = (index, data) =>
                {
                    // 关闭文件流
                    tempFileFileStreams[index].Close();
                    ocnt++;
                    if (ocnt >= threadCount)
                    {
                        // 将临时文件转为下载文件
                        if (!File.Exists(filePath))
                        {
                            File.Create(filePath).Dispose();
                        }
                        else
                        {
                            File.WriteAllBytes(filePath, new byte[] { });
                        }
                        FileStream fs = File.OpenWrite(filePath);
                        fs.Seek(fs.Length, System.IO.SeekOrigin.Current);
                        foreach (var tempPath in tempFilePaths)
                        {
                            var tempData = File.ReadAllBytes(tempPath);
                            fs.Write(tempData, 0, tempData.Length);
                            File.Delete(tempPath);
                        }
                        fs.Close();
                        PostMainThreadAction<byte[]>(onTrigger, File.ReadAllBytes(filePath));
                    }
                };
                // 分割文件尺寸，多线程下载
                long[] sizes = SplitFileSize(size, threadCount);
                Parallel.For(0, sizes.Length / 2, i =>
                {
                    long from = sizes[i * 2];
                    long to = sizes[i * 2 + 1];

                    // 断点续传
                    from += tempFileFileStreams[i].Length;
                    if (from >= to)
                    {
                        t_onTrigger(i, null);
                        return;
                    }

                    ThreadDownload(i, from, to, t_onDownloading, t_onTrigger);
                });
            });
        }
        /// <summary>
        /// 多线程下载文件至内存
        /// </summary>
        /// <param name="threadCount">线程总数</param>
        /// <param name="onDownloading">下载过程回调（已下载文件大小、总文件大小）</param>
        /// <param name="onTrigger">下载完毕回调（下载文件数据）</param>
        public void DownloadToMemory(int threadCount, Action<long, long> onDownloading = null, Action<byte[]> onTrigger = null)
        {
            isDownload = true;

            long csize = 0; // 已下载大小
            int ocnt = 0;   // 完成线程数
            byte[] cdata;  // 已下载数据
            // 下载逻辑
            GetFileSizeAsyn((size) =>
            {
                cdata = new byte[size];

                // 单线程下载过程回调函数
                Action<int, long, byte[], byte[]> t_onDownloading = (index, rsize, rbytes, data) =>
                {
                    csize += rsize;
                    PostMainThreadAction<long, long>(onDownloading, csize, size);
                };

                // 单线程下载完毕回调函数
                Action<int, byte[]> t_onTrigger = (index, data) =>
                {
                    long dIndex = (long)Math.Ceiling((double)(size * index / threadCount));
                    Array.Copy(data, 0, cdata, dIndex, data.Length);

                    ocnt++;
                    if (ocnt >= threadCount)
                    {
                        PostMainThreadAction<byte[]>(onTrigger, cdata);
                    }
                };

                // 分割文件尺寸，多线程下载
                long[] sizes = SplitFileSize(size, threadCount);
                ConcurrentDictionary<int, byte[]> dataDict = new ConcurrentDictionary<int, byte[]>();
                Parallel.For(0, sizes.Length / 2, i =>
                {
                    long from = sizes[i * 2];
                    long to = sizes[i * 2 + 1];
                    ThreadDownload(i, from, to, t_onDownloading, (index, data) =>
                    {
                        dataDict[index] = data;
                        t_onTrigger(index, data);
                    });
                });
            });
        }

        /// <summary>
        /// 单线程下载
        /// </summary>
        /// <param name="index">线程ID</param>
        /// <param name="from">下载起始位置</param>
        /// <param name="to">下载结束位置</param>
        /// <param name="onDownloading">下载过程回调（线程ID、单次下载数据大小、单次下载数据缓存区、已下载文件数据）</param>
        /// <param name="onTrigger">下载完毕回调（线程ID、下载文件数据）</param>
        private void ThreadDownload(int index, long from, long to, Action<int, long, byte[], byte[]> onDownloading = null, Action<int, byte[]> onTrigger = null)
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(Url));
                request.AddRange(from, to);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream ns = response.GetResponseStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] rbytes = new byte[8 * 1024];
                    int rSize = 0;
                    while (true)
                    {
                        if (!isDownload) return;
                        rSize = ns.Read(rbytes, 0, rbytes.Length);
                        if (rSize <= 0) break;
                        ms.Write(rbytes, 0, rSize);
                        if (onDownloading != null) onDownloading(index, rSize, rbytes, ms.ToArray());
                    }

                    if (ms.Length == (to - from) + 1)
                    {
                        if (onTrigger != null) onTrigger(index, ms.ToArray());
                    }
                    else
                    {
                        lock (errorlock)
                        {
                            if (isDownload)
                            {
                                OnErrorEX(new Exception("文件大小校验不通过"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnErrorEX(ex);
            }
        }

        public void Close()
        {
            isDownload = false;
        }

        /// <summary>
        /// 分割文件大小
        /// </summary>
        /// <returns></returns>
        private long[] SplitFileSize(long size, int count)
        {
            long[] result = new long[count * 2];
            for (int i = 0; i < count; i++)
            {
                long from = (long)Math.Ceiling((double)(size * i / count));
                long to = (long)Math.Ceiling((double)(size * (i + 1) / count)) - 1;
                result[i * 2] = from;
                result[i * 2 + 1] = to;
            }

            return result;
        }

        /// <summary>
        /// 异常通知
        /// </summary>
        /// <param name="ex"></param>
        private void OnErrorEX(Exception ex)
        {
            Close();
            PostMainThreadAction<Exception>(OnError, ex);
        }

        /// <summary>
        /// 通知主线程回调
        /// </summary>
        private void PostMainThreadAction(Action action)
        {
            mainThreadSynContext.Post(new SendOrPostCallback((o) =>
            {
                Action e = o as Action;
                if (e != null) e();
            }), action);
        }

        private void PostMainThreadAction<T>(Action<T> action, T arg1)
        {
            mainThreadSynContext.Post(o => action((T)o), arg1);
        }

        public void PostMainThreadAction<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            mainThreadSynContext.Post(o =>
            {
                var tuple = (Tuple<T1, T2>)o;
                action(tuple.Item1, tuple.Item2);
            }, Tuple.Create(arg1, arg2));
        }

        public void PostMainThreadAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            mainThreadSynContext.Post(o =>
            {
                var tuple = (Tuple<T1, T2, T3>)o;
                action(tuple.Item1, tuple.Item2, tuple.Item3);
            }, Tuple.Create(arg1, arg2, arg3));
        }

        public void PostMainThreadAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            mainThreadSynContext.Post(o =>
            {
                var tuple = (Tuple<T1, T2, T3, T4>)o;
                action(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
            }, Tuple.Create(arg1, arg2, arg3, arg4));
        }
    }
}