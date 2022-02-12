using System.Collections.Generic;
using DataSystem.Abstract;
using Zenject;

namespace DataSystem.Impl
{
   
    public class UserDataManagerFactory : IFactory
    {
        readonly DiContainer _container;
        readonly List<UnityEngine.Object> _prefabs;

        public UserDataManagerFactory(
            List<UnityEngine.Object> prefabs,
            DiContainer container)
        {
            _container = container;
            _prefabs = prefabs;
        }

        public IStateDataManager<T> Create<T>(IDataHandler<T> dataHandler)
        {
            return _container.Instantiate<StateDataManager<T>>(new []{dataHandler});
        }
    }
}