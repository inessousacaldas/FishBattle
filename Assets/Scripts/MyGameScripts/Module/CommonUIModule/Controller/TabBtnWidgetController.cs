// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TabBtnWidgetController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface ITabBtnController : IMonolessViewController
{
    void SetSelected(bool active);
    void SetBtnLbl(string name);
    void SetBtnLblFont(
        int selectSize
        , string selectColor
        , int normalSize
        , string normalColor);

    UniRx.IObservable<Unit> OnTabClick { get; }
    void SetBtnImages(string normal = "", string select = "");
    void SetBtnDepth(int depth);
}

public class GenericEnumComparer<T> : IEqualityComparer<T> where T : struct
{
    public bool Equals(T x, T y)
    {
        return x.Equals(y);
    }

    public int GetHashCode(T type)
    {
        return type.GetHashCode();
    }
}

public static class TabBtnHelper
{
    // 命名不规范，懒得自己改名打包tp，就这样吧 -－－fish 2017－05.31
    private static Dictionary<TabbtnPrefabPath, string> ImageDic
        = new Dictionary<TabbtnPrefabPath, string>(new GenericEnumComparer<TabbtnPrefabPath>())
        {
            //TabBtnWidget_H3_SHORT
            {TabbtnPrefabPath.TabBtnWidget_H3_SHORT, "button-001-selected"}
        };

    public static string GetBtnImages(TabbtnPrefabPath widgetName)
    {
        var name = string.Empty;
        ImageDic.TryGetValue(widgetName, out name);
        return name;
    }
}

public partial class TabBtnWidgetController : ITabBtnController
{
    private Subject<Unit> tabClickEvt;

    private string selectImage = string.Empty;
    private string unselectImage = string.Empty;

    //---设置默认值  xush
    private int selectSize = 26;
    private Color selectColor = ColorConstantV3.Color_VerticalSelectColor;
    private int normalSize = 26;
    private Color normalColor = ColorConstantV3.Color_VerticalUnSelectColor;

    private bool isSelcet = false;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (View.redFlag != null)
            View.redFlag.SetActive(false);
        UIHelper.AddButtonClickSound(this.gameObject, "sound_UI_tab_click");

        //unselectImage = View.TabBtnWidget_UISprite.spriteName;
        //selectImage = unselectImage.Replace("_Off", "_On");
        selectImage = View.TabBtnWidget_UISprite.spriteName;
        unselectImage = selectImage.Replace("_On", "_Off");
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        tabClickEvt = View.gameObject.OnClickAsObservable();
    }

    protected override void OnDispose()
    {
        tabClickEvt = tabClickEvt.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    //public void SetBtnLblFont(
    //    int selectSize = 22,
    //    string selectColor = ColorConstantV3.Color_VerticalSelectColor_Str,
    //    int normalSize = 20,
    //    string normalColor = ColorConstantV3.Color_VerticalUnSelectColor_Str)
    public void SetBtnLblFont(
        int selectSize = 22, 
        string selectColor = ColorConstantV3.Color_VerticalSelectColor_Str, 
        int normalSize = 20, 
        string normalColor =ColorConstantV3.Color_VerticalUnSelectColor_Str)
    {
        this.normalColor = NGUIText.ParseColor24(normalColor, 0);
        this.selectColor = NGUIText.ParseColor24(selectColor, 0);
        this.selectSize = selectSize;
        this.normalSize = normalSize;
        SetSelected(isSelcet);
    }

    public UniRx.IObservable<Unit> OnTabClick
    {
        get { return tabClickEvt; }
    }

    public void SetBtnLbl(string name)
    {
        _view.btnLbl_UILabel.text = name;
    }

    public void SetSelected(bool _selected)
    {
        isSelcet = _selected;
        View.TabBtnWidget_UISprite.spriteName = _selected
            ? selectImage
            : unselectImage;
        _view.btnLbl_UILabel.fontSize = _selected
            ? selectSize
            : normalSize;

        _view.btnLbl_UILabel.color = _selected
            ? selectColor
            : normalColor;

        //View.TabBtnWidget_UISprite.depth = _selected ? 6 : 5;   //被选中的要深度较高
    }

    public void SetBtnDepth(int depth)
    {
        _view.TabBtnWidget_UISprite.depth = depth;
    }

    public void SetBtnImages(string normal = "", string select = "")
    {
        selectImage = select;
        unselectImage = normal;
    }
}
