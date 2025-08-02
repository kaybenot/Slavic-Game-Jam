using Data.Path;
using Helpers.Path;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Splines;

namespace Authoring.Path {
    public class PathWalkerAuthoring : MonoBehaviour {
        [SerializeField] private GameObject path;
        [SerializeField] private float startPos;
        [SerializeField] private float speed;

        [SerializeField] private bool invert;
        
        
        public class SplinePathBaker : Baker<PathWalkerAuthoring> {
            public override void Bake(PathWalkerAuthoring authoring) {
                // var entity = GetEntity(TransformUsageFlags.Dynamic);
                //
                // var walker = new PathWalker() {
                //     spline = GetEntity(authoring.path, TransformUsageFlags.None),
                //     localPosition = authoring.startPos,
                //     localVelocity = authoring.speed,
                //     segment = -1,
                //     invert = (byte)(authoring.invert ? 1 : 0),
                //     moveSpeed = authoring.speed
                // };
                //
                // PathHelper.TryAdvanceSegment(ref walker, );
                
                // AddComponent(entity, );
            }
        }
    }
}