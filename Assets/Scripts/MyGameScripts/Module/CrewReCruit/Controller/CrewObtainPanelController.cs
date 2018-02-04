// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewObtainPanelController.cs
// Author   : DM-PC092
// Created  : 11/25/2017 10:53:42 AM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

public partial interface ICrewObtainPanelController
{
    void Open(List<CrewInfoDto> tCrewInfoDtoList,List<CrewChipDto> tCrewChipDtoList,int tBuyType);
}
public partial class CrewObtainPanelController
{
    private int mCrewID = 20205;
    private int[] mSkillArr ={0,0,0,0};
    private ModelDisplayController _modelDisplayer;
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        _modelDisplayer = AddChild<ModelDisplayController,ModelDisplayUIComponent>(
        View.BodyTexture_UITexture.gameObject
        ,ModelDisplayUIComponent.NAME);
        _modelDisplayer.Init(500,500);
        _modelDisplayer.SetBoxColliderEnabled(true);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        EventDelegate.Add(_view.SkillIcon_1_UIButton.onClick,OnDeleGateClickHandler1);
        EventDelegate.Add(_view.SkillIcon_2_UIButton.onClick,OnDeleGateClickHandler2);
        EventDelegate.Add(_view.SkillIcon_3_UIButton.onClick,OnDeleGateClickHandler3);
        EventDelegate.Add(_view.SkillIcon_4_UIButton.onClick,OnDeleGateClickHandler4);
    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _modelDisplayer.CleanUpModel();
        _disposable = _disposable.CloseOnceNull();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ICrewReCruitData data)
    {
        ServerType GetmServerType=data.IGetmServerType();
        switch(GetmServerType)
        {
            //播放特效
            case ServerType.UpDate:
                Open(data.IGetCrewInfoDtoList().ToList(),data.IGetCrewChipDtoList().ToList(),data.IGetmainRecruitType());
                break;
        }
    }

    public void Open(List<CrewInfoDto> tCrewInfoDtoList,List<CrewChipDto> tCrewChipDtoList,int tBuyType)
    {
        if(ProxyCrewReCruit.crewCurrencyAddTimes <= 0) {
            View.OneBuy_UIButton.isEnabled = false;
        }
        if(tCrewInfoDtoList != null && tCrewInfoDtoList.Count > 0) {
            mCrewID = tCrewInfoDtoList[0].crewId;
            Dictionary<int,GeneralCharactor> tCrewDataDic =  DataCache.getDicByCls<GeneralCharactor>();
            if(tCrewDataDic.ContainsKey(mCrewID))
            {
                Crew tCrewData = tCrewDataDic[mCrewID] as Crew;
                //CrewReCruitDataMgr.CrewReCruitNetMsg.Crew_List();
                switch(tCrewData.property) {
                    case 0:
                        //末知
                        break;
                    case 1:
                        //力量
                        View.PloygonMesh_UISprite.spriteName = "AstralFigure_strength";
                        break;
                    case 2:
                        //魔法
                        View.PloygonMesh_UISprite.spriteName = "AstralFigure_magic";
                        break;
                    case 3:
                        //控制
                        View.PloygonMesh_UISprite.spriteName = "AstralFigure_control";
                        break;
                    case 4:
                        //辅助
                        View.PloygonMesh_UISprite.spriteName = "AstralFigure_ancillary";
                        break;
                }
                if(tCrewData != null)
                {
                    string[] mSkillIconArr ={"","","",""};
                    Dictionary<int,Skill> tCraftsSkillDic = DataCache.getDicByCls<Skill>();
                    for(int i = 0;i < tCrewData.crafts.Count;i++)
                    {
                        if(tCraftsSkillDic[tCrewData.crafts[i]] != null)
                        {
                            mSkillIconArr[i] = tCraftsSkillDic[tCrewData.crafts[i]].icon;
                            mSkillArr[i] = tCrewData.crafts[i];
                        }
                    }
                    if(mSkillIconArr[0] != "") {
                        UIHelper.SetUITexture(View.SkillIcon_1_UISprite,mSkillIconArr[0],true);
                    }
                    if(mSkillIconArr[1] != "")
                    {
                        UIHelper.SetUITexture(View.SkillIcon_2_UISprite,mSkillIconArr[1],true);
                    }
                    if(mSkillIconArr[2] != "")
                    {
                        UIHelper.SetUITexture(View.SkillIcon_3_UISprite,mSkillIconArr[2],true);
                    }
                    if(mSkillIconArr[3] != "")
                    {
                        UIHelper.SetUITexture(View.SkillIcon_4_UISprite,mSkillIconArr[3],true);
                    }
                }
                if(!View.BodyGameobject.activeSelf)
                    View.BodyGameobject.SetActive(true);
                if(!View.ShowDataBg.activeSelf)
                    View.ShowDataBg.SetActive(true);
                if(!View.BG_Show.activeSelf)
                    View.BG_Show.SetActive(true);
                if(View.Chip_Gameobjec.activeSelf)
                    View.Chip_Gameobjec.SetActive(false);
                if(View.BG_Show_2.activeSelf)
                    View.BG_Show_2.SetActive(false);
                View.RewdIcon_Transrom.localPosition = new Vector3(-164,279);
                View.OneBuy_UIButton.transform.localPosition = new Vector3(285,-286,0);
                _modelDisplayer.SetupModel(InitModelStyleInfo(tCrewData.modelId));
                _modelDisplayer.SetModelScale(1f);
                _modelDisplayer.SetModelOffset(-0.15f);
                View.NameLabel_UILabel.text = tCrewData.name;
                _view.DesLabel.gameObject.SetActive(false);
            }
        }

        if(tCrewChipDtoList != null && tCrewChipDtoList.Count > 0)
        {
            if(View.BodyGameobject.activeSelf)
                View.BodyGameobject.SetActive(false);
            if(View.ShowDataBg.activeSelf)
                View.ShowDataBg.SetActive(false);
            if(View.BG_Show.activeSelf)
                View.BG_Show.SetActive(false);
            if(!View.Chip_Gameobjec.activeSelf)
                View.Chip_Gameobjec.SetActive(true);
            if(!View.BG_Show_2.activeSelf)
                View.BG_Show_2.SetActive(true);
            View.RewdIcon_Transrom.localPosition = new Vector3(1,220,0);
            View.OneBuy_UIButton.transform.localPosition = new Vector3(0,-286,0);
            UILabel ChipName = View.Chip_Gameobjec.transform.Find("ChipNameLabel").GetComponent<UILabel>();
            UILabel ChipNumber = View.Chip_Gameobjec.transform.Find("NumberLabel").GetComponent<UILabel>();
            UISprite ChipUISprite =  View.Chip_Gameobjec.transform.Find("Head").GetComponent<UISprite>();
            UISprite ChipUISsr = View.Chip_Gameobjec.transform.Find("SSR").GetComponent<UISprite>();
            ChipName.text = tCrewChipDtoList[0].chip.name;
            ChipNumber.text = tCrewChipDtoList[0].chipAmount.ToString();
            ChipUISprite.spriteName = "head_" + tCrewChipDtoList[0].chip.id;
            ChipUISsr.spriteName ="rare_"+tCrewChipDtoList[0].rare;
            ChipUISsr.MakePixelPerfect();
            _view.DesLabel.gameObject.SetActive(tCrewChipDtoList[0].conversion);
        }
    }

    private ModelStyleInfo InitModelStyleInfo(int id)
    {
        //        GameDebuger.Log("伙伴id=========" + id);
        ModelStyleInfo model = new ModelStyleInfo();
        model.defaultModelId = id;
        return model;
    }


    private void OnDeleGateClickHandler1() {
        if(mSkillArr[0] != 0) {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[0]);
            Vector3 tPos = View.SkillIcon_1_UIButton.transform.parent.localPosition + new Vector3(122,25,0);
            tip.SetTipsPosition(tPos);
        }
    }

    private void OnDeleGateClickHandler2()
    {
        if(mSkillArr[1] != 0) {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[1]);
            Vector3 tPos = View.SkillIcon_2_UIButton.transform.parent.localPosition + new Vector3(122,25,0);
            tip.SetTipsPosition(tPos);
        }
    }
    private void OnDeleGateClickHandler3()
    {
        if(mSkillArr[2] != 0) {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[2]);
            Vector3 tPos = View.SkillIcon_3_UIButton.transform.parent.localPosition + new Vector3(122,25,0);
            tip.SetTipsPosition(tPos);
        }
    }

    private void OnDeleGateClickHandler4()
    {
        if(mSkillArr[3] != 0) {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[3]);
            Vector3 tPos = View.SkillIcon_4_UIButton.transform.parent.localPosition + new Vector3(122,25,0);
            tip.SetTipsPosition(tPos);
        }
    }
}
