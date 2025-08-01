using Data.Player;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Player
{
    public class PlayerInputAuthoring : MonoBehaviour
    {
        private class PlayerInputAuthoringBaker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                AddComponent<PlayerInputData>(GetEntity(TransformUsageFlags.None));
            }
        }
    }
}