using GameLogic.Base;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    public class TaskMgr : SingleToneMgr<TaskMgr>
    {
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callback"></param>
        public async void StartTask(Action action, Action callback)
        {
            await RunTask(action, callback);
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="callback"></param>
        public async void StartTask<T>(Func<T> func, Action callback)
        {
            await RunTask(func, callback);
        }

        /// <summary>
        /// 激活任务
        /// </summary>
        /// <param name="action">执行方法</param>
        /// <param name="callback">任务回调</param>
        /// <param name="cancellationToken">任务取消</param>
        /// <param name="timeout">任务超时</param>
        /// <returns></returns>
        public async Task RunTask(Action action, Action callback, CancellationToken cancellationToken = default, TimeSpan? timeout = null)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    action?.Invoke();
                }, cancellationToken);

                if (timeout.HasValue)
                {
                    if (await Task.WhenAny(task, Task.Delay(timeout.Value, cancellationToken)) == task)
                    {
                        await task; // Propagate exceptions
                    }
                    else
                    {
                        throw new TimeoutException("The task has timed out.");
                    }
                }
                else
                {
                    await task;
                }

                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Task failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 激活任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">执行方法</param>
        /// <param name="callback">任务回调</param>
        /// <param name="cancellationToken">任务取消</param>
        /// <param name="timeout">任务超时</param>
        /// <returns></returns>
        public async Task<T> RunTask<T>(Func<T> func, Action callback, CancellationToken cancellationToken = default, TimeSpan? timeout = null)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return func != null ? func() : default(T);
                }, cancellationToken);

                T result;
                if (timeout.HasValue)
                {
                    if (await Task.WhenAny(task, Task.Delay(timeout.Value, cancellationToken)) == task)
                    {
                        result = await task; // Propagate exceptions
                    }
                    else
                    {
                        throw new TimeoutException("The task has timed out.");
                    }
                }
                else
                {
                    result = await task;
                }

                callback?.Invoke();
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Task failed: {ex.Message}");
                return default(T);
            }
        }
    }
}