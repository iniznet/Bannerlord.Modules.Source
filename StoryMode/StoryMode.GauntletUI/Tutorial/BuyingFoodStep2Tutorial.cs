using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000007 RID: 7
	public class BuyingFoodStep2Tutorial : TutorialItemBase
	{
		// Token: 0x0600001B RID: 27 RVA: 0x000022F0 File Offset: 0x000004F0
		public BuyingFoodStep2Tutorial()
		{
			base.Type = "GetSuppliesTutorialStep2";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "InventoryMicsFilter";
			base.MouseRequired = true;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000231C File Offset: 0x0000051C
		public override bool IsConditionsMetForCompletion()
		{
			return this._filterChangedToMisc;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002324 File Offset: 0x00000524
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.BuyingFoodBaseConditions && TutorialHelper.CurrentContext == 2;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002337 File Offset: 0x00000537
		public override void OnInventoryFilterChanged(InventoryFilterChangedEvent obj)
		{
			this._filterChangedToMisc = obj.NewFilter == 5;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002348 File Offset: 0x00000548
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 2;
		}

		// Token: 0x04000008 RID: 8
		private bool _filterChangedToMisc;
	}
}
