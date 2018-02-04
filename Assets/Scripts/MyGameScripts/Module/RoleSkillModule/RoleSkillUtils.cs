using AppDto;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Assets.Scripts.MyGameScripts.Module.RoleSkillModule
{
    class RoleSkillUtils
    {
        public static Dictionary<int,int> ParseAttr(string value,char firstSep = '|',char secondSep = ',')
        {
            if(string.IsNullOrEmpty(value)) return null;
            var dic = new Dictionary<int, int>();
            var arr = value.Split(firstSep);
            foreach(var obj in arr)
            {
                var tmp = obj.Split(secondSep);
                dic[StringHelper.ToInt(tmp[0])] = StringHelper.ToInt(tmp[1]);
            }
            return dic;
        }

        public static string DictToString(Dictionary<int,int> dict,char firstSep = ',',char secondSep = ':')
        {
            var msg = "";
            foreach(var key in dict.Keys)
            {
                msg += key + firstSep + dict[key] + secondSep;
            }
            if(msg.Length > 0) msg = msg.Substring(0,msg.Length - 1);
            return msg;
        }

        public static string GetSkillEnumName(Skill.SkillEnum eNum)
        {
            return eNum == Skill.SkillEnum.Crafts ? "战技" : "魔法";
        }

        public static string GetSTDescEnumName(int stValue)
        {
            Skill.STDescEnum eNum;
            if(stValue < 200)
            {
                eNum = Skill.STDescEnum.Fastest;
            }
            else if(stValue < 400)
            {
                eNum = Skill.STDescEnum.Fast;
            }
            else if(stValue < 600)
            {
                eNum = Skill.STDescEnum.Normal;
            }
            else if(stValue < 800)
            {
                eNum = Skill.STDescEnum.Slow;
            }
            else 
            {
                eNum = Skill.STDescEnum.Slowest;
            }
            return GetSTDescEnumName(eNum);
        }

        private static string GetSTDescEnumName(Skill.STDescEnum eNum)
        {
            var str = "";
            switch(eNum)
            {
                case Skill.STDescEnum.Slowest:
                    str = "极慢";
                    break;
                case Skill.STDescEnum.Slow:
                    str = "较慢";
                    break;
                case Skill.STDescEnum.Normal:
                    str = "普通";
                    break;
                case Skill.STDescEnum.Fast:
                    str = "较快";
                    break;
                case Skill.STDescEnum.Fastest:
                    str = "极快";
                    break;
            }
            return str;
        }

        public static string GetElementPropertyTypeName(Skill.ElementPropertyType type)
        {
            var str = "";
            switch(type)
            {
                case Skill.ElementPropertyType.Fire:
                    str = "火";
                    break;
                case Skill.ElementPropertyType.Water:
                    str = "水";
                    break;
                case Skill.ElementPropertyType.LAND:
                    str = "土";
                    break;
                case Skill.ElementPropertyType.Wind:
                    str = "风";
                    break;
                case Skill.ElementPropertyType.Time:
                    str = "时";
                    break;
                case Skill.ElementPropertyType.DREAM:
                    str = "幻";
                    break;
                case Skill.ElementPropertyType.Sky:
                    str = "空";
                    break;
            }
            return str;
        }

        public static string GetSkillEffectTypeDes(Skill skill)
        {
            string str = "";
            var dic = DataCache.getDicByCls<Skill>();
            Crafts crafts = dic[skill.id] as Crafts;
            Magic magic = dic[skill.id] as Magic;
            if (crafts != null)
            {
                switch ((Crafts.SkillEffectTypeEnum)crafts.effectType)
                {
                    case Crafts.SkillEffectTypeEnum.Single:
                        str = "单体";
                        break;
                    case Crafts.SkillEffectTypeEnum.Multi:
                        str = "群体";
                        break;
                    case Crafts.SkillEffectTypeEnum.Recovery:
                        str = "回复";
                        break;
                    case Crafts.SkillEffectTypeEnum.Controll:
                        str = "控制";
                        break;
                    case Crafts.SkillEffectTypeEnum.Assist:
                        str = "辅助";
                        break;
                    case Crafts.SkillEffectTypeEnum.Defence:
                        str = "防御";
                        break;
                }
            }
            if (magic != null)
            {
                switch ((Magic.SkillEffectTypeEnum)magic.effectType)
                {
                    case Magic.SkillEffectTypeEnum.Single:
                        str = "单体";
                        break;
                    case Magic.SkillEffectTypeEnum.Multi:
                        str = "群体";
                        break;
                    case Magic.SkillEffectTypeEnum.Recovery:
                        str = "回复";
                        break;
                    case Magic.SkillEffectTypeEnum.Controll:
                        str = "控制";
                        break;
                    case Magic.SkillEffectTypeEnum.Assist:
                        str = "辅助";
                        break;
                    case Magic.SkillEffectTypeEnum.Defence:
                        str = "防御";
                        break;
                }
            }
            return str;
        }

        public static string Formula(string des,int lv)
        {
            string pattern = @"{.*?}";
            var matches = Regex.Matches(des, pattern);
            int idx = 0;
            foreach (Match m in matches)
            {
                string newValue = m.Value;
                string val = newValue.Substring(1, newValue.Length - 2);
                val = GetProperty(val, lv,idx);
                des = des.Replace(m.Value, val);
                idx++;
            }
            return des;
        }

        private static string GetProperty(string str,int lv,int idx)
        {
            return ExpressionManager.SkillPropertyDes(str, lv,idx).ToString();
        }
    }
}
