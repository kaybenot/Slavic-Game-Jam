using Data.RPC;
using Helpers.Base;
using Helpers.Network;
using Helpers.Network.Rpc;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public delegate void DirectionChangedHandler(GameplayController gameplayController, BaseLane oldDirection, BaseLane newDirection);

public class GameplayController : MonoBehaviour
{
	public event DirectionChangedHandler OnDirectionChanged;

	[SerializeField]
	private BaseLane currentDirection;
	public BaseLane CurrentDirection => currentDirection;

	public void SendUnit(UnitType unitType)
	{
		var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		var clientWorld = WorldHelper.GetClientWorld();
		var rpc = new RequestUnitSpawnRpc
		{
			Lane = currentDirection,
			UnitType = unitType,
		};

		RPC.Send(
			rpc,
			ref commandBuffer,
			clientWorld.EntityManager,
			log: true);

		commandBuffer.Playback(clientWorld.EntityManager);
	}

	public void SetBattleDirection(BaseLane direction)
	{
		if (currentDirection != direction)
		{
			var old = currentDirection;
			currentDirection = direction;
			OnDirectionChanged?.Invoke(this, old, currentDirection);
		}
	}
}
