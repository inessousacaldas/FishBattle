using UnityEngine;
using System.Collections.Generic;

public class MultipleSelectionViewController : MonoViewController<MultipleSelectionView> {

	private Dictionary<string,System.Action<string>> _optionDic;
	private List<GameObject> _btnGoList = new List<GameObject>();
	private int _activeBtnCount;

    private System.Action _closeCallback;

    protected override void AfterInitView()
    {
        base.View.selectBtnPrefab.name = "selectBtn_0";
		UIButton uiBtn = base.View.selectBtnPrefab.GetComponent<UIButton>();
		EventDelegate.Set(uiBtn.onClick,OnClickSelectionBtn);
		_btnGoList.Add(base.View.selectBtnPrefab);
	}

	public void Open(GameObject target,
		Dictionary<string,System.Action<string>> optionDic,
        MultipleSelectionManager.Side side, 
		int rowCount,
        IEnumerable<Vector3> pos = null)
    {
		_optionDic = optionDic;
        

        InitBtnList();
	    SetRowCount(rowCount);
        ChangeAnchorMode(target,side);
		UICamera.onClick += ClickEventHandler;
	}



    public void SetCloseCallback(System.Action closeCallback)
    {
        _closeCallback = closeCallback;
    }

	private void InitBtnList(){
		int btnCount = _optionDic.Count;
		if(btnCount > _activeBtnCount)
		{
			if(btnCount > _btnGoList.Count){
				for(int i=_btnGoList.Count;i<btnCount;++i){
					GameObject newBtnGo = base.View.btnGridTrans.gameObject.AddChildAndAdjustDepth(base.View.selectBtnPrefab);
                    UIButton uiBtn = newBtnGo.GetComponent<UIButton>();
                    EventDelegate.Set(uiBtn.onClick, OnClickSelectionBtn);
                    _btnGoList.Add(newBtnGo);
				}
			}
			
			for(int i=_activeBtnCount;i<btnCount;++i){
				_btnGoList[i].SetActive(true);
			}
		}
		else{
			for(int i=_btnGoList.Count-1;i>=btnCount;--i){
				_btnGoList[i].SetActive(false);
			}
		}

		int index = 0;
		foreach(string optionName in _optionDic.Keys)
		{
			_btnGoList[index].name = optionName;
			UILabel btnLbl = _btnGoList[index++].GetComponentInChildren<UILabel>();
			btnLbl.text = optionName;
		}
		_activeBtnCount = btnCount;
	}

    public void ShowRedPoint(Dictionary<string, bool> dct)
    {
        for (int i = 0; i < _btnGoList.Count; ++i)
        {
            if (dct.ContainsKey(_btnGoList[i].name))
            {
                var go = _btnGoList[i].transform.Find("PointSprite").gameObject;
                go.SetActive(dct[_btnGoList[i].name]);
            }
        }
    }

    private int _rowCount;
    //设置行的个数
    public void SetRowCount(int count)
    {
        _rowCount = count;
    }

	public void SetupBtnName(int index,string name)
	{
		if(index < _activeBtnCount)
		{
			UILabel btnLbl = _btnGoList[index].GetComponentInChildren<UILabel>();
			btnLbl.text = name;
		}
	}

    public void SetBtnPos(IEnumerable<Vector3> posList)
    {
        posList.ForEachI((pos, idx) =>
        {
            if (idx < _btnGoList.Count)
            {
                _btnGoList[idx].transform.localPosition = pos;
                _btnGoList[idx].gameObject.SetActive(true);
            }
            else
                _btnGoList[idx].gameObject.SetActive(false);
        });
    }

    public void SetUpdateGrid()
    {
        _view.btnGrid.enabled = true;
        _view.btnGrid.Reposition();
    }
	
	private void ChangeAnchorMode(GameObject target,MultipleSelectionManager.Side side){

	    if (_rowCount > 0 && _activeBtnCount > _rowCount)
	    {
	        int col = (int) Mathf.Ceil(((float)_activeBtnCount/ _rowCount));
	        View.ContentBg.height = 18 + 72 * _rowCount;
            View.ContentBg.width = 50 + 200 * col + 10*(col-1);
	        View.btnGrid.maxPerLine = _rowCount;
	    }
	    else
	    {
            base.View.ContentBg.height = 18+72*_activeBtnCount;
	        View.ContentBg.width = 200;
            View.btnGrid.maxPerLine = 0;
	    }

	    int halfWidth = View.ContentBg.width / 2;
        switch (side){
		case MultipleSelectionManager.Side.Left:
			View.viewAnchor.side = UIAnchor.Side.Left;
			View.viewAnchor.pixelOffset = new Vector2(-10f,0f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Right;
			break;
		case MultipleSelectionManager.Side.LeftTop:
			View.viewAnchor.side = UIAnchor.Side.TopLeft;
			View.viewAnchor.pixelOffset = new Vector2(-10f,0f);
			View.ContentBg.rawPivot = UIWidget.Pivot.TopRight;
			break;
		case MultipleSelectionManager.Side.LeftBottom:
			View.viewAnchor.side = UIAnchor.Side.BottomLeft;
			View.viewAnchor.pixelOffset = new Vector2(-10f,0f);
			View.ContentBg.rawPivot = UIWidget.Pivot.BottomRight;
			break;
		case MultipleSelectionManager.Side.Right:
			View.viewAnchor.side = UIAnchor.Side.Right;
			View.viewAnchor.pixelOffset = new Vector2(10f,0f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Left;
			break;
		case MultipleSelectionManager.Side.RightTop:
			View.viewAnchor.side = UIAnchor.Side.TopRight;
			View.viewAnchor.pixelOffset = new Vector2(10f,0f);
			View.ContentBg.rawPivot = UIWidget.Pivot.TopLeft;
			break;
		case MultipleSelectionManager.Side.RightBottom:
			View.viewAnchor.side = UIAnchor.Side.BottomRight;
			View.viewAnchor.pixelOffset = new Vector2(100f,0f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Center;
			break;
		case MultipleSelectionManager.Side.Top:
			View.viewAnchor.side = UIAnchor.Side.Top;
			View.viewAnchor.pixelOffset = new Vector2 (0f, 10f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Bottom;
			break;
		case MultipleSelectionManager.Side.TopLeft:
			View.viewAnchor.side = UIAnchor.Side.TopLeft;
			View.viewAnchor.pixelOffset = new Vector2 (halfWidth, 10f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Bottom;
			break;
		case MultipleSelectionManager.Side.TopRight:
			View.viewAnchor.side = UIAnchor.Side.TopRight;
			View.viewAnchor.pixelOffset = new Vector2 (-halfWidth, 10f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Bottom;
			break;
		case MultipleSelectionManager.Side.Bottom:
			View.viewAnchor.side = UIAnchor.Side.Bottom;
			View.viewAnchor.pixelOffset = new Vector2 (0f, -10f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Top;
			break;
		case MultipleSelectionManager.Side.BottomLeft:
			View.viewAnchor.side = UIAnchor.Side.BottomLeft;
			View.viewAnchor.pixelOffset = new Vector2 (halfWidth, -10f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Top;
			break;
		case MultipleSelectionManager.Side.BottomRight:
			View.viewAnchor.side = UIAnchor.Side.BottomRight;
			View.viewAnchor.pixelOffset = new Vector2 (-halfWidth, -10f);
			View.ContentBg.rawPivot = UIWidget.Pivot.Top;
			break;
		}

//		View.btnGrid.Reposition();
		View.viewAnchor.container = target;
		View.viewAnchor.Update();
	}

	private void OnClickSelectionBtn(){
		string optionName = UIButton.current.name;
		if(_optionDic.ContainsKey(optionName)){
			if(_optionDic[optionName] != null)
				_optionDic[optionName](optionName);
		}

		CloseView();
	}

	void ClickEventHandler(GameObject clickGo){
		UIPanel panel = UIPanel.Find(clickGo.transform);
		if(panel != View.uiPanel)
			CloseView();
	}
	
	void CloseView(){
		_optionDic = null;
		UICamera.onClick -= ClickEventHandler;
		MultipleSelectionManager.Close();

	    if (_closeCallback != null)
	        _closeCallback();
	}
}
