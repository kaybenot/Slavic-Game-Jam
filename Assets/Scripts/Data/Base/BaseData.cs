using Helpers.Base;
using Unity.Entities;
using Unity.NetCode;

namespace Data.Base
{
    public struct BaseData : IComponentData
    {
        public BaseType BaseType;
        [GhostField] public int Health;
    }
}

