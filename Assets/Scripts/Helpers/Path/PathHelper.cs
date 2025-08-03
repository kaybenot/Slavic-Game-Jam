using Data.Path;
using Unity.Burst;

namespace Helpers.Path {
    
    [BurstCompile]
    public static class PathHelper {

        [BurstCompile]
        public static bool TryAdvanceSegment(ref PathWalker walker, in SplineData spline, bool invert) {
            var segmentId = walker.segment;
            segmentId += 1; //invert ? -1 : 1;

            if (segmentId >= 0 && segmentId < spline.points.Value.segmentCount) {
                var wrapper = spline.MakeWrapper(segmentId, invert);
                walker.segment = segmentId;
                //Move position to next segment
                walker.localPosition -= 1;
                walker.localVelocity = walker.moveSpeed / wrapper.Length();
                
                return true;
            }
            return false;
        }
        
    }
}