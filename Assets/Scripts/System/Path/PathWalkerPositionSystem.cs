using Data.Path;
using Helpers.Path;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace System.Path {
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct PathWalkerPositionSystem : ISystem {
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, PathWalker>().Build(ref state));
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var job = new Job() {
                splineLookup = state.GetComponentLookup<SplinePathData>(true)
            };
            job.ScheduleParallel(new JobHandle()).Complete();
        }


        [BurstCompile]
        private partial struct Job : IJobEntity {

            // [ReadOnly] public EntityManager manager;
            [ReadOnly] public ComponentLookup<SplinePathData> splineLookup;
            
            public void Execute(ref LocalTransform transform, in PathWalker walker) {
                // walke.position += walker.velocity * deltaTime;
                var spline = walker.spline;
                // var splineData = manager.GetComponentData<SplinePathData>(spline);
                var splineData = splineLookup.GetRefRO(spline).ValueRO;
                
                var path = splineData.PathFromSpline(false);
                var pos = path.Interpolate(walker.position);
                transform.Position = pos;
            }
        }
    }
}