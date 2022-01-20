using System;

namespace Plugins.UMDynamicEnum.Runtime
{
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple = true,Inherited = false)]
    public class EnumGenerationAttribute : Attribute
    {
        private string _enumName;
        private string _filePath;
        private bool _isBitFlag;

        public EnumGenerationAttribute(string enumName, string filePath, bool isBitFlag = false)
        {
            _enumName = enumName;
            _filePath = filePath;
            _isBitFlag = isBitFlag;
        }

        public bool IsBitFlag => _isBitFlag;

        public string ParentDirectory => _filePath;

        public string EnumName => _enumName;
    }
}
