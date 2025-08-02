using Unity.Entities;

namespace Data.Spawner
{
    public struct UnitSpawnerData : IComponentData
    {
        public Entity UnitPrefab;
    }
}