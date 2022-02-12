using System.Threading;
using Cysharp.Threading.Tasks;

namespace DataSystem.Abstract
{
    
    public interface IDataReader : IDataInfo
    {
        /// <summary>
        /// Reads serialized data
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UniTask<string> ReadData(CancellationToken token);

        bool FileExists();
    }

    public interface IDataReader<T> : IDataReader
    {
        /// <summary>
        /// Reads serialized data and attempts to return it as target type
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UniTask<T> ReadObject(CancellationToken token);
    }
}