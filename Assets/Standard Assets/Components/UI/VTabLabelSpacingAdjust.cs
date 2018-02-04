using UnityEngine;
using System.Collections;

public class VTabLabelSpacingAdjust : MonoBehaviour
{

	private UILabel _label;
	private int _lastLen = 0;

	public bool autoAdjust = false;

	void Awake()
	{
		_label = this.transform.GetComponentInChildren<UILabel>();
		if (_label == null)
		{
			this.enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
		ReAdjust();
		RefreshLblStyle();
	}

	public void ReAdjust()
	{
		_label.text = _label.text.Replace(" ","");
	}

	// Update is called once per frame
	void Update () {
		if (autoAdjust)
		{
			ReAdjust();
		}

		RefreshLblStyle();
	}

	void RefreshLblStyle()
	{
		if (_lastLen != _label.text.Length)
		{
			_lastLen = _label.text.Length;

			if (_lastLen == 2)
			{
				_label.spacingY = _label.fontSize;
                _label.transform.localPosition = new Vector3(_label.transform.localPosition.x, -10, _label.transform.localPosition.z);
			}
			else
			{
                _label.spacingY = 0;
                _label.transform.localPosition = new Vector3(_label.transform.localPosition.x, -2, _label.transform.localPosition.z);
			}
		}
	}
}
