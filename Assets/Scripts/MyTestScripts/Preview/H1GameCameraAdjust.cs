using UnityEngine;

public class H1GameCameraAdjust : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera camera = Camera.main;
		camera.orthographic = false;
		camera.transform.localPosition = CameraConst.WorldCameraLocalPosition;
		camera.transform.localEulerAngles = CameraConst.WorldCameraLocalEulerAngles;
		camera.fieldOfView = CameraConst.WorldCameraFieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private class CameraConst
    {
        //public static Vector3 BattleCameraLocalPosition = new Vector3(-13.65f, 16.50f, 22.00f);
        //public static Vector3 BattleCameraLocalEulerAngles = new Vector3(30.00f, 150.00f, 0.00f);
        //public static int BattleCameraFieldOfView = 20;
        //public static float BattleCameraOrthographicSize = 4.69f;

        public static Vector3 WorldCameraLocalPosition = new Vector3(0f, 14.4f, -24.5f);
        public static Vector3 WorldCameraLocalEulerAngles = new Vector3(30f, 0f, 0f);
        public static int WorldCameraFieldOfView = 20;
    }
}
