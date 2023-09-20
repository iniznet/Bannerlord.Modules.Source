using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000037 RID: 55
	public class CraftingOrdersTutorial : TutorialItemBase
	{
		// Token: 0x06000112 RID: 274 RVA: 0x000043F1 File Offset: 0x000025F1
		public CraftingOrdersTutorial()
		{
			base.Type = "CraftingOrdersTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "CraftingOrderSelectionButton";
			base.MouseRequired = false;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000441D File Offset: 0x0000261D
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 14;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00004421 File Offset: 0x00002621
		public override void OnCraftingWeaponClassSelectionOpened(CraftingWeaponClassSelectionOpenedEvent obj)
		{
			this._craftingCategorySelectionOpened = obj.IsOpen;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000442F File Offset: 0x0000262F
		public override void OnCraftingOrderSelectionOpened(CraftingOrderSelectionOpenedEvent obj)
		{
			this._craftingOrderSelectionOpened = obj.IsOpen;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000443D File Offset: 0x0000263D
		public override void OnCraftingOnWeaponResultPopupOpened(CraftingWeaponResultPopupToggledEvent obj)
		{
			this._craftingOrderResultOpened = obj.IsOpen;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000444B File Offset: 0x0000264B
		public override bool IsConditionsMetForActivation()
		{
			return !this._craftingCategorySelectionOpened && !this._craftingOrderResultOpened && TutorialHelper.IsCurrentTownHaveDoableCraftingOrder;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00004464 File Offset: 0x00002664
		public override bool IsConditionsMetForCompletion()
		{
			return this._craftingOrderSelectionOpened;
		}

		// Token: 0x04000054 RID: 84
		private bool _craftingCategorySelectionOpened;

		// Token: 0x04000055 RID: 85
		private bool _craftingOrderSelectionOpened;

		// Token: 0x04000056 RID: 86
		private bool _craftingOrderResultOpened;
	}
}
