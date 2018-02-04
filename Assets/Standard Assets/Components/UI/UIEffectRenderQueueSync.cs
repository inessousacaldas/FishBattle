using UnityEngine;
using System.Collections;

public class UIEffectRenderQueueSync : MonoBehaviour
{
	private UIWidget _widget;
	private Renderer[] _rendererList;
	public int renderQ = 0;
	public int additiveQueue = 0;
	public const int DEFAULT_RENDERQ = 3080;

	public bool needClip = false;
	public UIPanel _panel;

	public void Init ()
	{
		if (_rendererList == null)
			_rendererList = this.GetComponentsInChildren<Renderer> (true);

		if (_rendererList != null) {
			_widget = this.GetComponentInParent<UIWidget> ();
			if (_widget != null) {
				if (_widget is UIBasicSprite || _widget is UILabel)
					_widget.onRender = OnRenderWidget;
				else {
					if (this.gameObject.activeInHierarchy) {
						StartCoroutine (DelayUpdateRenderQ ());
					}
				}
			} else {
				UpdateRenderQ (DEFAULT_RENDERQ);
			}

			RecalculateEffectRegion ();
		}
	}

	IEnumerator DelayUpdateRenderQ ()
	{
		yield return null;
		if (_widget != null) {
			UIPanel panel = UIPanel.Find (_widget.cachedTransform);
			if (panel != null) {
				UpdateRenderQ (panel.startingRenderQueue);
			}
		}
	}

	public void RecalculateEffectRegion ()
	{
		if (_rendererList == null)
			return;		
		if (needClip) {
			_panel = UIPanel.Find (this.transform);

			if (_panel != null) {
				Vector3[] worldCorners = _panel.worldCorners;
			    Camera uiCam = NGUITools.FindCameraForLayer(_panel.cachedGameObject.layer);
				if (uiCam == null)
				{
					return;
				}
                Vector3 bottomLeft = uiCam.WorldToViewportPoint (worldCorners [0]);
				Vector3 topRight = uiCam.WorldToViewportPoint (worldCorners [2]);

				//因为WorldToViewportPoint取得的视口参数是bottomLeft为(0,0)，topRight为(1,1)
				//需要但Shader中的视口区间为bottomLeft为(-1,-1)，topRight为(1,1),故需要做区域映射的运算
				Vector4 cr = new Vector4 (2 * bottomLeft.x - 1f, 2 * bottomLeft.y - 1f, 2 * topRight.x - 1f, 2 * topRight.y - 1f);

				for (int i = 0; i < _rendererList.Length; ++i) {
					Material mat = _rendererList [i].material;
                    if(mat != null)
					    mat.SetVector ("_ClipRange", cr);
				}
			}
		} else {
			_panel = null;
			for (int i = 0; i < _rendererList.Length; ++i) {
				Material mat = _rendererList [i].material;
                if (mat != null)
                    mat.SetVector ("_ClipRange", new Vector4 (-1f, -1f, 1f, 1f));
			}
		}
	}

	void OnEnable ()
	{
		Init ();
	}

	void OnDisable ()
	{
		if (_widget != null) {
			_widget.onRender = null;
			_widget = null;
		}
	}

	void OnRenderWidget (Material mat)
	{
		int newRenderQ = mat.renderQueue + additiveQueue;
		if (renderQ != newRenderQ) {
			UpdateRenderQ (newRenderQ);
		}
	}

	private void UpdateRenderQ (int newRenderQ)
	{
		renderQ = newRenderQ;
		for (int i = 0; i < _rendererList.Length; ++i) {
			if (_rendererList [i] != null)
				_rendererList [i].material.renderQueue = newRenderQ;
		}
	}

	//销毁时清除clone出来的材质球
	void OnDestroy ()
	{
		if (_rendererList != null) {
			for (int i = 0; i < _rendererList.Length; ++i) {
				if (_rendererList [i] != null && _rendererList [i].material != null) {
					DestroyImmediate (_rendererList [i].material);
				}
			}
		}
	}
}
