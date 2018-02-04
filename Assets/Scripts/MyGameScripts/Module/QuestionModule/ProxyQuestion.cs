// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 12/12/2017 11:40:43 AM
// **********************************************************************

using AppDto;

public class ProxyQuestion
{
    public static void OpenQuestionView()
    {
        var date = SystemTimeManager.Instance.GetCHDayOfWeek();
        if (date == 7) return;  //7代表周日

        int type = date %2 == 1
            ? (int) QuestionType.QuestionTypeEnum.TYPE_1
            : (int) QuestionType.QuestionTypeEnum.TYPE_2;

        QuestionDataMgr.QuestionNetMsg.OpenQuestionView(type, () => { QuestionDataMgr.QuestionViewLogic.Open(); });
    }

    public static void CloseQuestionView()
    {
        UIModuleManager.Instance.CloseModule(QuestionView.NAME);
    }

    public static void OpenQuizariumView()
    {
       QuestionDataMgr.QuizariumViewLogic.Open();
    }

    public static void CloseQuizariumView()
    {
        UIModuleManager.Instance.CloseModule(QuizariumView.NAME);
    }
}

