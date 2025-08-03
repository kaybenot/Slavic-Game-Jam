using Unity.Entities;

namespace Data.Path {
    
    public struct PathFinished : IComponentData {
        public Entity spline;
        public byte wasInverted;
    }
}