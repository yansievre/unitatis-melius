#if ZENJECT
using Plugins.UMDataSystem.Impl;
using UnityEngine;
using Zenject;

namespace Plugins.UMDataSystem
{
    [CreateAssetMenu(fileName = "UserDataSystemInstaller", menuName = "Installers/UserDataSystemInstaller")]
    class DataSystemInstaller : ScriptableObjectInstaller<DataSystemInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<DataManagerFactory>().ToSelf().AsSingle().Lazy();
        }
    }
}
#endif