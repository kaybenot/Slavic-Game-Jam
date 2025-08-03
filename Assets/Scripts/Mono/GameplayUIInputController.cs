using Helpers.Base;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIInputController : MonoBehaviour
{
	[SerializeField]
	private GameplayController gameplayController;
	[SerializeField]
	private UIDocument uiDocument;

	[Header("Visual Elements")]
	[SerializeField]
	private VisualElementReference<Button> recruitTankButton;
	[SerializeField]
	private VisualElementReference<Button> recruitSupportButton;
	[SerializeField]
	private VisualElementReference<Button> recruitRangedButton;
	[SerializeField]
	private VisualElementReference<Button> sendLeftButton;
	[SerializeField]
	private VisualElementReference<Button> sendForwardButton;
	[SerializeField]
	private VisualElementReference<Button> sendRightButton;

	private void OnEnable()
	{
		recruitTankButton.VisualElement.clicked += RecruitTank;
		recruitSupportButton.VisualElement.clicked += RecruitSupport;
		recruitRangedButton.VisualElement.clicked += RecruitRanged;

		sendLeftButton.VisualElement.clicked += SetDirectionLeft;
		sendForwardButton.VisualElement.clicked += SetDirectionForward;
		sendRightButton.VisualElement.clicked += SetDirectionRight;
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
		TryUnsubscribe(recruitTankButton, RecruitTank);
		TryUnsubscribe(recruitSupportButton, RecruitSupport);
		TryUnsubscribe(recruitRangedButton, RecruitRanged);
		TryUnsubscribe(sendLeftButton, SetDirectionLeft);
		TryUnsubscribe(sendForwardButton, SetDirectionForward);
		TryUnsubscribe(sendRightButton, SetDirectionRight);
	}

	private void TryUnsubscribe(VisualElementReference<Button> visualElementReference, System.Action method)
	{
		if (visualElementReference.VisualElement != null)
			visualElementReference.VisualElement.clicked -= method;
	}
}
