#if ZENJECT
using UnityEngine;
using Zenject;

namespace UM.Runtime.UMDebug
{
    [CreateAssetMenu(fileName = "UMDebugSettingsInstaller", menuName = "UMDebug/UMDebugSettingsInstaller")]
    internal class UMDebugInstaller : ScriptableObjectInstaller<UMDebugInstaller>
    {
        public override void InstallBindings()
        {
            var instance = UMDebugFinder.Instance;
            Container.BindInterfacesTo<UMDebugSettings>().AsSingle().WithArguments(instance).Lazy();
        }
    }
}
#endif