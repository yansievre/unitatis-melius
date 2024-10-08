using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;

namespace UM.Runtime.UMUtility
{
    public static class JobsExtensions
    {
        public static JobHandle CombineDependencies(this IEnumerable<JobHandle> jobHandles)
        {
            var arr = new NativeArray<JobHandle>(jobHandles.ToArray(), Allocator.TempJob);
            var combined = JobHandle.CombineDependencies(arr);
            arr.Dispose();
            return combined;
        }
    }
}