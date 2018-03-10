// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuestionViewController.cs
// Author   : xush
// Created  : 12/12/2017 11:38:32 AM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

public partial interface IQuestionViewController
{
    UniRx.IObservable<int> GetItemClickHandler { get; }
}

public partial class QuestionViewController
{
    private List<AnswerItemController>  _txtAnswerList = new List<AnswerItemController>();
    private List<AnswerItemController> _textureAnswerList = new List<AnswerItemController>();
    private QuestionType _type;
    private string _wealthType;
    private int _boxState;
    private string[] _texturelist = {"Answer_Box_Unopenable", "Answer_Box_normal", "Answer_Box_open"};
    private Subject<int> _onItemClick = new Subject<int>();
    public UniRx.IObservable<int> GetItemClickHandler { get { return _onItemClick; } }
    private Question _curQuestion;
    private IQuestionData _data;
    private int _rightOption;
    private bool _clickstate;   //记录是否已经选择答案,避免多选
    private enum QuestionType
    {
        txt = 1,
        texture = 2
    }

	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        SetQuestionType();
        UIHelper.SetUITexture(_view.Texture_UITexture, "ShopNPC_207", false);
        for (int i = 0; i < _view.TxtGrid_UIGrid.transform.childCount; i++)
        {
            var item = _view.TxtGrid_UIGrid.GetChild(i);
            var ctrl = AddController<AnswerItemController, AnswerItem>(item.gameObject);
            _txtAnswerList.Add(ctrl);
            _disposable.Add(ctrl.GetOnClickHandler.Subscribe(idx =>
            {
                if (_clickstate)
                {
                    CheckAnswerIsRight(idx);
                    var option = _data.QuestionDto.curQuestion.answerIndexQueue[idx - 1];
                    JSTimer.Instance.SetupCoolDown("AnswerQuestion", 0.5f, null, () => { _onItemClick.OnNext(option); });
                }
                _clickstate = false;
            }));
        }

        for (int i = 0; i < _view.TextureGrid_UIGrid.transform.childCount; i++)
        {
            var item = _view.TextureGrid_UIGrid.GetChild(i);
            var ctrl = AddController<AnswerItemController, AnswerItem>(item.gameObject);
            _textureAnswerList.Add(ctrl);
            _disposable.Add(ctrl.GetOnClickHandler.Subscribe(idx =>
            {
                if (_clickstate)
                {
                    CheckAnswerIsRight(idx);
                    var option = _data.QuestionDto.curQuestion.answerIndexQueue[idx - 1];
                    JSTimer.Instance.SetupCoolDown("AnswerQuestion", 0.5f, null, () => { _onItemClick.OnNext(option); });
                }
                _clickstate = false;
            }));
        }

        _view.HelpBtn_UIButton.gameObject.SetActive(false); //没有公会系统暂时屏蔽    --xush
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd("AnswerQuestion");
        _txtAnswerList = null;
        _textureAnswerList = null;
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IQuestionData data)
    {
        _data = data;
        _boxState = data.QuestionDto.giftBoxState;

        QuestionFinish(data.IsEnd);
        if (data.IsEnd)
        {
            _view.ExpLb_UILabel.text = string.Format("获得经验: {0}", data.GetExp);
            _view.MenoyLb_UILabel.text = string.Format("获得{0}: {1}", _wealthType, data.GetMoney);
            _view.PrecisionLb_UILabel.text = string.Format("准确率: [7EE830]{0}/{1}[-]题", data.RightNum, data.TotleNum);
            UpdateRewardBox();
            return;
        }

        _curQuestion = DataCache.getDtoByCls<Question>(data.QuestionId);
        if (_curQuestion == null)
        {
            GameDebuger.LogError(string.Format("question表找不到{0}", data.QuestionId));
            return;
        }

        _rightOption = data.QuestionDto.curQuestion.answerIndexQueue.FindIndex(d => d == _curQuestion.correctIndex);
        UpdateGrid(data.QuestionDto.curQuestion.answerIndexQueue);
        UpdateQuestion(data);
        UpdateRewardBox();
    }

    private void UpdateQuestion(IQuestionData data)
    {
        _view.PrecisionLb_UILabel.text = string.Format("准确率: [7EE830]{0}/{1}[-]题", data.RightNum, data.TotleNum);
        _view.ExpLb_UILabel.text = string.Format("获得经验: {0}", data.GetExp);
        _view.MenoyLb_UILabel.text = string.Format("获得{0}: {1}", _wealthType, data.GetMoney);
        _view.QuestionNumLb_UILabel.text = string.Format("第{0}题/共10题", data.TotleNum + 1);
        _view.RewardBoxLb_UILabel.text = string.Format("答对{0}题后可领取", 6);
        _view.TitleLb_UILabel.text = _curQuestion.description;
    }

    private void UpdateGrid(List<int> list)
    {
        switch (_type)
        {
             case QuestionType.txt:
                int idx = 0;
                list.ForEach(i =>
                {
                    _clickstate = true;
                    _txtAnswerList[idx].SetItemInfo(idx + 1, _curQuestion.answers[i]);
                    idx += 1;
                });
                break;
            case QuestionType.texture:
                int idnex = 0;
                list.ForEach(i =>
                {
                    _clickstate = true;
                    _textureAnswerList[idnex].SetItemInfo(idnex + 1, _curQuestion.answers[i]);
                    idnex += 1;
                });
                break;
        }
    }

    private void SetQuestionType()
    {
        var date = SystemTimeManager.Instance.GetCHDayOfWeek();
        if (date == 7) return;  //7代表周日

        _type = date%2 == 1 ? QuestionType.txt : QuestionType.texture;
        _view.TxtGrid_UIGrid.gameObject.SetActive(_type == QuestionType.txt);
        _view.TextureGrid_UIGrid.gameObject.SetActive(_type == QuestionType.texture);
        var questiontype = DataCache.getDtoByCls<AppDto.QuestionType>((int) _type);
        if (questiontype == null)
        {
            GameDebuger.LogError(string.Format("questionType表找不到{0},请检查", (int) _type));
            return;
        }
        var wealthId = questiontype.wealth.Split(':')[0];
        _wealthType = DataCache.getDtoByCls<GeneralItem>(StringHelper.ToInt(wealthId)).name;
    }

    private void CheckAnswerIsRight(int idx)
    {
        var option = _data.QuestionDto.curQuestion.answerIndexQueue[idx - 1];
        switch (_type)
        {
            case QuestionType.txt:
                _txtAnswerList[idx - 1].IsRight(option == _curQuestion.correctIndex);
                if (_curQuestion.correctIndex != option)
                    _txtAnswerList[_rightOption].IsRight(true);
                break;
            case QuestionType.texture:
                _textureAnswerList[idx - 1].IsRight(option == _curQuestion.correctIndex);
                if (_curQuestion.correctIndex != option)
                    _textureAnswerList[_rightOption].IsRight(true);
                break;
        }
    }

    private void UpdateRewardBox()
    {
        string txt = _texturelist[_boxState];
        UIHelper.SetUITexture(_view.RewardBoxBtn_UITexture, txt);
    }

    private void QuestionFinish(bool b)
    {
        _view.RightGroup.SetActive(!b);
        _view.FinishGroup.SetActive(b);
    }
}
