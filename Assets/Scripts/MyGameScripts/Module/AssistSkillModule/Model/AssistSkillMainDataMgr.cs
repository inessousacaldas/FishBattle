// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/20/2017 7:57:58 PM
// **********************************************************************

using UniRx;
using System.Collections.Generic;
using Asyn;
using System;
using AppDto;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner assistSkillMainDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = AssistSkillMainDataMgr.DataMgr; });
    }
}
public sealed partial class AssistSkillMainDataMgr : AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete)
    {
        Action act = delegate ()
        {
            onComplete(this);
        };

        if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_42))
        {
            AssistSkillMainNetMsg.ReqAssistInfo(act);
            if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_69))
                AssistSkillMainNetMsg.ReqDelegateMissionMsg();
        }
        else
            GameUtil.SafeRun(act);
    }

    // 初始化
    private void LateInit()
    {
        
    }
    
    public void OnDispose(){
            
    }

    public void SetChoseFriendId(long id)
    {
        _data.ChoseFriendId = id;
        FireData();
    }

    public void ResetChoseCrewList(List<long> choseIdList)
    {
        DataMgr._data.ResetChoseCrewList(choseIdList);
        FireData();
    }
}
