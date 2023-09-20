using System;
using TaleWorlds.CampaignSystem;

namespace StoryMode
{
	public class StoryModeEvents : CampaignEventReceiver
	{
		public static StoryModeEvents Instance
		{
			get
			{
				return StoryModeManager.Current.StoryModeEvents;
			}
		}

		public override void RemoveListeners(object obj)
		{
			this._onMainStoryLineSideChosenEvent.ClearListeners(obj);
			this._onStoryModeTutorialEndedEvent.ClearListeners(obj);
			this._onBannerPieceCollectedEvent.ClearListeners(obj);
			this._onConspiracyActivatedEvent.ClearListeners(obj);
			this._onTravelToVillageTutorialQuestStartedEvent.ClearListeners(obj);
			this._onConfigChanged.ClearListeners(obj);
		}

		public static IMbEvent<MainStoryLineSide> OnMainStoryLineSideChosenEvent
		{
			get
			{
				return StoryModeEvents.Instance._onMainStoryLineSideChosenEvent;
			}
		}

		public void OnMainStoryLineSideChosen(MainStoryLineSide side)
		{
			StoryModeEvents.Instance._onMainStoryLineSideChosenEvent.Invoke(side);
		}

		public static IMbEvent OnStoryModeTutorialEndedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onStoryModeTutorialEndedEvent;
			}
		}

		public void OnStoryModeTutorialEnded()
		{
			StoryModeEvents.Instance._onStoryModeTutorialEndedEvent.Invoke();
		}

		public static IMbEvent OnBannerPieceCollectedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onBannerPieceCollectedEvent;
			}
		}

		public void OnBannerPieceCollected()
		{
			StoryModeEvents.Instance._onBannerPieceCollectedEvent.Invoke();
		}

		public static IMbEvent OnConspiracyActivatedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onConspiracyActivatedEvent;
			}
		}

		public void OnConspiracyActivated()
		{
			StoryModeEvents.Instance._onConspiracyActivatedEvent.Invoke();
		}

		public static IMbEvent OnTravelToVillageTutorialQuestStartedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onTravelToVillageTutorialQuestStartedEvent;
			}
		}

		public void OnTravelToVillageTutorialQuestStarted()
		{
			StoryModeEvents.Instance._onTravelToVillageTutorialQuestStartedEvent.Invoke();
		}

		public static IMbEvent OnConfigChangedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onConfigChanged;
			}
		}

		public void OnConfigChanged()
		{
			StoryModeEvents.Instance._onConfigChanged.Invoke();
		}

		private readonly MbEvent<MainStoryLineSide> _onMainStoryLineSideChosenEvent = new MbEvent<MainStoryLineSide>();

		private readonly MbEvent _onStoryModeTutorialEndedEvent = new MbEvent();

		private readonly MbEvent _onBannerPieceCollectedEvent = new MbEvent();

		private readonly MbEvent _onConspiracyActivatedEvent = new MbEvent();

		private readonly MbEvent _onTravelToVillageTutorialQuestStartedEvent = new MbEvent();

		private readonly MbEvent _onConfigChanged = new MbEvent();
	}
}
