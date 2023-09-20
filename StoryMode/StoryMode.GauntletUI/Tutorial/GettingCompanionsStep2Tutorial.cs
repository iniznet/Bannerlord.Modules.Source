using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class GettingCompanionsStep2Tutorial : TutorialItemBase
	{
		public GettingCompanionsStep2Tutorial()
		{
			base.Type = "GettingCompanionsStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "ApplicapleCompanion";
			base.MouseRequired = true;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._wantedCharacterPopupOpened;
		}

		public override void OnCharacterPortraitPopUpOpened(CharacterObject obj)
		{
			this._wantedCharacterPopupOpened = obj != null && obj.IsHero && obj.HeroObject.IsWanderer;
		}

		public override bool IsConditionsMetForActivation()
		{
			LocationComplex locationComplex = LocationComplex.Current;
			Location location = ((locationComplex != null) ? locationComplex.GetLocationWithId("tavern") : null);
			if (!TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.PlayerIsInNonEnemyTown && TutorialHelper.BackStreetMenuIsOpen && Clan.PlayerClan.Companions.Count == 0 && Clan.PlayerClan.CompanionLimit > 0)
			{
				bool? flag = TutorialHelper.IsThereAvailableCompanionInLocation(location);
				bool flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					return Hero.MainHero.Gold > TutorialHelper.MinimumGoldForCompanion;
				}
			}
			return false;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		private bool _wantedCharacterPopupOpened;
	}
}
