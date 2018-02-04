using System.Collections.Generic;
using UnityEngine;
using AppDto;

public static class ProxyMainUI{

	public const string PLAYERINFO_VIEW = "PlayerInfoView";
	private const string BUFFTIPS_VIEW = "MainUIBuffTipsView";
	public const string BATTLE_BUFFTIPS_VIEW = "BattleBuffTipsView";
	private const string SATIATIONPROPSUSE_VIEW = "SatiationPropsUseView";
	public const string FUNCTIONOPEN_VIEW = "FunctionOpenView";
	public const string BATTLE_ORDER_LIST_VIEW = "BattleOrderListView";
	public const string BATTLE_ORDER_EDITOR_VIEW = "BattleOrderEditorView";

    public static bool CheckMainViewOpen{
        get{
            return UIModuleManager.Instance.IsModuleCacheContainsModule (MainUIView.NAME);
        }
    }

	public static void Open(){
        MainUIDataMgr.MainUIViewLogic.Open();
	}

	public static void Hide(){
		UIModuleManager.Instance.HideModule(MainUIView.NAME);	
	}
	
    public static void Close(){
        UIModuleManager.Instance.CloseModule(MainUIView.NAME);   
    }

	public static void Show(){
		if (UIModuleManager.Instance.IsModuleCacheContainsModule(MainUIView.NAME))
		{
			UIModuleManager.Instance.OpenFunModule<MainUIViewController>(MainUIView.NAME,0,false);
		}
	}

    public static void TogleDisplayModel()
    {
        GameDebuger.TODO(@"if (UIModuleManager.Instance.IsModuleCacheContainsModule(MAINUI_VIEW))
        {
           GameObject ui = UIModuleManager.Instance.OpenFunModule(MAINUI_VIEW, 0, false);
           var controller = ui.GetMissingComponent<MainUIViewController>();
            controller.ToggleDisplayModel();
        }");
    }

    public static void OpenPlayerInfoView(Vector3 position,ScenePlayerDto playerDto){
        if (playerDto == null)
        {
            GameDebuger.LogError("select playerID       "+ playerDto.charactorId.ToString());
            return;
        }

        //var com = UIModuleManager.Instance.OpenFunModule<PlayerInfoViewController>(
        //    PlayerInfoView.NAME
        //    ,UILayerType.ThreeModule
        //    ,false
        //    , position);

        //com.UpdateView(playerDto);

        var ctrl = FriendDetailViewController.Show<FriendDetailViewController>(FriendDetailView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(playerDto);
    }

	public static void OpenPlayerInfoView(long playerId,Vector3 position){
        GameDebuger.TODO(@"ServiceRequestAction.requestServer (PlayerService.playerInfo(playerId),'GetPlayerInfo',(e)=>{');
            OpenPlayerInfoView(e as ScenePlayerDto,position);
        });");
	}

    public static void OpenPlayerInfoViewDynamic(long playerId, UILayerType layer = UILayerType.ThreeModule)
    {
        GameDebuger.TODO(@"ServiceRequestAction.requestServer(PlayerService.playerInfo(playerId), '', (e) =>
            {');
                GameObject view = UIModuleManager.Instance.OpenFunModule(PLAYERINFO_VIEW, layer, false);
                PlayerInfoViewController com = view.GetMissingComponent<PlayerInfoViewController>();
                com.Open(e as ScenePlayerDto);
            });");
    }

	public static void OpenPlayerInfoView(ScenePlayerDto playerDto,Vector3 position){
        GameDebuger.TODO(@"GameObject view = UIModuleManager.Instance.OpenFunModule(PLAYERINFO_VIEW,UILayerType.ThreeModule,false);
		view.transform.localPosition = position;
		PlayerInfoViewController com = view.GetMissingComponent<PlayerInfoViewController>();
		com.Open(playerDto);");
	}

	public static void ClosePlayerInfoView(){
		UIModuleManager.Instance.CloseModule(PLAYERINFO_VIEW);
	}

	public static void OpenBuffTipsView(){
		GameDebuger.TODO(@"GameObject view = UIModuleManager.Instance.OpenFunModule(BUFFTIPS_VIEW,UILayerType.SubModule,false);
        var com = view.GetMissingComponent<MainUIBuffTipsViewController>();");
		
	}

	public static void CloseBuffTipsView(){
		UIModuleManager.Instance.CloseModule(BUFFTIPS_VIEW);
	}

	public static void OpenSatiationPropsUseView(){
		GameDebuger.TODO(@"if(!ModelManager.Player.isFullSatiation()){
            GameObject view = UIModuleManager.Instance.OpenFunModule(SATIATIONPROPSUSE_VIEW,UILayerType.FourModule,false);
            var com = view.GetMissingComponent<SatiationPropsUseViewController>();
            com.Open();
        }else");
			TipManager.AddTip("饱食次数已满");
	}

	public static void CloseSatiationPropsUseView(){
		UIModuleManager.Instance.CloseModule(SATIATIONPROPSUSE_VIEW);
	}

	public static void OpenBattleBuffTipsView(MonsterController mc){
		GameDebuger.TODO(@"GameObject view = UIModuleManager.Instance.OpenFunModule(BATTLE_BUFFTIPS_VIEW,UILayerType.SubModule,false);
        var com = view.GetMissingComponent<BattleBuffTipsViewController>();
        com.Open(mc);");
	}

	public static bool IsBattleBuffTipsViewOpening()
	{
		return UIModuleManager.Instance.IsModuleCacheContainsModule(BATTLE_BUFFTIPS_VIEW);
	}

	public static void CloseBattleBuffTipsView(){
		UIModuleManager.Instance.CloseModule(BATTLE_BUFFTIPS_VIEW);
	}

    public static void OpenFunctionOpenView(List<int> openFuncIds)
    {
        GameDebuger.TODO(@" view = UIModuleManager.Instance.OpenFunModule(FUNCTIONOPEN_VIEW, UILayerType.Guide, false);
        var com = view.GetMissingComponent<FunctionOpenViewController>();
        com.Open(openFuncIds);");
    }

    public static void CloaseFunctionOpenView(){
		UIModuleManager.Instance.CloseModule(FUNCTIONOPEN_VIEW);
	}

    #region other function
    /// <summary>
    /// 一个安全代理
    /// </summary>
    public static bool HeadBtnGray
    {
        set
        {
            GameDebuger.TODO(@"if (null != MainUIViewController.Instance)
                MainUIViewController.Instance.HeadBtnGray = value;");
        }
    }

    public static void HideBtnsbByFuncIDs(IEnumerable<int> openFuncIds){
        GameDebuger.TODO(@"MainUIViewController.Instance.View.UpdateGOActiveByFuncIDSet (openFuncIds, false);");
    }

    public static Transform GetTopRightAnchorTrans(){
        GameDebuger.TODO(@"var ctrl = UIModuleManager.Instance.GetModuleController<MainUIViewController>(MainUIView.NAME);
        return ctrl != null ? ctrl.View.topRightAnchor.transform : null;");
        return null;
    }

    public static void SetMainUITopBtnGrid(bool b){
        GameDebuger.TODO(@"var ctrl = UIModuleManager.Instance.GetModuleController<MainUIViewController>(MainUIView.NAME);
        if (ctrl != null){
            MainUIViewController.Instance.View.topBtnGrid.gameObject.SetActive(b);
        }");
    }

    public static Vector3 GetBagBtnPosition()
    {
        var ctrl = UIModuleManager.Instance.GetModuleController<MainUIViewController>(MainUIView.NAME);
        return ctrl != null ?  ctrl.BagBtnPos : Vector3.zero;
    }


    #endregion
}
