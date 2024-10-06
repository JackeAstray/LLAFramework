using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    /// <summary>
    /// 树节点数据
    /// </summary>
    public class UITreeData
    {
        #region 属性
        public UITreeData parent;
        public List<UITreeData> childNodes;
        public int layer = 0;
        public string name = string.Empty;

        public UITreeData() { }
        public UITreeData(string name, int layer = 0)
        {
            this.name = name;
            this.layer = layer;
            parent = null;
            childNodes = new List<UITreeData>();
        }
        public UITreeData(string name, List<UITreeData> childNodes, int layer = 0)
        {
            this.name = name;
            parent = null;
            this.childNodes = childNodes;
            if (null == this.childNodes)
                this.childNodes = new List<UITreeData>();
            this.layer = layer;
            ResetChildren(this);
        }

        #endregion

        #region 设置
        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(UITreeData parent)
        {
            if (null != this.parent)
                this.parent.RemoveChild(this);
            this.parent = parent;
            this.layer = parent.layer + 1;
            parent.childNodes.Add(this);
            ResetChildren(this);
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(UITreeData child)
        {
            AddChild(new UITreeData[] { child });
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="children"></param>
        public void AddChild(IEnumerable<UITreeData> children)
        {
            foreach (UITreeData child in children)
                child.SetParent(this);
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(UITreeData child)
        {
            RemoveChild(new UITreeData[] { child });
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="children"></param>
        public void RemoveChild(IEnumerable<UITreeData> children)
        {
            foreach (UITreeData child in children)
            {
                for (int i = 0; i < childNodes.Count; i++)
                    if (child == childNodes[i])
                    {
                        childNodes.Remove(childNodes[i]);
                        break;
                    }
            }
        }

        /// <summary>
        /// 清空子节点
        /// </summary>
        public void ClearChildren()
        {
            childNodes = null;
        }

        /// <summary>
        /// 重置子节点的父节点和层级
        /// </summary>
        /// <param name="treeData"></param>
        private void ResetChildren(UITreeData treeData)
        {
            for (int i = 0; i < treeData.childNodes.Count; i++)
            {
                UITreeData node = treeData.childNodes[i];
                node.parent = treeData;
                node.layer = treeData.layer + 1;
                ResetChildren(node);
            }
        }

        #endregion

        #region 重写
        /// <summary>
        /// 重写Equals方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            UITreeData other = obj as UITreeData;
            if (null == other) return false;
            return other.name.Equals(name) && other.layer.Equals(layer);
        }

        /// <summary>
        /// 重写GetHashCode方法
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (parent != null ? parent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (childNodes != null ? childNodes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ layer;
                hashCode = (hashCode * 397) ^ (name != null ? name.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
