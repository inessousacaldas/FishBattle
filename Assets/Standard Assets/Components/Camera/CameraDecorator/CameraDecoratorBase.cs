using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDecoratorBase
{
    //返回增量
    public virtual Vector2 AddDistance(Vector2 rawHegiht) { return Vector2.zero; }


    public static Vector2 GetDisantaceAdd(List<CameraDecoratorBase> decoratorList, Vector2 rawDistance)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < decoratorList.Count; i++)
            sum += decoratorList[i].AddDistance(rawDistance);
        return sum;
    }
}
