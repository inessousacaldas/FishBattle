using AppDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model
{

    public class RoleSkillTalentItemVO
    {
        public int level;
        public List<RoleSkillTalentSingleItemVO> singleList;

    }

    public class RoleSkillTalentSingleItemVO
    {
        public int id;
        public int limitLv;
        public int index = -1;
        public TalentGradeDto gradeDto;
        public Talent cfgVO;

        public string Name
        {
            get { return cfgVO.name; }
        }

        public int Grade
        {
            get { return gradeDto != null ? gradeDto.grade : 0; }
        }
    }

    public class RoleSkillTalentData:IRoleSkillTalentData
    {
        public bool talentMax;
        public RoleSkillTalentSingleItemController lastItem;

        private Dictionary<int,RoleSkillTalentItemVO> listItem = new Dictionary<int,RoleSkillTalentItemVO>();//key等级，value对应的天赋list
        private Dictionary<int,Talent> cfgDict = new Dictionary<int, Talent>();//配置数据
        private Dictionary<int,TalentGradeDto> gradeDtoList = new Dictionary<int, TalentGradeDto>();//服务端数据
        private List<int> idList;//当前职业拥有的天赋，带顺序
        private List<int> recommendList;//当前职业推荐的天赋

        public void InitData()
        {
            var player = ModelManager.Player.GetPlayer();
            if (player.factionId <= 0)
            {
                GameDebuger.Log("player.factionId");
                return;
            }

            idList = player.faction.talents;
            recommendList = player.faction.recommendTalents;
            cfgDict = DataCache.getDicByCls<Talent>();

            var dto = DataCache.getDtoByCls<Faction>(player.factionId);
            if (dto == null)
            {
                return;
            }

            var cfgDictLevel = new Dictionary<int,List<int>>();
            dto.talents.ForEach(tID =>
            {
                var talent = DataCache.getDtoByCls<Talent>(tID);
                if (talent == null) return;

                if(cfgDictLevel.ContainsKey(talent.playerGradeLimit) == false)
                {
                    cfgDictLevel[talent.playerGradeLimit] = new List<int>();
                }
                cfgDictLevel[talent.playerGradeLimit].Add(talent.id);
            });


            foreach(var levelItemList in cfgDictLevel.Values)
            {
                levelItemList.Sort(SortListItem);
            }
            cfgDictLevel.ForEach(e =>
            {
                var vo = new RoleSkillTalentItemVO() { level = e.Key };
                var levelImteList = e.Value;
                vo.singleList = new List<RoleSkillTalentSingleItemVO>();
                for(int k =0,max = levelImteList.Count; k < max; k++)
                {
                    vo.singleList.Add(new RoleSkillTalentSingleItemVO() { id = levelImteList[k], cfgVO = GetCfgVOById(levelImteList[k]), limitLv = vo.level });
                }
                listItem[vo.level] = vo;
            });
        }

        private int SortListItem(int t1,int t2)
        {
            if(idList.IndexOf(t1) < idList.IndexOf(t2))
            {
                return -1;
            }else if(idList.IndexOf(t1) > idList.IndexOf(t2))
            {
                return 1;
            }
            return 0;
        }

        public void UpdateDto(TalentDto infoDto)
        {
            if(infoDto.talentGradeDtos != null)
            {
                for(var i = 0;i < infoDto.talentGradeDtos.Count;i++)
                {
                    var gradeDto = infoDto.talentGradeDtos[i];
                    UpdateDtoSingle(gradeDto);
                }
            }
        }

        public void UpdateDataReset()
        {
            listItem.ForEach(e =>
            {
                e.Value.singleList.ForEach(g =>
                {
                    g.gradeDto = null;
                });
            });
            gradeDtoList.Clear();
        }
        public void UpdateDtoSingle(TalentGradeDto gradeDto)
        {
            if(gradeDto != null)
            {
                gradeDtoList[gradeDto.talentId] = gradeDto;
                var cfgVO = GetCfgVOById(gradeDto.talentId);
                var list = listItem[cfgVO.playerGradeLimit].singleList;
                list.ForEach(e =>
                {
                    if (e.id == gradeDto.talentId)
                    {
                        e.gradeDto = gradeDto;
                    }
                });
                //listItem[cfgVO.playerGradeLimit].singleList[gradeDto.talentId].gradeDto = gradeDto;
            }
        }

        public Dictionary<int,RoleSkillTalentItemVO> ListItem
        {
            get { return listItem; }
        }

        public List<int> RecommendList
        {
            get { return recommendList; }
        }

        public Talent GetCfgVOById(int id)
        {
            return cfgDict.ContainsKey(id) ? cfgDict[id] : null;
        }

        public TalentGradeDto GetDtoById(int id)
        {
            return gradeDtoList.ContainsKey(id) ? gradeDtoList[id] : null;
        }

        public int GetAssignedPoint()
        {
            var point = 0;
            if(gradeDtoList != null)
            {
                gradeDtoList.ForEach(e =>
                {
                    point += e.Value.grade;
                });
            }
            return point;
        }

        //已分配和未分配的加起来
        public int GetTotalPoint()
        {
            return GetAssignedPoint() + (int)ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.TALENTPOINT);
        }

        public int GetNeedAddPointByOpen(int playerGradeLimit)
        {
            var formula = DataCache.GetStaticConfigValues(AppStaticConfigs.TALENT_NEED_ADDPOINT_FORMULA);
            return ExpressionManager.UpgradeSkillTalentLevelOpen(formula,playerGradeLimit);
        }

        public bool CheckTalentOpenByLevel(RoleSkillTalentSingleItemVO vo)
        {
            var needAddPointByOpen = GetNeedAddPointByOpen(vo.cfgVO.playerGradeLimit);
            return GetAssignedPoint()  - needAddPointByOpen >= 0;
        }

        public bool CheckTalentOpenByBefore(RoleSkillTalentSingleItemVO vo)
        {
            var beforTalentVO = GetDtoById(vo.cfgVO.beforeTalentId);
            var beforTalentCfgVO = GetCfgVOById(vo.cfgVO.beforeTalentId);
            return beforTalentVO != null ? beforTalentVO.grade == beforTalentCfgVO.maxGrade : true;
        }

        public RoleSkillTalentSingleItemController LastItem { get { return lastItem; } }

        public void Dispose()
        {
            lastItem = null;
        }
    }
}
