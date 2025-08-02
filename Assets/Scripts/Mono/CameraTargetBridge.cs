using System;
using System.Linq;
using Data.Camera;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class CameraTargetBridge : MonoBehaviour
    {
        private void Update()
        {
            for (var i = 0; i < World.All.Count; i++)
            {
                if (World.All[i].Name != "ClientWorld")
                {
                    continue;
                }
                    
                var entityManager = World.All[i].EntityManager;
                var query = entityManager.CreateEntityQuery(typeof(CameraTargetData));
                if (query.IsEmpty)
                {
                    return;
                }
            
                var localTransform = entityManager.GetComponentData<LocalTransform>(query.GetSingletonEntity());
                transform.position = localTransform.Position;
            }
        }
    }
}