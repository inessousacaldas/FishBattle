using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NGUIPrefabDependence
{
	private GameObject _prefab;

	public GameObject Prefab
	{
		get { return _prefab; }
	}

	private Dictionary<UIAtlas, List<UISprite>> _spriteDict = new Dictionary<UIAtlas, List<UISprite>>();

	public Dictionary<UIAtlas, List<UISprite>> SpriteDict
	{
		get { return _spriteDict; }
	}

	private Dictionary<UIFont, List<UILabel>> _labelDict = new Dictionary<UIFont, List<UILabel>>();

	public Dictionary<UIFont, List<UILabel>> LabelDict
	{
		get { return _labelDict; }
	}

	private Dictionary<Texture, List<UITexture>> _textureDict = new Dictionary<Texture, List<UITexture>>();


	public Dictionary<Texture, List<UITexture>> TextureDict
	{
		get { return _textureDict; }
	}


	public NGUIPrefabDependence(GameObject prefab)
	{
		_prefab = prefab;

		Update();
	}


	private void Update()
	{
		_spriteDict.Clear();
		_labelDict.Clear();
		_textureDict.Clear();

		var widgets = _prefab.GetComponentsInChildren<UIWidget>(true);
		foreach (var widget in widgets)
		{
			var nw = new NGUIWidget(widget);
			switch (GetUIWidgetType(widget))
			{
				case WidgetType.Sprite:
				{
					if (nw.IsUISpriteEmpty())
					{
						continue;
					}

					if (!_spriteDict.ContainsKey(nw.Sprite.atlas))
					{
						_spriteDict[nw.Sprite.atlas] = new List<UISprite>();
					}
					_spriteDict[nw.Sprite.atlas].Add(nw.Sprite);

					break;
				}
				case WidgetType.Label:
					{
						if (nw.IsUILabelEmpty())
						{
							continue;
						}

						if (!_labelDict.ContainsKey(nw.Label.bitmapFont))
						{
							_labelDict[nw.Label.bitmapFont] = new List<UILabel>();
						}
						_labelDict[nw.Label.bitmapFont].Add(nw.Label);

						break;
					}
				case WidgetType.Texture:
					{
						if (nw.IsUITextureEmpty())
						{
							continue;
						}

						if (!_textureDict.ContainsKey(nw.Texture.mainTexture))
						{
							_textureDict[nw.Texture.mainTexture] = new List<UITexture>();
						}
						_textureDict[nw.Texture.mainTexture].Add(nw.Texture);

						break;
					}
			}
		}
	}

	public void ChangeSkin(Object oldObj, Object newObj)
	{
		if (oldObj == null || newObj == null || oldObj == newObj)
		{
			return;
		}

		if (oldObj is UIAtlas)
		{
			ChangeSpriteAtlas(oldObj as UIAtlas, newObj as UIAtlas);
		}
		else if (oldObj is UIFont)
		{
			ChangeLabelFont(oldObj as UIFont, newObj as UIFont);
		}
		else if (oldObj is Texture)
		{
			ChangeTexture(oldObj as Texture, newObj as Texture);
		}
	}


	private void ChangeSpriteAtlas(UIAtlas oldAtlas, UIAtlas newAtlas)
	{
		if (_spriteDict.ContainsKey(oldAtlas))
		{
			foreach (var sprite in _spriteDict[oldAtlas])
			{
				// 仅含有小图的才进行更换
				if (newAtlas.GetSprite(sprite.spriteName) != null)
				{
					sprite.atlas = newAtlas;
				}
			}
			Update();
		}
	}


	private void ChangeLabelFont(UIFont oldFont, UIFont newFont)
	{
		if (_labelDict.ContainsKey(oldFont))
		{
			foreach (var label in _labelDict[oldFont])
			{
				label.bitmapFont = newFont;
			}
			Update();
		}
	}


	private void ChangeTexture(Texture oldTexture, Texture newTexture)
	{
		if (_textureDict.ContainsKey(oldTexture))
		{
			foreach (var texture in _textureDict[oldTexture])
			{
				texture.mainTexture = newTexture;
			}
			Update();
		}
	}


	private static WidgetType GetUIWidgetType(UIWidget widget)
	{
		if (widget is UISprite)
		{
			return WidgetType.Sprite;
		}
		else if (widget is UILabel)
		{
			return WidgetType.Label;
		}
		else if (widget is UITexture)
		{
			return WidgetType.Texture;
		}

		return WidgetType.None;
	}


	public enum WidgetType
	{
		None,
		Sprite,
		Label,
		Texture,
		All,
	}


	private class NGUIWidget
	{
		private UIWidget _widget;

		public UIWidget Widget
		{
			get { return _widget; }
		}

		public NGUIWidget(UIWidget widget)
		{
			_widget = widget;
		}

		public UISprite Sprite
		{
			get { return _widget as UISprite; }
		}

		public UILabel Label
		{
			get { return _widget as UILabel; }
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

		public bool IsUITextureEmpty()
		{
			if (Texture == null || Texture.mainTexture != null)
			{
				return false;
			}

			return true;
		}
	}
}
