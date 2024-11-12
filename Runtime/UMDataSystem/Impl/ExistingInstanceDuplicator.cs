using System.Threading;
using Cysharp.Threading.Tasks;
using UM.Runtime.UMDataSystem.Abstract;

namespace UM.Runtime.UMDataSystem.Impl
{
    public class ExistingInstanceDuplicator<T> : IDataReader<T>
    {
        private readonly ExistingInstanceReader<T> _reader;

        public ExistingInstanceDuplicator(T instance)
        {
#if ODIN_INSPECTOR
            _reader = new ExistingInstanceReader<T>((T) Sirenix.Serialization.SerializationUtility.CreateCopy(instance));
#else
            _reader = new ExistingInstanceReader<T>(UnityEngine.JsonUtility.FromJson<T>(UnityEngine.JsonUtility.ToJson(instance)));
#endif
        }
        
        public UniTask<T> ReadObject(CancellationToken token)
        {
            return _reader.ReadObject(token);
        }

        public UniTask<string> ReadData(CancellationToken token)
        {
            return _reader.ReadData(token);
        }

        public DataState CheckFile()
        {
            return _reader.CheckFile();
        }
    }
}