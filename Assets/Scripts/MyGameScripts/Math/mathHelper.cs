using UnityEngine;
public static class MathHelper
{
    public static bool Contains(this Bounds boundsA, Bounds boundsB)
    {
        Vector3 center = boundsB.center;
        Vector3 extents = boundsB.extents;
        Vector3 NW = new Vector3(center.x - extents.x, 0f, center.y + extents.y);
        Vector3 SE = new Vector3(center.x + extents.x, 0f, center.y - extents.y);
        return boundsA.Contains(boundsB.min)
            && boundsA.Contains(boundsB.max)
            && boundsA.Contains(NW)
            && boundsA.Contains(SE);
    }

    public static Bounds Bounds2D(Vector2 center, Vector2 size)
    {
        Bounds bounds = new Bounds(new Vector3(center.x, 0f, center.y), new Vector3(size.x, 0f, size.y));
        return bounds;
    }
    public static Bounds Bounds2D(float centerX, float centerY, float width, float height)
    {
        Bounds bounds = new Bounds(new Vector3(centerX, 0f, centerY), new Vector3(width, 0f, height));
        return bounds;
    }
    public static float x2D(this Bounds bounds)
    {
        return bounds.center.x;
    }
    public static float y2D(this Bounds bounds)
    {
        return bounds.center.z;
    }
    public static float width2D(this Bounds bounds)
    {
        return bounds.size.x;
    }
    public static float height2D(this Bounds bounds)
    {
        return bounds.size.z;
    }

    /// <summary>
    /// a,b,c 为圆上顺时针3点
    /// </summary>
    public static Vector3 Slerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 axis = new Plane(a, b, c).normal;
        Matrix4x4 rotateMatrix4X4 = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, axis), Vector3.one);
        Ray rayAB = new Ray((a + b) / 2, rotateMatrix4X4.MultiplyPoint3x4(b - a));
        Ray rayBC = new Ray((c + b) / 2, rotateMatrix4X4.MultiplyPoint3x4(c - b));

        Vector3 circleCenter;
        if (rayAB.Intersect(rayBC, out circleCenter))
        {
            return Vector3.Slerp(a - circleCenter, c - circleCenter, t) + circleCenter;
        }
        return Vector3.zero;
    }

    public static bool Intersect(this Ray r1, Ray r2, out Vector3 intersection)
    {
        Vector3 lineVec3 = r2.origin - r1.origin;
        Vector3 crossVec1and2 = Vector3.Cross(r1.direction, r2.direction);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, r2.direction);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = r1.origin + (r1.direction * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public struct TransformHelper
    {
        public Vector3 pos;
        public Quaternion rotation;

        public TransformHelper(Transform transform)
        {
            rotation = transform.rotation;
            pos = transform.position;
        }

        public Vector3 forward
        {
            get
            {
                return RotateVector3(Vector3.forward);
            }
        }

        public Vector3 Up
        {
            get
            {
                return RotateVector3(Vector3.up);
            }
        }
        private Vector3 RotateVector3(Vector3 vector3)
        {
            Vector3 newVector3 = vector3;
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            newVector3 = matrix4X4.MultiplyPoint3x4(pos);
            return newVector3;
        }
    }
}
