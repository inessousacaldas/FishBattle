// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFavorableViewController.cs
// Author   : xush
// Created  : 9/15/2017 5:08:08 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDto;
using AssetPipeline;
using MyGameScripts.Gameplay.Player;
using UniRx;
using UnityEngine;
using UnityEngine.Sprites;

public partial interface ICrewFavorableViewController: ICloseView
{
    void UpdateCrewInfo(CrewInfoDto crewData);
    UniRx.IObservable<int> GetRewardItemClick { get; }
    UniRx.IObservable<Unit> GetModelClick { get; }
    void ShowSecondPanel(bool b);
    void ShowRecordPanel();
    void ShowHistoryPanel();
    void ShowRewardList();
    void RewardPanelTween(bool b);
    void RecordPanelTween(bool b);
    void HistoryPanelTween(bool b);

    GameObject GetLeftPageBtn { get; }
    GameObject GetRightPageBtn { get; }
}

public partial class CrewFavorableViewController
{
    private ModelDisplayController _modelDisplayer = new ModelDisplayController();

    private List<CrewRewardItemController> _rewardItemList = new List<CrewRewardItemController>();
    private List<CrewFavorLabelController> _favorLbList = new List<CrewFavorLabelController>(); 
    private List<CrewFavor> _crewFavors = new List<CrewFavor>();  

    private Subject<int> _rewardItemClick = new Subject<int>(); 
    public UniRx.IObservable<int> GetRewardItemClick { get { return _rewardItemClick; } } 

    private Subject<Unit> _modelClickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetModelClick { get { return _modelClickEvt; } }

    private List<string> _contents = new List<string>()
    {
        "", "伙伴传记1", "表演动作", "伙伴跟随", "自定义喊话", "解锁皮肤功能",
        "伙伴传记2", "伙伴名字前缀", "伙伴传记3", "品质框样式", "自定义名字前缀"
    };

    private int _favorableLv;
    private CrewInfoDto _crewData;
    private Crew _crew;
    private List<CrewRecord> _crewRecordList;
    private bool _isFrist;  //第一次进入界面
    private bool _isShowModel;    //显示原画
    private bool _isOpenSecond;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        _crewFavors = DataCache.getArrayByCls<CrewFavor>();
        InitModel();
        InitRewardList();
        InitFavorLbList();
        _view.Texture_TweenPosition.Play(false);
        _view.ModelAnchor_TweenPosition.Play(false);
        _view.FavorableSlider_TweenPosition.Play(false);
        _view.Texture_TweenRotation.Play(false);
        RewardPanelTween(false);
        RecordPanelTween(false);
        HistoryPanelTween(false);
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        _disposable.Add(_modelDisplayer.ClickEvt.Subscribe(_ =>
        {
            ShowDialog();
            _modelClickEvt.OnNext(new Unit());
        }));
	    _disposable.Add(TipBtn_UIButtonEvt.Subscribe(_ =>
	    {
	        ProxyTips.OpenTextTips(17, new Vector3(-309, 275, 0));
	    }));
        _disposable.Add(CloseRecordBtn_UIButtonEvt.Subscribe(_ =>
        {
            RecordPanelTween(false);
            ShowSecondPanel(false);
        }));
        _disposable.Add(CloseHistoryBtn_UIButtonEvt.Subscribe(_ =>
        {
            HistoryPanelTween(false);
            ShowSecondPanel(false);
        }));
        _disposable.Add(CloseRewardBtn_UIButtonEvt.Subscribe(_ =>
        {
            RewardPanelTween(false);
            ShowSecondPanel(false);
        }));
        //_disposable.Add(ModelBtn_UIButtonEvt.Subscribe(_ => ShowModelGroup(true)));
        //_disposable.Add(TextureBtn_UIButtonEvt.Subscribe(_=>ShowModelGroup(false)));
        _disposable.Add(FavorableSlider_UIButtonEvt.Subscribe(_ =>
        {
            _view.SliderValue_UILabel.gameObject.SetActive(true);
            HideSliderValue();
        }));
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd("HideSliderValue");
        JSTimer.Instance.CancelCd("CrewShowDialog");
        _disposable = _disposable.CloseOnceNull();
        _modelDisplayer.CleanUpModel();
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
        _crewRecordList = DataCache.getArrayByCls<CrewRecord>();
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ICrewViewData data)
    {
        UpdateCrewInfo(data.GetSelfCrew().TryGetValue(data.CrewFavorableData.GetCurFavorIdx));
        ShowDialog();
        if (_isFrist)
            UpdateSecondPanel();

        _view.LastBtn_UIButton.gameObject.SetActive(data.CrewFavorableData.GetCurFavorIdx > 0);
        _view.NextBtn_UIButton.gameObject.SetActive(data.CrewFavorableData.GetCurFavorIdx < data.GetSelfCrew().Count() - 1);
        _isFrist = true;
    }

    private void UpdateSecondPanel()
    {
        ShowHistoryPanel();
        ShowRecordPanel();
        ShowRewardList();
    }

    public void UpdateCrewInfo(CrewInfoDto crewData)
    {
        _crewData = crewData;
        _crew = DataCache.getDtoByCls<GeneralCharactor>(crewData.crewId) as Crew;
        if (_crew == null)
        {
            GameDebuger.LogError(string.Format("检查crew表中是否包含{0}", crewData.crewId));
            return;
        }
        _view.NameLb_UILabel.text = _crew.name;
        SetFavorLv(crewData.favor);
        UpdateModel(_crew.modelId);
        UpdateSecondPanel();
        UpdateTexture(_crew.commonTexture);
    }

    private void SetFavorLv(int favor)
    {
        _crewFavors.ForEach(d =>
        {
            if (favor >= d.need)
                _favorableLv = d.id;
        });
        _view.LvLb_UILabel.text = string.Format("Lv:{0}", _favorableLv);
        if (_favorableLv + 1 >= _crewFavors.Count)
        {
            _view.FavorableSlider_UISprite.fillAmount = (float)favor / (float)_crewFavors[_crewFavors.Count - 1].need;
            _view.SliderValue_UILabel.text = string.Format("{0}/{1}", favor, _crewFavors[_crewFavors.Count - 1].need);
        }
        else
        {
            _view.FavorableSlider_UISprite.fillAmount = (float)favor / (float)_crewFavors[_favorableLv + 1].need;
            _view.SliderValue_UILabel.text = string.Format("{0}/{1}", favor, _crewFavors[_favorableLv + 1].need);
        }
    }

    private void HideSliderValue()
    {
        if (_view.SliderValue_UILabel.gameObject.activeSelf)
        {
            JSTimer.Instance.SetupCoolDown("HideSliderValue", 2f, null, () =>
            {
                if(_view != null)
                    _view.SliderValue_UILabel.gameObject.SetActive(false);
            });
        }
    }

    public void ShowSecondPanel(bool b)
    {
        _isOpenSecond = b;
        SetModelTweenParamX();
        _view.ModelAnchor_TweenPosition.Play(b);
        _view.Texture_TweenPosition.Play(b);
        _view.FavorableSlider_TweenPosition.Play(b);
        _view.BtnGrid.gameObject.SetActive(!b);
        _view.NameLb_UILabel.gameObject.SetActive(!b);

        _view.ModelBtn_UIButton.transform.localPosition = b ? new Vector3(-510, -282, 0) : new Vector3(521, -282, 0);
        _view.TextureBtn_UIButton.transform.localPosition = b ? new Vector3(-510, -282, 0) : new Vector3(521, -282, 0);
        //_view.Dialog.gameObject.SetActive(!b);
        //_view.DialogUnder_UISprite.gameObject.SetActive(b);
        //_view.Dialog.transform.localPosition = b ? new Vector3(-276, 195, 0) : new Vector3(21, 195, 0);
    }

    private void SetModelTweenParamY()
    {
        var x = _view.ModelAnchor_TweenPosition.transform.localPosition.x;
        _view.ModelAnchor_TweenPosition.from = new Vector3(x, -500, 0);
        _view.ModelAnchor_TweenPosition.to = new Vector3(x, 0, 0);
    }

    private void SetModelTweenParamX()
    {
        var y = _view.ModelAnchor_TweenPosition.transform.localPosition.y;
        _view.ModelAnchor_TweenPosition.from = new Vector3(238, y, 0);
        _view.ModelAnchor_TweenPosition.to = new Vector3(-134, y, 0);
    }

    private string GetLockContentByLv(int lv)
    {
        StringBuilder sb = new StringBuilder();
        var contentid = _crewFavors.TryGetValue(lv - 1).unlockContentIds;
        contentid.ForEachI((d, idx) =>
        {
            if (idx == contentid.Count - 1)
                sb.Append(_contents[d]);
            else
                sb.AppendLine(_contents[d]);
        });
        return sb.ToString();
    }

    public void UpdateModel(int modelId)
    {
        _modelDisplayer.SetupModel(InitModelStyleInfo(modelId));
        _modelDisplayer.SetModelScale(1f);
        _modelDisplayer.SetModelOffset(-0.15f);
    }

    private void InitFavorLbList()
    {
        _crewFavors.ForEachI((d, idx) =>
        {
            var item = AddChild<CrewFavorLabelController, CrewFavorLabel>(_view.HistoryTable_UITable.gameObject,
                CrewFavorLabel.NAME);

            _favorLbList.Add(item);
        });
    }

    private ModelStyleInfo InitModelStyleInfo(int id)
    {
        ModelStyleInfo model = new ModelStyleInfo();
        model.defaultModelId = id;
        return model;
    }

    private void ShowDialog()
    {
        JSTimer.Instance.SetupCoolDown("CrewShowDialog", 2f, null, () =>
        {
            if (_view != null)
            {
                _view.Dialog.gameObject.SetActive(false);
                _view.DialogUnder_UISprite.gameObject.SetActive(false);
                var crewRecord = _crewRecordList.Find(d => d.id == _crewData.crewId);
                if (crewRecord.dialogContent.Count == 0)
                {
                    _view.DialogLb_UILabel.text = "好高兴~~";
                    _view.DialogUnderLb_UILabel.text = "好高兴~~";
                }
                else
                {
                    var idx = UnityEngine.Random.Range(0, crewRecord.dialogContent.Count);
                    _view.DialogLb_UILabel.text = crewRecord.dialogContent[idx];
                    _view.DialogUnderLb_UILabel.text = crewRecord.dialogContent[idx];
                }
                _view.Dialog.UpdateAnchors();
                _view.DialogUnder_UISprite.UpdateAnchors();
            }
        });

        _view.Dialog.gameObject.SetActive(!_isOpenSecond);
        _view.DialogUnder_UISprite.gameObject.SetActive(_isOpenSecond);
    }

    private void InitModel()
    {
        _modelDisplayer = AddChild<ModelDisplayController, ModelDisplayUIComponent>(
            View.ModelAnchor
            , ModelDisplayUIComponent.NAME);

        _modelDisplayer.Init(300, 300);
        _modelDisplayer.SetBoxColliderEnabled(true);
    }

    private void UpdateTexture(string txt)
    {
        UIHelper.SetUITexture(_view.Texture_UITexture, txt, true);
    }

    private void InitRewardList()
    {
        //暂时写死4个
        for (int i = 0; i < 4; i++)
        {
            var item = AddChild<CrewRewardItemController, CrewRewardItem>(
                _view.RewardGrid_UIGrid.gameObject,
                CrewRewardItem.NAME);

            _rewardItemList.Add(item);
            _disposable.Add(item.GetClickHandler.Subscribe(tid =>
            {
                var bagItem = BackpackDataMgr.DataMgr.GetItemByItemID(tid);
                if (bagItem == null)
                {
                    var p = DataCache.getDtoByCls<GeneralItem>(tid);
                    TipManager.AddTip(string.Format("{0}数量不足!", p.name));
                    return;
                }
                _rewardItemClick.OnNext(tid);
            }));
            _disposable.Add(item.GetIconHandler.Subscribe(data =>
            {
                var general = DataCache.getDtoByCls<GeneralItem>(data.GetItemId);
                var controller = ProxyTips.OpenGeneralItemTips(general);
                controller.SetTipsPosition(new Vector3(-204, 178 - data.GetItemIdx*108, 0));
            }));
        }
    }

    public void ShowRewardList()
    {
        if (_crew == null) return;

        var itemID = _crew.giftIds.Split(',');
        _rewardItemList.ForEachI((item, idx) =>
        {
            var id = StringHelper.ToInt(itemID[idx].Split(':')[0]);
            item.UpdateItem(id, StringHelper.ToInt(itemID[idx].Split(':')[1]) == 1, idx);    //1代表偏好
        });

        _view.RewardGrid_UIGrid.Reposition();
        _view.RewardScrollView_UIScrollView.ResetPosition();
        
    }

    public void RewardPanelTween(bool b)
    {
        _view.RewardPanel_TweenPosition.Play(b);
    }

    public void ShowRecordPanel()
    {
        if (_crew == null) return;

        var crewRecord = _crewRecordList.Find(d => d.id == _crewData.crewId);
        _view.CrewNameLb_UILabel.text = crewRecord.name;
        _view.SexLb_UILabel.text = crewRecord.gender;
        _view.ProfessionLb_UILabel.text = crewRecord.occupation;
        _view.FamilyLb_UILabel.text = crewRecord.birth;
        _view.AgeLb_UILabel.text = crewRecord.age.ToString();
        _view.RecordDescLb_UILabel.text = "";
        Crew crew = DataCache.getDtoByCls<GeneralCharactor>(_crew.id) as Crew;
        UIHelper.SetPetIcon(_view.IconSprite_UISprite, crew == null ? "" : crew.icon);
        var time = DateUtil.UnixTimeStampToDateTime(_crewData.enlistTimeInMillis);
        StringBuilder sb = new StringBuilder();
        var txt = string.Format("◆结交时间:{0}({1}天)", time.ToString("yyyy年MM月dd日"), _crewData.cooperateDay);
        sb.AppendLine("备注:");
        sb.AppendLine(txt);
        crewRecord.introduction.Split(';').ForEach(t =>
        {
            sb.AppendLine(t);
        });
        _view.RecordDescLb_UILabel.text = sb.ToString();
    }

    public void RecordPanelTween(bool b)
    {
        _view.RecordPanel_TweenPosition.Play(b);
    }

    public void ShowHistoryPanel()
    {
        if (_crew == null) return;

        var gradelist = _crew.favorPropertys.Split(',');
        _favorLbList.ForEachI((item, idx) =>
        {
            if (idx < gradelist.Length)
            {
                item.gameObject.SetActive(true);
                var grade = StringHelper.ToInt(gradelist[idx].Split('_')[0]);
                StringBuilder sb = new StringBuilder();
                var list = gradelist[idx].Split('_')[1].Split('|');
                list.ForEachI((t, index) =>
                {
                    var id = t.Split(':')[0];
                    var num = t.Split(':')[1];
                    if (index == list.Length - 1)
                        sb.Append(string.Format("{0}+{1}", GlobalAttr.GetAttrName(StringHelper.ToInt(id)), num));
                    else
                        sb.AppendLine(string.Format("{0}+{1}", GlobalAttr.GetAttrName(StringHelper.ToInt(id)), num));
                });

                if(_favorableLv >= grade)
                    item.SetLabelInfo(grade, sb.ToString(), GetLockContentByLv(grade), ColorConstantV3.Color_Green_Str);
                else
                    item.SetLabelInfo(grade, sb.ToString(), GetLockContentByLv(grade));
            }
            else
                item.gameObject.SetActive(false);
        });
        _view.HistoryTable_UITable.Reposition();
        _view.HistoryScrollView_UIScrollView.ResetPosition();
    }

    public void HistoryPanelTween(bool b)
    {
        _view.HistoryPanel_TweenPosition.Play(b);
    }

    public void ShowModelGroup(bool b)
    {
        _isShowModel = b;
        SetModelTweenParamY();
        _view.ModelAnchor_TweenPosition.Play(b);
        _view.Texture_TweenRotation.Play(b);
        _view.TextureBtn_UIButton.gameObject.SetActive(b);
        _view.ModelBtn_UIButton.gameObject.SetActive(!b);
    }

    public GameObject GetLeftPageBtn { get { return _view.LastBtn_UIButton.gameObject; } }
    public GameObject GetRightPageBtn { get { return _view.NextBtn_UIButton.gameObject; } }
}
