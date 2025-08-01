using Data.RPC;
using Data.Spawner;
using Helpers.Logging;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems.Network.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnPlayerBaseSystem : ISystem
    {
        private int currentId;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
            state.RequireForUpdate<BaseSpawnerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (receiveRpcCommandRequest, entity) 
                     in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<RequestBaseSpawnRpc>().WithEntityAccess())
            {
                Logger.Log(new LogData
                {
                    Message = "Received RequestBaseSpawnRpc",
                    ShowClientServerPrefix = 1,
                    WorldUnmanaged = state.WorldUnmanaged
                });

                foreach (var (baseSpawnerData, localTransform)
                         in SystemAPI.Query<RefRO<BaseSpawnerData>, RefRW<LocalTransform>>())
                {
                    if (baseSpawnerData.ValueRO.Id == currentId)
                    {
                        var sourceEntity = receiveRpcCommandRequest.ValueRO.SourceConnection;
                        var baseObj = entityCommandBuffer.Instantiate(baseSpawnerData.ValueRO.BasePrefab);
                        entityCommandBuffer.SetComponent(baseObj, new LocalTransform
                        {
                            Position = localTransform.ValueRO.Position,
                            Rotation = localTransform.ValueRO.Rotation,
                            Scale = localTransform.ValueRO.Scale
                        });
                        var requesterNetworkId =
                            SystemAPI.GetComponent<NetworkId>(sourceEntity);
                        entityCommandBuffer.AddComponent(baseObj, new GhostOwner
                        {
                            NetworkId = requesterNetworkId.Value
                        });
                        
                        currentId++;
                        
                        Logger.Log(new LogData
                        {
                            Message = $"Created a base for NetworkId: {requesterNetworkId.Value}",
                            ShowClientServerPrefix = 1,
                            WorldUnmanaged = state.WorldUnmanaged
                        });
                        
                        break;
                    }
                }
                
                entityCommandBuffer.DestroyEntity(entity);
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}