using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Splines;
using SplineType = Helpers.Path.SplineType;

namespace Data.Path {
    public struct SplinePathData : IComponentData {
        public float3 first;
        public float3 second;
        public SplineType splineType;
    }
}