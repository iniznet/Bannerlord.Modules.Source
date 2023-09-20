using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000039 RID: 57
	public class CrimeTutorial : TutorialItemBase
	{
		// Token: 0x0600011E RID: 286 RVA: 0x000044E4 File Offset: 0x000026E4
		public CrimeTutorial()
		{
			base.Type = "CrimeTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "CrimeLabel";
			base.MouseRequired = false;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00004510 File Offset: 0x00002710
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00004513 File Offset: 0x00002713
		public override void OnCrimeValueInspectedInSettlementOverlay(SettlementMenuOverlayVM.CrimeValueInspectedInSettlementOverlayEvent obj)
		{
			this._inspectedCrimeValueItem = true;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000451C File Offset: 0x0000271C
		public override bool IsConditionsMetForActivation()
		{
			if (TutorialHelper.TownMenuIsOpen)
			{
				IFaction mapFaction = Settlement.CurrentSettlement.MapFaction;
				return mapFaction != null && mapFaction.MainHeroCrimeRating > 0f;
			}
			return false;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00004543 File Offset: 0x00002743
		public override bool IsConditionsMetForCompletion()
		{
			return this._inspectedCrimeValueItem;
		}

		// Token: 0x04000058 RID: 88
		private bool _inspectedCrimeValueItem;
	}
}
