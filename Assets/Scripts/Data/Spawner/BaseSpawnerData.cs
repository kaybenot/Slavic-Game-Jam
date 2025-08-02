using Helpers.Base;
using Unity.Entities;

namespace Data.Spawner
{
    public struct BaseSpawnerData : IComponentData
    {
        public Entity BasePrefab;
        public BaseType BaseType;
    }
}