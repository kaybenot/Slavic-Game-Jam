using Data.Base;
using Data.Path;
using Helpers.Base;
using Helpers.Path;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace System.Server
{
    public partial struct UnitDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PathFinished>();
        }

        [BurstCompile]
        private void DamageBase(ref SystemState state, BaseType startBase, BaseType endBase, byte wasInverted)
        {
            foreach (var baseData in SystemAPI.Query<RefRW<BaseData>>())
            {
                if (wasInverted == 0)
                {
                    if (baseData.ValueRO.BaseType == endBase)
                    {
                        baseData.ValueRW.Health--;
                    }
                }
                else
                {
                    if (baseData.ValueRO.BaseType == startBase)
                    {
                        baseData.ValueRW.Health--;
                    }
                }
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (pathFinished, entity)
                     in SystemAPI.Query<RefRO<PathFinished>>().WithEntityAccess())
            {
                var splinePathData = state.EntityManager.GetComponentData<SplineData>(pathFinished.ValueRO.spline);
                switch (splinePathData.type)
                {
                    case SplineType.RedGreen:
                        DamageBase(ref state, BaseType.Red, BaseType.Green, pathFinished.ValueRO.wasInverted);
                        break;
                    case SplineType.YellowBlue:
                        DamageBase(ref state, BaseType.Yellow, BaseType.Blue, pathFinished.ValueRO.wasInverted);
                        break;
                    case SplineType.YellowGreen:
                        DamageBase(ref state, BaseType.Yellow, BaseType.Green, pathFinished.ValueRO.wasInverted);
                        break;
                    case SplineType.YellowRed:
                        DamageBase(ref state, BaseType.Yellow, BaseType.Red, pathFinished.ValueRO.wasInverted);
                        break;
                    case SplineType.BlueRed:
                        DamageBase(ref state, BaseType.Blue, BaseType.Red, pathFinished.ValueRO.wasInverted);
                        break;
                    case SplineType.BlueGreen:
                        DamageBase(ref state, BaseType.Blue, BaseType.Green, pathFinished.ValueRO.wasInverted);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                ecb.DestroyEntity(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}