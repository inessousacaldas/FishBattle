using UnityEngine;
using System.Collections;

public class SM_rotateThis : MonoBehaviour
{
    public Vector3 rotationVector;
    private Transform cacheTransform = null;


	void Start ()
    {
        cacheTransform = transform;
	}
	

	void Update () 
    {
        transform.Rotate(rotationVector * Time.deltaTime);
	}
}
