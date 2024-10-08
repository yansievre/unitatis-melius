#if ZENJECT
using UM.Runtime.UMDataSystem.Impl;
using UnityEngine;
using Zenject;

namespace UM.Runtime.UMDataSystem
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