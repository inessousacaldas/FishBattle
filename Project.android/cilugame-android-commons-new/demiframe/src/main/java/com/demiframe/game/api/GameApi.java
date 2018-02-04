package com.demiframe.game.api;

public class GameApi extends LHGameApi
{
  public static GameApi gameApi;

  public static GameApi getInstance()
  {
    if (gameApi == null)
    {
      gameApi = new GameApi();
    }
    return gameApi;
  }
}