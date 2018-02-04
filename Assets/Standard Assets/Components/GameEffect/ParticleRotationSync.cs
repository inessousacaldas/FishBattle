using UnityEngine;

public class ParticleRotationSync : MonoBehaviour
{
    #region RotateType enum

    public enum RotateType
    {
        x,
        y,
        z
    }

    #endregion

    private Vector3 _lastEulerAngles = Vector3.zero;

    private ParticleSystem[] _psList;
    public RotateType rotateType = RotateType.y;

    public Transform target;
    // Use this for initialization
    private void Start()
    {
        _psList = GetComponentsInChildren<ParticleSystem>(true);
        if (_psList == null || _psList.Length == 0)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            Vector3 curEulerAngles = target.localEulerAngles;
            if (_lastEulerAngles != curEulerAngles)
            {
                _lastEulerAngles = curEulerAngles;
                for (int i = 0; i < _psList.Length; i++)
                {
                    if (rotateType == RotateType.x)
                    {
                        _psList[i].startRotation = curEulerAngles.x * Mathf.Deg2Rad;
                    }
                    else if (rotateType == RotateType.y)
                    {
                        _psList[i].startRotation = curEulerAngles.y * Mathf.Deg2Rad;
                    }
                    else
                    {
                        _psList[i].startRotation = curEulerAngles.z * Mathf.Deg2Rad;
                    }
                }
            }
        }
    }
}