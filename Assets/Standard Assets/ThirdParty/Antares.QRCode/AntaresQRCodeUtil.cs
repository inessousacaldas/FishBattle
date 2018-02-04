using System;
using UnityEngine;
using System.Collections;
using Antares.QRCode;
using ZXing;
using Result = Antares.QRCode.Result;

public static class AntaresQRCodeUtil
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

    private static Texture2D _texture2D;

	private static Texture2D Texture
	{
		get
		{
			if (_texture2D == null)
			{
				_texture2D = new Texture2D(0, 0, TextureFormat.RGBA32, false);
			}
			return _texture2D;
		}
	}

	private static ErrorCorrectionLevel[] ErrorCorrectionLevelList = new[]
    {
        ErrorCorrectionLevel.L,
        ErrorCorrectionLevel.M,
        ErrorCorrectionLevel.Q,
        ErrorCorrectionLevel.H,
    };

	public static Texture2D Encode(string msg, int size)
    {
        return Encode(msg, size, size, ErrorCorrectionType.L);
    }

    public static Texture2D Encode(string msg, int width, int height, ErrorCorrectionType type)
    {
        return QRCodeProcessor.Encode(msg, width, height, ErrorCorrectionLevelList[(int) type], null);
    }

    public static Result Decode(Color32[] colors, int width, int height)
    {
        Texture.Resize(width, height);
		Texture.SetPixels32(colors);
		Texture.Apply();

	    return Decode(Texture);
    }

	public static Result Decode(WebCamTexture tex, int cropWidth, int cropHeight)
	{
		return Decode(tex.GetPixels32(), tex.width, tex.height, cropWidth, cropHeight);
	}

	public static Result Decode(Color32[] oldColors, int oWidth, int oHeight, int cropWidth, int cropHeight)
	{
		var tempColors = new Color32[cropWidth * cropHeight];
		for (int i = 0; i < cropWidth; i++)
		{
			for (int j = 0; j < cropHeight; j++)
			{
				tempColors[i + j * cropWidth] =
					oldColors[
						(i + oWidth / 2 - cropWidth / 2) +
						(j + oHeight / 2 - cropHeight / 2) * oWidth
						];
			}
		}

		return Decode(tempColors, cropWidth, cropHeight);

	}

	public static Result Decode(Texture2D tex)
    {
        try
        {
            var result = QRCodeProcessor.Decode(tex);
            if (result != null && result.Text != null)
            {
                return result;
            }
        }
        catch (Exception e)
        {
            // ignored
        }
        return null;
    }
}
