using System.Linq;
using Data.Path;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

using static Unity.Mathematics.math;

namespace Authoring.Path {
    
    public class SplinePathAuthoring : MonoBehaviour {
        // [SerializeField] private Transform first;
        // [SerializeField] private Transform second;

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
                BlobBuilderArray<float3> normals = bb.Allocate(ref spb.points, authoring.transform.childCount - 1);

                for (int i = 0; i < t.childCount; i++) {
                    var c1 = t.GetChild(i);
                    points[i] = c1.position;
                    if (i > 0) {
                        var delta = points[i - 1] - (float3)c1.position;
                        var norm = normalize(delta);
                        //May be inverted but who cares
                        normals[i] = cross(norm, up());
                    }
                }

                spb.segmentCount = t.childCount - 1;
                
                var blobRef = bb.CreateBlobAssetReference<SplinePointsBlob>(Allocator.Persistent);
                
                AddBlobAsset(ref blobRef, out var hash);
                
                AddComponent(entity, new SplineData {
                    points = blobRef
                });
                
                // AddComponent(entity, new SplineSegmentData() {
                //     first = first,
                //     second = second
                // });
            }
        }
    }
}