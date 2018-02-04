using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model
{
    public class RoleSkillPotentialVO
    {
        public int id;
        public PotentialInfoDto infoDto;
        public Potential cfgVO;

        public string Name
        {
            get { return cfgVO.name; }
        }

        public int Level
        {
            get { return infoDto != null? infoDto.grade : 0; }
        }
    }
    public class RoleSkillPotentialData:IRoleSkillPotentialData
    {

        private Dictionary<int,RoleSkillPotentialVO> listData = new Dictionary<int, RoleSkillPotentialVO>();

        private Dictionary<int,int> limitList = new Dictionary<int, int>();
        private RoleSkillPotentialItem lastItem;

        public RoleSkillPotentialItem LastItem { get { return lastItem; } set { lastItem = value; } }
        private Dictionary<int, CharacterAbility> characterAbility = new Dictionary<int, CharacterAbility>();

        public int MaxLevel
        {
            get { return maxLevel; }
        }

        private int maxLevel;

        private int[] idList = new int[7] {0,1,2,3,4,5,6}; //从1开始

        private void InitData()
        {
            if(limitList.Count > 0) return;
            var faction = DataCache.getDtoByCls<Faction>(ModelManager.Player.FactionID);
            if(string.IsNullOrEmpty(faction.potentialLimits.ToString()) == false)
            {
                limitList = RoleSkillUtils.ParseAttr(faction.potentialLimits.ToString(),',',':');
            }
            maxLevel = DataCache.getArrayByCls<PotentialWealth>().Count;
            InitList();
        }

        public void InitList()
        {
            var cfgList = DataCache.getArrayByCls<Potential>();
            for(var i = 0;i< cfgList.Count;i++)
            {
                var vo = new RoleSkillPotentialVO() { id = cfgList[i].id,cfgVO = cfgList[i]};
                listData.Add(vo.id,vo);
            }
        }

        public void UpdateDto(DataList list)
        {
            InitData();
            list.items.ForEach(dto =>
            {
                var pDto = dto as PotentialInfoDto;
                var vo = listData[pDto.id];
                vo.infoDto = pDto;
            });
        }

        public void UpdateDtoByUpgrade(PotentialDto dto)
        {
            listData[dto.potentialInfoDto.id].infoDto = dto.potentialInfoDto;
        }

        public int GetIDByIndex(int index)
        {
            return idList[index];
        }

        

        //index:1,2,3,4,5,6
        public Potential GetPotentialByIndex(int index)
        {
            return GetPotentialByID(GetIDByIndex(index));
        }

        public Potential GetPotentialByID(int id)
        {
            InitData();
            return DataCache.getDtoByCls<Potential>(id);
        }

        public int GetLimitByID(int id)
        {
            InitData();
            return limitList.ContainsKey(id) ? limitList[id] : 0;
        }

        public Dictionary<int,int> LimitList
        {
            get
            {
                InitData();
                return limitList;
            }
        }
        public long GetCostByID(int id)
        {
            //获取下一级的cost
            var level = GetLevelByID(id)+1;
            if(level > maxLevel)
            {
                return -1;
            }else
            {
                return GetCostByLevel(level);
            }
        }

        public long GetCostByLevel(int level)
        {
            return DataCache.getDtoByCls<PotentialWealth>(level).wealth;
        }

        public RoleSkillPotentialVO GetVOByID(int id)
        {
            return listData.ContainsKey(id) ? listData[id] : null;
        }

        public int GetLevelByID(int id)
        {
            var dto = GetVOByID(id);
            if (dto == null) return 0;
            return dto.infoDto != null ? dto.infoDto.grade : 0;
        }

        public string GetEffectByLevel(RoleSkillPotentialVO vo,int level)
        {
            var msg = "";
            for(var i = 0;i < vo.cfgVO.addProperties.Count;i++)
            {
                var value = ExpressionManager.UpgradeSkillPotentialEffect(vo.cfgVO,vo.cfgVO.addProperties[i],vo.cfgVO.maths[i],level);
                var character = GetCharacterAbility();
                int characterID = vo.cfgVO.addProperties[i];
                string property = "";
                if (character.ContainsKey(characterID))
                {
                    property = character[characterID].name;
                }
                msg += string.Format("{0}+{1}\n", property, value.ToString());
            }
            return msg;
        }

        Dictionary<int,CharacterAbility> GetCharacterAbility()
        {
            if (characterAbility.Count == 0)
            {
                characterAbility = DataCache.getDicByCls<CharacterAbility>();
            }
            return characterAbility;
        }
    }
}
