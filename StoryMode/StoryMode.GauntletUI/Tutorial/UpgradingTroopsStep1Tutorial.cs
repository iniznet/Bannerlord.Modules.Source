using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class UpgradingTroopsStep1Tutorial : TutorialItemBase
	{
		public UpgradingTroopsStep1Tutorial()
		{
			base.Type = "UpgradingTroopsStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "PartyButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._partyScreenOpened || this._playerUpgradedTroop;
		}

		public override void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
		{
			this._playerUpgradedTroop = true;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._partyScreenOpened = obj.NewContext == 1;
		}

		public override bool IsConditionsMetForActivation()
		{
			return Hero.MainHero.Gold >= 100 && TutorialHelper.CurrentContext == 4 && !TutorialHelper.PlayerIsInAnySettlement && TutorialHelper.PlayerIsSafeOnMap && TutorialHelper.PlayerHasAnyUpgradeableTroop;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		private bool _partyScreenOpened;

		private bool _playerUpgradedTroop;
	}
}
