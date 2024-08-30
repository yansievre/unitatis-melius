#if ZENJECT
using UnityEngine;
using Zenject;
using Plugins.UMUtility;

[CreateAssetMenu(fileName = "UMUtilityInstaller", menuName = "Installers/UMUtilityInstaller")]
public class UMUtilityInstaller : ScriptableObjectInstaller<UMUtilityInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<UniTaskQueue>().FromMethod(ctx => ctx.Container.Instantiate<UniTaskQueue>()).Lazy();
    }
}
#endif