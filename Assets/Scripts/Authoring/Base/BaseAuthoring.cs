using Data.Base;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Base
{
    public class BaseAuthoring : MonoBehaviour
    {
        private class BaseAuthoringBaker : Baker<BaseAuthoring>
        {
            public override void Bake(BaseAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new BaseData());
            }
        }
    }
}