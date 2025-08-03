using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Splines;
using SplineType = Helpers.Path.SplineType;

namespace Data.Path {

    public partial struct SplineData : IComponentData {
        public BlobAssetReference<SplinePointsBlob> points;
        public SplineType type;
    }
    
    public struct SplinePointsBlob {
        public BlobArray<float3> points;
        public BlobArray<float3> normalsSide;
        public int segmentCount;
    }
}