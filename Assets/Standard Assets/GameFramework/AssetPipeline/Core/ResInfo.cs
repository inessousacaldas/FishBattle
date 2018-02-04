using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace AssetPipeline
{
    public class VersionConfig
    {
        public enum ServerType
        {
            Null = -1,
            Default,
            Beta,
        }


        //标记当前版本信息类型,用于区分游戏公测服,游戏审核服,游戏Beta服
        public ServerType serverType = ServerType.Default;
        //游戏引擎框架版本号,用来标识当前框架版本是否需要整包替换更新
        //例如Android下jar包无法热更,还有IOS的dll无法更新时,根据这个版本号来提示用户需要整包更新
        public long frameworkVer;
        //标记当前游戏服最新dll版本号,只有PC和Android会用到
        public long dllVersion;
        //标记当前游戏服最新资源版本号
        public long resVersion;
        //是否强制更新
        public bool forceUpdate = true;
        //版本更新帮助页面
        public string helpUrl = "";

        public string ToFileName()
        {
            return "versionConfig_" + (int)serverType + ".json";
        }

        public static string GetFileName(ServerType serverType)
        {
            return "versionConfig_" + (int)serverType + ".json";
        }

        public override string ToString()
        {
            return "serverType: " + serverType +
                "\nframeworkVer: " + frameworkVer +
                "\ndllVersion: " + dllVersion +
                "\nresVersion: " + resVersion +
                "\nforceUpdate: " + forceUpdate;
        }
    }


    public class DllInfo
    {
        public string dllName;
        public string MD5;
        public long size;

        public string ToFileName()
        {
            return dllName + "_" + MD5 + ".dll";
        }

    }

    public class DllVersion
    {
        public long Version;
        public Dictionary<string, DllInfo> Manifest;

        public VersionConfig.ServerType serverType = VersionConfig.ServerType.Default;

        public DllVersion()
        {
            Manifest = new Dictionary<string, DllInfo>();
        }

        public string ToFileName()
        {
            return "dllVersion_" + Version + ".json";
        }

        public static string GetFileName(long version)
        {
            return "dllVersion_" + version + ".json";
        }
    }

    public enum CompressType
    {
        Raw = 0,
        UnityLZMA = 1,
        UnityLZ4 = 2,
        CustomZip = 10,
        CustomLZMA = 11,
        CustomLZ4 = 12,
        CustomTex = 13,
    }

    /// <summary>
    /// 对于资源分组标识,根据分组标识导出到不同目录
    /// </summary>
    public enum ResGroup
    {
        None = 0,
        Common = 1,
        Scene = 2,
        Audio = 3,
        Config = 4,
        Script = 5,

        UIPrefab = 10,
        UIAtlas = 11,
        UIFont = 12,
        Image = 13,         //对话框头像,地图缩略图等一类需要异步加载的图像资源
        UITexture = 14,     //被UIPrefab直接引用到的一些图像资源
        StreamScene = 15,   //用于流式加载的场景资源

        Model = 20,
        Effect = 30,
        TileMap = 40,
    }

    /// <summary>
    /// 记录了游戏资源的配置信息，用于资源打包和加载资源处理资源的依赖关系
    /// </summary>
    public class ResInfo
    {
        //项目内BundleName
        public string bundleName;
        //当前资源包CRC值
        //注:相同的资源使用不同的压缩方式打包时,计算出的CRC是一样的
        public uint CRC;
        //当前资源包Hash128值
        //注:如果现在打包Android资源,但是贴图的PC平台导入配置修改了,会导致Hash变化,但是打包出来的CRC是一样的
        //简单来说就是Hash变了,CRC可能不变,但Hash不变,CRC也不会变
        public string Hash;
        //标记Bundle文件放在CDN上的压缩类型,不是指打包Bundle时的压缩类型
        //如果是使用LZ4或不压缩方式打包资源,需要再用Zip压缩一遍,上传给CDN,这样可以有效减少用户的下载数据总量
        public CompressType remoteZipType;
        //记录资源包文件MD5值(压缩后)
        public string MD5;
        //记录资源包文件文件大小(压缩后)
        public long size;
        //标记该资源为包内资源,小包或者更新过的资源都将置为false
        public bool isPackageRes;
        //标记该资源包是否需要预加载
        public bool preload;
        //当前资源包依赖资源包key列表
        public List<string> Dependencies;

        public ResInfo()
        {
            remoteZipType = CompressType.UnityLZ4;
            Dependencies = new List<string>();
        }

        public string GetABPath(string dir)
        {
#if BUNDLE_APPEND_HASH
            return dir + "/" + bundleName + "_" + Hash;
#else
            return dir + "/" + bundleName + "_" + CRC;
#endif
        }

        public string GetRemotePath(string dir)
        {
            if (remoteZipType == CompressType.CustomZip)
                return GetABPath(dir) + ".zip";
            return GetABPath(dir);
        }

        public string GetManifestPath(string dir)
        {
            if (remoteZipType == CompressType.CustomTex)
                return dir + "/" + Path.ChangeExtension(bundleName, ".json");
            else
                return dir + "/" + bundleName + ".manifest";
        }

        public string GetExportPath(string dir)
        {
#if BUNDLE_APPEND_HASH
            return dir + "/" + bundleName + "_" + Hash;
#else
            return dir + "/" + bundleName;
#endif
        }
    }

    public class ResConfig
    {
        public VersionConfig.ServerType serverType = VersionConfig.ServerType.Default;

        //记录本次打包版本号,版本号根据上一个版本进行递增
        public long Version;
        //记录本次打包资源的资源清单CRC值
        public uint lz4CRC;
        public uint lzmaCRC;
        public uint tileTexCRC;
        //记录该版本资源打包时间
        public long BuildTime;
        //资源压缩类型
        public CompressType compressType;
        //记录了AssetBundle文件的总大小（单位：byte）
        public long TotalFileSize;
        //标记当前资源是否是小包,从小包升级为整包后该标记置为false
        public bool isMiniRes;
        //以资源名_ResType为key
        public Dictionary<string, ResInfo> Manifest;

        public ResConfig()
        {
            compressType = CompressType.UnityLZ4;
            Manifest = new Dictionary<string, ResInfo>();
        }

        public ResInfo GetResInfo(string key)
        {
            if (Manifest.ContainsKey(key))
                return Manifest[key];
            return null;
        }

        public string ToFileName()
        {
            return "resConfig_" + Version + ".json";
        }

        public string ToRemoteName()
        {
            return "resConfig_" + Version + ".tz";
        }


        public static string GetRemoteFile(long version)
        {
            return "resConfig_" + version + ".tz";
        }

        /// <summary>
        /// 从BundleName获取对应的ResGroup
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static ResGroup GetResGroupFromBundleName(string bundleName)
        {
            if (!bundleName.Contains("/")) return ResGroup.None;

            var resGroupEnums = Enum.GetValues(typeof(ResGroup));
            return resGroupEnums.Cast<ResGroup>().FirstOrDefault(resGroup => bundleName.StartsWith(resGroup.ToString().ToLower()));
        }

        public List<string> GetAllDependencies(string bundleName)
        {
            var deps = new List<string>();
            GetDependenciesRecursive(bundleName, ref deps);
            return deps;
        }

        private void GetDependenciesRecursive(string bundleName, ref List<string> dependencies)
        {
            var resInfo = GetResInfo(bundleName);
            if (resInfo != null)
            {
                foreach (string dependency in resInfo.Dependencies)
                {
                    if (!dependencies.Contains(dependency)) //防止循环依赖
                    {
                        dependencies.Add(dependency);
                        GetDependenciesRecursive(dependency, ref dependencies);
                    }
                }
            }
        }

        public List<string> GetDirectDependencies(string bundleName)
        {
            var resInfo = GetResInfo(bundleName);
            if (resInfo != null)
            {
                return new List<string>(resInfo.Dependencies);
            }
            return null;
        }

        public void SaveFile(string path, bool compress)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
            string ver = "ver1";
            binaryWriter.Write(ver);
            binaryWriter.Write(Version);
            binaryWriter.Write(lz4CRC);
            binaryWriter.Write(lzmaCRC);
            binaryWriter.Write(tileTexCRC);
            binaryWriter.Write(BuildTime);
            binaryWriter.Write((int)compressType);
            binaryWriter.Write(TotalFileSize);
            binaryWriter.Write(isMiniRes);
            binaryWriter.Write(Manifest.Count);

            foreach (KeyValuePair<string, ResInfo> keyValuePair in Manifest)
            {
                ResInfo info = keyValuePair.Value;
                binaryWriter.Write(info.bundleName);
                binaryWriter.Write(info.CRC);
                binaryWriter.Write(info.Hash ?? string.Empty);
                binaryWriter.Write((int)info.remoteZipType);
                binaryWriter.Write(info.MD5);
                binaryWriter.Write(info.size);
                binaryWriter.Write(info.isPackageRes);
                binaryWriter.Write(info.preload);
                binaryWriter.Write(info.Dependencies.Count);
                for (int j = 0; j < info.Dependencies.Count; j++)
                {
                    binaryWriter.Write(info.Dependencies[j]);
                }
            }

            if (compress)
            {
                byte[] bytes = ZipLibUtils.Compress(memoryStream.ToArray());
                FileHelper.WriteAllBytes(path, bytes);
            }
            else
            {
                FileHelper.WriteAllBytes(path, memoryStream.ToArray());
            }

        }

        public static ResConfig ReadFile(byte[] bytes, bool isComporess)
        {
            if (bytes == null || bytes.Length == 0)
                return null;
            if (isComporess)
            {
                bytes = ZipLibUtils.Uncompress(bytes);
            }
            MemoryStream memoryStream = new MemoryStream(bytes, false);
            memoryStream.Position = 0;
            BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8);
            try
            {
                ResConfig config = new ResConfig();
                string ver = binaryReader.ReadString();
                config.Version = binaryReader.ReadInt64();
                config.lz4CRC = binaryReader.ReadUInt32();
                config.lzmaCRC = binaryReader.ReadUInt32();
                config.tileTexCRC = binaryReader.ReadUInt32();
                config.BuildTime = binaryReader.ReadInt64();
                config.compressType = (CompressType)binaryReader.ReadInt32();
                config.TotalFileSize = binaryReader.ReadInt64();
                config.isMiniRes = binaryReader.ReadBoolean();
                int resCount = binaryReader.ReadInt32();
                config.Manifest = new Dictionary<string, ResInfo>(resCount);

                for (int i = 0; i < resCount; i++)
                {
                    ResInfo info = new ResInfo();
                    info.bundleName = binaryReader.ReadString();
                    info.CRC = binaryReader.ReadUInt32();
                    info.Hash = binaryReader.ReadString();
                    info.remoteZipType = (CompressType)binaryReader.ReadInt32();
                    info.MD5 = binaryReader.ReadString();
                    info.size = binaryReader.ReadInt64();
                    info.isPackageRes = binaryReader.ReadBoolean();
                    info.preload = binaryReader.ReadBoolean();
                    int dependCount = binaryReader.ReadInt32();
                    info.Dependencies = new List<string>(dependCount);
                    for (int j = 0; j < dependCount; j++)
                    {
                        info.Dependencies.Add(binaryReader.ReadString());
                    }
                    config.Manifest.Add(info.bundleName, info);
                }
                return config;

            }
            catch (Exception ex)
            {
                CSGameDebuger.LogError(ex);
            }
            return null;
        }

        //检查依赖自引用
        public void CheckSelfDependencies()
        {
            foreach (var item in Manifest)
            {
                string name = item.Key;
                ResInfo resInfo = item.Value;
                List<string> list = new List<string>();
                for (int i = 0; i < resInfo.Dependencies.Count; i++)
                {
                    string bundleName = resInfo.Dependencies[i];
                    GetDependenciesRecursive(bundleName, ref list);
                }
                foreach (string dependName in list)
                {
                    if (dependName == name)
                    {
                        StringBuilder sb = new StringBuilder("检查Assetbundle引用错误: " + name);
                        sb.AppendLine();
                        sb.Append(name);
                        foreach (string str in list)
                        {
                            sb.Append("->" + str);
                        }
                        Debug.LogError(sb);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 游戏资源更新清单，每次资源更新时，先下载patchInfo来确认哪些资源需要更新，更新完资源之后，与本地ResConfig合并生成最新的版本资源信息
    /// </summary>
    public class ResPatchInfo
    {
        //当前版本号
        public long CurVer;
        public uint CurLz4CRC;
        public uint CurLzmaCRC;
        public uint CurTexCRC;
        //升级后的最终版本号
        public long EndVer;
        public uint EndLz4CRC;
        public uint EndLzmaCRC;
        public uint EndTexCRC;
        //记录了AssetBundle文件的总大小（单位：byte）
        public long TotalFileSize;
        //需要更新的文件列表
        public List<ResInfo> updateList;
        //需要清除的文件列表
        public List<string> removeList;

        public ResPatchInfo()
        {
            updateList = new List<ResInfo>();
            removeList = new List<string>();
        }

        public static string GetFileName(long curVer, long endVer)
        {
            return "patch_" + curVer + "_" + endVer + ".json";
        }

        //#if UNITY_EDITOR
        public string ToFileName()
        {
            return "patch_" + CurVer + "_" + EndVer + ".json";
        }
        //#endif
    }

    /// <summary>
    /// 小包资源配置,标记了ResConfig中哪些资源为包内资源,哪些资源为游戏时下载资源
    /// </summary>
    public class MiniResConfig
    {
        //存放小包缺失资源的Key,以及其替代资源信息
        public Dictionary<string, string> replaceResConfig = new Dictionary<string, string>();
    }
}
