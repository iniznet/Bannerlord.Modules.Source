using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200000E RID: 14
	public class GettingCompanionsStep1Tutorial : TutorialItemBase
	{
		// Token: 0x0600003E RID: 62 RVA: 0x000025E7 File Offset: 0x000007E7
		public GettingCompanionsStep1Tutorial()
		{
			base.Type = "GettingCompanionsStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "town_backstreet";
			base.MouseRequired = true;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002613 File Offset: 0x00000813
		public override bool IsConditionsMetForCompletion()
		{
			return this._wantedGameMenuOpened;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000261B File Offset: 0x0000081B
		public override void OnGameMenuOpened(MenuCallbackArgs obj)
		{
			base.OnGameMenuOpened(obj);
			this._wantedGameMenuOpened = obj.MenuContext.GameMenu.StringId == "town_backstreet";
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002644 File Offset: 0x00000844
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

		// Token: 0x06000042 RID: 66 RVA: 0x000026D3 File Offset: 0x000008D3
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x0400000F RID: 15
		private bool _wantedGameMenuOpened;
	}
}
