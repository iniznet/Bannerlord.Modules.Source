using System;
using System.Linq;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class PressLeaveToReturnFromMissionTutorial2 : TutorialItemBase
	{
		public PressLeaveToReturnFromMissionTutorial2()
		{
			base.Type = "PressLeaveToReturnFromMissionType2";
			base.Placement = TutorialItemVM.ItemPlacements.TopRight;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._changedContext;
		}

		public override void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			this._changedContext = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		public override bool IsConditionsMetForActivation()
		{
			string[] array = new string[] { "center", "lordshall", "tavern", "prison", "village_center", "arena" };
			return TutorialHelper.CurrentMissionLocation != null && array.Contains(TutorialHelper.CurrentMissionLocation.StringId) && TutorialHelper.PlayerIsInAnySettlement && !TutorialHelper.PlayerIsInAConversation && TutorialHelper.CurrentContext == 8;
		}

		private bool _changedContext;
	}
}
