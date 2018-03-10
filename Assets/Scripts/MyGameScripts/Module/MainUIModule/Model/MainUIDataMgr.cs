using UniRx;
using AppDto;
using System;
using UnityEngine;

public sealed partial class MainUIDataMgr
{
    private IDisposable _joyDisposable;

    private void LateInit()
    {
        _data = new MainUIData();
        _joyDisposable = JoystickModule.Stream.Subscribe(info=>{
            _data.selectedPlayer = info.SelectedPlayerDto;
            FireData();
        });
        _disposable.Add(
             WorldManager.WorkdModelStream
                 .Select(data => data == null ? string.Empty : data.SceneName)
                 .CombineLatest(WorldManager.OnHeroPosStreamChange,
                     delegate (string sceneName,Vector3 pos)
                     {
                         var x = (int)(pos.x * 10);
                         var z = (int)(pos.z * 10);
                         if (!Application.isEditor) return x + "," + z;
                         var y = (int)(pos.y * 10);
                         return string.Format("{0},{1},{2}", x, y, z);
                     })
                 .Subscribe(
                 str =>
                 {
                     _data.GetSceneMapName = str;
                     FireData();
                 })
         );
    }

    public void OnDispose(){
        _joyDisposable.Dispose();
        _joyDisposable = null;
    }

    public ShowState GetPanelShowState()
    {
        return _data.panelShowState;
    }
}
