// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class GMDataMgr
{

    public static partial class GMDtoConnViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void InitReactiveEvents(GMDtoConnViewController ctrl)
        {
            if(ctrl == null) return;
            if(_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }

            _disposable.Add(ctrl.chbStartCtrl.ClickStateHandler.Subscribe(isSel =>
            {
                GMDataMgr.isOpenDtoConn = isSel;
            }));
            _disposable.Add(ctrl.chbStartCtrl.ClickStateHandler.Subscribe(isSel =>
            {
                DataMgr._data.DtoConnData.isShield = isSel;
            }));
            _disposable.Add(ctrl.OnbtnClear_UIButtonClick.Subscribe(_ =>
            {
                DataMgr.DtoConnData.Clear();
                FireData();
            }));
        }

        public static void InitReactiveEvents(GMDtoConnItemController ctrl)
        {
            _disposable.Add(ctrl.View.gameObject.OnClickAsObservable().Subscribe(_ => OnItemClick(ctrl)));
        }

        private static void OnItemClick(GMDtoConnItemController ctrl)
        {
                if(DataMgr._data.DtoConnData.curCtrl != null)
                {
                DataMgr._data.DtoConnData.curCtrl.SetSel(false);
                }
                ctrl.SetSel(true);
            DataMgr._data.DtoConnData.curCtrl = ctrl;
            FireData();
        }

        public static void Dispose()
        {
            DataMgr._data.DtoConnData.curCtrl = null;
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }
    }
}


