using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000022 RID: 34
	public class EnterVillageTutorial : TutorialItemBase
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x000033EA File Offset: 0x000015EA
		public EnterVillageTutorial()
		{
			base.Type = "EnterVillageTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "storymode_tutorial_village_enter";
			base.MouseRequired = true;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003416 File Offset: 0x00001616
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003419 File Offset: 0x00001619
		public override bool IsConditionsMetForActivation()
		{
			if (!TutorialHelper.IsCharacterPopUpWindowOpen && TutorialHelper.CurrentContext == 4)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				return ((currentSettlement != null) ? currentSettlement.StringId : null) == "village_ES3_2";
			}
			return false;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003447 File Offset: 0x00001647
		public override void OnGameMenuOptionSelected(GameMenuOption obj)
		{
			base.OnGameMenuOptionSelected(obj);
			this._isEnterOptionSelected = obj.IdString == "storymode_tutorial_village_enter";
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003466 File Offset: 0x00001666
		public override bool IsConditionsMetForCompletion()
		{
			return this._isEnterOptionSelected;
		}

		// Token: 0x0400002B RID: 43
		private bool _isEnterOptionSelected;

		// Token: 0x0400002C RID: 44
		private const string _enterGameMenuOptionId = "storymode_tutorial_village_enter";
	}
}
