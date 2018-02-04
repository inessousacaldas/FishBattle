// --------------------------------------
//  Unity Foundation
//  Vector3Helper.cs
//  copyright (c) 2014 Nicholas Ventimiglia, http://avariceonline.com
//  All rights reserved.
//  -------------------------------------
// 
using UnityEngine;

public static class VectorHelper
{
	public static string ToString (Vector2 v)
	{
		return string.Format ("{0:F1},{1:F1}", v.x, v.y);
	}

	public static string ToString (Vector3 v)
	{
		return string.Format ("{0:F1},{1:F1},{2:F1}", v.x, v.y, v.z);
	}

	public static string ToString (Vector4 v)
	{
		return string.Format ("{0:F1},{1:F1},{2:F1},{3:F1}", v.x, v.y, v.z, v.w);
	}

	/// <summary>
	/// From string
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public static Vector2 ParseToVector2 (string text, Vector2 defaultValue)
	{
		var args = text.Split (',');
		
		if (args.Length != 2) {
			Debug.LogWarning ("Vector2.Parse takes an input of float,float");
			return defaultValue;
		} else {
			return new Vector2 (float.Parse (args [0]), float.Parse (args [1]));
		}
	}

	public static Vector3 ParseToVector3 (string text, Vector3 defaultValue)
	{
		var args = text.Split (',');

		if (args.Length != 3) {
			Debug.LogWarning ("Vector3.Parse takes an input of float,float,float");
			return defaultValue;
		} else {
			return new Vector3 (float.Parse (args [0]), float.Parse (args [1]), float.Parse (args [2]));
		}
	}

	public static Vector4 ParseToVector4 (string text, Vector4 defaultValue)
	{
		var args = text.Split (',');
		
		if (args.Length != 4) {
			Debug.LogWarning ("Vector4.Parse takes an input of float,float,float,float");
			return defaultValue;
		} else {
			return new Vector4 (float.Parse (args [0]), float.Parse (args [1]), float.Parse (args [2]), float.Parse (args [3]));
		}
	}

	/// <summary>
	/// Transforms Random.insideUnitCircle from 2d to 3d space (swap Y and Z).
	/// </summary>
	/// <returns></returns>
	public static Vector3 To2DSpace (this Vector3 v)
	{
		return new Vector3 (v.x, 0, v.y);
	}
}