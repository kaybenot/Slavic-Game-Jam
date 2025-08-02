using Helpers.Base;
using Unity.NetCode;

namespace Data.RPC
{
    public struct RequestUnitSpawnRpc : IRpcCommand
    {
        public UnitType UnitType;
        public BaseLane Lane;
    }
}