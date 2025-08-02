using Data.Spawner;
using Helpers.Base;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Spawner
{
    public class BaseSpawnerAuthoring : MonoBehaviour
    {
        public GameObject BasePrefab;
        public BaseType BaseType;
        
        private class BaseSpawnerAuthoringBaker : Baker<BaseSpawnerAuthoring>
        {
            public override void Bake(BaseSpawnerAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new BaseSpawnerData
                {
                    BasePrefab = GetEntity(authoring.BasePrefab, TransformUsageFlags.Dynamic),
                    BaseType = authoring.BaseType
                });
            }
        }
    }
}