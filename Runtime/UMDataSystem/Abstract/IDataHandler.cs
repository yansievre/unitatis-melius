namespace Plugins.UMDataSystem.Abstract
{
    public interface IDataHandler<T> : IDataReader<T>, IDataWriter<T>
    {
    }
}