using Helpers.Base;
using Unity.Entities;

namespace Data.Spawner
{
    public struct BaseSpawnerData : IComponentData
    {
        public Entity BasePrefab;
        public uint Id;
        public BaseType BaseType
        {
            set => Id = (uint) value;
            get => (BaseType) Id;
        }
    }
}