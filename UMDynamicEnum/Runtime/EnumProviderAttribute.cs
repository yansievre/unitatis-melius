using System;

namespace Plugins.UMDynamicEnum.Runtime
{
 
    
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false,Inherited = false)]
    public class EnumProviderAttribute : Attribute
    {
        private string _enumName;

        public EnumProviderAttribute(string enumName)
        {
            _enumName = enumName;
        }


        public string EnumName => _enumName;
    }
}
