using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GauntletUI.Tutorial
{
	public class QuestScreenTutorial : TutorialItemBase
	{
		public QuestScreenTutorial()
		{
			base.Type = "GetQuestTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "QuestsButton";
			base.MouseRequired = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return Mission.Current == null && TutorialHelper.CurrentContext == 11;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._contextChangedToQuestsScreen;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._contextChangedToQuestsScreen = obj.NewContext == 11;
		}

		private bool _contextChangedToQuestsScreen;
	}
}
