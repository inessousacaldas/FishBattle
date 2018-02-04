using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class LightMapAsset : ScriptableObject
{
	[SerializeField]
	public Texture2D[] lightmapFar;
	[SerializeField]
	public Texture2D[] lightmapNear;
	[SerializeField]
	public AmbientSetting ambientSetting;
}

[Serializable]
public class AmbientSetting
{
	public bool useFog;
	public Color fogColor;
	public float fogEndDistance;
	public float fogStartDistance;
	public FogMode fogMode;
	public Color ambientColor;
	public Color lightColor;
	public float lightIntensity;
	public Quaternion lightRotate;
	public string skyboxABName;

	public bool hasPointLight;
	public Color pointLightColor;
	public float pointLightRange;
	public float pointLightIntensity;
}