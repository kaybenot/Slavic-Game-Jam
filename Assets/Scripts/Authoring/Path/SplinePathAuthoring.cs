using Data.Path;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Splines;

namespace Authoring.Path {
    
    public class SplinePathAuthoring : MonoBehaviour {
        [SerializeField] private Transform first;
        [SerializeField] private Transform second;

        public class SplinePathBaker : Baker<SplinePathAuthoring> {
            public override void Bake(SplinePathAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);

                var first = authoring.first.position;
                var second = authoring.second.position;
                
                AddComponent(entity, new SplinePathData() {
                    first = first,
                    second = second
                });
            }
        }
    }
}