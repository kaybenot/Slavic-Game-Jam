using Data.Player;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.NetCode;
using Unity.Collections;
using System;

public class PlayerDataUIController : MonoBehaviour
{
	[SerializeField]
	private UIDocument uiDocument;

	[Space]
	[SerializeField]
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
		World world = default;
		for (int w = 0; w < World.All.Count; w++)
		{
			if (World.All[w].Name != "ClientWorld")
				continue;

			world = World.All[w];
		}

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
			var goldLabel = uiDocument.rootVisualElement.Q<Label>("GoldLabel");
			if (goldLabel != null)
				goldLabel.text = $"{currentGold} gold";
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
		var label = uiDocument.rootVisualElement.Q<Label>("IncomeLabel");
		if (label != null)
			label.text = $"{income} gold/s";
	}
}
