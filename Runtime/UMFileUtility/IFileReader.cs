using System.Threading;
using Cysharp.Threading.Tasks;

namespace UM.Runtime.UMFileUtility
{
    public interface IFileReader
    {
        UniTask<string> Read(CancellationToken token);
    }
}