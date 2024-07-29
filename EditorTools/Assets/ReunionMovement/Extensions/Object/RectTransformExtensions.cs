using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 矩形变换扩展
    /// </summary>
    public static class RectTransformExtensions
    {
        private static Vector2 pivotTopRight = new Vector2(1, 1);
        private static Vector2 pivotTop = new Vector2(0.5f, 1);
        private static Vector2 pivotCenter = new Vector2(0.5f, 0.5f);
        private static Vector2 AnchorPos = new Vector2(0, 0);

        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            var pos = rectTransform.anchoredPosition;
            pos.x = x;
            rectTransform.anchoredPosition = pos;
        }

        public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
        {
            var pos = rectTransform.anchoredPosition;
            pos.y = y;
            rectTransform.anchoredPosition = pos;
        }

        public static void SetAnchoredPositionZ(this RectTransform rectTransform, float z)
        {
            var pos = rectTransform.anchoredPosition3D;
            pos.z = z;
            rectTransform.anchoredPosition3D = pos;
        }

        public static void SetSizeDeltaX(this RectTransform rectTransform, float x)
        {
            var size = rectTransform.sizeDelta;
            size.x = x;
            rectTransform.sizeDelta = size;
        }

        public static void SetSizeDeltaY(this RectTransform rectTransform, float y)
        {
            var size = rectTransform.sizeDelta;
            size.y = y;
            rectTransform.sizeDelta = size;
        }

        public static void SetAnchorMinX(this RectTransform rectTransform, float x)
        {
            var anchor = rectTransform.anchorMin;
            anchor.x = x;
            rectTransform.anchorMin = anchor;
        }

        public static void SetAnchorMinY(this RectTransform rectTransform, float y)
        {
            var anchor = rectTransform.anchorMin;
            anchor.y = y;
            rectTransform.anchorMin = anchor;
        }

        public static void SetAnchorMaxX(this RectTransform rectTransform, float x)
        {
            var anchor = rectTransform.anchorMax;
            anchor.x = x;
            rectTransform.anchorMax = anchor;
        }

        public static void SetAnchorMaxY(this RectTransform rectTransform, float y)
        {
            var anchor = rectTransform.anchorMax;
            anchor.y = y;
            rectTransform.anchorMax = anchor;
        }

        public static void SetPivotX(this RectTransform rectTransform, float x)
        {
            var pivot = rectTransform.pivot;
        }

        /// <summary>
        /// 设置锚点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="Type"></param>
        public static void SetAnchor(this RectTransform rect, AnchorType Type)
        {
            if (rect == null)
                return;
            var size = rect.sizeDelta;
            //left,right对应x,top,bottom对应Y
            switch (Type)
            {
                case AnchorType.TopRight:
                    rect.pivot = pivotCenter;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, size.x);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, size.y);
                    rect.anchoredPosition = new Vector2(-size.x * .5f, -size.y * .5f);
                    break;
                case AnchorType.TopLeft:
                    rect.pivot = pivotCenter;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, size.x);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, size.y);
                    rect.anchoredPosition = new Vector2(size.x * .5f, -size.y * .5f);
                    break;
                case AnchorType.Stretch:
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    break;
                case AnchorType.StretchTop:
                    rect.pivot = pivotTop;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = Vector2.one;
                    break;
                case AnchorType.StretchBottom:
                    rect.pivot = pivotTop;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = new Vector2(1, 0);
                    break;
                case AnchorType.StretchLeft:
                    rect.pivot = pivotCenter;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = new Vector2(0, 1);
                    break;
                case AnchorType.StretchRight:
                    rect.pivot = pivotCenter;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rect.anchorMin = new Vector2(1, 0);
                    rect.anchorMax = Vector2.one;
                    break;
                default:
                    Debug.Log("未知的锚点类型");
                    break;
            }
        }
    }
}