// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : FontManager.cs
// Author   : senkay <senkay@126.com>
// Created  : 4/27/2016 
// Porpuse  : 
// **********************************************************************
//

using System.Collections.Generic;
using UnityEngine;
using AssetPipeline;
public class FontManager
{
    private static Dictionary<string, UIFont> fontDic;

    public static UIFont GetFont(string fontName)
    {
        if (fontDic == null)
        {
            fontDic = new Dictionary<string, UIFont>();
        }

        if (fontDic.ContainsKey(fontName))
        {
            return fontDic[fontName];
        }
        GameObject fontPrefab =
            AssetManager.Instance.LoadAsset(fontName, ResGroup.UIFont) as GameObject;
        if (fontPrefab != null)
        {
            UIFont uiFont = fontPrefab.GetComponent<UIFont>();
            if (uiFont != null)
            {
                fontDic.Add(fontName, uiFont);
                return uiFont;
            }
            return null;
        }
        return null;
    }
}