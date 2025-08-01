using Data.Camera;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Local
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation)]
    public partial class CameraTargetSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // TODO: Target something
            SynchronizeEntityPositionWithTransform();
        }
        
        private void SynchronizeEntityPositionWithTransform()
        {
            foreach (var (cameraTargetData, localTransform) 
                     in SystemAPI.Query<RefRO<CameraTargetData>, RefRW<LocalTransform>>())
            {
                localTransform.ValueRW.Position = cameraTargetData.ValueRO.Transform.Value.position;
            }
        }
    }
}