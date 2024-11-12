using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UM.Runtime.UMDataSystem.Abstract;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UM.Runtime.UMDataSystem.Impl
{
    public class ExistingFromResourceReader<T, TAsset> : IDataReader<T> where TAsset : Object
    {
        private readonly string _assetName;
        private readonly Func<TAsset, T> _getter;
        private readonly bool _copy;

        public ExistingFromResourceReader(string assetName, Func<TAsset, T> getter, bool copy = true)
        {
            _assetName = assetName;
            _getter = getter;
            _copy = copy;
        }
        public UniTask<T> ReadObject(CancellationToken token)
        {
            using(GetReader(out var reader))
            {
                return reader.ReadObject(token);
            }
        }

        public UniTask<string> ReadData(CancellationToken token)
        {
            using(GetReader(out var reader))
            {
                return reader.ReadData(token);
            }
        }

        public DataState CheckFile()
        {
            using(GetReader(out var reader))
            {
                return reader.CheckFile();
            }
        }

        private IDisposable GetReader(out IDataReader<T> reader)
        {
            var src = Resources.Load<TAsset>(_assetName);
            var instance = _getter(src);
            reader = _copy ? new ExistingInstanceDuplicator<T>(instance) : new ExistingInstanceReader<T>(instance);
            return Disposable.Create(() => Resources.UnloadAsset(src));
        }
    }
}