using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Helpers.Logging
{
    [BurstCompile]
    public struct LogData
    {
        public FixedString128Bytes Message;
        public bool ShowClientServerPrefix;
        public WorldUnmanaged WorldUnmanaged;
    }
}