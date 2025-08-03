using Data.Player;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.NetCode;
using Unity.Collections;
using Helpers.Network;

public class PlayerDataUIController : MonoBehaviour
{
	[SerializeField]
	private VisualElementReference<Label> goldLabel;	
	[SerializeField]
	private VisualElementReference<Label> incomeLabel;
	[SerializeField]
	private VisualElementReference<Label> baseHealthLabel;

	private int currentGold;
	private float incomeRefreshTimer;

	private void Start()
	{
		incomeRefreshTimer = 1;
		UpdateValues();
	}

	private void Update() => UpdateValues();

	private void UpdateValues()
	{
		World world = WorldHelper.GetClientWorld();
		if (world == default)
			return;

		var entityManager = world.EntityManager;
		RefreshGold(entityManager);

		incomeRefreshTimer += Time.deltaTime;
		if (incomeRefreshTimer > 1)
		{
			incomeRefreshTimer = 0;
			RefreshIncome(entityManager);
		}
	}

	private void RefreshGold(EntityManager entityManager)
	{
		using var playerDataQuery = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(GhostOwnerIsLocal));
		if (playerDataQuery.IsEmpty)
			return;

		var playerData = playerDataQuery.GetSingleton<PlayerData>();
		if (currentGold != playerData.Gold)
		{
			currentGold = playerData.Gold;
			goldLabel.VisualElement.text = $"{currentGold} gold";
		}
	}

	private void RefreshIncome(EntityManager entityManager)
	{
		float income = 0;
		using var incomeSourcesQuery = entityManager.CreateEntityQuery(typeof(IncomeSource), typeof(GhostOwnerIsLocal));
		var playersIncomeSources = incomeSourcesQuery.ToComponentDataArray<IncomeSource>(Allocator.Temp);
		for (int i = 0; i < playersIncomeSources.Length; i++)
		{
			var entity = playersIncomeSources[i];
			income += entity.GoldPerSecond;
		}

		incomeLabel.VisualElement.text = $"{income} gold/s";
	}
}
