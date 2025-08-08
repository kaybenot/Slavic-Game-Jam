using Unity.Entities;
using Unity.NetCode;

namespace Data.Player
{
	public struct PlayerData : IComponentData
	{
		[GhostField] public int Gold;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct GoldEarningData : IComponentData
	{
		public float earntGold;
	}
}
