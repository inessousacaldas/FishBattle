// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleOrderViewContorller.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;
using System.Collections.Generic;
using UniRx;
using AppDto;

public partial interface IBattleOrderViewContorller
{
    UniRx.IObservable<string> BtnClickEvt { get; }
}
public partial class BattleOrderViewContorller
{
    //每秒移动的时间   //一个格子移动的时间
    private float _movePerSec,mMoveSec=0.1f;
    //圆的半径,    //中心点的角度 ,    //中心点角度  //最大角度 //最小角度
    private float _radius,_centerAngle,_oneAngle,_maxAngle,_minAngle;
    //圆心坐标
    private Vector3 _circlePoint;
    private List<float> _angleList;
    private List<Transform> btnTransList =new List<Transform>(6);
    //是否在转动中,是否改变状态,是否发送消息
    private bool _isMoveAni = false,isTweenning,isSendMessage;

    private CompositeDisposable _disposable;

    private string _curBtnName;
    private Subject<string> _currClickEvt;

    //客户端所有技能读表数据
    private Dictionary<int, Skill> passiveCfgList;

    public UniRx.IObservable<string> BtnClickEvt {
        get { return _currClickEvt; }
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        _currClickEvt = new Subject<string>();
        
        btnTransList.Add(_view.AttackButton_Transform);
        btnTransList.Add(_view.MoveButton_Transform);
        btnTransList.Add(_view.EscapeButton_Transform);
        btnTransList.Add(_view.UserItemButton_Transform);
        btnTransList.Add(_view.SkillButton_Transform);
        btnTransList.Add(_view.MagicButton_Transform);
        
        
        Vector3 pos1 = _view.EscapeButton_Transform.localPosition;
        Vector3 pos3 = _view.AttackButton_Transform.localPosition;
        Vector3 pos6 = _view.MagicButton_Transform.localPosition;
        //中心点的坐标
        Vector3 _centerPoint = pos3;
        Vector3 posint = ToClicle(pos1,pos3,pos6,out _radius);
        _centerAngle = GetAngle(posint,_centerPoint,_radius);
        float angle1 = GetAngle(posint, pos1, _radius);
        float angle5 = GetAngle(posint, pos6, _radius);
        //显示距离
        _oneAngle = (angle5 - angle1) / 3;
        //速度
        _movePerSec = _oneAngle / mMoveSec;
        _angleList = new List<float>();
        #region 后期优化需要改成插件的形式
        
        
        #endregion
        for(int i = 0;i < 6;i++)
        {
            float tAngle = angle1 + _oneAngle * (i - 1);
            _angleList.Add(tAngle);
        }
        isSendMessage = false;
        MoveToCenter(btnTransList[0]);
        _minAngle = _angleList[0] - _oneAngle;
        _maxAngle = _angleList[_angleList.Count - 1] + _oneAngle;
        passiveCfgList = DataCache.getDicByCls<Skill>();
        View.BodyTween_TweenScale.PlayForward();
        View.BodyTween_TweenScale.SetOnFinished(() =>
        {
            if(View.BodyTween_TweenScale.transform.localScale == Vector3.zero)
            {
                //隐藏的时候还原坐标和角度
                isSendMessage = false;
                _curBtnName = "";
            }
            MoveToCenter(btnTransList[0]);
        });
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _disposable = new CompositeDisposable();
        _disposable.Add(MoveButton_UIButtonEvt.Subscribe(go =>
            OnSelectFactionItem(go.transform)));
        _disposable.Add(MagicButton_UIButtonEvt.Subscribe(go =>
            OnSelectFactionItem(go.transform)));
        _disposable.Add(EscapeButton_UIButtonEvt.Subscribe(go =>
            OnSelectFactionItem(go.transform)));
        _disposable.Add(AttackButton_UIButtonEvt.Subscribe(go =>
            OnSelectFactionItem(go.transform)));
        _disposable.Add(UserItemButton_UIButtonEvt.Subscribe(go =>
            OnSelectFactionItem(go.transform)));
        _disposable.Add(SkillButton_UIButtonEvt.Subscribe(go =>
            OnSelectFactionItem(go.transform)));
    }

    protected override void OnDispose()
    {
        isShown = false;
        _disposable = _disposable.CloseOnceNull();
        _currClickEvt = _currClickEvt.CloseOnceNull();
        JSTimer.Instance.CancelTimer("RoleCreateMove");
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    private void OnSelectFactionItem(Transform trans)
    {
        //如果重复点击相同的按钮和转盘正在移动中就不能点击
        if(_curBtnName == trans.name|| _isMoveAni || trans == null)
            return;
        _curBtnName = trans.name;
        isSendMessage = true;
        MoveToCenter(trans);
    }


    #region 旋转逻辑代码
    /// <summary>
    /// 将某个Gameobject选中
    /// </summary>
    /// <param name="item"></param>
    private void MoveToCenter(Transform transform)
    {
        float angle = GetAngle(_circlePoint,transform.localPosition,_radius);
        float moveAngle = _centerAngle - angle;
        if(transform.transform.localPosition.y < 0 &&isSendMessage)
        {
            moveAngle = 360 + moveAngle;
        }
        StartMoveTimer(moveAngle);
    }

    /// <summary>
    /// 移动所有UI图标
    /// </summary>
    /// <param name="moveAngle">需要移动的角度</param>
    private void StartMoveTimer(float moveAngle)
    {
        float curTime = Time.time;
        float absMiveAngle = Mathf.Abs(moveAngle);
        int direct = moveAngle > 0 ? 1 : -1;
        List<float> startAngleList = new List<float>();
        startAngleList.AddRange(_angleList);
        _isMoveAni = true;
        JSTimer.Instance.SetupTimer("RoleCreateMove",() =>
        {
            float passTime = Time.time - curTime;
            float angle = _movePerSec * passTime;
            if(angle >= absMiveAngle)
            {
                for(int i = 0;i < startAngleList.Count;++i)
                {
                    RefreshFactionPos(i,startAngleList[i] + moveAngle);
                }
                ReleaseMoveTimer();
            }
            else
            {
                angle = angle * direct;
                for(int i = 0;i < startAngleList.Count;++i)
                {
                    RefreshFactionPos(i,startAngleList[i] + angle);
                }
            }
        },0.01f);
    }



    /// <summary>
    /// 顺时针的逆时针计算，如果后期需优化从小角度，需要从这里下手
    /// </summary>
    /// <param name="i">第几个Item</param>
    /// <param name="angle">当前的角度</param>
    /// <param name="checkSelectCenter"></param>
    private void RefreshFactionPos(int i,float angle,bool checkSelectCenter=false)
    {
        if(i >= _angleList.Count)
        {
            GameDebuger.Log("超出数组索引");
            return;
        }
        //_view.ContorllerBG_Transform.Rotate(0,0,angle);
        if(angle <= _minAngle)
        {
            angle = _maxAngle - (_minAngle - angle) - _oneAngle;
        }
        else if(angle >= _maxAngle)
        {
            angle = _minAngle + (angle - _maxAngle) + _oneAngle;
        }
        if(checkSelectCenter)
        {
            //自动选中中间
            if(Mathf.Abs(angle - _centerAngle) < _oneAngle / 2)
            {
                //OnSetSelectFaction(_factionItemList[i]);
            }
        }
        _angleList[i] = angle;
        if(btnTransList[i] != null)
            btnTransList[i].transform.localPosition = GetPoint(_circlePoint,_radius,angle);
    }


    /// <summary>
    /// 清除计时器,执行回调
    /// </summary>
    private void ReleaseMoveTimer()
    {
        if(_isMoveAni)
        {
            _isMoveAni = false;
            JSTimer.Instance.CancelTimer("RoleCreateMove");
            if(isSendMessage)
                _currClickEvt.OnNext(_curBtnName);
        }
    }
    #endregion

    #region 控制指令盘的显示和隐藏

    /// <summary>
    /// 控制指令盘的显示和隐藏
    /// </summary>
    /// <param name="IsShowSkillBtns">ture为显示，false为隐藏</param>
    private bool isShown;
    public void UpdateView(bool IsShowSkillBtns, string skillID)
    {
        if (isShown && isShown == IsShowSkillBtns)
            return;
        isShown = IsShowSkillBtns;
        if(isShown)
        {
            View.BodyTween_TweenScale.PlayForward();
            UIHelper.SetUITexture(View.QuickIcon_UITexture,skillID);
            View.QuickIcon_UITexture.MakePixelPerfect();
        }
        else
        {
            if(View.BodyTween_TweenScale.transform.localScale == Vector3.one)
            {
                View.BodyTween_TweenScale.PlayReverse();
            }
        }
    }
    #endregion

    #region 角度计算开始
    /// <summary>
    /// 输入圆弧上的三个点，算出圆的中心和半径
    /// </summary>
    /// <param name="pt1">点一</param>
    /// <param name="pt2">点二</param>
    /// <param name="pt3">点三</param>
    /// <param name="radius">点四</param>
    /// <returns></returns>
    private Vector3 ToClicle(Vector3 pt1,Vector3 pt2,Vector3 pt3,out float radius)
    {
        float x1 = pt1.x,x2 = pt2.x,x3 = pt3.x;
        float y1 = pt1.y,y2 = pt2.y,y3 = pt3.y;
        float a = x1 - x2;
        float b = y1 - y2;
        float c = x1 - x3;
        float d = y1 - y3;
        float e = ((x1 * x1 - x2 * x2) + (y1 * y1 - y2 * y2)) / 2.0f;
        float f = ((x1 * x1 - x3 * x3) + (y1 * y1 - y3 * y3)) / 2.0f;
        float det = b * c - a * d;
        if(Mathf.Abs(det) < 1e-5)
        {
            radius = -1;
            return new Vector3(0f,0f,0f);
        }

        float x0 = -(d * e - b * f) / det;
        float y0 = -(a * f - c * e) / det;
        radius = Vector3.Distance(new Vector3(x0,y0,0),new Vector3(x1,y1,0));
        return new Vector3(x0,y0,0f);

    }

    /// <summary>
    /// 根据当前坐标，圆的半径，角度，算出移动的坐标
    /// </summary>
    /// <param name="point">当前坐标</param>
    /// <param name="radius">半径</param>
    /// <param name="angle">角度</param>
    /// <returns></returns>
    private Vector3 GetPoint(Vector3 point,float radius,float angle)
    {
        float x = point.x + radius * Mathf.Cos(angle * Mathf.PI / 180f);
        float y = point.y + radius * Mathf.Sin(angle*Mathf.PI / 180f);
        return new Vector3(x,y,0);
    }

    private float GetAngle(Vector3 cirlePoint,Vector3 point,float radius)
    {
        float angle = Mathf.Acos((point.x - cirlePoint.x) / radius) *180f / Mathf.PI;
        if(point.y - cirlePoint.y < 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }
    #endregion
}