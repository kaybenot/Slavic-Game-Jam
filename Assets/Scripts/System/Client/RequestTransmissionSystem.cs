using Data.RPC;
using Helpers.Logging;
using Helpers.Network.Rpc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace System.Client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct RequestTransmissionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_, entity) 
                     in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
            {
                entityCommandBuffer.AddComponent<NetworkStreamInGame>(entity);
                Logger.Log(new LogData
                {
                    Message = "Staring snapshot data transmission",
                    ShowClientServerPrefix = 1,
                    WorldUnmanaged = state.WorldUnmanaged
                });
                
                RPC.Send(new RequestTransmissionRpc(), ref entityCommandBuffer, state.EntityManager, true);
                RPC.Send(new SpawnPlayerDataRpc(), ref entityCommandBuffer, state.EntityManager, true);
                RPC.Send(new RequestBaseSpawnRpc(), ref entityCommandBuffer, state.EntityManager, true);
                
                var recenterCameraRequest = entityCommandBuffer.CreateEntity();
                entityCommandBuffer.AddComponent<ReceiveRpcCommandRequest>(recenterCameraRequest);
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }
    }
}