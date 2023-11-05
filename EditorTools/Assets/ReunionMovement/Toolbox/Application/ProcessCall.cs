//===========================
//*Copyright(C) 2022 by 六牛科技 
//*All rights reserved. 
//*FileName:  UnityCallAndroid.cs 
//*Author:    六牛科技 
//*Version:   1.00
//*UnityVersion:  Unity2019.4.37f1c1
//*CreateTime:    2022-09-06 17:47:03
//*Description:     功能介绍    
//===========================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCall : SingleToneManager<ProcessCall>
{
    public string deeplinkURL;

    void Awake()
    {
        Instance = this;

        Application.deepLinkActivated += onDeepLinkActivated;

        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            //冷启动和应用。absoluteURL不为空，因此处理Deep Link
            onDeepLinkActivated(Application.absoluteURL);
            Debug.Log("AbsoluteURL: " + Application.absoluteURL);
        }
        // 初始化DeepLink Manager全局变量
        else
        {
            deeplinkURL = "[None]";
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            ApplicationChrome.SetStatusBar(true);
            int color = ApplicationChrome.ConvertColorToAndroidColor(ColorConversion.HexToColor("00000000"));
            ApplicationChrome.SetStatusBarColor(color, true);
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 修改状态栏颜色
    /// </summary>
    /// <param name="colorStr"></param>
    /// <param name="isBlack"></param>
    public void SetColor(string colorStr, bool isBlack)
    {

    }

    private void onDeepLinkActivated(string url)
    {
        StartCoroutine(onDeepLinkActivated2(url));
    }

    IEnumerator onDeepLinkActivated2(string url)
    {
        yield return new WaitForSeconds(2f);

        // 更新 DeepLink Manager 全局变量，以便可以从任何位置访问 URL。
        deeplinkURL = url;
        Debug.Log(deeplinkURL);

        string analysis_1 = deeplinkURL.Split('?')[1];

        string[] analysis_2 = analysis_1.Split('&');

        string type = "";
        string itemid = "";
        string code = "";

        if (analysis_2.Length > 0)
        {
            type = analysis_2[0].Split('=')[1];
            itemid = analysis_2[1].Split('=')[1];
            code = analysis_2[2].Split('=')[1];
        }

        if (!string.IsNullOrEmpty(GameManager.Token))
        {
            if (type == "1")
            {
                MainManager.GetInstance.SendMsg((uint)BranchMainManager.ReturnFromObjectContent, new Notification());

                JumpTools.GetItemInfo(itemid, code, type);
            }
            else if (type == "2")
            {
                MainManager.GetInstance.SendMsg((uint)BranchMainManager.ReturnFromItemContent, new Notification());

                GameManager.ItemId = itemid;
                GameManager.ItemType = int.Parse(type);
                MainManager.GetInstance.SendMsg((uint)BranchMainManager.OpenItem, new Notification());
            }
            else if (type == "3")
            {
                MainManager.GetInstance.SendMsg((uint)BranchMainManager.ReturnFromItemContent, new Notification());

                GameManager.ItemId = itemid;
                GameManager.ItemType = int.Parse(type);
                MainManager.GetInstance.SendMsg((uint)BranchMainManager.OpenItem, new Notification());
            }
            else if (type == "4")
            {
                MainManager.GetInstance.SendMsg((uint)BranchMainManager.ReturnFromObjectContent, new Notification());

                JumpTools.GetUserItemInfo(itemid);
            }
        }
        else
        {
            GameManager.Tip = "请先登录！";
            Notification notification = new Notification("Tip");
            UIManager.GetInstance.SendMsg((uint)BranchUIManager.PushUI, notification);
        }
    }
}
