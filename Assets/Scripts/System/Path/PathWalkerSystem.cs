using Data.Path;
using Helpers.Path;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace System.Path {
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct PathWalkerSystem : ISystem {
        
        private ComponentLookup<SplinePathData> _splineLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PathWalker>();
            _splineLookup = state.GetComponentLookup<SplinePathData>(true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _splineLookup.Update(ref state);
            var job = new Job() {
                deltaTime = SystemAPI.Time.DeltaTime,
                splineLookup = _splineLookup
            };
            job.ScheduleParallel(new JobHandle()).Complete();
        }


        [BurstCompile]
        private partial struct Job : IJobEntity {

            public float deltaTime;
            [ReadOnly] public ComponentLookup<SplinePathData> splineLookup;
            
            public void Execute(ref PathWalker walker) {
                walker.position += walker.velocity * deltaTime / splineLookup.GetRefRO(walker.spline).ValueRO.PathFromSpline(false).Length();
            }
        }
    }
    
}