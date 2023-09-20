using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class RansomingPrisonersStep1Tutorial : TutorialItemBase
	{
		public RansomingPrisonersStep1Tutorial()
		{
			base.Type = "RansomingPrisonersStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "town_backstreet";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._wantedGameMenuOpened;
		}

		public override void OnGameMenuOpened(MenuCallbackArgs obj)
		{
			this._wantedGameMenuOpened = obj.MenuContext.GameMenu.StringId == "town_backstreet";
		}

		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.PlayerIsInNonEnemyTown && TutorialHelper.TownMenuIsOpen && !Hero.MainHero.IsPrisoner && MobileParty.MainParty.PrisonRoster.TotalManCount > 0;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		private bool _wantedGameMenuOpened;
	}
}
