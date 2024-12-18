﻿using System.Threading;
using Cysharp.Threading.Tasks;

namespace UM.Runtime.UMDataSystem.Abstract
{
    public interface IStateDataManager<T>
    {
        T ActiveInstance { get; }
        void ActivateStateInstance(T state);
        UniTask<T> LoadStateData(CancellationToken token);
        UniTask<bool> LoadStateDataAndActivate(CancellationToken token);
        UniTask<bool> SaveStateData(CancellationToken token);
        bool SaveExists();
    }
}