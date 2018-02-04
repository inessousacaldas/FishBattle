using UnityEngine;

public static class ZXingHelper
{
    /// <summary>
    ///     对合并的颜色进行一个调整
    /// </summary>
    /// <param name="bColor"></param>
    /// <param name="fColor"></param>
    /// <returns></returns>
    private static Color GetCombineColor(Color bColor, Color fColor)
    {
        if (fColor.a == 0)
        {
            fColor = Color.white;
        }
        fColor.a = 1;

        return fColor;
    }


    /// <summary>
    ///     将小图合并在大图上
    /// </summary>
    /// <param name="backTexture"></param>
    /// <param name="frontTexture"></param>
    /// <returns></returns>
    public static Texture2D CombineQRTexture(Texture2D backTexture, Texture2D frontTexture)
    {
        if (backTexture == null ||
            frontTexture == null)
        {
            return null;
        }

        if (backTexture.width < frontTexture.width ||
            backTexture.height < frontTexture.height)
        {
            return null;
        }

        var texture = new Texture2D(backTexture.width, backTexture.height, TextureFormat.RGBA32, false);
        texture.SetPixels(backTexture.GetPixels());
        var startX = Mathf.FloorToInt((backTexture.width - frontTexture.width)/2f);
        var startY = Mathf.FloorToInt((backTexture.height - frontTexture.height)/2f);
        for (int y = 0; y < frontTexture.height; y++)
        {
            for (int x = 0; x < frontTexture.width; x++)
            {
                var c = GetCombineColor(texture.GetPixel(startX + x, startY + y),
                    frontTexture.GetPixel(x, y));

                texture.SetPixel(startX + x, startY + y,
                    c);
            }
        }
        texture.Apply();

        return texture;
    }

//    /// 将小图合并在大图上


//    /// <summary>
//    /// </summary>
//    /// <param name="backTexture"></param>
//    /// <param name="frontTexture"></param>
//    /// <returns></returns>
//    public static Texture2D CombineQRTexture(Texture2D backTexture, UISprite frontSprite)
//    {
//        if (backTexture == null ||
//            frontSprite == null)
//        {
//            return null;
//        }
//
//        var spriteData = frontSprite.GetAtlasSprite();
//        if (spriteData == null)
//        {
//            return null;
//        }
//
//        if (backTexture.width < spriteData.width ||
//            backTexture.height < spriteData.height)
//        {
//            return null;
//        }
//
//        var texture = new Texture2D(backTexture.width, backTexture.height);
//        texture.SetPixels(backTexture.GetPixels());
//        var startX = Mathf.FloorToInt((backTexture.width - spriteData.width) / 2f);
//        var startY = Mathf.FloorToInt((backTexture.height - spriteData.height) / 2f);
//
//        var sTexture = frontSprite.mainTexture as Texture2D;
//        for (int y = 0; y < spriteData.height; y++)
//        {
//            for (int x = 0; x < spriteData.width; x++)
//            {
//                var c = GetCombineColor(texture.GetPixel(startX + x, startY + y),
//                    sTexture.GetPixel(x + spriteData.x,
//                        y + sTexture.height - spriteData.y - spriteData.height));
//
//                texture.SetPixel(startX + x, startY + y,
//                    c);
//            }
//        }
//        texture.Apply();
//
//        return texture;
//    }
}