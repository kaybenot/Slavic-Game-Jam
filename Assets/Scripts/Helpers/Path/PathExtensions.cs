using Data.Path;
using Unity.Burst;
using Unity.Mathematics;

namespace Helpers.Path {
    
    [BurstCompile]
    public static class PathExtensions {

        [BurstCompile]
        public static void MakeWrapper(in this SplineData spline, int idx, bool invert, ref PathWrapper wrapper){
            ref var points = ref spline.points.Value;
            
            if (invert) {
                idx = points.segmentCount - idx - 1;
            }
            
            var p1 = points.points[idx];
            var p2 = points.points[idx + 1];

            MakeWrapper(p2, p1, points.normalsSide[idx], invert, ref wrapper);
        }

        [BurstCompile]
        private static void MakeWrapper(in float3 start, in float3 end, in float3 normal, bool invert, ref PathWrapper wrapper)
        {
            wrapper.start = invert ? start : end;
            wrapper.end = invert ? end : start;
            wrapper.normalSide = invert ? -normal : normal;
        }
        
    }
}