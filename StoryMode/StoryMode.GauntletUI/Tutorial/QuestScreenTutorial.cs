using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000025 RID: 37
	public class QuestScreenTutorial : TutorialItemBase
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x000035DA File Offset: 0x000017DA
		public QuestScreenTutorial()
		{
			base.Type = "GetQuestTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "QuestsButton";
			base.MouseRequired = true;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003606 File Offset: 0x00001806
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003609 File Offset: 0x00001809
		public override bool IsConditionsMetForActivation()
		{
			return Mission.Current == null && TutorialHelper.CurrentContext == 11;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000361D File Offset: 0x0000181D
		public override bool IsConditionsMetForCompletion()
		{
			return this._contextChangedToQuestsScreen;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00003625 File Offset: 0x00001825
		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._contextChangedToQuestsScreen = obj.NewContext == 11;
		}

		// Token: 0x04000032 RID: 50
		private bool _contextChangedToQuestsScreen;
	}
}
