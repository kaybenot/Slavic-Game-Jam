using Data.Player;
using Data.RPC;
using Helpers.Base;
using Helpers.Network.Rpc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace System.Client
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct TestInputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkStreamInGame>();
            state.RequireForUpdate<PlayerInputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var playerInputData in SystemAPI.Query<RefRW<PlayerInputData>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    playerInputData.ValueRW.TestAction.Set();
                    RPC.Send(new RequestUnitSpawnRpc
                    {
                        Lane = BaseLane.Forward
                    }, ref ecb, state.EntityManager, true);
                }
                else
                {
                    playerInputData.ValueRW.TestAction = default;
                }
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}