﻿// ------------------------------------------------------------------------------
//  This code is generated by a tool
// ------------------------------------------------------------------------------

using UnityEngine;

public sealed class GuildDonateView : BaseView
{
    public const string NAME ="GuildDonateView";

    #region Element Bindings

    /// bind gameobject
    public UILabel titleLabel_UILabel;
    public UIRecycledList UIGrid_UIRecycledList;
    public GameObject right;
    public UISprite itemIcon_UISprite;
    public UILabel itemLabel_UILabel;
    public UILabel getItemCount_UILabel;
    public UILabel getContributionCount_UILabel;
    public UILabel donateCount_UILabel;
    public UIButton minusBtn_UIButton;
    public UIButton addBtn_UIButton;
    public UIButton donateBtn_UIButton;
    public UIButton closeBtn_UIButton;
    public UIInput donateCount_UIIpunt;


    protected override void InitElementBinding ()
    {
	    var root = this.gameObject;
        titleLabel_UILabel = root.FindScript<UILabel>("titleLabel");
        UIGrid_UIRecycledList = root.FindScript<UIRecycledList>("left/ItemPanel/UIGrid");
        right = root.FindGameObject("right");
        itemIcon_UISprite = root.FindScript<UISprite>("right/item/itemIcon");
        itemLabel_UILabel = root.FindScript<UILabel>("right/item/itemLabel");
        getItemCount_UILabel = root.FindScript<UILabel>("right/getItem/getItemCount");
        getContributionCount_UILabel = root.FindScript<UILabel>("right/getContribution/getContributionCount");
        donateCount_UILabel = root.FindScript<UILabel>("right/donateCount/donateCount");
        minusBtn_UIButton = root.FindScript<UIButton>("right/donateCount/minusBtn");
        addBtn_UIButton = root.FindScript<UIButton>("right/donateCount/addBtn");
        donateBtn_UIButton = root.FindScript<UIButton>("right/donateBtn");
        closeBtn_UIButton = root.FindScript<UIButton>("closeBtn");
        donateCount_UIIpunt = root.FindScript<UIInput>("right/donateCount/getbg");
    }
    #endregion
}
