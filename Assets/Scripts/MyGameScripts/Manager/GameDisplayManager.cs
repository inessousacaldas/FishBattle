// **********************************************************************
// Copyright  2013 Baoyugame. All rights reserved.
// File     :  GameDisplayManager.cs
// Author   : senkay
// Created  : 5/22/2013 9:56:11 AM
// Purpose  : 
// **********************************************************************

public static class GameDisplayManager
{
    public enum DisplayLevel
    {
        Low = 0,
        Middle,
        High
    }

    public enum VisibleType
    {
        Player,
        Npc
    }

    /// <summary>
    ///     当前场景最大玩家数量
    /// </summary>
    public static int MaxPlayerDataCount = 50;

    //场景坐骑可见数量
    public static int MaxRideVisibleCount = 2;

    /// <summary>
    ///     当前显示玩家的数量
    /// </summary>
    public static int MaxPlayerVisibleCount = 30;

    private static DisplayLevel _currentLevel = DisplayLevel.Low;
    /// <summary>
    /// 缓存NPCView的最大数量
    /// </summary>
    public static int MaxNpcViewPoolCount = 25;

    public static DisplayLevel CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = value; }
    }
}
