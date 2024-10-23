using System;
using System.Buffers;

namespace UM.Runtime.UMUtility.CollectionUtility
{
    public static class ArrayUtility
    {

        public static void ShiftArray<T>(ref T[] array, int startIndex)
        {
            startIndex %= array.Length;

            if (startIndex == 0)
                return;
            var cache = ArrayPool<T>.Shared.Rent(array.Length);
            Array.Copy(array, cache, array.Length);
            Array.Copy(cache, startIndex, array, 0, array.Length - startIndex);
            Array.Copy(cache, 0, array, array.Length - startIndex, startIndex);
        }


        public static T[] AsArray<T>(this T target)
        {
            return new[] { target };   
        }
    }
}