#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
[AddComponentMenu("美术/SceneGoInfo")]
public class SceneGoInfoComponent : MonoBehaviour
{
    public int treeLevel;
    public Bounds bounds;
    public static bool showAll;
    [NonSerialized]
    public bool select;

    private Vector3 _tempPos;

    private void Start()
    {
        _tempPos = transform.position;
    }

    void Update()
    {
        if (_tempPos != transform.position)
        {
            Debug.LogError("Transform 修改过，请重新导出场景");    
            Undo.DestroyObjectImmediate(this);
        }

        if (showAll || select)
        {
            //以世界空间Z轴为左手螺旋定律正方向
            //从下到上 顺时针编码 0点为左下角
            Vector3[] vector3s = new Vector3[8];
            CaculatePoin(this, vector3s);
            DrawLine(vector3s);
        }
    }


    private void CaculatePoin(SceneGoInfoComponent t, Vector3[] vector3s)
    {
        for (int i = 0; i < vector3s.Length; i++)
        {
            vector3s[i] = t.bounds.center;
        }

        vector3s[0] = CaculatePoinHeler(vector3s[0], t.bounds.extents, -1, -1, -1);
        vector3s[1] = CaculatePoinHeler(vector3s[1], t.bounds.extents, -1, +1, -1);
        vector3s[2] = CaculatePoinHeler(vector3s[2], t.bounds.extents, +1, +1, -1);
        vector3s[3] = CaculatePoinHeler(vector3s[3], t.bounds.extents, +1, -1, -1);

        vector3s[4] = CaculatePoinHeler(vector3s[4], t.bounds.extents, -1, -1, +1);
        vector3s[5] = CaculatePoinHeler(vector3s[5], t.bounds.extents, -1, +1, +1);
        vector3s[6] = CaculatePoinHeler(vector3s[6], t.bounds.extents, +1, +1, +1);
        vector3s[7] = CaculatePoinHeler(vector3s[7], t.bounds.extents, +1, -1, +1);

    }
    private Vector3 CaculatePoinHeler(Vector3 vector3, Vector3 extents, float _x, float _y, float _z)
    {
        //float _x = Mathf.Sign(x);
        //float _y = Mathf.Sign(y);
        //float _z = Mathf.Sign(z);
        vector3.x += _x * extents.x;
        vector3.y += _y * extents.y;
        vector3.z += _z * extents.z;
        return vector3;
    }
    static readonly int[] intArray = new[]
        {
            0, 1, 1, 2, 2, 3, 3, 0,
            4, 5, 5, 6, 6, 7, 7, 4,
            0, 0 + 4,
            1, 1 + 4,
            2, 2 + 4,
            3, 3 + 4
        };
    private void DrawLine(Vector3[] vector3s)
    {
        for (int i = 0; i < intArray.Length - 1; i += 2)
        {
            Debug.DrawLine(vector3s[intArray[i]], vector3s[intArray[i + 1]], new Color(145f, 244f, 139f, 210f) / 255f);
        }
    }
}
#endif