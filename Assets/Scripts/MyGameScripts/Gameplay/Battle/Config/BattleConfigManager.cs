using System;
using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;
using Fish;
using JsonC = Newtonsoft.Json.JsonConvert;

public sealed class BattleConfigManager
{
    private Dictionary<int, SkillConfigInfo> _configDict;
	
    private BattleConfigManager()
    {
		_configDict = new Dictionary<int, SkillConfigInfo> ();
    }
	
	private static readonly BattleConfigManager instance = new BattleConfigManager();
    public static BattleConfigManager Instance
    {
        get
		{
			return instance;
		}
    }

	private const string BattleConfig_ReadPath = "ConfigFiles/BattleConfig/BattleConfig";

    public void Setup()
    {
	    ResourcePoolManager.Instance.LoadConfig("BattleConfig", (asset) => {
		    if (asset == null) return;
		    var textAsset = asset as TextAsset;
		    if (textAsset == null) return;
		    var cfgJsonStr = textAsset.text;
		    //TODO fish: change to new config class CorrectBattleConfigInfo
		    if (cfgJsonStr.Contains("$type"))
		    {
			    var newCfg = JsonC.DeserializeObject<BattleConfigInfo>(cfgJsonStr);
			    if (newCfg == null) return;
			    _configDict.Clear();
			    newCfg.list.Sort((x, y) => (x.id - y.id));
			    newCfg.list.ForEach(info =>
			    {
				    if(_configDict.ContainsKey(info.id))
					    GameDebuger.LogError(string.Format("[错误]BattleConfig这个ID已存在，策划赶紧改下。id:{0},name:{1}",info.id,info.name));
				    else{
					    _configDict.Add (info.id, info);
				    }				    
			    });
			    return;
		    }
		    var config = JsonC.DeserializeObject<JsonBattleConfigInfo> (cfgJsonStr);
		    if (config == null) return;
		    _configDict.Clear ();

		    var list = config.list.ToArray();
		    Array.Sort(list, (x, y) => (x.id - y.id)); 

		    //SkillConfigInfo tSkillConfigInfo = null;
		    list.ForEach(info =>
		    {
			    if(_configDict.ContainsKey(info.id))
				    GameDebuger.LogError(string.Format("[错误]BattleConfig这个ID已存在，策划赶紧改下。id:{0},name:{1}",info.id,info.name));
			    else{
				    //tSkillConfigInfo = info.ToSkillConfigInfo();
//                                SortSkillConfigInfoByTime(tSkillConfigInfo);
				    _configDict.Add (info.id, info.ToSkillConfigInfo ());
			    }
		    });
	    });
	    
#if UNITY_EDITOR
	    _correctCfg = OldBattleConfigConverter.LoadConvertedBattleConfig();
#endif
    }
	
#if UNITY_EDITOR
	private static Dictionary<int,CorrectSkillConfig> _correctCfg = new Dictionary<int, CorrectSkillConfig>();
	public CorrectSkillConfig GetCorrectConfig(int skillId)
	{
		CorrectSkillConfig result;
		_correctCfg.TryGetValue(skillId, out result);
		return result;
	}
#endif
	

	public SkillConfigInfo getSkillConfigInfo(int skillID)
	{
		int key = skillID;
		
		if (_configDict == null)
		{
			return null;
		}
		
		SkillConfigInfo skillConfigInfo = null;
		
		_configDict.TryGetValue( key, out skillConfigInfo );

		return skillConfigInfo;
	}

    public bool UpdateSkillConfigInfo(SkillConfigInfo pSkillConfigInfo)
    {
        if (null == pSkillConfigInfo || pSkillConfigInfo.id <= 0)
            return false;
        SkillConfigInfo tSkillConfigInfo = null;
        if (_configDict.TryGetValue(pSkillConfigInfo.id, out tSkillConfigInfo))
            _configDict[pSkillConfigInfo.id] = pSkillConfigInfo;
        else
            _configDict.Add(pSkillConfigInfo.id,pSkillConfigInfo);
        return true;
    }
}
