using System;
using R3;

namespace UM.Runtime.UMUtility.ProgressTracker
{
    public interface IReadableProgress : IProgress<float>
    {
        ReadOnlyReactiveProperty<float> Progress { get;  }
        void Report(int currentContribution);
    }
}