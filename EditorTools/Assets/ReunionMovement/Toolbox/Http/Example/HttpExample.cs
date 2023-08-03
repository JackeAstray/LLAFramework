using GameLogic.HttpModule.Service;
using GameLogic.HttpModule;
using UnityEngine;

namespace GameLogic.Example
{
    public class HttpExample : MonoBehaviour
    {

        public string Url = "http://geological.pro6.liuniukeji.net/";

        void Start()
        {
            string tempUrl = Url + "api.php/SjapplyPile/getSjapplyBars";
            WWWForm form = new WWWForm();
            form.AddField("project_id", 1);
            form.AddField("drill_number", 2);
            
            var request = Http.Post(tempUrl, form).
                OnSuccess(HandleSuccess_Histogram)
                .OnError(response => Debug.LogError(response.StatusCode))
                .Send();
        }

        /// <summary>
        /// 处理柱状图数据
        /// </summary>
        /// <param name="response"></param>
        private void HandleSuccess_Histogram(HttpResponse response)
        {
            // HistogramV2 histogram = JsonMapper.ToObject<HistogramV2>(response.Text);
            // if (histogram != null && histogram.data.Count > 0)
            // {
            //     histogram.data.Sort((x, y) => (int.Parse(x.sort)).CompareTo(int.Parse(y.sort)));
            // }
            // histogramScrollView.SetActive(true);
            // HistogramScrollView.Instance.SetText(histogram);
        }
    }
}