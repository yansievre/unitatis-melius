using System;

namespace UM.Runtime.UMRefresh
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnRefreshAttribute : Attribute
    {
        public bool IsPreRefresh { get; }
        public OnRefreshAttribute(bool isPreRefresh = false)
        {
            IsPreRefresh = isPreRefresh;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class OnPreRefreshAttribute : OnRefreshAttribute
    {
        public OnPreRefreshAttribute() : base(true)
        {
        }
    }
}