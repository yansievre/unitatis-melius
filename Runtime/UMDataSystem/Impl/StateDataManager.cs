using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UM.Runtime.UMDataSystem.Abstract;
using UM.Runtime.UMLogger.Interfaces;

namespace UM.Runtime.UMDataSystem.Impl
{
    internal class StateDataManager<T> : IStateDataManager<T>
    {
        private readonly IDataWriter<T> _dataWriter;
        private readonly IDataReader<T>[] _dataReaders;
        private readonly IUMLogger _logger;
        private T _instance;

        public StateDataManager(IDataReader<T>[] readers, IDataWriter<T> writer, IUMLogger logger)
        {
            _dataWriter = writer;
            _dataReaders = readers;
            _logger = logger;
        }
        public StateDataManager(IDataHandler<T> dataHandler,IDataReader<T>[] alternativeReaders, IUMLogger logger)
        {
            _dataWriter = dataHandler;
            _dataReaders = alternativeReaders.Prepend(dataHandler).ToArray();
            _logger = logger;
        }

        public T ActiveInstance => _instance;

        public void ActivateStateInstance(T state)
        {
            if (_instance != null)
            {
                _logger.LogWarning("Replacing previous instance");
            }
            _instance = state;
        }

        public async UniTask<T> LoadStateData(CancellationToken token)
        {
            foreach (var dataReader in _dataReaders)
            {
                var fileState = dataReader.CheckFile();
                switch (fileState)
                {
                    case DataState.NotFound:
                        continue;
                    case DataState.Unknown:
                    case DataState.Found:
                        try
                        {
                            return await dataReader.ReadObject(token);
                        }
                        catch (Exception e)
                        {
                           _logger.LogWarning($"Failed to read data with {dataReader.GetType().Name}"); 
                        }
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            throw new DataSystemException("Failed to read data from any source");
        }

        public async UniTask<bool> LoadStateDataAndActivate(CancellationToken token)
        {
            try
            {
                var stateData = await LoadStateData(token);
                ActivateStateInstance(stateData);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return false;
            }

            return true;
        }

        public UniTask<bool> SaveStateData(CancellationToken token)
        {
            return _dataWriter.WriteData(_instance, token);
        }
    }
}