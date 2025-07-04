using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Globalization;
using UnityEngine.Networking;

namespace LLAFramework
{
    /// <summary>
    /// 引擎扩展
    /// </summary>
    public static class EngineExtensions
    {
        #region Application
        public static bool IsDebug()
        {
            if (Application.isEditor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }


        /// <summary>
        /// 退出
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

        #region object转换
        /// <summary>
        /// 从对象数组中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openArgs"></param>
        /// <param name="offset">越界</param>
        /// <param name="isLog">开启log</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T Get<T>(this object[] openArgs, int offset, bool isLog = true)
        {
            if (offset < 0 || offset >= openArgs.Length)
            {
                if (isLog)
                {
                    Log.Error($"[获取错误<object[]>],  越界: {offset}  {openArgs.Length}");
                }
                return default(T);
            }

            var arrElement = openArgs[offset];
            if (arrElement == null)
            {
                return default(T);
            }

            try
            {
                return (T)arrElement;
            }
            catch (InvalidCastException)
            {
                try
                {
                    return (T)Convert.ChangeType(arrElement, typeof(T));
                }
                catch (Exception ex)
                {
                    if (isLog)
                    {
                        Log.Error($"[获取错误<object[]>],  '{arrElement}' 无法转换为类型<{typeof(T)}>: {ex}");
                    }
                    return default(T);
                }
            }
        }
        /// <summary>
        /// object转int32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ObjToInt32(this object obj)
        {
            if (obj is int i)
            {
                return i;
            }

            try
            {
                return Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                Log.Error("ToInt32 : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// object转int64 | long
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ObjToInt64(this object obj)
        {
            if (obj is long l)
            {
                return l;
            }

            try
            {
                return Convert.ToInt64(obj);
            }
            catch (Exception ex)
            {
                Log.Error("ToInt64 : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// object转float
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float ObjToFloat(this object obj)
        {
            if (obj is float f)
            {
                return f;
            }

            try
            {
                return (float)Math.Round(Convert.ToSingle(obj), 2);
            }
            catch (Exception ex)
            {
                Log.Error("object转float失败 : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// object转string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToString(this object obj)
        {
            if (obj is string s)
            {
                return s;
            }

            try
            {
                return Convert.ToString(obj);
            }
            catch (Exception ex)
            {
                Log.Error("object转string失败 : " + ex);
                return null;
            }
        }
        #endregion



        #region 分辨率
        /// <summary>
        /// 设置屏幕分辨率
        /// </summary>
        /// <param name="width">屏幕宽度</param>
        /// <param name="height">屏幕高度</param>
        /// <param name="fullScreen">是否全屏显示</param>
        public static void SetScreen(int width, int height, bool fullScreen)
        {
            Screen.SetResolution(width, height, fullScreen);
        }
        #endregion
    }
}