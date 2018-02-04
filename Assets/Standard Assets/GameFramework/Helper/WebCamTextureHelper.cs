using System;
using UnityEngine;
using System.Collections;


/// <summary>
/// 用于调用摄像机
/// </summary>
public static class WebCamTextureHelper
{
    /// <summary>
    /// 摄像机设备，这玩意据说消耗大，所以获取后缓存下来
    /// </summary>
    private static WebCamDevice[] _webCamDevices;

    public static WebCamDevice[] WebCamDevices
    {
        get
        {
            if (_webCamDevices == null)
            {
                _webCamDevices = WebCamTexture.devices;
            }
            return _webCamDevices;
        }
    }

    /// <summary>
    /// 判断是否有摄像机，但是这个无法判断摄像机权限
    /// </summary>
    public static bool HasWebCamDevices
    {
        get { return WebCamDevices.Length > 0; }
    }


    /// <summary>
    /// 返回摄像机Texture，返回的时候是未打开的，所有设置都得在Play之前设置好
    /// </summary>
    /// <param name="onlyBackCamera"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="fps"></param>
    /// <returns></returns>
    public static WebCamTexture GetNewWebCamTexture(int width = 0, int height = 0, bool onlyBackCamera = true, int fps = 0)
    {
        WebCamTexture texture = null;
        for (int i = 0; i < WebCamDevices.Length; i++)
        {
            var device = WebCamDevices[i];
            if (!onlyBackCamera || !device.isFrontFacing)
            {
                try
                {
                    texture = new WebCamTexture(device.name);
                    if (width > 0)
                    {
                        texture.requestedWidth = width;
                    }
                    if (height > 0)
                    {
                        texture.requestedHeight = height;
                    }
                    if (fps > 0)
                    {
                        texture.requestedFPS = fps;
                    }
                    else
                    {
                        texture.requestedFPS = 30;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                break;
            }
        }
        return texture;
    }
}
