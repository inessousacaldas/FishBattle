using UnityEngine;
using System.Collections;

public class CameraTextrue : MonoBehaviour
{
    public Material mat;
    public GameObject effectGO;
    public string propertyName;
    public string cameraTag;
    private RenderTexture renderTexture;
    private Camera mainCamera;
    private Camera mCamera;
    void OnEnable()
    {
        if (string.IsNullOrEmpty(cameraTag))
        {
            mainCamera = Camera.main;
        }
        else
        {
            var cameras = Camera.allCameras;
            for (int i = 0; i < cameras.Length; i++)
            {
                var item = cameras[i];
                if (item.CompareTag(cameraTag))
                {
                    mainCamera = item;
                    break;
                }
            }
        }
        if (mainCamera == null)
        {
            Debug.LogError("CameraTextre 找不到对应的摄像机，GameObject 名字为：" + gameObject.name);
        }
        else if (effectGO == null)
        {
            Debug.LogError("CameraTextre 特效挂点effectGO 为Null，GameObject 名字为：" + gameObject.name);
        }
        else
        {
            if (mCamera == null)
            {
                GameObject cameraGO = new GameObject("CopyCamera");
                mCamera = cameraGO.AddComponent<Camera>();
                mCamera.CopyFrom(mainCamera);
                mCamera.enabled = true;
                renderTexture = RenderTexture.GetTemporary(mainCamera.pixelWidth, mainCamera.pixelHeight, 24);
                mCamera.targetTexture = renderTexture;
            }
            else
            {
                mCamera.enabled = true;
            }

            effectGO.SetActive(false);
            StartCoroutine(GetRenderTexture());
        }
    }

    IEnumerator GetRenderTexture()
    {
        yield return null;  //等待下一帧删除摄像机

        mat.SetTexture(propertyName, renderTexture);
        effectGO.SetActive(true);
        mCamera.targetTexture = null;
        Destroy(mCamera.gameObject);

    }

    void OnDisable()
    {
        if (mCamera != null)
        {
            mCamera.enabled = false;
        }
    }
    void OnDestroy()
    {
        mat.SetTexture(propertyName, null);
        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }
    }

}
