using Data.Player;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Systems.Network.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TestInputProcessingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerInputData, entity) in SystemAPI.Query<RefRO<PlayerInputData>>()
                         .WithEntityAccess().WithAll<Simulate>())
            {
                if (playerInputData.ValueRO.TestAction.IsSet)
                {
                    Debug.Log($"Test action triggered on server for entity: {entity.Index}");
                }
            }
        }
    }
}