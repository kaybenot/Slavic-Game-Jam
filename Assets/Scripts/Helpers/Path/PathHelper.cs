using Data.Path;
using Unity.Burst;

namespace Helpers.Path {
    
    [BurstCompile]
    public static class PathHelper {

        [BurstCompile]
        public static bool TryAdvanceSegment(ref PathWalker walker, in SplineData spline, sbyte direction) {
            var segmentId = walker.segment;
            segmentId += direction;

            if (segmentId >= 0 && segmentId < spline.points.Value.segmentCount) {
                // direction < 0 -> position + 1 // -dir
                // direction > 0 -> position - 1 // -dir
                var wrapper = spline.MakeWrapper(segmentId, false);
                walker.segment = segmentId;
                walker.localPosition -= direction;
                walker.localVelocity = walker.moveSpeed / wrapper.Length();
                
                return true;
            }
            return false;
        }
        
    }
}