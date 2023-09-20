using System;
using System.Linq;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000017 RID: 23
	public class PressLeaveToReturnFromMissionTutorial1 : TutorialItemBase
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00002C4A File Offset: 0x00000E4A
		public PressLeaveToReturnFromMissionTutorial1()
		{
			base.Type = "PressLeaveToReturnFromMissionType1";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002C76 File Offset: 0x00000E76
		public override bool IsConditionsMetForCompletion()
		{
			return this._changedContext;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002C7E File Offset: 0x00000E7E
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._changedContext = true;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00002C87 File Offset: 0x00000E87
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002C8C File Offset: 0x00000E8C
		public override bool IsConditionsMetForActivation()
		{
			string[] array = new string[] { "center", "lordshall", "tavern", "prison", "village_center", "arena" };
			return TutorialHelper.CurrentMissionLocation != null && array.Contains(TutorialHelper.CurrentMissionLocation.StringId) && TutorialHelper.PlayerIsInAnySettlement && !TutorialHelper.PlayerIsInAConversation && TutorialHelper.CurrentContext == 8;
		}

		// Token: 0x0400001B RID: 27
		private bool _changedContext;
	}
}
