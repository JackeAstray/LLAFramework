using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 射线管理器
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

        public bool CastRayFromScreenPoint(Vector2 screenPoint, out RaycastHit hitInfo)
        {
            Ray ray = camera.ScreenPointToRay(screenPoint);
            return Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask);
        }
    }
}
