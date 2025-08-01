using Unity.Mathematics;

namespace Helpers.Path {
    public struct Path {

        public float3 start;
        public float3 end;

        public float3 Interpolate(float t) {
            return math.lerp(start, end, t);
        }
    }
}