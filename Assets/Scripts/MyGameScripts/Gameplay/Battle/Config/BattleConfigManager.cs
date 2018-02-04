using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;

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
			if (asset != null) {
				TextAsset textAsset = asset as TextAsset;
				if (textAsset != null) {
					JsonBattleConfigInfo config = JsHelper.ToObject<JsonBattleConfigInfo> (textAsset.text);
					if (config != null) {
						_configDict.Clear ();
						for (int i=0,len=config.list.Count; i<len; i++)
						{
							JsonSkillConfigInfo info = config.list[i];
							if(_configDict.ContainsKey(info.id))
                                GameDebuger.LogError(string.Format("[错误]BattleConfig这个ID已存在，策划赶紧改下。id:{0},name:{1}",info.id,info.name));
							else
								_configDict.Add (info.id, info.ToSkillConfigInfo ());
						}
					}
				}
			}
		});
    }


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
