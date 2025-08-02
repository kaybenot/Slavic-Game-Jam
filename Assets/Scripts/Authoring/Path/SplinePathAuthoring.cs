using Data.Path;
using Unity.Entities;
using UnityEngine;
using SplineType = Helpers.Path.SplineType;

namespace Authoring.Path {
    
    public class SplinePathAuthoring : MonoBehaviour {
        [SerializeField] private Transform first;
        [SerializeField] private Transform second;
        [SerializeField] private SplineType splineType;

        public class SplinePathBaker : Baker<SplinePathAuthoring> {
            public override void Bake(SplinePathAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);

                var first = authoring.first.position;
                var second = authoring.second.position;
                
                AddComponent(entity, new SplinePathData() {
                    first = first,
                    second = second,
                    splineType = authoring.splineType
                });
            }
        }
    }
}