// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuizariumViewController.cs
// Author   : xush
// Created  : 12/13/2017 8:36:31 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

public partial interface IQuizariumViewController
{
    UniRx.IObservable<int> GetItemClickHandler { get; }
    UniRx.IObservable<Unit> GetCloseHanlder { get; }
}

public partial class QuizariumViewController
{
    private List<AnswerItemController> _answerItems = new List<AnswerItemController>();

    private Subject<int> _onItemClickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetItemClickHandler { get { return _onItemClickEvt; } }
    private Subject<Unit> _onCloseEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetCloseHanlder { get { return _onCloseEvt; } }  

    private readonly int TotleTime = 60;    //答题时间60s

    private IQuestionData _data;
    private Question _question;
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        for (int i = 0; i < _view.AnswerGrid_UIGrid.transform.childCount; i++)
        {
            var item = _view.AnswerGrid_UIGrid.GetChild(i);
            var controller = AddController<AnswerItemController, AnswerItem>(item.gameObject);
            _answerItems.Add(controller);
            _disposable.Add(controller.GetOnClickHandler.Subscribe(idx =>
            {
                var option = _data.QuizariumData.AnswerIndexQueue.TryGetValue(idx - 1);
                _onItemClickEvt.OnNext(option);
                SelectItem(idx);

                _onCloseEvt.OnNext(new Unit());
            }));
        }
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        _disposable.Add(CloseBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip(string.Format("答题还在继续哦,剩余{0}秒", _data.QuizariumData.GetQuizariumTime));
            _onCloseEvt.OnNext(new Unit());
        }));
    }

    protected override void RemoveCustomEvent ()
    {
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        JSTimer.Instance.CancelCd("Quizarium");
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IQuestionData data)
    {
        _question = DataCache.getDtoByCls<Question>(data.QuizariumData.QuizariumId);
        if (_question == null)
        {
            GameDebuger.LogError(string.Format("Question表找不到{0}", data.QuizariumData.QuizariumId));
            return;
        }

        _data = data;
        _view.TitleLb_UILabel.text = _question.description;
        UpdateGrid(data.QuizariumData.AnswerIndexQueue);
        UpdateSlider(data.QuizariumData.GetQuizariumTime);
        UpdateTime(data.QuizariumData.GetQuizariumTime);
    }

    private void UpdateGrid(IEnumerable<int> list)
    {
        _answerItems.ForEachI((item, idx) =>
        {
            var answer = _question.answers[list.TryGetValue(idx)];
            item.SetItemInfo(idx + 1, answer);
        });
    }

    private void UpdateSlider(int time)
    {
        if (_view == null)
            return;

        _view.TimeLb_UILabel.text = 
            time >= 0 ? string.Format("{0}秒",time) : "0秒";

        _view.Slider_UISlider.value = (float) time/(float) TotleTime;
    }

    private void UpdateTime(int time)
    {
        JSTimer.Instance.SetupCoolDown("Quizarium", time + 1, e =>
        {
            time -= 1;
            UpdateSlider(time);
        }, () =>
        {
            UpdateSlider(time);
            _onCloseEvt.OnNext(new Unit());
            TipManager.AddTip("答题时间已到");
        }, 1f);
    }

    private void SelectItem(int idx)
    {
        _answerItems.ForEachI((item, i) =>
        {
            if (i == idx)
                item.Select();
        });
    }
}
