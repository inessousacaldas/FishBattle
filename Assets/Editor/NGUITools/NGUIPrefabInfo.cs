


using System.Collections.Generic;
using UnityEngine;

public class NGUIPrefabInfo
{
	private GameObject _prefab;

	public GameObject Prefab
	{
		get { return _prefab; }
	}

	/// <summary>
	/// 全部塞进一个列表
	/// </summary>
	private List<NGUIWidgetInfo> _widgetList;

	public List<NGUIWidgetInfo> WidgetList
	{
		get { return _widgetList; }
	}

	/// <summary>
	/// 根据实际depth塞进列表
	/// </summary>
	private Dictionary<int, List<NGUIWidgetInfo>> _widgetDict;

	public Dictionary<int, List<NGUIWidgetInfo>> WidgetDict
	{
		get { return _widgetDict; }
	}

	/// <summary>
	/// 根据Panel Depth换算后的列表
	/// </summary>
	private Dictionary<int, List<NGUIWidgetInfo>> _panelFiexeWidgetList;

	public Dictionary<int, List<NGUIWidgetInfo>> PanelFiexeWidgetList
	{
		get { return _panelFiexeWidgetList; }
	}

	public NGUIPrefabInfo(GameObject prefab)
	{
		_prefab = prefab;
		_widgetList = new List<NGUIWidgetInfo>();
		_widgetDict = new Dictionary<int, List<NGUIWidgetInfo>>();
		_panelFiexeWidgetList = new Dictionary<int, List<NGUIWidgetInfo>>();

		UpdateAllWidgetInfo();
	}


	private void UpdateAllWidgetInfo()
	{
		_widgetList.Clear();
		_widgetDict.Clear();
		_panelFiexeWidgetList.Clear();
		var widgets = _prefab.GetComponentsInChildren<UIWidget>(true);

		foreach (var widget in widgets)
		{
			if (widget is UISprite || widget is UILabel || widget is UITexture)
			{
				var info = new NGUIWidgetInfo(widget);
				//				Debug.Log(info);
				_widgetList.Add(info);

				if (!_widgetDict.ContainsKey(info.RaycastDepth))
				{
					_widgetDict[info.RaycastDepth] = new List<NGUIWidgetInfo>();
				}
				_widgetDict[info.RaycastDepth].Add(info);

				if (!_panelFiexeWidgetList.ContainsKey(info.PanelFixedDepth))
				{
					_panelFiexeWidgetList[info.PanelFixedDepth] = new List<NGUIWidgetInfo>();
				}
				_panelFiexeWidgetList[info.PanelFixedDepth].Add(info);
			}
		}
	}


	public List<NGUIWidgetInfo> GetUISpriteEmptyList()
	{
		var list = new List<NGUIWidgetInfo>();
		foreach (var info in _widgetList)
		{
			if (info.IsUISpriteEmpty())
			{
				list.Add(info);
			}
		}
		return list;
	}

	public List<NGUIWidgetInfo> GetUILabelEmptyList()
	{
		var list = new List<NGUIWidgetInfo>();
		foreach (var info in _widgetList)
		{
			if (info.IsUILabelEmpty())
			{
				list.Add(info);
			}
		}
		return list;
	}

	public List<NGUIWidgetInfo> GetUILabelNotSatisfiedDepthList(int depth)
	{
		var list = new List<NGUIWidgetInfo>();
		foreach (var info in _widgetList)
		{
			if (!info.IsUILabelSatisfiedDepth(depth))
			{
				list.Add(info);
			}
		}
		return list;
	}

	public List<NGUIWidgetInfo> GetSameDepthCannotCombineDrawCall()
	{
		var list = new List<NGUIWidgetInfo>();

		foreach (var widgetList in _widgetDict)
		{
			// 满足可以合并
			var satisfied = true;
			var info1 = widgetList.Value[0];
			for (int i = 1; i < widgetList.Value.Count; i++)
			{
				var info2 = widgetList.Value[i];
				//				if (info1.Widget.GetType() != info2.Widget.GetType() || (info1.Sprite != null && info1.Sprite.atlas != info2.Sprite.atlas) || (info1.Label != null && info1.Label.bitmapFont != info2.Label.bitmapFont) || (info1.Texture != null && info1.Texture.mainTexture != info2.Texture.mainTexture))
				if (info1.Widget.material != info2.Widget.material)
				{
					satisfied = false;
					break;
				}
			}

			if (!satisfied)
			{
				list.AddRange(widgetList.Value);
			}
		}

		return list;
	}


	private int GetWidgetMaxDepth<T>(int panelFixedDepth) where T : UIWidget
	{
		var depth = int.MinValue;

		if (_panelFiexeWidgetList.ContainsKey(panelFixedDepth))
		{
			foreach (var widgetInfo in _panelFiexeWidgetList[panelFixedDepth])
			{
				if (widgetInfo.Widget is T)
				{
					depth = Mathf.Max(widgetInfo.Widget.depth, depth);
				}
			}
		}

		return depth;
	}


	public void FixedLabelDepth(int minDepth)
	{
		foreach (var panel in _panelFiexeWidgetList)
		{
			var depth = Mathf.Max(minDepth, GetWidgetMaxDepth<UISprite>(panel.Key), GetWidgetMaxDepth<UITexture>(panel.Key));
			foreach (var widgetInfo in panel.Value)
			{
				if (widgetInfo.Label != null)
				{
					widgetInfo.Widget.depth = depth;
				}
			}
		}

		UpdateAllWidgetInfo();
	}


	public void CompressWidgetDepth(int minLabelDepth)
	{
		foreach (var panel in PanelFiexeWidgetList)
		{
			// 先按深度进行排序
			panel.Value.Sort((info1, info2) => info1.Widget.depth.CompareTo(info2.Widget.depth));
			var curInfo = panel.Value[0];
			for (int i = 1; i < panel.Value.Count; i++)
			{
				var nextInfo = panel.Value[i];
				var move = true;
				var j = i + 1;
				// 把同depth的抽出来
				for (; j < panel.Value.Count; j++)
				{
					var sameNextInfo = panel.Value[j];
					// 不是同层的，j记录到这里就好了
					if (nextInfo.Widget.depth != sameNextInfo.Widget.depth)
					{
						break;
					}

					// 同层，但是不同mat，这种不给移动
					// 空的mat也不给移动
					if (nextInfo.Widget.material != sameNextInfo.Widget.material || sameNextInfo.Widget.material == null)
					{
						move = false;
						//						break;
					}
				}

				// 如果可以移动，并且满足和前一个深度的相关条件，则可以移动
				if (move && curInfo.Widget.material != null && nextInfo.Widget.material != null && curInfo.Widget.material == nextInfo.Widget.material && nextInfo.Widget.depth > curInfo.Widget.depth + 1)
				{
					// Label有个最小限制，因为前面有mat的判断，所以这里只需要判断最初的一个就行了
					var moveDepth = curInfo.Label == null
						? curInfo.Widget.depth + 1
						: Mathf.Max(curInfo.Widget.depth + 1, minLabelDepth);
					// 保证移动过后不会比原来的大
					moveDepth = Mathf.Min(moveDepth, nextInfo.Widget.depth);
					// 将之前得到的同层的都移过去
					for (int k = i; k < j; k++)
					{
						nextInfo = panel.Value[k];
						nextInfo.Widget.depth = moveDepth;
					}
				}
				i = j - 1;
				curInfo = nextInfo;
			}
		}

		UpdateAllWidgetInfo();
	}


	/// <summary>
	/// 这东西忽略Label
	/// </summary>
	/// <param name="minLabelDepth"></param>
	public void OptimizeWidgetsDepth(int minLabelDepth)
	{
		foreach (var panel in _panelFiexeWidgetList)
		{
			// 先按深度进行排序
			panel.Value.Sort((info1, info2) => info1.Widget.depth.CompareTo(info2.Widget.depth));
			for (int i = 0; i < panel.Value.Count; i++)
			{
				var curInfo = panel.Value[i];
				// 排除Label
				if (curInfo.Label != null)
				{
					continue;
				}
				// 不明的mat排除
				if (curInfo.Widget.material == null)
				{
					continue;
				}

				// 全部都以根节点作为计算Bound
				for (int j = i + 1; j < panel.Value.Count; j++)
				{
					var nextInfo = panel.Value[j];
					// 排除Label
					if (nextInfo.Label != null)
					{
						continue;
					}
					if (nextInfo.Widget.material == null)
					{
						continue;
					}

					// 不同层，相同mat，可以移动
					if (curInfo.Widget.depth != nextInfo.Widget.depth && curInfo.Widget.material == nextInfo.Widget.material)
					{
						var nextBounds = nextInfo.Widget.CalculateBounds(_prefab.transform);

						var k = j - 1;
						for (; k > i; k--)
						{
							// Label跳过
							if (panel.Value[k].Label != null)
							{
								continue;
							}

							// 保留原本相同mat的一个次序
							if (panel.Value[k].Widget.material == nextInfo.Widget.material || panel.Value[k].Widget.CalculateBounds(_prefab.transform).Intersects(nextBounds))
							{
								break;
							}
						}

						// 中间被拦截到了，跳出
						if (k != i)
						{
							continue;
						}
						// 排除Label，必须得和所有同Depth的进行比较
						var moveDepth = EncapsulateBounds(_widgetDict[curInfo.RaycastDepth].FindAll(info => info.Label == null)).Intersects(nextBounds) ? curInfo.Widget.depth + 1 : curInfo.Widget.depth;
						//						var intersectInfo = panel.Value[k];
						//						var moveDepth = intersectInfo.Widget.depth;
						//						// 说明中间被拦截了
						//						if (intersectInfo.Widget.CalculateBounds(_prefab.transform).Intersects(nextBounds))
						//						{
						//							moveDepth = intersectInfo.Widget.depth + 1;
						//						}

						// 不应该越算越大
						if (moveDepth < nextInfo.Widget.depth)
						{
							Debug.Log(string.Format("nextInfo:{0} curDepth:{1} moveDepth:{2}", nextInfo.Widget.name, nextInfo.Widget.depth, moveDepth));
							nextInfo.Widget.depth = moveDepth;
							// 移位
							panel.Value.Remove(nextInfo);
							panel.Value.Insert(k + 1, nextInfo);

							// 移位有可能导致重叠，重叠的往后移 +1
							for (int l = k + 2; l < panel.Value.Count; l++)
							{
								var moveInfo = panel.Value[l];
								// label跳过
								if (moveInfo.Label != null)
								{
									continue;
								}
								// 没重叠了，不用移动了
								if (moveInfo.Widget.depth > moveDepth)
								{
									break;
								}

								// 前一个比后面一个大，说明原本是同层的，则不需要改动
								moveDepth = moveDepth <= moveInfo.Widget.depth ? moveInfo.Widget.depth + 1 : moveDepth;
								Debug.Log(string.Format("moveInfo:{0} curDepth:{1} moveDepth:{2}", moveInfo.Widget.name, moveInfo.Widget.depth, moveDepth));
								moveInfo.Widget.depth = moveDepth;
							}

							// 整个列表都乱套了，重新来
							UpdateAllWidgetInfo();
							OptimizeWidgetsDepth(minLabelDepth);
							return;
						}
					}
				}
			}
		}

		//		UpdateAllWidgetInfo();
	}

	private Bounds EncapsulateBounds(List<NGUIWidgetInfo> infoList)
	{
		var combineBounds = new Bounds();
		foreach (var widgetInfo in infoList)
		{
			combineBounds.Encapsulate(widgetInfo.Widget.CalculateBounds(_prefab.transform));
		}
		return combineBounds;
	}
}
