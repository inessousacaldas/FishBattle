using UnityEngine;

public static class CameraConst
{
    #region 战斗摄像机的信息会在 LayerManager 的 CacheCameraInfo 中赋值
    public static Vector3 BattleCameraLocalPosition = new Vector3 (-3.3f,6.99f, 11.66f);
    public static Vector3 BattleCameraLocalEulerAngles = new Vector3 (30.00f,164.00f,0.00f);
    public static float BattleCameraFieldOfView = 19f;
	public static float BattleMaxFieldOfView = 38f;
	public static float BattleMinFieldOfView = 20f;
    public static float BattleCameraOrthographicSize = 6.4699f;
    public static Quaternion BATTLECAMERA_DEFAULT_ROTATION = Quaternion.Euler(new Vector3(35.58f,174.99f,0f));
	public static Vector3 BATTLECAMERA_DEFAULT_ROTATION_INIT = new Vector3(0f,0f,0f);
	public static Vector3 BattleSceneCenter = new Vector3(0f, 0f, 0f);
    #endregion
	
	public static Vector3 WorldCameraLocalPosition = new Vector3 (7f, 10f, 11f);
	public static Vector3 WorldCameraLocalEulerAngles = new Vector3 (35f, -150f,0f);
	public static int WorldCameraFieldOfView = 30;
	public static float WorldMaxFieldOfView = 30f;
	public static float WorldMinFieldOfView = 15f;

}