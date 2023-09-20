using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace StoryMode.GauntletUI.Tutorial
{
	public class MovementInMissionTutorial : TutorialItemBase
	{
		public MovementInMissionTutorial()
		{
			base.Type = "MovementInMissionTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		public override bool IsConditionsMetForCompletion()
		{
			return this._playerMovedBackward && this._playerMovedLeft && this._playerMovedRight && this._playerMovedForward;
		}

		public override void OnPlayerMovementFlagChanged(MissionPlayerMovementFlagsChangeEvent obj)
		{
			base.OnPlayerMovementFlagChanged(obj);
			this._playerMovedRight = this._playerMovedRight || (obj.MovementFlag & 4) == 4;
			this._playerMovedLeft = this._playerMovedLeft || (obj.MovementFlag & 8) == 8;
			this._playerMovedForward = this._playerMovedForward || (obj.MovementFlag & 1) == 1;
			this._playerMovedBackward = this._playerMovedBackward || (obj.MovementFlag & 2) == 2;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		public override bool IsConditionsMetForActivation()
		{
			return Mission.Current != null && Mission.Current.Mode != 6 && !TutorialHelper.PlayerIsInAConversation && TutorialHelper.CurrentContext == 8;
		}

		private bool _playerMovedForward;

		private bool _playerMovedBackward;

		private bool _playerMovedLeft;

		private bool _playerMovedRight;
	}
}
