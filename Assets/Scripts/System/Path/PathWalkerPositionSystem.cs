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

        private ComponentLookup<SplineData> _splineLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, PathWalker>().Build(ref state));
            _splineLookup = state.GetComponentLookup<SplineData>(true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _splineLookup.Update(ref state);
            var job = new Job() {
                splineLookup = _splineLookup
            };
            job.ScheduleParallel(new JobHandle()).Complete();
        }

        [BurstCompile]
        private partial struct Job : IJobEntity {
            
            // [ReadOnly] public ComponentLookup<SplineData> pathLookup;
            [ReadOnly] public ComponentLookup<SplineData> splineLookup;
            
            public void Execute(ref LocalTransform transform, in PathWalker walker) {
                // var spline = walker.path;
                // var splineData = splineLookup.GetRefRO(spline).ValueRO;
                //
                // var path = splineData.PathFromSpline(false);
                // var pos = path.Interpolate(walker.position);
                // transform.Position = pos;

                var splineEntity = walker.spline;
                var spline = splineLookup.GetRefRO(splineEntity).ValueRO;

                spline.MakeWrapper(walker.segment, walker.moveSpeed > 0);
                
                
                
            }
        }
    }
}