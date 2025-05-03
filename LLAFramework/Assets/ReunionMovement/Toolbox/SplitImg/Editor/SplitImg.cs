using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LLAFramework.EditorTools
{
    /// <summary>
    /// 切割图片非透明区域工具
    /// </summary>

    public class SplitImg
    {
        [MenuItem("Assets/切割图片空白区域")]
        private static void SelectSplitImg()
        {
            string param = "\"";
            Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
            foreach (Texture2D texture2D in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture2D);
                path = path.Replace("Assets/", string.Empty);
                path = Path.Combine(Application.dataPath, path);
                path = path.Replace("/", "\\");
                param += path + ";";
            }

            param += "\"";
            
            Log.Debug(param);
            
            string toolsPath = Path.Combine(Application.dataPath, "../Tools/SplitImg/SplitImg.exe");
            Process p = new Process();
            p.StartInfo.FileName = toolsPath;
            p.StartInfo.Arguments = param;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WorkingDirectory = Application.dataPath;
            p.OutputDataReceived += DataReceivedEvent;
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.Close();
            p.Dispose();
            AssetDatabase.Refresh();
        }

        private static void DataReceivedEvent(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Log.Debug(e.Data);
            }
        }
    }
}
