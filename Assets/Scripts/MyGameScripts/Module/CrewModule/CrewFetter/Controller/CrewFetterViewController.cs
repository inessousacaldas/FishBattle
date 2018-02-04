// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFetterViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;
using Debug = GameDebuger;
using System.Collections.Generic;
using System.Linq;


public partial interface ICrewFetterViewController
{
    UniRx.IObservable<ICrewFetterItemVo> OnClickCrewFetterStream { get; }

    UniRx.IObservable<Unit> OnactivateBtn_UIButtonClick { get; }

    UniRx.IObservable<Unit> OnCrewFetterTipBtn_UIButtonlick { get;}

    void SetGoActive(bool b);
}
public partial class CrewFetterViewController: ICrewFetterViewController
{
    List<CrewFetterItemController> crewFetterItemLists = new List<CrewFetterItemController>();

    private Subject<ICrewFetterItemVo> clickCrewFetterBtn_UIButtonEvt = new Subject<ICrewFetterItemVo>();
    CompositeDisposable _disposable;
    public UniRx.IObservable<ICrewFetterItemVo> OnClickCrewFetterStream
    {
        get { return clickCrewFetterBtn_UIButtonEvt; }
    }


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
       
    }
    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        clickCrewFetterBtn_UIButtonEvt = clickCrewFetterBtn_UIButtonEvt.CloseOnceNull();
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }



    public void UpdateView(ICrewFetterData data)
    {
        if (data == null)
        {
            //GameLog.LogCrewFettew("ICrewFetterData isNull");
            //GameDebuger.LogError("CurrenChoice Fetter isNull");
            return;
        }

        UpdateShowFetterItem(data.CrewFetterVoList,data);
        UpdateCrewFetterDetail(data.GetCurCrewFetterVo);
        if (data.GetCurCrewFetterVo != null)
        {
            var crew = DataCache.getDtoByCls<GeneralCharactor>(data.GetCurCrewFetterVo.CrweId) as Crew;
            UIHelper.SetPetIcon(_view.SelfIcon_UISprite, crew.icon);
        }
           
    }

    private void UpdateCrewFetterDetail(ICrewFetterItemVo vo)
    {
        if(vo == null)
        {
            return;
        }
        var crewInfo = vo.CrewFetter;
        UIHelper.SetSkillIcon(_view.FetterAttrIcon_UISprite, vo.CrewFetter.icon);
        bool isColectAll = vo.CrewDic.All(x => x.Value == true);

        if (isColectAll)
        {
            _view.FetterName_UILabel.text = vo.crewFetterDto.currentlevel > 0
                ? string.Format("{0}+{1}", crewInfo.name, vo.crewFetterDto.currentlevel)
                : crewInfo.name;
        }
        else
            _view.FetterName_UILabel.text = crewInfo.name;


        string parnerNames = "羁绊伙伴:    ".WrapColor(ColorConstantV3.Color_Black_Str);
        int i = 0;
       
        foreach(var c in crewInfo.crewids)
        {
            var crew = DataCache.getDtoByCls<GeneralCharactor>(c) as Crew;
            //var crew = c.Key;
            if (i != crewInfo.crewids.Count - 1)
            {
                parnerNames += (crew == null ? "" : crew.name + "、").WrapColor(ColorConstantV3.Color_Blue_Str2);
            }
            else
                parnerNames += (crew == null ? "" : crew.name + "、").WrapColor(ColorConstantV3.Color_Blue_Str2);
            i++;
        }
        _view.FetterParnerInfoNames_UILabel.text = parnerNames;
        _view.FetterDetailInfo_UILabel.text = "羁绊效果:    ".WrapColor(ColorConstantV3.Color_Black_Str) + crewInfo.shortDescription.WrapColor(ColorConstantV3.Color_Blue_Str2);
        _view.activateBtn_Label_UILabel.text = !vo.Acitve ? "激活" : "取消";
    }
    private void UpdateShowFetterItem(List<ICrewFetterItemVo> voList, ICrewFetterData data)
    {
        int itemNumber = voList.Count;
        _disposable.Clear();
        crewFetterItemLists.ForEach(x => {
            x.Hide();     
        });
        for (int i=0;i< itemNumber;i++)
        {
            if(crewFetterItemLists.Count < i + 1)
            {
                
                var con = AddChild<CrewFetterItemController, CrewFetterItem>(_view.FetterList_Transform.gameObject, CrewFetterItem.NAME);
                
                
                crewFetterItemLists.Add(con);
            }
            //int crewFetterId = voList[i].CrewFetterId;    
            var vo = voList[i];
            _disposable.Add(crewFetterItemLists[i].OnRelateParners_UIButtonClick.Subscribe(_ => clickCrewFetterBtn_UIButtonEvt.OnNext(vo)));

            int testAngle = 360 / itemNumber * i;
            if (itemNumber == 4)
                testAngle += 45;
            crewFetterItemLists[i].InitViewSpwan(testAngle);
            crewFetterItemLists[i].Show();
            
            crewFetterItemLists[i].UpdateView(voList[i],voList[i].CrewFetterId == data.GetCurCrewFetterId);
        }
    }

    public void SetGoActive(bool b)
    {
        gameObject.SetActive(b);
    }
}
