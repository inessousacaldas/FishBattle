using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerListController : MonoViewController<ServerListView>
{
    private Dictionary<int, int> _areaDic = new Dictionary<int, int>();
    //key 是tabId value是区Id
    private List<AccountPlayerDto> _curAccountPlayerDtos;

    private ServerListItemController _currentServerListItem;
    private int _curSelectServer = -1;
    private GameServerInfo _curSelectServerInfo;
    private bool _curSelectServerRoleCountIsFull = true;
    private int _curSelectTab = -2;
    private GameObject _districtBtnObject;
    private GameObject _districtBtnPrafab;
    private GameObject _emptyGameObject;
    private bool _isInitView = true;

    private DistrictBtnController _lastDistrictBtnController;
    private Action<GameServerInfo, AccountPlayerDto> _onSelectCallBack;

    private Dictionary<int, List<GameServerInfo>> _serverInfoDic = new Dictionary<int, List<GameServerInfo>>();

    private Dictionary<int, ServerListItemController> _serverListItemDict;
    private ServerRoleBar _serverRoleBar;

    private GameObject _serverRoleBarGo;
    private GameObject _serverRoleBarPrefab;
    private GameObject _serverRoleItemPrefab;
    private bool _showCurSelectServer;

    private List<DistrictBtnController> _tabBtnList = new List<DistrictBtnController>();
    private Dictionary<int, DistrictBtnController> _tabBtnDic = new Dictionary<int, DistrictBtnController>();
    //tan Btn controller


    #region IViewController Members

    /// <summary>
    ///     从DataModel中取得相关数据对界面进行初始化
    /// </summary>
   

    protected override void AfterInitView ()
    {
        _serverListItemDict = new Dictionary<int, ServerListItemController>();
    }

    /// <summary>
    ///     Registers the event.
    ///     DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.ReturnBtn_UIButton.onClick, OnCloseButtonClick);
        EventDelegate.Set(View.SubmitBtn_UIButton.onClick, CreateRole);
    }

    /// <summary>
    ///     关闭界面时清空操作放在这
    /// </summary>
    protected override void OnDispose()
    {
        _serverListItemDict.Clear();
        if (_currentServerListItem != null)
            _currentServerListItem.UnSelectServer();
        //        StopRefreshTimer();
    }
    

    #endregion

    private void CreateRole()
    {
        if (_curSelectServerRoleCountIsFull)
        {
            TipManager.AddTip("玩家在每个服务器，最多拥有3个角色");
        }
        else
        {
            LoginPlayer(_curSelectServerInfo, null);
        }
    }

    private void LoginPlayer(GameServerInfo serverInfo, AccountPlayerDto playerDto)
    {
        if (serverInfo.runState == 3)
        {
            //TipManager.AddTip("服务器维护中，请稍候");
            //return;
        }

        if (playerDto != null && playerDto.id == ModelManager.Player.GetPlayerId())
        {
            GameDebuger.Log("select the same player at game");
        }
        else
        {
            if (_onSelectCallBack != null)
            {
                _onSelectCallBack(serverInfo, playerDto);
            }
        }

        ProxyServerListModule.Close();
    }

    private void OnCloseButtonClick()
    {
        ProxyServerListModule.Close();
    }

    public void Open(Action<GameServerInfo, AccountPlayerDto> selectCallback)
    {
        _onSelectCallBack = selectCallback;
        Paging(GameServerInfoManager.GetOpenServerList(), GameServerInfoManager.GetRecommendServerList(false),GameServerInfoManager.GetRecentlyServerList(false));
        InitTabBtn();

        //        RefreshServerListItem (GetServerList());
        //        SetupRefreshTimer();
        RefreshServerList();
    }

    private void InitTabBtn()
    {
        int index = -1;
        foreach (var serverInfo in _serverInfoDic)
        {
            _districtBtnObject = NGUITools.AddChild(View.ServerGrid_UIGrid.gameObject, AssetPipeline.ResourcePoolManager.Instance.LoadUI("DistrictBtn"));
            
            DistrictBtnController con = _districtBtnObject.GetMissingComponent<DistrictBtnController>();
            con.SetData(index, GameServerInfoManager.GetAreaName(serverInfo.Key), TabBtnCallBack);
            _tabBtnList.Add(con);
            _tabBtnDic.Add(index, con);
            ++index;
        }
        //        for (int i = 0; i < _serverInfoDic.Count; i++)
        //        {
        //            _districtBtnObject = NGUITools. AddChild(View.ServerGrid_UIGrid.gameObject,AssetPipeline.ResourcePoolManager.Instance.LoadUI("DistrictBtn"));
            
        //            DistrictBtnController con=  _districtBtnObject.GetMissingComponent<DistrictBtnController>();
        //            con.UpdateView(i, GetAreaName(_serverInfoDic), TabBtnCallBack);
        //            _tabBtnList.Add(con);
        //        }
        View.ServerGrid_UIGrid.Reposition();
        //if (_tabBtnList.Count > 0)
        //{
        //    TabBtnCallBack(-1);
        //}
        if(_tabBtnDic.Count > 0)
        {
            TabBtnCallBack(-1);
        }
    }

    private void TabBtnCallBack(int index, bool refresh = false)
    {
        if (index == _curSelectTab && !refresh)
            return;
        View.Tips.SetActive(false);
        if (_lastDistrictBtnController != null)
        {
            _lastDistrictBtnController.SetSelected(false);
        }
        //_lastDistrictBtnController = _tabBtnList[index];
        _lastDistrictBtnController = _tabBtnDic[index];
        _lastDistrictBtnController.SetSelected(true);
        _curSelectTab = index;
        _curSelectServer = -1;
        if (_areaDic.ContainsKey(index) && _serverInfoDic.ContainsKey(_areaDic[index]))
        {
            RefreshServerListItem(_serverInfoDic[_areaDic[index]]);
        }
    }

    private void Paging(List<GameServerInfo> serveromInfos, List<GameServerInfo> recommendServerList,List<GameServerInfo> recentlyServerList)
    {
        _serverInfoDic.Clear();
        _areaDic.Clear();
        int tabIndex = -1;

        _areaDic.Add(tabIndex, tabIndex);
        _serverInfoDic.Add(tabIndex++, recommendServerList);
        _areaDic.Add(tabIndex, tabIndex);
        _serverInfoDic.Add(tabIndex++, recentlyServerList);
        List<int> areaList = new List<int>(); //区
        for (int i = 0; i < serveromInfos.Count; i++)
        {
            if (!areaList.Contains(serveromInfos[i].areaId))
            {
                areaList.Add(serveromInfos[i].areaId);
            }
        }

        for (int i = 0; i < areaList.Count; i++)
        {
            int areaId = areaList[i];
            List<GameServerInfo> tList = new List<GameServerInfo>();
            for (int j = 0; j < serveromInfos.Count; j++)
            {
                if (serveromInfos[j].areaId == areaId)
                {
                    tList.Add(serveromInfos[j]);
                }
            }
            //排序
            if (tList.Count > 0)
            {
                tList.Sort((a, b) =>
                    {
                        if (a.openTime == b.openTime)
                        {
                            return a.serverId.CompareTo(b.serverId);
                        }
                        return b.openTime.CompareTo(a.openTime);
                    });
            }
            _areaDic.Add(tabIndex++, areaId);
            _serverInfoDic.Add(areaId, tList);
        }
    }

    private void RefreshServerListItem(List<GameServerInfo> serverList)
    {
        GameDebuger.Log("Refresh the ServerList");
        _serverListItemDict.Clear();
        //删除原本的子列表

        View.ServerListGrid_UIGrid.gameObject.RemoveChildren();

        string lastServerId = PlayerPrefs.GetString(GameSetting.LastServerPrefsName);
        string preServerId = PlayerPrefs.GetString("preServerId");

        bool defaultServer = false;

        List<GameServerInfo> list = serverList; //GetServerList();
        View.Tips.SetActive(false);
        //更新服务器列表,如果存在默认选默认，否则选择最后一个推荐的服务器
        if (list.Count > 0)
        {
            GameDebuger.Log("ServerList Count=" + list.Count);

            //推荐服务器
            GameServerInfo recommendServerMsg = null;
            GameServerInfo firstServerMsg = null;
            //临时
            ServerListItemController tmpServerItem = null;

            //初始化ServerListTable
            //ServerConfig.SortServerList();  //取消客户端排序操作，服务端控制
            for (int i = 0, len = list.Count; i < len; i++)
            {
                GameServerInfo serverMsg = list[i];
                //GameDebuger.Log("service " + i + " : " + serverMsg.GetServerUID());

                //版本控制flag
                bool passVerLimit = true;
                //                    if (ServiceProviderManager.HasSP() && GameSetting.Release)
                //                    {
                //                        passVerLimit = (GameSetting.ver >= serverMsg.limitVer && serverMsg.limitMaxVer >= GameSetting.ver);
                //                    }

                if (serverMsg != null && serverMsg.dboState != 0 && passVerLimit)
                {
                    //加入到所有服务器Table中
                    tmpServerItem = AddCachedChild<ServerListItemController, ServerListItem>(
                        View.ServerListGrid_UIGrid.gameObject
                        , "ServerListItem");

                    tmpServerItem.SetData(i, serverMsg, OnSelectServer);

                    int modVal = (i + 1) % 2;
                    modVal = modVal == 0 ? 2 : modVal;

                    var item = tmpServerItem.gameObject;
                    item.name = Math.Floor(i / 2f) + "-" + modVal;

                    //最近登录列表中的服务器，只有处于“开放”状态的才加进去
                    if (preServerId == serverMsg.GetServerUID() && serverMsg.dboState == 1)
                    {
                    }

                    if (lastServerId == serverMsg.GetServerUID() && serverMsg.dboState == 1)
                    {
                        OnSelectServer(tmpServerItem, false);
                        defaultServer = true;
                    }

                    if (!_serverListItemDict.ContainsKey(serverMsg.serverId))
                    {
                        GameDebuger.Log("Add serviceId : " + serverMsg.serverId);
                        _serverListItemDict.Add(serverMsg.serverId, tmpServerItem);
                    }

                    //记录一个已开放的推荐服
                    if (serverMsg.recommendType > 0 && serverMsg.dboState == 1)
                    {
                        recommendServerMsg = serverMsg;
                    }
                    firstServerMsg = list[0]; //serverMsg;//这里改为选择第一个
                }
            }

            if (list.Count % 2 == 1)
            {
                GameObject emptyGameObject = NGUITools.AddChild(View.ServerListGrid_UIGrid.gameObject);
                emptyGameObject.name = Math.Floor(list.Count / 2f) + "-" + 2;
            }

            //若无默认服务器，选择最新推荐服
            if (defaultServer == false)
            {
                if (recommendServerMsg != null)
                {
                    OnSelectServer(GetServerItemControllerWithInfo(recommendServerMsg), false);
                }
                else //若无推荐服务器，选择第一个服，加入最近登录列表
                {
                    if (firstServerMsg != null)
                    {
                        OnSelectServer(GetServerItemControllerWithInfo(firstServerMsg), false);
                    }
                }
            }
        }

        View.ServerListGrid_UIGrid.enabled = true;
        View.ServerListGrid_UIGrid.Reposition();
    }

    private ServerListItemController GetServerItemControllerWithInfo(GameServerInfo info)
    {
        ServerListItemController controller = null;
        _serverListItemDict.TryGetValue(info.serverId, out controller);
        return controller;
    }

    private void SelectServerItem(ServerListItemController serverItem)
    {
        if (_currentServerListItem == serverItem)
        {
            return;
        }
        TalkingDataHelper.OnEventSetp("SelectServer", "选择服务器");
        SPSDK.gameEvent("10016");   //选择服务器
        if (_currentServerListItem != null)
            _currentServerListItem.UnSelectServer();

        serverItem.SelectServer();

        _currentServerListItem = serverItem;
    }

    /// <summary>
    ///     选择当前服务器
    /// </summary>
    /// <param name="listItem"></param>
    /// <param name="showRoleList"></param>
    private void OnSelectServer(ServerListItemController listItem, bool showRoleList = false)
    {
        SelectServerItem(listItem);
        //展开
        if (_isInitView || showRoleList)
        {
            if (_curSelectServer != listItem.GetIndex())
            {
                _showCurSelectServer = true;
            }
            if (_curSelectServer == listItem.GetIndex())
            {
                _showCurSelectServer = !_showCurSelectServer;
            }

            if (_showCurSelectServer)
            {
                _curSelectServer = listItem.GetIndex();
                if (_serverRoleBarGo != null)
                {
                    _serverRoleBarGo.SetActive(true);
                }
                if (_emptyGameObject != null)
                {
                    _emptyGameObject.SetActive(true);
                }
                //View.Tips.SetActive(true);
            }
            else
            {
                if (_serverRoleBarGo != null)
                {
                    _serverRoleBarGo.SetActive(false);
                }
                if (_emptyGameObject != null)
                {
                    _emptyGameObject.SetActive(false);
                }
               // View.Tips.SetActive(false);
            }


            int index = listItem.GetIndex();
            if (_serverRoleBarGo == null)
            {
                _serverRoleBarGo = AddCachedChild(View.ServerListGrid_UIGrid.gameObject, "ServerRoleBar");
                _emptyGameObject = NGUITools.AddChild(View.ServerListGrid_UIGrid.gameObject);
                _serverRoleBar = BaseView.Create<ServerRoleBar>(_serverRoleBarGo);
            }

            _serverRoleBarGo.name = Math.Floor(index / 2f) + "-" + 3;
            _emptyGameObject.name = Math.Floor(index / 2f) + "-" + 4;
            _isInitView = false;
            ShowRoleList(_serverRoleBar, listItem);
        }
        View.ServerListGrid_UIGrid.Reposition();
    }
    private List<ServerRoleItemController> c = null;
    private void ShowRoleList(ServerRoleBar serverRoleBar, ServerListItemController listItem)
    {
        _curSelectServerInfo = listItem.GetServerInfo();
        _curAccountPlayerDtos = ServerManager.Instance.GetPlayersAtServer(_curSelectServerInfo.serverId);
        View.SubmitBtn_UIButton.gameObject.SetActive(true);
        var roleListGridGo = serverRoleBar.RoleListGrid_UIGrid.gameObject;
        if (c.IsNullOrEmpty())
        {
            c = new List<ServerRoleItemController>();
        }
        //c.ForEach<ServerRoleItemController>(s=>RemoveCachedChild<ServerRoleItemController, ServerRoleItem>(s));
        
        roleListGridGo.RemoveChildren();

        for (int i = 0, imax = _curAccountPlayerDtos.Count; i < imax; i++)
        {
            View.SubmitBtn_UIButton.gameObject.SetActive(false);

            AccountPlayerDto accountPlayerDto = _curAccountPlayerDtos[i];

            var com = AddCachedChild<ServerRoleItemController, ServerRoleItem>(
                roleListGridGo
                , "ServerRoleItem");
            com.SetData(accountPlayerDto, OnClickRoleItem, OnDeleteRoleItem);
            c.AddIfNotExist(com);
        }

        if (_curAccountPlayerDtos.Count < 3)
        {
            _curSelectServerRoleCountIsFull = false;
            var com = AddCachedChild<ServerRoleItemController, ServerRoleItem>(
                roleListGridGo
                , "ServerRoleItem");
            com.SetData(null, OnClickRoleItem, null);
            c.AddIfNotExist(com);
        }

        serverRoleBar.RoleListGrid_UIGrid.enabled = true;
        serverRoleBar.RoleListGrid_UIGrid.Reposition();
    }

    private void OnClickRoleItem(ServerRoleItemController roleItem)
    {
        LoginPlayer(_curSelectServerInfo, roleItem.PlayerDto);
    }

    private void OnDeleteRoleItem(ServerRoleItemController roleItem)
    {
        ProxyWindowModule.OpenInputWindow(0, 100, "", "角色删除后将无法恢复，请谨慎操作", "请输入你要删除的角色ID", "", playerId =>
            {
                var accountPlayerDto = roleItem.PlayerDto;
                GameDebuger.LogError(accountPlayerDto.id);
                if (playerId == accountPlayerDto.id.ToString())
                {
                    if (accountPlayerDto.grade < 8)
                    {
                        ServiceProviderManager.RequestPlayerDelete(accountPlayerDto.id.ToString(),
                            _curSelectServerInfo.destServerId.ToString(), !ProxyLoginModule.IsOpen(),
                            response =>
                            {
                                if (response.code == 0)
                                {
                                    for (int j = 0; j < _curAccountPlayerDtos.Count; j++)
                                    {
                                        if (_curAccountPlayerDtos[j].id == accountPlayerDto.id)
                                        {
                                            ServerManager.Instance.DelectPlayer(_curAccountPlayerDtos[j]);
                                            if (accountPlayerDto.id == ModelManager.Player.GetPlayerId())
                                            {
                                                ExitGameScript.Instance.HanderRelogin();
                                            }
                                            else
                                            {
                                                Paging(GameServerInfoManager.GetOpenServerList(),
                                                    GameServerInfoManager.GetRecommendServerList(true),
                                                     GameServerInfoManager.GetRecentlyServerList(true));
                                                TabBtnCallBack(_curSelectTab, true);
                                            }
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogError(response.msg);
                                    TipManager.AddTip(response.msg);
                                }
                            });
                    }
                    else
                    {
                        TipManager.AddTip("8级以上角色不允许删除!");
                    }
                }
                else
                {
                    TipManager.AddTip("ID不符合，无法删除");
                    roleItem.ShowDeleteBtn(false);
                }
            }, () => roleItem.ShowDeleteBtn(false));
    }

    #region 动态刷新列表

    private void RefreshServerList()
    {
        if (GameServerInfoManager.CanRefreshServerList)
        {
            GameServerInfoManager.RefreshServerList();
        }
    }

    //    private const float RefreshTime = 10f;
    //    private const string RefreshServerListTimer = "RefreshServerList";
    //    private void SetupRefreshTimer()
    //    {
    //        CoolDownManager.Instance.SetupCoolDown(RefreshServerListTimer, RefreshTime, null, OnRefreshServerList);
    //    }
    //
    //    private void OnRefreshServerList()
    //    {
    //        GameServerInfoManager.RequestDynamicServerList(AppGameVersion.SpVersionCode, GameSetting.Channel,
    //           GameSetting.Platform, null,null);
    //    }
    //
    //    private void StopRefreshTimer()
    //    {
    //        CoolDownManager.Instance.CancelCoolDown(RefreshServerListTimer);
    //    }

    #endregion
}