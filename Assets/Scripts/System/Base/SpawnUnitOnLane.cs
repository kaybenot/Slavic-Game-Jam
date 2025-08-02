using Data.Base;
using Data.Path;
using Data.RPC;
using Data.Spawner;
using Helpers.Base;
using Helpers.Logging;
using Helpers.Path;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace System.Base
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnUnitOnLane : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RequestUnitSpawnRpc>();
            state.RequireForUpdate<UnitSpawnerData>();

            var query = SystemAPI.QueryBuilder().WithAll<BaseData, GhostOwnerIsLocal>().Build();
            state.RequireForUpdate(query);
        }

        [BurstCompile]
        private void SetWalkerComponent(ref SystemState state, ref EntityCommandBuffer entityCommandBuffer,
            ref Entity unitEntity, SplineType splineType, bool reverse)
        {
            foreach (var (splinePathData, splineEntity)
                     in SystemAPI.Query<RefRO<SplinePathData>>().WithEntityAccess())
            {
                if (splinePathData.ValueRO.splineType == splineType)
                {
                    if (reverse)
                    {
                        entityCommandBuffer.SetComponent(unitEntity, new PathWalker
                        {
                            spline = splineEntity,
                            position = 1f,
                            velocity = -1f
                        });
                    }
                    else
                    {
                        entityCommandBuffer.SetComponent(unitEntity, new PathWalker
                        {
                            spline = splineEntity,
                            position = 0f,
                            velocity = 1f
                        });
                    }
                }
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiveRpcCommandRequest, unitSpawnRequest, entity) 
                     in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<RequestUnitSpawnRpc>>().WithEntityAccess())
            {
                Logger.Log(new LogData
                {
                    Message = "Received RequestUnitSpawnRpc",
                    ShowClientServerPrefix = 1,
                    WorldUnmanaged = state.WorldUnmanaged
                });

                var spawnerData = SystemAPI.GetSingleton<UnitSpawnerData>();
                var unitEntity = entityCommandBuffer.Instantiate(spawnerData.UnitPrefab);

                foreach (var baseData in SystemAPI.Query<RefRO<BaseData>>().WithAll<GhostOwnerIsLocal>())
                {
                    switch (baseData.ValueRO.BaseType)
                    {
                        case BaseType.Red:
                            switch (unitSpawnRequest.ValueRO.Lane)
                            {
                                case BaseLane.Left:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.BlueRed, true);
                                    break;
                                case BaseLane.Right:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.YellowRed, true);
                                    break;
                                case BaseLane.Forward:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.RedGreen, false);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case BaseType.Blue:
                            switch (unitSpawnRequest.ValueRO.Lane)
                            {
                                case BaseLane.Left:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.BlueGreen, false);
                                    break;
                                case BaseLane.Right:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.RedGreen, false);
                                    break;
                                case BaseLane.Forward:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.YellowBlue, true);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case BaseType.Yellow:
                            switch (unitSpawnRequest.ValueRO.Lane)
                            {
                                case BaseLane.Left:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.YellowRed, false);
                                    break;
                                case BaseLane.Right:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.YellowGreen, false);
                                    break;
                                case BaseLane.Forward:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.YellowBlue, false);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case BaseType.Green:
                            switch (unitSpawnRequest.ValueRO.Lane)
                            {
                                case BaseLane.Left:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.YellowGreen, true);
                                    break;
                                case BaseLane.Right:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.BlueGreen, true);
                                    break;
                                case BaseLane.Forward:
                                    SetWalkerComponent(ref state, ref entityCommandBuffer, ref unitEntity,
                                        SplineType.RedGreen, true);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                Logger.Log(new LogData
                {
                    Message = $"Spawned unit on lane {unitSpawnRequest.ValueRO.Lane}",
                    ShowClientServerPrefix = 1,
                    WorldUnmanaged = state.WorldUnmanaged
                });
                
                entityCommandBuffer.DestroyEntity(entity);
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }
    }
}