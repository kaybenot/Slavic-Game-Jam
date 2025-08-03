using Unity.Burst;
using Unity.Mathematics;

namespace Helpers.Path {
    [BurstCompile]
    public ref struct PathWrapper {

        public float3 start;
        public float3 end;
        public float3 normalSide;

        [BurstCompile]
        public float3 Interpolate(float t) {
            return math.lerp(start, end, t);
        }

        [BurstCompile]
        public float Length() {
            return math.distance(start, end);
        }
    }
}