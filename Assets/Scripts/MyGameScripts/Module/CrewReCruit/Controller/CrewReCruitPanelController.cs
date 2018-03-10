// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewReCruitPanelController.cs
// Author   : DM-PC092
// Created  : 11/20/2017 5:02:26 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;
using System.Collections.Generic;

public partial interface ICrewReCruitPanelController
{

}
public partial class CrewReCruitPanelController    {
    private TextTips tFunTooltip_1;
    private TextTips tFunTooltip_2;
    private bool IsFireDara = false;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.NormalDesTipsButton_UIButton.onClick,OnNormalDeleGateClickHandler1);
        EventDelegate.Add(_view.SeniorDesTipsButton_UIButton.onClick,OnSeniorDeleGateClickHandler1);
        EventDelegate.Add(_view.MakeButton_UIButton.onClick,OnMakeButtonDeleGateClickHandler1);
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
        List<CrewRecruitType> tCrewDataDic = DataCache.getArrayByCls<CrewRecruitType>();
        tFunTooltip_1 = DataCache.getDtoByCls<TextTips>(1);
        tFunTooltip_2 = DataCache.getDtoByCls<TextTips>(2);
        int mNormalOneCast =  tCrewDataDic[0].virtualItemCount;
        int mSeniorOneCase = tCrewDataDic[2].virtualItemCount;
        View.NormalOneSpend_UILabel.text = mNormalOneCast.ToString();
        View.SeniorOneSpend_UILabel.text = mSeniorOneCase.ToString();
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ICrewReCruitData data) {
        View.CruitLabel_UILabel.text = "剩余招募次数：" + ProxyCrewReCruit.crewCurrencyAddTimes;
        ServerType GetmServerType=data.IGetmServerType();
        switch(GetmServerType)
        {
            //播放特效
            case ServerType.UpDate:
                PlayerEffect(data.IGetCrewInfoDtoList().ToList(),data.IGetCrewChipDtoList().ToList(),data.IGetmainRecruitType());
                break;
        }
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    private void PlayerEffect(List<CrewInfoDto> tCrewInfoDtoList,List<CrewChipDto> tCrewChipDtoList,int tBuyType)
    {
        //打开特效面板
        CrewObtainPanelDataMgr.CrewObtainPanelLogic.Open(tCrewInfoDtoList,tCrewChipDtoList,tBuyType);
    }

    private void OnNormalDeleGateClickHandler1()
    {
        View.TipLabel_UILabel.transform.position = View.NormalDesTipsButton_UIButton.transform.position;
        View.TipLabel_UILabel.transform.localPosition += new UnityEngine.Vector3(130,50,0);
        View.TipLabel_UILabel.text = tFunTooltip_1.content;
        View.TipPanel_Transform.gameObject.SetActive(!View.TipPanel_Transform.gameObject.activeSelf);
    }

    private void OnSeniorDeleGateClickHandler1()
    {
        View.TipLabel_UILabel.transform.position = View.SeniorDesTipsButton_UIButton.transform.position;
        View.TipLabel_UILabel.transform.localPosition += new UnityEngine.Vector3(100,50,0);
        View.TipLabel_UILabel.text = tFunTooltip_2.content;
        View.TipPanel_Transform.gameObject.SetActive(!View.TipPanel_Transform.gameObject.activeSelf);
    }

    private void OnMakeButtonDeleGateClickHandler1()
    {
        if(View.TipPanel_Transform.gameObject.activeSelf)
            View.TipPanel_Transform.gameObject.SetActive(false);
    }
}
