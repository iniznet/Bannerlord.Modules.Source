using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000036 RID: 54
	public class CraftingStep1Tutorial : TutorialItemBase
	{
		// Token: 0x0600010B RID: 267 RVA: 0x0000437A File Offset: 0x0000257A
		public CraftingStep1Tutorial()
		{
			base.Type = "CraftingStep1Tutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "FreeModeClassSelectionButton";
			base.MouseRequired = false;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000043A6 File Offset: 0x000025A6
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 14;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000043AA File Offset: 0x000025AA
		public override void OnCraftingWeaponClassSelectionOpened(CraftingWeaponClassSelectionOpenedEvent obj)
		{
			this._craftingCategorySelectionOpened = obj.IsOpen;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000043B8 File Offset: 0x000025B8
		public override void OnCraftingOrderSelectionOpened(CraftingOrderSelectionOpenedEvent obj)
		{
			this._craftingOrderSelectionOpened = obj.IsOpen;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000043C6 File Offset: 0x000025C6
		public override void OnCraftingOnWeaponResultPopupOpened(CraftingWeaponResultPopupToggledEvent obj)
		{
			this._craftingOrderResultOpened = obj.IsOpen;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000043D4 File Offset: 0x000025D4
		public override bool IsConditionsMetForActivation()
		{
			return !this._craftingOrderSelectionOpened && !this._craftingOrderResultOpened;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000043E9 File Offset: 0x000025E9
		public override bool IsConditionsMetForCompletion()
		{
			return this._craftingCategorySelectionOpened;
		}

		// Token: 0x04000051 RID: 81
		private bool _craftingCategorySelectionOpened;

		// Token: 0x04000052 RID: 82
		private bool _craftingOrderSelectionOpened;

		// Token: 0x04000053 RID: 83
		private bool _craftingOrderResultOpened;
	}
}
