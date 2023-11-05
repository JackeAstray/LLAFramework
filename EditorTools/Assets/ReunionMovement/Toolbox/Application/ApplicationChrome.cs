using System;
using System.Collections.Generic;
using UnityEngine;

class ApplicationChrome
{
    private static AndroidJavaObject activity;

    private static AndroidJavaObject Activity
    {
        get
        {
            if (activity == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return activity;
        }
    }

    private static AndroidJavaObject Window
    {
        get
        {
            return Activity.Call<AndroidJavaObject>("getWindow");
        }
    }

    private static AndroidJavaObject View
    {
        get
        {
            return Window.Call<AndroidJavaObject>("getDecorView");
        }
    }
    /// <summary>
    /// 测试
    /// </summary>
    public static void TestCall()
    {
        if (Activity != null)
        {
            Debug.Log("Activity 不为空");
        }
        else
        {
            Debug.Log("Activity 为空");
        }


        if (Window != null)
        {
            Debug.Log("Window 不为空");
        }
        else
        {
            Debug.Log("Window 为空");
        }


        if (View != null)
        {
            Debug.Log("View 不为空");
        }
        else
        {
            Debug.Log("View 为空");
        }
    }
    /// <summary>
    /// 设置是否显示状态栏
    /// </summary>
    /// <param name="isShow"></param>
    public static void SetStatusBar(bool isShow)
    {
        if (isShow)
        {
            RunOnAndroidUiThread(()=> 
            {
                if (Window != null)
                {
                    Window.Call("clearFlags", 1024);
                }
            });
        }
        else
        {
            RunOnAndroidUiThread(() =>
            {
                if (Window != null)
                {
                    Window.Call("addFlags", 1024);
                }
            });
        }
    }

    /// <summary>
    /// 设置状态栏颜色&状态栏字体颜色
    /// </summary>
    /// <param name="color"></param>
    /// <param name="isBlack"></param>
    public static void SetStatusBarColor(int color, bool isBlack)
    {
        if (Window != null)
        {
            RunOnAndroidUiThread(() =>
            {
                if (Window != null)
                {
                    Window.Call("setStatusBarColor", color);
                }
            });
        }

        if (isBlack)
        {
            RunOnAndroidUiThread(() =>
            {
                if (View != null)
                {
                    View.Call("setSystemUiVisibility", 8192);
                }
            });
        }
        else
        {
            RunOnAndroidUiThread(() =>
            {
                if (View != null)
                {
                    View.Call("setSystemUiVisibility", 256);
                }
            });
        }
    }

    /// <summary>
    /// 运行
    /// </summary>
    /// <param name="target"></param>
    private static void RunOnAndroidUiThread(Action target)
    {
        AndroidJavaObject activity = Activity;
        if (activity != null)
        {
            activity.Call("runOnUiThread", new AndroidJavaRunnable(target));
        }
    }

    /// <summary>
    /// 将颜色转为int
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static int ConvertColorToAndroidColor(Color color)
    {
        Color32 color32 = color;
        int alpha = color32.a;
        int red = color32.r;
        int green = color32.g;
        int blue = color32.b;
        using (var colorClass = new AndroidJavaClass("android.graphics.Color"))
        {
            int androidColor = colorClass.CallStatic<int>("argb", alpha, red, green, blue);
            return androidColor;
        }
    }
}