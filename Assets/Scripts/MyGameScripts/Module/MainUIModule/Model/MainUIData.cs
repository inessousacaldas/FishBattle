using System.Runtime.InteropServices;
using AppDto;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public enum ActivityPollType
{
    None,
    FourTower,   //四轮之塔
    Kungfu      //武术大会
}

public interface IMainUIData
{
    ScenePlayerDto SelectedPlayer{ get;}
    MainUIDataMgr.ShowState panelShowState { get; }
    string GetSceneMapName { get; set; }
    Dictionary<int, ItemTip> ItemTipDic { get; }
    ActivityPollType CurActivityType { get; }
}

public sealed partial class MainUIDataMgr
{
    public struct ShowState
    {
        public bool allBtnPanelHide;            // 全部按钮栏隐藏
        public bool rightBottomBtnPanelShow;    // 右下角按钮栏
        public bool rightBtnPanelShow;          // 右侧按钮栏
        public bool topBtnPanelShow;            // 上排按钮
        public bool expandPanelShow;            // 任务组队栏显示
    }
    
    public sealed partial class MainUIData:IMainUIData
    {
        public ScenePlayerDto selectedPlayer = null;
        public ScenePlayerDto SelectedPlayer{ get{ return selectedPlayer; }}
        private Dictionary<int, ItemTip> tipDic = null;
        private string mSceneMapName;
        private ActivityPollType curActivityType = ActivityPollType.None;       //当前活动引导

        public string GetSceneMapName {
            get { return mSceneMapName; }
            set { mSceneMapName = value; }
        }

        public ActivityPollType CurActivityType
        {
            get { return curActivityType; }
            set { curActivityType = value; }
        }

        public ShowState panelShowState {
            get { return _showState; }
        }

        public ShowState _showState = new ShowState()
        {
            rightBottomBtnPanelShow = true
            , rightBtnPanelShow = true
            , topBtnPanelShow = true
            ,expandPanelShow = false
        };

        public void Dispose()
        {
            if (tipDic != null) tipDic = null;
        }

        public void InitData()
        {
           
        }

        public Dictionary<int ,ItemTip> ItemTipDic
        {
            get
            {
                if (tipDic == null)
                {
                    tipDic = DataCache.getDicByCls<ItemTip>();
                }
                return tipDic;
            }
        }
    }
}
