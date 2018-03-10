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
        //7代表周日
        if (date == 7)
        {
            TipManager.AddTip("周一至周六10:00至24:00可进行答题活动");
            return; 
        }

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

