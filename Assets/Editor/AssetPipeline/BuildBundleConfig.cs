using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using YamlDotNet.Serialization;

namespace AssetPipeline
{
    public static class BuildBundlePath
    {
        //公共资源项目路径
        public static HashSet<string> CommonFilePath = new HashSet<string>
        {
            //Unity5打包动画资源时无需依赖打包BaseAC,直接打包相应的OverrideController,否则会导致播放动画异常
            //"Assets/GameResources/ArtResources/Characters/AnimatorController/BaseAC.controller", 
            "Assets/GameResources/ArtResources/Audios/Sound/sound_UI/sound_UI_button_click.wav"
        };

        //Shader资源路径
        public static HashSet<string> ShaderFilePath = new HashSet<string>
        {
            "Assets/Shaders/BaoyuShader/Toon/Baoyu-Unlit-ToonOcclusion.shader",
            "Assets/Shaders/BaoyuShader/Toon/Baoyu-Unlit-Toon.shader",

        };

        //Shader资源目录
        public static string[] ShaderFolder =
        {
            "Assets/NGUI/Resources"
        };

        //UI资源目录与指定文件路径
        public static string[] UIPrefabFolder =
        {
            "Assets/UI/Prefabs"
        };
        public static string[] UIAtlasFolder =
        {
            "Assets/UI/Atlas"
        };
        public static string[] UIFontFolder =
        {
            "Assets/UI/Fonts"
        };
        public static HashSet<string> ImageFilePath = new HashSet<string>
        {
        };
        public static string[] ImageFolder =
        {
            "Assets/GameResources/ArtResources/Images",
            "Assets/UI/Images",
        };

        public static string[] UITextureFolder =
        {
            "Assets/UI/Atlas/CommonTextures",
            "Assets/Resources/Textures"
        };

        //模型资源目录
        public static string[] ModelPrefabFolder =
        {
            "Assets/GameResources/ArtResources/Characters/Pet",
            "Assets/GameResources/ArtResources/Characters/Other",
            "Assets/GameResources/ArtResources/Characters/Weapon",
            "Assets/GameResources/ArtResources/Characters/Fashion",
            "Assets/GameResources/ArtResources/Characters/Spine",
        };
        //特效资源目录
        public static string[] EffectPrefabFolder =
        {
            "Assets/GameResources/ArtResources/Effects",
            "Assets/GameResources/ArtResources/Characters/Soul",
            "Assets/GameResources/ArtResources/Scenes2d/SceneEffects"
        };

        //音频资源目录
        public static string[] AudioFolder =
        {
           "Assets/GameResources/ArtResources/Audios"
        };

        //配置资源目录
        public static string[] ConfigFolder =
        {
            "Assets/GameResources/ConfigFiles",
            "Assets/GameResources/ArtResources/SceneConfig/LightMap",
            "Assets/GameResources/ArtResources/SceneConfig/Navmesh",
            "Assets/GameResources/ArtResources/SceneConfig/SceneGo",
            "Assets/GameResources/ArtResources/SceneConfig/SceneTrigger",

        };

        //脚本代码资源目录
        public static string[] ScriptFolder =
        {
            "Assets/JavaScript",
            "Assets/Scripts/GameProtocol/h1-clientservice/javascript",
            "Assets/TempJSBCode/Editor/Scripts/GameProtocol/app-clientservice/javascript"
        };

        public static string[] StreamScenes =
        {
            "Assets/GameResources/ArtResources/Scenes/StreamScenes",
        };
        public static bool IsCommonAsset(this string path)
        {
            return CommonFilePath.Contains(path);
        }

        public static bool IsUIPrefab(this string path)
        {
            return UIPrefabFolder.Any(path.StartsWith);
        }

        public static bool IsUIAtlas(this string path)
        {
            return UIAtlasFolder.Any(path.StartsWith);
        }

        public static bool IsUIFont(this string path)
        {
            return UIFontFolder.Any(path.StartsWith);
        }

        public static bool IsGameImage(this string path)
        {
            return ImageFilePath.Contains(path) || ImageFolder.Any(path.StartsWith);
        }

        public static bool IsGameUITexture(this string path)
        {
            return UITextureFolder.Any(path.StartsWith);
        }


        public static bool IsGameModel(this string path)
        {
            return ModelPrefabFolder.Any(path.StartsWith);
        }

        public static bool IsGameEffect(this string path)
        {
            return EffectPrefabFolder.Any(path.StartsWith);
        }

        public static bool IsGameAudio(this string path)
        {
            return AudioFolder.Any(path.StartsWith);
        }

        public static bool IsGameConfig(this string path)
        {
            return ConfigFolder.Any(path.StartsWith);
        }

        public static bool IsGameScript(this string path)
        {
            return ScriptFolder.Any(path.StartsWith);
        }

        public static bool IsPrefabFile(this string path)
        {
            return path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsConfigFile(this string path)
        {
            return path.EndsWith(".bytes", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsAudioFile(this string path)
        {
            return path.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsTextureFile(this string path)
        {
            return path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".tga", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsShaderFile(this string path)
        {
            return path.EndsWith(".shader");
        }

        public static bool IsMaterial(this string path)
        {
            return path.EndsWith(".mat");
        }

        public static bool IsFBX(this string path)
        {
            return path.EndsWith(".FBX");
        }

        internal static bool EnableException = false;
        public static bool UpdateBundleName(this AssetImporter importer, string bundleName)
        {
            bundleName = bundleName.ToLower();
            if (!CheckBundleName(bundleName))
            {
                string errorMessage = string.Format("BundleName 命名不合法:{0} \n AssetPath:{1}", bundleName, importer.assetPath);
                Debug.LogError(errorMessage, AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(importer.assetPath));
                if (EnableException)
                    throw new Exception(errorMessage);
                else
                    EditorUtility.DisplayDialog("错误", "资源命名不符合规范\n" + errorMessage, "确定");
            }
            var oldBundleName = importer.assetBundleName;
            if (oldBundleName != bundleName)
            {
                importer.SetAssetBundleNameAndVariant(bundleName, null);
                return true;
            }
            return false;
        }
        private static bool CheckBundleName(string bundleName)
        {
            Regex regex = new Regex("\\s");
            return !regex.Match(bundleName).Success;
        }

        /// <summary>
        /// 注意:同一资源分组下的资源名不能重复,然后根据资源分组生成BundleName
        /// 例如:MainUIView.prefab --> ui/mainuiview
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="resGroup"></param>
        /// <returns></returns>
        public static string GetAssetBundleName(this AssetImporter importer, ResGroup resGroup)
        {
            string assetName = Path.GetFileNameWithoutExtension(importer.assetPath);
            return AssetManager.GetBundleName(assetName, resGroup);
        }

        public static string ExtractResName(this string path, bool removeExtension = true)
        {
            return removeExtension ? Path.GetFileNameWithoutExtension(path) : Path.GetFileName(path);
        }
    }

    /// <summary>
    /// 导出小包配置策略,定义了如何生成MiniResConfig信息
    /// </summary>
    public class BuildBundleStrategy
    {
        public Dictionary<string, string> replaceResConfig; //策划自定义小包替代资源信息
        public Dictionary<string, string> minResConfig; //小包必需资源信息
        public Dictionary<string, bool> preloadConfig; //预加载资源配置

        public BuildBundleStrategy()
        {
            replaceResConfig = new Dictionary<string, string>();
            minResConfig = new Dictionary<string, string>();
            preloadConfig = new Dictionary<string, bool>();
        }

        public void AddMinResKey(string bundleName)
        {
            minResConfig[bundleName] = "";
        }

        public void RemoveMinResKey(string bundleName)
        {
            minResConfig.Remove(bundleName);
        }
    }

    /// <summary>
    /// Unity打包Bundle后生成的总资源清单YAML文件对应数据类
    /// </summary>
    public class RawAssetManifest
    {
        public int ManifestFileVersion { get; set; }
        public uint CRC { get; set; }

        [YamlMember(Alias = "AssetBundleManifest")]
        public RawBundleManifest Manifest { get; set; }

        public class RawBundleManifest
        {
            public Dictionary<string, RawBundleInfo> AssetBundleInfos { get; set; }

            public class RawBundleInfo
            {
                public string Name { get; set; }
                public Dictionary<string, string> Dependencies { get; set; }
            }
        }
    }

    /// <summary>
    /// Unity打包Bundle后每个Bundle对应YAML文件的数据类
    /// </summary>
    public class RawBundleManifest
    {
        public int ManifestFileVersion { get; set; }
        public uint CRC { get; set; }
        public Dictionary<string, HashInfo> Hashes { get; set; }
        public int HashAppended { get; set; }
        public List<object> ClassTypes { get; set; }
        public List<string> Assets { get; set; }
        public List<string> Dependencies { get; set; }

        public class HashInfo
        {
            public int serializedVersion { get; set; }
            public string Hash { get; set; }
        }
    }

    public class EnableBundleNameException:IDisposable
    {
        public EnableBundleNameException()
        {
            BuildBundlePath.EnableException = true;
        }
        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                BuildBundlePath.EnableException = false;
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~EnableBundleNameException()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}