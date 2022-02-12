namespace DataSystem.Abstract
{
    public interface IDataHandler<T> : IDataReader<T>, IDataWriter<T>
    {
    }
}