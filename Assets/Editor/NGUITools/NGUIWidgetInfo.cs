
using UnityEngine;

public class NGUIWidgetInfo
{
	public NGUIWidgetInfo(UIWidget widget)
	{
		_widget = widget;

		var parent = _widget.transform.parent;
		while (parent != null)
		{
			var panel = parent.GetComponent<UIPanel>();
			if (panel != null)
			{
				_panel = panel;
				break;
			}

			parent = parent.parent;
		}
	}


	private UIWidget _widget;

	public UIWidget Widget
	{
		get { return _widget; }
	}


	/// <summary>
	/// Panel Depth *1000 之后的值
	/// </summary>
	public int PanelFixedDepth
	{
		get { return _panel != null ? _panel.depth*1000 : -1000; }
	}


	/// <summary>
	/// 某些没有Panel的情况，为了便于区分，使用 -1000
	/// </summary>
	public int RaycastDepth
	{
		get { return Widget.depth + PanelFixedDepth; }
	}

	private UIPanel _panel;


	public UISprite Sprite
	{
		get { return _widget as UISprite;}
	}

	public UILabel Label
	{
		get { return _widget as UILabel;}
	}

	public UITexture Texture
	{
		get { return _widget as UITexture; }
	}


	public bool IsUISpriteEmpty()
	{
		if (Sprite == null || Sprite.atlas != null)
		{
			return false;
		}

		return true;
	}

	public bool IsUILabelEmpty()
	{
		if (Label == null || Label.bitmapFont != null)
		{
			return false;
		}

		return true;
	}

	public bool IsUILabelSatisfiedDepth(int depth)
	{
		if (Label == null || Label.depth == depth)
		{
			return true;
		}

		return false;
	}

	public override string ToString()
	{
		return string.Format("Depth：{0:D4} Mat：{1} Path：{2}", RaycastDepth, _widget.material != null ? _widget.material.name : "null",  NGUITools.GetHierarchy(_widget.gameObject));
	}
}
