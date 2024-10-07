using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeViewExample : MonoBehaviour
{
    public UITree UITree = null;

    public void Awake()
    {
        var data = new UITreeData("Test", new List<UITreeData>()
        {
            new UITreeData("Button",new List<UITreeData>()
            {
                new UITreeData("DoubleClickButton",2),
                new UITreeData("LongClickButton")
            }),
            new UITreeData("Pie"),
            new UITreeData("DatePicker"),
            new UITreeData("C#",new List<UITreeData>()
            {
                new UITreeData("high-level syntax",new List<UITreeData>()
                {
                    new UITreeData("Action",new List<UITreeData>()
                        {
                            new UITreeData("One parameter"),
                            new UITreeData("Two parameter"),
                            new UITreeData("Three parameter"),
                            new UITreeData("Four parameter"),
                            new UITreeData("Action2",new List<UITreeData>()
                            {
                                new UITreeData("One parameter"),
                                new UITreeData("Two parameter"),
                                new UITreeData("Three parameter"),
                                   new UITreeData("Action3",new List<UITreeData>()
                                    {
                                        new UITreeData("One parameter"),
                                        new UITreeData("Two parameter"),
                                        new UITreeData("Three parameter"),
                                    })
                            })
                        }),
                    new UITreeData("Func"),
                    new UITreeData("delegate")
                }),
                new UITreeData("Reflect")
            })
        });
        //UITree.SetData(data);
        UITree.Insert(data);
    }
}
