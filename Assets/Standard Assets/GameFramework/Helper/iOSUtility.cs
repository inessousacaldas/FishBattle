using UnityEngine;
using System.Collections;


public class iOSUtility
{
#if UNITY_EDITOR || UNITY_IPHONE
    [System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern void _XcodeLog ( string message );
	
	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern uint _GetFreeMemory ( );
	
	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern uint _GetTotalMemory ( );
	
	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern float _GetTotalDiskSpaceInBytes ( );
	
	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern float _GetFreeDiskSpaceInBytes ( );

	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern bool _IsBattleCharging ( );

	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern float _GetBatteryLevel ( );

	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern float _ExcludeFromBackupUrl ( string url );

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void saveToGallery(string path);

	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern float _GetBrightness ();

	[System.Runtime.InteropServices.DllImport ("__Internal")]
	private static extern void _SetBrightness (float brightness);
#endif

    public static void XCodeLog ( string message )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            _XcodeLog( message );
#endif
        }
	}
	//in Kbytes
	public static uint GetFreeMemory ( )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            return _GetFreeMemory ( );
#endif
        }
		return 0;
	}
	
	//in Kbytes
	public static uint GetTotalMemory ( )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            return _GetTotalMemory ( );
#endif
        }
		return 0;
	}
	
	public static float GetTotalDiskSpaceInBytes ( )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            return _GetTotalDiskSpaceInBytes ( );
#endif
        }
		return 0f;
	}
	
	public static float GetFreeDiskSpaceInBytes ( )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            return _GetFreeDiskSpaceInBytes ( );
#endif
        }
		return 0f;
	}

	public static int GetBatteryLevel ( )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            float level = _GetBatteryLevel();
			return (int)level;
#endif
        }
		return 100;
	}

	public static bool IsBattleCharging ( )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            return _IsBattleCharging ( );
#endif
        }
		return false;
	}

	public static void ExcludeFromBackupUrl( string url )
	{
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_EDITOR || UNITY_IPHONE
            UnityEngine.iOS.Device.SetNoBackupFlag(url);
#endif
        }
    }

    public static void SaveToGallery(string path)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_EDITOR || UNITY_IPHONE
            saveToGallery(path);
#endif
        }
    }

    public static int GetBrightness ( )
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            float iosBrightness = _GetBrightness();
			Debug.Log("GetBrightness " + iosBrightness);
			return (int)(iosBrightness*255f);
#endif
        }
		return 255;
	}

	public static void SetBrightness (int brightness)
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if UNITY_EDITOR || UNITY_IPHONE
            float iosBrightness = (float)brightness/255f;
			Debug.Log("SetBrightness " + iosBrightness);
			_SetBrightness ( iosBrightness );
#endif
        }
	}
}
