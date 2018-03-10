﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class FlowerMainView : BaseView
{
    public const string NAME ="FlowerMainView";

    #region Element Bindings

    /// bind gameobject
    public UIButton CloseBtn_UIButton;
    public UIGrid FriendGrid_UIGrid;
    public UIButton SearchBtn_UIButton;
    public UIInput InputField_UIInput;
    public UIGrid FlowerGrid_UIGrid;
    public UIGrid EffGrid_UIGrid;
    public UIInput InputEffLabel_UIInput;
    public GameObject PageTurn;
    public UIButton MaxBtn_UIButton;
    public UILabel DegreeLabel_UILabel;
    public UIButton GiveBtn_UIButton;
    public GameObject NoFriendPanel;
    public UITexture Texture_UITexture;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        CloseBtn_UIButton = root.FindScript<UIButton>("CloseBtn");
        FriendGrid_UIGrid = root.FindScript<UIGrid>("ScrollView/FriendGrid");
        SearchBtn_UIButton = root.FindScript<UIButton>("SearchBtn");
        InputField_UIInput = root.FindScript<UIInput>("InputField");
        FlowerGrid_UIGrid = root.FindScript<UIGrid>("RightUpBg/FlowerGrid");
        EffGrid_UIGrid = root.FindScript<UIGrid>("RightDownBg/EffGrid");
        InputEffLabel_UIInput = root.FindScript<UIInput>("RightLabelBg/InputEffLabel");
        PageTurn = root.FindGameObject("PageTurn");
        MaxBtn_UIButton = root.FindScript<UIButton>("MaxBtn");
        DegreeLabel_UILabel = root.FindScript<UILabel>("DegreeLabel");
        GiveBtn_UIButton = root.FindScript<UIButton>("GiveBtn");
        NoFriendPanel = root.FindGameObject("NoFriendPanel");
        Texture_UITexture = root.FindScript<UITexture>("NoFriendPanel/Texture");

    }
    #endregion
}
