using UnityEngine;
using System.Collections;


/// <summary>
/// 对一些框架导出js接口的，或者转js不支持的，封装在这里
/// 建议仅对第三方sdk或者不方便修改源码的封装在此
/// 能修改源码的还是修改源码吧
/// </summary>
public static class JSBindingFixHelper
{
    #region UnityEngine
    public static Collider2D[] Physics2D_OverlapPointNonAlloc(Vector2 point,out int resultCount, int colliderCount)
    {
        var colliders = new Collider2D[colliderCount];
        resultCount = Physics2D.OverlapPointNonAlloc(point, colliders);
        return colliders;
    }
    #endregion


    #region A*
    public static Pathfinding.Int3 ToInt3(this Vector3 ob)
    {
        return new Pathfinding.Int3(
            (int)System.Math.Round(ob.x * Pathfinding.Int3.FloatPrecision),
            (int)System.Math.Round(ob.y * Pathfinding.Int3.FloatPrecision),
            (int)System.Math.Round(ob.z * Pathfinding.Int3.FloatPrecision)
        );
    }

    public static Vector3 ToVector3(this Pathfinding.Int3 ob)
    {
        return new Vector3(ob.x * Pathfinding.Int3.PrecisionFactor, ob.y * Pathfinding.Int3.PrecisionFactor, ob.z * Pathfinding.Int3.PrecisionFactor);
    }
    #endregion
}
