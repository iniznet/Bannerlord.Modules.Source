using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000011 RID: 17
	public class RansomingPrisonersStep1Tutorial : TutorialItemBase
	{
		// Token: 0x0600004D RID: 77 RVA: 0x000028A3 File Offset: 0x00000AA3
		public RansomingPrisonersStep1Tutorial()
		{
			base.Type = "RansomingPrisonersStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "town_backstreet";
			base.MouseRequired = true;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000028CF File Offset: 0x00000ACF
		public override bool IsConditionsMetForCompletion()
		{
			return this._wantedGameMenuOpened;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000028D7 File Offset: 0x00000AD7
		public override void OnGameMenuOpened(MenuCallbackArgs obj)
		{
			this._wantedGameMenuOpened = obj.MenuContext.GameMenu.StringId == "town_backstreet";
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000028F9 File Offset: 0x00000AF9
		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.PlayerIsInNonEnemyTown && TutorialHelper.TownMenuIsOpen && !Hero.MainHero.IsPrisoner && MobileParty.MainParty.PrisonRoster.TotalManCount > 0;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002938 File Offset: 0x00000B38
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x04000012 RID: 18
		private bool _wantedGameMenuOpened;
	}
}
