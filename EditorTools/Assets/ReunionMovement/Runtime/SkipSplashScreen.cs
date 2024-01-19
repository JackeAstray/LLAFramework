#if !UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

/// <summary>
/// 运行时代码，用于跳过Unity的启动画面
/// <summary>

namespace GameLogic
{
    [Preserve]
    public class SkipSplashScreen
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
#if UNITY_WEBGL
            Application.focusChanged += Application_focusChanged;
#else
            System.Threading.Tasks.Task.Run(AsyncSkip);
#endif
        }
#if UNITY_WEBGL
        private static void Application_focusChanged(bool obj)
        {
            Application.focusChanged -= Application_focusChanged;
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
#else
        private static void AsyncSkip()
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
#endif
    }
}
#endif