﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class QuartzForge : BaseView
{
    public const string NAME ="QuartzForge";

    #region Element Bindings

    /// bind gameobject
    public UIGrid ItemGrid_UIGrid;
    public Transform PropsGrid_Transform;
    public UILabel NeedCashLb_UILabel;
    public UIButton CommonBtn_UIButton;
    public UIButton StrengthBtn_UIButton;
    public UISprite CashSprite_UISprite;
    public UIButton StrengthToggle_UIButton;
    public UISprite hook_UISprite;
    public UILabel TimesLb_UILabel;
    public UIButton TipsBtn_UIButton;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        ItemGrid_UIGrid = root.FindScript<UIGrid>("ScrollView/ScrollView/ItemGrid");
        PropsGrid_Transform = root.FindTrans("PropsGrid");
        NeedCashLb_UILabel = root.FindScript<UILabel>("NeedCashLb");
        CommonBtn_UIButton = root.FindScript<UIButton>("CommonBtn");
        StrengthBtn_UIButton = root.FindScript<UIButton>("StrengthBtn");
        CashSprite_UISprite = root.FindScript<UISprite>("NeedCashLb/CashSprite");
        StrengthToggle_UIButton = root.FindScript<UIButton>("StrengthToggle");
        hook_UISprite = root.FindScript<UISprite>("StrengthToggle/hookBg/hook");
        TimesLb_UILabel = root.FindScript<UILabel>("TimesLb");
        TipsBtn_UIButton = root.FindScript<UIButton>("TipsBtn");

    }
    #endregion
}
