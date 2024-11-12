using System.Threading;
using Cysharp.Threading.Tasks;
using UM.Runtime.UMDataSystem.Abstract;

namespace UM.Runtime.UMDataSystem.Impl
{
    public class ExistingInstanceReader<T> : IDataReader<T>
    {
        private readonly T _instance;

        public ExistingInstanceReader(T instance)
        {
            _instance = instance;
        }
        public UniTask<T> ReadObject(CancellationToken token)
        {
            return UniTask.FromResult(_instance);
        }

        public UniTask<string> ReadData(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public DataState CheckFile()
        {
            return _instance != null ? DataState.Found : DataState.NotFound;
        }
    }
}