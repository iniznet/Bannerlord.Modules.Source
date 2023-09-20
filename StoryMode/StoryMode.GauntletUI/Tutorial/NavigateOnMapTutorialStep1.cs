using System;
using SandBox.View.Map;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	public class NavigateOnMapTutorialStep1 : TutorialItemBase
	{
		public NavigateOnMapTutorialStep1()
		{
			base.Type = "NavigateOnMapTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.TownMenuIsOpen && !TutorialHelper.VillageMenuIsOpen && TutorialHelper.CurrentContext == 4;
		}

		public override void OnMainMapCameraMove(MainMapCameraMoveEvent obj)
		{
			base.OnMainMapCameraMove(obj);
			this._movedPosition = obj.PositionChanged || this._movedPosition;
			this._movedRotation = obj.RotationChanged || this._movedRotation;
			if (this._movedRotation && this._movedPosition && this._completionTime == DateTime.MinValue)
			{
				this._completionTime = TutorialHelper.CurrentTime;
			}
		}

		public override bool IsConditionsMetForCompletion()
		{
			return !(this._completionTime == DateTime.MinValue) && (TutorialHelper.CurrentTime - this._completionTime).TotalSeconds > 2.0;
		}

		private bool _movedPosition;

		private bool _movedRotation;

		private const float _delayInSeconds = 2f;

		private DateTime _completionTime = DateTime.MinValue;
	}
}
