using UnityEngine;
using System.IO;

namespace ColorPaletterV2
{
    public static class ColorPaletterDataManager
    {
        /// <summary>
        /// Save the data
        /// </summary>
        public static void SaveData<T>(string fileName, T data, bool debugMessages = true)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                // serialize data wrapper to JSON
                string jsonData = JsonUtility.ToJson(data, true);

                // write JSON data to file
                File.WriteAllText(filePath, jsonData);

                if (debugMessages)
                    Debug.Log($"Successfully saved!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save to file: {e}");
            }
        }

        /// <summary>
        /// Load the data 
        /// </summary>
        public static T LoadData<T>(string fileName, bool debugMessages = true)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            // create file if needed
            if (!File.Exists(filePath))
            {
                Debug.Log("Save file will automatically be generated upon saving for the first time");
                return default(T);
            }

            try
            {
                // read JSON data from file
                string jsonData = File.ReadAllText(filePath);

                // deserialize JSON data to data wrapper
                T data = JsonUtility.FromJson<T>(jsonData);

                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Couldn't load data from file: {e}");
                return default(T);
            }
        }
    }
}