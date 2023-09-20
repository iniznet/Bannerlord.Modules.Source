using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class GettingCompanionsStep1Tutorial : TutorialItemBase
	{
		public GettingCompanionsStep1Tutorial()
		{
			base.Type = "GettingCompanionsStep1";
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
			base.OnGameMenuOpened(obj);
			this._wantedGameMenuOpened = obj.MenuContext.GameMenu.StringId == "town_backstreet";
		}

		public override bool IsConditionsMetForActivation()
		{
			LocationComplex locationComplex = LocationComplex.Current;
			Location location = ((locationComplex != null) ? locationComplex.GetLocationWithId("tavern") : null);
			if (!TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.PlayerIsInNonEnemyTown && TutorialHelper.TownMenuIsOpen && Clan.PlayerClan.Companions.Count == 0 && Clan.PlayerClan.CompanionLimit > 0)
			{
				bool? flag = TutorialHelper.IsThereAvailableCompanionInLocation(location);
				bool flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && Hero.MainHero.Gold > TutorialHelper.MinimumGoldForCompanion)
				{
					return TutorialHelper.CurrentContext == 4;
				}
			}
			return false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		private bool _wantedGameMenuOpened;
	}
}
