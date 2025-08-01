using Data.Spawner;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Spawner
{
    public class BaseSpawnerAuthoring : MonoBehaviour
    {
        public GameObject BasePrefab;
        public uint Id;
        
        private class BaseSpawnerAuthoringBaker : Baker<BaseSpawnerAuthoring>
        {
            public override void Bake(BaseSpawnerAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new BaseSpawnerData
                {
                    BasePrefab = GetEntity(authoring.BasePrefab, TransformUsageFlags.Dynamic),
                    Id = authoring.Id
                });
            }
        }
    }
}