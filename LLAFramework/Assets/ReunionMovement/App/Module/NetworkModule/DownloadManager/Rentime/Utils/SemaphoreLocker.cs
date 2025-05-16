using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LLAFramework.Download
{
    /// <summary>
    /// 信号锁
    /// </summary>
    public class SemaphoreLocker
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 异步执行带锁的操作
        /// </summary>
        /// <param name="worker">需要加锁执行的异步方法</param>
        public async Task LockAsync(Func<Task> worker)
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                await worker().ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// 异步执行带锁的操作并返回结果
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="worker">需要加锁执行的异步方法</param>
        /// <returns>异步操作结果</returns>
        public async Task<T> LockAsync<T>(Func<Task<T>> worker)
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                return await worker().ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// 同步执行带锁的操作
        /// </summary>
        /// <param name="worker">需要加锁执行的方法</param>
        public void Lock(Action worker)
        {
            semaphore.Wait();
            try
            {
                worker();
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// 同步执行带锁的操作并返回结果
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="worker">需要加锁执行的方法</param>
        /// <returns>操作结果</returns>
        public T Lock<T>(Func<T> worker)
        {
            semaphore.Wait();
            try
            {
                return worker();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
