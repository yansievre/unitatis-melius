using System;

namespace UM.Runtime.UMDynamicEnum
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
