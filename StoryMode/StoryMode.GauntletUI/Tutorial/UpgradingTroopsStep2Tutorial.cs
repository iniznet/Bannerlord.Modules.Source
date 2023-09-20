using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class UpgradingTroopsStep2Tutorial : TutorialItemBase
	{
		public UpgradingTroopsStep2Tutorial()
		{
			base.Type = "UpgradingTroopsStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Left;
			base.HighlightedVisualElementID = "UpgradePopupButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerUpgradedTroop || this._playerOpenedUpgradePopup;
		}

		public override void OnPlayerToggledUpgradePopup(PlayerToggledUpgradePopupEvent obj)
		{
			if (obj.IsOpened)
			{
				this._playerOpenedUpgradePopup = true;
			}
		}

		public override void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
		{
			this._playerUpgradedTroop = true;
		}

		public override bool IsConditionsMetForActivation()
		{
			if (Hero.MainHero.Gold > 100 && TutorialHelper.CurrentContext == 1)
			{
				PartyScreenManager instance = PartyScreenManager.Instance;
				if (instance != null && instance.CurrentMode <= 0)
				{
					return TutorialHelper.PlayerHasAnyUpgradeableTroop;
				}
			}
			return false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 1;
		}

		private bool _playerUpgradedTroop;

		private bool _playerOpenedUpgradePopup;
	}
}
