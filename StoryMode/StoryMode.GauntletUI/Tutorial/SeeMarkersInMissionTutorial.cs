using System;
using System.Linq;
using SandBox.ViewModelCollection.Missions.NameMarker;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000015 RID: 21
	public class SeeMarkersInMissionTutorial : TutorialItemBase
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00002A9A File Offset: 0x00000C9A
		public SeeMarkersInMissionTutorial()
		{
			base.Type = "SeeMarkersInMissionTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Left;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002AC6 File Offset: 0x00000CC6
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerEnabledNameMarkers;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002ACE File Offset: 0x00000CCE
		public override void OnMissionNameMarkerToggled(MissionNameMarkerToggleEvent obj)
		{
			this._playerEnabledNameMarkers = obj.NewState;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002ADC File Offset: 0x00000CDC
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002AE0 File Offset: 0x00000CE0
		public override bool IsConditionsMetForActivation()
		{
			string[] array = new string[] { "center", "lordshall", "tavern", "prison", "village_center" };
			return TutorialHelper.PlayerIsInAnySettlement && TutorialHelper.CurrentContext == 8 && TutorialHelper.CurrentMissionLocation != null && array.Contains(TutorialHelper.CurrentMissionLocation.StringId) && !TutorialHelper.PlayerIsInAConversation;
		}

		// Token: 0x04000016 RID: 22
		private bool _playerEnabledNameMarkers;
	}
}
