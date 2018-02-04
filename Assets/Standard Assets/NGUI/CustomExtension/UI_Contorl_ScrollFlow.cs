using System;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class UI_Contorl_ScrollFlow : MonoBehaviour {
    public enum RefreshType
    {
        Init,
        Refresh
    }
    [Header("---滑动力的大小，值越大滑动越快---")]
    public float firctionForce=1;
    [Header("---伙伴头像水平方向的弧形动画曲线---")]
    public AnimationCurve PositionCurveX;
    [Header("---伙伴头像垂直方向的弧形动画曲线---")]
    public AnimationCurve PositionCurveY;
    [Header("---伙伴头像灰度和透明度的弧形动画曲线---")]
    public AnimationCurve ApaCurve;
    [Header("---伙伴头像缩放的弧形动画曲线---")]
    public AnimationCurve ScaleCurve;
    public float VMin=0.1f,VMax=0.9f;
    public bool isRun=false;
    private float StartValue=0.5f,AddValue=0.2f,v=0, width=178f,high=590f;
    private float Addv=0,VK=0,CurrentV=0,Vtotal=0,VT=0,_anim_speed=1f;
    private Vector2 start_point,add_vect;
    private List<UI_Contorl_ScrollFlowItem> Items=new List<UI_Contorl_ScrollFlowItem>();
    private List<UI_Contorl_ScrollFlowItem> GoToFirstItem=new List<UI_Contorl_ScrollFlowItem>();
    private bool _anim=false;
    private UI_Contorl_ScrollFlowItem Current;
    //public event Action<UI_Contorl_ScrollFlowItem> MoveEnd;
    private Transform grid;
    private float scaleWidth=1;
    private Subject<int> _clickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetClickEvt { get { return _clickEvt; } }
    public Transform selectEffect;
    /// <summary>
    /// 调整滑动参数
    /// </summary>
    /// <param name="_scaleWidth">弧度</param>
    /// <param name="_anim_speed">动画速度</param>
    /// <param name="_AddValue">上下间隔</param>
    public void Init(Vector2 localPos,float _scaleWidth=1,float anim_speed=1,float _AddValue = 0.2f)
    {
        grid=transform.Find("PartnerList").transform;
        //selectEffect = transform.Find("PartnerList/SelectEffect").transform;
        grid.localPosition = localPos;
        scaleWidth = _scaleWidth;
        _anim_speed = anim_speed;
        AddValue = _AddValue;
        Refresh(1);
    }

    public void BrushItem(int Send)
    {
        for(int j = 0;j < Items.Count;j++)
        {
            var item=Items[j].GetComponent<UI_Contorl_ScrollFlowItem>();
            item.Clear();
        }
        if(selectEffect != null)
            selectEffect.gameObject.SetActive(false);
        Refresh(Send);
    }

    // Use this for initialization
    private void Refresh(int Send) {
        Items.Clear();
        Current = null;
        var panel=GetComponent<UIPanel>();
        width = panel.width; high = panel.height;
        for(var i = 0;i < grid.childCount;i++)
        {
            var tran=grid.GetChild(i);
            if(!tran.gameObject.activeInHierarchy) { continue; }
            var item=tran.GetComponent<UI_Contorl_ScrollFlowItem>();

            if (item == null) continue;
            Items.Add(item);
            item.Init(this);
            item.Drag(StartValue + (Items.Count - 1) * AddValue);
            if(item.v-0.5<0.05f)
            {
                Current = item;
                if(selectEffect !=null)
                    selectEffect.localPosition = Current.transform.localPosition;
            }
        }

        if(Items.Count<5)
        {
            VMax = (float)(0.5 + (Items.Count - 1) * AddValue);
            VMin = (float)(0.5 - (Items.Count - 1) * AddValue);
        }
        else
        {
            VMax = 0.9f + (Items.Count - 5) * AddValue;
            VMin = 0.1f;
        }
        Check(1);
        for(int i = 0;i < Items.Count;i++)
        {
            Items[i].Refresh();
        }
        if(Current != null)
        {
            if(selectEffect != null)
                selectEffect.gameObject.SetActive(true);
        }
        if(Send==1 && Current != null)
            _clickEvt.OnNext(Current.Index);
    }

    public void OnDrag(Vector2 wp)
    {
        add_vect = wp;
        v = -wp.y * firctionForce / high;
        for(int i = 0;i < Items.Count;i++)
        {
            Items[i].Drag(v);
        }
        if(selectEffect != null)
            if(selectEffect.gameObject.activeSelf)
                selectEffect.gameObject.SetActive(false);
        Check(v);
    }

    private void Check(float _v)
    {
        if(Items.Count < 5)
        {
            return;
        }
        if(_v < 0)
        {
            for(var i = 0;i < Items.Count;i++)
            {
                if(Items[i].v < (VMin - AddValue / 2))
                {
                    GoToFirstItem.Add(Items[i]);
                }
            }
            if (GoToFirstItem.Count <= 0) return;

            for(var i = 0;i < GoToFirstItem.Count;i++)
            {
                GoToFirstItem[i].v = Items[Items.Count - 1].v + AddValue;
                Items.Remove(GoToFirstItem[i]);
                Items.Add(GoToFirstItem[i]);
            }
            GoToFirstItem.Clear();
        }
        else if(_v > 0)
        {

            for(var i = Items.Count-1;i > 0;i--)
            {
                if(Items[i].v >= VMax)
                {
                    GoToFirstItem.Add(Items[i]);
                }
            }
            if (GoToFirstItem.Count <= 0) return;
            
            for(var i = 0;i < GoToFirstItem.Count;i++)
            {
                GoToFirstItem[i].v = Items[0].v - AddValue;
                Items.Remove(GoToFirstItem[i]);
                Items.Insert(0,GoToFirstItem[i]);
            }
            GoToFirstItem.Clear();
        }
    }

    public void OnEndDrag()
    {
        if(Items.Count < 5)
        {
            if(Items[Items.Count - 1].v > VMax)
            {
                add_vect = Vector3.zero;
                AnimToEnd(VMax - Items[Items.Count - 1].v);
                return;
            }
            if(Items[0].v < VMin)
            {
                add_vect = Vector3.zero;
                AnimToEnd(VMin - Items[0].v);
                return;
            }
        }

        float k=0,v1;
        
        for(var i=0;i<Items.Count;i++)
        {
            if (!(Items[i].v >= VMin)) continue;
            v1 = (Items[i].v - VMin) % AddValue;
            if(add_vect.y >= 0)
            {
                k = AddValue - v1;
            }
            else
            {
                k = v1 * -1;
            }
            break;
        }
        add_vect = Vector3.zero;
        AnimToEnd(k);
    }

    public void AnimToEnd(float k)
    {
        Addv = k;
        if(Addv > 0) { VK = 1; }
        else if(Addv < 0) { VK = -1; }
        else return;

        Vtotal = 0;
        _anim = true;
        isRun = true;
    }

    public void OnPointerClick(UI_Contorl_ScrollFlowItem item)
    {
        if (!(add_vect.sqrMagnitude <= 1)) return;
        var script=item;
        if (script == null) return;
        var k=script.v;
        k = 0.5f - k;
        if(selectEffect != null)
            if(selectEffect.gameObject.activeSelf&& item!=Current)
                selectEffect.gameObject.SetActive(false);
        AnimToEnd(k);
    }

    void Update()
    {
        if (!_anim) return;
        CurrentV = Time.deltaTime * _anim_speed * VK;
        VT = Vtotal + CurrentV;
        if (VK > 0 && VT >= Addv)
        {
            _anim = false;
            CurrentV = Addv - Vtotal;
        }
        if (VK < 0 && VT <= Addv)
        {
            _anim = false;
            CurrentV = Addv - Vtotal;
        }
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].Drag(CurrentV);
            if (Items[i].v - 0.5 < 0.05f)
            {
                Current = Items[i];
            }
        }
        Check(CurrentV);
        Vtotal = VT;
        if (_anim) return;
        _clickEvt.OnNext(Current.Index);
        isRun = false;
        if(selectEffect != null)
        {
            if(!selectEffect.gameObject.activeSelf)
                selectEffect.gameObject.SetActive(true);
            selectEffect.localPosition = Current.transform.localPosition;
        }
    }

    public float GetApa(float v)
    {
        return ApaCurve.Evaluate(v);
    }

    public float GetScale(float v)
    {
        return ScaleCurve.Evaluate(v);
    }

    public float GetPositionX(float v)
    {
        return PositionCurveX.Evaluate(v)*width/2* scaleWidth;
    }

    public float GetPositionY(float v)
    {
        return PositionCurveY.Evaluate(v)*high;
    }
}
