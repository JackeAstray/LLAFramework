using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeViewExample : MonoBehaviour
{
    public TreeView UITree = null;

    public void Awake()
    {
        var data = new TreeViewData("Test", new List<TreeViewData>()
        {
            new TreeViewData("Button",new List<TreeViewData>()
            {
                new TreeViewData("DoubleClickButton",2),
                new TreeViewData("LongClickButton")
            }),
            new TreeViewData("Pie"),
            new TreeViewData("DatePicker"),
            new TreeViewData("C#",new List<TreeViewData>()
            {
                new TreeViewData("high-level syntax",new List<TreeViewData>()
                {
                    new TreeViewData("Action",new List<TreeViewData>()
                        {
                            new TreeViewData("One parameter"),
                            new TreeViewData("Two parameter"),
                            new TreeViewData("Three parameter"),
                            new TreeViewData("Four parameter"),
                            new TreeViewData("Action2",new List<TreeViewData>()
                            {
                                new TreeViewData("One parameter"),
                                new TreeViewData("Two parameter"),
                                new TreeViewData("Three parameter"),
                                   new TreeViewData("Action3",new List<TreeViewData>()
                                    {
                                        new TreeViewData("One parameter"),
                                        new TreeViewData("Two parameter"),
                                        new TreeViewData("Three parameter"),
                                    })
                            })
                        }),
                    new TreeViewData("Func"),
                    new TreeViewData("delegate")
                }),
                new TreeViewData("Reflect")
            })
        });
        //UITree.SetData(data);
        UITree.Insert(data);
    }
}
