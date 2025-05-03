//此脚本为自动生成

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LLAFramework.UI
{
    public class TerminalUIPlane : UIController
    {
        string command;
        public Button clear;    //清除
        public Button close;    //关闭
        public TMP_InputField input;//输入

        public GameObject root;
        public GameObject itemGo;

        public override void OnInit()
        {
            base.OnInit();

            clear.onClick.RemoveAllListeners();
            close.onClick.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();

            command = "";

            clear.onClick.AddListener(() => 
            {
                root.ThisClearChild();
            });
            
            close.onClick.AddListener(() =>
            {
                UIModule.Instance.CloseWindow("TerminalUIPlane");
            });

            input.onEndEdit.AddListener(OnEndEdit);
        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
        }

        public override void OnSet(params object[] args)
        {
            base.OnSet(args);

            if (args.Length > 0)
            {
                switch(args[0].ToString())
                {
                    case "CreateItem":
                        if(args.Length >= 2)
                        {
                            CreateItem(args[1].ToString());
                        }
                        break;
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public void OnDestroy()
        {

        }

        public void OnEndEdit(string text)
        {
            command = text;
            /*"TestTerminal 2 2"*/
            TerminalModule.Instance.terminalRequest.ParseCommand(command);
            CreateItem(command);
        }

        public void CreateItem(string str)
        {
            if (root == null)
            {
                return;
            }
            if (itemGo == null)
            {
                return;
            }

            GameObject @object = Instantiate(itemGo, Vector3.zero, Quaternion.identity);
            @object.transform.SetParent(root.transform);
            @object.GetComponent<TerminalItem>().SetText(str);

            LayoutRebuilder.ForceRebuildLayoutImmediate(root.GetComponent<RectTransform>());
        }
    }
}
