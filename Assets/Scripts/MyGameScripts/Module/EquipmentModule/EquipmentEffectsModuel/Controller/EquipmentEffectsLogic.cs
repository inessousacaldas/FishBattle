// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 9/22/2017 3:49:56 PM
// **********************************************************************

using UniRx;

public sealed partial class EquipmentEffectsDataMgr
{
    
    public static partial class EquipmentEffectsLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = EquipmentEffectsController.Show<EquipmentEffectsController>(
                EquipmentEffects.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IEquipmentEffectsController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => { UIModuleManager.Instance.CloseModule(EquipmentEffects.NAME); }));
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));

            //镶嵌按钮
            _disposable.Add(ctrl.OnHoleBtn_UIButtonClick.Subscribe(_ => {

            }));
            _disposable.Add(ctrl.OnChoiceEquipmentStream.Subscribe(x => {
                DataMgr._data.CurChoiceEquipment = x;
                FireData();
            }));
            //选中特技石
            _disposable.Add(ctrl.OnClickGoodsStream.Subscribe(_ => {
                
            }));
           
        }
            
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            
        }

    
    }
}

