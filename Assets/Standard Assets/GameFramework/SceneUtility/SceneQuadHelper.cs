using System.Runtime.CompilerServices;
using UnityEngine;

namespace SceneUtility
{
    public static class SceneQuadHelper
    {
        public static bool Contains2D(this Bounds boundsA, Bounds boundsB)
        {
            Vector3 centerA = boundsA.center;
            Vector3 sizeA = boundsA.size;
            centerA.y = 0;
            sizeA.y = 0;
            boundsA = new Bounds(centerA, sizeA);

            Vector3 center = boundsB.center;
            Vector3 extents = boundsB.extents;
            return boundsA.Contains(new Vector3(center.x - extents.x, 0f, center.z + extents.z))
                   && boundsA.Contains(new Vector3(center.x + extents.x, 0f, center.z + extents.z))
                   && boundsA.Contains(new Vector3(center.x + extents.x, 0f, center.z - extents.z))
                   && boundsA.Contains(new Vector3(center.x - extents.x, 0f, center.z - extents.z));
        }

        public static void EncapsulateY(ref Bounds boundsA, Bounds boundsB)
        {
            Vector3 centerA = boundsA.center;
            centerA.y = boundsB.max.y;
            boundsA.Encapsulate(centerA);
            centerA.y = boundsB.min.y;
            boundsA.Encapsulate(centerA);
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

        public static float x(this Bounds bounds)
        {
            return bounds.center.x;
        }

        public static float z(this Bounds bounds)
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

        public static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near,
            float far)
        {
            float x = 2.0F * near / (right - left);
            float y = 2.0F * near / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);
            float e = -1.0F;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;
        }
    }
}