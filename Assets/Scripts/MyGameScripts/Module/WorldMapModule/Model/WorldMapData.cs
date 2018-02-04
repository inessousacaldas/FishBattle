// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 07/04/2017 20:44:09
// **********************************************************************

public interface IWorldMapData
{
    string GetSceneName { get; }
    int FactionID { get; }
    int CurSceneId { get; }
    int HeadTex { get; }
}

public sealed partial class WorldMapDataMgr
{
    public sealed partial class WorldMapData:IWorldMapData
    {

        public void InitData()
        {
        }

        public void Dispose()
        {

        }
        
        public string GetSceneName
        {
            get { return WorldManager.Instance.GetModel().SceneName; }
        }

        public int FactionID {
            get { return ModelManager.Player.FactionID; }
        }

        public int CurSceneId {
            get { return WorldManager.Instance.GetModel().GetSceneId();}
        }

        public int HeadTex {
            get { return ModelManager.Player.GetPlayer().charactor.texture; }
        }
    }
}
