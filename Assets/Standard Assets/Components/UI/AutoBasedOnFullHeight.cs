using UnityEngine;
using System.Collections;

public class AutoBasedOnFullHeight : MonoBehaviour
{
	
	// Use this for initialization
	void Start ()
	{
		UpdateOne ();
	}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
	private void Awake()
	{
		UICamera.onScreenResize += UpdateOne;
	}


	private void OnDestroy()
	{
		UICamera.onScreenResize -= UpdateOne;
	}
#endif


	[ContextMenu("Execute")]
	public void UpdateOne ()
	{
		UIWidget widget = this.GetComponent<UIWidget>();
		if (widget != null)
		{
			Transform trans = this.transform;
			float factor = UIRoot.GetPixelSizeAdjustment(this.gameObject);

			float newHeight = Screen.height * factor + 10f;

			widget.height = (int)newHeight;
		}
	}
}

