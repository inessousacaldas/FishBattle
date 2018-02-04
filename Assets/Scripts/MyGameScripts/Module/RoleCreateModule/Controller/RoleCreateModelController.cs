using UnityEngine;
using AssetPipeline;
public class RoleCreateModelController : MonoBehaviour
{
    private bool _autoRotate;
    private CameraPathAnimator _camAnimator;

    private float _isRotatePercentage;
    private Camera _mCamera;
    private GameObject _sceneRoot;
    //private Action _onLoadFinish;
    private bool _isReady = true;
    public bool IsReady
    {
        get { return _isReady; }
    }

    public static RoleCreateModelController GenerateModel()
    {
        GameObject go = new GameObject("RoleCreate");
        DontDestroyOnLoad(go);
        GamePlayer.CameraManager.Instance.SetActive(false);
        var com = go.GetMissingComponent<RoleCreateModelController>();
        return com;
    }

    public void SetupModel(int modelId)
    {
        _isReady = false;
        _autoRotate = false;
        _isRotatePercentage = 0;
        AssetManager.Instance.LoadLevelAsync("hero_" + modelId, false, LoadLevelFinish);
    }

    private void Update()
    {
        if (_autoRotate && _camAnimator != null)
        {
            float delta = 0.1f * Time.deltaTime;
            _isRotatePercentage += delta;
            if (_isRotatePercentage >= 0.2f)
            {
                _autoRotate = false;
            }
            float percentage = _camAnimator.percentage;
            percentage += delta;
            if (percentage > 1f)
                percentage = 0f;
            else if (percentage < 0f)
            {
                percentage = 1f;
            }
            _camAnimator.Seek(percentage);
        }
    }

    private void LoadLevelFinish()
    {
        if (gameObject == null) return;

        _sceneRoot = GameObject.Find("/HeroStage");
        _camAnimator = GameObject.Find("/HeroStage/ModelCameraPath").GetComponent<CameraPathAnimator>();
        _mCamera = GameObject.Find("/HeroStage/ModelCamera").GetComponent<Camera>();
        _mCamera.enabled = false;
        _camAnimator.Seek(0.8f);
        _mCamera.enabled = true;
        _autoRotate = true;

        _isReady = true;
        //if (_onLoadFinish != null)
        //{
        //    _onLoadFinish();
        //}
    }

    public void Seek(float val)
    {
        if (!_isReady) return;

        _autoRotate = false;
        float percentage = _camAnimator.percentage;
        percentage += val > 0 ? 0.01f : -0.01f;
        if (percentage > 1f)
            percentage = 0f;
        else if (percentage < 0f)
        {
            percentage = 1f;
        }
        _camAnimator.Seek(percentage);
        //        Debug.LogError(percentage);
    }

    public void Dispose()
    {
        if (_sceneRoot)
        {
            Destroy(_sceneRoot);
            _sceneRoot = null;
        }
        Destroy(gameObject);

        if (GamePlayer.CameraManager.Instance)
        {
            GamePlayer.CameraManager.Instance.SetActive(true);
        }
    }
}