using LLAFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeViewExample : MonoBehaviour
{
    public TreeView UITree = null;
    List<TreeViewData> rootData = new List<TreeViewData>();

    public void Awake()
    {
        var data = new TreeViewData("空调", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
                new TreeViewData("电路图",new List<TreeViewData>()
                {
                },
                ()=>{ Debug.Log("测试"); })
                },
            null),
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("蓄电池", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("逆变器", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("照明设备", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("真空集便器", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("控制柜", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("充电器", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("塞拉门", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("电子防滑器", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        rootData.Add(data);
        data = new TreeViewData("轴温报警装置", new List<TreeViewData>()
        {
            new TreeViewData("二级结构",new List<TreeViewData>()
            {
            },
            null),
            new TreeViewData("电路图",new List<TreeViewData>()
            {
            },
            null)
        },
        null);
        UITree.Insert(rootData);
    }

    public void UpdateNodeDisplayDecorate(TreeViewData data, bool displayDecorate)
    {
        // 遍历所有的 TreeViewNode 并更新 displayDecorate
        foreach (var node in UITree.treeRootNodes)
        {
            if (node.GetTreeData() == data)
            {
                node.UpdateDisplayDecorate(displayDecorate);
            }
        }
    }
}
