using Data.Camera;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace System.Client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct MoveCameraSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraTargetData>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            foreach (var localTransform
                     in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CameraTargetData>())
            {
                var x = Input.GetAxisRaw("Horizontal");
                var y = Input.GetAxisRaw("Vertical");

                localTransform.ValueRW.Position += new float3(x, 0f, y);
            }
        }
    }
}