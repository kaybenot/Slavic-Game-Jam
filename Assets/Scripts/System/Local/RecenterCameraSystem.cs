using Data.Base;
using Data.Camera;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Local
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct RecenterCameraSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraRecenterRequestData>();
            state.RequireForUpdate<CameraTargetData>();

            var query = SystemAPI.QueryBuilder().WithAll<BaseData, GhostOwnerIsLocal>().Build();
            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var localTransform 
                     in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<BaseData, GhostOwnerIsLocal>())
            {
                foreach (var cameraTargetLocalTransform 
                         in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CameraTargetData>())
                {
                    foreach (var (recenterRequest, entity)
                             in SystemAPI.Query<RefRO<CameraRecenterRequestData>>().WithEntityAccess())
                    {
                        cameraTargetLocalTransform.ValueRW.Position = localTransform.ValueRO.Position;
                
                        ecb.DestroyEntity(entity);
                    }
                }
                break;
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}