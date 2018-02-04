using UnityEngine;

/// <summary>
/// Scene helper.主要用于设置场景效果
/// </summary>
using System;

public static class SceneHelper
{
    #region Scene Stand Position

    public static Vector3 GetSceneStandPosition(Vector3 sourcePos, Vector3 defaultPos)
    {
        if (AstarPath.active == null)
            return defaultPos;
        Pathfinding.NNInfo nNInfo = AstarPath.active.GetNearest(sourcePos);
        return nNInfo.clampedPosition != Vector3.zero ? nNInfo.clampedPosition : defaultPos;
    }

    public static bool IsCanWalkScope(Vector3 pos)
    {
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(pos.x, 50, pos.z), new Vector3(0, -1, 0));
        if (!Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Terrain")))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

    public static void ToggleSceneEffect(bool toggle)
    {
        GameObject sceneEffect = LayerManager.Instance.SceneEffect;
        if (sceneEffect != null)
        {
            sceneEffect.SetActive(toggle);
        }
    }

    #region CheckAtBattleScope

    static public bool CheckAtBattleScope(Vector3 point3D)
    {
        Vector2[] polygons = new Vector2[4];
        polygons[0] = new Vector2(3.4f, 7.2f);
        polygons[1] = new Vector2(12.6f, -1f);
        polygons[2] = new Vector2(21.5f, 9f);
        polygons[3] = new Vector2(14.5f, 17f);

        //		Vector2 centerPoint2D = new Vector2(14.5f, 1.65f);
        Vector2 point2D = new Vector2(point3D.x, point3D.z);
        //		float distance = Vector2.Distance(centerPoint2D, point2D);
        //		return distance <= 5f;

        return IsPointInPolygon(point2D, polygons);
    }

    //7.87 4.46
    //18 8
    //21 -0.5
    //11.1  -4.04
    static private bool IsPointInPolygon(Vector2 p, Vector2[] polygon)
    {
        float minX = polygon[0].x;
        float maxX = polygon[0].x;
        float minY = polygon[0].y;
        float maxY = polygon[0].y;
        for (int i = 1; i < polygon.Length; i++)
        {
            Vector2 q = polygon[i];
            minX = Math.Min(q.x, minX);
            maxX = Math.Max(q.x, maxX);
            minY = Math.Min(q.y, minY);
            maxY = Math.Max(q.y, maxY);
        }

        if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY)
        {
            return false;
        }

        // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if ((polygon[i].y > p.y) != (polygon[j].y > p.y) &&
                p.x < (polygon[j].x - polygon[i].x)*(p.y - polygon[i].y)/(polygon[j].y - polygon[i].y) + polygon[i].x)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    #endregion

    #region CheckAtWeddingScope

    public static bool CheckAtWeddingScope(Vector3 point3D)
    {
        Vector2[] polygons = new Vector2[4];
        polygons[0] = new Vector2(27.0f, 24.1f);
        polygons[1] = new Vector2(30.3f, 27.4f);
        polygons[2] = new Vector2(34.0f, 24.4f);
        polygons[3] = new Vector2(29.9f, 20.3f);

        Vector2 point2D = new Vector2(point3D.x, point3D.z);
        return IsPointInPolygon(point2D, polygons);
    }

    #endregion

    #region CheckAtBridalSedanScope

    public static bool CheckAtBridalSedanScope(Vector3 point3D)
    {
        Vector2[] polygons = new Vector2[4];
        polygons[0] = new Vector2(13.0f, -53.0f);
        polygons[1] = new Vector2(51.0f, 0f);
        polygons[2] = new Vector2(111f, -68.0f);
        polygons[3] = new Vector2(70f, -115f);

        Vector2 point2D = new Vector2(point3D.x, point3D.z);
        return IsPointInPolygon(point2D, polygons);
    }

    #endregion

	#region 判断A点是否在B点指定范围内
	public static bool CheckAtTwoPointInRang(Vector3 aPoint, Vector3 bPoint, int radius) {
		Vector2 a = new Vector2 (aPoint.x, aPoint.z);
		Vector2 b = new Vector2 (bPoint.x, bPoint.z);

		float distance = Vector2.Distance(a, b);
		return distance < radius;
	}
	#endregion

}
