using Data.Player;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Player
{
	public class IncomeSourceAuthoring : MonoBehaviour
    {
        [SerializeField]
        private int incomePerSecond;

		private class Baker : Baker<IncomeSourceAuthoring>
		{
			public override void Bake(IncomeSourceAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new IncomeSource
                {
                    GoldPerSecond = authoring.incomePerSecond
                });
			}
		}
	}
}
