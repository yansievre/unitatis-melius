namespace UM.Runtime.UMDataSystem.Abstract
{
    public interface IDataInfo
    {
        string DataPath { get; }
        string DataFileName { get; }
        string DataFileExtension { get; }
        string DataFileNameWithoutExtension { get; }
    }
}