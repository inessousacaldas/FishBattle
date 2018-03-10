﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class BracerLvView : BaseView
{
    public const string NAME ="BracerLvView";

    #region Element Bindings

    /// bind gameobject
    public UIButton CloseBtn_UIButton;
    public UISprite Icon_UISprite;
    public UILabel LvName_UILabel;
    public UILabel Info_1_UILabel;
    public UILabel Num_1_UILabel;
    public UILabel Info_2_UILabel;
    public UILabel Num_2_UILabel;
    public UILabel Info_3_UILabel;
    public UILabel Num_3_UILabel;
    public UILabel Info_4_UILabel;
    public UILabel Num_4_UILabel;
    public UILabel Info_5_UILabel;
    public UILabel Num_5_UILabel;
    public UILabel Info_6_UILabel;
    public UILabel Num_6_UILabel;
    public UIGrid Grid_UIGrid;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        CloseBtn_UIButton = root.FindScript<UIButton>("CloseBtn");
        Icon_UISprite = root.FindScript<UISprite>("Left/Icon");
        LvName_UILabel = root.FindScript<UILabel>("Left/LvBg/LvName");
        Info_1_UILabel = root.FindScript<UILabel>("Left/Info_1");
        Num_1_UILabel = root.FindScript<UILabel>("Left/Info_1/Num_1");
        Info_2_UILabel = root.FindScript<UILabel>("Left/Info_2");
        Num_2_UILabel = root.FindScript<UILabel>("Left/Info_2/Num_2");
        Info_3_UILabel = root.FindScript<UILabel>("Left/Info_3");
        Num_3_UILabel = root.FindScript<UILabel>("Left/Info_3/Num_3");
        Info_4_UILabel = root.FindScript<UILabel>("Left/Info_4");
        Num_4_UILabel = root.FindScript<UILabel>("Left/Info_4/Num_4");
        Info_5_UILabel = root.FindScript<UILabel>("Left/Info_5");
        Num_5_UILabel = root.FindScript<UILabel>("Left/Info_5/Num_5");
        Info_6_UILabel = root.FindScript<UILabel>("Left/Info_6");
        Num_6_UILabel = root.FindScript<UILabel>("Left/Info_6/Num_6");
        Grid_UIGrid = root.FindScript<UIGrid>("ScrollView/Grid");

    }
    #endregion
}
