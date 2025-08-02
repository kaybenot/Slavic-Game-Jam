using Unity.Entities;

namespace Data.Player
{
	public struct IncomeSource : IComponentData
	{
		public int GoldPerSecond;
	}
}
