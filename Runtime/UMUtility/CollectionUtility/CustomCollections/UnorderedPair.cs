using System;
using System.Collections.Generic;
using Unity.Burst;

namespace UM.Runtime.UMUtility.CollectionUtility.CustomCollections
{
    public struct UnorderedPair
    {
        public static UnorderedPair<T> Create<T>(T first, T second) => new UnorderedPair<T>(first, second);
    }

    [Serializable]
    public struct UnorderedPair<T> : IEquatable<UnorderedPair<T>>
    {
        public static UnorderedPair<T> Empty => new UnorderedPair<T>();
        public static UnorderedPair<T> Create(T first, T second) => new UnorderedPair<T>(first, second);
        public T first;
        public T second;

        public UnorderedPair(T first, T second)
        {
            this.first = first;
            this.second = second;
        }

        public bool Contains(T value)
        {
            return first.Equals(value) || second.Equals(value);
        }
        
        [BurstDiscard]
        public override bool Equals(object obj)
        {
            if (obj is UnorderedPair<T> pair)
            {
                return (first.Equals(pair.first) && second.Equals(pair.second)) ||
                       (first.Equals(pair.second) && second.Equals(pair.first));
            }

            return false;
        }

        public override int GetHashCode()
        {
            return first.GetHashCode() ^ second.GetHashCode();
        }
        
        public static bool operator ==(UnorderedPair<T> a, UnorderedPair<T> b)
        {
            return a.Equals(b);
        }
        
        public static bool operator !=(UnorderedPair<T> a, UnorderedPair<T> b)
        {
            return !a.Equals(b);
        }

        public bool Equals(UnorderedPair<T> other)
        {
            return EqualityComparer<T>.Default.Equals(first, other.first) && EqualityComparer<T>.Default.Equals(second, other.second);
        }
    }
}