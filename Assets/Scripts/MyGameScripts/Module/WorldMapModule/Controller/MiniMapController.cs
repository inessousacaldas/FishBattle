using System;
using System.Collections;
using System.Collections.Generic;
using AppDto;
using UnityEngine;
using AssetPipeline;

public class MiniMapController : MonoViewController<MiniMapView>
{
    private static MinMapConfig _minMapConfig;
    private readonly List<GameObject> _walkPointList = new List<GameObject>();
    private readonly Queue<GameObject> _cachedPoints = new Queue<GameObject>();

    private HeroView _heroView;
    private float _mapOffX = 299f;
    private float _mapOffY = 333f;
    private float _scale = 72f / 10f;

    private float _timer;

    private GameObject _walkPointPrefab;

    #region IViewController Members

    /// <summary>
    ///     从DataModel中取得相关数据对界面进行初始化
    /// </summary>
    protected override void AfterInitView()
    {
        _walkPointPrefab = ResourcePoolManager.Instance.SpawnUIGo("WalkPointSprite");
        ChangeSkinHelper.ChangeMiniMapSkin(this);
    }

    /// <summary>
    ///     Registers the event.
    ///     DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(View.Texture_UIEventTrigger.onClick, OnClickMinMapTexture);
        EventDelegate.Set(View.CloseBtn_UIButton.onClick, CloseView);
        EventDelegate.Set(View.ChangeMapBtn_UIButton.onClick, OnChangeMapButtonClick);
    }

    /// <summary>
    ///     关闭界面时清空操作放在这
    /// </summary>
    protected override void OnDispose()
    {
        UIHelper.DisposeUITexture(View.Texture_UITexture);
        _heroView = null;
        _walkPointPrefab = null;
    }

    #endregion

    public void Open()
    {
        View.Content.SetActive(false);
        if (_minMapConfig == null)
        {
            ResourcePoolManager.Instance.LoadConfig("MiniMapConfig", asset =>
                {
                    if (asset != null)
                    {
                        var textAsset = asset as TextAsset;
                        if (textAsset != null)
                        {
                            _minMapConfig = JsHelper.ToObject<MinMapConfig>(textAsset.text);
                            InitMinMapData();
                        }
                    }
                });
        }
        else
        {
            InitMinMapData();
        }
    }

    private void CloseView()
    {
        ProxyWorldMapModule.CloseMiniMap();
    }

    private void OnChangeMapButtonClick()
    {
        ProxyWorldMapModule.CloseMiniMap();
        ProxyWorldMapModule.OpenMiniWorldMap();
    }

    private void OnClickMinMapTexture()
    {
        if (JoystickModule.DisableMove)
            return;

        Vector3 clickPos = UICamera.lastEventPosition;
        float factor = UIRoot.GetPixelSizeAdjustment(gameObject);

        float startX = (Screen.width - View.Texture_UITexture.width / factor) / 2;
        float startY = (Screen.height - View.Texture_UITexture.height / factor) / 2;

        float x = clickPos.x - startX;
        float y = Screen.height - clickPos.y - startY;

        x *= factor;
        y *= factor;

        var dest = new Vector3(-1 * (_mapOffX - x) / _scale, 0, -1 * (y - _mapOffY) / _scale);
        dest = SceneHelper.GetPositionInScene(dest.x, dest.y, dest.z);
        GameDebuger.Log("walkTo=" + dest);
        _heroView.WalkToPoint(dest);
        GameDebuger.TODO(@"MissionDataModel.Instance.HeroCharacterControllerEnable(true, 0);");

        View.WalkSpriteTrans.localPosition = new Vector3(x, -y, 0);
        View.WalkSpriteGo.SetActive(true);

        StartCoroutine(DelayUpdatePathPoint());
    }

    IEnumerator DelayUpdatePathPoint()
    {
        yield return null;
        yield return null;
        UpdateWalkPoints(_heroView.GetWalkPathList());
    }

    private Vector3 ConverWorldPosToMinMapPos(float worldX, float worldZ)
    {
        return new Vector3(worldX * _scale + _mapOffX, worldZ * _scale - _mapOffY, 0);
    }

    private void UpdateWalkPoints(Vector3[] corners)
    {
        for (int i = 0; i < _walkPointList.Count; i++)
        {
            DespawnWalkPoint(_walkPointList[i]);
        }
        _walkPointList.Clear();

        for (int i = 0, len = corners.Length; i < len; i++)
        {
            if (i + 1 < len)
            {
                GeneratePoints(corners[i], corners[i + 1]);
            }
        }
    }

    private GameObject SpawnWalkPoint()
    {
        GameObject go = null;
        if (_cachedPoints.Count > 0)
        {
            go = _cachedPoints.Dequeue();
            go.SetActive(true);
            return go;
        }

        return NGUITools.AddChild(View.SignPanel, _walkPointPrefab);
    }

    private void DespawnWalkPoint(GameObject go)
    {
        go.SetActive(false);
        _cachedPoints.Enqueue(go);
    }

    private const float Grap = 3f;
    /// <summary>
    ///     通过插值生成两点之间的点
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void GeneratePoints(Vector3 start, Vector3 end)
    {
        var beginPoint = new Vector2(start.x, start.z);
        var endPoint = new Vector2(end.x, end.z);

        float Ox = beginPoint.x;
        float Oy = beginPoint.y;

        float radian = (float)Math.Atan2(endPoint.y - Oy, endPoint.x - Ox);
        float totalLen = Vector2.Distance(beginPoint, endPoint);

        float currLen = Grap;
        float x, y;

        while (currLen < totalLen)
        {
            x = Ox + (float)Math.Cos(radian) * currLen;
            y = Oy + (float)Math.Sin(radian) * currLen;

            var go = SpawnWalkPoint();
            go.transform.localPosition = ConverWorldPosToMinMapPos(x, y);
            _walkPointList.Add(go);

            currLen += Grap;
        }
    }

    private void Update()
    {
        if (_heroView == null)
            return;

        //人物标志行走动画
        _timer += Time.deltaTime;
        if (_timer > 0.3f)
        {
            _timer = 0f;
            View.HeroSpriteTrans.localEulerAngles = new Vector3(0, View.HeroSpriteTrans.localEulerAngles.y + 180, 0);
        }

        var heroWorldPos = _heroView.transform.localPosition;
        var heroPosition = ConverWorldPosToMinMapPos(heroWorldPos.x, heroWorldPos.z);
        View.HeroSpriteTrans.localPosition = heroPosition;

        int needReomve = 0;
        for (int i = _walkPointList.Count - 1; i >= 0; i--)
        {
            var go = _walkPointList[i];
            var distance = heroPosition - go.transform.localPosition;
            float magnitude = distance.magnitude;

            if (magnitude < 1.5f || i < needReomve)
            {
                needReomve = i;
                DespawnWalkPoint(go);
                _walkPointList.Remove(go);
            }
        }

        if (View.WalkSpriteGo.activeInHierarchy)
        {
            var distance2 = heroPosition - View.WalkSpriteTrans.localPosition;
            if (distance2.magnitude < 0.5f)
            {
                View.WalkSpriteGo.SetActive(false);
            }
        }
    }

    #region 初始化小地图数据

    private void InitMinMapData()
    {
        if (WorldManager.Instance.GetModel().GetSceneDto() == null)
        {
            View.Texture_UITexture.mainTexture = null;
            View.Texture_UITexture.gameObject.SetActive(false);
            return;
        }

        View.Texture_UITexture.gameObject.SetActive(true);

        int resId = WorldManager.Instance.GetModel().GetSceneDto().sceneMap.resId;

        string configStr = null;
        _minMapConfig.map.TryGetValue(resId.ToString(), out configStr);
        if (string.IsNullOrEmpty(configStr))
        {
            Debug.LogError(string.Format("预览图{0}无配置数据", resId));
            configStr = "100,200,200";
        }

        var configs = configStr.Split(',');
        _scale = float.Parse(configs[0]) / 10f;
        _mapOffX = float.Parse(configs[1]);
        _mapOffY = float.Parse(configs[2]);

        _heroView = WorldManager.Instance.GetHeroView();
        View.WalkSpriteGo.SetActive(false);

        string imageResKey = string.Format("{0}", resId);
        ResourcePoolManager.Instance.LoadImage(imageResKey, asset =>
            {
                if (asset != null)
                {
                    var tex = asset as Texture2D;
                    UIHelper.DisposeUITexture(View.Texture_UITexture);

                    View.Texture_UITexture.mainTexture = tex;
                    if (tex != null)
                    {
                        View.Texture_UITexture.width = View.Texture_UITexture.mainTexture.width;
                        View.Texture_UITexture.height = View.Texture_UITexture.mainTexture.height;

                        View.BgSprite_UISprite.width = View.Texture_UITexture.width;
                        View.BgSprite_UISprite.height = View.Texture_UITexture.height;

                        View.ChangeMapBtn_UISprite.UpdateAnchors();

                        View.TopBorderSprite_UISprite.width = View.Texture_UITexture.width + 14;
                        View.TopBorderSprite_UISprite.height = View.Texture_UITexture.height + 14;

                        View.Texture_UITexture.cachedTransform.localPosition =
                            new Vector3(-1 * View.Texture_UITexture.width / 2,
                                View.Texture_UITexture.height / 2, 0);

                        View.CloseBtn_UIButton.transform.localPosition = new Vector3(
                            View.Texture_UITexture.width / 2 - 8,
                            View.Texture_UITexture.height / 2 - 10, 0);
                    }
                }
                Update();

                View.Content.SetActive(true);

                View.Texture_UITexture.ResizeCollider();

                var npcs = WorldManager.Instance.GetNpcViewManager().GetNpcUnits();
                foreach (var npc in npcs.Values)
                {
                    if (npc.IsVisible())
                    {
                        Npc.NpcType tNPCType = (Npc.NpcType)(npc.GetNpc().type);
                        if (tNPCType == Npc.NpcType.DoubleTeleport && npc.GetNpc() is NpcDoubleTeleport)
                        {
                            AddTeleportNpc(npc.GetNpc());
                        }
                        else if (tNPCType == Npc.NpcType.DoubleTeleport && npc.GetNpc() is NpcSceneTeleportUnit)
                        {
                            AddSceneTeleportNpc(npc as NpcSceneTeleportUnit);
                        }
                        else if (tNPCType == Npc.NpcType.General)
                        {
                            //这里有一个坑爹的处理，因为动态npc的原因，所以一些Monster怪也设置为NpcType.General类型的
                            AddGeneralNpc(npc.GetNpc() as NpcGeneral);
                        }
                    }
                }
            });
    }

    private string GetNpcName(string npcName)
    {
        return npcName.Replace("#", "\n");
    }

    private void AddTeleportNpc(Npc npc)
    {
        if (npc is NpcDoubleTeleport)
        {
            var go = NGUITools.AddChild(View.SignPanel,
                ResourcePoolManager.Instance.SpawnUIGo("TeleportSprite"));
            var uiLabel = go.GetComponentInChildren<UILabel>();
            if (uiLabel != null)
            {
                uiLabel.text = "[b]" + GetNpcName(npc.name);
            }

            go.transform.localPosition = ConverWorldPosToMinMapPos(npc.x, npc.z);
        }
    }

    // 迷宫中的传送点:传送点是静态点,迷宫中的传送点是动态的
    private void AddSceneTeleportNpc(NpcSceneTeleportUnit npc)
    {
        var go = NGUITools.AddChild(View.SignPanel,
             ResourcePoolManager.Instance.SpawnUIGo("TeleportSprite"));
        var uiLabel = go.GetComponentInChildren<UILabel>();
        if (uiLabel != null)
        {
            uiLabel.text = "[b]" + GetNpcName(npc.GetNpc().name);
        }

        go.transform.localPosition = ConverWorldPosToMinMapPos(npc.getNpcState().x, npc.getNpcState().z);
    }

    private void AddGeneralNpc(NpcGeneral npc)
    {
        if (npc == null)
        {
            return;
        }

        var go = NGUITools.AddChild(View.SignPanel,ResourcePoolManager.Instance.SpawnUIGo("NpcSprite"));
        var nameLabel = go.GetComponentInChildren<UILabel>();
        var pointSprite = go.GetComponentInChildren<UISprite>();
        nameLabel.text = "[b]" + GetNpcName(npc.shortName);

        var textColor = Color.white;
        string pointName = "green-npc";
        switch ((NpcGeneral.NpcGeneralKindEnum)npc.kind)
        {
            case NpcGeneral.NpcGeneralKindEnum.Idler:
                textColor = ColorConstant.Color_MiniMap_Npc_Idle;
                pointName = "yellow-npc";
                break;
            case NpcGeneral.NpcGeneralKindEnum.Function:
                textColor = ColorConstant.Color_MiniMap_Npc_Function;
                pointName = "green-npc";
                break;
            case NpcGeneral.NpcGeneralKindEnum.Area:
                textColor = ColorConstant.Color_MiniMap_Npc_Area;
                pointName = "green-npc";
                break;
        }

        nameLabel.color = textColor;
        pointSprite.spriteName = pointName;

        go.transform.localPosition = ConverWorldPosToMinMapPos(npc.x, npc.z);
    }

    #endregion
}

public class MinMapConfig
{
    /// <summary>
    ///     {"id":3008,"config":"174,182,126"}
    /// </summary>
    public Dictionary<string, string> map = new Dictionary<string, string>();
}