using UnityEngine;
using UnityEngine.UI;

namespace GameLogin.DatePicker
{
    public interface ILineChart
    {
        VertexHelper DrawLineChart(VertexHelper vh, Rect rect, LineChartData basis);
        VertexHelper DrawMesh(VertexHelper vh);
        VertexHelper DrawAxis(VertexHelper vh);
    }
}