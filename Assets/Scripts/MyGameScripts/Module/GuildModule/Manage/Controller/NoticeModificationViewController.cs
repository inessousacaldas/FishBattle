// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  NoticeModificationViewController.cs
// Author   : DM-PC092
// Created  : 1/24/2018 11:00:03 AM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;

public partial interface INoticeModificationViewController
{
    NoticeModificationViewController.NoticeType noticeType { get; }
    string InputValue { get; }
    void SetData(NoticeModificationViewController.NoticeType noticeType, IGuildMainData data);
}
public partial class NoticeModificationViewController
{
    public enum NoticeType
    {
        guildNotic = 0,
        guildManifesto = 1
    }
    private CompositeDisposable _disposable;

    private string _noticTitle = "公会公告";
    private string _manifestoTitle = "公会宣言";

    private NoticeType _noticeType;

    private int _maxInputLength = 100;
    private int _changeNoticeFee = 50;
    private int _changeMemoFee = 500000;
    private int _assets;

    private IGuildMainData data = null;
    public NoticeType noticeType { get { return _noticeType; } }

    public string InputValue { get { return View.TextInput_UIInput.value; } }

    public static INoticeModificationViewController Show<T>(
          string moduleName
          , UILayerType layerType
          , bool addBgMask
          , bool bgMaskClose = true)
          where T : MonoController, INoticeModificationViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as INoticeModificationViewController;

        return controller;
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Clear();
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        _disposable.Add(OnCloseBtn_UIButtonClick.Subscribe(_ => OnCloseBtnClick()));
        _disposable.Add(OnPublishBtn_UIButtonClick.Subscribe(_ =>
        {
            if (ValidateInputString())
            {
                if (_noticeType == NoticeType.guildNotic)
                    GuildMainDataMgr.GuildMainNetMsg.ReqChangeNotice(InputValue);
                else
                    GuildMainDataMgr.GuildMainNetMsg.ReqChangeManifesto(InputValue);
                OnCloseBtnClick();
            }
        }));
    }

    protected override void RemoveCustomEvent()
    {
        
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        data = null;
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {


    }
    public void SetData(NoticeType noticeType, IGuildMainData data)
    {
        _noticeType = noticeType;
        this.data = data;
        View.TitleLabel_UILabel.text = noticeType == NoticeType.guildNotic ? _noticTitle : _manifestoTitle;

    }

    private void OnCloseBtnClick()
    {
        UIModuleManager.Instance.CloseModule(NoticeModificationView.NAME);
    }

    private bool ValidateInputString()
    {
        if (string.IsNullOrEmpty(InputValue))
        {
            TipManager.AddTip("没有输入任何内容，请重新输入");
            return false;
        }
        int length = AppStringHelper.GetGBLength(InputValue);
        if (length > _maxInputLength)
        {
            TipManager.AddTip("字数太多，请精简至50个汉字长度以下");
            return false;
        }
        if(noticeType== NoticeType.guildManifesto)
        {
            if (data == null) return false;
            int assetss = data.GuildDetailInfo.wealthInfo.assets;
            var consume = DataCache.GetStaticConfigValue(AppStaticConfigs.GUILD_ASSETS_CONSUME_FROM_MEMO_NOTICE);
            if(assetss < consume)
            {
                int minus = consume - assetss;
                TipManager.AddTip(string.Format("公会资金不足{0}，无法修改宣言", minus));
                return false;
            }
        }
        if (noticeType == NoticeType.guildNotic)
        {
            var vigour = ModelManager.Player.GetPlayerWealthById((int)AppVirtualItem.VirtualItemEnum.VIGOUR);
            var consume = DataCache.GetStaticConfigValue(AppStaticConfigs.GUILD_VIGOUR_CONSUME_FROM_MEMO_NOTICE);
            if(vigour < consume)
            {
                TipManager.AddTip("活力不足，无法发布公告");
                return false;
            }
        }
        return true;
    }

}
