using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000012 RID: 18
	public class RansomingPrisonersStep2Tutorial : TutorialItemBase
	{
		// Token: 0x06000052 RID: 82 RVA: 0x0000293B File Offset: 0x00000B3B
		public RansomingPrisonersStep2Tutorial()
		{
			base.Type = "RansomingPrisonersStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "sell_all_prisoners";
			base.MouseRequired = true;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002967 File Offset: 0x00000B67
		public override bool IsConditionsMetForCompletion()
		{
			return this._sellPrisonersOptionsSelected;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000296F File Offset: 0x00000B6F
		public override void OnGameMenuOptionSelected(GameMenuOption obj)
		{
			this._sellPrisonersOptionsSelected = obj.IdString == "sell_all_prisoners";
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002987 File Offset: 0x00000B87
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000298A File Offset: 0x00000B8A
		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4 && TutorialHelper.PlayerIsInNonEnemyTown && TutorialHelper.BackStreetMenuIsOpen && !Hero.MainHero.IsPrisoner && MobileParty.MainParty.PrisonRoster.TotalManCount > 0;
		}

		// Token: 0x04000013 RID: 19
		private bool _sellPrisonersOptionsSelected;
	}
}
