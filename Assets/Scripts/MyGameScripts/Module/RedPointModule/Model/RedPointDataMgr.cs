// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/25/2018 5:23:19 PM
// **********************************************************************

using UniRx;
using AppDto;
using Asyn;
using System;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner redPointDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = RedPointDataMgr.DataMgr; });
    }
}

public sealed partial class RedPointDataMgr : AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete)
    {
        Action act = delegate ()
        {
            onComplete(this);
        };

        GameUtil.SafeRun(act);
    }

    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<ShowRedPointTypeDto>(UpdateServicePointData));
        _disposable.Add(NotifyListenerRegister.RegistListener<ShowRedPointTypeListDto>(UpdateServicePointData));
    }
    
    private void OnDispose(){
            
    }

    public void UpdateSingleData(int type, bool isShow, int n = 0)
    {
        DataMgr._data.UpdateSingleData(type, isShow, n);
    }

    public RedPointSingleData GetRedPointData(int type)
    {
        return DataMgr._data.GetRedPointData(type);
    }

    public void UpdateServicePointData(ShowRedPointTypeDto dto)
    {
        if (dto == null) return;
        DataMgr._data.UpdateSingleData(dto.redPointId, true, dto.count);
    }

    public void UpdateServicePointData(ShowRedPointTypeListDto dto)
    {
        if (dto == null || dto.showRedPointTypeListDto == null) return;
        dto.showRedPointTypeListDto.ForEach(itemData =>
        {
            DataMgr._data.UpdateSingleData(itemData.redPointId, true, itemData.count);
        });
    }
}
