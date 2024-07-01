using GameLogic;
using GameLogic.Download;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadImageExample : MonoBehaviour
{
    RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();

        Invoke("Init",2f);
    }

    public void Init()
    {
        Log.Debug("Init");
        DownloadImageModule.Instance.DownloadImage("http://gips0.baidu.com/it/u=3602773692,1512483864&fm=3028&app=3028&f=JPEG&fmt=auto?w=960&h=1280", (texture) =>
        {
            rawImage.texture = texture;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}