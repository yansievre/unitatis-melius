#if ZENJECT
using UnityEngine;
using Zenject;

namespace UM.Runtime.UMUtility
{
    [CreateAssetMenu(fileName = "UMUtilityInstaller", menuName = "Installers/UMUtilityInstaller")]
    public class UMUtilityInstaller : ScriptableObjectInstaller<UMUtilityInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<UniTaskQueue>().FromMethod(ctx => ctx.Container.Instantiate<UniTaskQueue>()).Lazy();
        }
    }
}
#endif