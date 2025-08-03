using Data.Path;
using Unity.Burst;
using Unity.Mathematics;

namespace Helpers.Path {
    
    [BurstCompile]
    public static class PathExtensions {

        [BurstCompile]
        public static PathWrapper MakeWrapper(in this SplineData spline, int idx, bool invert) {
            ref var points = ref spline.points.Value;
            
            if (invert) {
                idx = points.segmentCount - idx - 1;
            }
            
            var p1 = points.points[idx];
            var p2 = points.points[idx + 1];

            return MakeWrapper(p1, p2, points.normalsSide[idx], invert);
        }

        [BurstCompile]
        public static PathWrapper MakeWrapper(float3 start, float3 end, float3 normal, bool invert) {
            return new PathWrapper {
                start = invert ? start : end,
                end = invert ? end : start,
                normalSide = invert ? -normal : normal
            };
        }
        
    }
}