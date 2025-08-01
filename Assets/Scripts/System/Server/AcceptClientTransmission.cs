using Data.RPC;
using Helpers.Logging;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Logger = Helpers.Logging.Logger;

namespace Systems.Network.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct AcceptClientInGame : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiveRpcCommandRequest, entity) 
                     in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<RequestTransmissionRpc>().WithEntityAccess())
            {
                Logger.Log(new LogData
                {
                    Message = "Received RequestTransmissionRpc",
                    ShowClientServerPrefix = true,
                    WorldUnmanaged = state.WorldUnmanaged
                });
                
                var sourceEntity = receiveRpcCommandRequest.ValueRO.SourceConnection;
                entityCommandBuffer.AddComponent<NetworkStreamInGame>(sourceEntity);
                
                Logger.Log(new LogData
                {
                    Message = $"Client connected (Entity {sourceEntity.Index}), starting snapshot transmission",
                    ShowClientServerPrefix = true,
                    WorldUnmanaged = state.WorldUnmanaged
                });
                
                entityCommandBuffer.DestroyEntity(entity);
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }
    }
}