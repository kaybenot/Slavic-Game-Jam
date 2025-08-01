using Data.Camera;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Camera
{
    public class CameraTarget : MonoBehaviour
    {
        private class CameraTargetBaker : Baker<CameraTarget>
        {
            public override void Bake(CameraTarget authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new CameraTargetData
                {
                    Transform = authoring.transform
                });
            }
        }
    }
}