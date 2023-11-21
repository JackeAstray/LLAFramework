// using UnityEditor;
// using UnityEngine;

// namespace GameLogic.EditorTools
// {
//     /// <summary>
//     /// 图片导入时处理
//     /// </summary>
//     public class TextureImporter : AssetPostprocessor
//     {
//         /// <summary>
//         /// 处理Texture
//         /// </summary>
//         public void OnPreprocessTexture()
//         {
//             var importer = assetImporter as UnityEditor.TextureImporter;

//             //第一次导入时设置
//             if (importer.importSettingsMissing)
//             {
//                 var standaloneSettings = new TextureImporterPlatformSettings();
//                 var webGLSettings = new TextureImporterPlatformSettings();
//                 var andSettings = new TextureImporterPlatformSettings();
//                 var iosSettings = new TextureImporterPlatformSettings();

//                 andSettings.name = "Android";
//                 andSettings.overridden = true;
//                 andSettings.compressionQuality = 50;
//                 andSettings.format = TextureImporterFormat.ETC2_RGBA8;
//                 andSettings.androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit;
//                 andSettings.maxTextureSize = 1024;
//                 importer.SetPlatformTextureSettings(andSettings);

//                 iosSettings.name = "iPhone";
//                 iosSettings.overridden = true;
//                 iosSettings.compressionQuality = 50;
//                 iosSettings.format = TextureImporterFormat.ASTC_6x6;
//                 iosSettings.maxTextureSize = 1024;
//                 importer.SetPlatformTextureSettings(iosSettings);

//                 standaloneSettings.name = "Standalone";
//                 standaloneSettings.overridden = true;
//                 standaloneSettings.compressionQuality = 50;
//                 standaloneSettings.format = TextureImporterFormat.DXT5;
//                 standaloneSettings.maxTextureSize = 1024;
//                 importer.SetPlatformTextureSettings(standaloneSettings);

//                 webGLSettings.name = "WebGL";
//                 webGLSettings.overridden = true;
//                 webGLSettings.compressionQuality = 50;
//                 webGLSettings.format = TextureImporterFormat.DXT5;
//                 webGLSettings.maxTextureSize = 1024;
//                 importer.SetPlatformTextureSettings(webGLSettings);

//                 string[] rootDir = importer.assetPath.Split('/');

//                 if (rootDir.Length >= 3)
//                 {
//                     if (rootDir[2] == "Sprites")
//                     {
//                         importer.isReadable = true;
//                         //把格式改成sprite
//                         importer.textureType = TextureImporterType.Sprite;
//                         //图片不用分割，就一张
//                         importer.spriteImportMode = SpriteImportMode.Single;
//                         //有透明通道
//                         importer.alphaIsTransparency = true;
//                         //不需要图片深度
//                         importer.mipmapEnabled = false;
//                     }
//                     else if (importer.assetPath.Contains(".exr"))
//                     {
//                         importer.textureType = TextureImporterType.Lightmap;
// #if UNITY_2018_3_OR_NEWER
//                         importer.textureCompression = TextureImporterCompression.Uncompressed;
//                         var ftm = importer.GetDefaultPlatformTextureSettings();
//                         ftm.format = TextureImporterFormat.RGBA32;
//                         importer.SetPlatformTextureSettings(ftm);
// #else
//                         importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
// #endif
//                         importer.wrapMode = TextureWrapMode.Clamp;
//                     }
//                 }
//             }
//         }
//     }
// }