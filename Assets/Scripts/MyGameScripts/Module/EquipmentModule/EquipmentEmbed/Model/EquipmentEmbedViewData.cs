// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentEmbedViewData.cs
// Author   : Zijian
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************
using System;
using System.Collections.Generic;
using AppDto;
/// <summary>
/// 每个宝石孔的开放等级~
/// </summary>
public class EmbedHoleOpenInfo
{
    /// <summary>
    /// 顺时针 0 1 2 3
    /// </summary>
    public int holePos;
    public int openGrade;
    public EmbedHoleOpenInfo(int holePos, int openGrade)
    {
        this.holePos = holePos;
        this.openGrade = openGrade;
    }
}

/// <summary>
/// 中间宝石的
/// </summary>
public class EquipmentEmbedHoleVo
{
    public EmbedHoleOpenInfo holeInfo { get; private set; }
    public int embedid
    {
        get { return dto == null ? -1 : dto.embedItemId; }
    }

    private PropsParam_1 _propsAttr;
    public PropsParam_1 PropsAttr
    {
        get
        {
            return _propsAttr;
        }
    }
    public EmbedApertureDto dto { get; private set; }
    //宝石的数据

    //是否已经开放
    public bool isOpen
    {
        get
        {
            return ModelManager.Player.GetPlayerLevel() >= holeInfo.openGrade;
        }
    }
    public EquipmentEmbedHoleVo(EmbedHoleOpenInfo info)
    {
        this.holeInfo = info;
    }
    public void UpdateDto(EmbedApertureDto dto)
    {
        this.dto = dto;
        if(dto != null)
        {
            var props  = DataCache.getDtoByCls<GeneralItem>(dto.embedItemId) as Props;
            if(props != null)
            {
                _propsAttr = props.propsParam as PropsParam_1;
            }
        }
        else
        {
            _propsAttr = null;
        }
    }
}

/// <summary>
/// 左侧宝石部位选项的数据
/// </summary>
public class EquipmentEmbedCellVo
{
    public string name { get; private set; }
    //部位
    public Equipment.PartType part { get; private set; }

    EmbedHoleDto dto;
    List<EquipmentEmbedHoleVo> _embedHoleVoList = new List<EquipmentEmbedHoleVo>();
    public List<EquipmentEmbedHoleVo> EmbedHoleVoList { get { return _embedHoleVoList; } }
    //宝石的数量
    public int embedCount { get
        {
            return _embedHoleVoList.Count(x=>x.dto!=null);
        } }

    List<CharacterPropertyDto> _totalPropertys = new List<CharacterPropertyDto>();
    public IEnumerable<CharacterPropertyDto> TotalProperty
    {
        get { return _totalPropertys; }
    }
    /// <summary>
    /// 计算总能力值
    /// </summary>
    private void CalulateTotalAbility()
    {
        _totalPropertys.Clear();
        EmbedHoleVoList.ForEach(x => {
            if(x.PropsAttr != null)
            {
                var _property = _totalPropertys.Find(p => x.PropsAttr.caId == p.propId);
                if(_property != null)
                {
                    _property.propValue += x.PropsAttr.value;
                }
                else
                {
                    CharacterPropertyDto property = new CharacterPropertyDto();
                    property.propId = x.PropsAttr.caId;
                    property.propValue = x.PropsAttr.value;
                    _totalPropertys.Add(property);
                }
               
            }
        });
    }
    public EquipmentEmbedCellVo(Equipment.PartType part)
    {
        this.part = part;
        //饰品2在导标里面跟饰品1 一致
        if (part == Equipment.PartType.AccTwo)
            part = Equipment.PartType.AccOne;
        var equipmentTypeConfig = DataCache.getDtoByCls<EquipmentPart>((int)part);
        
        this.name = equipmentTypeConfig.name;

        if (this.part == Equipment.PartType.AccTwo)
            this.name += "二";
        else if (this.part == Equipment.PartType.AccOne)
            this.name += "一";

            //默认初始化4个孔
        var embedHoleInfos = EquipmentMainDataMgr.DataMgr.GetEmbedHoleOpenInfo();
        for (int i = 0; i < EquipmentMainDataMgr.EquipmentEmbedViewData.MaxHoleNum; i++)
        {
            EquipmentEmbedHoleVo itemVo = new EquipmentEmbedHoleVo(embedHoleInfos[i]);
            _embedHoleVoList.Add(itemVo);
        }

    }

    public void UpdateDto(EmbedHoleDto dto)
    {
        this.dto = dto;
        //更新每个孔的Dto
        _embedHoleVoList.ForEachI((x,i) => {
            EmbedApertureDto holeDto = null;
            if (dto != null && dto.apertures!=null)
            {
                holeDto = dto.apertures.Find(g => g.apId == i);
            }
            x.UpdateDto(holeDto);
        });

        CalulateTotalAbility();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="isEmbed">安装/摘下</param>
    public void UpdateDto(EmbedApertureDto dto,bool isEmbed)
    {
        int pos = dto.apId;
        if (isEmbed)
            _embedHoleVoList[pos].UpdateDto(dto);
        else
            _embedHoleVoList[pos].UpdateDto(null);

        CalulateTotalAbility();
    }
}
public interface IEquipmentEmbedViewData
{
    //当前选中的部位
    EquipmentEmbedCellVo CurChoicePartVo { get; }
    //当前选中的孔
    EquipmentEmbedHoleVo CurChoiceHoleVo { get; }

    //部位的列表
    IEnumerable<EquipmentEmbedCellVo> EmbedCellPartItems { get; }
    IEnumerable<EquipmentDto> CurEquipments { get; }
    /// <summary>
    /// 中间4个宝石孔
    /// </summary>
    List<EquipmentEmbedHoleVo> EmbedItemVos { get; }

    /// <summary>
    /// 身上的宝石~
    /// </summary>
    IEnumerable<Eq_GoodItemVo> propsList { get; }
}
public sealed partial class EquipmentMainDataMgr
{
    public List<EmbedHoleOpenInfo> GetEmbedHoleOpenInfo()
    {
        return _data.EmbedData.embedHoleInfos;
    }
    public EquipmentEmbedCellVo GetEmbedInfoByPart(Equipment.PartType part)
    {
        return _data.EmbedData.EmbedCellPartItems.Find(x => x.part == part);
    }
    public class EquipmentEmbedViewData : IEquipmentEmbedViewData
    {
        /// <summary>
        /// 设置默认值
        /// </summary>
        public void InitData()
        {
            var embedConfig = DataCache.getDtoByCls<StaticConfig>(AppStaticConfigs.EMBED_HOLE_CAPACITY);
            var raw_str_0 = embedConfig.value;
            var raw_str_1 = raw_str_0.Split('|');
            raw_str_1.ForEach(x =>
            {
                var raw_str_2 = x.Split('/');
                int holePos = StringHelper.ToInt(raw_str_2[0]);
                int openGrade = StringHelper.ToInt(raw_str_2[1]);
                EmbedHoleOpenInfo info = new EmbedHoleOpenInfo(holePos, openGrade);
                embedHoleInfos.Add(info);
            });
            //4个
            MaxHoleNum = embedHoleInfos.Count;

            
            //固定6个部位
            _embedCellPartItems.Add(GetEmbedPartInfo(Equipment.PartType.Weapon));
            _embedCellPartItems.Add(GetEmbedPartInfo(Equipment.PartType.Clothes));
            _embedCellPartItems.Add(GetEmbedPartInfo(Equipment.PartType.Shoe));
            _embedCellPartItems.Add(GetEmbedPartInfo(Equipment.PartType.Glove));
            _embedCellPartItems.Add(GetEmbedPartInfo(Equipment.PartType.AccOne));
            _embedCellPartItems.Add(GetEmbedPartInfo(Equipment.PartType.AccTwo));


            //默认选中第一个部位
            CurChoicePartVo = _embedCellPartItems[0];
        }
        #region 配表信息
        /// <summary>
        /// 每个宝石孔的开放等级~
        /// </summary>
        public List<EmbedHoleOpenInfo> embedHoleInfos = new List<EmbedHoleOpenInfo>();
        /// <summary>
        /// 每个部位最多有的宝石数量~
        /// </summary>
        public static int MaxHoleNum { private set; get; }
        #endregion
        private EquipmentEmbedCellVo _curChoicePartVo;
        private EquipmentEmbedHoleVo _curChoiceHoleVo;
        private List<EquipmentEmbedCellVo> _embedCellPartItems = new List<EquipmentEmbedCellVo>();
        private List<Eq_GoodItemVo> _propsList =new List<Eq_GoodItemVo>();
        private int curEmbedPhaseId, topEmbedPhaseId;//宝石阶段

        public void UpdateEmbedPhase(int new_curPhase)
        {
            if(new_curPhase > topEmbedPhaseId)
            {
                EquipmentEmbedGroupFirstShowController.Show(new_curPhase, topEmbedPhaseId);
                //激活宝石套装
                topEmbedPhaseId = new_curPhase;
            }
            this.curEmbedPhaseId = new_curPhase;
        }
        public IEnumerable<EquipmentEmbedCellVo> EmbedCellPartItems
        {
            get
            {
                return _embedCellPartItems;
            }
        }
        public EquipmentEmbedCellVo CurChoicePartVo
        {
            get
            {
                return _curChoicePartVo;
            }
            set
            {
                var lastValue = _curChoicePartVo;
                _curChoicePartVo = value;
                if (_curChoicePartVo != lastValue)
                {
                    OnUpdateChoicePart();
                    FireData();
                }
            }
        }
        public IEnumerable<Eq_GoodItemVo> propsList
        {
            get
            {
                _propsList.Clear();
                var bagItemDto = BackpackDataMgr.DataMgr.GetBagItems(AppItem.ItemTypeEnum.Embed);
                var res = bagItemDto.Filter(x =>
                {
                    var props = x.item as Props;
                    var embedParam = props.propsParam as PropsParam_1;
                    return embedParam.partTypes.Contains((int)CurChoicePartVo.part);
                });

                res.ForEach(x=> {
                    var props = x.item as Props;
                    var embedParam = props.propsParam as PropsParam_1;
                    var cbConfig = DataCache.getDtoByCls<CharacterAbility>(embedParam.caId);
                    string des = string.Format("{0}+{1}", cbConfig.name, embedParam.value);
                    var vo = new Eq_GoodItemVo(props,x.count, des);
                    _propsList.Add(vo);
                });

                return _propsList;
            }
        }
        public EquipmentEmbedHoleVo CurChoiceHoleVo
        {
            get
            {
                return _curChoiceHoleVo;
            }
            set
            {
                _curChoiceHoleVo = value;
            }
        }
        public List<EquipmentEmbedHoleVo> EmbedItemVos
        {
            get
            {
                return CurChoicePartVo.EmbedHoleVoList;
            }
        }

        public IEnumerable<EquipmentDto> CurEquipments
        {
            get
            {
                var equipList =  DataMgr.GetEquipmentDtoList(EquipmentHoldTab.Equip);
                return equipList;
            }
        }
        #region NetUpdate
        public void UpdateHolesDto(EmbedHolesDto dto)
        {
            UpdateEmbedCellParts(dto);
        }
        public void UpdateApertureDto(int partId,EmbedApertureDto dto,bool isEmbed)
        {
            var vo = _embedCellPartItems.Find(x => (int)x.part == partId);
            if(vo == null)
            {
                GameLog.LogEquipment("Part不正确！！");
                return;
            }
            vo.UpdateDto(dto,isEmbed);
        }
        #endregion
        /// <summary>
        /// 更新部位信息
        /// </summary>
        private void UpdateEmbedCellParts(EmbedHolesDto HolesDto)
        {
            _embedCellPartItems.ForEachI((x, i) =>
            {
                var hole = HolesDto.ehDto.Find(h => h.partId == (int)x.part);
                x.UpdateDto(hole);
            });

            curEmbedPhaseId = HolesDto.curephaseId;
            topEmbedPhaseId = HolesDto.ephaseId;
        }
        /// <summary>
        /// 获取部位的信息
        /// </summary>
        /// <param name="part"></param>
        private EquipmentEmbedCellVo GetEmbedPartInfo(Equipment.PartType part)
        {
            EquipmentEmbedCellVo vo = new EquipmentEmbedCellVo(part);
            return vo;

        }
        private void OnUpdateChoicePart()
        {

            //每次切换部位，都默认选择第一个宝石孔
            CurChoiceHoleVo = EmbedItemVos[0];
        }
    }
}

