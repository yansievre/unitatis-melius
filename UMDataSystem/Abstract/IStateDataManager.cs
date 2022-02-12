using System.Threading;
using Cysharp.Threading.Tasks;

namespace Plugins.UMDataSystem.Abstract
{
    public interface IStateDataManager<T>
    {
        
        void ActivateStateInstance(T state);
        UniTask<T> LoadStateData(CancellationToken token);
        UniTask<bool> LoadStateDataAndActivate(CancellationToken token);
        UniTask<bool> SaveStateData(CancellationToken token);
    }
}