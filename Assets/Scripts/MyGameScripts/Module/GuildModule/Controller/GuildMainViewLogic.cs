// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/10/2018 2:11:20 PM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class GuildMainDataMgr
{
    
    public static partial class GuildMainViewLogic
    {
        private static CompositeDisposable _noGuildDisposable;
        private static CompositeDisposable _hasGuildDisposable;

        public static void Open(GuildTab tab)
        {
            var data = DataMgr._data;
            if (data.GuildState == GuildState.NotJoin)
                OpenNotJoin();
            else
                OpenHasJoin(tab);
        }

        //打开未加入界面
        private static void OpenNotJoin()
        {
	        var layer = UILayerType.DefaultModule;
            var ctrl = GuildMainViewController.Show<GuildMainViewController>(
                GuildMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }

        //打开已加入界面
        private static void OpenHasJoin(GuildTab tab)
        {
            var layer = UILayerType.DefaultModule;
            var ctrl = GuildHasJoinViewController.Show<GuildHasJoinViewController>(
                GuildHasJoinView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }

        #region 未加入公会
        private static void InitReactiveEvents(IGuildMainViewController ctrl)
        {
            if (ctrl == null) return;
            if (_noGuildDisposable == null)
                _noGuildDisposable = new CompositeDisposable();
            else
            {
                _noGuildDisposable.Clear();
            }
            var data = DataMgr._data;
            _noGuildDisposable.Add(ctrl.CloseEvt.Subscribe(_=> NoGuildDispose()));
            _noGuildDisposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=> CloseNotJoinView()));
            _noGuildDisposable.Add(ctrl.TabMgr.Stream.Subscribe(e =>
            {
                data.JoinOrCreateState = (NotJoinViewState)e;
                if (data.JoinOrCreateState == NotJoinViewState.Join)
                    FireData();
                else
                    GuildMainNetMsg.ReqGuildCount();
            }));
            _noGuildDisposable.Add(ctrl.OndiamondBtn_UIButtonClick.Subscribe(e => CreateByDiamond(ctrl)));//钻石创建
            _noGuildDisposable.Add(ctrl.OngoldBtn_UIButtonClick.Subscribe(e => CreateByGold(ctrl)));  //金币创建
            _noGuildDisposable.Add(ctrl.ScrollDragFinish.Subscribe(e =>
            {
                //拖动到最后一个,请求数据
                if (e)
                {
                    GuildMainNetMsg.ReqGuildList(data.reqGuildPageIndex, ReqGuildListCount);
                }
            }));
            _noGuildDisposable.Add(ctrl.GuildItemCtrl.Subscribe(e => ctrl.OnGuildItemClick(e)));    //公会点击
            _noGuildDisposable.Add(ctrl.OnsearchBtn_UIButtonClick.Subscribe(e => SearchHandler(ctrl)));  //搜索
            _noGuildDisposable.Add(ctrl.OnContactBtn_UIButtonClick.Subscribe(e => ContactPresidentHandler()));   //联系城主
            _noGuildDisposable.Add(ctrl.OnoneKeyApplyBtn_UIButtonClick.Subscribe(e => OneKeyApplyGuildHandler()));   //一键申请
            _noGuildDisposable.Add(ctrl.OnapplyBtn_UIButtonClick.Subscribe(e => ApplyGuildHandler(ctrl.CurSelectGuild,ctrl.SelItemCtrl)));     //申请加入
            _noGuildDisposable.Add(ctrl.SearchInputChange.Subscribe(e => OnSearchInputChange(ctrl,e)));     //搜索Input
            Init(ctrl);
        }

        private static void Init(IGuildMainViewController ctrl)
        {
            DataMgr._data.JoinOrCreateState = NotJoinViewState.Join;
            GuildMainNetMsg.ReqGuildList(DataMgr._data.reqGuildPageIndex, ReqGuildListCount);
        }
            
        #region 创建
        private static void CreateByDiamond(IGuildMainViewController ctrl)
        {
            if (!CheckCanCreateGuild(ctrl)) return;
            ExChangeHelper.CheckIsNeedExchange(AppDto.AppVirtualItem.VirtualItemEnum.DIAMOND, DataMgr._data.CreateNeed(true), delegate
             {
                 GuildMainNetMsg.ReqCreateByDiamond(ctrl.CreateNameStr, ctrl.CreateManifestoStr);
             });
        }

        private static void CreateByGold(IGuildMainViewController ctrl)
        {
            if (!CheckCanCreateGuild(ctrl)) return;
            ExChangeHelper.CheckIsNeedExchange(AppDto.AppVirtualItem.VirtualItemEnum.GOLD, DataMgr._data.CreateNeed(false), delegate
            {
                GuildMainNetMsg.ReqCreateByGold(ctrl.CreateNameStr, ctrl.CreateManifestoStr);
            });
        }

        //检查是否可以创建公会
        private static bool CheckCanCreateGuild(IGuildMainViewController ctrl)
        {
            var data = DataMgr._data;
            int lv = ModelManager.Player.GetPlayerLevel();
            int limit = data.CreateLevelLimit;
            if (lv < limit)
            {
                TipManager.AddTip("创建公会需人物等级" + limit + "级");
                return false;
            }
            //玩家是否有公会
            if(data.PlayerGuildInfo!= null)
            {
                TipManager.AddTip("您已有公会");
                return false;
            }
            //名字和宣言统一改成字符判断
            string nameError = GuildMainHelper.ValidateStrLength(ctrl.CreateNameStr,4,10,true);
            if (!string.IsNullOrEmpty(nameError))
            {
                TipManager.AddTip(nameError);
                return false;
            }
            string manifestoError = GuildMainHelper.ValidateStrLength(ctrl.CreateManifestoStr, 2, 100,false);
            if (!string.IsNullOrEmpty(manifestoError))
            {
                TipManager.AddTip(manifestoError);
                return false;
            }

            return true;
        }


        private static void NoGuildDispose()
        {
            _noGuildDisposable = _noGuildDisposable.CloseOnceNull();
            OnNoGuildDispose();
        }


        // 两个预制件共用Logic层，所以OnDispose进行改名区分
        private static void OnNoGuildDispose()
        {
            DataMgr._data.ClearGuildList();
        }

        public static void CloseNotJoinView()
        {
            UIModuleManager.Instance.CloseModule(GuildMainView.NAME);
        }
        #endregion

        #region 加入

        //搜索
        private static void SearchHandler(IGuildMainViewController ctrl)
        {
            string str = ctrl.SearchStr;
            var list = DataMgr._data.TmpGuildList;

            if (string.IsNullOrEmpty(str))
            {
                TipManager.AddTip("请输入编号或名字进行搜索");
                return;
            }
            if(list == null)
            {
                return;
            }
            ctrl.FirstOpen = true;
            ctrl.TmpItemCtrl = ctrl.SelItemCtrl;
            //int id = StringHelper.ToInt(str);
            GuildMainNetMsg.ReqSearch(str);
            
            //if (id == 0)
            //{
            //    var val = list.Find(e => e.name == str);
            //    //如果本地数据有，则取本地
            //    if (val != null)
            //    {
            //        DataMgr._data.UpdateGuildList(val);
            //    }
            //    else
            //    {
            //        GuildMainNetMsg.ReqSearch(str);
            //    }
            //}
            //else
            //{
            //    GuildMainNetMsg.ReqSearch(str);
            //}
        }

        private static void OnSearchInputChange(IGuildMainViewController ctrl,string str)
        {
            if (DataMgr._data.SearchGuildState == IsSearchGuildState.No || !string.IsNullOrEmpty(str)) return;
            DataMgr._data.UpdateGuildList();
        }

        //联系城主
        private static void ContactPresidentHandler()
        {
            GameDebuger.LogError("联系城主");
        }

        //一键申请
        private static void OneKeyApplyGuildHandler()
        {
            if (!CanRequstGuild(BatchId)) return;
            GuildMainNetMsg.ReqBatchRequstGuild();
        }
        
        //申请加入
        private static void ApplyGuildHandler(int showId, GuildItemController ctrl)
        {
            if (!CanRequstGuild(showId)) return;
            GuildMainNetMsg.ReqRequstGuild(showId, ctrl.ItemInfoDto.name);
        }
        //是否可以申请加入公会
        private static bool CanRequstGuild(int showId)
        {
            var data = DataMgr._data;
            if (data.GuildState == GuildState.HasJoin)
            {
                TipManager.AddTip("您当前已经有公会了，不要三心二意了。");
                return false;
            }
            if (data.IsRepeatRequst(showId))
            {
                TipManager.AddTip("你已发送加入公会申请，请稍后再尝试");
                return false;
            }
            return true;
        }

        #endregion

        #endregion


        #region 已加入公会

        private static void InitReactiveEvents(IGuildHasJoinViewController ctrl)
        {
            if (ctrl == null) return;
            if (_hasGuildDisposable == null)
                _hasGuildDisposable = new CompositeDisposable();
            else
            {
                _hasGuildDisposable.Clear();
            }

            _hasGuildDisposable.Add(ctrl.CloseEvt.Subscribe(_ => HasGuildDispose()));
            _hasGuildDisposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseHasJoinView()));
            _hasGuildDisposable.Add(ctrl.TabMgr.Stream.Subscribe(e => 
            {
                GuildTab tab = (GuildTab)ctrl.FilterList.TryGetValue(e).EnumValue;
                ctrl.OnTabClick(tab);
                OnTabClick(tab, ctrl);
            }));

            #region 信息界面
            _hasGuildDisposable.Add(ctrl.OnmemberBtn_UIButtonClick.Subscribe(_ => memberBtn_UIButtonClick()));
            _hasGuildDisposable.Add(ctrl.OngradeBtn_UIButtonClick.Subscribe(_ => gradeBtn_UIButtonClick()));
            _hasGuildDisposable.Add(ctrl.Onjob_UIButtonClick.Subscribe(_ => job_UIButtonClick()));
            _hasGuildDisposable.Add(ctrl.Oncontribution_UIButtonClick.Subscribe(_ => contribution_UIButtonClick()));
            _hasGuildDisposable.Add(ctrl.Ononline_UIButtonClick.Subscribe(_ => online_UIButtonClick()));
            _hasGuildDisposable.Add(ctrl.OnmemberInfoBtn_UIButtonClick.Subscribe(_ => memberInfoBtn_UIButtonClick(ctrl)));
            _hasGuildDisposable.Add(ctrl.OnguildInfoBtn_UIButtonClick.Subscribe(_ => guildInfoBtn_UIButtonClick(ctrl)));
            _hasGuildDisposable.Add(ctrl.OnsendMesBtn_UIButtonClick.Subscribe(_ => sendMesBtn_UIButtonClick(ctrl.GuildInfoViewCtrl.LastSelItemCtrl)));
            _hasGuildDisposable.Add(ctrl.OnaddFriendBtn_UIButtonClick.Subscribe(_ => addFriendBtn_UIButtonClick(ctrl.GuildInfoViewCtrl.LastSelItemCtrl)));
            _hasGuildDisposable.Add(ctrl.OnmodifyJobBtn_UIButtonClick.Subscribe(_ => modifyJobBtn_UIButtonClick(ctrl)));        //修改职位按钮点击
            _hasGuildDisposable.Add(ctrl.OnkickMemBtn_UIButtonClick.Subscribe(_ => kickMemBtn_UIButtonClick(ctrl)));            //开除成员
            _hasGuildDisposable.Add(ctrl.OnguildListBtn_UIButtonClick.Subscribe(_ => guildListBtn_UIButtonClick()));
            _hasGuildDisposable.Add(ctrl.OnleaveBtn_UIButtonClick.Subscribe(_ => leaveBtn_UIButtonClick()));                    //脱离势力
            _hasGuildDisposable.Add(ctrl.OnrequestListBtn_UIButtonClick.Subscribe(_ => requestListBtn_UIButtonClick(ctrl)));    //申请列表点击
            _hasGuildDisposable.Add(ctrl.OnbackGuildBtn_UIButtonClick.Subscribe(_ => backGuildBtn_UIButtonClick()));
            _hasGuildDisposable.Add(ctrl.MemItemClick.Subscribe(_ => ctrl.GuildInfoViewCtrl.OnMemItemClick(_)));
            _hasGuildDisposable.Add(ctrl.OnmodifyCloseBtn_UIButtonClick.Subscribe(_ => modifyJobClose_UIButtonClick(ctrl)));    //修改职位关闭按钮
            _hasGuildDisposable.Add(ctrl.modifyJobClick.Subscribe(_ => ctrl.GuildInfoViewCtrl.OnModifyBtnClick(_)));            //职位点击
            _hasGuildDisposable.Add(ctrl.OnappointBtn_UIButtonClick.Subscribe(_ => appointBtn_ButtonClick(ctrl)));              //确定委托
            _hasGuildDisposable.Add(ctrl.OnrCloseBtn_UIButtonClick.Subscribe(_ => ctrl.GuildInfoViewCtrl.ShowRequestList(false)));  //关闭申请列表
            _hasGuildDisposable.Add(ctrl.OnrClearListBtn_UIButtonClick.Subscribe(_ => OnrClearAllApprovalList()));    //清除申请列表
            _hasGuildDisposable.Add(ctrl.OnrOneKeyAccept_UIButtonClick.Subscribe(_ => GuildMainNetMsg.ReqOnekeyAccept(1,ReqRequestListCount)));        //一键接收
            _hasGuildDisposable.Add(ctrl.OnrequesterStatEvt.Subscribe(_ => OnAcceptRequester(_))); //接收入会
            _hasGuildDisposable.Add(ctrl.OnrCloseBtn_UIButtonClick.Subscribe(_ => OnrCloseBtn_UIButtonClick(ctrl)));            // 关闭申请列表
            _hasGuildDisposable.Add(ctrl.MemDragBottom.Subscribe(e =>
            {
                if (e)
                {
                    GuildMainNetMsg.ReqGuildMemList(DataMgr._data.reqMemPageIndex, ReqMemListCount);
                }
            }));
            _hasGuildDisposable.Add(ctrl.ReqDragBottom.Subscribe(e =>
            {
                if (e)
                {
                    GuildMainNetMsg.ReqRequestList(DataMgr._data.reqRequesterPageIndex, ReqRequestListCount);
                }
            }));
            #endregion

            #region 建筑
            _hasGuildDisposable.Add(ctrl.OnexplainBtn_UIButtonClick.Subscribe(e => { OnExplainBtnClik(); }));
            _hasGuildDisposable.Add(ctrl.Onbg_UIButtonClick.Subscribe(e => { OnClickCheckBtn(ctrl,null); }));
            _hasGuildDisposable.Add(ctrl.OnupgradeBtn_UIButtonClick.Subscribe(e => OnClickUpgradeBtn(ctrl.GuildBuildViewCtrl.SelCtrl)));
            _hasGuildDisposable.Add(ctrl.OncheckBtn_UIButtonClick.Subscribe(e => { OnClickCheckBtn(ctrl, e); }));

            #endregion

            #region 管理

            _hasGuildDisposable.Add(ctrl.OnNoticeModificationBtn_UIButtonClick.Subscribe(e => OnNoticeModificationClick()));
            _hasGuildDisposable.Add(ctrl.OnManifestoModificationBtn_UIButtonClick.Subscribe(e => OnManifestoModificationClick()));
            _hasGuildDisposable.Add(ctrl.OnMoreMessageLabel_UIButtonClick.Subscribe(e => { }));

            #endregion

            ctrl.OnTabClick(GuildTab.InfoView);
            guildInfoBtn_UIButtonClick(ctrl);
            OnTabClick(GuildTab.InfoView, ctrl);
        }

        #region 管理

        private static void OnNoticeModificationClick()
        {
            INoticeModificationViewController controller = UIModuleManager.Instance.OpenFunModule<NoticeModificationViewController>(NoticeModificationView.NAME, UILayerType.SubModule, true);
            controller.SetData(NoticeModificationViewController.NoticeType.guildNotic,DataMgr._data);
        }

        private static void OnManifestoModificationClick()
        {
            INoticeModificationViewController controller = UIModuleManager.Instance.OpenFunModule<NoticeModificationViewController>(NoticeModificationView.NAME, UILayerType.SubModule, true);
            controller.SetData(NoticeModificationViewController.NoticeType.guildManifesto, DataMgr._data);
        }
        private static void OnMoreMessageLabelClick()
        {
            //GuildEventViewController _guildMessageCtr = UIModuleManager.Instance.OpenFunModule<GuildEventViewController>(GuildEventView.NAME, UILayerType.SubModule, true);
            //_guildMessageCtr.UpdateMessageItem(_messageInfoList);
        }

        private static void CloseNoticeView()
        {
            UIModuleManager.Instance.CloseModule(NoticeModificationView.NAME);
        }

        #endregion
        #region 建筑
        private static void OnExplainBtnClik()
        {
            ProxyTips.OpenTextTips(28, new UnityEngine.Vector3(260, 115), true);
        }
        //查看
        private static void OnClickCheckBtn(IGuildHasJoinViewController ctrl, GuildBuildItemController item)
        {
            ctrl.GuildBuildViewCtrl.OnClickCheckBtn(DataMgr._data, item);
        }
        //升级
        private static void OnClickUpgradeBtn(GuildBuildItemController item)
        {
            var data = DataMgr._data;
            int idx = item.buildDetailMsg.Idx;
            if(item == null)
            {
                GameDebuger.Log("数据出错");
                return;
            }
            
            int guildLv = data.GuildBaseInfo.grade;
            var buildInfo = data.GuildDetailInfo.buildingInfo;
            int pub = buildInfo.barpubGrade;
            int treasury = buildInfo.treasuryGrade;
            int tower = buildInfo.guardTowerGrade;
            int workshop = buildInfo.workshopGrade;
            if (idx == (int)GuildBuilding.GuildBuildingType.Grade)
            {
                if(guildLv != pub)
                {
                    TipManager.AddTip(DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.BarPub) + "等级不满足");
                    return;
                }
                else if(guildLv != treasury)
                {
                    TipManager.AddTip(DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.Treasury) + "等级不满足");
                    return;
                }
                else if(guildLv != tower)
                {
                    TipManager.AddTip(DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.GuardTower) + "等级不满足");
                    return;
                }
                else if(guildLv != workshop)
                {
                    TipManager.AddTip(DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.Workshop) + "等级不满足");
                    return;
                }
                else
                {
                    //
                }
            }
            else if(idx == (int)GuildBuilding.GuildBuildingType.BarPub)
            {
                if(guildLv == pub)
                {
                    TipManager.AddTip("请先升级" + DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.Grade));
                    return;
                }
            }
            else if(idx == (int)GuildBuilding.GuildBuildingType.Treasury)
            {
                if (guildLv == treasury)
                {
                    TipManager.AddTip("请先升级" + DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.Grade));
                    return;
                }
            }
            else if (idx == (int)GuildBuilding.GuildBuildingType.GuardTower)
            {
                if (guildLv == tower)
                {
                    TipManager.AddTip("请先升级" + DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.Grade));
                    return;
                }
            }
            else if (idx == (int)GuildBuilding.GuildBuildingType.Workshop)
            {
                if (guildLv == pub)
                {
                    TipManager.AddTip("请先升级" + DataMgr._data.BuildList.TryGetValue((int)GuildBuilding.GuildBuildingType.Grade));
                    return;
                }
            }
            GuildMainNetMsg.ReqUpgradeBuilding(item.buildDetailMsg.Idx);
        }
        #endregion

        private static void OnTabClick(GuildTab tab, IGuildHasJoinViewController ctrl)
        {
            switch (tab)
            {
                case GuildTab.InfoView:
                    //只有在打开界面时请求信息
                    if (ctrl.GuildInfoViewCtrl == null)
                        GuildMainNetMsg.ReqGuildMemList(DataMgr._data.reqMemPageIndex, ReqMemListCount);
                    break;
                case GuildTab.ManageView:
                    ReqGuildDetailInfo();
                    break;
                case GuildTab.WelfareView:
                    ReqGuildDetailInfo();
                    break;
                case GuildTab.ActivityView:
                    ReqGuildDetailInfo();
                    break;
                case GuildTab.BuildView:
                    GuildMainNetMsg.ReqGuildInfo(DataMgr._data.PlayerGuildInfo.guildShowId, true);
                    break;
            }
        }

        //公会详细数据，只有当数据为空才请求，其余情况看刷新（可能后面也是请求）
        private static void ReqGuildDetailInfo()
        {
            if (DataMgr._data.GuildDetailInfo == null)
            {
                GuildMainNetMsg.ReqGuildInfo(DataMgr._data.PlayerGuildInfo.guildShowId, true);
            }
            else
                FireData();
        }

        #region 信息逻辑
        private static void OnoneKeyAcceptRequester()
        {
            if (CheckAcceptRequester())
            {
                //todo
            }
        }
        //接收成员
        private static void OnAcceptRequester(IGuildRequesterController ctrl)
        {
            if(CheckAcceptRequester())
                GuildMainNetMsg.ReqApproval(ctrl.GuildApprovalDto.applyerId);
        }
        private static bool CheckAcceptRequester()
        {
            var list = DataMgr._data.GuildApprovalList;
            if(list == null || list.ToList().Count == 0)
            {
                TipManager.AddTip("当前没有玩家申请进入公会");
                return false;
            }
            var selfInfo = DataMgr._data.PlayerGuildInfo;
            var selfPos = DataMgr._data.GuildPosition.Find(e => e.id == selfInfo.positionId);
            if (!selfPos.approval)
            {
                TipManager.AddTip("你没有权限接收成员");
                return false;
            }
            return true;
        }
        //清空申请列表
        private static void OnrClearAllApprovalList()
        {
            var selfInfo = DataMgr._data.PlayerGuildInfo;
            var selfPos = DataMgr._data.GuildPosition.Find(e => e.id == selfInfo.positionId);
            if (!selfPos.clearList)
            {
                TipManager.AddTip("你没有权限清空申请列表");
                return;
            }
            var list = DataMgr._data.GuildApprovalList;
            if (list == null || list.ToList().Count == 0) return;
            GuildMainNetMsg.ReqClearApplyList();
        }
        //申请列表关闭
        private static void OnrCloseBtn_UIButtonClick(IGuildHasJoinViewController ctrl)
        {
            ctrl.GuildInfoViewCtrl.ShowRequestList(false);
            //string str = DataMgr._data.applyIdStr;
            //if (!string.IsNullOrEmpty(str))
            //{
            //    GuildMainNetMsg.ReqClearApplyItem();
            //}
        }
        private static void memberBtn_UIButtonClick()
        {
            //GameDebuger.LogError("公会成员排序");
        }
        private static void gradeBtn_UIButtonClick()
        {
            //GameDebuger.LogError("等级排序");
        }
        private static void job_UIButtonClick()
        {
            //GameDebuger.LogError("职位排序");
        }
        private static void contribution_UIButtonClick()
        {
            //GameDebuger.LogError("贡献排序");
        }
        private static void online_UIButtonClick()
        {
            //GameDebuger.LogError("状态排序");
        }
        //成员信息分页
        private static void memberInfoBtn_UIButtonClick(IGuildHasJoinViewController ctrl)
        {
            var guildInfoCtrl = ctrl.GuildInfoViewCtrl;
            if (guildInfoCtrl == null) return;
            guildInfoCtrl.OnTabBtnClick(MemOrGuildEnum.Mem);
        }
        //公会信息分页
        private static void guildInfoBtn_UIButtonClick(IGuildHasJoinViewController ctrl)
        {
            var guildInfoCtrl = ctrl.GuildInfoViewCtrl;
            if (guildInfoCtrl == null) return;
            guildInfoCtrl.OnTabBtnClick(MemOrGuildEnum.Guild);
        }
        private static void sendMesBtn_UIButtonClick(IGuildMemItemController ctrl)
        {
            if (ctrl == null) return;
            if (ctrl.MemberDto.id == ModelManager.Player.GetPlayerId())
            {
                TipManager.AddTip("不能对自己操作");
                return;
            }
            var win = FriendDetailViewController.Show<FriendDetailViewController>(FriendDetailView.NAME, UILayerType.ThreeModule, false, true);
            win.UpdateGuildView(ctrl.MemberDto);
        }
        private static void addFriendBtn_UIButtonClick(IGuildMemItemController ctrl)
        {
            if (ctrl == null) return;
            if (ctrl.MemberDto.id == ModelManager.Player.GetPlayerId())
            {
                TipManager.AddTip("不能对自己操作");
                return;
            }
            FriendDataMgr.FriendNetMsg.ReqAddFriend(ctrl.MemberDto.id);
        }
        //修改职位
        private static void modifyJobBtn_UIButtonClick(IGuildHasJoinViewController ctrl)
        {
            var guildInfo = DataMgr._data.PlayerGuildInfo;  //自己的公会信息
            var selMemDto = ctrl.GuildInfoViewCtrl.LastSelItemCtrl.MemberDto;//所选人的信息
            var guildPosList = DataMgr._data.GuildPosition;     //职位表
            var val = guildPosList.Find(e => e.id == guildInfo.positionId);     //自己的职位信息
            if (CheckAppoint(val,selMemDto))
                ctrl.GuildInfoViewCtrl.ShowModifyJobPanel(DataMgr._data);
        }
        private static void modifyJobClose_UIButtonClick(IGuildHasJoinViewController ctrl)
        {
            ctrl.GuildInfoViewCtrl.ShowModifyJobPanel(DataMgr._data);
        }

        //委任点击
        private static void appointBtn_ButtonClick(IGuildHasJoinViewController ctrl)
        {
            var setPos = ctrl.GuildInfoViewCtrl.ModifyJobPos;  //所选职位
            var guildInfo = DataMgr._data.PlayerGuildInfo;  //自己的公会信息
            var selMemDto = ctrl.GuildInfoViewCtrl.LastSelItemCtrl.MemberDto;//所选人的信息
            var guildPosList = DataMgr._data.GuildPosition;     //职位表
            var val = guildPosList.Find(e => e.id == guildInfo.positionId);     //自己的职位信息
            if (setPos == null)
            {
                TipManager.AddTip("请选择职位");
                return;
            }
            if (CheckAppoint(val,selMemDto,setPos.posDto))
            {
                if (setPos.posDto.id == (int)AppDto.GuildPosition.GuildPositionEnum.Boss)
                {
                    string des = "是否将公会长职位移交给" + selMemDto.name;
                    string title = "";
                    var window = ProxyBaseWinModule.Open();
                    BaseTipData data = BaseTipData.Create(title, des, 0, delegate
                    {
                        GuildMainNetMsg.ReqTransferBoss(ctrl.GuildInfoViewCtrl.SelMemId);
                        ctrl.GuildInfoViewCtrl.ShowModifyJobPanel(DataMgr._data);
                    }, null);
                    window.InitView(data);
                }
                else
                {
                    GuildMainNetMsg.ReqGuildAppoint(ctrl.GuildInfoViewCtrl.SelMemId, setPos.posDto.id, selMemDto.name, setPos.posDto.name);
                    ctrl.GuildInfoViewCtrl.ShowModifyJobPanel(DataMgr._data);
                }
            }
        }
        
        //点击修改职位拦截判断
        private static bool CheckAppoint(GuildPosition selfPos, GuildMemberDto otherPos)
        {
            if(otherPos.id == ModelManager.Player.GetPlayerId())
            {
                TipManager.AddTip("不能变更自己的职位");
                return false;
            }
            var list = selfPos.appoints;
            if (!list.Contains(otherPos.position))
            {
                //没有权限任免这个人
                TipManager.AddTip("不能修改职位高于自己成员的职位");
                return false;
            }
            else
            {
                if (otherPos.position == selfPos.id)
                {
                    TipManager.AddTip("不能修改同级成员的职位");
                    return false;
                }
            }
            return true;
        }
        //委任时，还要做一层委任判断
        private static bool CheckAppoint(GuildPosition selfPos ,GuildMemberDto otherPos ,GuildPosition setPos)
        {
            var list = selfPos.appoints;
            if (!list.Contains(otherPos.position))
            {
                //没有权限任免这个人
                TipManager.AddTip("不能修改高于自己成员的职位");
                return false;
            }
            else
            {
                if (otherPos.position == selfPos.id)
                {
                    TipManager.AddTip("不能修改同级成员的职位");
                    return false;
                }
            }
            if (!list.Contains(setPos.id))
            {
                TipManager.AddTip("不能修改高于自己的职位");
                return false;
            }
            else
            {
                if (otherPos.position == setPos.id)
                {
                    TipManager.AddTip("职位信息没有发生变革");
                    return false;
                }
            }
            return true;
        }
        //开除成员
        private static void kickMemBtn_UIButtonClick(IGuildHasJoinViewController ctrl)
        {
            if (CheckKickOut(ctrl))
            {
                var selCtrl = ctrl.GuildInfoViewCtrl.LastSelItemCtrl;
                if (selCtrl == null) return;
                string name = selCtrl.MemberDto.name;
                string des = "是否开除公会成员【" + name + "】";
                string title = "";
                var window = ProxyBaseWinModule.Open();
                BaseTipData data = BaseTipData.Create(title, des, 0, delegate
                {
                    ctrl.GuildInfoViewCtrl.FirstOpen = true;
                    GuildMainNetMsg.ReqKickOut(ctrl.GuildInfoViewCtrl.SelMemId);
                }, null);
                window.InitView(data);
            }
        }
        private static bool CheckKickOut(IGuildHasJoinViewController ctrl)
        {
            var otherCtrl = ctrl.GuildInfoViewCtrl.LastSelItemCtrl;
            if (otherCtrl == null) return false;
            var selfInfo = DataMgr._data.PlayerGuildInfo;
            var selfPosInfo = DataMgr._data.GuildPosition.Find(e => selfInfo.positionId == e.id);
            var otherInfo = otherCtrl.MemberDto;//所选人的信息
            if (selfPosInfo != null)
            {
                if (!selfPosInfo.kickout)
                {
                    TipManager.AddTip("你没有权限开除成员");
                    return false;
                }
            }
            else
                return false;
            if(otherInfo.position != (int)AppDto.GuildPosition.GuildPositionEnum.Masses)
            {
                TipManager.AddTip("无法开除有职位的成员");
                return false;
            }
            return true; ;
        }
        private static void guildListBtn_UIButtonClick()
        {
            OpenNotJoin();
        }
        private static void leaveBtn_UIButtonClick()
        {
            var guildInfo = DataMgr._data.PlayerGuildInfo;
            if(guildInfo.positionId == (int)AppDto.GuildPosition.GuildPositionEnum.Boss)
            {
                TipManager.AddTip("会长不能够离开公会");
                return;
            }
            string des = "确认离开公会";
            string title = "";
            var ctrl = ProxyBaseWinModule.Open();
            BaseTipData data = BaseTipData.Create(title, des, 0, delegate
            {
                GuildMainNetMsg.ReqLeave();
            }, null);
            ctrl.InitView(data);
        }
        private static void requestListBtn_UIButtonClick(IGuildHasJoinViewController ctrl)
        {
            ctrl.GuildInfoViewCtrl.ShowRequestList(true);
            GuildMainNetMsg.ReqRequestList(1, ReqRequestListCount);
        }
        private static void backGuildBtn_UIButtonClick()
        {
            var scene = DataMgr._data.GuildScene;
            if (scene != null)
            {
                if (WorldManager.Instance != null && WorldManager.Instance.GetModel() != null && WorldManager.Instance.GetModel().GetSceneDto() != null)
                {
                    if (WorldManager.Instance.GetModel().GetSceneDto().id == scene.id)
                    {
                        TipManager.AddTip("当前已在"+scene.name+"中");
                        return;
                    }
                }
                GuildMainNetMsg.ReqEnterGuildMap(scene.id,scene.name);
            }
        }
        
        #endregion

        public static void CloseHasJoinView()
        {
            UIModuleManager.Instance.CloseModule(GuildHasJoinView.NAME);
        }
        
        private static void HasGuildDispose()
        {
            _hasGuildDisposable = _hasGuildDisposable.CloseOnceNull();
            OnHasGuildDispose();
        }
        // 两个预制件共用Logic层，所以OnDispose进行改名区分
        private static void OnHasGuildDispose()
        {
            DataMgr._data.ClearMemList();
        }
        #endregion

    }
}

