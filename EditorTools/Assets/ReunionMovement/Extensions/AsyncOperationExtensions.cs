using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public static class AsyncOperationExtensions
    {
        public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation request) => new AsyncOperationAwaiter(request);
    }
}