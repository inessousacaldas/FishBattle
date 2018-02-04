using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    private Transform _mTrans;
    public Vector3 rotationSpeed = new Vector3(0f, 1f, 0f);

    // Use this for initialization
    private void Start()
    {
        _mTrans = transform;
    }

    // Update is called once per frame
    private void Update()
    {
        _mTrans.Rotate(Time.timeScale * rotationSpeed);
    }
}