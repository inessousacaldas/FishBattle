using System;
using UnityEngine;

/// <summary>
/// 手机相册打开并读取图片
/// </summary>
public static class MobilePhotoReader
{
    public const string PACKAGE_NAME = "com.cilugame.mobilephotoreader";

    public static string ReadAndCropPhoto(string objectName, string funName, string fileName)
    {
        if (string.IsNullOrEmpty(objectName) || string.IsNullOrEmpty(funName) || string.IsNullOrEmpty(fileName))
            return "参数错误！";

#if UNITY_EDITOR
	    return "photoReader can't work in editor";
#elif !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            AndroidJavaClass andrioidJavaClass = new AndroidJavaClass(PACKAGE_NAME + ".PhotoReaderMain");

            AndroidJavaClass currUnityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            object currActivity = currUnityActivityClass.GetStatic<AndroidJavaObject>("currentActivity");

            andrioidJavaClass.CallStatic("ReadAndCropPhoto", objectName, funName, fileName, currActivity);

	        return null;
        }
        catch (Exception e)
        {
	        return e.Message;
        }
#endif
#pragma warning disable 0162
        return "未知错误！";
#pragma warning restore
    }


    public static string ReadAndCompressPhoto(string objectName, string funName, string fileName)
    {
        if (string.IsNullOrEmpty(objectName) || string.IsNullOrEmpty(funName) || string.IsNullOrEmpty(fileName))
            return "参数错误！";

#if UNITY_EDITOR
        return "photoReader can't work in editor";
#elif !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            AndroidJavaClass andrioidJavaClass = new AndroidJavaClass(PACKAGE_NAME+ ".PhotoReaderMain");

            AndroidJavaClass currUnityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            object currActivity = currUnityActivityClass.GetStatic<AndroidJavaObject>("currentActivity");

            andrioidJavaClass.CallStatic("ReadAndCompressPhoto", objectName, funName, fileName, currActivity);

	        return null;
        }
        catch (Exception e)
        {
	        return e.Message;
        }
#endif
#pragma warning disable 0162
        return "未知错误！";
#pragma warning restore
    }
}

