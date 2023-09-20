using System;
using System.Linq;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000018 RID: 24
	public class PressLeaveToReturnFromMissionTutorial2 : TutorialItemBase
	{
		// Token: 0x06000070 RID: 112 RVA: 0x00002D01 File Offset: 0x00000F01
		public PressLeaveToReturnFromMissionTutorial2()
		{
			base.Type = "PressLeaveToReturnFromMissionType2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002D2D File Offset: 0x00000F2D
		public override bool IsConditionsMetForCompletion()
		{
			return this._changedContext;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00002D35 File Offset: 0x00000F35
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._changedContext = true;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002D3E File Offset: 0x00000F3E
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002D44 File Offset: 0x00000F44
		public override bool IsConditionsMetForActivation()
		{
			string[] array = new string[] { "center", "lordshall", "tavern", "prison", "village_center", "arena" };
			return TutorialHelper.CurrentMissionLocation != null && array.Contains(TutorialHelper.CurrentMissionLocation.StringId) && TutorialHelper.PlayerIsInAnySettlement && !TutorialHelper.PlayerIsInAConversation && TutorialHelper.CurrentContext == 8;
		}

		// Token: 0x0400001C RID: 28
		private bool _changedContext;
	}
}
