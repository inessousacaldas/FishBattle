// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : ServerListItemViewController.cs
// Author   : senkay <senkay@126.com>
// Created  : 05/13/2015 
// Porpuse  : 
// **********************************************************************
//
using UnityEngine;

public class ServerListItemController : MonolessViewController<ServerListItem>
{
    private int _index;

    /// <summary>
    /// 从DataModel中取得相关数据对界面进行初始化
    /// </summary>
   

    

    /// <summary>
    /// Registers the event.
    /// DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();
        EventDelegate.Set(_view.ServerListItem_UIButton.onClick, OnListItemClick);
    }

    /// <summary>
    /// 关闭界面时清空操作放在这
    /// </summary>


    //服务器回调信息
    public delegate void GetServerMessageFunc(ServerListItemController item, bool show);
    private GetServerMessageFunc _callBackFunc = null;

    private GameServerInfo _serverInfo;

    public void SetData(int index, GameServerInfo serverInfo, GetServerMessageFunc func)
    {
        _index = index;
        _serverInfo = serverInfo;

        _callBackFunc = func;

        UpdateServerInfo();
        UpdateServerRunState();
    }

    //private Color _selectColor = new Color(167 / 256f, 112 / 256f, 36 / 256f, 255 / 256f);
    /// <summary>
    /// 选择服务器
    /// </summary>
    public void SelectServer()
    {
        //维护状态的服务器不给选择
        //		if (_serverInfo.dboState == 2)
        //		{
        //			return;
        //		}

        //		if (_isSelect) return;
        //		_isSelect = true;
        //		
        //		
        //		_backgroudSprite.color = _selectColor;
        _view.BgSprite_UISprite.spriteName = "Opt_2_On";
    }

    /// <summary>
    /// 不选中状态
    /// </summary>
    public void UnSelectServer()
    {
        //		_isSelect = false;
        _view.BgSprite_UISprite.spriteName = "Opt_2_Off";
    }

    public void OnListItemClick()
    {
        if (_serverInfo != null && _serverInfo.runState == 3)
        {
            TipManager.AddTip("服务器维护中，请稍候");
            return;
        }

        if (_callBackFunc != null && _serverInfo != null)
        {
            _callBackFunc(this, true);
        }
    }

    public GameServerInfo GetServerInfo()
    {
        return _serverInfo;
    }

    private void UpdateServerInfo()
    {
        if (_serverInfo == null) return;

        _view.NameLabel_UILabel.text = _serverInfo.name;
		if (_serverInfo.recommendType > 0)
		{
			_view.NewSprite.SetActive(true);
			_view.NewSprite_UISprite.spriteName = "The-push";
		}
		else if(_serverInfo.newServer)
		{
			_view.NewSprite.SetActive(true);
			_view.NewSprite_UISprite.spriteName = "The-server";
		}
		else
		{
			_view.NewSprite.SetActive(false);
		}
        int count = ServerManager.Instance.GetPlayerCount(_serverInfo.serverId);
        if (count > 0)
        {
            //_view.RoleSprite.SetActive(true);
            //_view.RoleCountLabel_UILabel.text = count.ToString();
        }
        else
        {
            //_view.RoleSprite.SetActive(false);
            _view.RoleCountLabel_UILabel.text = "";
        }
    }

    #region 服务器忙碌状态
    private void UpdateServerRunState()
    {
        if (_serverInfo == null) return;

        _view.StateSprite_UISprite.spriteName = ServerNameGetter.GetServiceStateSpriteName(_serverInfo);
    }
    #endregion
    public int GetIndex()
    {
        return _index;
    }
}

