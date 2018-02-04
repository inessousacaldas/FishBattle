using System;
using AppDto;
using UnityEngine;

public partial interface IBackpackView
{
    //在这里添加自定义接口
    GameObject TabGridAnchor { get; }
    GameObject ItemsPosAnchor { get; }
    GameObject ModelAnchorGO { get; }
    UIGrid LeftEquipGrid { get; }
    UIGrid RightEquipGrid { get; }
    void SetPlayerRanking(int num);

    void UpdatePlayerInfo(IPlayerModel model);
    void UpdateView(IBackpackData _data);

    GameObject WareHousePosAnchor { get; }
    
}

public sealed partial class BackpackView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {
        b_sortLabel_UILabel.text = "整理";
        compositeLabel_UILabel.text = "合成";
        decomposeLabel_UILabel.text = "分解";

        saveItemLabel_UILabel.text = "存入";
        getItemLabel_UILabel.text = "取出";
        w_sortLabel_UILabel.text = "整理";
    }

    protected override void OnDispose()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IBackpackData _data)
    {
        var data = _data.BackpackViewData;

        switch (data.CurTab)
        {
            case BackpackViewTab.Backpack:
                ShowBackPack(true);
                ShowWarehouse(false);
                break;
            case BackpackViewTab.Warehouse:
                ShowBackPack(false);
                ShowWarehouse(true);
                break;
        }
    }

    public GameObject TabGridAnchor {
        get { return WinTabGroup_UIGrid.gameObject; }
    }

    public GameObject ItemsPosAnchor {
        get { return ItemsPos; }
    }

    public GameObject ModelAnchorGO {
        get { return ModelAnchor; }
    }

    public UIGrid LeftEquipGrid {
        get { return LEquip_UIGrid; }
    }
    public UIGrid RightEquipGrid {
        get { return REquip_UIGrid; }
    }

    public void SetPlayerRanking(int num)
    {
        var n = Math.Max(0, num);
        PlayerRankingLabel_UILabel.text =  num.ToString();
    }

    private void ShowBackPack(bool show)
    {
        if(show)
        {
            TitleNameSprite_UISprite.spriteName = "ModuleTitle_pack";
            TitleNameSprite_UISprite.MakePixelPerfect();
        }   
        LeftGroup.SetActive(show);
        Composite_UIButton.gameObject.SetActive(show);
        DecomposeBtn_UIButton.gameObject.SetActive(show);
    }

    private void ShowWarehouse(bool show)
    {
        if(show)
        {
            TitleNameSprite_UISprite.spriteName = "ModuleTitle_storehouse";
            TitleNameSprite_UISprite.MakePixelPerfect();
        }
        WareHouseGroup.SetActive(show);
    }

    public void UpdatePlayerInfo(IPlayerModel model)
    {
        CopperValueLabel_UILabel.text = model.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.GOLD).ToString();
        SiliverValueLabel_UILabel.text = model.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER).ToString();
    }

    public GameObject WareHousePosAnchor {
        get { return WareHouseAnchor; }
    }

}