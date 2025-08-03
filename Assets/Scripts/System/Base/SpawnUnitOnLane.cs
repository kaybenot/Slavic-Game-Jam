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
    public partial struct SpawnUnitOnLane : ISystem {
        private Unity.Mathematics.Random _random;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RequestUnitSpawnRpc>();
            state.RequireForUpdate<UnitSpawnerData>();

            var query = SystemAPI.QueryBuilder().WithAll<BaseData, GhostOwner>().Build();
            state.RequireForUpdate(query);
            
            _random = Unity.Mathematics.Random.CreateFromIndex(1337);
        }

        [BurstCompile]
        private void SetWalkerComponent(ref SystemState state, ref EntityCommandBuffer entityCommandBuffer,
            ref Entity unitEntity, SplineType splineType, bool reverse)
        {
            foreach (var (splinePathData, splineEntity)
                     in SystemAPI.Query<RefRO<SplineData>>().WithEntityAccess())
            {
                if (splinePathData.ValueRO.type == splineType)
                {
                    var pw = new PathWalker {
                        spline = splineEntity,
                        segment = reverse ? splinePathData.ValueRO.points.Value.segmentCount : -1,
                        moveSpeed = 4f + _random.NextFloat() * 1f,
                        localPosition = 1f,
                        invert = (byte) (reverse ? 1 : 0),
                        offset = (sbyte)_random.NextInt(-16, 16)
                    };

                    PathHelper.TryAdvanceSegment(ref pw, splinePathData.ValueRO, reverse);
                    
                    entityCommandBuffer.SetComponent(unitEntity, pw);
                }
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (receiveRpcCommandRequest, unitSpawnRequest, rpcEntity) 
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

                foreach (var (baseData, ghostOwner) in SystemAPI.Query<RefRO<BaseData>, RefRO<GhostOwner>>())
                {
                    if (ghostOwner.ValueRO.NetworkId != SystemAPI.GetComponent<NetworkId>(receiveRpcCommandRequest.ValueRO.SourceConnection).Value)
                    {
                        continue;
                    }
                    
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
                                        SplineType.YellowRed, false);
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
                
                entityCommandBuffer.DestroyEntity(rpcEntity);
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }
    }
}