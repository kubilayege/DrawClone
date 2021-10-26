using UnityEngine;

namespace Utils
{
    public static class VectorHelper
    {
        public static Vector3 WithX(this Vector3 v, float newX)
        {
            v.x = newX;
            return v;
        }
        
        public static Vector3 WithY(this Vector3 v, float newY)
        {
            v.y = newY;
            return v;
        }
        
        public static Vector3 WithZ(this Vector3 v, float newZ)
        {
            v.z = newZ;
            return v;
        }
        
        public static Vector3 AddX(this Vector3 v, float newX)
        {
            v.x += newX;
            return v;
        }
        
        public static Vector3 AddY(this Vector3 v, float newY)
        {
            v.y += newY;
            return v;
        }
        
        public static Vector3 AddZ(this Vector3 v, float newZ)
        {
            v.z += newZ;
            return v;
        }
        
        public static Vector2 WithY(this Vector2 v, float newY)
        {
            v.y = newY;
            return v;
        }

    }
}