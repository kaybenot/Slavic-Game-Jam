using Unity.Entities;
using UnityEngine;

namespace Data.Camera
{
    public struct CameraTargetData : IComponentData
    {
        public UnityObjectRef<Transform> Transform;
    }
}