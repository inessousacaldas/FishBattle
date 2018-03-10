// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BaseTipsController.cs
// Author   : xjd
// Created  : 9/6/2017 3:14:28 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using AppDto;

public partial interface IBaseTipsController
{
    TipsBtnPanelViewController SetBtnView(string left = "取消", string right = "确定", Action leftClick = null, Action rightClick = null);

    TipsBtnPanelViewController SetBtnView(Dictionary<string, Action> leftDic, string right, Action rightClick);

    void ReSetAllPos(TipsBtnPanelViewController ctrl);

    void UpdateView(GeneralItem dto, string price, long time);

    void SetTipsPosition(Vector3 position, bool isLeft = true);

    void SetTipsCompareType(bool isLeft = true);
    void SwapCompare_MainPostion();

    void SetCompareActive(bool active);
    void Close();
    void SetBtnPressClose(bool b);
}

public partial class BaseTipsController    {

    public static IBaseTipsController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IBaseTipsController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IBaseTipsController;
            
        return controller;        
    }

	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        SetCompareActive(false);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnClose;
    }

    protected override void RemoveCustomEvent ()
    {
        UICamera.onClick -= OnClose;
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    private bool _compareActive;
    private int _titleHeight;
    private const int MaxHeight = 440;//tips最大高度
    private const int MinHeight = 190;//tips最小高度
    private const int GapConst = 20;
    private const int InBgHeighter = 12; //inBg Anchor bottom
    private TipsBtnPanelViewController _btnPanelCtrl = null;  //按钮panel 用来判断cameraclick
    private bool _btnPressClose = true;


    //action=null默认关闭Tips
    public virtual void UpdateView(GeneralItem general, string price, long time)
    {
        if (general == null)
        {
            Close();
            return;
        }

        var realitem = general as RealItem;
        var appitem = general as AppItem;
        var propsitem = general as Props;
        var virtualitem = general as AppVirtualItem;
        var missionItem = general as AppMissionItem;
        if (realitem != null)
            SetTitle(general.icon, appitem == null ? 0 : appitem.quality, general.name, realitem.itemType, propsitem == null ? 0 : propsitem.minGrade);
        else if (virtualitem != null)
            SetTitle(general.icon, virtualitem.quality, general.name, virtualitem.itemType, 0);
        else if(missionItem != null)
            SetTitle(general.icon, missionItem.quality, general.name, missionItem.itemType, 0);

        SetLineView(false);
        //SetLabelView(price);
        SetLabelView(general.description);
        //SetLabelView(time.ToString());
    }

    public TipsTitleViewController SetTitle(string icon, int quality, string name, int type, int lv = 0, bool isEquip = false,bool isBind = false)
    {
        var ctrl = AddChild<TipsTitleViewController, TipsTitleView>(View.TitleAnchor_Transform.gameObject, TipsTitleView.NAME);
        ctrl.UpdateView(icon, quality, name, type, lv, isEquip,isBind);
        //View.Table_UITable.Reposition();

        _titleHeight = ctrl.GetHeight();
        return ctrl;
    }
      
    public TipsTitleViewController SetCompareTitle(string icon, int quality, string name, int type, int lv = 0, bool isEquip = false, bool isBind = false)
    {
        var ctrl = AddChild<TipsTitleViewController, TipsTitleView>(View.ComTitleAnchor_Transform.gameObject, TipsTitleView.NAME);
        ctrl.UpdateView(icon, quality, name, type, lv, isEquip,isBind);
        //View.ComTable_UITable.Reposition();
        return ctrl;
    }

    public void SetLabelView(string str)
    {
        var ctrl = AddChild<TipsLabelItemController, TipsLabelItem>(View.Table_UITable.gameObject, TipsLabelItem.NAME);
        ctrl.UpdateView(str);

        View.Table_UITable.Reposition();
    }

    public void SetLabAndSprView(string str, List<string> list, List<int> quaList=null)
    {
        var ctrl = AddChild<TipsLabAndSprViewController, TipsLabAndSprView>(View.Table_UITable.gameObject, TipsLabAndSprView.NAME);
        ctrl.UpdateView(str,list, quaList);

        View.Table_UITable.Reposition();
    }

    public void SetLineView(bool isLine = true)
    {
        var ctrl = AddChild<TipsLineController, TipsLine>(View.Table_UITable.gameObject, TipsLine.NAME);
        ctrl.SetLineOrSpace(isLine);

        View.Table_UITable.Reposition();
    }

    public void SetQuartzItemView(QuartzExtraDto dto)
    {
        var ctrl = AddChild<TipsQuartzViewController, TipsQuartzView>(View.Table_UITable.gameObject, TipsQuartzView.NAME);
        ctrl.UpdateView(dto);

        View.Table_UITable.Reposition();
    }

    public void AddProperty(List<CharacterPropertyDto> dtos, string color= ColorConstantV3.Color_White_Str)
    {
        dtos.ForEachI((x, i) =>
        {
            var cb = DataCache.getDtoByCls<CharacterAbility>(x.propId);
            var name = cb.name;
            var value = x.propValue;
            string text = string.Empty;
            if(cb.per)
                text = string.Format("{0} : {1}%", name, value * 100);
            else
                text = string.Format("{0} : {1}", name, (int)value);
            
            SetLabelView(text.WrapColor(color));
        });
    }

    public TipsBtnPanelViewController SetBtnView(string left = "取消",
        string right = "确定",
        Action leftClick= null, 
        Action rightClick= null)
    {
        if (leftClick == null && rightClick == null)
            return null;

        var ctrl = AddChild<TipsBtnPanelViewController, TipsBtnPanelView>(View.Bg_UISprite.gameObject, TipsBtnPanelView.NAME);
        if (_btnPressClose)
        {
            rightClick += Close;
            if(leftClick != null)
                leftClick += Close;
        }
        ctrl.UpdateView(left, right, leftClick, rightClick);
        _btnPanelCtrl = ctrl;

        return ctrl;
    }

    public TipsBtnPanelViewController SetBtnView(Dictionary<string, Action> leftDic, string right, Action rightClick)
    {
        if (leftDic == null && rightClick == null)
            return null;

        if (leftDic == null)
            return SetBtnView("", right, rightClick: rightClick);

        var ctrl = AddChild<TipsBtnPanelViewController, TipsBtnPanelView>(View.Bg_UISprite.gameObject, TipsBtnPanelView.NAME);
        if (_btnPressClose)
        {
            rightClick += Close;
            var dicList = new List<string>(leftDic.Keys);
            for (int i = 0; i < dicList.Count; i++)
            {
                leftDic[dicList[i]] += Close;
            }
        }
        ctrl.UpdateView(leftDic, right, rightClick, View.ScrollView_UIScrollView.GetComponent<UIPanel>().depth+1);
        _btnPanelCtrl = ctrl;

        return ctrl;
    }

    public void SetBtnPressClose(bool isClose)
    {
        _btnPressClose = isClose;
    }

    /*
     private const int MaxHeight = 440;//tips最大高度
    private const int MinHeight = 190;//tips最小高度
    private const int GapConst = 20;
     */
    //计算各组件位置
    public void ReSetAllPos(TipsBtnPanelViewController ctrl)
    {
        View.Table_UITable.Reposition();
        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.Table_UITable.transform);
        var height = ctrl == null ? b.size.y + _titleHeight + GapConst : b.size.y + _titleHeight + ctrl.GetHeight();
        height = height > MaxHeight ? MaxHeight : height;
        height = height < MinHeight ? MinHeight : height;
        var inHeight = ctrl == null ? height - _titleHeight : height - _titleHeight - ctrl.GetHeight();
        View.Bg_UISprite.SetDimensions((int)b.size.x, (int)height);
        View.Pos_UIWidget.SetDimensions((int)b.size.x, (int)height);

        if (ctrl != null)
        {
            View.ScrollView_UIScrollView.GetComponent<UIPanel>().SetAnchor(View.Bg_UISprite.gameObject, 0, 70, 0, -83);
            View.DragScrollview_UIDragScrollView.GetComponent<UIWidget>().SetAnchor(View.Bg_UISprite.gameObject, 0, 70, 0, -83);
            View.InBg_UISprite.GetComponent<UIWidget>().SetAnchor(View.Bg_UISprite.gameObject, 5, 70, -5, -83);
            ctrl.gameObject.transform.localPosition = new Vector3(View.Bg_UISprite.width / 2, ctrl.GetHeight() / 2 - height, 0);
        }
        else
        {
            View.ScrollView_UIScrollView.GetComponent<UIPanel>().SetAnchor(View.Bg_UISprite.gameObject, 0, InBgHeighter, 0, -83);
            View.DragScrollview_UIDragScrollView.GetComponent<UIWidget>().SetAnchor(View.Bg_UISprite.gameObject, 0, InBgHeighter, 0, -83);
            View.InBg_UISprite.GetComponent<UIWidget>().SetAnchor(View.Bg_UISprite.gameObject, 5, InBgHeighter, -5, -83);
        }

        //对比部分
        
        View.ComTable_UITable.Reposition();
        Bounds bCom = NGUIMath.CalculateRelativeWidgetBounds(View.ComTable_UITable.transform);
        height = bCom.size.y + _titleHeight + GapConst;
        height = height > MaxHeight ? MaxHeight : height;
        height = height < MinHeight ? MinHeight : height;
        inHeight = height - _titleHeight;
        View.ComBg_UISprite.SetDimensions((int)bCom.size.x, (int)height);
        View.ComPos_UIWidget.SetDimensions((int)bCom.size.x, (int)height);
        View.CompareBg_UISprite.SetDimensions((int)b.size.x, (int)inHeight);
    }
    public void SetCompareActive(bool active)
    {
        _compareActive = active;
        View.ComBg_UISprite.gameObject.SetActive(active);
    }
    /*对比tips部分*/
    public void SetCompareLabelView(string str)
    {
        var ctrl = AddChild<TipsLabelItemController, TipsLabelItem>(View.ComTable_UITable.gameObject, TipsLabelItem.NAME);
        ctrl.UpdateView(str);

        View.ComTable_UITable.Reposition();
    }

    public void SetCompareLabAndSprView(string str, List<string> list, List<int> qualityList=null)
    {
        var ctrl = AddChild<TipsLabAndSprViewController, TipsLabAndSprView>(View.ComTable_UITable.gameObject, TipsLabAndSprView.NAME);
        ctrl.UpdateView(str, list, qualityList);

        View.ComTable_UITable.Reposition();
    }

    public void SetCompareLineView(bool isLine = true)
    {
        var ctrl = AddChild<TipsLineController, TipsLine>(View.ComTable_UITable.gameObject, TipsLine.NAME);
        ctrl.SetLineOrSpace(isLine);

        View.ComTable_UITable.Reposition();
    }

    public void AddCompareProperty(List<CharacterPropertyDto> dtos, string color = ColorConstantV3.Color_White_Str)
    {
        dtos.ForEachI((x, i) =>
        {
            var cb = DataCache.getDtoByCls<CharacterAbility>(x.propId);
            var name = cb.name;
            var value = x.propValue;
            string text = string.Format("{0} : {1}", name, (int)value);
            SetCompareLabelView(text.WrapColor(color));
        });
    }

    public void SetTipsPosition(Vector3 position, bool compareIsLeft = true)
    {
        //位置若导致tips超出屏幕 作限制
        var panel = GetComponent<UIPanel>();
        if(panel != null)
        if (position.y - View.Bg_UISprite.height < -panel.height/2)
            position.y = - panel.height/2 + View.Bg_UISprite.height;
        if (position.y > panel.height/2)
            position.y = panel.height/2;
        if (position.x + View.Bg_UISprite.width > panel.width/2)
            position.x = panel.width/2 - View.Bg_UISprite.width;
        if (position.x < -panel.width/2)
            position.x = _compareActive ? -panel.width/2 + View.ComBg_UISprite.width : -panel.width/2;

        View.Pos_UIWidget.transform.localPosition = position;
        if (compareIsLeft)
            View.ComPos_UIWidget.transform.localPosition = new Vector3(position.x - View.ComBg_UISprite.width, position.y, position.z);
        else
            View.ComPos_UIWidget.transform.localPosition = new Vector3(position.x + View.ComBg_UISprite.width, position.y, position.z);
    }

    public void SetTipsCompareType(bool isLeft = true)
    {
        var width = View.Bg_UISprite.width;
        var position = View.Bg_UISprite.transform.localPosition;
        if (isLeft)
            View.ComPos_UIWidget.transform.localPosition = new Vector3(position.x - width, position.y, position.z);
        else
            View.ComPos_UIWidget.transform.localPosition = new Vector3(position.x + width, position.y, position.z);
    }
    //交换对比 / 主要的位置
    public void SwapCompare_MainPostion()
    {
        var pos_a = View.ComPos_UIWidget.transform.localPosition;
        View.ComPos_UIWidget.transform.localPosition = View.Pos_UIWidget.transform.localPosition;
        View.Pos_UIWidget.transform.localPosition = pos_a;
    }

    //有其他操作也会关闭tips
    public void Close()
    {
        UIModuleManager.Instance.CloseModule(BaseTipsView.NAME);
    }

    private void OnClose(GameObject go)
    {
        if (View == null)
            return;

        UIPanel panel = UIPanel.Find(go.transform);
        if (_btnPanelCtrl != null && panel == _btnPanelCtrl.gameObject.GetComponent<UIPanel>())
            return;

        if (panel != View.GetComponent<UIPanel>() 
            && panel != View.ScrollView_UIScrollView.GetComponent<UIPanel>()
            && panel != View.ComScrollView_UIScrollView.GetComponent<UIPanel>()
            )
            Close();
    }
}
