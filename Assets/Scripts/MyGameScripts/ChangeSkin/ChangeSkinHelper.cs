using UnityEngine;
 
public static class ChangeSkinHelper
{
    #region 通用接口
    public static void ChangeAtlas(UISprite sprite, UIAtlas newAtlas, bool makePixelPerfect = true, bool check = true)
    {
        if (sprite != null && newAtlas != null)
        {
            bool canChange = true;

            if (check)
            {
                if (newAtlas.GetSprite(sprite.spriteName) == null)
                {
                    canChange = false;
                }
            }

            if (canChange)
            {
                sprite.atlas = newAtlas;

                if (makePixelPerfect)
                {
                    sprite.MakePixelPerfect();
                }
            }
        }
    }


    public static void ChangeTexture(UITexture tex, Texture res, bool makePixelPerfect = true)
    {
        if (tex != null && res != null)
        {
            tex.mainTexture = res;

            if (makePixelPerfect)
            {
                tex.MakePixelPerfect();
            }
        }
    }
    #endregion


    #region 地图
    public static void ChangeMiniMapSkin(MiniMapController controller)
    {
        GameDebuger.TODO(@"if (controller != null)
        {
            switch (GameSetting.Game)
            {
                case GameSetting.GameType.Yhxj:
                    {
                        YhxjSkin.ChangeMiniMapSkin(controller);
                        break;
                    }
            }
        }");
    }

    public static string GetWorldMapTexResName(string defaultName)
    {
        GameDebuger.TODO(@"switch (GameSetting.Game)
        {
            case GameSetting.GameType.Yhxj:
                {
                    defaultName = YhxjSkin.WorldMap_Yhxj;
                    break;
                }
        }");

        return defaultName;
    }
    #endregion

    #region 充值元宝
//    public static void ChangeGoldBuyWinSkin(GoldBuyWinUIController controller)
//    {
//        if (controller != null)
//        {
//            switch (GameSetting.Game)
//            {
//                case GameSetting.GameType.Yhxj:
//                    {
//                        YhxjSkin.ChangeGoldBuyWinSkin(controller);
//                        break;
//                    }
//            }
//        }
//    }

    public static string GetGoldCellResName(string defaultName)
    {
        GameDebuger.TODO(@"switch (GameSetting.Game)
        {
            case GameSetting.GameType.Yhxj:
                {
                    defaultName = YhxjSkin.GoldCell_Yhxj;
                    break;
                }
        }");

        return defaultName;
    }
    #endregion

    #region 主界面
//    public static void ChangeMainUISkin(MainUIViewController controller)
//    {
//        GameDebuger.TODO(@"if (controller != null)
//        {
//            switch (GameSetting.GameType)
//            {
//                case GameSetting.GameType:
//                    {
//                        YhxjSkin.ChangeMainUISkin(controller);
//                        break;
//                    }
//            }
//        }");
//    }
    #endregion
}
