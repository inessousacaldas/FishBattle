// --------------------------------------
//  Unity Foundation
//  PlayerPrefsExt.cs
//  copyright (c) 2014 Nicholas Ventimiglia, http://avariceonline.com
//  All rights reserved.
//  -------------------------------------
// 
using UnityEngine;


/// <summary>
/// Extension methods for PlayerPrefs
/// </summary>
public static class PlayerPrefsExt
{
	public static string PLAYER_PREFIX = "";

	/// <summary>
	/// Returns the PlayerPref serialized as a bool.
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public static bool GetBool (string key)
	{
		return PlayerPrefs.GetInt (key, 0) == 1;
	}

	/// <summary> 
	/// Returns the PlayerPref serialized as a bool.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	public static bool GetBool (string key, bool defaultValue)
	{
		return PlayerPrefs.GetInt (key, defaultValue ? 1 : 0) == 1;
	}

	/// <summary>
	/// Sets the PlayerPref with a boolean value.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	public static void SetBool (string key, bool value)
	{
		PlayerPrefs.SetInt (key, value ? 1 : 0);
	}

	#region Operate PlayerPrefs with PlayerID

	public static void SetPlayerString (string key, string value)
	{
		key = PLAYER_PREFIX + "_" + key;
		PlayerPrefs.SetString (key, value);
	}

	public static string GetPlayerString (string key)
	{
		key = PLAYER_PREFIX + "_" + key;
		return PlayerPrefs.GetString (key);
	}

	public static void SetPlayerInt (string key, int value)
	{
		key = PLAYER_PREFIX + "_" + key;
		PlayerPrefs.SetInt (key, value);
	}

	public static int GetPlayerInt (string key, int defaultValue = 0)
	{
		key = PLAYER_PREFIX + "_" + key;
		int result = PlayerPrefs.GetInt (key, defaultValue);
		return result;
	}

    public static void SetPlayerBool(string key, bool value)
    {
        key = PLAYER_PREFIX + "_" + key;
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool GetPlayerBool(string key, bool defaultValue = false)
    {
        key = PLAYER_PREFIX + "_" + key;
        bool result = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        return result;
    }

    public static void SetPlayerFloat (string key, float value)
	{
		key = PLAYER_PREFIX + "_" + key;
		PlayerPrefs.SetFloat(key, value);
	}

	public static float GetPlayerFloat (string key, float defaultValue = 0f)
	{
        return defaultValue;
		key = PLAYER_PREFIX + "_" + key;
		float result = PlayerPrefs.GetFloat (key, defaultValue);
		return result;
	}

	public static bool HasPlayerKey (string key)
	{
		key = PLAYER_PREFIX + "_" + key;
		return PlayerPrefs.HasKey (key);
	}

	public static void DeletePlayerKey (string key)
	{
		key = PLAYER_PREFIX + "_" + key;
		PlayerPrefs.DeleteKey (key);
	}

	#endregion
}