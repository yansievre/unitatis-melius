using System.Threading;
using Cysharp.Threading.Tasks;
using UM.Runtime.UMDataSystem.Abstract;

namespace UM.Runtime.UMDataSystem.Impl
{
    public class GenericDataHandler<T> : IDataHandler<T>
    {
        public static GenericDataHandler<T> CreateInstance(IDataReader<T> dataReaderImpl, IDataWriter<T> dataWriterImpl)
        {
            return new GenericDataHandler<T>(dataReaderImpl, dataWriterImpl);
        }

        private readonly IDataReader<T> _dataReaderImpl;
        private readonly IDataWriter<T> _dataWriterImpl;

        private GenericDataHandler(IDataReader<T> dataReaderImpl, IDataWriter<T> dataWriterImpl)
        {
            _dataReaderImpl = dataReaderImpl;
            _dataWriterImpl = dataWriterImpl;
        }


        public UniTask<T> ReadObject(CancellationToken token)
        {
            return _dataReaderImpl.ReadObject(token);
        }

        public UniTask<string> ReadData(CancellationToken token)
        {
            return _dataReaderImpl.ReadData(token);
        }

        public DataState CheckFile()
        {
            return _dataReaderImpl.CheckFile();
        }

        public UniTask<bool> WriteData(T targetObject, CancellationToken token)
        {
            return _dataWriterImpl.WriteData(targetObject, token);
        }

        public UniTask<bool> WriteData(string serializedData, CancellationToken token)
        {
            return _dataWriterImpl.WriteData(serializedData, token);
        }

        public string DataPath => _dataWriterImpl.DataPath;
        public string DataFileName => _dataWriterImpl.DataFileName;
        public string DataFileExtension => _dataWriterImpl.DataFileExtension;
        public string DataFileNameWithoutExtension => _dataWriterImpl.DataFileNameWithoutExtension;
    }
}