using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LLAFramework.AnimationUI
{
    /// <summary>
    /// UI波纹动画
    /// </summary>
    [RequireComponent(typeof(Mask))]
    [RequireComponent(typeof(Image))]
    public class UIRipple : MonoBehaviour
    {

        /// <summary> 
        /// 将渲染的精灵
        /// </summary>
        public Sprite ShapeSprite;

        /// <summary> 
        /// 纹波增长的速度
        /// </summary>
        [Range(0.25f, 5f)]
        public float Speed = 1f;

        /// <summary> 
        /// 如果为 true，MaxSize 将自动设置
        /// </summary>
        public bool AutomaticMaxSize = true;

        /// <summary> 
        /// 波纹的最大尺寸
        /// </summary>
        public float MaxSize = 4f;

        /// <summary> 
        /// 波纹起始颜色
        /// </summary>
        public Color StartColor = new Color(1f, 1f, 1f, 1f);

        /// <summary> 
        /// 波纹结束颜色
        /// </summary>
        public Color EndColor = new Color(1f, 1f, 1f, 1f);

        /// <summary> 
        /// 如果为 true，则仅当您单击 UI 元素时才会出现波纹
        /// </summary>
        public bool OnUIOnly = true;

        /// <summary> 
        /// 如果 true 波纹将出现在 UI 元素中所有其他子项的顶部 
        /// </summary>
        public bool RenderOnTop = false;

        /// <summary> 
        /// 如果为 true，波纹将从 UI 元素的中心开始
        /// </summary>
        public bool StartAtCenter = false;

        void Awake()
        {
            //根据需要自动设置 MaxSize
            if (AutomaticMaxSize)
            {
                RectTransform RT = gameObject.transform as RectTransform;
                MaxSize = (RT.rect.width > RT.rect.height) ? 4f * ((float)Mathf.Abs(RT.rect.width) / (float)Mathf.Abs(RT.rect.height)) : 4f * ((float)Mathf.Abs(RT.rect.height) / (float)Mathf.Abs(RT.rect.width));

                if (float.IsNaN(MaxSize))
                {
                    MaxSize = (transform.localScale.x > transform.localScale.y) ? 4f * transform.localScale.x : 4f * transform.localScale.y;
                }
            }

            MaxSize = Mathf.Clamp(MaxSize, 0.5f, 1000f);
        }

        void Update()
        {
            // 检测鼠标左键的点击或者手机屏幕的触摸
            if ((Input.GetMouseButtonDown(0) ||
                (Input.touchCount > 0 &&
                Input.GetTouch(0).phase == TouchPhase.Began)) &&
                (!OnUIOnly || IsOnUIElement(Input.mousePosition)))
            {
                //创建波纹
                CreateRipple(Input.mousePosition);
            }
        }

        /// <summary>
        /// 创建波纹
        /// </summary>
        /// <param name="Position"></param>
        public void CreateRipple(Vector2 Position)
        {
            //创建游戏对象并添加组件
            var ThisRipple = new GameObject();
            ThisRipple.AddComponent<Ripple>();
            ThisRipple.AddComponent<Image>();
            ThisRipple.GetComponent<Image>().sprite = ShapeSprite;
            ThisRipple.name = "Ripple";

            //设置父对象
            ThisRipple.transform.SetParent(gameObject.transform);

            //如果需要，重新排列子对象
            if (!RenderOnTop)
            { ThisRipple.transform.SetAsFirstSibling(); }

            //将波纹设置在正确的位置
            if (StartAtCenter)
            { ThisRipple.transform.localPosition = new Vector2(0f, 0f); }
            else
            { ThisRipple.transform.position = Position; }

            //在Ripple中设置参数
            var ripple = ThisRipple.GetComponent<Ripple>();
            ripple.Speed = Speed;
            ripple.MaxSize = MaxSize;
            ripple.StartColor = StartColor;
            ripple.EndColor = EndColor;
        }


        /// <summary>
        /// 是在UI元素上
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public bool IsOnUIElement(Vector2 Position)
        {
            //如果该点位于 UIElement 的范围内
            return gameObject.transform.position.x + (GetComponent<RectTransform>().rect.width / 2f) >= Position.x
                && gameObject.transform.position.x - (GetComponent<RectTransform>().rect.width / 2f) <= Position.x
                && gameObject.transform.position.y + (GetComponent<RectTransform>().rect.height / 2f) >= Position.y
                && gameObject.transform.position.y - (GetComponent<RectTransform>().rect.height / 2f) <= Position.y;
        }
    }
}