﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LLAFramework
{
    /// <summary>
    /// 树节点
    /// </summary>
    public class TreeViewNode : UIBehaviour
    {
        #region 属性
        public int layer = 0;
        public Sprite transparent;
        public Sprite arrow;
        private TreeViewData treeData = null;
        private TreeView uiTree = null;
        private Toggle toggle = null;
        private Image icon = null;
        private Image bg = null;
        private Text text = null;
        private Transform toggleTransform = null;
        private Transform myTransform = null;
        private Transform container = null;
        private List<GameObject> children = new List<GameObject>();

        private Action action = null;
        private bool enableAction = false;
        #endregion

        #region 方法
        /// <summary>
        /// 获取组件
        /// </summary>
        private void GetComponent()
        {
            myTransform = this.transform;
            bg = myTransform.GetComponent<Image>();
            container = myTransform.Find("Container");
            toggle = container.Find("Toggle").GetComponent<Toggle>();
            //icon = container.Find("Toggle/Icon").GetComponent<Image>();
            text = container.Find("Toggle/Text").GetComponent<Text>();
            toggleTransform = toggle.transform.Find("Image");
            icon = toggle.transform.Find("Image").GetComponent<Image>();
            uiTree = myTransform.parent.parent.parent.GetComponent<TreeView>();
        }
        /// <summary>
        /// 重置组件
        /// </summary>
        private void ResetComponent()
        {
            container.localPosition = new Vector3(0, container.localPosition.y, 0);
            toggleTransform.localEulerAngles = new Vector3(0, 0, 90);
            //toggleTransform.gameObject.SetActive(true);
            icon.sprite = arrow;
        }
        #endregion

        #region 外部调用接口
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="data"></param>
        public void Insert(TreeViewData data)
        {
            if (myTransform == null)
            {
                GetComponent();
            }
            ResetComponent();
            treeData = data;
            text.text = data.name;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OpenOrClose);
            container.localPosition += new Vector3(container.GetComponent<RectTransform>().sizeDelta.y * treeData.layer, 0, 0);
            if (data.childNodes.Count.Equals(0))
            {
                icon.sprite = transparent;
                //toggleTransform.gameObject.SetActive(false);
                //icon.sprite = uiTree.lastLayerIcon;
            }
            else
            {
                //icon.sprite = toggle.isOn ? uiTree.openIcon : uiTree.closeIcon;
            }

            enableAction = data.enableAction;
            action = data.action;

            SetColor(data.layer);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        public void SetData(TreeViewData data)
        {
            //if (null == myTransform)
            //{
            //    GetComponent();
            //}
            //ResetComponent();
            //treeData = data;
            //text.text = data.name;
            //toggle.isOn = false;
            //toggle.onValueChanged.AddListener(OpenOrClose);
            //container.localPosition += new Vector3(container.GetComponent<RectTransform>().sizeDelta.y * treeData.layer, 0, 0);
            //if (data.childNodes.Count.Equals(0))
            //{
            //    icon.sprite = transparent;
            //    //toggleTransform.gameObject.SetActive(false);
            //    //icon.sprite = uiTree.lastLayerIcon;
            //}
            //else
            //{
            //    //icon.sprite = toggle.isOn ? uiTree.openIcon : uiTree.closeIcon;
            //}

            //action = data.action;

            //SetColor(data.layer);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="layer"></param>
        public void SetColor(int layer)
        {
            this.layer = layer;

            if (layer < uiTree.colors.Count)
            {
                bg.color = uiTree.colors[layer];
            }
        }

        public void UpdateDisplayDecorate(bool displayDecorate)
        {
            // 根据 displayDecorate 的值更新节点的显示
            if (displayDecorate)
            {
                // 显示装饰
            }
            else
            {
                // 隐藏装饰
            }
        }

        public TreeViewData GetTreeData()
        {
            return treeData;
        }
        #endregion

        #region 折叠 展开
        /// <summary>
        /// 折叠或展开
        /// </summary>
        /// <param name="isOn"></param>
        private void OpenOrClose(bool isOn)
        {
            if (isOn) OpenChildren();
            else CloseChildren();
            toggleTransform.localEulerAngles = isOn ? new Vector3(0, 0, 0) : new Vector3(0, 0, 90);
            //icon.sprite = toggle.isOn ? uiTree.openIcon : uiTree.closeIcon;

            if (enableAction)
            {
                action?.Invoke();
            }
        }
        /// <summary>
        /// 展开子节点
        /// </summary>
        private void OpenChildren()
        {
            if (!enableAction)
            {
                action?.Invoke();
            }

            children = uiTree.Pop(treeData.childNodes, transform.GetSiblingIndex());
        }
        /// <summary>
        /// 关闭子节点
        /// </summary>
        protected void CloseChildren()
        {
            for (int i = 0; i < children.Count; i++)
            {
                TreeViewNode node = children[i].GetComponent<TreeViewNode>();
                node.RemoveListener();
                node.CloseChildren();
            }
            uiTree.Push(children);
            children = new List<GameObject>();
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        private void RemoveListener()
        {
            toggle.onValueChanged.RemoveListener(OpenOrClose);
        }
        #endregion
    }
}