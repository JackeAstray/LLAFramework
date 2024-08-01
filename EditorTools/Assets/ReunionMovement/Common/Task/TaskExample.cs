using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    public class TaskExample : MonoBehaviour
    {
        private void Start()
        {
            Invoke("Init", 3);
        }

        public void Update()
        {
        }

        public void Init()
        {
            // 示例1：启动一个简单的任务
            TaskMgr.Instance.StartTask(() =>
            {
                Debug.Log("任务1正在运行...");
                Task.Delay(1000).Wait(); // 模拟一些工作
                Debug.Log("任务1完成");
            }, () =>
            {
                Console.WriteLine("任务1的回调");
            });

            // 示例2：启动一个带返回值的任务

            try
            {
                TaskMgr.Instance.StartTask(() =>
                {
                    Debug.Log("任务2正在运行...");
                    Task.Delay(4000).Wait(); // 模拟一些工作
                    Debug.Log("任务2完成");
                    return 256; // 返回一个值
                }, (value) =>
                {
                    Debug.Log("任务2的回调:" + value);
                },
                TimeSpan.FromSeconds(2));
            }
            catch (OperationCanceledException)
            {
                Debug.Log("任务2取消");
            }
            catch (TimeoutException ex)
            {
                Debug.Log($"任务2超时: {ex.Message}");
            }

            // 示例3：启动一个带超时的任务
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(20)); // 设置超时时间为3秒

                TaskMgr.Instance.StartTask(() =>
                {
                    Debug.Log("任务3正在运行...");
                    Task.Delay(5000).Wait(); // 模拟一些工作
                    Debug.Log("任务3完成");
                    return 512; // 返回一个值
                },
                (value) =>
                {
                    Debug.Log("任务3的回调" + value);
                },
                TimeSpan.FromSeconds(20),
                cts.Token);

                cts.Cancel();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("任务3取消");
            }
            catch (TimeoutException ex)
            {
                Debug.Log($"任务3超时: {ex.Message}");
            }
        }
    }
}