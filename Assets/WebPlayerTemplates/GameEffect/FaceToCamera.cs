using UnityEngine;
using System.Collections;

public class FaceToCamera : MonoBehaviour
{
	public Camera targetCamera;
	public bool reverseFace = false;

	private Transform mTrans;
	private Transform mCamTrans;

	void  Start ()
	{
		mTrans = this.transform;
		// if no camera referenced, grab the main camera
		if (!targetCamera)
			targetCamera = Camera.main;

		if (targetCamera != null)
			mCamTrans = targetCamera.transform;
		else
			this.enabled = false;
	}

	void  Update ()
	{
		if (mCamTrans != null) {
			// rotates the object relative to the camera
			Vector3 targetPos = mTrans.position + mCamTrans.rotation * (reverseFace ? Vector3.back : Vector3.forward);
			Vector3 targetOrientation = mCamTrans.rotation * Vector3.up;
			mTrans.LookAt (targetPos, targetOrientation);
		}
	}
}