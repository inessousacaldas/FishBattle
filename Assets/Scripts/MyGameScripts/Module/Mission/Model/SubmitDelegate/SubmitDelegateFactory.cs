using AppDto;
using System;
using System.Collections.Generic;


/// <summary>
/// SubmitDelegate 享元工厂,增加 ISubmitDelegate 后要在此注册
/// </summary>
public class SubmitDelegateFactory {
    private Dictionary<string,ISubmitDelegate> _submitDelegates;
    private ISubmitDelegate _defaultSubmitDelegate;

    public void Setup(MissionDataMgr.MissionData missionModel) {
        RegisterSubmit(missionModel);
    }

    public ISubmitDelegate GetHandleSubmitDelegate(string submitName) {
        ISubmitDelegate submitDelegate;
        if(_submitDelegates.ContainsKey(submitName))
        {
            submitDelegate = _submitDelegates[submitName];
        }
        else {
#if UNITY_EDITRO
            GameDebuger.OrangeLog(string.Format("{0}没有对应的 SubmitDelegate,使用默认的处理方法", submitName));
#endif
            submitDelegate = _defaultSubmitDelegate;
        }
        return submitDelegate;
    }

    private void RegisterSubmit(MissionDataMgr.MissionData missionModel) {
        _defaultSubmitDelegate = new DefaultSubmitDelegate(missionModel,string.Empty);
        _submitDelegates = new Dictionary<string,ISubmitDelegate>();
        Action<BaseSubmitDelegate> addSubmitDelegate = (submitDelegate) => {
            _submitDelegates.Add(submitDelegate.GameSubmitName(),submitDelegate as ISubmitDelegate);
        };
        addSubmitDelegate(new TalkSubmitDelegate(missionModel,typeof(TalkSubmitDto).Name));
        addSubmitDelegate(new ShowMonsterSubmitDelegate(missionModel,typeof(ShowMonsterSubmitDto).Name));
        addSubmitDelegate(new ApplyItemSubmitDelegate(missionModel,typeof(ApplyItemSubmitDto).Name));
        addSubmitDelegate(new HiddenMonsterSubmitDelegate(missionModel,typeof(HiddenMonsterSubmitDto).Name));
        //采集任务
        addSubmitDelegate(new CollectionSubmitDelegate(missionModel,typeof(PickItemSubmitInfoDto).Name));
        //多提交任务
        addSubmitDelegate(new FindtemSubmitDelegate(missionModel,typeof(FindtemSubmitInfoDto).Name));
        //指定怪掉落道具
        addSubmitDelegate(new ShowMonsterItemSubmitDelegate(missionModel, typeof(ShowMonsterItemSubmitDto).Name));
        //护送任务
        //addSubmitDelegate(new ProtectSubmitDelegate(missionModel,typeof(ProtectSubmitDto).Name));
        //收集道具（商城购买）任务
        addSubmitDelegate(new CollectionItemSubmitDelegate(missionModel,typeof(CollectionItemSubmitDto).Name));
        //收集道具（商城购买）任务
        addSubmitDelegate(new CollectionItemCategorySubmitDtoDelegate(missionModel,typeof(CollectionItemCategorySubmitDto).Name));
        //宣言任务 
        addSubmitDelegate(new SpeakSubmitDelegate(missionModel,typeof(SpeakSubmitDto).Name));
    }
}
