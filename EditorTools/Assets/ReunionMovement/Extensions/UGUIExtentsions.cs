using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace GameLogic
{
    public static class UGUIExtentsions
    {
        #region 根据对象的扩展
        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="textStr">文本内容</param>
        /// <returns></returns>
        public static Text SetText(this GameObject obj, string name, string textStr)
        {
            var text = obj.Get<Text>(name);
            text.text = textStr;
            return text;
        }
        /// <summary>
        /// 设置按钮点击回调
        /// </summary>
        /// <param name="onClick">点击回调</param>
        /// <returns></returns>
        public static Button SetButton(this GameObject obj, string name, Action<GameObject> onClick)
        {
            var btn = obj.Get<Button>(name);
            btn.onClick.AddListener(() =>
            {
                if (null != onClick)
                {
                    onClick(btn.gameObject);
                }
            });
            return btn;
        }

        /// <summary>
        /// 设置输入框文本输入完毕回调
        /// </summary>
        /// <param name="onEndEdit">文本输入完毕后回调</param>
        /// <returns></returns>
        public static InputField SetInputField(this GameObject obj, string name, Action<string> onEndEdit)
        {
            var input = obj.Get<InputField>(name);
            input.onEndEdit.AddListener((v) =>
            {
                if (null != onEndEdit)
                {
                    onEndEdit(v);
                }
            });
            return input;
        }

        /// <summary>
        /// 设置下拉框选择回调
        /// </summary>
        /// <param name="onValueChanged">选择回调</param>
        /// <returns></returns>
        public static Dropdown SetDropDown(this GameObject obj, string name, Action<int> onValueChanged)
        {
            var dropdown = obj.Get<Dropdown>(name);
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener((v) =>
            {
                if (null != onValueChanged)
                    onValueChanged(v);
            });

            return dropdown;
        }

        /// <summary>
        /// 设置单选框选择回调
        /// </summary>
        /// <param name="onValueChanged">选择回调</param>
        /// <returns></returns>
        public static Toggle SetToggle(this GameObject obj, string name, Action<bool> onValueChanged)
        {
            var toogle = obj.Get<Toggle>(name);
            toogle.onValueChanged.RemoveAllListeners();
            toogle.onValueChanged.AddListener((v) =>
            {
                if (null != onValueChanged)
                    onValueChanged(v);
            });

            return toogle;
        }

        /// <summary>
        /// 设置单选框选择回调
        /// </summary>
        /// <param name="onValueChanged">选择回调</param>
        /// <returns></returns>
        public static Slider SetSlider(this GameObject obj, string name, Action<float> onValueChanged)
        {
            var slider = obj.Get<Slider>(name);
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((v) =>
            {
                if (null != onValueChanged)
                    onValueChanged(v);
            });

            return slider;
        }
        #endregion
    }
}
