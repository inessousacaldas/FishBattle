// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillWindowController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;
using UnityEngine;
public interface ICrewSkillWindowController
{
    UniRx.IObservable<Unit> OnbigBG_UIButtonClick { get; }
    void HideWindow();

    void ShowWindow();

    void UpdateView(string title);

    Transform Trans { get; }

    bool IsShow { get; }

}
public partial class CrewSkillWindowController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    
    public void HideWindow()
    {
        View.Hide();
    }

    public void ShowWindow()
    {
        View.Show();
        //View.GetComponent<UIPanel>().depth = 350;//直接放到此UI界面最高层
    }

    public void UpdateView(string title)
    {
        View.lblTitle_UILabel.text = title;
    }
    public Transform Trans
    {
        get { return View.transform; }
    }
    public bool IsShow
    {
        get { return View.gameObject.activeSelf; }
    }
}
