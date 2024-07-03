#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameLogic.AssetsModule
{
    /// <summary>
    /// 在编辑器中运行协程函数的实用程序
    /// </summary>
    public class EditorCoroutine
    {
        private readonly Stack<IEnumerator> coroutineList = new Stack<IEnumerator>();

        /// <summary>
        ///     Start running a coroutine
        /// </summary>
        public static void Start(IEnumerator routine)
        {
            new EditorCoroutine(routine).Start();
        }

        private EditorCoroutine(IEnumerator coroutine)
        {
            coroutineList.Push(coroutine);
        }

        private void Start()
        {
            EditorApplication.update += Update;
        }

        private void Stop()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            try
            {
                var current = coroutineList.Peek();

                if (current.MoveNext() == false)
                {
                    // 列表底部的协程已完成
                    coroutineList.Pop();
                }
                else if (current.Current is IEnumerator)
                {
                    // Coroutine 正在调用一个协程，将其添加到列表中并开始监视该协程直到它完成。
                    coroutineList.Push((IEnumerator)current.Current);
                }
                else if (current.Current is AsyncOperation)
                {
                    var tmp = (AsyncOperation)current.Current;
                    coroutineList.Push(new AsyncToCustomYield(tmp));
                }

                // 如果没有更多协程则停止监听更新事件
                if (coroutineList.Count == 0)
                {
                    Stop();
                }
            }
            catch (Exception)
            {
                // 如果出现任何类型的错误，为了简单起见，我们将终止整个堆栈。
                coroutineList.Clear();
                Stop();
                throw;
            }
        }

        internal class AsyncToCustomYield : CustomYieldInstruction
        {
            private AsyncOperation async;

            public AsyncToCustomYield(AsyncOperation async)
            {
                this.async = async;
            }

            public override bool keepWaiting
            {
                get { return async.isDone == false; }
            }
        }
    }
}
#endif