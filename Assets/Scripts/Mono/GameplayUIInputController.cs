using Helpers.Base;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIInputController : MonoBehaviour
{
	[SerializeField]
	private GameplayController gameplayController;
	[SerializeField]
	private UIDocument uiDocument;

	private Button recruitTankButton;
	private Button recruitSupportButton;
	private Button recruitRangedButton;

	private Button sendLeftButton;
	private Button sendForwardButton;
	private Button sendRightButton;

	private void OnEnable()
	{
		recruitTankButton = uiDocument.rootVisualElement.Q<Button>("RecruitTankButton");
		recruitSupportButton = uiDocument.rootVisualElement.Q<Button>("RecruitSupportButton");
		recruitRangedButton = uiDocument.rootVisualElement.Q<Button>("RecruitRangedButton");
		
		sendLeftButton = uiDocument.rootVisualElement.Q<Button>("SendLeftButton");
		sendForwardButton = uiDocument.rootVisualElement.Q<Button>("SendForwardButton");
		sendRightButton = uiDocument.rootVisualElement.Q<Button>("SendRightButton");

		recruitTankButton.clicked += RecruitTank;
		recruitSupportButton.clicked += RecruitSupport;
		recruitRangedButton.clicked += RecruitRanged;

		sendLeftButton.clicked += SetDirectionLeft;
		sendForwardButton.clicked += SetDirectionForward;
		sendRightButton.clicked += SetDirectionRight;
	}

	private void RecruitTank() => RecruitUnit(UnitType.Tank);
	private void RecruitSupport() => RecruitUnit(UnitType.Support);
	private void RecruitRanged() => RecruitUnit(UnitType.Ranged);

	private void RecruitUnit(UnitType unitType) => gameplayController.SendUnit(unitType);

	private void SetDirectionLeft() => SetDirection(BaseLane.Left);
	private void SetDirectionForward() => SetDirection(BaseLane.Forward);
	private void SetDirectionRight() => SetDirection(BaseLane.Right);

	private void SetDirection(BaseLane lane) => gameplayController.SetBattleDirection(lane);

	private void OnDisable()
	{
		recruitTankButton.clicked -= RecruitTank;
		recruitSupportButton.clicked -= RecruitSupport;
		recruitRangedButton.clicked -= RecruitRanged;

		sendLeftButton.clicked -= SetDirectionLeft;
		sendForwardButton.clicked -= SetDirectionForward;
		sendRightButton.clicked -= SetDirectionRight;
	}
}
