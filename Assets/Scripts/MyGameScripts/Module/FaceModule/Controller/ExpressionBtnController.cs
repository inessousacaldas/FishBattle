// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ExpressionBtnController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;
using Unit = UniRx.Unit;

public partial class ExpressionBtnController : ITabBtnController
{
    private Subject<Unit> tabClickEvt;
    private string originName;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        tabClickEvt = View.gameObject.OnClickAsObservable();
    }

    protected override void OnDispose()
    {
        tabClickEvt = tabClickEvt.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    
    public void SetSelected(bool select)
    {
            View.face_UISprite.spriteName = select ? originName + "_1" : originName;
            View.Expression_UISprite.spriteName = select ? "betton_a" : "betton_b";
            View.face_UISprite.MakePixelPerfect();
            //View.SelectEff_UISprite.enabled = select;
    }

    public void SetBtnLbl(string name)
    {
        View.btnLbl_UILabel.text = name;
    }
    
    public void SetBtnLblFont(int selectSize = 22, 
        string selectColor = ColorConstantV3.Color_VerticalSelectColor_Str, 
        int normalSize = 20, 
        string normalColor = ColorConstantV3.Color_VerticalUnSelectColor_Str)
    {
        ;
    }

    public void SetBtnDepth(int depth)
    {
        
    }

    public UniRx.IObservable<Unit> OnTabClick{
        get {return tabClickEvt;}
    }
    
    public void SetBtnImages(string normal = "", string select = "")
    {
        if (!string.IsNullOrEmpty(select))
            View.SelectEff_UISprite.spriteName = select;
        if (!string.IsNullOrEmpty(normal))
        View.face_UISprite.spriteName = normal;
        originName = normal;
        View.face_UISprite.MakePixelPerfect();
    }
}
