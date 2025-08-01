using Unity.Entities;

namespace Data.Spawner
{
    public struct PlayerDataSpawnerData : IComponentData
    {
        public Entity PlayerDataPrefab;
    }
}