using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200000B RID: 11
	public class ChoosingPerkUpgradesStep3Tutorial : TutorialItemBase
	{
		// Token: 0x0600002F RID: 47 RVA: 0x0000249D File Offset: 0x0000069D
		public ChoosingPerkUpgradesStep3Tutorial()
		{
			base.Type = "ChoosingPerkUpgradesStep3";
			base.Placement = TutorialItemVM.ItemPlacements.BottomRight;
			base.HighlightedVisualElementID = "PerkSelectionContainer";
			base.MouseRequired = true;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000024C9 File Offset: 0x000006C9
		public override bool IsConditionsMetForCompletion()
		{
			return this._perkSelectedByPlayer;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000024D1 File Offset: 0x000006D1
		public override void OnPerkSelectedByPlayer(PerkSelectedByPlayerEvent obj)
		{
			this._perkSelectedByPlayer = true;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000024DA File Offset: 0x000006DA
		public override bool IsConditionsMetForActivation()
		{
			return (TutorialHelper.PlayerIsInAnySettlement || TutorialHelper.PlayerIsSafeOnMap) && Hero.MainHero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().Count > 1 && TutorialHelper.CurrentContext == 3;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000250B File Offset: 0x0000070B
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 3;
		}

		// Token: 0x0400000C RID: 12
		private bool _perkSelectedByPlayer;
	}
}
