using Unity.Entities;
using Unity.NetCode;

namespace Data.Player
{
	public struct PlayerData : IComponentData
	{
		[GhostField] public int Gold;
		public float EarntGold;
	}

	public static class PlayerDataExtensions
	{
		public static PlayerData AddGold(this PlayerData playerData, float gold)
		{
			playerData.EarntGold += gold;
			if (playerData.EarntGold < 1)
				return playerData;

			int goldIncome = (int)playerData.EarntGold;
			playerData.EarntGold -= goldIncome;
			playerData.Gold += goldIncome;
			return playerData;
		}
	}
}
