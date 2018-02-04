using UnityEngine;
using System.Collections;

public class CameraMove2d : MonoBehaviour
{
    public Transform target;
    public bool followTarget = false;

    public bool isSmooth = true;
    public float damping = 5f;

    public float xMin;
    public float xMax = 999f;
    public float yMin;
    public float yMax = 999f;

    private Transform mTrans;
	public Transform cacheTrans {
		get{ return mTrans; }
	}

	public Vector3 lastCachePos;

    void Awake()
    {
        mTrans = this.transform;
		lastCachePos = mTrans.localPosition;
    }

    public void Follow(Transform t)
    {
        target = t;
        followTarget = true;
        SyncTargetPos();
    }

    public void SyncTargetPos()
    {
        if (target != null && mTrans != null)
        {
			lastCachePos = mTrans.localPosition;

            var end = target.position;
            end.x = Mathf.Clamp(end.x, xMin, xMax);
            end.y = Mathf.Clamp(end.y, yMin, yMax);
            end.z = mTrans.position.z;
            mTrans.position = end;
        }
    }

    public void UpdateViewRange(float xMin, float xMax, float yMin, float yMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
        SyncTargetPos();
    }

    void LateUpdate()
    {
		if (target == null || !followTarget) return;
		lastCachePos = mTrans.localPosition;

        Vector3 start = mTrans.position;
        Vector3 end = isSmooth ? Vector3.Lerp(start, target.position, Time.deltaTime * damping) : target.position;

        end.x = Mathf.Clamp(end.x, xMin, xMax);
        end.y = Mathf.Clamp(end.y, yMin, yMax);
        end.z = start.z;

        mTrans.position = end;
    }

    public void Reset()
    {
		if (mTrans != null) {
			SyncTargetPos();
			/*
			Vector3 finlEndPoint = new Vector3(0, 0, mTrans.localPosition.z);
			finlEndPoint.x = Mathf.Clamp(finlEndPoint.x, xMin, xMax);
			finlEndPoint.y = Mathf.Clamp(finlEndPoint.y, yMin, yMax);
			mTrans.localPosition = finlEndPoint;
			*/
		}

        target = null;
        followTarget = false;
    }
}
