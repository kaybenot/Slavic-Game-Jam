using Data.Path;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Splines;

namespace Authoring.Path {
    public class PathWalkerAuthoring : MonoBehaviour {
        [SerializeField] private GameObject path;
        [SerializeField] private float startPos;
        [SerializeField] private float speed;
        
        public class SplinePathBaker : Baker<PathWalkerAuthoring> {
            public override void Bake(PathWalkerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new PathWalker() {
                    spline = GetEntity(authoring.path, TransformUsageFlags.None),
                    position = authoring.startPos,
                    velocity = authoring.speed
                });
            }
        }
    }
}