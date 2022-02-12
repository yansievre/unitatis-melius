using DataSystem.Impl;
using UnityEngine;
using Zenject;

namespace DataSystem
{
    [CreateAssetMenu(fileName = "UserDataSystemInstaller", menuName = "Installers/UserDataSystemInstaller")]
    class DataSystemInstaller : ScriptableObjectInstaller<DataSystemInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<UserDataManagerFactory>().ToSelf().AsSingle().Lazy();
        }
    }
}