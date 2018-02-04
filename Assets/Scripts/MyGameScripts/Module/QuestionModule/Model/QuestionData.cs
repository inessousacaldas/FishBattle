// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 12/12/2017 11:40:43 AM
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UnityEngine;

public interface IQuestionData
{
    long GetExp { get; }
    long GetMoney { get; }
    int QuestionId { get; }
    int RightNum { get; }
    int TotleNum { get; }
    IQuizariumData QuizariumData { get; }
    QuestionType.QuestionTypeEnum GetQuestionType { get; }
    PlayerQuestionDto QuestionDto { get; }
    bool IsEnd { get; }
}

public interface IQuizariumData
{
    int GetQuizariumTime { get; }
    int QuizariumId { get; }
    bool QuizariumState { get; }
    IEnumerable<int> AnswerIndexQueue { get; }
}

public sealed partial class QuestionDataMgr
{
    public sealed partial class QuestionData:
        IQuestionData
        ,IQuizariumData
    {
        public IQuizariumData QuizariumData { get { return this; } }
        private QuestionType.QuestionTypeEnum _questionType;
        public QuestionType.QuestionTypeEnum GetQuestionType { get { return _questionType; } }

        #region 每日问答
        private long _exp;
        private long _money;
        private PlayerQuestionDto _questionDto;
        private bool _isEnd;

        public bool IsEnd
        {
            get { return _isEnd; }
            set { _isEnd = value; }
        }

        public PlayerQuestionDto QuestionDto
        {
            get { return _questionDto; }
            set { _questionDto = value; }
        }
        public long GetExp { get { return _exp; } }
        public long GetMoney { get { return _money; } }

        private int _curQuestionId;
        public int QuestionId
        {
            get { return _curQuestionId; }
            set { _curQuestionId = value; }
        }

        private int _rightNum;
        public int RightNum
        {
            get { return _rightNum; }
            set { _rightNum = value; }
        }

        private int _totleNum;
        public int TotleNum
        {
            get { return _totleNum; }
            set { _totleNum = value; }
        }
        #endregion

        #region 知识竞答

        private int _quizariumId = 1;

        public int QuizariumId
        {
            get { return _quizariumId; }
            set { _quizariumId = value; }
        }
        private int _quizariumTime; //当前题目所剩时间
        public int GetQuizariumTime { get { return _quizariumTime; } }

        private bool _quizariumBtnState;

        public bool QuizariumState
        {
            get { return _quizariumBtnState; }
            set { _quizariumBtnState = value; }
        }
        #endregion

        public void InitData()
        {
        }

        public void Dispose()
        {

        }

        private List<int> _answerIndexQueue = new List<int>();
        public IEnumerable<int> AnswerIndexQueue {get { return _answerIndexQueue; } }

        public void SetQuestionType(int type)
        {
            _questionType = (QuestionType.QuestionTypeEnum)type;
        }

        #region 每日问答
        public void SetExp(long exp)
        {
            _exp = exp;
        }

        public void SetSilver(long silver)
        {
            _money = silver;
        }
        #endregion

        #region 知识竞答
        public void SetQuizariumTime(int time)
        {
            _quizariumTime = time;
            JSTimer.Instance.SetupCoolDown("QuizariumTime", DataMgr._data.GetQuizariumTime, e =>
            {
                _quizariumTime -= 1;
            },
            () =>
            {
                _quizariumTime = 0;
                JSTimer.Instance.CancelTimer("QuizariumTime");
            }, 1f);
        }

        public void SetAnswerIndexQueue(List<int> list)
        {
            _answerIndexQueue = list;
        }
        #endregion
    }
}
