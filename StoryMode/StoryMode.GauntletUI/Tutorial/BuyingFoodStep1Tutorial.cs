using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000006 RID: 6
	public class BuyingFoodStep1Tutorial : TutorialItemBase
	{
		// Token: 0x06000016 RID: 22 RVA: 0x0000228E File Offset: 0x0000048E
		public BuyingFoodStep1Tutorial()
		{
			base.Type = "GetSuppliesTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "storymode_tutorial_village_buy";
			base.MouseRequired = true;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000022BA File Offset: 0x000004BA
		public override bool IsConditionsMetForCompletion()
		{
			return this._contextChangedToInventory;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000022C2 File Offset: 0x000004C2
		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.BuyingFoodBaseConditions && TutorialHelper.CurrentContext == 4;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000022DC File Offset: 0x000004DC
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._contextChangedToInventory = obj.NewContext == 2;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000022ED File Offset: 0x000004ED
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x04000007 RID: 7
		private bool _contextChangedToInventory;
	}
}
