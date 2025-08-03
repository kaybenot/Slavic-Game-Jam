using Data.Path;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace System.Path {
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TargetPosLerper : ISystem {
        
        private ComponentLookup<SplineData> _splineLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PathWalker, LocalTransform>().Build());
            _splineLookup = state.GetComponentLookup<SplineData>(true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _splineLookup.Update(ref state);
            var dt = SystemAPI.Time.DeltaTime;
            var job = new Job() {
                deltaTimeFactor = math.pow(0.5f, dt * PathConstants.TargetFollowSpeed)
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
        }

        [BurstCompile]
        private partial struct Job : IJobEntity {

            public float deltaTimeFactor;
            
            public void Execute(in TargetPos pos, ref LocalTransform transform) {
                transform.Position = math.lerp(transform.Position, pos.targetPos, deltaTimeFactor);
            }
        }
    }
}