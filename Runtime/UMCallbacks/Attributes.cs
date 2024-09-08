using System;

namespace UM.Runtime.UMRefresh
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Attributes : Attribute
    {
        public bool IsPreRefresh { get; }
        public Attributes(bool isPreRefresh = false)
        {
            IsPreRefresh = isPreRefresh;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class OnPreRefreshAttribute : Attributes
    {
        public OnPreRefreshAttribute() : base(true)
        {
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class OnDeletedAttribute : Attribute
    {
        public OnDeletedAttribute()
        {
        }
    }
}