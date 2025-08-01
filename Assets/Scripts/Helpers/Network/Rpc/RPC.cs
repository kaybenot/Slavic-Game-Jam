using System;
using Helpers.Logging;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Logger = Helpers.Logging.Logger;

namespace Helpers.Network.Rpc
{
    [BurstCompile]
    public static class RPC
    {
        [BurstCompile]
        public static void Send<T>(T rpcCommand, ref EntityCommandBuffer ecb, EntityManager entityManager,
            bool log = false) where T : unmanaged, IRpcCommand
        {
            var rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity, rpcCommand);
            ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);

            if (log)
            {
                Logger.Log(new LogData
                {
                    Message = $"Sending {rpcCommand}",
                    ShowClientServerPrefix = true,
                    WorldUnmanaged = entityManager.WorldUnmanaged
                });
            }
        }
    }
}