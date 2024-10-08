using UnityEngine;
using UnityEngine.UI;

namespace GameLogin.DatePicker
{
    public interface ISpringGUIBase
    {
        // 两点绘制直线
        UIVertex[] GetQuad(Vector2 startPos, Vector2 endPos, Color color0, float LineWidth = 2.0f);
        // 零点绘制直线
        VertexHelper GetQuad(VertexHelper vh, Vector2 startPos, Vector2 endPos, Color color0, float LineWidth = 2.0f);

        // 两点绘制虚线
        VertexHelper GetQuadDottedline(VertexHelper vh, Vector2 startpos, Vector2 endPos, Color color0, float lineWidth = 2.0f);

        // 获取UIVertex
        UIVertex GetUIVertex(Vector2 point, Color color0);
        // 获取UIVertex
        UIVertex GetUIVertex(Vector2 point, Color color0, Vector2 uv);
    }
}