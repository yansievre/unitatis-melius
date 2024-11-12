using System.Threading;
using Cysharp.Threading.Tasks;

namespace UM.Runtime.UMDataSystem.Abstract
{
  
    public interface IDataWriter
    {
        /// <summary>
        /// Reads serialized data
        /// </summary>
        /// <param name="serializedData"></param>
        /// <param name="token"></param>
        /// <returns>Success</returns>
        UniTask<bool> WriteData(string serializedData, CancellationToken token);
    }

    public interface IDataWriter<in T> : IDataWriter
    {
        /// <summary>
        /// Reads serialized data and attempts to return it as target type
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        UniTask<bool> WriteData(T targetObject, CancellationToken token);
    }
}