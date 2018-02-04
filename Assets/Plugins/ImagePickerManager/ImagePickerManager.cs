using UnityEngine;
using System;
using System.Collections;


public class ImagePickerManager : MonoBehaviour
{
    public enum ImageSource
    {
        Library = 0,
        Album = 1,
        Camera = 2
    }


    private static ImagePickerManager _instance;

    public static ImagePickerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject(typeof(ImagePickerManager).Name);
                _instance = go.AddComponent<ImagePickerManager>();
            }
            return _instance;
        }
    }

    public event Action<bool, string> OnImagePicked;
    public event Action<bool> OnImageSaved;
    public event Action<string> OnVideoPathPicked;

    private bool _isWaitngForResponce = false;

    #region 辅助
    public static Texture2D GetTexture2DFromString(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return null;
        }

        var decodedFromBase64 = DecodeString(data);
        var image = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        image.LoadImage(decodedFromBase64);

        return image;
    }

    public static byte[] DecodeString(string data)
    {
        byte[] byteArray = null;
        if (data != null)
        {
            byteArray = System.Convert.FromBase64String(data);
        }
        return byteArray;
    }

    public static int ScreenWidth
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return IOSImagePickerManager.ScreenWidth();
            }

            return Screen.width;
        }
    }


    public static int ScreenHeight
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return IOSImagePickerManager.ScreenHeight();
            }

            return Screen.height;
        }
    }

    public static int ConvertPixelToPoint(float pixel, bool width)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            float scale = 0;
            if (width)
            {
                scale = 1f * ScreenWidth / Screen.width;
            }
            else
            {
                scale = 1f * ScreenHeight / Screen.height;
            }

            return (int)(pixel * scale);
        }

        return (int)pixel;
    }
    #endregion


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }


    public void SaveTextureToCameraRoll(Texture2D texture)
    {
        if (texture != null)
        {
            byte[] datas = null;
            datas = texture.EncodeToPNG();
            var bytesString = System.Convert.ToBase64String(datas);

            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    {
                        IOSImagePickerManager.SaveToCameraRoll(bytesString);
                        break;
                    }
            }
        }
    }


    public void SaveScreenshotToCameraRoll()
    {
        StartCoroutine(SaveScreenshot());
    }

    private IEnumerator SaveScreenshot()
    {

        yield return new WaitForEndOfFrame();
        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        SaveTextureToCameraRoll(tex);

        Destroy(tex);
    }


    public void GetVideoPathFromAlbum()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
                {
                    IOSImagePickerManager.GetVideoPathFromAlbum();
                    break;
                }
        }
    }


    public void PickImage(ImageSource source, float width, float height, bool allowsEditing = true)
    {
        int pointWidth = ConvertPixelToPoint(width, true);
        int pointHeight = ConvertPixelToPoint(height, false);

        if (_isWaitngForResponce)
        {
            return;
        }
        _isWaitngForResponce = true;

        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
                {
                    // 使用自定义的裁剪，所以传false
                    IOSImagePickerManager.PickImage((int)source, allowsEditing, pointWidth, pointHeight);
                    break;
                }
        }
    }



    private void OnImagePickedEvent(string data)
    {
        _isWaitngForResponce = false;

        if (OnImagePicked != null)
        {
            OnImagePicked(!string.IsNullOrEmpty(data), data);
        }
    }

    private void OnImageSaveSuccess()
    {
        _isWaitngForResponce = false;

        if (OnImageSaved != null)
        {
            OnImageSaved(false);
        }
    }

    private void OnImageSaveFailed()
    {
        _isWaitngForResponce = false;

        if (OnImageSaved != null)
        {
            OnImageSaved(true);
        }
    }

    private void OnVideoPickedEvent(string path)
    {
        _isWaitngForResponce = false;

        if (OnVideoPathPicked != null)
        {
            OnVideoPathPicked(path);
        }
    }
}
