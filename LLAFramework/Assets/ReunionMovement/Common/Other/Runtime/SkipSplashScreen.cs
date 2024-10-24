using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class SkipSplashScreen
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Run()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }
        else
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
            });
        }
    }
}