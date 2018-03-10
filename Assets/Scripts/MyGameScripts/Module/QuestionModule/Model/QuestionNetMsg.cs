// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 12/12/2017 11:40:43 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class QuestionDataMgr
{
    public static class QuestionNetMsg
    {
        #region 每日问答
        //PlayerQuestionDto
        public static void OpenQuestionView(int type, Action callback)
        {
            GameUtil.GeneralReq(Services.Question_Enter(type), e =>
            {
                PlayerQuestionDto dto = e as PlayerQuestionDto;
                if (dto == null || dto.curQuestion == null)
                {
                    if(dto.end)
                        TipManager.AddTip("答题已经结束");
                    GameDebuger.LogError("服务器下发的PlayerQuestionDto为空,请查看");
                    return;
                }

                DataMgr._data.QuestionDto = dto;
                DataMgr._data.QuestionId = dto.curQuestion.id;
                DataMgr._data.RightNum = dto.correctCount;
                DataMgr._data.TotleNum = dto.answerCount;
                DataMgr._data.SetExp(dto.playerExpCount);
                DataMgr._data.SetSilver(dto.silverCount);
                DataMgr._data.QuestionDto.giftBoxState = dto.giftBoxState;
                DataMgr._data.SetQuestionType(dto.curQuestion.questionTypeId);
                if (callback != null)
                    callback();
            });
        }

        //PlayerQuestionDto
        public static void QuestionAnswer(int type, int idx)
        {
            ServiceRequestAction.requestServer(Services.Question_Answer(type, idx), "AnswerQuestion", e =>
            {
                PlayerQuestionDto dto = e as PlayerQuestionDto;
                if (dto == null)
                {
                    if (dto.end)
                        TipManager.AddTip("答题已经结束");
                    GameDebuger.LogError("服务器下发的PlayerQuestionDto为空,请查看");
                    return;
                }

                DataMgr._data.QuestionDto = dto;
                DataMgr._data.QuestionId = dto.end ? 0 : dto.curQuestion.id;
                DataMgr._data.RightNum = dto.correctCount;
                DataMgr._data.TotleNum = dto.answerCount;
                DataMgr._data.SetExp(dto.playerExpCount);
                DataMgr._data.SetSilver(dto.silverCount);
                DataMgr._data.IsEnd = dto.end;
                DataMgr._data.QuestionDto.giftBoxState = dto.giftBoxState;
                FireData();
            });
        }

        //HarvestQuestionGiftBoxDto
        public static void QuestionHarvest(int type)
        {
            ServiceRequestAction.requestServer(Services.Question_Harvest(type), "GetRewardBox", e =>
            {
                HarvestQuestionGiftBoxDto dto = e as HarvestQuestionGiftBoxDto;
                if (dto == null)
                {
                    GameDebuger.LogError("服务器下发的HarvestQuestionGiftBoxDto为空,请查看");
                    return;
                }

                if (dto.boxId > 0)
                {
                    var props = DataCache.getDtoByCls<GeneralItem>(dto.boxId);
                    TipManager.AddTip(string.Format("领取宝箱成功,获得{0}", props.name));
                }
                DataMgr._data.QuestionDto.giftBoxState = dto.giftBoxState;
                FireData();
            });
        }

        public static void QuestionHelp()
        {

        }
        #endregion

        #region 知识竞答

        public static void QuizariumQuestion(int idx)
        {
            ServiceRequestAction.requestServer(Services.Question_Answer((int)QuestionType.QuestionTypeEnum.TYPE_3, idx),
                "QuizariumQuestion", e =>
                {
                    DataMgr._data.QuizariumState = false;
                    TipManager.AddTip("你已答题，等待答案公布");
                    FireData();
                });
        }
        #endregion
    }
}
