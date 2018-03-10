// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillItemCellController.cs
// Author   : DM-PC092
// Created  : 8/24/2017 7:31:54 PM
// Porpuse  : 
// **********************************************************************

using AppDto;


public partial interface ICrewSkillItemCellController
{
    bool GetItemInBag { get; }
    PassiveSkillBook PsvBookData { get; }
}

public partial class CrewSkillItemCellController    {

    public static ICrewSkillItemCellController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, ICrewSkillItemCellController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as ICrewSkillItemCellController;
            
        return controller;        
    }

    private PassiveSkillBook psvBookData;       
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        psvBookData = null;
    }
    #region 研修

    public void UpdateView(CrewSkillCraftsVO vo)
    {
        if (vo == null || vo.cfgVO == null) return;
        View.lblGrade_UILabel.text = "";
        View.lblNum_UILabel.text = "";
        View.isS_Transform.gameObject.SetActive(vo.IsSuperCrafts);
        //UIHelper.SetSkillIcon(View.spIcon_UISprite, vo.Icon);
        UIHelper.SetUITexture(View.spIcon_Tex, vo.Icon, false);
        UIHelper.SetSkillQualityIcon(View.bg_UISprite, vo.cfgVO.quality);
        View.spIcon_UISprite.gameObject.SetActive(false);
        View.spIcon_Tex.gameObject.SetActive(true);
    }
    
    public void UpdateNone()
    {
        View.lblGrade_UILabel.text = "";
        View.lblNum_UILabel.text = "";
        UIHelper.SetSkillQualityIcon(View.bg_UISprite, 1);
        View.isS_Transform.gameObject.SetActive(false);
        View.spIcon_UISprite.gameObject.SetActive(false);
        View.spIcon_Tex.gameObject.SetActive(false);
    }
    #endregion
    

    #region 技巧
    private bool isItemInBag = false;
    public void SetItemInBag(bool set)
    {
        isItemInBag = set;
    }
    public bool GetItemInBag
    {
        get { return isItemInBag; }
    }
    public void UpdatePsvView(string icon,string num,int quality)
    {
        UIHelper.SetItemIcon(View.spIcon_UISprite, icon);
        UIHelper.SetSkillQualityIcon(View.bg_UISprite, quality);
        View.lblNum_UILabel.text = num;
        View.lblNum_UILabel.gameObject.SetActive(true);
        View.spIcon_UISprite.gameObject.SetActive(true);
        View.spIcon_Tex.gameObject.SetActive(false);
    }

    public void SetPsvBookData(PassiveSkillBook book)
    {
        psvBookData = book;
    }

    public PassiveSkillBook PsvBookData { get { return psvBookData; } }
    #endregion

    public void ShowSel()
    {
        View.spSelected_Transform.gameObject.SetActive(true);
    }

    public void HideSel()
    {
        View.spSelected_Transform.gameObject.SetActive(false);
    }

    public void SetParent(UnityEngine.Transform p)
    {
        View.transform.parent = p;
    }
}
