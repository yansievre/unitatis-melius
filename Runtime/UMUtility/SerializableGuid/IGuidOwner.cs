using System;

namespace UM.Runtime.UMUtility.SerializableGuid
{
    public interface IGuidOwner
    {
        Guid Guid { get; }
    }
}