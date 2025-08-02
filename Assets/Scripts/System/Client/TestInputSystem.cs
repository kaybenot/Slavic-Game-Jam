using Data.Player;
using Unity.Burst;
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
            foreach (var playerInputData in SystemAPI.Query<RefRW<PlayerInputData>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    playerInputData.ValueRW.TestAction.Set();
                }
                else
                {
                    playerInputData.ValueRW.TestAction = default;
                }
            }
        }
    }
}