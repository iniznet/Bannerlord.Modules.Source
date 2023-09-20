using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200000F RID: 15
	public class GettingCompanionsStep2Tutorial : TutorialItemBase
	{
		// Token: 0x06000043 RID: 67 RVA: 0x000026D6 File Offset: 0x000008D6
		public GettingCompanionsStep2Tutorial()
		{
			base.Type = "GettingCompanionsStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "ApplicapleCompanion";
			base.MouseRequired = true;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002702 File Offset: 0x00000902
		public override bool IsConditionsMetForCompletion()
		{
			return this._wantedCharacterPopupOpened;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000270A File Offset: 0x0000090A
		public override void OnCharacterPortraitPopUpOpened(CharacterObject obj)
		{
			this._wantedCharacterPopupOpened = obj != null && obj.IsHero && obj.HeroObject.IsWanderer;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000272C File Offset: 0x0000092C
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

		// Token: 0x06000047 RID: 71 RVA: 0x000027BB File Offset: 0x000009BB
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x04000010 RID: 16
		private bool _wantedCharacterPopupOpened;
	}
}
