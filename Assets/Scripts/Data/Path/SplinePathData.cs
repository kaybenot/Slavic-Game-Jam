using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Splines;

namespace Data.Path {
    public partial struct SplinePathData : IComponentData {
        public float3 first;
        public float3 second;
    }
}