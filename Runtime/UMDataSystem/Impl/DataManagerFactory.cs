using System.Collections.Generic;
using UM.Runtime.UMDataSystem.Abstract;
using Zenject;

namespace UM.Runtime.UMDataSystem.Impl
{
   
    public class DataManagerFactory : IFactory
    {
        readonly DiContainer _container;
        readonly List<UnityEngine.Object> _prefabs;

        public DataManagerFactory(
            List<UnityEngine.Object> prefabs,
            DiContainer container)
        {
            _container = container;
            _prefabs = prefabs;
        }
        
        /// <summary>
        /// Readers are in priority order
        /// </summary>
        /// <param name="readers"></param>
        /// <param name="writer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IStateDataManager<T> Create<T>(IDataReader<T>[] readers, IDataWriter<T> writer)
        {
            return _container.Instantiate<StateDataManager<T>>(new object[]{readers,writer});
        }
        
        /// <summary>
        /// Data handler takes priority. Alternative readers are in priority order.
        /// </summary>
        /// <param name="dataHandler"></param>
        /// <param name="alternativeReaders"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IStateDataManager<T> Create<T>(IDataHandler<T> dataHandler,IDataReader<T>[] alternativeReaders)
        {
            return _container.Instantiate<StateDataManager<T>>(new object[]{dataHandler,alternativeReaders});
        }
    }
}