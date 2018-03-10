// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GrooveViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public partial class GrooveViewController
{
    private List<MedallionGrooveRuneItemController> _runeItemList = new List<MedallionGrooveRuneItemController>();

    private List<EquipmentEmbedHoleController> holeCtrls = new List<EquipmentEmbedHoleController>();
    private List<List<Vector3>> _transforList = new List<List<Vector3>>();

    private static int minHoleType = 2;

    private static int maxHoleType = 5;
    private CompositeDisposable mydisposable = new CompositeDisposable();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        //icons=坐标初始化
        for(int i=minHoleType; i<maxHoleType; i++)
        {
            List<Vector3> list = new List<Vector3>();
            var name = string.Format("Type_{0}", i);
            var typeItem = transform.Find(name);
            if (typeItem == null)
                continue;

            for(int j=1;j<=i; j++)
            {
                var itemCase = typeItem.transform.Find(string.Format("Item_{0}", j));
                if (itemCase == null)
                    continue;

                list.Add(itemCase.transform.position);
            }

            _transforList.Add(list);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }
    IEngraveData data=null;
    public void UpdateView(IEngraveData data)
    {
        this.data = data;
        if (data.CurSelMedallionId == 0)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);

        MedallionDto itemDto = data.SelMedallionData.extra as MedallionDto;
        MedallionProps appItem = data.SelMedallionData.item as MedallionProps;
        List<EngraveDto> _engravesList = itemDto.engraves;

        #region 纹章镶嵌图type
        UnityEngine.Transform curType = null;
        for (int i = minHoleType; i <= maxHoleType; i++)
        {
            var name = string.Format("Type_{0}", i);
            var typeItem = transform.Find(name);
            if (typeItem == null)
                continue;

            if (i == appItem.hole)
            {
                curType = typeItem;
                typeItem.gameObject.SetActive(true);
            }
            else
                typeItem.gameObject.SetActive(false);
        }

        if (curType == null)
            return;

        #endregion

        #region 纹章已镶嵌铭刻符icon
        int dif = appItem.hole - holeCtrls.Count;
        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
            {
                var ctrl = AddChild<EquipmentEmbedHoleController, EquipmentEmbedHole>(this.gameObject, EquipmentEmbedHole.NAME);
                holeCtrls.Add(ctrl);
                int tempIdx = i;
                mydisposable.Add(ctrl.OnBg_UIButtonClick.Subscribe(_ => {
                    Debug.Log("tempIdx:" + tempIdx);
                    ClickHole(tempIdx);
                }));
            }
        }
        else if (dif < 0)
        {
            for (int i = 0; i < Math.Abs(dif); i++)
            {
                holeCtrls[holeCtrls.Count - i - 1].Hide();
            }
        }
        for(int i=0;i< appItem.hole;i++)
        {
            var ctrl = holeCtrls[i];
            if (_engravesList.Count > i)
            {
                var itemEngrave = _engravesList[i];
                
                
                ctrl.UpdateView(itemEngrave);
            }
            else
            {
                ctrl.UpdateEmpty();
            }
            ctrl.Show();
            ctrl.transform.position = _transforList[appItem.hole - minHoleType][i];
        }
        //_engravesList.ForEachI((itemEngrave, index) =>
        //{
        //    var ctrl = holeCtrls[index];
        //    ctrl.Show();
        //    ctrl.UpdateView(itemEngrave);
        //    ctrl.transform.position = _transforList[appItem.hole-minHoleType][index];
        //});
        #endregion
    }
    private void ClickHole(int idx)
    {
        if (data == null)
            return;
        MedallionDto itemDto = data.SelMedallionData.extra as MedallionDto;
        MedallionProps appItem = data.SelMedallionData.item as MedallionProps;
        List<EngraveDto> _engravesList = itemDto.engraves;
        if (_engravesList.Count <= idx)
            return;
        var tData = _engravesList[idx];
        var itemData = ItemHelper.GetGeneralItemByItemId(tData.itemId);

        var localData = ItemHelper.GetGeneralItemByItemId(tData.itemId);
        var propsParam = (localData as Props).propsParam as PropsParam_3;
        string titleStr = string.Format("{0}\n", localData.name.WrapColor(ColorConstantV3.Color_Purple));
        string detailStr = "";
        if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY)
            detailStr = string.Format("{0}+{1}", DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name, tData.effect).WrapColor(ColorConstantV3.Color_Green);
        else
            detailStr = string.Format("当前纹章属性*{0}", (int)tData.effect).WrapColor(ColorConstantV3.Color_Green);

        var _propsStr = titleStr + detailStr;
        TipManager.AddTopTip(_propsStr);
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

}
