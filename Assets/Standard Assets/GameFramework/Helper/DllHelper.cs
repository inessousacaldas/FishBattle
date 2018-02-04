using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

public static class DllHelper
{
    public const long Max_Versoin = 999999999999999999;
    #region 需要处理的dll
    private static readonly string[] ProjectDlls =
    {
        "Assembly-CSharp",
        "Assembly-CSharp-firstpass",
        "Assembly-UnityScript",
        "Assembly-UnityScript-firstpass",
        "AppModel",
        "H1Model",
    };


    public static bool IsProjectDll(string dllName)
    {
        var result =  Array.IndexOf(ProjectDlls, dllName) >= 0;
        Debug.Log(string.Format("Is {0} Project Dll {1}", dllName, result));

        return result;
    }
    #endregion


    #region 加密解密处理
    private static readonly byte[] DLL_KEY = Encoding.UTF8.GetBytes("QHE8BxTiPWzMr8Je");
    private static readonly byte[] DLL_TAG = Encoding.UTF8.GetBytes("CLDLL");

    public static byte[] DecryptDll(byte[] originBytes)
    {
        var tempBytes = new byte[originBytes.Length - DLL_TAG.Length];
        Array.Copy(originBytes, DLL_TAG.Length, tempBytes, 0, tempBytes.Length);

        return XXTEA.Decrypt(tempBytes, DLL_KEY);
    }


    public static byte[] EncryptDll(byte[] originBytes)
    {
        var encryptBytes = XXTEA.Encrypt(originBytes, DLL_KEY);
        var tempBytes = new byte[encryptBytes.Length + DLL_TAG.Length];
        Array.Copy(DLL_TAG, tempBytes, DLL_TAG.Length);
        Array.Copy(encryptBytes, 0, tempBytes, DLL_TAG.Length, encryptBytes.Length);

        return tempBytes;
    }


    public static void RestartGame()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                {
                    BaoyugameSdk.RestartGame();

                    break;
                }
            case RuntimePlatform.WindowsPlayer:
                {
                    var curApp = Process.GetCurrentProcess();
                    Process.Start(curApp.ProcessName + ".exe");

                    Application.Quit();

                    break;
                }
        }
    }
    #endregion
}
