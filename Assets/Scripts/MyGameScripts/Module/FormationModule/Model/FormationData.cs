//using System;
//using System.Collections.Generic;
//using AppDto;
//using UnityEngine;
//
public interface IFormationData
{
    
}
//public interface IFormationData
//{
//    Formation CurFormation { get; }
//    int curLevel { get; }
//    int ActiveFormationID { get; }
//    FormationState GetFormationState(int id);
//    Formation GetFormationByIdx(int i);
//    modelStyleInfo GetModelDataByIdx(int posKey);
//    Tuple<string, int> GetActivatedFormationNameAndLev();
//}
//
//public sealed partial class FormationDataMgr
//{
//    public sealed partial class FormationData : IFormationData
//    {
//        public FormationData()
//        {
//
//        }
//
//        public void Dispose()
//        {
//            _positiveEffectDict.Clear();
//            _negativeEffectDict.Clear();
//        }
//
//        #region static data
//
//        public static Comparison<Formation> _comparison = null;
//        //<阵法id,<克制值,克制阵法Id列表>>
//        public static Dictionary<int, Dictionary<int, List<int>>> _positiveEffectDict;
//        //<阵法id,<被克值,被克阵法Id列表>>
//        public static Dictionary<int,Dictionary<int, List<int>>> _negativeEffectDict;
//
//        #endregion
//
//        #region Client Data
//
//        public Formation _curFormation = null;
//
//        public Formation CurFormation
//        {
//            set { _curFormation = value; }
//            get { return _curFormation; }
//        }
//
//        private int _curLevel = 1;
//
//        //当前预览等级
//        public int curLevel
//        {
//            get { return _curLevel; }
//            set { _curLevel = value; }
//        }
//
//        public int activeFormationID = (int)Formation.FormationType.Regular;
//
//        public int ActiveFormationID
//        {
//            //ModelManager.Team.GetFormationId();
//            get { return activeFormationID; }
//        }
//
//        public FormationState GetFormationState(int id)
//        {
//            if (id == (int)Formation.FormationType.Regular)
//                return FormationState.Learned;
//            else
//            {
//                var info = acquiredFormation.Find<FormationInfoDto>(s => s.id == id);
//                if (info != null)
//                    return FormationState.Learned;
//                else
//                {
//                    // todo fish:判断是否有道具
//                    return FormationState.UnEnable;
//                }
//            }
//        }
//
//        public Formation GetFormationByIdx(int i)
//        {
//            return allFormationList.TryGetValue(i);
//        }
//
//        public modelStyleInfo GetModelDataByIdx(int posKey)
//        {
//            return ModelStyleInfoWrapper.CreateModelStyleInfo(ModelManager.Player); // temp code , todo fish
//            if (TeamDataMgr.DataMgr.HasTeam())
//            {
//                return TeamDataMgr.DataMgr.GetMemberByFormationPos(posKey);
//            }
//            else
//            {
//                return ModelManager.Player.GetMemberByFormationPos(posKey);
//            }
//        }
//
//        public Tuple<string, int> GetActivatedFormationNameAndLev(){
//            var f = allFormationList.Find<Formation>(s=>s.id ==ActiveFormationID);
//            if (f == null)
//                return null;
//            
//            if (ActiveFormationID == (int)Formation.FormationType.Regular)
//            {
//                return Tuple.Create<string, int>(f.name, 1);
//            }
//            else
//            {
//                var lev = acquiredFormation.Find<FormationInfoDto>(s => s.id == ActiveFormationID).level;
//                return Tuple.Create<string, int>(f.name, lev);
//            }
//        } 
//
//        #endregion
//
//        #region serverData
//
//        public List<Formation> allFormationList;
//        public List<object> teamMembers;
//        #endregion
//
//        //掌握的阵法
//        private List<FormationInfoDto> _acquiredFormation;
//
//        public List<FormationInfoDto> acquiredFormation
//        {
//            get { return _acquiredFormation; }
//            set { _acquiredFormation = value; }
//        }
//
//        /** 阵型方案列表 (0为防御方案,之后为攻击方案) */
//        private List<int> _formationCaseIds;
//
//        /** 当前阵型方案序号 */
//        public int ActiveCaseIndex = -1;
//
//        public int CurSelectCaseIndex = -1;
//        public const int DEFENSE_FORMATION_INDEX = 0;
//
//        private Dictionary<int, Vector3> _formationPosInfoDic;
//
//        //	private Dictionary<int,Props> _formationPropsDic;		//阵法道具数据
//        private Dictionary<int, Dictionary<float, List<int>>> _formationPositiveEffectDic;
//
//        //<阵法id,<克制值,克制阵法Id列表>>
//        private Dictionary<int, Dictionary<float, List<int>>> _formationNegativeEffectDic;
//        //<阵法id,<被克值,被克阵法Id列表>>
//        //	private Dictionary<int,List<FormationEffectInfo>> _formationEffectInfoDic; 			//<阵法id,阵法效果列表>
//
//        public void InitData()
//        {
//            acquiredFormation = new List<FormationInfoDto>();
//            if (_comparison == null)
//            {
//                _comparison = (a, b) =>
//                {
//                    bool hasA = acquiredFormation.Find<FormationInfoDto>(s=>s.id == a.id) != null;
//                    bool hasB = acquiredFormation.Find<FormationInfoDto>(s=>s.id == b.id) != null;
//                    if (hasA && !hasB)
//                        return -1;
//                    else if (!hasA && hasB)
//                        return 1;
//                    else
//                    {
//                        return a.id.CompareTo(b.id);
//                    }
//                };
//            }
//
//            allFormationList = DataCache.getArrayByCls<Formation>();
//            allFormationList.Sort(_comparison);
//            InitConstrainData();
//        }
//
//    private void InitConstrainData()
//    {
//        _positiveEffectDict = new Dictionary<int, Dictionary<int, List<int>>>();
//        _negativeEffectDict = new Dictionary<int, Dictionary<int, List<int>>>();
//        for (int i = 0; i < allFormationList.Count; ++i)
//        {
//            Formation formation = allFormationList[i];
//            string[] effectArray = formation.debuffTargetsStr.Split(',');
//            for (int j = 0; j < effectArray.Length; ++j)
//            {
//                var param = effectArray[j].Split(':');
//
//                int id = 0;
//                int.TryParse(param[0], out id);
//                int val = 0;
//                int.TryParse(param[1], out val);
//
//                if (!_positiveEffectDict.ContainsKey(formation.id))
//                    _positiveEffectDict.Add(formation.id, new Dictionary<int, List<int>>());
//                if (!_positiveEffectDict[formation.id].ContainsKey(val))
//                {
//                    _positiveEffectDict[formation.id].Add(val, new List<int>(4));
//                }
//                _positiveEffectDict[formation.id][val].Add(id);
//
//                if (!_negativeEffectDict.ContainsKey(id))
//                {
//                    _negativeEffectDict.Add(id, new Dictionary<int, List<int>>());
//                }
//                if (!_negativeEffectDict[id].ContainsKey(val))
//                {
//                    _negativeEffectDict[id].Add(val, new List<int>(4));
//                }
//                _negativeEffectDict[id][val].Add(formation.id);
//            }
//        }
//    }
//
//    public void SetupFormationInfo(AfterLoginDto afterLoginDto)
//        {
//            GameDebuger.TODO(@"
//		_acquiredFormationIds = afterLoginDto.acquiredFormationIds;
//		_formationCaseIds = afterLoginDto.formationCaseIds;
//		ActiveCaseIndex = afterLoginDto.activeFormationCaseNum;
//		CurSelectCaseIndex = afterLoginDto.activeFormationCaseNum;
//");
//
//            //初始化阵型位置Map
//            _formationPosInfoDic = new Dictionary<int, Vector3>(12);
//            _formationPosInfoDic.Add(1, new Vector3(-12f, -64f, 0f));
//            _formationPosInfoDic.Add(2, new Vector3(-74f, -102f, 0f));
//            _formationPosInfoDic.Add(3, new Vector3(52f, -26f, 0f));
//            _formationPosInfoDic.Add(4, new Vector3(-140f, -140f, 0f));
//            _formationPosInfoDic.Add(5, new Vector3(118f, 12f, 0f));
//            _formationPosInfoDic.Add(11, new Vector3(56f, -90f, 0f));
//            _formationPosInfoDic.Add(12, new Vector3(-5f, -128f, 0f));
//            _formationPosInfoDic.Add(13, new Vector3(120f, -52f, 0f));
//            _formationPosInfoDic.Add(14, new Vector3(125f, -116f, 0f));
//
//
//            List<Formation> formationList = DataCache.getArrayByCls<Formation>();
//            //阵法效果初始化
//            GameDebuger.TODO(@"
//        _formationEffectInfoDic = new Dictionary<int, List<FormationEffectInfo>>(formationList.Count);
//		for(int fi=0;fi<formationList.Count;++fi){
//			Formation formation = formationList[fi];
//			if(!string.IsNullOrEmpty(formation.posEffectStr)){
//				_formationEffectInfoDic.Add(formation.id,new List<FormationEffectInfo>(5));
//				string[] posSection = formation.posEffectStr.Split('|');
//				for(int pi=0;pi<posSection.Length;++pi){
//					FormationEffectInfo effInfo = new FormationEffectInfo(posSection[pi]);
//					_formationEffectInfoDic[formation.id].Add(effInfo);;
//				}
//			}
//		}
//");
//
//            //阵法相克数据初始化
//            _formationPositiveEffectDic = new Dictionary<int, Dictionary<float, List<int>>>(formationList.Count);
//            _formationNegativeEffectDic = new Dictionary<int, Dictionary<float, List<int>>>(formationList.Count);
//            for (int i = 0; i < formationList.Count; ++i)
//            {
//                Formation formation = formationList[i];
//                string[] effectArray = formation.debuffTargetsStr.Split(',');
//                _formationPositiveEffectDic.Add(formation.id, new Dictionary<float, List<int>>(effectArray.Length));
//                for (int j = 0; j < effectArray.Length; ++j)
//                {
//                    string[] param = effectArray[j].Split(':');
//                    int id = StringHelper.ToInt(param[0]);
//                    float val = float.Parse(param[1]);
//                    if (!_formationPositiveEffectDic[formation.id].ContainsKey(val))
//                    {
//                        _formationPositiveEffectDic[formation.id].Add(val, new List<int>(4));
//                    }
//                    _formationPositiveEffectDic[formation.id][val].Add(id);
//
//                    if (!_formationNegativeEffectDic.ContainsKey(id))
//                    {
//                        _formationNegativeEffectDic.Add(id, new Dictionary<float, List<int>>());
//                    }
//                    if (!_formationNegativeEffectDic[id].ContainsKey(val))
//                    {
//                        _formationNegativeEffectDic[id].Add(val, new List<int>(4));
//                    }
//                    _formationNegativeEffectDic[id][val].Add(formation.id);
//                }
//            }
//
//        }
//
//        //	public FormationEffectInfo GetFormationEffectInfo(int formationId,int pos){
//        //		if(_formationEffectInfoDic == null) return null;
//        //
//        //		List<FormationEffectInfo> posEffectList = null;
//        //		if(_formationEffectInfoDic.TryGetValue(formationId,out posEffectList)){
//        //			if(pos < posEffectList.Count){
//        //				return posEffectList[pos];
//        //			}
//        //		}
//        //
//        //		return null;
//        //	}
//
//        public string GetFormationPositiveEffectInfo(int formationId)
//        {
//            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
//            strBuilder.AppendLine(string.Format("#f{0} 克制下列阵法", formationId));
//
//            Dictionary<float, List<int>> effectDic = _formationPositiveEffectDic[formationId];
//            System.Text.StringBuilder str1 = new System.Text.StringBuilder();
//            System.Text.StringBuilder str2 = new System.Text.StringBuilder();
//            foreach (var pair in effectDic)
//            {
//                var infoBuilder = pair.Key == 0.05f ? str1 : str2;
//                float percent = pair.Key * 100f;
//                infoBuilder.AppendLine(string.Format("克制 {0}",
//                    (percent + "%").WrapColor(ColorConstantV3.Color_Green_Strong_Str)));
//                for (int i = 0, imax = pair.Value.Count; i < imax; ++i)
//                {
//                    Formation formation = DataCache.getDtoByCls<Formation>(pair.Value[i]);
//                    infoBuilder.AppendFormat("#f{0} {1} ", formation.id, formation.name);
//                }
//                infoBuilder.AppendLine();
//            }
//
//            strBuilder.Append(str1.ToString());
//            strBuilder.Append(str2.ToString());
//            return strBuilder.ToString();
//        }
//
//        public string GetFormationNegativeEffectInfo(int formationId)
//        {
//            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
//            strBuilder.AppendLine(string.Format("#f{0} 被下列阵法克制", formationId));
//
//            Dictionary<float, List<int>> effectDic = _formationNegativeEffectDic[formationId];
//            System.Text.StringBuilder str1 = new System.Text.StringBuilder();
//            System.Text.StringBuilder str2 = new System.Text.StringBuilder();
//            foreach (var pair in effectDic)
//            {
//                var infoBuilder = pair.Key == 0.05f ? str1 : str2;
//                float percent = pair.Key * 100f;
//                infoBuilder.AppendLine(string.Format("被克 {0}", (percent + "%").WrapColor("c30000")));
//                for (int i = 0, imax = pair.Value.Count; i < imax; ++i)
//                {
//                    Formation formation = DataCache.getDtoByCls<Formation>(pair.Value[i]);
//                    infoBuilder.AppendFormat("#f{0} {1} ", formation.id, formation.name);
//                }
//                infoBuilder.AppendLine();
//            }
//
//            strBuilder.Append(str1.ToString());
//            strBuilder.Append(str2.ToString());
//            return strBuilder.ToString();
//        }
//
//        //	public Props GetFormationPropsById(int formationId){
//        //		if(_formationPropsDic == null){
//        //			_formationPropsDic = new Dictionary<int, Props>();
//        //			var itemList = DataCache.getArrayByCls<GeneralItem>();
//        //			for(int i=0,imax=itemList.Count;i<imax;++i){
//        //				if(itemList[i] is Props){
//        //					Props props = itemList[i] as Props;
//        //					if(props.logicId == Props.PropsLogicEnum_EIGHT_DIAGRAMS){
//        //						GameDebuger.TODO(@"PropsParam_8 logicParams = props.propsParam as PropsParam_8;
//        //                        _formationPropsDic.Add(logicParams.formationId,props);");
//        //					}
//        //				}
//        //			}
//        //		}
//        //
//        //		if(_formationPropsDic.ContainsKey(formationId))
//        //			return _formationPropsDic[formationId];
//        //
//        //		return null;
//        //	}
//
//        public int GetFormationLevel(int formationId)
//        {
//            if (FormationHelper.IsRegular(formationId))
//                return 1;
//            var dto = acquiredFormation.Find<FormationInfoDto>(s => s.id == formationId);
//            return dto == null ? 0 : dto.level;
//        }
//
//        //获取所有阵型Id列表，优先已掌握的
//        public IEnumerable<Formation> GetAllFormationInfo(out int length)
//        {
//            length = allFormationList.IsNullOrEmpty() ? 0 : allFormationList.Count;
//            return allFormationList;
//
//        }
//
//        public Vector3 GetFormationPos(int pos)
//        {
//            if (_formationPosInfoDic.ContainsKey(pos))
//            {
//                return _formationPosInfoDic[pos];
//            }
//            return Vector3.zero;
//        }
//
//        //获取当前激活方案的阵型Id
//        public int GetActiveFormationId()
//        {
//            return GetFormationCaseId(ActiveCaseIndex);
//        }
//
//        public bool HasAcquiredFormation(int formationId)
//        {
//            return _acquiredFormation.Find<FormationInfoDto>(s => s.id == formationId) != null;
//        }
//
//        //获取指定方案的阵型Id
//        public int GetFormationCaseId(int caseIndex)
//        {
//            if (caseIndex < _formationCaseIds.Count)
//                return _formationCaseIds[caseIndex];
//
//            return -1;
//        }
//
//        public void SetFormationCaseId(int caseIndex, int newFormationId)
//        {
//            GameDebuger.TODO(@"
//ServiceRequestAction.requestServer(CrewService.changeFormation(caseIndex,newFormationId),""PlayerChangeFormation"",(e)=>{
//            if(caseIndex < _formationCaseIds.Count){
//                _formationCaseIds[caseIndex] = newFormationId;
//
//                GameEventCenter.SendEvent(GameEvent.Player_OnFormationChange);
//            }
//        });
//");
//        }
//
//        public void LearnNewFormation(BagItemDto itemDto, Formation newFormation, bool free)
//        {
//            GameDebuger.TODO(@"
//ServiceRequestAction.requestServer(PlayerService.learnFormation(itemDto.index,free),'LearnFormation',(e)=>{
//            LearnFormationDto learnFormatinDto = e as LearnFormationDto;
//            _formationCaseIds = learnFormatinDto.formationCaseIds;
//
//            int defaultCapacity = DataCache.GetStaticConfigValue(AppStaticConfigs.FORMATION_DEFAULT_CAPACITY,4);
//            if(_acquiredFormationIds.Count < defaultCapacity){
//                TipManager.AddTip(string.Format('你学会了{0}',newFormation.name.WrapColor(ColorConstant.Color_Tip_LostCurrency_Str)), true);
//            }else{
//                if(free){
//                    for(int i=0;i<_acquiredFormationIds.Count;++i){
//                        int originFormationId = _acquiredFormationIds[i];
//                        if(!learnFormatinDto.formationIds.Contains(originFormationId))
//                        {
//                            Formation replaceFormation = DataCache.getDtoByCls<Formation>(originFormationId);
//                            TipManager.AddTip(string.Format('你学会了{0}，但遗忘了{1}',
//                                                            newFormation.name.WrapColor(ColorConstant.Color_Tip_LostCurrency_Str),
//                                                            replaceFormation.name.WrapColor(ColorConstant.Color_Tip_LostCurrency_Str)), true);
//                            break;
//                        }
//                    }
//                }else{
//                    int ingotCount = DataCache.GetStaticConfigValue(AppStaticConfigs.FORMATION_OVER_DEFAULT_CAPACITY,200);
//                    TipManager.AddTip(string.Format('消耗了{0}{1}，你学会了{2}',ingotCount.ToString().WrapColor(ColorConstant.Color_Tip_LostCurrency_Str),
//                                                    ItemIconConst.Ingot,
//                                                    newFormation.name.WrapColor(ColorConstant.Color_Tip_LostCurrency_Str)), true);
//                }
//            }
//
//            _acquiredFormationIds = learnFormatinDto.formationIds;
//            GameEventCenter.SendEvent(GameEvent.Player_OnFormationChange);
//
//            //首次学会阵型自动使用
//            if (GetAcquiredFormationIdList().Count == 2)
//            {
//                if (ModelManager.Team.HasTeam())
//                {
//                    if (ModelManager.Team.IsTeamLeader())
//                    {
//                        ModelManager.Team.ChangeTeamFormation(newFormation.id);
//                    }
//                }
//                else
//                {
//                    ModelManager.Player.SetFormationCaseId(ModelManager.Player.ActiveCaseIndex, newFormation.id);
//                }
//            }
//        });
//");
//        }
//
//        public void UpdateCurFormationByIdx(int idx)
//        {
//            _curFormation = allFormationList.TryGetValue(idx);
//            var info = acquiredFormation.Find<FormationInfoDto>(s => s.id == _curFormation.id);
//            if (_curFormation.id == (int) Formation.FormationType.Regular)
//                curLevel = 1;
//            else if (info != null)
//            {
//                curLevel = info.level;
//            }
//            else
//            {
//                curLevel = -1;
//            }
//        }
//
//    }
//}
