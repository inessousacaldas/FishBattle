using UnityEngine;
using System.Collections;

public class UI_Contorl_ScrollFlowItem:MonoBehaviour {
    private int _index;
    public int Index {
        get { return _index; }
        set { _index = value; }
    }
    private bool isOnDrag=false;
    private UI_Contorl_ScrollFlow parent;
    public float v=0f,sv=0f;
    private Vector3 p,s;
    private Color color;
    private UISprite sprite,BackGround,Icon,LvSprite,NeetType;
    private UILabel lvLabel;

    void Awake()
    {
        sprite = GetComponent<UISprite>();
        BackGround=transform.Find("BackGround").GetComponent<UISprite>();
        Icon= transform.Find("Icon").GetComponent<UISprite>();
        LvSprite= transform.Find("Lv/Sprite").GetComponent<UISprite>();
        NeetType = transform.Find("Type").GetComponent<UISprite>();
        lvLabel= transform.Find("Lv").GetComponent<UILabel>();
    }
    public void Init(UI_Contorl_ScrollFlow _parent)
    {
        parent = _parent;
        color = sprite.color;
    }


    public void Drag(float value)
    {
        v += value;
        Refresh();
    }

    public void Refresh(){
        p = transform.localPosition;
        p.x = parent.GetPositionX(v);
        p.y = -parent.GetPositionY(v);
        transform.localPosition = p;
        if(v>1||v<0)
        {
            transform.localScale = Vector3.zero;
        }
        else
        {
            //color.a = parent.GetApa(v);
            //color.r = parent.GetApa(v);
            //color.g = parent.GetApa(v);
            //color.b = parent.GetApa(v);
            color = new Color(parent.GetApa(v),parent.GetApa(v),parent.GetApa(v),1f);
            BackGround.color = color;
            Icon.color = color;
            LvSprite.color = color;
            NeetType.color = color;
            lvLabel.color = color;
            sv = parent.GetScale(v);
            s.x = sv;
            s.y = sv;
            transform.localScale = s;
        }
    }

    void OnDrag(Vector2 delta)
    {
        isOnDrag = true;
        parent.OnDrag(delta);
    }
    void OnPress()
    {
        if(isOnDrag)
        {
            parent.OnEndDrag();
            isOnDrag = false;
            //parent._anim = false;
        }
    }

    void OnClick()
    {
        if(!isOnDrag)
            parent.OnPointerClick(this);
    }

    public void Clear()
    {
        isOnDrag = false;
        v = 0;
    }
}
