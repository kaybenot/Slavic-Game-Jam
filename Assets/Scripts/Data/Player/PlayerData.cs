using Unity.Entities;
using Unity.NetCode;

namespace Data.Player
{
    public struct PlayerData : IComponentData
    {
        [GhostField] public int Gold;
    }
}