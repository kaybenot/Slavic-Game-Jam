using Data.RPC;
using Data.Spawner;
using Helpers.Logging;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Systems.Network.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnPlayerDataSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
            state.RequireForUpdate<PlayerDataSpawnerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spawnerData = SystemAPI.GetSingleton<PlayerDataSpawnerData>();
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (receiveRpcCommandRequest, entity) 
                     in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<SpawnPlayerDataRpc>().WithEntityAccess())
            {
                Logger.Log(new LogData
                {
                    Message = "Received SpawnPlayerDataRpc",
                    ShowClientServerPrefix = true,
                    WorldUnmanaged = state.WorldUnmanaged
                });
                
                var sourceEntity = receiveRpcCommandRequest.ValueRO.SourceConnection;
                var playerData = entityCommandBuffer.Instantiate(spawnerData.PlayerDataPrefab);
                var requesterNetworkId =
                    SystemAPI.GetComponent<NetworkId>(sourceEntity);
                entityCommandBuffer.AddComponent(playerData, new GhostOwner
                {
                    NetworkId = requesterNetworkId.Value
                });
                
                Logger.Log(new LogData
                {
                    Message = $"Created PlayerData for NetworkId: {requesterNetworkId.Value}",
                    ShowClientServerPrefix = true,
                    WorldUnmanaged = state.WorldUnmanaged
                });
                
                entityCommandBuffer.DestroyEntity(entity);
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }
    }
}