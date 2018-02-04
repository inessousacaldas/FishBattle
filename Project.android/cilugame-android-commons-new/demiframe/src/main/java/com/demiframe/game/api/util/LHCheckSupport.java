package com.demiframe.game.api.util;

import com.demiframe.game.api.connector.ICheckSupport;

public class LHCheckSupport
{
  private static ICheckSupport checkSupport;

  public static ICheckSupport getCheckSupport()
  {
    return checkSupport;
  }

  public static void setCheckSupport(ICheckSupport iCheckSupport)
  {
    checkSupport = iCheckSupport;
  }
}