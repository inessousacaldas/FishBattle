using UnityEngine;
using System;
using System.Collections;
using AssetPipeline;

public class TextureLoader
{
    private static byte[] tempByte;
    public static Texture2D LoadTexture2D(byte[] rawData)
    {
        if (AssetManager.ResLoadMode == AssetManager.LoadMode.EditorLocal)
        {
            return null;
        }
        else
        {
            return LoadPVRTexture(rawData);
        }

    }
    private static Texture2D LoadPVRTexture(byte[] rawData)
    {
        if (rawData == null || rawData.Length <= 64)
        {
            return null;
        }

        ulong format = BitConverter.ToUInt64(rawData, 8);
        TextureFormat textureFormat;
        switch (format)
        {
            case 0:
                textureFormat = TextureFormat.PVRTC_RGB2;
                break;
            case 1:
                textureFormat = TextureFormat.PVRTC_RGBA2;
                break;
            case 2:
                textureFormat = TextureFormat.PVRTC_RGB4;
                break;
            case 3:
                textureFormat = TextureFormat.PVRTC_RGBA4;
                break;
            case 6:
                textureFormat = TextureFormat.ETC_RGB4;
                break;
            case 7:
                textureFormat = TextureFormat.DXT1;
                break;
            default:
                return null;
        }

        uint mipmapCount = BitConverter.ToUInt32(rawData, 44);
        if (mipmapCount != 1)
        {
            return null;
        }
        //ProfileHelper.ProfilerBegin("LoadPVRTexture");
        uint colorSpace = BitConverter.ToUInt32(rawData, 16);
        bool linear = colorSpace == 0;
        uint height = BitConverter.ToUInt32(rawData, 24);
        uint width = BitConverter.ToUInt32(rawData, 28);
        uint dataSize = BitConverter.ToUInt32(rawData, 60);
        int headerSize = 64 + (int)dataSize;
        Texture2D texture = new Texture2D((int)width, (int)height, textureFormat, false, linear);
        texture.wrapMode = TextureWrapMode.Clamp;

        if (tempByte == null || tempByte.Length != rawData.Length - headerSize)
        {
            tempByte = new byte[rawData.Length - headerSize];  
        }
        Buffer.BlockCopy(rawData, headerSize, tempByte, 0, tempByte.Length);
        texture.LoadRawTextureData(tempByte);

        texture.Apply();
        //ProfileHelper.ProfilerEnd("LoadPVRTexture");
        return texture;
    }
    public static Texture2D LoadAlpha8Texture(byte[] rawData, int width, int height)
    {
        if (rawData == null || width <= 0 || height <= 0
            || rawData.Length < width * height)
        {
            return null;
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.Alpha8, false);
        texture.LoadRawTextureData(rawData);
        texture.Apply();
        return texture;
    }
    
}
