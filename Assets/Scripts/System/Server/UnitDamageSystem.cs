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
            state.RequireForUpdate<PathWalker>();
        }

        [BurstCompile]
        private void DamageBase(ref SystemState state, in PathWalker pathWalker, BaseType type0, BaseType type1, int damage = 1)
        {
            if (pathWalker.position < 0f)
            {
                foreach (var baseData in SystemAPI.Query<RefRW<BaseData>>())
                {
                    if (baseData.ValueRO.BaseType == type0)
                    {
                        baseData.ValueRW.Health--;
                    }
                }
            }
            else if (pathWalker.position > 1f)
            {
                foreach (var baseData in SystemAPI.Query<RefRW<BaseData>>())
                {
                    if (baseData.ValueRO.BaseType == type1)
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
            
            foreach (var (pathWalker, entity) in SystemAPI.Query<RefRO<PathWalker>>().WithEntityAccess())
            {
                if (pathWalker.ValueRO.position is <= 1f or > 0f)
                {
                    continue;
                }
                
                var splinePathData = state.EntityManager.GetComponentData<SplinePathData>(pathWalker.ValueRO.spline);
                switch (splinePathData.splineType)
                {
                    case SplineType.RedGreen:
                        DamageBase(ref state, in pathWalker.ValueRO, BaseType.Red, BaseType.Green);
                        break;
                    case SplineType.YellowBlue:
                        DamageBase(ref state, in pathWalker.ValueRO, BaseType.Yellow, BaseType.Blue);
                        break;
                    case SplineType.YellowGreen:
                        DamageBase(ref state, in pathWalker.ValueRO, BaseType.Yellow, BaseType.Green);
                        break;
                    case SplineType.YellowRed:
                        DamageBase(ref state, in pathWalker.ValueRO, BaseType.Yellow, BaseType.Red);
                        break;
                    case SplineType.BlueRed:
                        DamageBase(ref state, in pathWalker.ValueRO, BaseType.Blue, BaseType.Red);
                        break;
                    case SplineType.BlueGreen:
                        DamageBase(ref state, in pathWalker.ValueRO, BaseType.Blue, BaseType.Green);
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