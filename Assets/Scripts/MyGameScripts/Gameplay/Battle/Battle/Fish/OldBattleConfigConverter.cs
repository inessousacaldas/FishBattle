using System;
using System.Collections.Generic;
using System.IO;
using JsonC = Newtonsoft.Json.JsonConvert;

namespace Fish
{
    public static class OldBattleConfigConverter
    {
        public const string BattleConfig_Path = "Assets/GameResources/ConfigFiles/BattleConfig/BattleConfig.bytes";
        public const string ConvertedBattleConfig_Path = "Assets/GameResources/ConfigFiles/BattleConfig/ConvertedBattleConfig.json";

        public static Dictionary<int,CorrectSkillConfig> LoadConvertedBattleConfig()
        {
            //TODO fish: to be deleted if new battle system is ready
            if (!File.Exists(ConvertedBattleConfig_Path))
            {
                ReformatedOldConfig();
                ConvertOldAndSave();
            }
            
            var jsonTxt = File.ReadAllText(ConvertedBattleConfig_Path);
            var newCfgList = JsonC.DeserializeObject<CorrectBattleConfigInfo>(jsonTxt);
            var result = new Dictionary<int,CorrectSkillConfig>(newCfgList.list.Count+1);
            var hasDefault = false;
            foreach (var cfg in newCfgList.list)
            {
                result.Add(cfg.id,cfg);
                if (cfg.id == 0)
                    hasDefault = true;
            }

            if (!hasDefault)
            {
                result.Add(0,new CorrectSkillConfig());
            }

            return result;
        }

        public static void ConvertOldAndSave()
        {
            //var jsonTxt = File.ReadAllText("ReformattedBattleConfig.json");
            var jsonTxt = File.ReadAllText(BattleConfig_Path);
            var old = JsonC.DeserializeObject<BattleConfigInfo>(jsonTxt);
            ConvertOldAndSave(old);
        }

        public static void ConvertOldAndSave(BattleConfigInfo old)
        {
            var converted = new CorrectBattleConfigInfo {time = old.time};
            converted.list = FromOld(old.list);
            var serializeObject = converted.ToBattleJsonStr();
            File.WriteAllText(ConvertedBattleConfig_Path, serializeObject);
        }

        public static void ReformatedOldConfig()
        {
            var cfgJsonStr = System.IO.File.ReadAllText(BattleConfig_Path);
            if (!cfgJsonStr.Contains("$type"))//reformatted json contain $type
            {
                var json = JsonC.DeserializeObject<JsonBattleConfigInfo>(cfgJsonStr);
                var reformated = new BattleConfigInfo
                {
                    time = json.time,
                    list = new List<SkillConfigInfo>(json.list.Count)
                };
                foreach (var jsonSkillConfigInfo in json.list)
                {
                    reformated.list.Add(jsonSkillConfigInfo.ToSkillConfigInfo());
                }
                reformated.list.Sort((x, y) => (x.id - y.id));
                var reformatedJson = reformated.ToBattleJsonStr();
                File.WriteAllText(BattleConfig_Path,reformatedJson);
            }
        }
        
        public static List<CorrectSkillConfig> FromOld(List<SkillConfigInfo> oldLst)
        {
            var result=new List<CorrectSkillConfig>();
            foreach (var oldCfg in oldLst)
            {
                var newCfg = FromOld(oldCfg);
                if (newCfg == null) continue;
                result.Add(newCfg);
            }
            return result;
        }

        /*public static List<CorrectSkillConfig> FromOldNormalAttack(List<SkillConfigInfo> oldLst)
        {
            var result=new List<CorrectSkillConfig>();
            foreach (var oldCfg in oldLst)
            {
                if (oldCfg.name!="普攻") continue;
                var newCfg = FromOld(oldCfg);
                if (newCfg == null) continue;
                result.Add(newCfg);
            }

            return result;
        }
        
        public static void ConvertAndSaveNormalAttack()
        {
            var jsonTxt = File.ReadAllText("ReformattedBattleConfig.json");
            var old = JsonC.DeserializeObject<BattleConfigInfo>(jsonTxt);
            var converted = new CorrectBattleConfigInfo {time = old.time};
            converted.list = FromOldNormalAttack(old.list);
            var serializeObject = converted.ToBattleJsonStr();
            File.WriteAllText("ConvertedNormalAttackConfig.json", serializeObject);
        }*/
        
        public static CorrectSkillConfig FromOld(SkillConfigInfo copy)
        {
            if (null == copy || copy.id <= 0)
                return null;
            var oldCfg = copy.DeepCopy();
            var injuredPhrase = Convert(oldCfg.injurerActions);
            var attackPhrase = Convert(oldCfg.attackerActions,injuredPhrase);
            if (attackPhrase == null) return null;
            
            var result = new CorrectSkillConfig
            {
                id = oldCfg.id,
                name = oldCfg.name,
                battlePhrase = attackPhrase
            };
            return result;
        }

        private static BattlePhraseBase Convert(List<BaseActionInfo> attackerList, BattlePhraseBase injuredPhrase)
        {
            var inserted = injuredPhrase;
            var lst = new BattlePhraseBase[attackerList.Count];
            for (var i = 0; i < attackerList.Count; i++)
            {
                var actInfo = attackerList[i];
                var removeIndex = -1;
                for (var index = 0; index < actInfo.effects.Count; index++)
                {
                    var eff = actInfo.effects[index];
                    eff.type = null;
                    if (eff is TakeDamageEffectInfo)
                    {
                        removeIndex = index;
                    }
                }

                if (removeIndex >= 0)
                {
                    actInfo.effects.RemoveAt(removeIndex);
                }

                actInfo.initiator = ActionInitiator.Attacker;
                FixTime(actInfo);
                lst[i]=ActionPhrase.Create(actInfo);
                if (removeIndex >= 0)
                {
                    lst[i] = lst[i].Parall(inserted);
                    inserted = null;
                }
            }

            return lst.ToSeq().Parall(inserted);
        }

        private static void FixTime(BaseActionInfo actInfo)
        {
            actInfo.type = null;
            /*var move = actInfo as MoveActionInfo;
            if (move != null && Math.Abs(move.time) < float.Epsilon)
            {
                move.time = 1f;
            }

            var moveback = actInfo as MoveBackActionInfo;
            if (moveback != null && Math.Abs(moveback.time) < float.Epsilon)
            {
                moveback.time = 1f;
            }*/
        }

        private static BattlePhraseBase Convert(List<BaseActionInfo> injuredList)
        {
            var lst = new BattlePhraseBase[injuredList.Count];
            for (var i = 0; i < injuredList.Count; i++)
            {
                var info = injuredList[i];
                info.initiator = ActionInitiator.Victim;
                foreach (var eff in info.effects)
                {
                    eff.type = null;
                }
                FixTime(info);
                lst[i]=ActionPhrase.Create(info);
            }
            return lst.ToSeq();
        }

        public static List<CorrectSkillConfig> LoadConvertedBattleConfigList()
        {
            //TODO fish: to be deleted if new battle system is ready
            if (!File.Exists(ConvertedBattleConfig_Path))
            {
                ReformatedOldConfig();
                ConvertOldAndSave();
            }
            
            var jsonTxt = File.ReadAllText(ConvertedBattleConfig_Path);
            var newCfgList = JsonC.DeserializeObject<CorrectBattleConfigInfo>(jsonTxt);
            var result = newCfgList.list;
            var hasDefault = false;
            foreach (var cfg in newCfgList.list)
            {
                if (cfg.id == 0)
                    hasDefault = true;
            }

            if (!hasDefault)
            {
                result.Add(new CorrectSkillConfig());
            }

            result.Sort((x,y)=>x.id-y.id);
            return result;
        }

        public static void SaveCorrectBattleConfigInfo(List<CorrectSkillConfig> skillInfos)
        {
            var cfg = new CorrectBattleConfigInfo();
            cfg.time=(DateTime.UtcNow.Ticks / 10000).ToString();
            cfg.list = skillInfos;
            var serializeObject = cfg.ToBattleJsonStr();
            File.WriteAllText(ConvertedBattleConfig_Path, serializeObject);
        }

        public static CorrectSkillConfig DeepCopySkillInfo(CorrectSkillConfig info)
        {
            var tmp=JsonC.SerializeObject(info);
            return JsonC.DeserializeObject<CorrectSkillConfig>(tmp);
        }
    }
}