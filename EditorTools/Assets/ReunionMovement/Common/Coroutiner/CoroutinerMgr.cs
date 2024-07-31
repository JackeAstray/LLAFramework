using GameLogic.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoroutinerPool<T> where T : class, new()
    {
        private readonly Stack<T> poolStack = new Stack<T>();
        private readonly object lockObj = new object();
        private readonly int maxCapacity;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxCapacity"></param>
        public CoroutinerPool(int maxCapacity)
        {
            this.maxCapacity = maxCapacity;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            lock (lockObj)
            {
                if (poolStack.Count > 0)
                {
                    return poolStack.Pop();
                }
                else
                {
                    return new T();
                }
            }
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="obj"></param>
        public void Release(T obj)
        {
            lock (lockObj)
            {
                if (poolStack.Count < maxCapacity)
                {
                    poolStack.Push(obj);
                }
            }
        }
    }

    /// <summary>
    /// 协程管理器
    /// </summary>
    public class CoroutinerMgr : SingleToneMgr<CoroutinerMgr>
    {
        public long taskIndex = 0;
        private readonly object taskLock = new object();
        private readonly CoroutinerPool<CoroutineTask> taskPool = new CoroutinerPool<CoroutineTask>(100);
        /// <summary>
        /// 协程任务
        /// </summary>
        public class CoroutineTask : IReference
        {
            public IEnumerator Enumerator;
            public Action<Coroutine> Callback;
            public long TaskId;

            public void Clear()
            {
                TaskId = -1;
                Enumerator = null;
                Callback = null;
            }
        }

        // 协程任务
        private List<CoroutineTask> coroutineTasks = new List<CoroutineTask>();
        private List<CoroutineTask> tasksToProcess = new List<CoroutineTask>();

        void Update()
        {
            // 更新事件
            RefreshCoroutineTasks();
        }

        /// <summary>
        /// 添加协程
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="callback"></param>
        public void AddCoroutine(IEnumerator routine, Action<Coroutine> callback)
        {
            var task = taskPool.Get();
            task.Enumerator = routine;
            task.Callback = callback;
            task.TaskId = Interlocked.Increment(ref taskIndex);

            lock (taskLock)
            {
                coroutineTasks.Add(task);
            }
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(Action handler)
        {
            return StartCoroutine(EnumCoroutine(handler));
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(Action handler, Action callback)
        {
            return StartCoroutine(EnumCoroutine(handler, callback));
        }

        /// <summary>
        /// 停止所有协程
        /// </summary>
        public new void StopAllCoroutines()
        {
            base.StopAllCoroutines();
        }

        /// <summary>
        /// 停止目标协程
        /// </summary>
        /// <param name="enumerator"></param>
        public void StopTargetCoroutine(IEnumerator enumerator)
        {
            base.StopCoroutine(enumerator);
        }

        /// <summary>
        /// 停止目标协程
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopTargetCoroutine(Coroutine coroutine)
        {
            base.StopCoroutine(coroutine);
        }

        /// <summary>
        /// 枚举协程
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator EnumCoroutine(Coroutine routine, Action callBack)
        {
            yield return routine;
            callBack?.Invoke();
        }

        /// <summary>
        /// 枚举协程
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        IEnumerator EnumCoroutine(Action handler)
        {
            handler?.Invoke();
            yield return null;
        }

        /// <summary>
        /// 枚举协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callack"></param>
        /// <returns></returns>
        IEnumerator EnumCoroutine(Action handler, Action callack)
        {
            yield return StartCoroutine(handler);
            callack?.Invoke();
        }

        /// <summary>
        /// 枚举断言协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator EnumPredicateCoroutine(Func<bool> handler, Action callBack)
        {
            yield return new WaitUntil(handler);
            callBack();
        }

        /// <summary>
        /// 协程任务
        /// </summary>
        void RefreshCoroutineTasks()
        {
            lock (taskLock)
            {
                tasksToProcess.AddRange(coroutineTasks);
                coroutineTasks.Clear();
            }

            // 处理任务
            foreach (var task in tasksToProcess)
            {
                try
                {
                    // 启动协程
                    var coroutine = StartCoroutine(task.Enumerator);
                    task.Callback?.Invoke(coroutine);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Coroutine task {task.TaskId} threw an exception: {ex}");
                }
                finally
                {
                    // 清理任务
                    task.Clear();
                    taskPool.Release(task);
                }
            }

            tasksToProcess.Clear();
        }

        /// <summary>
        /// 取消协程任务
        /// </summary>
        /// <param name="taskId"></param>
        public void CancelCoroutineTask(long taskId)
        {
            lock (taskLock)
            {
                var task = coroutineTasks.Find(t => t.TaskId == taskId);
                if (task != null)
                {
                    coroutineTasks.Remove(task);
                    task.Clear();
                    taskPool.Release(task);
                }
            }
        }
    }
}