using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class TakingPrisonersTutorial : TutorialItemBase
	{
		public TakingPrisonersTutorial()
		{
			base.Type = "TakeAndRescuePrisonerTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "TransferButtonOnlyOtherPrisoners";
			base.MouseRequired = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 1;
		}

		public override bool IsConditionsMetForActivation()
		{
			PartyScreenManager instance = PartyScreenManager.Instance;
			PartyState partyState;
			return instance != null && instance.CurrentMode == 2 && (partyState = GameStateManager.Current.ActiveState as PartyState) != null && partyState.PartyScreenLogic.PrisonerRosters[0].Count > 0 && TutorialHelper.CurrentContext == 2;
		}

		public override void OnPlayerMoveTroop(PlayerMoveTroopEvent obj)
		{
			base.OnPlayerMoveTroop(obj);
			if (obj.IsPrisoner && obj.ToSide == 1 && obj.Amount > 0)
			{
				this._playerMovedOtherPrisonerTroop = true;
			}
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerMovedOtherPrisonerTroop;
		}

		private bool _playerMovedOtherPrisonerTroop;
	}
}
