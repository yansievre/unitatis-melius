using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;
using Progress = UM.Runtime.UMUtility.ProgressTracker.Progress;

namespace UM.Runtime.UMUtility.CollectionUtility
{
    public static class EnumerableUtility
    {
        private const int K_YieldEvery = 1000/30;
        /// <summary>
        /// Returns true if you should yield
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<(T item, bool yieldFrame)> EnumerateStable<T>(this IEnumerable<T> enumerable)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var elapsed_time = 0L;
            List<T> list = new List<T>();
            bool yieldFrame = false;
            
            foreach (var item in enumerable)
            {
                yieldFrame = false;
                if (elapsed_time > K_YieldEvery)
                {
                    elapsed_time -= K_YieldEvery;
                    yieldFrame = true;
                }
                stopwatch.Start();
                yield return (item, yieldFrame);
                stopwatch.Stop();
                elapsed_time += stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
            }
        }

        public static async UniTask WithProgressBar(this IEnumerable<Progress> progressEnumerable, string title)
        {
            try
            {
                foreach (var stableData in progressEnumerable.EnumerateStable())
                {
                    UnityEditor.EditorUtility.DisplayProgressBar(title, stableData.item.description,
                        stableData.item.progress);
                    if (stableData.yieldFrame)
                        await UniTask.Yield();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                UnityEditor.EditorUtility.DisplayDialog(title, "An exception occured during the operation", "ok");
            }
            UnityEditor.EditorUtility.ClearProgressBar();
        }

        public static IEnumerable WithProgressBarSync(this IEnumerable<Progress> progressEnumerable, string title)
        {
            try
            {
                foreach (var stableData in progressEnumerable)
                {
                    UnityEditor.EditorUtility.DisplayProgressBar(title, stableData.description, stableData.progress);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                UnityEditor.EditorUtility.DisplayDialog(title, "An exception occured during the operation", "ok");
                yield break;
            }
            UnityEditor.EditorUtility.ClearProgressBar();
        }
        
        public static IEnumerable<(T first, T second)> Pairwise<T>(this IEnumerable<T> enumerable, bool loop = false)
        {
            T first = default;
            T previous;
            T current = default;
            var i = 0;

            foreach (var trg in enumerable)
            {
                previous = current;
                current = trg;

                if (i == 0)
                    first = current;

                if (previous != null && i != 0)
                    yield return (previous, current);
                i++;
            }

            if (loop)
                yield return (current, first);
        }

        public static IEnumerable<(T first, K second)> SideBySide<T, K>(this IEnumerable<T> enumerable,
            IEnumerable<K> other)
        {
            var enumerator1 = enumerable.GetEnumerator();
            var enumerator2 = other.GetEnumerator();

            while (true)
            {
                if (!enumerator1.MoveNext())
                    break;
                if (!enumerator2.MoveNext())
                    break;

                yield return (enumerator1.Current, enumerator2.Current);
            }

            enumerator1.Dispose();
            enumerator2.Dispose();
        }
        public static IEnumerable<T> EndWithFirstElement<T>(this IEnumerable<T> enumerable)
        {
            var enumerator1 = enumerable.GetEnumerator();
            var hasFirst = false;
            T first = default;

            while (enumerator1.MoveNext())
            {
                if (!hasFirst)
                {
                    first = enumerator1.Current;
                    hasFirst = true;
                }

                yield return enumerator1.Current;
            }

            if (hasFirst)
                yield return first;

            enumerator1.Dispose();
        }
        
           /// <summary>
        /// Does not force execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
                yield return item;
            }
        }
        
        /// <summary>
        /// Does not force execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T, int> action)
        {
            int num = 0;
            foreach (T item in enumeration)
            {
                action(item, num++);
                yield return item;
            }
        }
        
        public static IEnumerable<T> WhereTrue<T>(this IEnumerable<T> enumeration)
        {
            return enumeration.Where(x => Convert.ToBoolean(x));
        }
        
        public static T FirstOrValue<T>(this IEnumerable<T> enumeration, T value)
        {
            foreach (T item in enumeration)
            {
                return item;
            }

            return value;
        }

        public static (T, T) MinMax<T>(this IEnumerable<T> enumeration) where T : IComparable
        {
            T min = default;
            T max = default;
            bool first = true;
            foreach (T item in enumeration)
            {
                if (first)
                {
                    min = item;
                    max = item;
                    first = false;
                }
                else
                {
                    if (item.CompareTo(min) < 0)
                        min = item;
                    if (item.CompareTo(max) > 0)
                        max = item;
                }
            }

            return (min, max);
        }
        
        public delegate bool SelectWhereDelegate<TElement, TSelect>(TElement value, out TSelect selected);
        /// <summary>
        /// Does not force execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSelect> SelectWhere<TElement, TSelect>(this IEnumerable<TElement> enumeration, SelectWhereDelegate<TElement, TSelect> selector)
        {
            foreach (TElement item in enumeration)
            {
                if(selector(item, out var selected))
                    yield return selected;
            }
        }
        

        public delegate uint HashDelegate<TElement>(TElement value);
        public static IEnumerable<TValue> Distinct<TValue>(this IEnumerable<TValue> enumeration, HashDelegate<TValue> hash)
        {
            var set = new HashSet<uint>();

            foreach (var value in enumeration)
            {
                if (set.Add(hash(value)))
                    yield return value;
            }
        }
        
        /// <summary>
        /// Does not force execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<(T val, int index)> WithIndex<T>(this IEnumerable<T> enumeration)
        {
            int num = 0;
            foreach (T item in enumeration)
            {
                yield return (item, num++);
            }
        }

        /// <summary>
        /// Forces execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value)
        {
            return IndexOf<T>(enumeration, value, EqualityComparer<T>.Default.Equals);
        }

        public delegate bool EqualityDelegate<TElement>(TElement value1, TElement value2);
        /// <summary>
        /// Forces execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="value"></param>
        /// <param name="equality"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, EqualityDelegate<T> equality)
        {
            int num = 0;
            foreach (T item in enumeration)
            {
                if(equality(item, value))
                    return num;
                num++;
            }

            return -1;
        }        
        /// <summary>
        /// Does not force execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="offset"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<(T val, int index)> WithIndexOffset<T>(this IEnumerable<T> enumeration, int offset)
        {
            int num = 0;
            foreach (T item in enumeration)
            {
                yield return (item, offset+num++);
            }
        }

        public static T AddToCollection<T>(this T item, ICollection<T> collection)
        {
            collection.Add(item);

            return item;
        }

    
        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }
        /// <summary>
        /// Forces execution
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, HashDelegate<T> hash)
        {
            var valueHash = hash(value);
            int num = 0;
            foreach (T item in enumeration)
            {
                if(valueHash == hash(item))
                    return num;
                num++;
            }

            return -1;
        }
        public static (TSource, TSource) MinMax<T, TSource>(this IEnumerable<TSource> enumeration, Func<TSource, T> getter) where T : IComparable
        {
            TSource min = default;
            T minVal = default;
            TSource max = default;
            T maxVal = default;
            bool first = true;
            foreach (TSource item in enumeration)
            {
                if (first)
                {
                    min = item;
                    minVal = getter(item);
                    max = item;
                    maxVal = getter(item);
                    first = false;
                }
                else
                {
                    if (getter(item).CompareTo(minVal) < 0)
                    {
                        min = item;
                        minVal = getter(item);
                    }
                    if (getter(item).CompareTo(maxVal) > 0)
                    {
                        max = item;
                        maxVal = getter(item);
                    }
                }
            }

            return (min, max);
        }
    }
}