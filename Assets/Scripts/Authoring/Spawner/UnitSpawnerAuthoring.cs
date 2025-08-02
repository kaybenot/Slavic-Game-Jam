using Data.Spawner;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Spawner
{
    public class UnitSpawnerAuthoring : MonoBehaviour
    {
        public GameObject UnitPrefab;
        
        private class UnitSpawnerAuthoringBaker : Baker<UnitSpawnerAuthoring>
        {
            public override void Bake(UnitSpawnerAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.None), new UnitSpawnerData
                {
                    UnitPrefab = GetEntity(authoring.UnitPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}