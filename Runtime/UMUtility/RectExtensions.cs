using Unity.Mathematics;
using UnityEngine;

namespace UM.Runtime.UMUtility
{
    public static class RectExtensions
    {
        public static float2 Clamp(this Rect rect, float2 point)
        {
            return new float2(
                math.clamp(point.x, rect.xMin, rect.xMax),
                math.clamp(point.y, rect.yMin, rect.yMax)
            );
        }
        
        public static float2 Clamp(this float2 point, Rect rect)
        {
            return Clamp(rect, point);
        }
        
        public static Vector2 Clamp(this Rect rect, Vector2 point)
        {
            return Clamp(rect, (float2) point);
        }
        
        public static float2 Clamp(this Vector2 point, Rect rect)
        {
            return Clamp(rect, point);
        }
    }
}