using UnityEngine;

public class GameHintViewController : MonoBehaviour {
	
	public UIAnchor posAnchor;
	public UILabel hintLbl;
	public UISprite hintBg;
	public UIPanel panel;

	private float _time;
	private const float FIXED_OFFSET = 5f;
    private bool _needTimeClose = true;
	
	public void Open(GameObject target,string hint,UIAnchor.Side side,int maxWidth,bool needTimeClose = true,int spacingY = 5)
    {
		if(maxWidth == -1)
			hintLbl.overflowMethod = UILabel.Overflow.ResizeFreely;
		else{
			hintLbl.overflowMethod = UILabel.Overflow.ResizeHeight;
			hintLbl.width = maxWidth;
		}
		hintLbl.text = hint;
	    hintLbl.spacingY = spacingY;
	
		SetupAnchor(target,side);
        _needTimeClose = needTimeClose;
        _time = 0f;
	}

	private void SetupAnchor(GameObject target,UIAnchor.Side side){
		posAnchor.side = UIAnchor.Side.Top;

		if(side == UIAnchor.Side.Top){
			hintLbl.pivot = UIWidget.Pivot.Bottom;
			posAnchor.pixelOffset = new Vector2(0f,FIXED_OFFSET);
		}else if(side == UIAnchor.Side.TopLeft){
			hintLbl.pivot = UIWidget.Pivot.BottomRight;
			posAnchor.pixelOffset = new Vector2(0f,FIXED_OFFSET);
		}else if(side == UIAnchor.Side.TopRight){
			hintLbl.pivot = UIWidget.Pivot.BottomLeft;
			posAnchor.pixelOffset = new Vector2(0f,FIXED_OFFSET);
		}else if(side == UIAnchor.Side.Bottom){
			hintLbl.pivot = UIWidget.Pivot.Top;
			posAnchor.side = UIAnchor.Side.Bottom;
			posAnchor.pixelOffset = new Vector2(0f,-FIXED_OFFSET);
        }else if (side == UIAnchor.Side.BottomRight){
            hintLbl.pivot = UIWidget.Pivot.TopLeft;
            posAnchor.side = UIAnchor.Side.BottomRight;
            posAnchor.pixelOffset = new Vector2(0f, -FIXED_OFFSET);
        }else if(side == UIAnchor.Side.Left){
			hintLbl.pivot = UIWidget.Pivot.Right;
			posAnchor.side = UIAnchor.Side.Left;
			posAnchor.pixelOffset = new Vector2(-FIXED_OFFSET,0f);
		}else if(side == UIAnchor.Side.Right){
			hintLbl.pivot = UIWidget.Pivot.Left;
			posAnchor.side = UIAnchor.Side.Right;
			posAnchor.pixelOffset = new Vector2(FIXED_OFFSET,0f);
		}else if(side == UIAnchor.Side.Center){
			hintLbl.pivot = UIWidget.Pivot.Center;
			posAnchor.pixelOffset = Vector2.zero;
			posAnchor.side = UIAnchor.Side.Center;
		}

		posAnchor.container = target;
		posAnchor.Update();
		
		hintBg.UpdateAnchors();
		if(panel == null)
			panel = UIPanel.Find(hintLbl.cachedTransform);
		
		panel.ConstrainTargetToBounds(hintLbl.cachedTransform,true);
	}
	
	// Use this for initialization
	void OnEnable () {
		UICamera.onClick += ClickEventHandler;
	}
	
	void OnDisable(){
		UICamera.onClick -= ClickEventHandler;
	}
	
	void Update(){
		_time += Time.deltaTime;
		if(_time > GameHintManager.FADEOUT_TIME && _needTimeClose)
		{
			CloseView();
		}
	}
	
	void ClickEventHandler(GameObject go){
		CloseView();
	}
	
	void CloseView(){
		GameHintManager.Close();
	}
}
