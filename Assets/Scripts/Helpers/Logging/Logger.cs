using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using UnityEngine;

namespace Helpers.Logging
{
    [BurstCompile]
    public static class Logger
    {
        [BurstCompile]
        public static void Log(LogData data)
        {
            FixedString512Bytes message = data.Message;

            if (data.ShowClientServerPrefix)
            {
                var isClient = data.WorldUnmanaged.IsClient();
                FixedString32Bytes prefix = isClient ? "[Client]" : "[Server]";
                message = $"{prefix} {message}";
            }
            
            Debug.Log(message);
        }
    }
}