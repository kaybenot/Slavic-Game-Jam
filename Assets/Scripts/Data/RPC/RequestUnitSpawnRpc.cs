using Helpers.Base;
using Unity.Entities;
using Unity.NetCode;

namespace Data.RPC
{
    public struct RequestUnitSpawnRpc : IRpcCommand
    {
        public BaseLane Lane;
    }
}