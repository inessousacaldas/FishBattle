﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed partial class BaseTipsView : BaseView
{
    public const string NAME ="BaseTipsView";

    #region Element Bindings

    /// bind gameobject
    public UIWidget Pos_UIWidget;
    public UISprite Bg_UISprite; 
    public Transform TitleAnchor_Transform;
    public UIScrollView ScrollView_UIScrollView;
    public UIWidget ScrollView_UIWidget;
    public UIDragScrollView DragScrollview_UIDragScrollView;
    public UIWidget BtnAnchor_UIWidget;
    public UITable Table_UITable;
    public UITable ComTable_UITable;
    public UIWidget ComPos_UIWidget;
    public UISprite ComBg_UISprite;
    public Transform ComTitleAnchor_Transform;
    public UIScrollView ComScrollView_UIScrollView;
    public UIDragScrollView ComDragScrollview_UIDragScrollView;
    public UIWidget DragScrollView_UIWidget;
    public UISprite InBg_UISprite;
    public UISprite CompareBg_UISprite;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        Pos_UIWidget = root.FindScript<UIWidget>("Pos");
        Bg_UISprite = root.FindScript<UISprite>("Pos/Bg");
        TitleAnchor_Transform = root.FindTrans("Pos/Bg/TitleAnchor");
        ScrollView_UIScrollView = root.FindScript<UIScrollView>("Pos/Bg/ScrollView");
        ScrollView_UIWidget = root.FindScript<UIWidget>("Pos/Bg/ScrollView");
        DragScrollview_UIDragScrollView = root.FindScript<UIDragScrollView>("Pos/Bg/ScrollViewTrigger");
        DragScrollView_UIWidget = root.FindScript<UIWidget>("Pos/Bg/ScrollViewTrigger");
        Table_UITable = root.FindScript<UITable>("Pos/Bg/ScrollView/InTable");
        BtnAnchor_UIWidget = root.FindScript<UIWidget>("Pos/Bg/BtnAnchor");
        ComPos_UIWidget = root.FindScript<UIWidget>("ComparePos");
        ComBg_UISprite = root.FindScript<UISprite>("ComparePos/CompareBg");
        ComTable_UITable = root.FindScript<UITable>("ComparePos/CompareBg/ScrollView/InTable");
        ComTitleAnchor_Transform = root.FindTrans("ComparePos/CompareBg/TitleAnchor");
        ComScrollView_UIScrollView = root.FindScript<UIScrollView>("ComparePos/CompareBg/ScrollView");
        ComDragScrollview_UIDragScrollView = root.FindScript<UIDragScrollView>("ComparePos/CompareBg/ScrollViewTrigger");
        InBg_UISprite = root.FindScript<UISprite>("Pos/Bg/InBg");
        CompareBg_UISprite = root.FindScript<UISprite>("ComparePos/CompareBg/InBg");
    }
    #endregion
}
