using System.Collections.Generic;
using AppDto;
using AppServices;
using System;
using GamePlot;

public partial class MissionModel:IModuleModel
{
    private MissionDelegateFactory _missionDelegateFactory;
    private SubmitDelegateFactory _submitDelegateFactory;
    public void SetUp(Action callback) {
        //SetupCore();
        //_missionDelegateFactory = new MissionDelegateFactory();
        //_missionDelegateFactory.Setup(this);
        //_submitDelegateFactory = new SubmitDelegateFactory();
        //_submitDelegateFactory.Setup(this);

        // 跨天回调
        SystemTimeManager.Instance.OnChangeNextDay += OnChangeNextDay;
    }
    public void Dispose()
    {
    }

    #region 跨天处理,跨周处理
    private void OnChangeNextDay() {
        
    }
    #endregion
}
