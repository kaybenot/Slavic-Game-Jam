using Data.Camera;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Local
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class CameraTargetSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            PassLocalTransformToGameObject();
        }
        
        private void PassLocalTransformToGameObject()
        {
            foreach (var (cameraTargetData, localTransform) 
                     in SystemAPI.Query<RefRW<CameraTargetData>, RefRO<LocalTransform>>())
            {
                cameraTargetData.ValueRW.Transform.Value.position = localTransform.ValueRO.Position;
            }
        }
    }
}