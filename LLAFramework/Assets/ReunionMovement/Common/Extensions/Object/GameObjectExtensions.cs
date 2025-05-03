using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

namespace LLAFramework
{
    /// <summary>
    /// 游戏对象扩展
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 设置宽
        /// </summary>
        /// <param name="rectTrans"></param>
        /// <param name="width"></param>
        public static void SetWidth(this RectTransform rectTrans, float width)
        {
            rectTrans.sizeDelta = new Vector2(width, rectTrans.sizeDelta.y);
        }

        /// <summary>
        /// 设置高
        /// </summary>
        /// <param name="rectTrans"></param>
        /// <param name="height"></param>
        public static void SetHeight(this RectTransform rectTrans, float height)
        {
            rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, height);
        }
        /// <summary>
        /// 获取位置的X轴
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newX"></param>
        public static void SetPositionX(this Transform t, float newX)
        {
            t.position = new Vector3(newX, t.position.y, t.position.z);
        }
        /// <summary>
        /// 获取位置的Y轴
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newY"></param>
        public static void SetPositionY(this Transform t, float newY)
        {
            t.position = new Vector3(t.position.x, newY, t.position.z);
        }
        /// <summary>
        /// 获取位置的Z轴
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newZ"></param>
        public static void SetPositionZ(this Transform t, float newZ)
        {
            t.position = new Vector3(t.position.x, t.position.y, newZ);
        }
        /// <summary>
        /// 获取本地位置的X轴
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newX"></param>
        public static void SetLocalPositionX(this Transform t, float newX)
        {
            t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
        }
        /// <summary>
        /// 获取本地位置的Y轴
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newY"></param>
        public static void SetLocalPositionY(this Transform t, float newY)
        {
            t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newZ"></param>
        public static void SetLocalPositionZ(this Transform t, float newZ)
        {
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
        }
        /// <summary>
        /// 设置缩放为0
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newScale"></param>
        public static void SetLocalScale(this Transform t, Vector3 newScale)
        {
            t.localScale = newScale;
        }
        /// <summary>
        /// 设置本地缩放为0
        /// </summary>
        /// <param name="t"></param>
        public static void SetLocalScaleZero(this Transform t)
        {
            t.localScale = Vector3.zero;
        }
        /// <summary>
        /// 获取位置的X轴
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetPositionX(this Transform t)
        {
            return t.position.x;
        }
        /// <summary>
        /// 获取位置的Y轴
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetPositionY(this Transform t)
        {
            return t.position.y;
        }
        /// <summary>
        /// 获取位置的Z轴
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetPositionZ(this Transform t)
        {
            return t.position.z;
        }
        /// <summary>
        /// 获取本地位置的X轴
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetLocalPositionX(this Transform t)
        {
            return t.localPosition.x;
        }
        /// <summary>
        /// 获取本地位置的Y轴
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetLocalPositionY(this Transform t)
        {
            return t.localPosition.y;
        }
        /// <summary>
        /// 获取本地位置的Z轴
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetLocalPositionZ(this Transform t)
        {
            return t.localPosition.z;
        }
        /// <summary>
        /// 判断活动状态
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsActive(this Transform t)
        {
            return t?.gameObject.activeInHierarchy ?? false;
        }
        /// <summary>
        /// 变换转矩阵变换
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static RectTransform RectTransform(this Transform t)
        {
            return t?.gameObject.GetComponent<RectTransform>();
        }
        /// <summary>
        /// 判断刚体是否存在
        /// </summary>
        /// <param name="gobj"></param>
        /// <returns></returns>
        public static bool HasRigidbody(this GameObject gobj)
        {
            return gobj.GetComponent<Rigidbody>() != null;
        }
        /// <summary>
        /// 判断动画是否存在
        /// </summary>
        /// <param name="gobj"></param>
        /// <returns></returns>
        public static bool HasAnimation(this GameObject gobj)
        {
            return gobj.GetComponent<Animation>() != null;
        }
        /// <summary>
        /// 设置动画速度
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="newSpeed"></param>
        public static void SetSpeed(this Animation anim, float newSpeed)
        {
            anim[anim.clip.name].speed = newSpeed;
        }
        /// <summary>
        /// v3转v2
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        /// <summary>
        /// 设置活动状态
        /// </summary>
        /// <param name="com"></param>
        /// <param name="visible"></param>
        public static void SetActive(this Component com, bool visible)
        {
            if (com && com.gameObject && com.gameObject.activeSelf != visible) com.gameObject.SetActive(visible);
        }
        /// <summary>
        /// 设置活动状态（反向）
        /// </summary>
        /// <param name="go"></param>
        /// <param name="visible"></param>
        public static void SetActiveReverse(this GameObject go, bool visible)
        {
            if (go && go.activeSelf != visible) go.SetActive(visible);
        }
        /// <summary>
        /// 设置名字
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        public static void SetName(this GameObject go, string name)
        {
            if (go && go.name != name) go.name = name;
        }

        /// <summary>
        /// 获取附加到给定游戏对象的组件
        /// 如果找不到，则附加一个新的并返回
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>Previously or newly attached component.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary>
        /// 检查游戏对象是否附加了类型为T的组件
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        /// 搜索子物体组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="subnode"></param>
        /// <returns></returns>
        public static T Get<T>(this GameObject go, string subnode) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.transform.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 递归设置游戏对象的层
        /// </summary>
        public static void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                SetLayer(child.gameObject, layer);
            }
        }

        /// <summary> 
        /// 在指定物体上添加指定图片 
        /// </summary>
        public static Image AddImage(this GameObject target, Sprite sprite)
        {
            target.SetActive(false);
            Image image = target.GetComponent<Image>();
            if (!image)
            {
                image = target.AddComponent<Image>();
            }
            image.sprite = sprite;
            image.SetNativeSize();
            target.SetActive(true);
            return image;
        }

        #region 查找子对象
        /// <summary>
        /// 查找子对象
        /// </summary>
        /// <param name="go">自己</param>
        /// <param name="objName">对象名称</param>
        /// <returns></returns>
        public static GameObject Child(this GameObject go, string objName)
        {
            return Child(go.transform, objName);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        /// <param name="go">自己</param>
        /// <param name="objName">对象名称</param>
        /// <returns></returns>
        public static GameObject Child(Transform go, string objName )
        {
            Transform tran = go.Find(objName);
            return tran?.gameObject;
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        /// <param name="go">自己</param>
        /// <param name="objName">对象名</param>
        /// <param name="check_visible">检查可见</param>
        /// <param name="error">错误</param>
        /// <returns></returns>
        public static GameObject Child(this GameObject go, string objName, bool check_visible = false, bool error = true)
        {
            if (!go)
            {
                if (error)
                {
                    Log.Error("查找失败，GameObject是空的！");
                }
                return null;
            }

            var t = Child(go, objName, check_visible, error);
            return t?.gameObject;
        }

        /// <summary>
        /// 查找子对象组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go">自己</param>
        /// <param name="objName">对象名</param>
        /// <param name="check_visible">检查可见</param>
        /// <param name="error">错误</param>
        /// <returns></returns>
        public static T Child<T>(this GameObject go, string objName, bool check_visible = false, bool error = true) where T : Component
        {
            var t = Child(go, objName, check_visible, error);
            return t?.GetComponent<T>();
        }

        /// <summary>
        /// 查找子项组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T FindInChild<T>(this GameObject go, string name = "") where T : Component
        {
            if (!go)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(name) && !go.name.Contains(name))
            {
                return null;
            }

            var comp = go.GetComponent<T>();
            if (comp)
            {
                return comp;
            }

            return go.GetComponentsInChildren<T>().FirstOrDefaultEx();
        }
        #endregion

        #region 取平级对象
        /// <summary>
        /// 取平级对象
        /// </summary>
        /// <param name="go"></param>
        /// <param name="subnode"></param>
        /// <returns></returns>
        public static GameObject Peer(this GameObject go, string subnode)
        {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        /// <param name="go"></param>
        /// <param name="subnode"></param>
        /// <returns></returns>
        public static GameObject Peer(Transform go, string subnode)
        {
            Transform tran = go.parent.Find(subnode);
            return tran?.gameObject;
        }
        #endregion

        #region 清除
        /// <summary>
        /// 清除单个实例，默认延迟为0，仅在场景中删除对应对象
        /// </summary>
        public static void DestroyObject(this UnityEngine.Object obj, float delay = 0)
        {
            GameObject.Destroy(obj, delay);
        }

        /// <summary>
        /// 立刻清理实例对象，会在内存中清理实例，Editor适用
        /// </summary>
        public static void DestroyObjectImmediate(this UnityEngine.Object obj)
        {
            GameObject.DestroyImmediate(obj);
        }

        /// <summary>
        /// 清除一组实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="objs">对象实例集合</param>
        public static void DestroyObjects<T>(IEnumerable<T> objs) where T : UnityEngine.Object
        {
            foreach (var obj in objs)
            {
                GameObject.Destroy(obj);
            }
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        /// <param name="go"></param>
        public static void ClearChild(GameObject go)
        {
            var tran = go.transform;

            while (tran.childCount > 0)
            {
                var child = tran.GetChild(0);

                if (Application.isEditor && !Application.isPlaying)
                {
                    GameObject.DestroyImmediate(child.gameObject);
                }
                else
                {
                    GameObject.Destroy(child.gameObject);
                }
                child.parent = null;
            }
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        /// <param name="go"></param>
        public static void ThisClearChild(this GameObject go)
        {
            ClearChild(go);
        }
        #endregion
    }
}