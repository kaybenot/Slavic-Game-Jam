using Unity.Entities;
using Unity.NetCode;

namespace Data.Player
{
    public struct PlayerInputData : IInputComponentData
    {
        public InputEvent TestAction;
    }
}