using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace UM.Runtime.UMUtility.MathUtility
{ 
    public static class MathExtensions
    {
        public const float K_Epsilon = 0.001f;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 OnGroundPlane(this float3 vector)
        {
            return new float2(vector.x, vector.z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 OnGroundPlane(this Vector3 vector)
        {
            return new float2(vector.x, vector.z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FromGroundPlane(this float2 vector)
        {
            return new float3(vector.x, 0, vector.y);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 FromGroundPlane(this Vector2 vector)
        {
            return new float3(vector.x, 0, vector.y);
        }
        
        public static (float min, float max) GetMinMax(params float[] values)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            float k;
            for (int i = 0; i < values.Length; i++)
            {
                k = values[i];
                if (k < min)
                    min = k;
                if (k > max)
                    max = k;
            }

            return (min, max);
        }
        /// <summary>
        /// Remaps the value in outMin-outMax range from 0f-1f
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outMin"></param>
        /// <param name="outMax"></param>
        /// <returns></returns>
        public static float Remap(this float value, float outMin, float outMax)
        {
            return Remap(value, 0f, 1f, outMin, outMax);
        }

        public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax)
        {
            var normalized = (value - inMin) / (inMax - inMin);

            return Mathf.Lerp(outMin, outMax, normalized);
        }
        
        //[BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AccelerateTowards(this float current, float target, float accelRate, float time)
        {
            if (math.abs(current - target) < K_Epsilon)
                return target;
            var direction = math.sign(target - current);
            var newVal = current + direction * accelRate * time;
            var min = math.min(current, target);
            var max = math.max(current, target);
            return math.clamp(newVal, min, max);
        }
    
        public static Vector2 AccelerateTowards(this Vector2 current, Vector2 target, float accelRate, float time)
        {
            return ((Vector3)current).AccelerateTowards(target, accelRate, time);
        }

        public static bool InRange(this Vector2 range, float value, bool minInclusive = true, bool maxInclusive = true)
        {
            return (minInclusive ? value >= range.x : value > range.x) 
                   && (maxInclusive ? value <= range.y : value < range.y);
        }
        public static bool InRange(this Vector2Int range, int value, bool minInclusive = true, bool maxInclusive = true)
        {
            return InRange(range, (float) value, minInclusive, maxInclusive);
        }

        public static bool InRangeInclusive(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static float2 Slerp(this float2 current, float2 target, float time)
        {
            var magnitudeLerp = math.sqrt(math.lerp(math.lengthsq(current), math.lengthsq(target), time));
            var angleLerp = Vector2.SignedAngle(current, target)*time;
            return math.mul(quaternion.Euler(0, 0, angleLerp), new float3(math.normalize(current), 0)).xy * magnitudeLerp;
        }
 
        public static Vector2 Slerp(this Vector2 current, Vector2 target, float time)
        {
            var magnitudeLerp = Mathf.Lerp(current.magnitude, target.magnitude, time);
            var angleLerp = Vector2.SignedAngle(current, target)*time;
            return Quaternion.Euler(0, 0, angleLerp) * current.normalized * magnitudeLerp;
        }
        
    }
}