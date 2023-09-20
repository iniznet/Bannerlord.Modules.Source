using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200000A RID: 10
	public class ChoosingPerkUpgradesStep2Tutorial : TutorialItemBase
	{
		// Token: 0x0600002A RID: 42 RVA: 0x0000242C File Offset: 0x0000062C
		public ChoosingPerkUpgradesStep2Tutorial()
		{
			base.Type = "ChoosingPerkUpgradesStep2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = "AvailablePerks";
			base.MouseRequired = true;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002458 File Offset: 0x00000658
		public override bool IsConditionsMetForCompletion()
		{
			return this._perkPopupOpened;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002460 File Offset: 0x00000660
		public override void OnPerkSelectionToggle(PerkSelectionToggleEvent obj)
		{
			this._perkPopupOpened = true;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002469 File Offset: 0x00000669
		public override bool IsConditionsMetForActivation()
		{
			return (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && Hero.MainHero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().Count > 1 && TutorialHelper.CurrentContext == 3;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000249A File Offset: 0x0000069A
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 3;
		}

		// Token: 0x0400000B RID: 11
		private bool _perkPopupOpened;
	}
}
