using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Data.Player
{
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	public partial struct PlayerDataSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach (var (source, incomeOwner, e) in SystemAPI.Query<RefRO<IncomeSource>, RefRO<GhostOwner>>()
				.WithEntityAccess())
			{
				float deltaTime = SystemAPI.Time.DeltaTime;

				var job = new Job
				{
					DeltaTime = deltaTime,
					IncomeSource = source.ValueRO,
					IncomeSourceOwner = incomeOwner.ValueRO,
				};
				job.ScheduleParallel();
			}
		}

		[BurstCompile]
		private partial struct Job : IJobEntity
		{
			public float DeltaTime;
			[ReadOnly] public IncomeSource IncomeSource;
			[ReadOnly] public GhostOwner IncomeSourceOwner;

			[BurstCompile]
			public readonly void Execute(ref PlayerData playerData, ref GoldEarningData goldEarningData, in GhostOwner player)
			{
				if (player.NetworkId != IncomeSourceOwner.NetworkId)
					return;

				goldEarningData.earntGold += IncomeSource.GoldPerSecond * DeltaTime;
				if (goldEarningData.earntGold < 1)
					return;
				
				int goldIncome = (int)goldEarningData.earntGold;
				goldEarningData.earntGold -= goldIncome;
				playerData.Gold += goldIncome;
			}
		}
	}
}
