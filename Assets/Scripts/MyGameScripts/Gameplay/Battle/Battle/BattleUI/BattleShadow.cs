using UnityEngine;

public class BattleShadow : MonoBehaviour
{
    private Transform followTarget;
    private Transform myTransform;

    public void Setup(Transform target)
    {
        myTransform = transform;
        myTransform.position = new Vector3(0, 0.1f, 0);
        myTransform.eulerAngles = new Vector3(0.0f, 0, 0);

        followTarget = target;
        Update();
    }

    private void Update()
    {
        if (myTransform)
            myTransform.position = new Vector3(followTarget.position.x, 0.1f, followTarget.position.z);
    }
}