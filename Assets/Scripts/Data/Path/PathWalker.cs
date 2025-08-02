using Unity.Entities;

namespace Data.Path {
    public struct PathWalker : IComponentData {
        public Entity spline;
        public float position;
        public float velocity;
    }
}