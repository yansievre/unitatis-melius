using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DataSystem.Abstract;
using UMLogger.Plugins.UMLogger.Interfaces;

namespace DataSystem.Impl
{
    internal class StateDataManager<T> : IStateDataManager<T>
    {
        private readonly IDataHandler<T> _dataHandler;
        private readonly IUMLogger _logger;
        private T _instance;

        public StateDataManager(IDataHandler<T> dataHandler, IUMLogger logger)
        {
            _dataHandler = dataHandler;
            _logger = logger;
        }

        public void ActivateStateInstance(T state)
        {
            if (_instance != null)
            {
                _logger.LogWarning("Replacing previous instance");
            }
            _instance = state;
        }

        public UniTask<T> LoadStateData(CancellationToken token)
        {
            return _dataHandler.ReadObject(token);
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
            return _dataHandler.WriteData(_instance, token);
        }
    }
}