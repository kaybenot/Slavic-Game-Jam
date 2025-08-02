using Unity.Entities;

namespace Data.Path {
    public partial struct PathWalker : IComponentData {
        public Entity spline;
        public int segment;
        public float localPosition;
        public float localVelocity;
        public float moveSpeed;
        public byte invert;
    }
}