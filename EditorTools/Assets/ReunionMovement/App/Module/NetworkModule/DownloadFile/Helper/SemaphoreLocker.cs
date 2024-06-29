using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameLogic.Download
{
    /// <summary>
    /// 用于在等待时处理异步锁定
    /// </summary>
    public class SemaphoreLocker
    {
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 用于异步锁定
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task LockAsync(Func<Task> worker, object args = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                await worker();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 用于异步锁定(具有返回类型（泛型T）的非void方法的重载变体)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="worker"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> LockAsync<T>(Func<Task<T>> worker, object args = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await worker();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}