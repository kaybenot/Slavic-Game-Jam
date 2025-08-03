using Data.Path;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using SplineType = Helpers.Path.SplineType;

namespace Authoring.Path {
    
    public class SplinePathAuthoring : MonoBehaviour {
        // [SerializeField] private Transform first;
        // [SerializeField] private Transform second;

        [SerializeField] private SplineType type;
        [SerializeField] private Color gizmoColor = Color.blue;
        
        public class SplinePathBaker : Baker<SplinePathAuthoring> {
            public override void Bake(SplinePathAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                //
                // var first = authoring.first.position;
                // var second = authoring.second.position;

                var t = authoring.transform;
                // Enumerable.Range(0, t.childCount)
                //     .Select(i => t.GetChild(i).position).ToArray();
                
                var bb = new BlobBuilder(Allocator.Temp);
                ref var spb = ref bb.ConstructRoot<SplinePointsBlob>();

                BlobBuilderArray<float3> points = bb.Allocate(ref spb.points, authoring.transform.childCount);
                BlobBuilderArray<float3> normals = bb.Allocate(ref spb.normalsSide, authoring.transform.childCount - 1);

                for (int i = 0; i < t.childCount; i++) {
                    var c1 = t.GetChild(i);
                    points[i] = c1.position;
                    if (i > 0) {
                        var delta = points[i - 1] - (float3)c1.position;
                        var norm = normalize(delta);
                        //May be inverted but who cares
                        normals[i - 1] = cross(norm, up());
                    }
                }

                spb.segmentCount = t.childCount - 1;
                
                var blobRef = bb.CreateBlobAssetReference<SplinePointsBlob>(Allocator.Persistent);
                
                bb.Dispose();
                
                AddBlobAsset(ref blobRef, out var hash);
                
                AddComponent(entity, new SplineData {
                    points = blobRef,
                    type = authoring.type
                });
                
                // AddComponent(entity, new SplineSegmentData() {
                //     first = first,
                //     second = second
                // });
            }
        }

        private void OnDrawGizmos() {
            var c = Gizmos.color;
            Gizmos.color = gizmoColor;
            var points = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) {
                var t = transform.GetChild(i);
                points[i] = t.position;
                Gizmos.DrawSphere(t.position, 0.25f);
            }
            Gizmos.DrawLineStrip(points, false);
            Gizmos.color = c;
        }
    }
}