using System;
using System.Linq;
using SandBox.ViewModelCollection.Missions.NameMarker;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class SeeMarkersInMissionTutorial : TutorialItemBase
	{
		public SeeMarkersInMissionTutorial()
		{
			base.Type = "SeeMarkersInMissionTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Left;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerEnabledNameMarkers;
		}

		public override void OnMissionNameMarkerToggled(MissionNameMarkerToggleEvent obj)
		{
			this._playerEnabledNameMarkers = obj.NewState;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		public override bool IsConditionsMetForActivation()
		{
			string[] array = new string[] { "center", "lordshall", "tavern", "prison", "village_center" };
			return TutorialHelper.PlayerIsInAnySettlement && TutorialHelper.CurrentContext == 8 && TutorialHelper.CurrentMissionLocation != null && array.Contains(TutorialHelper.CurrentMissionLocation.StringId) && !TutorialHelper.PlayerIsInAConversation;
		}

		private bool _playerEnabledNameMarkers;
	}
}
