using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameLogic.Download;

namespace GameLogic.Example
{
    public class ExampleLoadImage : MonoBehaviour
    {
        public RawImage image;
        public string url;
        void Start()
        {
            if (null == image)
                image = GetComponent<RawImage>();
            if (null == url)
                url = "";
            TestLoadImage();
        }

        private void TestLoadImage()
        {
            LoadImageHelper helper = gameObject.AddComponent<LoadImageHelper>();
            helper.LoadImage(image, url);
        }
    }
}