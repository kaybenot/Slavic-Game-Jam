using Data.Path;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace System.Path {
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct PathWalkerSystem : ISystem {
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PathWalker>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var job = new Job() {
                deltaTime = SystemAPI.Time.DeltaTime
            };
            job.ScheduleParallel(new JobHandle()).Complete();
        }


        [BurstCompile]
        private partial struct Job : IJobEntity {

            public float deltaTime;
                
            public void Execute(ref PathWalker walker) {
                walker.position += walker.velocity * deltaTime;
            }
        }
        
    }
    
}