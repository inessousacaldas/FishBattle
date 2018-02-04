// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerLvViewController.cs
// Author   : xjd
// Created  : 11/15/2017 11:40:55 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial interface IBracerLvViewController
{

}
public partial class BracerLvViewController    {

    public static IBracerLvViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IBracerLvViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IBracerLvViewController;
            
        return controller;        
    }

    private List<BracerLvItemController> _bracerLvCtrlList = new List<BracerLvItemController>();
    private Dictionary<int, BracerGrade> _bracerLvGradeDic = DataCache.getDicByCls<BracerGrade>();
    private Dictionary<int, CharacterAbility> _characterDic = DataCache.getDicByCls<CharacterAbility>();
    private List<UILabel> _propsNameList = new List<UILabel>();
    private List<UILabel> _propsValueList = new List<UILabel>();
    private int _choseId = 0;
    private CompositeDisposable _disposable = null;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();

        _propsNameList.Add(View.Info_1_UILabel);
        _propsNameList.Add(View.Info_2_UILabel);
        _propsNameList.Add(View.Info_3_UILabel);
        _propsNameList.Add(View.Info_4_UILabel);
        _propsNameList.Add(View.Info_5_UILabel);
        _propsNameList.Add(View.Info_6_UILabel);

        _propsValueList.Add(View.Num_1_UILabel);
        _propsValueList.Add(View.Num_2_UILabel);
        _propsValueList.Add(View.Num_3_UILabel);
        _propsValueList.Add(View.Num_4_UILabel);
        _propsValueList.Add(View.Num_5_UILabel);
        _propsValueList.Add(View.Num_6_UILabel);

        _choseId = ModelManager.Player.GetBracerGrade;
        if (_choseId == 0)
            _choseId = 1;
        UpdateView();

    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            UIModuleManager.Instance.CloseModule(BracerLvView.NAME);
        });
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;

        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    public void UpdateView()
    {
        _disposable.Clear();
        var itemCount = 0;
        _bracerLvCtrlList.GetElememtsByRange(itemCount, -1).ForEach(s => 
        {
            s.Hide();
            s.SetIsGrey(false);
        });

        _bracerLvGradeDic.ForEachI((item, index) =>
        {
            if(item.Value.id > 0)
            {
                var itemCtrl = AddLvItemIfNotExist(index);
                itemCtrl.UpdateView(item.Value);
                itemCtrl.Show();
                itemCtrl.SetIsChose(item.Value.id == _choseId);
                if (item.Value.id > ModelManager.Player.GetBracerGrade)
                    itemCtrl.SetIsGrey(true);

                _disposable.Add(itemCtrl.OnClickItemStream.Subscribe(id =>
                {
                    _choseId = id;
                    UpdateView();
                }));
            }
        });

        View.Grid_UIGrid.Reposition();

        //左侧属性 读下一等级
        if (!_bracerLvGradeDic.ContainsKey(_choseId))
            return;

        View.Icon_UISprite.spriteName = _bracerLvGradeDic[_choseId].icon;
        itemCount = 0;
        _propsNameList.GetElememtsByRange(itemCount, -1).ForEach(s => s.enabled=false);
        _propsValueList.GetElememtsByRange(itemCount, -1).ForEach(s => s.enabled = false);
        _bracerLvGradeDic[_choseId].attrId.ForEachI((id, idx) =>
        {
            if (_characterDic.ContainsKey(id))
            {
                _propsNameList[idx].text = _characterDic[id].name;
                _propsValueList[idx].text = string.Format("+{0}", _bracerLvGradeDic[_choseId].attrAdd[idx]);
                _propsNameList[idx].enabled = true;
                _propsValueList[idx].enabled = true;
            } 
        });

        View.LvName_UILabel.text = _bracerLvGradeDic[_choseId].name;
    }

    private BracerLvItemController AddLvItemIfNotExist(int idx)
    {
        BracerLvItemController ctrl = null;
        _bracerLvCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<BracerLvItemController, BracerLvItem>(View.Grid_UIGrid.gameObject, BracerLvItem.NAME);
            _bracerLvCtrlList.Add(ctrl);
        }

        return ctrl;
    }
}
