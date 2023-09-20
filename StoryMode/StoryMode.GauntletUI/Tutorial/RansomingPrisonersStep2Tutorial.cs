using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class RansomingPrisonersStep2Tutorial : TutorialItemBase
	{
		public RansomingPrisonersStep2Tutorial()
		{
			base.Type = "RansomingPrisonersStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "sell_all_prisoners";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._sellPrisonersOptionsSelected;
		}

		public override void OnGameMenuOptionSelected(GameMenuOption obj)
		{
			this._sellPrisonersOptionsSelected = obj.IdString == "sell_all_prisoners";
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.PlayerIsInNonEnemyTown && TutorialHelper.BackStreetMenuIsOpen && !Hero.MainHero.IsPrisoner && MobileParty.MainParty.PrisonRoster.TotalManCount > 0;
		}

		private bool _sellPrisonersOptionsSelected;
	}
}
