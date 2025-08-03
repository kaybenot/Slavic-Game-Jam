using Data.Path;
using Helpers.Path;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Splines;

namespace Authoring.Path {
    public class PathWalkerAuthoring : MonoBehaviour {
        
        public class SplinePathBaker : Baker<PathWalkerAuthoring> {
            public override void Bake(PathWalkerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PathWalker>(entity);
                AddComponent<TargetPos>(entity);
            }
        }
    }
}