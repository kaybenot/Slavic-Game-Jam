using Unity.Entities;

namespace Data.Path {
    public partial struct PathWalker : IComponentData {
        public Entity spline;
        public float position;
        public float velocity;
    }
}