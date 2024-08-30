namespace UM.Runtime.UMDynamicEnum
{
    public readonly struct EnumGenInfo
    {
        public readonly string ValueName;
        public readonly int Value;

        public EnumGenInfo(string valueName, int value)
        {
            ValueName = valueName;
            Value = value;
        }
    }

}