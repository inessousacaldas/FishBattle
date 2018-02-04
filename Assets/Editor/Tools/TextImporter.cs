///----------------------------------------------------------------------------------------
///            		TextImporter
///	Description:脚本编码转换工具,对新建脚本和导入脚本
///				能自动进行编码的转换。
///	Author：明轩
///	Note：个别情况会报警告某个路径不存在，忽略即可（因为目录里是空的或者目录名字带个‘.’ 点）！
///----------------------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

using Object = UnityEngine.Object;

public class TextImporter : AssetPostprocessor 
{	
	/// <summary>
	/// 自动检测脚本文件并转换编码为：UTF-8 +BOM
	/// </summary>
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{				
		string dic = Directory.GetCurrentDirectory();
		//脚本处理
		foreach(string importedAsset in importedAssets)
		{
			string file = dic + "/" + importedAsset;
			SetFileFormatToUTF8_BOM(file);
		}
	}
	
	/// <summary>
	/// 将工程所有脚本编码转换为：UTF-8 +BOM
	/// </summary>
	[MenuItem("Tools/Encoding/UTF-8 +BOM All")]
	public static void SetAllScriptsToUTF8_BOM()
	{
		if (EditorUtility.DisplayDialog ("转换确认", "是否确认将工程所有脚本编码转换为：UTF-8 +BOM?", "确认", "取消")) 
		{
			string folder = Application.dataPath + "/";		
			SetFolderFormatToUTF8_BOM(folder);
		}
	}
	
	/// <summary>
	/// 将选中脚本编码转换为：UTF-8 +BOM
	/// </summary>
	[MenuItem("Tools/Encoding/UTF-8 +BOM Selected")]
	public static void SetSelectedScriptsToUTF8_BOM()
	{
		object[] objs = Selection.objects;
		if(objs.Length == 0)
		{
			Debug.Log("请选中脚本Shader或者Txt再进行转换！");
			return;
		}
		
		string dic = Directory.GetCurrentDirectory();
		foreach(Object obj in objs)
		{
			string file = dic + "/" + AssetDatabase.GetAssetPath(obj);
			SetFileFormatToUTF8_BOM(file);
		}
	}
	
	/// <summary>
	/// 将指定文件编码转换为：UTF-8 +BOM
	/// </summary>
	public static void SetFileFormatToUTF8_BOM(string file)
	{
        string extension = Path.GetExtension(file);
        if (extension == "" || extension == "\n")//传过来的是文件夹就不处理任何事
        {
            return;
        }
        else
        {
            if (!File.Exists(file))
            {
                Debug.LogWarning(string.Format("不存在文件：{0}", file));
                return;
            }
        }
       
		//仙灵项目忽略对这个服务器生产代码的处理
		if (file.Contains("app-clientservice"))
		{
			return;
		}

		//if (extension == ".cs" || extension == ".js" || extension == ".boo" || extension == ".shader" || extension == ".txt" || extension == ".rsp")		
		if (extension == ".cs")
		{	
			//先检测文件编码格式，防止无用刷新
			//先判断BOM模式，防止无用检测
			bool isUTF8_BOM = FileEncoding.isUTF8_BOM(file);	
			Encoding fileEncoding = null;
			if(extension != ".shader")
			{
				if(isUTF8_BOM)
					return;
			}
			//shader脚本不添加签名，因为内置shader编译器暂不支持带签名的UTF8脚本
			else if(!isUTF8_BOM)
			{
				fileEncoding = FileEncoding.GetType(file);
				if(fileEncoding == Encoding.UTF8)
					return;
			}
			
			//根据具体编码格式读出内容，再设置对象编码，防止出现乱码
			if(fileEncoding == null)
				fileEncoding = FileEncoding.GetType(file);
			UTF8Encoding utf8 = new UTF8Encoding((extension != ".shader"));	
			File.WriteAllText(file, File.ReadAllText(file, fileEncoding), utf8);
			Debug.Log("SetFileFormatToUTF8_BOM file = " + file);
		}
	}
	
	/// <summary>
	/// 将指定文件夹下的所有脚本编码转换为：UTF-8 +BOM
	/// </summary>
	public static void SetFolderFormatToUTF8_BOM(string folder)
	{
		if(!Directory.Exists(folder))
		{
			Debug.LogWarning(string.Format("不存在文件夹：{0}", folder));
			return;
		}
		
		string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
			SetFileFormatToUTF8_BOM(file);
		}
	}
	
	/// <summary>
	/// 将文件转换为指定的编码类型
	/// </summary>
	public static void SetFileFormat(string file, Encoding encoding)
	{
		if(!File.Exists(file))
		{
			Debug.LogWarning(string.Format("不存在文件：{0}", file));
			return;
		}
		
		File.WriteAllText(file, File.ReadAllText(file, Encoding.Default), encoding);
	}
}

/// <summary>
/// 获取文件的编码格式
/// </summary>
public class FileEncoding
{		
	/// <summary>
	/// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
	/// </summary>
	/// <param name="FILE_NAME">文件路径</param>
	/// <returns>是否是带签名的UTF8编码</returns>
	public static bool isUTF8_BOM(string FILE_NAME)
	{
		FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
		BinaryReader r = new BinaryReader(fs, Encoding.Default);
		int i;
		int.TryParse(fs.Length.ToString(), out i);
		byte[] ss = r.ReadBytes(i);
		r.Close();
		fs.Close();
		
		return IsUTF8_BOMBytes(ss);
	}
	
	/// <summary>
	/// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
	/// </summary>
	/// <param name="FILE_NAME">文件路径</param>
	/// <returns>文件的编码类型</returns>
	public static Encoding GetType(string FILE_NAME)
	{
		FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
		Encoding r = GetType(fs);
		fs.Close();
		return r;
	}
	
	/// <summary>
 	/// 通过给定的文件流，判断文件的编码类型
 	/// </summary>
 	/// <param name="fs">文件流</param>
 	/// <returns>文件的编码类型</returns>
 	public static Encoding GetType(FileStream fs)
	{
//		byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
//		byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
//		byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
		Encoding reVal = Encoding.Default;
		
		BinaryReader r = new BinaryReader(fs, Encoding.Default);
		int i;
		int.TryParse(fs.Length.ToString(), out i);
		byte[] ss = r.ReadBytes(i);
		if (IsUTF8Bytes(ss) || IsUTF8_BOMBytes(ss))
		{
			reVal = Encoding.UTF8;
		}
		else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
		{
			reVal = Encoding.BigEndianUnicode;
		}
		else if (ss[0] == 0xFF && ss[1] == 0xFE)
		{
			reVal = Encoding.Unicode;
		}
		r.Close();
		
		return reVal;
	}
	
	/// <summary>
	/// 将文件格式转换为UTF-8-BOM
	/// </summary>
	/// <param name="FILE_NAME">文件路径</param>
	public static void CovertToUTF8_BOM(string FILE_NAME)
	{
		byte[] BomHeader = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
		FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.ReadWrite);
		
		//按默认编码获取文件内容
		BinaryReader r = new BinaryReader(fs, Encoding.Default);
		int i;
		int.TryParse(fs.Length.ToString(), out i);
		byte[] ss = r.ReadBytes(i);
		r.Close();
		
		bool isBom = false;
		if(ss.Length >= 3)
		{
			if(ss[0] == BomHeader[0] && ss[1] == BomHeader[1] && ss[2] == BomHeader[2])
			{
				isBom = true;
			}
		}
		
		//将内容转换为UTF8格式，并添加Bom头
		if(!isBom)
		{
			string content = Encoding.Default.GetString(ss);
			byte[] newSS = Encoding.UTF8.GetBytes(content);
			
			fs.Seek(0, SeekOrigin.Begin);
			fs.Write(BomHeader, 0, BomHeader.Length);
			fs.Write(newSS, 0, i);
		}	
		
		fs.Close();
	}
	
	/// <summary>
	/// 判断是否是不带 BOM 的 UTF8 格式
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	private static bool IsUTF8Bytes(byte[] data)
	{
		int charByteCounter = 1;	//计算当前正分析的字符应还有的字节数
		byte curByte; //当前分析的字节.
		for (int i = 0; i < data.Length; i++)
		{
			curByte = data[i];
			if (charByteCounter == 1)
			{
				if (curByte >= 0x80)
				{
					//判断当前
					while (((curByte <<= 1) & 0x80) != 0)
					{
						charByteCounter++;
					}
					//标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
					if (charByteCounter == 1 || charByteCounter > 6)
					{
						return false;
					}
				}
			}
			else
			{
				//若是UTF-8 此时第一位必须为1
				if ((curByte & 0xC0) != 0x80)
				{
					return false;
				}
				charByteCounter--;
			}
		}
		
		if (charByteCounter > 1)
		{
			Debug.LogError("非预期的byte格式");
		}
		
		return true;
	} 
	
	/// <summary>
	/// 判断是否是带 BOM 的 UTF8 格式
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	private static bool IsUTF8_BOMBytes(byte[] data)
	{
		if(data.Length < 3)
			return false;
		
		return ((data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF));
	}
}