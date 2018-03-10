// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 7/29/2017 10:26:04 AM
// **********************************************************************

using AppDto;
using System.Collections.Generic;

/// <summary>
/// 选项卡（指令，协议流程，协议收发）
/// </summary>
public enum GMDataTab
{
    Code
   , DtoConn
   , Dto
}

public class GMDataCodeVO
{
    public int id;
    public GmCode cfgVO;
    public string clientParam;//可能会动态改变参数列表

    public string FullCode
    {
        get { return string.IsNullOrEmpty(cfgVO.code) ? "" : cfgVO.code + " " + clientParam; }
    }
}

public interface IGMData
{
    GMDataTab MainTab { get; }
    int CodeTab { get; }
    Dictionary<string,List<GMDataCodeVO>> CodeDict { get; }
    Dictionary<int,string> CodeTabDict { get; }
    List<GMDataCodeVO> GetCodeListByIndex(int index);
    GMDataCodeVO CurCodeVO { get; }

    GMDataMgr.GMDtoConnData DtoConnData { get; }
}

public interface IGMDtoConnData
{

}

public sealed partial class GMDataMgr
{
    public sealed partial class GMData:IGMData
    {
        public GMDataTab mainTab = GMDataTab.Code;
        private GMDtoConnData dtoConnData;

        private const int MaxCodeCache = 20;
        public int codeTab = 0;
        public GMDataCodeVO curCodeVO;
        private Dictionary<int,string> codeTabDict = new Dictionary<int,string>();
        private Dictionary<string,List<GMDataCodeVO>> codeDict = new Dictionary<string, List<GMDataCodeVO>>();
        private List<GMDataCodeVO> codeCacheList = new List<GMDataCodeVO>();
        private string codeCacheStr;
        public GMData()
        {

        }

        public void InitData()
        {
            dtoConnData = new GMDtoConnData();
            dtoConnData.InitData();

            var cfgList = DataCache.getArrayByCls<GmCode>();
            var curIndex = 1;
            for(var i = 0;i < cfgList.Count;i++)
            {
                var cfgItem = cfgList[i];
                if(codeDict.ContainsKey(cfgItem.type) == false)
                {
                    codeDict[cfgItem.type] = new List<GMDataCodeVO>();
                    codeTabDict[curIndex] = cfgItem.type;
                    curIndex++;
                }
                var itemVO = new GMDataCodeVO() { id = cfgItem.id,cfgVO = cfgItem };
                itemVO.clientParam = itemVO.cfgVO.parm;
                codeDict[cfgItem.type].Add(itemVO);
            }
            RefreshCodeCache();
        }

        public GMDtoConnData DtoConnData { get{return dtoConnData;} }

        public void Dispose()
        {

        }

        public GMDataTab MainTab { get { return mainTab; } }

        #region GM指令
        public int CodeTab { get { return codeTab; } }

        public Dictionary<string,List<GMDataCodeVO>> CodeDict
        {
            get { return codeDict; }
        }

        public Dictionary<int,string> CodeTabDict
        {
            get { return codeTabDict; }
        }

        public List<GMDataCodeVO> GetCodeListByIndex(int index)
        {
            var type = GetCodeTypeByIndex(index);
            return codeDict.ContainsKey(type) ? codeDict[type] : null;
        }

        public string GetCodeTypeByIndex(int index)
        {
            return codeTabDict.ContainsKey(index) ? codeTabDict[index] : "全部";
        }

        public GMDataCodeVO CurCodeVO { get { return curCodeVO; } }

        public void CacheUserCode(GMDataCodeVO vo,string param)
        {
            int id = 0;
            try
            {
                id = GetCodeIDByParam(vo,param);
                if(id == -1) return;
                param = param.Substring(param.IndexOf(" ") + 1);
                var msg = string.Format("{0},{1}",id,param);

                for(var i = 0;i < codeCacheList.Count;i++)
                {
                    if(codeCacheList[i].id == id)
                    {
                        codeCacheList.RemoveAt(i);
                        break;
                    }
                }
                curCodeVO = new GMDataCodeVO() { id = id,clientParam = param };//要更新下当前的选中VO，因为本地数据会直接替换VO，内存地址会改变
                curCodeVO.cfgVO = DataCache.getDtoByCls<GmCode>(curCodeVO.id);
                codeCacheList.Insert(0,curCodeVO);
                if(codeCacheList.Count > MaxCodeCache)
                {
                    codeCacheList.RemoveAt(codeCacheList.Count - 1);
                }
                codeCacheStr = "";
                for(var i = 0;i < codeCacheList.Count;i++)
                {
                    codeCacheStr += string.Format("{0},{1}",codeCacheList[i].id,codeCacheList[i].clientParam) + "|";
                }
                codeCacheStr = codeCacheStr.Substring(0,codeCacheStr.Length - 1);
                UnityEngine.PlayerPrefs.SetString("GMCODE_UserCode",codeCacheStr);
            }catch(System.Exception ex)
            {
                GameLog.ShowError("CacheUserCode id:{0},param:{1},msg:{2}",id + "," + param + "," + ex.Message);
            }
            RefreshCodeCache();
        }

        private void RefreshCodeCache()
        {
            var type = "常用指令";
            if(codeDict.ContainsKey(type) == false)
            {
                codeDict[type] = new List<GMDataCodeVO>();
                codeTabDict[codeTabDict.Count+1] = type;
            }
            codeDict[type].Clear();
            codeCacheList.Clear();
            codeCacheStr = UnityEngine.PlayerPrefs.GetString("GMCODE_UserCode","");
            if(string.IsNullOrEmpty(codeCacheStr) == false)
            {
                var tempList = codeCacheStr.Split('|').ToList();
                for(var i = 0;i<tempList.Count;i++)
                {
                    var tempItemList = tempList[i].Split(',');
                    for(var j = 0;j < codeCacheList.Count;j++)
                    {
                        if(codeCacheList[j].id == StringHelper.ToInt(tempItemList[0]))
                        {
                            codeCacheList.RemoveAt(j);
                            break;
                        }
                    }
                    codeCacheList.Add(new GMDataCodeVO() { id = StringHelper.ToInt(tempItemList[0]),clientParam = tempItemList[1] });
                }
            }

            if(codeCacheList != null)
            {
                for(var i = 0;i < codeCacheList.Count;i++)
                {
                    var itemVO = codeCacheList[i];
                    itemVO.cfgVO = DataCache.getDtoByCls<GmCode>(itemVO.id);
                    codeDict[type].Add(itemVO);
                }
            }
        }

        private int GetCodeIDByParam(GMDataCodeVO vo, string param)
        {
            if(string.IsNullOrEmpty(param)) return -1;
            param = param.Substring(0,param.IndexOf(" ")).ToLower();
            if(param.Contains("call"))
            {
                foreach(var itemList in codeDict.Values)
                {
                    for(var i = 0;i < itemList.Count;i++)
                    {
                        if(itemList[i].cfgVO.parm.Contains("call)"))
                        {
                            return itemList[i].id;
                        }
                    }
                }
            }else
            {
                return vo != null && vo.cfgVO.code != param ? -1 : vo.id;//前缀不一样的就不替换了
            }
            return -1;
        }

        public bool IsGMCode(string code)
        {
            return code.Substring(0,1) == "#";
        }
        #endregion
    }
}
