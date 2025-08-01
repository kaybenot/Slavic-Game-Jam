using Data.Spawner;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Spawner
{
    public class PlayerDataSpawnerAuthoring : MonoBehaviour
    {
        public GameObject PlayerDataPrefab;
        
        private class PlayerDataSpawnerAuthoringBaker : Baker<PlayerDataSpawnerAuthoring>
        {
            public override void Bake(PlayerDataSpawnerAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic),
                    new PlayerDataSpawnerData
                    {
                        PlayerDataPrefab = GetEntity(authoring.PlayerDataPrefab, TransformUsageFlags.Dynamic)
                    });
            }
        }
    }
}