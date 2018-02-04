using AppDto;
using System.Collections.Generic;
/// <summary>
/// 装备洗练界面数据
/// </summary>
public interface IEquipmentResetViewData
{

    EquipmentMainDataMgr.EquipmentHoldTab CurTab { get; }
    //当期选中的装备
    EquipmentDto CurChoiceEquipment { get; }
    /// <summary>
    /// 洗练后服务器返回来的结果~退出界面后清除
    /// </summary>
    //EquipmentExtraDto CurResetEquipmentResult { get; }
    //当前的装备列表
    IEnumerable<EquipmentDto> EquipmentItems { get; }
    /// <summary>
    /// 原属性
    /// </summary>
    EquipmentResetAttrContentVo OldAttrContent { get; }
    /// <summary>
    /// 新属性
    /// </summary>
    EquipmentResetAttrContentVo NewAttrContent { get; }

    /// <summary>
    /// 洗练需要的材料
    /// </summary>
    SmithItemVo CurSmithItemVo { get; }
}
public sealed partial class EquipmentMainDataMgr
{
   
    public class EquipmentResetViewData : IEquipmentResetViewData
    {
        public static int MinOpenGrade { private set; get; }


        /// <summary>
        /// 设置默认值
        /// </summary>
        public void InitData()
        {
            var eq_resetGradeConfig = DataCache.getDtoByCls<StaticConfig>(AppStaticConfigs.EQUIP_RESET_GRADE_LIMIT);
            MinOpenGrade = StringHelper.ToInt(eq_resetGradeConfig.value);


            CurTab = EquipmentMainDataMgr.EquipmentHoldTab.Equip;
            _oldAttrContent = new EquipmentResetAttrContentVo("原属性", true, "请选择一件装备");
            _newAttrContent = new EquipmentResetAttrContentVo("新属性", true, "点击“洗炼”按钮开始洗炼");
            _curSmithItemVo = new SmithItemVo();
        }

        public static ITabInfo[] tabinfos = new ITabInfo[]
        {
        TabInfoData.Create((int)EquipmentMainDataMgr.EquipmentHoldTab.Equip,"已装备"),
        TabInfoData.Create((int)EquipmentMainDataMgr.EquipmentHoldTab.Bag,"包裹"),
        };

        private EquipmentDto curChoiceEquipment;
        public EquipmentDto CurChoiceEquipment
        {
            get
            {
                return curChoiceEquipment;
            }
            set
            {
                var lastValue = curChoiceEquipment;
                curChoiceEquipment = value;

                if (curChoiceEquipment != lastValue)
                {
                    OnCurChoiceEquipmentChange();
                }
            }
        }


        public EquipmentMainDataMgr.EquipmentHoldTab CurTab { get; set; }

        int SortMethod(EquipmentDto a,EquipmentDto b)
        {
            var eq_a = a.equip as Equipment;
            var eq_b = b.equip as Equipment;
            var eqpart_a = eq_a.partType[0];
            var eqpart_b = eq_b.partType[0];
            //战力
            if (a.property.power.CompareTo(b.property.power) != 0)
                return b.property.power - a.property.power;
            else if (eq_a.grade != eq_b.grade)
            {
                //装备等级
                return eq_b.grade - eq_a.grade;
            }
            //装备品质
            else if (a.property.quality != b.property.quality)
                return b.property.quality - a.property.quality;
            //TODO：带装备特效的

            //部分排序 

            else if (eqpart_a != eqpart_b)
                return eqpart_a - eqpart_b;
            else
                //装备ID排序
                return b.equipId - a.equipId;
        }

        int SortMethod_OnEquip(EquipmentDto a, EquipmentDto b)
        {
            var eq_a = a.equip as Equipment;
            var eq_b = b.equip as Equipment;
            var eqpart_a = eq_a.partType[0];
            var eqpart_b = eq_b.partType[0];
            return eqpart_a - eqpart_b;
        }
        public IEnumerable<EquipmentDto> EquipmentItems
        {
            get
            {
                var equipList = EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(CurTab);
                Dictionary<int,Equipment> equipment= DataCache.getDicByCls<Equipment>();
                List<EquipmentDto> _equipList = new List<EquipmentDto>();
                for(int s = 0;s < equipList.Count;s++) {
                    Equipment EQUP = equipList[s].equip as Equipment;
                    if(EQUP.grade >= 40)
                        _equipList.Add(equipList[s]);
                }

                if(CurTab == EquipmentHoldTab.Equip)
                {
                    _equipList.Sort(SortMethod_OnEquip);
                }
                else
                {
                    _equipList.Sort(SortMethod);
                }
                return _equipList;
            }
        }


        private EquipmentResetAttrContentVo _oldAttrContent;
        public EquipmentResetAttrContentVo OldAttrContent
        {
            get
            {
                return _oldAttrContent;
            }
        }
        private EquipmentResetAttrContentVo _newAttrContent;
        public EquipmentResetAttrContentVo NewAttrContent
        {
            get
            {
                return _newAttrContent;
            }
        }

        private SmithItemVo _curSmithItemVo;
        public SmithItemVo CurSmithItemVo
        {
            get
            {
                return _curSmithItemVo;
            }
        }

        /// <summary>
        /// 更新当前选中的标签的装备列表
        /// </summary>
        public void UpdateCurEquipmentItemList()
        {
            var tempList = EquipmentItems.ToList();
            CurChoiceEquipment = tempList.IsNullOrEmpty() ? null : tempList[0];
        }

        /// <summary>
        /// 当前装备的属性范围
        /// </summary>
        private List<EquipmentPropertyRange> _CurEquipmentPropertRanges = new List<EquipmentPropertyRange>();

        /// <summary>
        /// 当更换选择物品的时候更新装备身上的属性条
        /// </summary>
        public void UpdateCurEquipmentPropertRange()
        {
            _CurEquipmentPropertRanges.Clear();
            if (CurChoiceEquipment!=null)
            {
                
                _CurEquipmentPropertRanges = EquipmentMainDataMgr.DataMgr.GetEquipmentPropertyRange(CurChoiceEquipment.equipId, CurChoiceEquipment.property.quality);
            }
            
            UpdateOldAttrContent();
            UpdateNewAttrContent();
            
        }

        /// <summary>
        /// 更新旧的装备属性展示面板
        /// </summary>
        private void UpdateOldAttrContent()
        {
            if(CurChoiceEquipment == null || CurChoiceEquipment.property.currentProperty == null)
            {
                _oldAttrContent.isEmpty = true;
                _oldAttrContent.UpdateAttrItemVo(null,null);
            }
            else
            {
                ///更新属性显示
                var curEquipmentExtra = CurChoiceEquipment.property.currentProperty;
                var oldPropertyList = new List<CharacterPropertyDto>();
                oldPropertyList.AddRange(curEquipmentExtra.baseProps);
                oldPropertyList.AddRange(curEquipmentExtra.secondProps);
                //oldPropertyList.AddRange(curEquipmentExtra.extraProps);
                List<ResetAttrItemVo> itemVos = new List<ResetAttrItemVo>();
                oldPropertyList.ForEach(x =>
                {
                    ResetAttrItemVo itemVo = new ResetAttrItemVo();
                    itemVo.value_offset = 0;
                    itemVo.eq_range = GetAttrRange(x.propId);
                    itemVo.property = x;
                    itemVos.Add(itemVo);
                });

                //附加属性在额外的表~
                curEquipmentExtra.extraProps.ForEachI((x, i) =>
                {
                    ResetAttrItemVo itemVo = new ResetAttrItemVo();

                    int quality = CurChoiceEquipment.property.quality;
                    int lv = (curChoiceEquipment.equip as Equipment).grade;

                    itemVo.eq_range = EquipmentMainDataMgr.DataMgr.GetEquipmentExtraProperty(x.propId, quality, lv, curEquipmentExtra.extraProps.Count>1);
                    itemVo.property = x;
                    itemVo.value_offset = 0;

                    itemVos.Add(itemVo);
                });
                _oldAttrContent.isEmpty = false;
                _oldAttrContent.UpdateAttrItemVo(CurChoiceEquipment,itemVos);
            }  
        }
        /// <summary>
        /// 更新新的装备属性展示面板
        /// </summary>
        private void UpdateNewAttrContent()
        {
            if (CurChoiceEquipment == null || CurChoiceEquipment.property.resetProperty == null)
            {
                _newAttrContent.isEmpty = true;
                _newAttrContent.UpdateAttrItemVo(null,null);
            }
            else
            {

                ///更新属性显示
                var curEquipmentExtra = CurChoiceEquipment.property.currentProperty;
                var oldPropertyList = new List<CharacterPropertyDto>();
                oldPropertyList.AddRange(curEquipmentExtra.baseProps);
                oldPropertyList.AddRange(curEquipmentExtra.secondProps);
                //oldPropertyList.AddRange(curEquipmentExtra.extraProps);

                //新的属性列表
                var newEquipmentExtra = CurChoiceEquipment.property.resetProperty;
                var newPropertyList = new List<CharacterPropertyDto>();
                newPropertyList.AddRange(newEquipmentExtra.baseProps);
                newPropertyList.AddRange(newEquipmentExtra.secondProps);
                // newPropertyList.AddRange(CurResetEquipmentResult.extraProps);


                //TODO:需要确保原属性和新属性的 列表顺序一致
                List<ResetAttrItemVo> itemVos = new List<ResetAttrItemVo>();

                newPropertyList.ForEachI((x, i) =>
                {
                   
                    ResetAttrItemVo itemVo = new ResetAttrItemVo();                  
                    float oldValue = oldPropertyList[i].propValue;
                    float newValue = newPropertyList[i].propValue;

                    itemVo.eq_range = GetAttrRange(x.propId);
                    itemVo.property = x;
                    if (oldPropertyList[i].propId == newPropertyList[i].propId)
                    {
                        itemVo.value_offset = ((int)newValue - (int)oldValue);
                    }
                    else
                        itemVo.value_offset = 0;
                    

                    itemVos.Add(itemVo);
                });

                //附加属性的范围在别的表~
                //附加属性不参与对比~~
                newEquipmentExtra.extraProps.ForEachI((x, i) =>
                {
                    ResetAttrItemVo itemVo = new ResetAttrItemVo();
                    //float oldValue = curEquipmentExtra.extraProps[i].propValue;
                    //float newValue = x.propValue;

                    int quality = CurChoiceEquipment.property.quality;
                    int lv = (curChoiceEquipment.equip as Equipment).grade;

                    itemVo.eq_range = EquipmentMainDataMgr.DataMgr.GetEquipmentExtraProperty(x.propId, quality, lv, newEquipmentExtra.extraProps.Count>1);
                    itemVo.property = x;
                    //itemVo.value_offset = newValue - oldValue;
                    itemVo.value_offset = 0;
                    itemVos.Add(itemVo);
                });


                _newAttrContent.isEmpty = false;
                _newAttrContent.UpdateAttrItemVo(CurChoiceEquipment, itemVos);
            }
        }
        /// <summary>
        /// 当选中的装备有所改变~
        /// </summary>
        private void OnCurChoiceEquipmentChange()
        {
            UpdateCurEquipmentPropertRange();

            UpdateCurSmith();

        }

        /// <summary>
        /// 更新当前选中物品的打造材料
        /// </summary>
        public void UpdateCurSmith()
        {
            if (CurChoiceEquipment == null)
                return;
            var equipment = CurChoiceEquipment.equip as Equipment;
            int quality = CurChoiceEquipment.property.quality;
            EquipmentQuality eq_qulity = DataCache.getDtoByCls<EquipmentQuality>(quality);
            var _prop = DataCache.getDtoByCls<GeneralItem>(eq_qulity.itemId) as Props;
            if (_prop == null)
            {
                GameLog.LogEquipment("OnCurChoiceEquipmentChange Prop == null,请检测导表");
                return;
            }
            _curSmithItemVo.props = _prop;
            _curSmithItemVo.needCount = equipment.resetIronAmount;
            int hasCount = (int)BackpackDataMgr.DataMgr.GetItemCountByItemID(_prop.id);
            _curSmithItemVo.currentCount = hasCount;
        }


        /// <summary>
        /// 在当前装备的身上，获取某一个属性的范围值,不包含附加的属性~因为附加属性在另一张表里面
        /// </summary>
        /// <param name="attrId"></param>
        /// <returns></returns>
        public EquipmentPropertyRange GetAttrRange(int attrId)
        {
            var res = _CurEquipmentPropertRanges.Find(x => x.abilityId == attrId);
            return res;
        }

    }

}
