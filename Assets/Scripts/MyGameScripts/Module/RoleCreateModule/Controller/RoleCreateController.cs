using System;
using System.Collections.Generic;
using AppDto;
using AppServices;
using AssetPipeline;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoleCreateController : MonoViewController<RoleCreateView>
{
    private const string RoleItemCellPath = "RoleItemCell";
    private const string FactioncellPath = "FactionCell";

    public const string Role2DBackRes = "roll2d_back";
    public const string Role2DHeroPrefix = "roll2d_hero_";

    private FactionCellController _lastSelectFactionItem;
    private List<FactionCellController> _factionItemList = new List<FactionCellController>();

    private string _lastRandomRoleName = "";

    private RoleItemCellController _lastSelectRoleItem;
    private Action<GeneralResponse> _onCreateSuccess;

    private RoleCreateModelController _roleModelController;
    private List<RoleItemCellController> _roleItemList;
    private GameServerInfo _serverInfo;

    private ModelDisplayController _modelDisplayer;

    private int playerModelId = -1;
    private int weaponModelId = -1;
    public void Open(GameServerInfo info, Action<GeneralResponse> onCreatePlayerSuccess)
    {
        _serverInfo = info;
        _onCreateSuccess = onCreatePlayerSuccess;
        TalkingDataHelper.OnEventSetp("CreatePlayer", "ShowView");
        OnRollNameButtonClick();
        if (null == _roleItemList || _roleItemList.Count <= 0)
            GameDebuger.LogError("_roleItemList is null or length invalid！");
        else
        {
            var itemList = _roleItemList.Filter(e => e.Charactor.usable).ToList();
            OnSelectRoleItem(itemList[Random.Range(0, itemList.Count)]);
        }

        DragStart();
        View.PartnerList_UITable.GetComponent<UICenterOnChild>().onCenter = e =>
        {
            FactionCellController item = e.GetMissingComponent<FactionCellController>();
            if (item == null)
                GameDebuger.LogError("职业设置为空");
            else
                OnSelectFactionItem(item);
        };
    }

    #region 处理职业拖拽
    
    public float LeftBound = 999.88f;
    public float RightBound = 1001.75f;
    public float scaleRate = 0.5f;
    private Vector3 Vec3 = Vector3.one;

    float CurrentXCenter;
    float HalfBound;
    float RateChange;
    List<Transform> currentWidgets = new List<Transform>();
    private const string RoleCreateView = "RoleCreateView";
    private void DragStart()
    {
        JSTimer.Instance.SetupTimer(RoleCreateView, delegate { UpdateDrag(); },0.01f);
        CurrentXCenter = (RightBound + LeftBound) / 2; //1000.815
        HalfBound = Mathf.Abs(RightBound - CurrentXCenter);
        RateChange = HalfBound / scaleRate;
        foreach(Transform trans in View.PartnerList_UITable.transform)
        {
            currentWidgets.Add(trans);
        }
    }

    private void UpdateDrag()
    {
        for(int i = 0,max = currentWidgets.Count; i < max; i++)
        {
            Transform trans = currentWidgets[i].transform;
            float sub = Mathf.Abs(1 - Mathf.Abs(trans.position.x - CurrentXCenter)*0.4f);
            if(sub > 1)
                trans.localScale = Vec3;
            else
                trans.localScale = Vec3 * sub;
        }
    }
    #endregion
    #region IViewController Members

    /// <summary>
    ///     从DataModel中取得相关数据对界面进行初始化
    /// </summary>


    protected override void AfterInitView()
    {
        View.RoleNameInput_UIInput.characterLimit = 14;
        //初始化角色按钮
        _roleItemList = new List<RoleItemCellController>(6);
        
        var generalCharactors = DataCache.getArrayByCls<GeneralCharactor>();
        if (null == generalCharactors || generalCharactors.Count <= 0)
            GameDebuger.LogError("GeneralCharactor 数据表读取失败！");
        else
        {
            for (int i = 0, imax = generalCharactors.Count; i < imax; i++)
            {
                var mainCharactor = generalCharactors[i] as MainCharactor;
                if (mainCharactor != null)
                {
                    var com = AddCachedChild<RoleItemCellController, RoleItemCell>(
                        View.RoleGrid_UIGrid.gameObject
                        , RoleItemCellPath);
                    com.SetData(mainCharactor, OnSelectRoleItem);
                    var item = com.gameObject;
                    _roleItemList.Add(com);
                }
            }
        }
        View.RoleGrid_UIGrid.Reposition();
        View.Role2DGroup_Transform.gameObject.SetActive(true);
        AssetPipeline.ResourcePoolManager.Instance.LoadImage(Role2DBackRes,
            asset =>
            {
                View.Back2D_UITexture.mainTexture = asset as Texture;
            });

        GameDebuger.TODO("SensitiveWordFilter.Instance.InitSpecialSymbols();");
        InitModel();
        AudioManager.Instance.PlayMusic("music_login");
    }

    /// <summary>
    ///     Registers the event.
    ///     DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.StartGameButton_UIButton.onClick, OnStartGameButtonClick);
        EventDelegate.Set(View.CloseButton_UIButton.onClick, OnCloseButtonClick);
        EventDelegate.Set(View.RollNameButton_UIButton.onClick, OnRollNameButtonClick);
        EventDelegate.Set(View.RoleNameInput_UIInput.onSubmit, OnSubmitInputName);

        LoginManager.Instance.OnReloginSuccess += HandleOnReloginSuccess;
    }

    /// <summary>
    ///     关闭界面时清空操作放在这
    /// </summary>
    protected override void OnDispose()
    {
        LoginManager.Instance.OnReloginSuccess -= HandleOnReloginSuccess;
        UIHelper.DisposeUITexture(View.InfoTexture_UITexture);

        for (int i = 0; i < _factionItemList.Count; i++)
        {
            _factionItemList[i].Dispose();
        }
        _factionItemList.Clear();
        _modelDisplayer.CleanUpModel();
        currentWidgets.Clear();
        JSTimer.Instance.CancelTimer(RoleCreateView);
        playerModelId = -1;
        weaponModelId = -1;
        UIHelper.DisposeUITexture(View.Back2D_UITexture);
        //UIHelper.DisposeUITexture(View.Role2D_UITexture);
    }

    #endregion

    private void HandleOnReloginSuccess()
    {
        if (_lastRandomRoleName == View.RoleNameInput_UIInput.value)
        {
            OnRollNameButtonClick();
        }
    }
    
    private void OnSelectFactionItem(FactionCellController factionItem)
    {
        TalkingDataHelper.OnEventSetp("CreatePlayer", "选择门派", "Faction", factionItem.Faction.id.ToString());
        SPSDK.gameEvent("10019");   //选择职业
        //if (factionItem.Faction.id != 7)
        //{
        //    TipManager.AddTip("该职业暂未开放");
        //    return; //应策划要求先屏蔽其余职业，只留下魔枪手
        //}
        if (_lastSelectFactionItem != null)
        {
            _lastSelectFactionItem.SetSel(false);
        }

        _lastSelectFactionItem = factionItem;
        _lastSelectFactionItem.SetSel(true);
        weaponModelId = factionItem.Faction.weaponModelId;
        UpdateModel();
        GameDebuger.TODO("View.FactionDescLbl_UILabel.text = factionItem.Faction.description;");
    }

    private void OnCloseButtonClick()
    {
        TalkingDataHelper.OnEventSetp("CreatePlayer", "Back");
        ProxyRoleCreateModule.Close();
        ProxyLoginModule.Show();
        ExitGameScript.Instance.HanderRelogin();
    }

    private void OnSelectRoleItem(RoleItemCellController roleItem)
    {
        MainCharactor mainCharactor = roleItem.Charactor;
        if (!mainCharactor.usable)
        {
            TipManager.AddTip("敬请期待");
            return;
        }
        if (_lastSelectRoleItem == roleItem)
            return;

        if (_lastSelectRoleItem != null)
            _lastSelectRoleItem.SetSelect(false);

        _lastSelectRoleItem = roleItem;
        roleItem.SetSelect(true);

        TalkingDataHelper.OnEventSetp("CreatePlayer", "选择主角", "Character", mainCharactor.id.ToString());
        SPSDK.gameEvent("10018");       //选择角色
        string imageResKey = string.Format("roll_tip_{0}", mainCharactor.id);
        AssetPipeline.ResourcePoolManager.Instance.LoadImage(imageResKey, asset =>
            {
                UIHelper.DisposeUITexture(View.InfoTexture_UITexture);
                Texture2D texture = asset as Texture2D;
                if (texture != null)
                {
                    View.InfoTexture_UITexture.mainTexture = texture;
                    View.InfoTexture_UITexture.MakePixelPerfect();
                }
            });
        playerModelId = mainCharactor.modelId;
        UpdateFaction(mainCharactor);
        _modelDisplayer.SetUpCreateRoleAnimationList(roleItem.AnimString.ToArray());
        //RefreshFactionItemList(_lastSelectRoleItem.Charactor);
        AudioManager.Instance.PlaySound(mainCharactor.sound);
        GameDebuger.Log("playSound:" + mainCharactor.sound);
        
    }

    private void OnRollNameButtonClick()
    {
        TalkingDataHelper.OnEventSetp("CreatePlayer", "随机取名");
        int gender = _lastSelectRoleItem != null ? _lastSelectRoleItem.Charactor.gender : 1;
        ServiceRequestAction.requestServer(Services.Login_RandomName(gender), "", delegate (GeneralResponse e)
            {
                PlayerNameDto nameDto = e as PlayerNameDto;
                View.RoleNameInput_UIInput.value = nameDto.name;
                _lastRandomRoleName = nameDto.name;
            });
    }

    private void OnSubmitInputName()
    {
        TalkingDataHelper.OnEventSetp("CreatePlayer", "手动取名");
    }

    private void OnStartGameButtonClick()
    {
        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_14, true)) return;

        if (_lastSelectRoleItem == null)
        {
            return;
        }

        if (_lastSelectFactionItem == null)
        {
            TipManager.AddTip("请选择门派");
            return;
        }

        if (string.IsNullOrEmpty(View.RoleNameInput_UIInput.value))
        {
            TipManager.AddTip("昵称不能为空");
            return;
        }

        GameDebuger.TODO(@"string specialSymbols = SensitiveWordFilter.Instance.SpecialSymbols.FindOne(View.RoleNameInput_UIInput.value);
        if (SensitiveWordFilter.Instance.HasSpecialSymbols(View.RoleNameInput_UIInput.value)
            || !string.IsNullOrEmpty(specialSymbols))
        {
            if (!string.IsNullOrEmpty(specialSymbols))
            {
                GameDebuger.Log('包含屏蔽字：' + specialSymbols);
            }
            TipManager.AddTip('包含非法关键字');
            return;
        }
            ");

        string error = AppStringHelper.ValidateStrLength(View.RoleNameInput_UIInput.value);
        if (!string.IsNullOrEmpty(error))
        {
            TipManager.AddTip(error);
            return;
        }

        TalkingDataHelper.OnEventSetp("CreatePlayer", "Start");
        SPSDK.gameEvent("10020");       //开始创建角色
        string token = ServerManager.Instance.loginAccountDto.token;
        string ip = HaApplicationContext.getConfiguration().getLocalIp();
        ServiceRequestAction.requestServer(
            Services.Login_PlayerCreate(token, ip,
                View.RoleNameInput_UIInput.value, _lastSelectRoleItem.Charactor.id, _lastSelectFactionItem.Faction.id,
                _serverInfo.serverId, BaoyugameSdk.getUUID(),string.Empty),
            "创建角色中",
            CreatePlayerSuccess, CreatePlayerFail);
    }


    private void CreatePlayerSuccess(GeneralResponse e)
    {
        TalkingDataHelper.OnEventSetp("CreatePlayer", "Success");
        SPSDK.gameEvent("10021");       //创建角色成功
        LoginManager.Instance.OnReloginSuccess -= HandleOnReloginSuccess;

        ProxyLoginModule.Show();

        if (_onCreateSuccess != null)
        {
            _onCreateSuccess(e);
            _onCreateSuccess = null;
        }

        ProxyRoleCreateModule.Close();
    }

    private void CreatePlayerFail(ErrorResponse e)
    {
        TalkingDataHelper.OnEventSetp("CreatePlayer", "Fail");
        SPSDK.gameEvent("10022");       //创建角色失败
        TipManager.AddTip(e.message);
    }

    private void InitModel()
    {
        _modelDisplayer = AddChild<ModelDisplayController, ModelDisplayUIComponent>(
            View.ModelDragregion_UIEventTrigger.gameObject
            , ModelDisplayUIComponent.NAME);

        _modelDisplayer.Init(420, 420,0,1);
        _modelDisplayer.SetBoxColliderEnabled(true);
    }

    private void UpdateModel()
    {
        _modelDisplayer.SetupModel(InitModelStyleInfo());
        _modelDisplayer.SetModelScale(0.9f);
        _modelDisplayer.SetModelOffset(-0.2f);
        var anim = _lastSelectRoleItem.AnimString.Random();
        _modelDisplayer.PlayAnimation(anim);
    }
    private ModelStyleInfo InitModelStyleInfo()
    {
        //        GameDebuger.Log("伙伴id=========" + id);
        ModelStyleInfo model = new ModelStyleInfo();
        model.defaultModelId = playerModelId;
        model.weaponId = weaponModelId;
        return model;
    }

    #region 职业

    private void UpdateFaction(MainCharactor charactor)
    {
        var center = View.PartnerList_UITable.GetComponent<UICenterOnChild>();
        var centerdObject = center.centeredObject;
        var list = charactor.factionIds;
        int poolCount = _factionItemList.Count;
        int count = list.Count;
        for(int i = 0; i < poolCount; i++)
        {
            var item = _factionItemList[i];
            item.gameObject.SetActive(i < count);
            if (i < count)
            {
                int factionId = list[i];
                item.UpdateView(factionId,OnSelectFactionItem, centerdObject);
            }
        }
        for(; poolCount < count; poolCount++)
        {
            var item = NGUITools.AddChild(View.PartnerList_UITable.gameObject, AssetPipeline.ResourcePoolManager.Instance.LoadUI(FactioncellPath));
            item.name = poolCount.ToString();
            int factionId = list[poolCount];
            FactionCellController com = item.GetMissingComponent<FactionCellController>();
            com.UpdateView(factionId,OnSelectFactionItem, centerdObject);
            _factionItemList.Add(com);
        }
        if(centerdObject != null)
            center.CenterOn(centerdObject.transform);
    }

    #endregion
}