using System;
using R3;

namespace UM.Runtime.UMUtility.ReactiveUtility
{
    public static class ObservableExtensions
    {
        public static IDisposable SubscribeBlind<T>(this Observable<T> source, Action onNext) =>
            source.Subscribe(unit => onNext());
    }
}