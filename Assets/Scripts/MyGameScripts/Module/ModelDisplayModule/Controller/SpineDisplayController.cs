
using AssetPipeline;
using Spine.Unity;
using UnityEngine;

public partial class SpineDisplayUIComponent : BaseView
{
    public const string NAME = "ModelDisplayUIComponent";

    public UITexture modelUITexture;
    public BoxCollider boxCollider;

    protected override void LateElementBinding()
    {
        var root = this.gameObject;
        modelUITexture = root.FindScript<UITexture>("");
        boxCollider = root.FindScript<BoxCollider>("");
    }
}

public class SpineDisplayController : MonolessViewController<SpineDisplayUIComponent>
{
    private const string MODEL_RENDERER = "ModelDisplayRenderer";
    private static Vector3 CAMERA_POS = new Vector3(0f, 3.2f, 4f);
    private static Vector3 CamEularAngle = new Vector3(0, 180, 0);

    private static int ModelRendererCount;
    private int _width;
    private int _height;
    private float _orthographicSize = 4;

    private UITexture _mUITexture;
    private GameObject _modelRenderer;
    private Transform _mCamTrans;
    private Camera _mCam;

    private bool isDispose = false;
    private float _yOffset;
    private Vector3 _scale = Vector3.one;

    private GameObject spineGameObject;

    protected override void AfterInitView()
    {
        _mUITexture = View.modelUITexture;

        //必须在Start后才可以设置Camera的targetTexture
        _modelRenderer = AssetPipeline.ResourcePoolManager.Instance.SpawnUIGo(MODEL_RENDERER);
        GameObject.DontDestroyOnLoad(_modelRenderer);

        int index = ModelRendererCount++;
        _modelRenderer.name = "SpineRender_" + index;
        _modelRenderer.transform.position = new Vector3(-500f*index - 1000f, 0, 0);
        _mCamTrans = _modelRenderer.transform.Find("ModelCamera");
        _mCam = _mCamTrans.GetComponent<Camera>();

    }

    public void Init(int width, int height)
    {
        _width = width;
        _height = height;

        int newW = _width;
        int newH = _height;

        var renderTexture = RenderTexture.GetTemporary(newW, newH, 16);
        renderTexture.name = _modelRenderer.name;
        renderTexture.generateMips = false;
        _mCam.targetTexture = renderTexture;
        _mCam.orthographic = true;
        _mCam.orthographicSize = _orthographicSize;
        _mCam.enabled = true;
        _mCam.transform.localEulerAngles = CamEularAngle;
        _mUITexture.mainTexture = _mCam.targetTexture;
        _mUITexture.SetDimensions(newW, newH);
    }

    public void SetSpineID(int spineID)
    {
        ResourcePoolManager.Instance.SpawnModelAsync("Spine_" + spineID, (go) =>
        {
            if (isDispose)
            {
                ResourcePoolManager.Instance.DespawnModel(go);
                return;
            }
            if (spineGameObject != null)
                ResourcePoolManager.Instance.DespawnModel(spineGameObject);

            spineGameObject = go;
            go.transform.parent = _modelRenderer.transform;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;

            SetModelOffset(_yOffset);
            SetModelScale(_scale);
        });
    }

    public void SetModelOffset(float offsetY)
    {
        _yOffset = offsetY;
        if (_mCamTrans != null)
            _mCamTrans.localPosition = CAMERA_POS + new Vector3(0f, -_yOffset, 0f);
    }

    public void SetModelScale(Vector3 scale)
    {
        _scale = scale;
        if (spineGameObject != null)
            spineGameObject.transform.localScale = _scale;
    }

    public void SetBoxCollider(int width, int height)
    {
        if (_mUITexture != null)
        {
            BoxCollider box = _mUITexture.GetComponent<BoxCollider>();
            if (box != null)
            {
                box.size = new Vector3(width, height, 0f);
            }
        }
    }

    protected override void OnDispose()
    {
        isDispose = true;
        if (_mUITexture != null)
        {
            _mUITexture.mainTexture = null;
        }
        //这里要先归还池，否则父节点被Destroy后Render会被Unity Set Disbale
        if (spineGameObject != null)
        {
            ResourcePoolManager.Instance.DespawnModel(spineGameObject);
        }

        if (_modelRenderer != null)
        {
            RenderTexture renderTexture = _mCam.targetTexture;
            _mCam.targetTexture = null;
            GameObject.Destroy(_modelRenderer);
            if (renderTexture != null)
                renderTexture.Release();
        }
        base.OnDispose();
    }
}
