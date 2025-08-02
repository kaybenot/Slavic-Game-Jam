using Data.Player;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Player
{
	public class PlayerDataAuthoring : MonoBehaviour
    {
        public int StartingGold = 100;
        
        private class PlayerDataAuthoringBaker : Baker<PlayerDataAuthoring>
        {
            public override void Bake(PlayerDataAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.None), new PlayerData
                {
                    Gold = authoring.StartingGold
                });
            }
        }
    }
}