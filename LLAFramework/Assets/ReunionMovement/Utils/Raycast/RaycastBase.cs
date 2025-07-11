﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 射线管理器-基类
    /// </summary>
    public class RaycastBase
    {
        private LayerMask layerMask;
        private Camera camera;

        public RaycastBase(string layerName, Camera camera)
        {
            layerMask = 1 << LayerMask.NameToLayer(layerName);
            this.camera = camera;
        }
        public RaycastBase(LayerMask layerMask, Camera camera)
        {
            this.layerMask = layerMask;
            this.camera = camera;
        }

        public bool CastRayFromScreenPoint(Vector2 screenPoint, out RaycastHit hitInfo)
        {
            Ray ray = camera.ScreenPointToRay(screenPoint);
            return Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask);
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
        }

        public void SetLayerName(string layerName)
        {
            layerMask = 1 << LayerMask.NameToLayer(layerName);
        }
    }
}
