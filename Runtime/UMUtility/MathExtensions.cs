using UnityEngine;

namespace UM.Runtime.UMUtility
{
        public static class MathExtensions
        {
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
        }
}