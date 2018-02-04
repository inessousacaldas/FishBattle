// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PreviewContainerController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

#region 数据模型
/// <summary>
/// 装备展示专用的~
/// </summary>
public class Pv_EquipmentVo
{
    public EquipmentDto equipmentDto;

    //对应部位的宝石信息
    public EquipmentEmbedCellVo embedVo;
    //战力对比的变化
    public int powerChange;
    //头部标签
    public PreviewHeadController.HeadLabel headLabel
    {
        get
        {
            if (isEquip)
            {
                return PreviewHeadController.HeadLabel.Equip;
            }else if(powerChange > 0)
            {
                return PreviewHeadController.HeadLabel.Recommend;
            }else
            {
                return PreviewHeadController.HeadLabel.None;
            }
        }
       
        
    }
    ////是否已经装备在身上
    public bool isEquip;
    ////装备是否已经加锁
    //public bool isLock;
    ////装备是否可以分解
    //public bool isCanSplit;
    public Pv_EquipmentVo(
        EquipmentDto dto,
        EquipmentEmbedCellVo embedVo,
        List<Pv_ButtonVo> leftButtonList=null,
        Pv_ButtonVo rightButton=null)
    {
        this.equipmentDto = dto;
        this.embedVo = embedVo;
        this.leftButtonList = leftButtonList;
        this.rightButton = rightButton;
    }
    public void SetBtn(List<Pv_ButtonVo> leftButtonList, Pv_ButtonVo rightButton)
    {
        this.leftButtonList = leftButtonList;
        this.rightButton = rightButton;
    }

    public List<Pv_ButtonVo> leftButtonList = new List<Pv_ButtonVo>();
    public Pv_ButtonVo rightButton;
    //返回所有按钮，方便进行查找更改
    public List<Pv_ButtonVo> AllButtons
    {
        get
        {
            var res = new List<Pv_ButtonVo>();
            res.AddRange(leftButtonList);
            res.Add(rightButton);
            return res;
        }
    }
    //按钮名字对应按钮都放到这里~~
    //void UpdataPVButton()
    //{
    //    leftButtonList.Clear();
    //    leftButtonList.Add(new Pv_ButtonVo((int)EquipmentBtnType.Lock, "加锁"));
    //    leftButtonList.Add(new Pv_ButtonVo((int)EquipmentBtnType.Sell, "摆摊"));
    //    leftButtonList.Add(new Pv_ButtonVo((int)EquipmentBtnType.Split, "分解"));
    //    leftButtonList.Add(new Pv_ButtonVo((int)EquipmentBtnType.Strange, "进阶"));

    //    if (isEquip)
    //        rightButton = new Pv_ButtonVo((int)EquipmentBtnType.TakeOff, "卸下");
    //    else
    //        rightButton = new Pv_ButtonVo((int)EquipmentBtnType.TakeOff, "装备");
    //}
}
/// <summary>
/// Tips按钮，通用
/// </summary>
public class Pv_ButtonVo
{
    public int enumValue;
    public string BtnName;
    public Pv_ButtonVo(int enumValue, string BtnName)
    {
        this.enumValue = enumValue;
        this.BtnName = BtnName;
    }
}
#endregion
public partial class PreviewContainerController
{

    private List<UIButton> btnPool = new List<UIButton>();
    private List<UISprite> linePool = new List<UISprite>();
    int linePoolIndex = 0;
    int btnPoolIndex = 0;
    CompositeDisposable _btnDisposable = new CompositeDisposable();

    float DefaultBgHegiht = 243;
    int buttonRect = 80;
    bool isShowBtn;

    private Vector2 _curSize = new Vector2();
    //当前的Size
    private Vector2 CurSize {
        get
        {
            float width = View.Bg_UISprite.width;
            float height = View.Bg_UISprite.height;
            _curSize.x = width;
            _curSize.y = height;
            return _curSize;
        }
    }
    //屏幕的Size
    private Vector2 screenSize;

    private int BottomHeight
    {
        get
        {
            return isShowBtn ? buttonRect : 0;
        }
    }

    Subject<int> _onButtonClickStream = new Subject<int>();
    public UniRx.IObservable<int> OnButtonClickStream { get { return _onButtonClickStream; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        var root = View.transform.GetComponentInParent<UIRoot>();
        screenSize = new Vector2(root.manualWidth, root.activeHeight);

        linePool.AddRange(View.LinePool_Transform.GetComponentsInChildren<UISprite>(true));
        btnPool.AddRange(View.BtnPool_Transform.GetComponentsInChildren<UIButton>(true));

        this.isShowBtn = false;
        View.LeftBtn_UIButton.gameObject.SetActive(false);
        View.RigthBtn_UIButton.gameObject.SetActive(false);
        var bottomTable = View.BotomAnchor.GetComponent<UITable>();
        if (bottomTable != null)
            bottomTable.Reposition();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        ReclycleMyPool();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
           
    }
    protected override void OnHide()
    {
        base.OnHide();
    }
    private void ReclycleMyPool()
    {
        linePoolIndex = 0;
        linePool.ForEach(x => x.transform.SetParent(View.LinePool_Transform));
        btnPoolIndex = 0;
        btnPool.ForEach(x => x.transform.SetParent(View.BtnPool_Transform));
        _btnDisposable.Clear();
    }
    private void ClearAll()
    {
        DespawnUIList();
        ReclycleMyPool();
    }
    /// <summary>
    /// 单独打开装备提示
    /// </summary>
    /// <param name="dto"></param>
    public void MakeEquipmentTips(Pv_EquipmentVo vo)
    {
        ClearAll();
        var equipmentDto = vo.equipmentDto;
        //头部，顶部
        var headCtrl = AddCachedChild<PreviewHeadController, PreviewHead>(View.Content_UITable.gameObject, PreviewHead.NAME);
        headCtrl.MakeEquipmentSmithShow(equipmentDto,vo.powerChange,vo.headLabel);
        //属性行
        var equipExtraDto = equipmentDto.property.currentProperty;
        AddProperty(equipExtraDto.baseProps);
        AddProperty(equipExtraDto.secondProps);
        AddLine();
        AddProperty(equipExtraDto.extraProps);
        if(equipExtraDto.extraProps.Count > 0)
            AddLine();
        //宝石属性
        var embedVo = vo.embedVo;
        if(embedVo != null && embedVo.embedCount > 0)
        {
            List<string> embedIconList = new List<string>();
            List<string> embedPropertList = new List<string>();
            embedVo.EmbedHoleVoList.ForEachI((x, i) => {
                if (x.embedid == -1)
                    return;
                var item = DataCache.getDtoByCls<GeneralItem>(x.embedid);
                embedIconList.Add(item.icon);
            });
            embedVo.TotalProperty.ForEachI((x, i) => {
                var cbConfig = DataCache.getDtoByCls<CharacterAbility>(x.propId);
                var cbValue = x.propValue;
                string text = string.Format("{0}+{1}", cbConfig.name, cbValue);
                embedPropertList.Add(text);
            });
            AddNameAndIcons("宝石：", embedIconList);
            AddGridAttrView(embedPropertList);
        }

        //纹章属性
        if(equipmentDto.property.medallion != null)
        {
            List<EngraveDto> engravesList = equipmentDto.property.medallion.engraves;
            List<string> effectStrList = new List<string>();
            List<string> iconNameStrList = new List<string>();
            float propsTimes = 1.0f;
            Dictionary<int, float> idToEffect = new Dictionary<int, float>();

            if (!engravesList.IsNullOrEmpty())
            {
                engravesList.ForEach(ItemDto =>
                {
                    var localData = ItemHelper.GetGeneralItemByItemId(ItemDto.itemId);
                    var propsParam = (localData as Props).propsParam as PropsParam_3;
                    //强化铭刻符提升属性总倍数
                    if (propsParam.type == (int)PropsParam_3.EngraveType.STRENGTHEN)
                    {
                        propsTimes *= ItemDto.effect;
                    }
                    else if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY) //相同属性叠加
                    {
                        if (idToEffect.ContainsKey(ItemDto.itemId))
                            idToEffect[ItemDto.itemId] = idToEffect[ItemDto.itemId] + ItemDto.effect;
                        else
                            idToEffect.Add(ItemDto.itemId, ItemDto.effect);
                    }

                    //Icon
                    iconNameStrList.Add(localData.icon);
                });

                AddNameAndIcons("纹章", iconNameStrList);

                idToEffect.ForEach(item =>
                {
                    var localData = ItemHelper.GetGeneralItemByItemId(item.Key);
                    var propsParam = (localData as Props).propsParam as PropsParam_3;
                    effectStrList.Add(DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name + "+" + (item.Value * propsTimes).ToString());
                });

                //effectStrList.ForEach(i =>
                //{
                //    AddSimpleText(i);
                //});
                AddGridAttrView(effectStrList);
            }
            AddLine();
        }

        AddBtn(vo.leftButtonList, vo.rightButton);

        View.Content_UITable.Reposition();
        //设置背景图的长度


        View.Bg_UISprite.height = (int)CaculateHeight() + 40 + BottomHeight;

    }


    /// <summary>
    /// 装备打造前的预览
    /// </summary>
    public void MakePreviewEquipment(Equipment equipment,int quality)
    {
        ClearAll();
        var headCtrl = AddCachedChild<PreviewHeadController, PreviewHead>(View.Content_UITable.gameObject, PreviewHead.NAME);
        headCtrl.MakePreviewEquipment(equipment);

        var AttrList=  EquipmentMainDataMgr.DataMgr.GetEquipmentPropertyRange(equipment.id, quality);

        AttrList.ForEach(x=> {
            var cb = DataCache.getDtoByCls<CharacterAbility>(x.abilityId);
            string res = string.Format("{0}:{1}~{2}", cb.name, x.minValue, x.maxValue);
            AddSimpleText(res);
        });

        AddLine();
        AddSimpleText("随机1~2条附加属性");

        //List<Pv_ButtonVo> leftBtns = new List<Pv_ButtonVo>();
        //leftBtns.Add(PreviewShowHelper.GetPvBtn(PvBtnType.Equip));
        //leftBtns.Add(PreviewShowHelper.GetPvBtn(PvBtnType.Lock));
        //leftBtns.Add(PreviewShowHelper.GetPvBtn(PvBtnType.Split));
        //Pv_ButtonVo rightBtns = PreviewShowHelper.GetPvBtn(PvBtnType.Sell);

        //AddBtn(leftBtns, rightBtns);
        View.Content_UITable.Reposition();
        //设置背景图的长度
        View.Bg_UISprite.height = (int)CaculateHeight() + 40 + BottomHeight;
    }

    /// <summary>
    /// 背包道具的Tips
    /// </summary>
    /// <param name="dto"></param>
    public void MakeBagItemPreview(BagItemDto dto)
    {
        ClearAll();
        for(int i=0;i<15;i++)
            AddSimpleText("测试道具");
        View.Content_UITable.Reposition();
        //设置背景图的长度
        View.Bg_UISprite.height = (int)CaculateHeight() + 40 + BottomHeight;
    }
    #region 位置计算
    public enum CellPos
    {
        Left,
        Right
    }

    /// <summary>
    /// 默认Pivot 在正上方，没有考虑太多~如果以后有需要再拿出去用~~
    /// </summary>
    /// <param name="targetInfo"></param>
    /// <param name="cellpos"></param>
    public void SetPosWithAnchor(Rect targetInfo,CellPos cellpos = CellPos.Left)
    {
        var targetPos = targetInfo.position;
        var tagetLocalPos = View.transform.InverseTransformPoint(targetPos);

        //View.transform.localPosition
        var pos = View.transform.localPosition;
        //4个边界
        //Pivot 在上
        //float curTop = pos.y + curSize.y / 2;
        //float curBottom = pos.y - curSize.y / 2; 
        float curTop = pos.y;
        float curBottom = pos.y - CurSize.y;
        float curLeft = pos.x - CurSize.x / 2;
        float curRight = pos.x + CurSize.x / 2;
        Vector2 newPos = new Vector2();

        //计算高度的偏差
        float targetY = tagetLocalPos.y + targetInfo.size.y / 2;
        curBottom = targetY - CurSize.y;
        if (curBottom > screenSize.y / 2)
        {
            targetY = screenSize.y;
        }
        if (curBottom < -screenSize.y / 2)
        {
            targetY = -screenSize.y;
        }
        //计算左右的位置
        if (cellpos == CellPos.Right)
        {
            float targetX = tagetLocalPos.x + targetInfo.size.x / 2 + CurSize.x / 2;
            curRight = targetX + CurSize.x / 2;
            if(curRight >= screenSize.x / 2)
            {
                targetX = screenSize.x / 2;
            }
            newPos = new Vector2(targetX, targetY);
        }
        else if(cellpos == CellPos.Left)
        {
            float targetX = tagetLocalPos.x - targetInfo.size.x / 2 - CurSize.x / 2;
            curLeft = targetX - CurSize.x / 2;
            if (curLeft <= screenSize.x / 2)
            {
                targetX = -screenSize.x / 2;
            }
            //计算高度的偏差
            newPos = new Vector2(tagetLocalPos.x - targetInfo.size.x / 2 - CurSize.x / 2, targetY);
        }
        



        View.transform.localPosition = newPos;
    }

    /// <summary>
    /// 获取 Table的高度，根据最后一个AddLine的位置进行计算
    /// </summary>
    /// <returns></returns>
    private float CaculateHeight()
    {
        var  height = DefaultBgHegiht;
        if (linePoolIndex > 0)
        {
            var bgLocalPos = View.Bg_UISprite.transform.InverseTransformPoint(linePool[linePoolIndex - 1].transform.position);

            height = linePool[linePoolIndex - 1].height / 2 + bgLocalPos.y;
        }
        return Math.Abs(height);
    }

    public float GetWidth()
    {
        return View.Bg_UISprite.width;
    }
    #endregion

    #region 各种公用组件组件

    private void AddProperty(List<CharacterPropertyDto> dtos)
    {
        dtos.ForEachI((x, i) => {
            var cb = DataCache.getDtoByCls<CharacterAbility>(x.propId);
            var name = cb.name;
            var value = x.propValue;
            string text = string.Format("{0} : {1}", name, (int)value);
            AddSimpleText(text);
        });
    }
   /// <summary>
   /// 加一行字
   /// </summary>
   /// <param name="text"></param>
    private void AddSimpleText(string text)
    {
        var lblCtrl = AddCachedChild<PreviewLabelItemController, PreviewLabelItem>(View.Content_UITable.gameObject, PreviewLabelItem.NAME);
        lblCtrl.UpdateViewData(text);
    }
    /// <summary>
    /// 名字，对应后面图标，用于宝石，纹章
    /// </summary>
    /// <param name="title"></param>
    /// <param name="IconStrList"></param>
    private void AddNameAndIcons(string title, List<string> IconStrList)
    {
        var ctrl = AddCachedChild<PreviewSTLineController, PreviewSTLine>(View.Content_UITable.gameObject, PreviewSTLine.NAME);
        ctrl.InitDataView(title, IconStrList);
    }

    /// <summary>
    /// 增加格子形式的属性~
    /// </summary>
    /// <param name="attrList"></param>
    private void AddGridAttrView(List<string> attrList)
    {
        var ctrl = AddCachedChild<PreviewGridAttrViewController, PreviewGridAttrView>(View.Content_UITable.gameObject, PreviewGridAttrView.NAME);
        ctrl.InitViewData(attrList);
    }
    

    private void AddBtn(List<Pv_ButtonVo> leftButtonList, Pv_ButtonVo rightButton)
    {
        isShowBtn = (leftButtonList!=null && leftButtonList.Count > 0) || rightButton !=null;
        View.LeftButtonAnchor.SetActive(false);
        if(leftButtonList != null)
        {
            //View.LeftBtn_UIButton.gameObject.SetActive(false);
            //如果数量大于1，则变换为更多
            if (leftButtonList.Count > 1)
            {
                SetBtnName(View.LeftBtn_UIButton, "更多");
                _btnDisposable.Add(OnLeftBtn_UIButtonClick.Subscribe(_ =>
                {
                    //打开下面的更多东西的~
                    View.LeftButtonAnchor.SetActive(!View.LeftButtonAnchor.activeSelf);
                }));
                leftButtonList.ForEachI((x, i) =>
                {
                    var btn = btnPool[btnPoolIndex++];
                    btn.transform.SetParent(View.LeftButtonAnchor.transform);
                    //var go = View.LeftButtonAnchor.gameObject.AddChildAndAdjustDepth("PreviewBtn");
                    //var btn = go.GetComponent<UIButton>();
                    var rx = btn.AsObservable();
                    SetBtnName(btn, x.BtnName);
                    _btnDisposable.Add(rx.Subscribe(_ =>
                    {
                        var tempValue = x.enumValue;
                        _onButtonClickStream.OnNext(tempValue);
                    }));
                });
                var leftGrid = View.LeftButtonAnchor.GetComponent<UIGrid>();
                if (leftGrid != null)
                    leftGrid.Reposition();
            }
            else if (leftButtonList.Count == 1)
            {
                SetBtnName(View.LeftBtn_UIButton, leftButtonList[0].BtnName);
                _btnDisposable.Add(OnLeftBtn_UIButtonClick.Subscribe(_ =>
                {
                    _onButtonClickStream.OnNext(leftButtonList[0].enumValue);
                }));
            }
        }
        else
        {
            //View.LeftBtn_UIButton.gameObject.SetActive(true);
        }
            
        

        if (rightButton != null)
        {
            //View.RigthBtn_UIButton.gameObject.SetActive(true);
            SetBtnName(View.RigthBtn_UIButton, rightButton.BtnName);
            _btnDisposable.Add(OnRigthBtn_UIButtonClick.Subscribe(_ =>
            {
                _onButtonClickStream.OnNext(rightButton.enumValue);
            }));
        }
        else
        {
            //View.RigthBtn_UIButton.gameObject.SetActive(false);
        }


        View.RigthBtn_UIButton.gameObject.SetActive(rightButton!=null);
        View.LeftBtn_UIButton.gameObject.SetActive(leftButtonList != null && leftButtonList.Count > 0);

        var bottomTable = View.BotomAnchor.GetComponent<UITable>();
        if (bottomTable != null)
            bottomTable.Reposition();
    }

    private void SetBtnName(UIButton btn, string name)
    {
        var leftLbl = btn.transform.Find("Label");
        if (leftLbl != null)
            leftLbl.GetComponent<UILabel>().text = name;
    }
    /// <summary>
    /// 增加一条线，同时用作 Table 排序后的高度计算
    /// </summary>
    /// <param name="isShow"></param>
    private void AddLine(bool isShow = true)
    {
        var line = linePool[linePoolIndex++];
        line.transform.SetParent(View.Content_UITable.transform);
        line.alpha = isShow ? 1 : 0;
    }
    #endregion    
}


#region 废弃保留
///// <summary>
///// 对按钮的设置
///// </summary>
///// <param name="leftClick"></param>
///// <param name="rightClick"></param>
//public void InitBeforeMakeView(SimpleBtnVo leftClick = null, SimpleBtnVo rightClick = null)
//{
//    this.isShowBtn = leftClick != null || rightClick != null;
//    View.LeftBtn_UIButton.gameObject.SetActive(leftClick != null);
//    View.RigthBtn_UIButton.gameObject.SetActive(rightClick != null);

//    var leftLbl = View.LeftBtn_UIButton.transform.Find("Label");
//    if (leftLbl != null)
//        leftLbl.GetComponent<UILabel>().text = leftClick == null ? string.Empty : leftClick.btnName;
//    var rightLbl = View.RigthBtn_UIButton.transform.Find("Label");
//    if (rightLbl != null)
//        rightLbl.GetComponent<UILabel>().text = rightClick == null ? string.Empty : rightClick.btnName;

//    //===按钮的监听===
//    OnLeftBtn_UIButtonClick.Subscribe(_ =>
//    {
//        if (leftClick != null && leftClick.onClick != null)
//            leftClick.onClick(View.LeftBtn_UIButton.gameObject);
//    });
//    OnRigthBtn_UIButtonClick.Subscribe(_ =>
//    {
//        if (rightClick != null && rightClick.onClick != null)
//            rightClick.onClick(View.RigthBtn_UIButton.gameObject);
//    });


//    var bottomTable = View.BotomAnchor.GetComponent<UITable>();
//    if (bottomTable != null)
//        bottomTable.Reposition();
//}


//public void InitButtonView(List<string> leftButtons, string rightButtons = "")
//{
//    if (leftButtons == null || leftButtons.Count == 0)
//    {
//        this.isShowBtn = false;
//        View.Botom.SetActive(false);
//        goto End;
//    }
//    else
//    {
//        View.Botom.gameObject.SetActive(true);
//    }

//    if (leftButtons.Count > 1)
//    {
//        SetBtnName(View.LeftBtn_UIButton, "更多");

//        OnLeftBtn_UIButtonClick.Subscribe(_ =>
//        {
//            //打开下面的更多东西的~
//            View.LeftButtonAnchor.SetActive(!View.LeftButtonAnchor.activeSelf);
//        });
//        leftButtons.ForEachI((x, i) =>
//        {
//            var go = View.LeftButtonAnchor.gameObject.AddChildAndAdjustDepth("PreviewBtn");
//            var btn = go.GetComponent<UIButton>();
//            var rx = btn.AsObservable();
//            SetBtnName(btn, x);
//            rx.Subscribe(_ =>
//            {
//                var tempstr = x;
//                _onButtonClickStream.OnNext(tempstr);
//            });
//        });

//        View.LeftButtonAnchor.SetActive(false);
//    }
//    else if (leftButtons.Count == 1)
//    {
//        SetBtnName(View.LeftBtn_UIButton, leftButtons[0]);
//        OnLeftBtn_UIButtonClick.Subscribe(_ =>
//        {
//            _onButtonClickStream.OnNext(leftButtons[0]);
//        });
//    }

//    SetBtnName(View.RigthBtn_UIButton, rightButtons);
//    OnRigthBtn_UIButtonClick.Subscribe(_ =>
//    {
//        _onButtonClickStream.OnNext(rightButtons);
//    });
//    this.isShowBtn = true;
//    View.LeftBtn_UIButton.gameObject.SetActive(true);
//    View.RigthBtn_UIButton.gameObject.SetActive(true);

//    //必到之地~
//    End:
//    var bottomTable = View.BotomAnchor.GetComponent<UITable>();
//    if (bottomTable != null)
//        bottomTable.Reposition();
//}
#endregion