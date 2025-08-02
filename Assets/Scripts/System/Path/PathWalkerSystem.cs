using Data.Path;
using Helpers.Path;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace System.Path {
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct PathWalkerSystem : ISystem {
        
        private ComponentLookup<SplineData> _splineLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PathWalker>();
            _splineLookup = state.GetComponentLookup<SplineData>(true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _splineLookup.Update(ref state);
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var job = new Job() {
                deltaTime = SystemAPI.Time.DeltaTime,
                splineLookup = _splineLookup,
                commandBuffer = ecb.AsParallelWriter()
            };
            job.ScheduleParallel(new JobHandle()).Complete();
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        private partial struct Job : IJobEntity {

            public float deltaTime;
            [ReadOnly] public ComponentLookup<SplineData> splineLookup;
            public EntityCommandBuffer.ParallelWriter commandBuffer;
            
            public void Execute(Entity entity, [ChunkIndexInQuery] int idx, ref PathWalker walker) {
                var spline = splineLookup.GetRefRO(walker.spline).ValueRO;
                walker.localPosition += walker.localVelocity * deltaTime;

                if (walker.localPosition > 1f && !PathHelper.TryAdvanceSegment(ref walker, spline, (sbyte)walker.moveSpeed)) {
                    commandBuffer.DestroyEntity(idx, entity);
                }
                
            }
        }
    }
}