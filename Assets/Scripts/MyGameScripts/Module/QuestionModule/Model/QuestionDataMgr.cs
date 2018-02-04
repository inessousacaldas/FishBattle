// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 12/12/2017 11:40:43 AM
// **********************************************************************

using System;
using AppDto;
using UniRx;

public sealed partial class QuestionDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<QuestionOpenNotify>(QuestionOpen));
        _disposable.Add(NotifyListenerRegister.RegistListener<QuestionClosureNotify>(QuestionClose));
        _disposable.Add(NotifyListenerRegister.RegistListener<QuestionEntityNotify>(QuizariumNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<AfterAnswerNotify>(QuizariumAnswerNotify));
    }
    
    public void OnDispose(){
            
    }

    private void QuestionOpen(QuestionOpenNotify notify)
    {
        var controller = ProxyBaseWinModule.Open();
        var title = "每日答题";
        var txt = "每日答题开始了,是否前往参加?";
        BaseTipData data = BaseTipData.Create(title, txt, 0, () =>
        {
            FunctionOpen functionOpen = DataCache.getDtoByCls<FunctionOpen>((int)FunctionOpen.FunctionOpenEnum.FUN_47);
            if(ModelManager.Player.GetPlayerLevel() < functionOpen.grade)
                TipManager.AddTip(string.Format("{0}级开启每日问答", functionOpen.grade));
            else
                ProxyQuestion.OpenQuestionView();
        }, null, "否","是");
        DataMgr._data.IsEnd = false;
        DataMgr._data.SetQuestionType(notify.questionType);
        controller.InitView(data);
    }

    private void QuestionClose(QuestionClosureNotify notify)
    {
        DataMgr._data.IsEnd = true;
        FireData();
    }

    private void QuizariumNotify(QuestionEntityNotify notify)
    {
        var limitTime = DataCache.getDtoByCls<QuestionType>((int)AppDto.QuestionType.QuestionTypeEnum.TYPE_3).limitTime;
        var time = notify.outTime + limitTime - SystemTimeManager.Instance.GetUTCTimeStamp();
        DataMgr._data.SetQuizariumTime((int)(time / 1000));
        DataMgr._data.QuizariumId = notify.id;
        DataMgr._data.SetAnswerIndexQueue(notify.answerIndexQueue);
        FunctionOpen functionOpen = DataCache.getDtoByCls<FunctionOpen>((int)FunctionOpen.FunctionOpenEnum.FUN_73);
        DataMgr._data.QuizariumState = ModelManager.Player.GetPlayerLevel() >= functionOpen.grade;
        FireData();
    }

    private void QuizariumAnswerNotify(AfterAnswerNotify notify)
    {
        var question = DataCache.getDtoByCls<Question>(notify.questionId);
        var rightOption = question.answers[notify.correctIndex].Split('|')[0];
        if (!notify.end)
            TipManager.AddTip(string.Format("本次知识竞答答案为[7EE830]【{0}】[-],请留意下次知识竞答时间",rightOption));
        else
            TipManager.AddTip(string.Format("本次知识竞答答案为[7EE830]【{0}】[-],今日知识竞答已结束", rightOption));

    }
}
