using Unity.Entities;

namespace Data.Path {
    public struct PathWalker : IComponentData {
        public Entity spline;
        public int segment;
        
        /// <summary>
        /// Normalized position on current segment
        /// </summary>
        public float localPosition;
        
        /// <summary>
        /// Velocity on current segment in normalized units per per second <br/>
        /// value &gt; 0
        /// </summary>
        public float localVelocity;
        
        /// <summary>
        /// value &gt; 0
        /// </summary>
        public float moveSpeed;
        
        /// <summary>
        /// if value != 0 -> inverted <br/>
        /// if value == 0 -> not inverted
        /// </summary>
        public byte invert;

        /// <summary>
        /// offset from the path along normal
        /// </summary>
        public sbyte offset;
    }
}