using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public static class TextureHelper
{
	public static Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight, bool destroySource = true)
	{
		if (source == null)
		{
			return null;
		}

		var result = new Texture2D(targetWidth, targetHeight, source.format, false);
		try
		{
			var incX = 1f/targetWidth;
			var incY = 1f/targetHeight;
			var newColors = new Color[targetWidth * targetHeight];
			for (int i = 0; i < result.height; i++)
			{
				for (int j = 0; j < result.width; j++)
				{
					newColors[i*result.width + j] = source.GetPixelBilinear(incX*j, incY*i);
				}
			}

			result.SetPixels(newColors);
			result.Apply();

			if (destroySource)
			{
				Object.Destroy(source);
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e);
			result = source;
		}

		return result;
	}


	public static Texture2D LimitingTextureSize(Texture2D tex, int limitedWidth, int limitedHeight,
		bool destroySource = true)
	{
		if (tex.width > limitedWidth || tex.height > limitedHeight)
		{
			var widthScale = 1f*limitedWidth/tex.width;
			var heightScale = 1f*limitedHeight/tex.height;
            var scale = Mathf.Min(widthScale, heightScale);
			return ResizeTexture(tex, (int)(tex.width * scale), (int)(tex.height * scale), destroySource);
		}
		else
		{
			return tex;
		}
	}
}
