using AppDto;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProxyTips
{
    #region textTips tips在按钮上方 将tips bg的中心设为Bottom，在下方就设为Top，pos为tips Bg的localposition
    //有参数的情况未考虑 todo xjd
    public static void OpenTextTips(int id, Vector3 pos, bool isUp = false, params string[] strParam)
    {
        var ctrl = TextTipsViewController.Show<TextTipsViewController>(TextTipsView.NAME, UILayerType.Dialogue, false, true);
        ctrl.UpdateText(id, pos, isUp, strParam);
    }
    #endregion

    public static IBaseTipsController OpenGeneralItemTips(GeneralItem general, string price = "", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = BaseTipsController.Show<BaseTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(general, price, time);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    public static IBaseTipsController OpenGeneralItemTips(GeneralItem general, Dictionary<string, Action> leftDic, string right = "", Action rightClick = null, string price = "", long time = 0)
    {
        var ctrl = BaseTipsController.Show<BaseTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(general, price, time);
        var btnCtrl = ctrl.SetBtnView(leftDic, right, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    //纹章
    public static MedallionTipsController OpenMedallionTips(BagItemDto dto, string price="", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<MedallionTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(dto, price, time);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    public static MedallionTipsController OpenMedallionTips(BagItemDto dto, Dictionary<string, Action> leftDic, string right = "", Action rightClick = null, string price="", long time = 0)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<MedallionTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(dto, price, time);
        var btnCtrl = ctrl.SetBtnView(leftDic, right, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    //铭刻符
    public static RuneTipsController OpenRuneTips(GeneralItem dto, string price = "", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<RuneTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(dto, price, time);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    public static RuneTipsController OpenRuneTips(GeneralItem dto, Dictionary<string, Action> leftDic, string right = "", Action rightClick = null, string price = "", long time = 0)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<RuneTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(dto, price, time);
        var btnCtrl = ctrl.SetBtnView(leftDic, right, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    //生活技能料理
    public static AssistSkillPropTipsController OpenAssistSkillPropTips(GeneralItem dto, string price = "", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<AssistSkillPropTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(dto, price, time);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    public static AssistSkillPropTipsController OpenAssistSkillPropTips(BagItemDto dto, string price = "", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<AssistSkillPropTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(dto, price, time);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    public static AssistSkillPropTipsController OpenAssistSkillPropTips(GeneralItem dto, Dictionary<string, Action> leftDic, string right = "", Action rightClick = null, string price = "", long time = 0)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<AssistSkillPropTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(dto, price, time);
        var btnCtrl = ctrl.SetBtnView(leftDic, right, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    //装备
    public static EquipmentTipsController OpenEquipmentTips(EquipmentDto equipDto,EquipmentEmbedCellVo embedVo,EquipmentDto compareDto= null, string price = "", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<EquipmentTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
  
        ctrl.UpdateView(equipDto, embedVo, compareDto);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    public static EquipmentTipsController OpenEquipmentTips(EquipmentDto equipDto, EquipmentEmbedCellVo embedVo, Dictionary<string, Action> leftDic, string right = "", Action rightClick = null, string price = "", long time = 0)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<EquipmentTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        //var ctrl = MedallionTipsController.Show<MedallionTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        //ctrl.UpdateView(equipDto, embedVo, price, time);
        //var btnCtrl = ctrl.SetBtnView(leftDic, right, rightClick);
        //ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }
    //装备
    public static EquipmentTipsController OpenEquipmentTips_FromBag(EquipmentDto _equipmentDto, bool isShowCompare = false,string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        if (_equipmentDto == null)
            return null;
        var ctrl = UIModuleManager.Instance.OpenFunModule<EquipmentTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);

        EquipmentEmbedCellVo embedVo = null;
        left = "更多";

        var curEquipmentDto = EquipmentMainDataMgr.DataMgr.GetSameEquipmentByPart(_equipmentDto);

        if(curEquipmentDto != null)
            embedVo = EquipmentMainDataMgr.DataMgr.GetEmbedInfoByPart((Equipment.PartType)curEquipmentDto.partType);

        if (curEquipmentDto != null && curEquipmentDto.equipUid == _equipmentDto.equipUid)
        {
            right = "卸下";
            rightClick = () =>
            {
                EquipmentMainDataMgr.EquipmentMainNetMsg.ReqTakeOffEquipment(_equipmentDto);
                UIModuleManager.Instance.CloseModule(BaseTipsView.NAME);
            };
           
        }
        else
        {
            right = "装备";
            rightClick = () =>
            {
                EquipmentMainDataMgr.EquipmentMainNetMsg.ReqEquip_Wear(_equipmentDto);
                UIModuleManager.Instance.CloseModule(BaseTipsView.NAME);
            };
        }
        bool isEquip = curEquipmentDto != null && curEquipmentDto.equipUid != _equipmentDto.equipUid;
        if (isShowCompare && isEquip)
            ctrl.UpdateView(_equipmentDto, embedVo, curEquipmentDto);
        else
            ctrl.UpdateView(_equipmentDto, embedVo, null);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }
    public static QuartzTipsController OpenQuartzTips(BagItemDto itemDto, string price = "", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<QuartzTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(itemDto, price, time);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }
    public static QuartzTipsController OpenQuartzTips(BagItemDto itemDto, Dictionary<string, Action> leftDic, string right = "", Action rightClick = null, string price = "", long time = 0)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<QuartzTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(itemDto, price, time);
        var btnCtrl = ctrl.SetBtnView(leftDic, right, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    //暂时留着 之后完善后 把.cs一并删除 todo xjd
    //虚物品
    //public static VirtualItemTipsController OpenVirtualItemTips(AppVirtualItem itemDto, string price = "", long time = 0, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    //{
    //    var ctrl = UIModuleManager.Instance.OpenFunModule<VirtualItemTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
    //    ctrl.UpdateView(itemDto, price, time);
    //    var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
    //    ctrl.ReSetAllPos(btnCtrl);

    //    return ctrl;
    //}

    //public static VirtualItemTipsController OpenVirtualItemTips(AppVirtualItem itemDto, Dictionary<string, Action> leftDic, string right = "", Action rightClick = null, string price = "", long time = 0)
    //{
    //    var ctrl = UIModuleManager.Instance.OpenFunModule<VirtualItemTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
    //    ctrl.UpdateView(itemDto, price, time);
    //    var btnCtrl = ctrl.SetBtnView(leftDic, right, rightClick);
    //    ctrl.ReSetAllPos(btnCtrl);

    //    return ctrl;
    //}

    //任务
    //public static MissionTipsController OpenMissionTips(GeneralItem itemDto, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    //{
    //    var ctrl = UIModuleManager.Instance.OpenFunModule<MissionTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
    //    ctrl.UpdateView(itemDto);
    //    var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
    //    ctrl.ReSetAllPos(btnCtrl);

    //    return ctrl;
    //}

    //魔法
    public static MagicTipsController OpenMagicTips(int id, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<MagicTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(id);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }
    //技能
    public static SkillTipsController OpenSkillTips(int id, string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null)
    {
        var ctrl = UIModuleManager.Instance.OpenFunModule<SkillTipsController>(BaseTipsView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(id);
        var btnCtrl = ctrl.SetBtnView(left, right, leftClick, rightClick);
        ctrl.ReSetAllPos(btnCtrl);

        return ctrl;
    }

    public static IBaseTipsController OpenTipsWithBagItemDto(BagItemDto dto, Vector3 tipsShowPos)
    {
        var itemType = (AppItem.ItemTypeEnum)dto.item.itemType;
        IBaseTipsController _tipsCtrl = null;
        switch (itemType)
        {
            case AppItem.ItemTypeEnum.Virtual:
            case AppItem.ItemTypeEnum.MissionItem:
            case AppItem.ItemTypeEnum.VirtualItem:
            case AppItem.ItemTypeEnum.CrewChipDto:
            case AppItem.ItemTypeEnum.PointType:
            case AppItem.ItemTypeEnum.Experience:
            case AppItem.ItemTypeEnum.Vigour:
            case AppItem.ItemTypeEnum.RealItem:
            case AppItem.ItemTypeEnum.Props:
            case AppItem.ItemTypeEnum.Embed:
            case AppItem.ItemTypeEnum.SkillBook:
            case AppItem.ItemTypeEnum.TreasureMap:
            case AppItem.ItemTypeEnum.PassiveSkillBook:
            case AppItem.ItemTypeEnum.Horn:
            case AppItem.ItemTypeEnum.GiftPackage:
            case AppItem.ItemTypeEnum.CrewGift:
            case AppItem.ItemTypeEnum.ContestInvitation:
            case AppItem.ItemTypeEnum.FormationBook:
                _tipsCtrl = OpenGeneralItemTips(dto.item as GeneralItem);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
            case AppItem.ItemTypeEnum.Medallion:
                _tipsCtrl = OpenMedallionTips(dto);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
            case AppItem.ItemTypeEnum.Engrave:
                _tipsCtrl = OpenRuneTips(dto.item as GeneralItem);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
            case AppItem.ItemTypeEnum.CarryCook:
            case AppItem.ItemTypeEnum.GrailCook:
                _tipsCtrl = OpenAssistSkillPropTips(dto);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
            case AppItem.ItemTypeEnum.Equipment:
                var _equipmentDto = EquipmentMainDataMgr.DataMgr.MakeEquipmentDto(dto);
                var isEquip = EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(_equipmentDto);
                var curEquipmentDto = EquipmentMainDataMgr.DataMgr.GetSameEquipmentByPart(_equipmentDto);
                //装备在身上
                if (isEquip && curEquipmentDto != null)
                {
                    var embedVo = EquipmentMainDataMgr.DataMgr.GetEmbedInfoByPart((Equipment.PartType)curEquipmentDto.partType);
                    _tipsCtrl = OpenEquipmentTips(_equipmentDto, embedVo);
                    //_tipsCtrl.SwapCompare_MainPostion();
                }
                else
                {
                    _tipsCtrl = OpenEquipmentTips(_equipmentDto, null);
                }
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
            case AppItem.ItemTypeEnum.Quartz:
                _tipsCtrl = OpenQuartzTips(dto);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
        }

        return _tipsCtrl;
    }

    public static IBaseTipsController OpenTipsWithGeneralItem(GeneralItem dto, Vector3 tipsShowPos)
    {
        var itemType = AppItem.ItemTypeEnum.Unknown;
        if(dto as RealItem != null)
            itemType = (AppItem.ItemTypeEnum)(dto as RealItem).itemType;
        else if(dto as AppVirtualItem != null)
            itemType = (AppItem.ItemTypeEnum)(dto as AppVirtualItem).itemType;
        else if(dto as AppMissionItem != null)
            itemType = (AppItem.ItemTypeEnum)(dto as AppMissionItem).itemType;

        IBaseTipsController _tipsCtrl = null;
        switch (itemType)
        {
            case AppItem.ItemTypeEnum.Virtual:
            case AppItem.ItemTypeEnum.MissionItem:
            case AppItem.ItemTypeEnum.VirtualItem:
            case AppItem.ItemTypeEnum.CrewChipDto:
            case AppItem.ItemTypeEnum.PointType:
            case AppItem.ItemTypeEnum.Experience:
            case AppItem.ItemTypeEnum.Vigour:
            case AppItem.ItemTypeEnum.RealItem:
            case AppItem.ItemTypeEnum.Props:
            case AppItem.ItemTypeEnum.Embed:
            case AppItem.ItemTypeEnum.SkillBook:
            case AppItem.ItemTypeEnum.Medallion:
            case AppItem.ItemTypeEnum.Equipment:
            case AppItem.ItemTypeEnum.TreasureMap:
            case AppItem.ItemTypeEnum.PassiveSkillBook:
            case AppItem.ItemTypeEnum.Horn:
            case AppItem.ItemTypeEnum.GiftPackage:
            case AppItem.ItemTypeEnum.ContestInvitation:
            case AppItem.ItemTypeEnum.FormationBook:
            case AppItem.ItemTypeEnum.CrewGift:
            case AppItem.ItemTypeEnum.Quartz:
                _tipsCtrl = OpenGeneralItemTips(dto);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
            case AppItem.ItemTypeEnum.Engrave:
                _tipsCtrl = OpenRuneTips(dto);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
            case AppItem.ItemTypeEnum.CarryCook:
            case AppItem.ItemTypeEnum.GrailCook:
                _tipsCtrl = OpenAssistSkillPropTips(dto);
                _tipsCtrl.SetTipsPosition(tipsShowPos);
                break;
        }

        return _tipsCtrl;
    }
}

