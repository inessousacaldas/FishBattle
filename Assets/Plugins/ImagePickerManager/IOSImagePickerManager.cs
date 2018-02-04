
#if UNITY_EDITOR || UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

public class IOSImagePickerManager
{
#if UNITY_EDITOR || UNITY_IPHONE
    [DllImport("__Internal")]
	private static extern void __saveToCameraRoll(string encodedMedia);

	[DllImport("__Internal")]
	private static extern void __getVideoPathFromAlbum();

	[DllImport("__Internal")]
	private static extern void __pickImage(int source, bool allowsEditing, int width, int height);

	[DllImport("__Internal")]
    private static extern int __screenHeight();

    [DllImport("__Internal")]
    private static extern int __screenWidth();
#endif


    public static void SaveToCameraRoll(string encodedMedia)
	{
#if UNITY_EDITOR || UNITY_IPHONE
        __saveToCameraRoll(encodedMedia);
#endif
    }

	public static void GetVideoPathFromAlbum()
	{
#if UNITY_EDITOR || UNITY_IPHONE
        __getVideoPathFromAlbum();
#endif
    }

	public static void PickImage(int source, bool allowsEditing, int width, int height)
	{
#if UNITY_EDITOR || UNITY_IPHONE
        __pickImage(source, allowsEditing, width, height);
#endif
    }


    public static int ScreenWidth()
	{
#if UNITY_EDITOR || UNITY_IPHONE
        return __screenWidth();
#endif

        return UnityEngine.Screen.width;
	}


	public static int ScreenHeight()
	{
#if UNITY_EDITOR || UNITY_IPHONE
        return __screenHeight();
#endif

        return UnityEngine.Screen.height;
    }
}
