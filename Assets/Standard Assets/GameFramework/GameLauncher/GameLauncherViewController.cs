using UnityEngine;

public class GameLauncherViewController : MonoBehaviour
{
    private static GameLauncherViewController _instance;
    private GameLauncherView _view;

    public static GameLauncherViewController OpenView(string version)
    {
        if (_instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Built-inAssets/GameLauncherView");
            GameObject module = NGUITools.AddChild(UICamera.eventHandler.gameObject, prefab);
            var com = module.AddMissingComponent<GameLauncherViewController>();
            com.InitView(version);
            _instance = com;
        }
        return _instance;
    }

    public static void CloseView()
    {
        if (_instance != null)
        {
            _instance.Dispose();
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

	public static void ChangeParentLayer(GameObject parentGO)
	{
        if (parentGO == null) return;

        if (_instance != null)
        {
            UIPanel panel = _instance.GetComponent<UIPanel>();
            if (panel != null)
            {
                panel.cachedTransform.parent = parentGO.transform;
                panel.cachedTransform.localPosition = Vector3.zero;
            }
        }
    }

    public static void ShowTips(string tips)
	{
		if (_instance != null)
		{
			_instance.DoUpdateTips(tips);
		}
	}

    private void InitView(string version)
    {
        _view = gameObject.AddMissingComponent<GameLauncherView>();
        _view.Setup(transform);

        _view.tipsLbl.cachedGameObject.SetActive(false);

        _view.VersionLabel_UILabel.text = version;

		_view.LogoTexture_UITexture.mainTexture = Resources.Load<Texture>("Textures/logo");
        _view.LogoTexture_UITexture.MakePixelPerfect();
        _view.InitBgTexture_UITexture.mainTexture = Resources.Load<Texture>("Textures/LoadingBG/loginBG");
    }

    private void Dispose()
    {
        ReleaseTexture(_view.LogoTexture_UITexture);
        ReleaseTexture(_view.InitBgTexture_UITexture);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void ReleaseTexture(UITexture uiTexture)
    {
        if (uiTexture != null)
        {
            Texture tex = uiTexture.mainTexture;
            if (tex != null)
            {
                uiTexture.mainTexture = null;
                Resources.UnloadAsset(tex);
            }
        }
    }

    private void DoUpdateTips(string tips)
    {
        if (string.IsNullOrEmpty(tips))
        {
            _view.tipsLbl.cachedGameObject.SetActive(false);
        }
        else
        {
            _view.tipsLbl.text = tips;
            _view.tipsLbl.cachedGameObject.SetActive(true);
        }
    }
}