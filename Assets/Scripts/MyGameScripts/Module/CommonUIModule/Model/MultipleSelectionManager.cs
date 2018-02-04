using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Multiple selection manager.
/// 用于游戏中多项选择列表
/// </summary>
public static class MultipleSelectionManager{
	
	public const string VIEWNAME ="MultipleSelectionView";
	//12种对齐样式
	public enum Side
	{
		Left,
		LeftTop,
		LeftBottom,
		Right,
		RightTop,
		RightBottom,
		Top,
		TopLeft,
		TopRight,
		Bottom,
		BottomLeft,
		BottomRight,
	}
	
	private static MultipleSelectionViewController _instance;
	public static void Open(
	    GameObject target
	    , Dictionary<string,System.Action<string>> optionDic
	    , Side side
	    , System.Action closeCallback=null
	    , int rowCount = 0
	    , Dictionary<string, bool> redpointDct=null
        ,IEnumerable<Vector3> btnPosList = null )
    {
		if(optionDic == null || optionDic.Count == 0) return;
		if(_instance == null)
            _instance = UIModuleManager.Instance.OpenFunModule<MultipleSelectionViewController>(VIEWNAME, UILayerType.FourModule, false);

        _instance.Open(target, optionDic, side, rowCount);
        _instance.SetCloseCallback(closeCallback);

	    if (redpointDct != null)
            _instance.ShowRedPoint(redpointDct);

	    if (btnPosList != null)
	        _instance.SetBtnPos(btnPosList);
	    else
	        _instance.SetUpdateGrid();
    }
	
	public static void Close(){
        UIModuleManager.Instance.CloseModule(VIEWNAME);
	}

	public static void Dispose(){
		UIModuleManager.Instance.CloseModule(VIEWNAME);
		_instance = null;
	}
}
