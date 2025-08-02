using Helpers.Logging;
using Unity.Burst;
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
			foreach (var (playerData, playerOwner, entity) in SystemAPI.Query<RefRW<PlayerData>, RefRO<GhostOwner>>()
				.WithEntityAccess())
			{
				foreach (var (source, incomeOwner, e) in SystemAPI.Query<RefRO<IncomeSource>, RefRO<GhostOwner>>()
					.WithEntityAccess())
				{
					int playerNetworkId = playerOwner.ValueRO.NetworkId;
					if (playerNetworkId == incomeOwner.ValueRO.NetworkId)
					{
						playerData.ValueRW.Gold += source.ValueRO.GoldPerSecond;
						Logger.Log(new LogData
						{
							Message = $"Player {playerNetworkId} has {playerData.ValueRW.Gold} gold",
							ShowClientServerPrefix = 1,
							WorldUnmanaged = state.WorldUnmanaged
						});
					}
				}
			}
		}
	}
}
