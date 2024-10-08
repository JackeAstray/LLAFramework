
using UnityEngine;
using UnityEngine.UI;

namespace GameLogin.DatePicker
{
    public interface IBarChartFactory
    {
        VertexHelper DrawBarChart(VertexHelper vh, Rect rect, SgSettingBase baseSetting, BarChartSetting barChartSetting, BarChartData data = null);
    }
}
