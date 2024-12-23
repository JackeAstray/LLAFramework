using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

namespace GameLogic
{
    /// <summary>
    /// 树形视图
    /// </summary>
    public class TreeView : UIBehaviour
    {
        #region 属性
        // 打开图标
        public Sprite openIcon = null;
        // 关闭图标
        public Sprite closeIcon = null;
        // 最后一层图标
        public Sprite lastLayerIcon = null;

        public List<Color> colors = new List<Color>();

        // 预制体
        public TreeViewNode tvObj = null;
        // 跟节点
        public List<TreeViewNode> treeRootNodes = null;
        private Transform container = null;
        private GameObject nodePrefab = null;
        public GameObject NodePrefab
        {
            get { return nodePrefab ?? (nodePrefab = container.GetChild(0).gameObject); }
            set { nodePrefab = value; }
        }
        #endregion

        #region external call interface
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="rootData"></param>
        public void Insert(List<TreeViewData> rootData)
        {
            if (null == container)
            {
                GetComponent();
            }

            foreach (var item in rootData)
            {
                TreeViewNode treeView = Instantiate<TreeViewNode>(tvObj, container);
                treeView.Insert(item);
                treeRootNodes.Add(treeView);
            }
        }

        //// insert new node method. The next version will add this funcion.
        //[Obsolete("Next version will add this funcion")]
        //public void Inject(UITreeData insertData, UITreeData parentData)
        //{

        //}

        //[Obsolete("This method is replaced by Inject.")]
        public void SetData(List<TreeViewData> rootData)
        {
            //if (null == container)
            //{
            //    GetComponent();
            //}

            //foreach (var item in rootData)
            //{
            //    TreeViewNode treeView = Instantiate<TreeViewNode>(tvObj, container);
            //    treeView.Insert(item);
            //    treeRootNodes.Add(treeView);
            //}
        }

        #endregion

        #region 获取组件

        private void GetComponent()
        {
            container = transform.Find("Viewport/Content");
            //if (container.childCount.Equals(0))
            //{
            //    throw new Exception("UITreeNode Template can not be null! Create a Template!");
            //}
            //treeRootNode = container.GetChild(0).GetComponent<TreeViewNode>();
        }

        #endregion

        #region cache pool functions

        private readonly List<GameObject> pool = new List<GameObject>();
        private Transform poolParent = null;

        public List<GameObject> Pop(List<TreeViewData> datas, int siblingIndex)
        {
            List<GameObject> result = new List<GameObject>();
            for (int i = datas.Count - 1; i >= 0; i--)
            {
                result.Add(Pop(datas[i], siblingIndex));
            }
            return result;
        }
        public GameObject Pop(TreeViewData data, int siblingIndex)
        {
            GameObject treeNode = null;
            if (pool.Count > 0)
            {
                treeNode = pool[0];
                pool.RemoveAt(0);
            }
            else
            {
                treeNode = CloneTreeNode();
            }
            treeNode.transform.SetParent(container);
            treeNode.SetActive(true);
            //treeNode.GetComponent<UITreeNode>().SetData(data);
            treeNode.GetComponent<TreeViewNode>().Insert(data);
            treeNode.transform.SetSiblingIndex(siblingIndex + 1);
            return treeNode;
        }

        public void Push(List<GameObject> treeNodes)
        {
            foreach (GameObject node in treeNodes)
            {
                Push(node);
            }
        }
        public void Push(GameObject treeNode)
        {
            if (null == poolParent)
            {
                poolParent = new GameObject("CachePool").transform;
            }
            treeNode.transform.SetParent(poolParent);
            treeNode.SetActive(false);
            pool.Add(treeNode);
        }

        private GameObject CloneTreeNode()
        {
            GameObject result = Instantiate(NodePrefab);
            result.transform.SetParent(container);
            return result;
        }

        #endregion
    }
}
