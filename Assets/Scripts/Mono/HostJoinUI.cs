using System;
using Helpers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mono
{
    [RequireComponent(typeof(UIDocument))]
    public class HostJoinUI : MonoBehaviour
    {
        [SerializeField] private SubScene subScene;
        
        private UIDocument document;
        private Button hostButton;
        private Button joinButton;

        private void Awake()
        {
            if (!TryGetComponent(out document))
            {
                throw new Exception("No UIDocument found");
            }
            
            hostButton = document.rootVisualElement.Q<Button>("Host");
            joinButton = document.rootVisualElement.Q<Button>("Join");
            
            hostButton.RegisterCallback<ClickEvent>(OnHostClick);
            joinButton.RegisterCallback<ClickEvent>(OnJoinClick);
        }

        private void HideUI()
        {
            var hostJoinPanel = document.rootVisualElement.Q<VisualElement>("HostJoinPanel");
            hostJoinPanel.visible = false;

            var gamePanel = document.rootVisualElement.Q<VisualElement>("GamePanel");
            gamePanel.visible = true;
		}

		private void OnHostClick(ClickEvent evt)
        {
            var (clientWorld, serverWorld) = BootstrapWorld("ClientWorld", "ServerWorld");

            var port = (ushort)2137;

            var networkStreamDriver = serverWorld.EntityManager
                .CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
            networkStreamDriver.ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(port));
            
            var connectionNetworkEndpoint = NetworkEndpoint.LoopbackIpv4.WithPort(port);
            networkStreamDriver = clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver))
                .GetSingletonRW<NetworkStreamDriver>();
            networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectionNetworkEndpoint);
            
            HideUI();
        }

        private void OnJoinClick(ClickEvent evt) {
            var (clientWorld, _) = BootstrapWorld("ClientWorld");

            var port = (ushort)2137;
            var ip = "127.0.0.1";
            
            var connectionNetworkEndpoint = NetworkEndpoint.Parse(ip, port);
            var networkStreamDriver = clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver))
                .GetSingletonRW<NetworkStreamDriver>();
            networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectionNetworkEndpoint);
            
            HideUI();
        }

        #nullable enable
        public (World client, World? server) BootstrapWorld(string client, string? server = null) {
            var clientWorld = ClientServerBootstrap.CreateClientWorld(client);
            var serverWorld = server?.Map(ClientServerBootstrap.CreateServerWorld);
            
            //Dispose Game Worlds
            foreach (var world in World.All)
            {
                //HasFlag instead of equality?
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }

            World.DefaultGameObjectInjectionWorld ??= serverWorld ?? clientWorld;
            
            serverWorld?.Run(w => SceneSystem.LoadSceneAsync(w.Unmanaged, subScene.SceneGUID, new SceneSystem.LoadParameters {
                AutoLoad = true
            }));
            SceneSystem.LoadSceneAsync(clientWorld.Unmanaged, subScene.SceneGUID, new SceneSystem.LoadParameters
            {
                AutoLoad = true
            });

            return (clientWorld, serverWorld);
        }
    }
}
