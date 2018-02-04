using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using LITJson;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

class Platform
{
    public static int ANDRIOD = 1;
	public static int IOS = 2;
}

class TestInPath
{
	public static string ANDRIOD_PATH_ALL = "/Docs/testin/andriod/andriod_all.csv";
	public static string ANDRIOD_PATH_PART = "/Docs/testin/andriod/andriod_part.csv";
	
	public static string IOS_PATH_ALL = "/Docs/testin/ios/ios_all.csv";
	public static string IOS_PATH_PART = "/Docs/testin/ios/ios_part.csv";
}


#region 云测辅助工具
public class Testin : EditorWindow
{
    //Http 控制
    private static MonoBehaviour _httpController = null;
	private List<TestInInfo> infoList;
	private CSTimer.TimerTask _infoListTimer;
	private CSTimer.TimerTask _timer;

    [MenuItem("Tools/Testin")]
    static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 500, 400);
        Testin window = (Testin)EditorWindow.GetWindowWithRect(typeof(Testin), wr, true, "Testin");
        window.Show();

        GameObject go = new GameObject();
        GameObject.DontDestroyOnLoad(go);
        go.name = "testinWWW";

        _httpController = go.AddComponent<MonoBehaviour>();
    }

    private static string reqUrlModel = "http://api.apm.testin.cn/excep/exceplist?app_key={0}&timestamp={1}&sign={2}&time_period={3}";
    private static string modifyUrlModel = "http://api.apm.testin.cn/excep/modify/excepsta?app_key={0}&timestamp={1}&sign={2}&idarry={3}&sta={4}";
	private static string stmDetailUrlModel = "http://api.apm.testin.cn/excep/detail?app_key={0}&timestamp={1}&sign={2}&crid={3}";
    private static string andriod_app_key = "675e18a4db1224c5971df10418e32165";
	private static string ios_app_key = "a31abe6c04670d39a21c0d4e898ee9af";
	private static string andriod_secret_key = "d43cb9044d0d02ef99b8f16044a0dc38";
	private static string ios_secret_key = "a26d7c6534043acdd79c6e59768afa4a";
	
    private int time_period = 3 * 24 * 60;//3天前

	private int platform = 1;
	private string app_key = "";
	private string secret_key = "";
   
    void Start()
    {
        
    }
    void OnGUI()
    {
		GUILayout.TextField("--------------Andriod---------------");
		GUILayout.TextField(String.Format("路径:{0}",TestInPath.ANDRIOD_PATH_ALL));

        if (GUILayout.Button("[所有]提交已修复的", GUILayout.Width(200)))
        {
			app_key = andriod_app_key;
			secret_key = andriod_secret_key;
			platform = Platform.ANDRIOD;
            UpdateJson(true);
        }

        if (GUILayout.Button("[所有]导出所有[提交后再导出]", GUILayout.Width(200)))
        {
			app_key = andriod_app_key;
			secret_key = andriod_secret_key;
			platform = Platform.ANDRIOD;
			_all = true;
			ClearTimer();
			ClearInfoListTimer();
			infoList = null;
			_infoListTimer = CSTimer.Instance.SetupTimer("TestInListTimer",TestInfoListTimer,3f);
			_httpController.StartCoroutine(RequestJson(_all));
        }

        GUILayout.TextField("--------------以下针对部分操作---------------");
		GUILayout.TextField(String.Format("路径:{0}",TestInPath.ANDRIOD_PATH_PART));

        if (GUILayout.Button("[部分]提交已修复的", GUILayout.Width(200)))
        {
			app_key = andriod_app_key;
			secret_key = andriod_secret_key;
			platform = Platform.ANDRIOD;
            UpdateJson(false);
        }

        if (GUILayout.Button("[部分]只导出未修复部分[提交后再导出]", GUILayout.Width(200)))
        {
			app_key = andriod_app_key;
			secret_key = andriod_secret_key;
			platform = Platform.ANDRIOD;
			_all = false;
			ClearTimer();
			ClearInfoListTimer();
			infoList = null;
			_infoListTimer = CSTimer.Instance.SetupTimer("TestInListTimer",TestInfoListTimer,3f);
			_httpController.StartCoroutine(RequestJson(_all));
        }


		GUILayout.TextField("");
		GUILayout.TextField("");
		GUILayout.TextField("");
		GUILayout.TextField("");
		
		GUILayout.TextField("--------------Ios---------------");
		GUILayout.TextField(String.Format("路径:{0}",TestInPath.IOS_PATH_ALL));
		
		if (GUILayout.Button("[所有]提交已修复的", GUILayout.Width(200)))
		{
			app_key = ios_app_key;
			secret_key = ios_secret_key;
			platform = Platform.IOS;
			UpdateJson(true);
		}
		
		if (GUILayout.Button("[所有]导出所有[提交后再导出]", GUILayout.Width(200)))
		{
			app_key = ios_app_key;
			secret_key = ios_secret_key;
			platform = Platform.IOS;
			_all = true;
			ClearTimer();
			ClearInfoListTimer();
			infoList = null;
			_infoListTimer = CSTimer.Instance.SetupTimer("TestInListTimer",TestInfoListTimer,3f);
			_httpController.StartCoroutine(RequestJson(_all));
		}
		
		GUILayout.TextField("--------------以下针对部分操作---------------");
		GUILayout.TextField(String.Format("路径:{0}",TestInPath.IOS_PATH_PART));
		
		if (GUILayout.Button("[部分]提交已修复的", GUILayout.Width(200)))
		{
			app_key = ios_app_key;
			secret_key = ios_secret_key;
			platform = Platform.IOS;
			UpdateJson(false);
		}
		
		if (GUILayout.Button("[部分]只导出未修复部分[提交后再导出]", GUILayout.Width(200)))
		{
			app_key = ios_app_key;
			secret_key = ios_secret_key;
			platform = Platform.IOS;
			_all = false;
			ClearTimer();
			ClearInfoListTimer();
			infoList = null;
			_infoListTimer = CSTimer.Instance.SetupTimer("TestInListTimer",TestInfoListTimer,3f);
			_httpController.StartCoroutine(RequestJson(_all));
		}
    }

	private void TestInfoListTimer()
	{
		if(infoList == null)
		{
			_httpController.StartCoroutine(RequestJson(_all));
			return;
		}
		ClearInfoListTimer();
	}

    IEnumerator RequestJson(bool all)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long timestamp = Convert.ToInt64(ts.TotalSeconds);
        string sign = MD5(app_key + timestamp + secret_key);
        string reqUrl = String.Format(reqUrlModel, app_key, timestamp, sign, time_period);
		Debug.Log("reqUrl: " + reqUrl);

        var w = new WWW(reqUrl);
        yield return w;
        Debug.Log("w.text: " + w.text);

        JsonData jd = JsonMapper.ToObject(w.text);
        string json = JsonMapper.ToJson(jd["data"]["list"]);
        AnalysisJson(json,all);
    }

    //判断是否已经修复
    private string getCrid(string content)
    {
        if (content.Substring(content.Length - 1, 1) == "1")
        {
            //查找ID
            char[] sep = { ',' };
            string[] arr = content.Split(sep);
            if (arr != null)
            {
                if (arr.Length > 0)
                {
                    Debug.Log("已修复: " + arr[0]);
                    return arr[0];
                }
            }
        }
        
        return null;
    }
	
	private Boolean _all = false;
    private void AnalysisJson(string str,bool all)
    {
		_all = all;
        Debug.Log("请求异常列表返回str: " + str);
		infoList = JsonMapper.ToObject<List<TestInInfo>>(str);
        infoList.Sort(delegate (TestInInfo r1, TestInInfo r2) { return r2.crs.CompareTo(r1.crs); });
       
		if(infoList.Count > 0)
		{
			ClearTimer();
			_timer = CSTimer.Instance.SetupTimer("TestInTimer",TestInTimer,1f);
		}
    }

	private void TestInTimer()
	{
		if(infoList != null && infoList.Count > 0)
		{
			for(int i = 0; i < infoList.Count; i++)
			{
				if(infoList[i].detail == "")
				{
					_httpController.StartCoroutine(RequestStmDetail(infoList[i].crid,_all));
					return;
				}
			}
		}
		ClearTimer();
		WriteFile(_all);
	}

	/// <summary>
	/// 请求异常信息
	/// </summary>
	/// <param name="crid">Crid.</param>
	IEnumerator RequestStmDetail(string crid,bool all)
	{
		Debug.Log("reqIndex： " + crid);
		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
		long timestamp = Convert.ToInt64(ts.TotalSeconds);
		string sign = MD5(app_key + timestamp + secret_key);
		string reqUrl = String.Format(stmDetailUrlModel, app_key, timestamp, sign, crid);
		Debug.Log("请求异常信息： " + reqUrl);
		
		var w = new WWW(reqUrl);
		yield return w;

		JsonData jd = JsonMapper.ToObject(w.text);
		string json = JsonMapper.ToJson(jd["data"]["list"]);
		Debug.Log("请求异常信息返回: " + w.text);
		List<TestStaInfo> stas = JsonMapper.ToObject<List<TestStaInfo>>(json);

		if(stas.Count > 0)
		{
			Debug.Log("请求异常信息单个: " + stas[0].sta);
		}

		if(infoList != null)
		{
			if(stas.Count > 0)
			{
				for(int i = 0; i < infoList.Count; i++)
				{
					if(infoList[i].crid == stas[0].crid)
					{
						infoList[i].detail = stas[0].sta;
					}
				}
			}
		}
	}

	/// <summary>
	/// 保存文件
	/// </summary>
	/// <param name="all">If set to <c>true</c> all.</param>
	private void WriteFile(bool all)
	{
		string path = "";
		if(platform == Platform.ANDRIOD)
		{
			if(all)
			{
				path = TestInPath.ANDRIOD_PATH_ALL;
			}else
			{
				path = TestInPath.ANDRIOD_PATH_PART;
			}
		}
		else if(platform == Platform.IOS)
		{
			if(all)
			{
				path = TestInPath.IOS_PATH_ALL;
			}else
			{
				path = TestInPath.IOS_PATH_PART;
			}
		}
		using (StreamWriter sw = new StreamWriter(Application.dataPath + path, false, Encoding.Default))
		{
			sw.WriteLine("id,版本,状态,摘要,异常信息,最后发生时间,首次发生时间,异常次数,影响用户数,是否修复[修复后置1]");
			TestInInfo info;
			for (int i = 0; i < infoList.Count; i++)
			{
				info = infoList[i];
				if(all || (!all && info.sta != 2))
				{
					sw.WriteLine(info.crid + "," + info.ver + "," + (info.sta == 2 ? "已修复" : "未修复") + "," + info.sumy.Replace(",", ".") + ",\""+info.detail+"\"," + getTestInDate(info.etm) + "," + getTestInDate(info.stm) + "," + info.crs.ToString() + "," + info.unum.ToString() + "," + "" + (info.sta == 2 ? 1 : 0).ToString() + "");
				}
			}
			sw.Flush();
			sw.Close();
		}

		EditorUtility.DisplayDialog("提示：", "导出完成！！！", "确定");
	}
   
    //请求json
    static private void RequestJson(string url,Action<string> downLoadFinishCallBack)
    {
        HttpController.Instance.DownLoad(url, delegate (ByteArray byteArray)
        {
			string json = byteArray.ToUTF8String();

            Debug.Log("testin json: " + json);

            downLoadFinishCallBack(json);
        }, null, delegate (Exception obj)
        {
            downLoadFinishCallBack(null);
            Debug.Log("testin json error: " + obj.ToString());
        }, false, SimpleWWW.ConnectionType.Short_Connect);
    }

    private void UpdateJson(Boolean all)
    {
        List<string> idarry = new List<string>();
		string path = "";
		if(platform == Platform.ANDRIOD)
		{
			if(all)
			{
				path = TestInPath.ANDRIOD_PATH_ALL;
			}else
			{
				path = TestInPath.ANDRIOD_PATH_PART;
			}
		}
		else if(platform == Platform.IOS)
		{
			if(all)
			{
				path = TestInPath.IOS_PATH_ALL;
			}else
			{
				path = TestInPath.IOS_PATH_PART;
			}
		}
        FileStream file = File.Open(Application.dataPath + path, FileMode.Open);
        StreamReader sr = new StreamReader(file);
        string strLine = sr.ReadLine();
        for (int i = 0; i < 1000; i++)
        {
            strLine = sr.ReadLine();
            if (String.IsNullOrEmpty(strLine))
            {
                sr.Close();
                break;
            }
            else
            {
                string crid = getCrid(strLine);
                if (String.IsNullOrEmpty(crid))
                {

                }
                else
                {
                    idarry.Add(crid);
                }
            }
        }
        sr.Close();

        UpdateModify(idarry);
    }

    //更新修复
    private void UpdateModify(List<string> ids,int sta = 2)
    {
        if(ids.Count <= 0)
        {
			EditorUtility.DisplayDialog("提示：", "没有可提交的修复", "确定");
            return;
        }

        string idsFormat = "";
        for (int i = 0; i < ids.Count; i++)
        {
            idsFormat = idsFormat + ids[i];
            if(i < (ids.Count-1))
            {
                idsFormat = idsFormat + ",";
            }
        }

        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long timestamp = Convert.ToInt64(ts.TotalSeconds);
        string sign = MD5(app_key + timestamp + secret_key);
        Debug.Log("提交修复的IDs:" + idsFormat);
		EditorUtility.DisplayDialog("提示：", "提交完成，条数:"+ids.Count.ToString(), "确定");

		string reqUrl = String.Format(modifyUrlModel, app_key, timestamp, sign, idsFormat,sta);
		Debug.Log("提交修复的reqUrl:" + reqUrl);
		_httpController.StartCoroutine(UpJson(reqUrl));
    }

    IEnumerator UpJson(string url)
    {
        var w = new WWW(url);
        yield return w;
    }

    public string MD5(string str)
    {
        //微软md5方法参考return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
        //byte[] b = Encoding.Default.GetBytes(str + MD5KEY);
        byte[] b = Encoding.Default.GetBytes(str);
        b = new MD5CryptoServiceProvider().ComputeHash(b);
        string ret = "";
        for (int i = 0; i < b.Length; i++)
            ret += b[i].ToString("x").PadLeft(2, '0');
        return ret;
    }

	private void ClearTimer()
	{
		if(_timer != null)
		{
			_timer.Cancel();
			_timer = null;
		}
	}

    private void ClearInfoListTimer()
	{
		if(_infoListTimer != null)
		{
			_infoListTimer.Cancel();
			_infoListTimer = null;
		}
	}

    public static string getTestInDate(long time)
    {
        DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = Int64.Parse(time.ToString() + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);

        DateTime dt = dateTimeStart.Add(toNow);

        // yyyy-MM-dd HH:mm:ss
        return String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
    }
}

public class TestInInfo
{
    public string crid;
    public string ver;
    public string sumy;
    public int sta;
    public int stm;
    public int crs;
    public int etm;
    public int unum;
	public string detail = "";//异常信息
}
public class TestStaInfo
{
	public string crid;
	public string sta;
	public string log;
}
#endregion