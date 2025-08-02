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
                throw new System.Exception("No UIDocument found");
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
        }

        private void OnHostClick(ClickEvent evt)
        {
            var serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
            var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }

            if (World.DefaultGameObjectInjectionWorld == null)
            {
                World.DefaultGameObjectInjectionWorld = serverWorld;
            }

            var port = (ushort)2137;

            SceneSystem.LoadSceneAsync(serverWorld.Unmanaged, subScene.SceneGUID, new SceneSystem.LoadParameters
            {
                AutoLoad = true
            });
            SceneSystem.LoadSceneAsync(clientWorld.Unmanaged, subScene.SceneGUID, new SceneSystem.LoadParameters
            {
                AutoLoad = true
            });

            var networkStreamDriver = serverWorld.EntityManager
                .CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
            networkStreamDriver.ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(port));
            
            var connectionNetworkEndpoint = NetworkEndpoint.LoopbackIpv4.WithPort(port);
            networkStreamDriver = clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver))
                .GetSingletonRW<NetworkStreamDriver>();
            networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectionNetworkEndpoint);
            
            HideUI();
        }

        private void OnJoinClick(ClickEvent evt)
        {
            var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }

            if (World.DefaultGameObjectInjectionWorld == null)
            {
                World.DefaultGameObjectInjectionWorld = clientWorld;
            }
            
            SceneSystem.LoadSceneAsync(clientWorld.Unmanaged, subScene.SceneGUID, new SceneSystem.LoadParameters
            {
                AutoLoad = true
            });

            var port = (ushort)2137;
            var ip = "127.0.0.1";
            
            var connectionNetworkEndpoint = NetworkEndpoint.Parse(ip, port);
            var networkStreamDriver = clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver))
                .GetSingletonRW<NetworkStreamDriver>();
            networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectionNetworkEndpoint);
            
            HideUI();
        }
    }
}
