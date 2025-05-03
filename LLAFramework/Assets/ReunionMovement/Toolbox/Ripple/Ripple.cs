using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LLAFramework.AnimationUI
{
    /// <summary>
    /// UI波纹动画
    /// </summary>

    public class Ripple : MonoBehaviour
    {
        //波纹参数
        public float Speed;
        public float MaxSize;
        public Color StartColor;
        public Color EndColor;

        void Start()
        {
            //设置尺寸和颜色
            transform.localScale = new Vector3(0f, 0f, 0f);
            GetComponent<Image>().color = new Color(StartColor.r, StartColor.g, StartColor.b, 1f);
        }

        void Update()
        {
            //调整比例和颜色
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(MaxSize, MaxSize, MaxSize), Time.deltaTime * Speed);
            GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, new Color(EndColor.r, EndColor.g, EndColor.b, 0f), Time.deltaTime * Speed);

            //在生命的尽头摧毁
            if (transform.localScale.x >= MaxSize * 0.995f)
            {
                Destroy(gameObject);
            }
        }
    }
}