using Unity.Collections;
using Unity.Entities;

namespace Helpers.Logging
{
    public struct LogData
    {
        public FixedString128Bytes Message;
        public bool ShowClientServerPrefix;
        public WorldUnmanaged WorldUnmanaged;
    }
}