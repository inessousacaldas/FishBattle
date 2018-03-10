using System;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class TitleLbView : BaseView
{
	public const string Name = "TitleLabel";
	public UILabel Title_UILabel;

	protected override void InitElementBinding()
	{
		var root = this.gameObject.transform;
		Title_UILabel = root.GetComponent<UILabel>();
	}
}

public class TitleLbController : MonolessViewController<TitleLbView>
{
	public void SetInfo(string str)
	{
		_view.Title_UILabel.text = str;
	}
}
