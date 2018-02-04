// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildHasJoinViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IGuildHasJoinViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}

    #region 信息界面接口
    UniRx.IObservable<Unit> OnmemberBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OngradeBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> Onjob_UIButtonClick{get;}
     UniRx.IObservable<Unit> Oncontribution_UIButtonClick{get;}
     UniRx.IObservable<Unit> Ononline_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnmemberInfoBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnguildInfoBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnsendMesBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnaddFriendBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnmodifyJobBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnkickMemBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnguildListBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnleaveBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnrequestListBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnbackGuildBtn_UIButtonClick{get;}
    UniRx.IObservable<Unit> OnmodifyCloseBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnappointBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnrCloseBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnrClearListBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnrOneKeyAccept_UIButtonClick { get; }
    UniRx.IObservable<IGuildMemItemController> MemItemClick { get; } //公会成员点击
    UniRx.IObservable<IGuildRequesterController> OnrequesterStatEvt { get; }
    UniRx.IObservable<guildModifyJobBtnController> modifyJobClick { get; } //点击职位
    UniRx.IObservable<bool> MemDragBottom { get; }
    UniRx.IObservable<bool> ReqDragBottom { get; }
    #endregion

    #region 建筑
    UniRx.IObservable<Unit> OnexplainBtn_UIButtonClick { get; } //建筑问号点击
    UniRx.IObservable<Unit> Onbg_UIButtonClick { get; }     //建筑bg点击
    UniRx.IObservable<Unit> OnupgradeBtn_UIButtonClick { get; } //建筑升级点击
    UniRx.IObservable<GuildBuildItemController> OncheckBtn_UIButtonClick { get; }   //建筑查看点击
    #endregion

    #region 管理
    UniRx.IObservable<Unit> OnNoticeModificationBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnManifestoModificationBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnMoreMessageLabel_UIButtonClick { get; }
    #endregion
}

public partial class GuildHasJoinViewController:FRPBaseController<
    GuildHasJoinViewController
    , GuildHasJoinView
    , IGuildHasJoinViewController
    , IGuildMainData>
    , IGuildHasJoinViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    #region 信息界面接口
    private Subject<Unit> memberBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnmemberBtn_UIButtonClick{
        get {return memberBtn_UIButtonEvt;}
    }

    private Subject<Unit> gradeBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OngradeBtn_UIButtonClick{
        get {return gradeBtn_UIButtonEvt;}
    }

    private Subject<Unit> job_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> Onjob_UIButtonClick{
        get {return job_UIButtonEvt;}
    }

    private Subject<Unit> contribution_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> Oncontribution_UIButtonClick{
        get {return contribution_UIButtonEvt;}
    }

    private Subject<Unit> online_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> Ononline_UIButtonClick{
        get {return online_UIButtonEvt;}
    }

    private Subject<Unit> memberInfoBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnmemberInfoBtn_UIButtonClick{
        get {return memberInfoBtn_UIButtonEvt;}
    }

    private Subject<Unit> guildInfoBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnguildInfoBtn_UIButtonClick{
        get {return guildInfoBtn_UIButtonEvt;}
    }

    private Subject<Unit> sendMesBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnsendMesBtn_UIButtonClick{
        get {return sendMesBtn_UIButtonEvt;}
    }

    private Subject<Unit> addFriendBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnaddFriendBtn_UIButtonClick{
        get {return addFriendBtn_UIButtonEvt;}
    }

    private Subject<Unit> modifyJobBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnmodifyJobBtn_UIButtonClick{
        get {return modifyJobBtn_UIButtonEvt;}
    }

    private Subject<Unit> kickMemBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnkickMemBtn_UIButtonClick{
        get {return kickMemBtn_UIButtonEvt;}
    }

    private Subject<Unit> guildListBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnguildListBtn_UIButtonClick{
        get {return guildListBtn_UIButtonEvt;}
    }

    private Subject<Unit> leaveBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnleaveBtn_UIButtonClick{
        get {return leaveBtn_UIButtonEvt;}
    }

    private Subject<Unit> requestListBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnrequestListBtn_UIButtonClick{
        get {return requestListBtn_UIButtonEvt;}
    }

    private Subject<Unit> backGuildBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnbackGuildBtn_UIButtonClick{
        get {return backGuildBtn_UIButtonEvt;}
    }

    private Subject<Unit> modifyCloseBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnmodifyCloseBtn_UIButtonClick
    {
        get { return modifyCloseBtn_UIButtonEvt; }
    }

    private Subject<Unit> appointBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnappointBtn_UIButtonClick
    {
        get { return appointBtn_UIButtonEvt; }
    }

    private Subject<Unit> rCloseBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnrCloseBtn_UIButtonClick
    {
        get { return rCloseBtn_UIButtonEvt; }
    }

    private Subject<Unit> rClearListBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnrClearListBtn_UIButtonClick
    {
        get { return rClearListBtn_UIButtonEvt; }
    }

    private Subject<Unit> rOneKeyAccept_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnrOneKeyAccept_UIButtonClick
    {
        get { return rOneKeyAccept_UIButtonEvt; }
    }
    private Subject<IGuildMemItemController> memItemEvt = new Subject<IGuildMemItemController>();
    public UniRx.IObservable<IGuildMemItemController> MemItemClick { get { return memItemEvt; } }   //公会成员点击


    private Subject<IGuildRequesterController> requesterStatEvt = new Subject<IGuildRequesterController>();
    public UniRx.IObservable<IGuildRequesterController> OnrequesterStatEvt { get { return requesterStatEvt; } }    //公会申请列表状态点击

    private Subject<guildModifyJobBtnController> modifyJobEvt = new Subject<guildModifyJobBtnController>();   //公会职位点击
    public UniRx.IObservable<guildModifyJobBtnController> modifyJobClick { get { return modifyJobEvt; } }
    private Subject<bool> memDragBottom = new Subject<bool>();                  //成员列表向下拉数据
    public UniRx.IObservable<bool> MemDragBottom { get { return memDragBottom; } }

    private Subject<bool> reqDragBottom = new Subject<bool>();                  //申请列表向下拉数据
    public UniRx.IObservable<bool> ReqDragBottom { get { return reqDragBottom; } }
    #endregion

    #region 建筑

    private Subject<Unit> explainBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnexplainBtn_UIButtonClick
    {
        get { return explainBtn_UIButtonEvt; }
    }

    private Subject<Unit> bg_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> Onbg_UIButtonClick
    {
        get { return bg_UIButtonEvt; }
    }

    private Subject<Unit> upgradeBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnupgradeBtn_UIButtonClick
    {
        get { return upgradeBtn_UIButtonEvt; }
    }

    private Subject<GuildBuildItemController> checkBtn_UIButtonEvt = new Subject<GuildBuildItemController>();
    public UniRx.IObservable<GuildBuildItemController> OncheckBtn_UIButtonClick
    {
        get { return checkBtn_UIButtonEvt; }
    }

    #endregion

    #region 管理

    private Subject<Unit> NoticeModificationBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnNoticeModificationBtn_UIButtonClick
    {
        get { return NoticeModificationBtn_UIButtonEvt; }
    }

    private Subject<Unit> ManifestoModificationBtn_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnManifestoModificationBtn_UIButtonClick
    {
        get { return ManifestoModificationBtn_UIButtonEvt; }
    }

    private Subject<Unit> MoreMessageLabel_UIButtonEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnMoreMessageLabel_UIButtonClick
    {
        get { return MoreMessageLabel_UIButtonEvt; }
    }

    #endregion
}
