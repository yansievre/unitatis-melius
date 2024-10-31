using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;

namespace UM.Runtime.UMFileUtility
{
    public interface IFileWriter
    {
        UniTask<bool> Write(string text, CancellationToken token);
        void RemoveTargetFile();
    }
}