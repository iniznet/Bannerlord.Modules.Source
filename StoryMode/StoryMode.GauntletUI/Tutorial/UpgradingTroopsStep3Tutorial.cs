using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class UpgradingTroopsStep3Tutorial : TutorialItemBase
	{
		public UpgradingTroopsStep3Tutorial()
		{
			base.Type = "UpgradingTroopsStep3";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "UpgradeButton";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerUpgradedTroop;
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
	}
}
