using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

public static class ZXingUtils
{
    /// <summary>
    /// 容错级别
    /// </summary>
    public enum ErrorCorrectionType
    {
        L,
        M,
        Q,
        H,
    }

    private static Dictionary<ErrorCorrectionType, ErrorCorrectionLevel> _correctionLevelDict = new Dictionary
        <ErrorCorrectionType, ErrorCorrectionLevel>()
    {
        {ErrorCorrectionType.L, ErrorCorrectionLevel.L},
        {ErrorCorrectionType.M, ErrorCorrectionLevel.M},
        {ErrorCorrectionType.Q, ErrorCorrectionLevel.Q},
        {ErrorCorrectionType.H, ErrorCorrectionLevel.H},
    };

    /// <summary>
    /// 生成二维码，如果有错误，返回string
    /// </summary>
    /// <param name="encodeText"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Texture2D EncodeQR(string encodeText, int size = 256, ErrorCorrectionType type = ErrorCorrectionType.L, string character = "UTF-8")
    {
        var hints = new Dictionary<EncodeHintType, object>();
        // 设置容错率
        var level = _correctionLevelDict.ContainsKey(type) ? _correctionLevelDict[type] : ErrorCorrectionLevel.L;
        hints.Add(EncodeHintType.ERROR_CORRECTION, level);
        // 设置编码
        hints.Add(EncodeHintType.CHARACTER_SET, character);
        // 设置外框大小
        hints.Add(EncodeHintType.MARGIN, 0);

        var bit = new MultiFormatWriter().encode(
            encodeText, BarcodeFormat.QR_CODE, size, size, hints);
        var texture = new Texture2D(bit.Width, bit.Height, TextureFormat.RGBA32, false);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (bit[y, x])
                {
                    texture.SetPixel(x, y, Color.black);
                }
                else
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }
        texture.Apply();

        return texture;
    }




    /// <summary>
    /// 解码图片，如果解码失败，则返回null
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public static Result DecodeQR(Texture2D texture)
    {
        var barcodeReader = new BarcodeReader
        {
            Options = new DecodingOptions
            {
                CharacterSet = "UTF-8",
            },
            AutoRotate = true,
            TryInverted = false,
        };
        return barcodeReader.Decode(texture.GetPixels32(), texture.width, texture.height);
    }
}
