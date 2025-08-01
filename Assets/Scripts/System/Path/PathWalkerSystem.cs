using System;
using Unity.Entities;
using Unity.Jobs;

namespace Data.Path {
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct PathWalkerSystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PathWalker>();
        }

        public void OnUpdate(ref SystemState state) {
            var job = new Job() {
                deltaTime = SystemAPI.Time.DeltaTime
            };
            job.ScheduleParallel(new JobHandle()).Complete();
        }


        private partial struct Job : IJobEntity {

            public float deltaTime;
                
            public void Execute(ref PathWalker walker) {
                walker.position += walker.velocity * deltaTime;
            }
        }
        
    }
    
}