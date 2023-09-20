using System;
using TaleWorlds.CampaignSystem;

namespace StoryMode
{
	// Token: 0x02000012 RID: 18
	public class StoryModeEvents : CampaignEventReceiver
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000049C8 File Offset: 0x00002BC8
		public static StoryModeEvents Instance
		{
			get
			{
				return StoryModeManager.Current.StoryModeEvents;
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000049D4 File Offset: 0x00002BD4
		public override void RemoveListeners(object obj)
		{
			this._onMainStoryLineSideChosenEvent.ClearListeners(obj);
			this._onStoryModeTutorialEndedEvent.ClearListeners(obj);
			this._onBannerPieceCollectedEvent.ClearListeners(obj);
			this._onConspiracyActivatedEvent.ClearListeners(obj);
			this._onTravelToVillageTutorialQuestStartedEvent.ClearListeners(obj);
			this._onConfigChanged.ClearListeners(obj);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00004A29 File Offset: 0x00002C29
		public static IMbEvent<MainStoryLineSide> OnMainStoryLineSideChosenEvent
		{
			get
			{
				return StoryModeEvents.Instance._onMainStoryLineSideChosenEvent;
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004A35 File Offset: 0x00002C35
		public void OnMainStoryLineSideChosen(MainStoryLineSide side)
		{
			StoryModeEvents.Instance._onMainStoryLineSideChosenEvent.Invoke(side);
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00004A47 File Offset: 0x00002C47
		public static IMbEvent OnStoryModeTutorialEndedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onStoryModeTutorialEndedEvent;
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004A53 File Offset: 0x00002C53
		public void OnStoryModeTutorialEnded()
		{
			StoryModeEvents.Instance._onStoryModeTutorialEndedEvent.Invoke();
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00004A64 File Offset: 0x00002C64
		public static IMbEvent OnBannerPieceCollectedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onBannerPieceCollectedEvent;
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004A70 File Offset: 0x00002C70
		public void OnBannerPieceCollected()
		{
			StoryModeEvents.Instance._onBannerPieceCollectedEvent.Invoke();
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00004A81 File Offset: 0x00002C81
		public static IMbEvent OnConspiracyActivatedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onConspiracyActivatedEvent;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004A8D File Offset: 0x00002C8D
		public void OnConspiracyActivated()
		{
			StoryModeEvents.Instance._onConspiracyActivatedEvent.Invoke();
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00004A9E File Offset: 0x00002C9E
		public static IMbEvent OnTravelToVillageTutorialQuestStartedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onTravelToVillageTutorialQuestStartedEvent;
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004AAA File Offset: 0x00002CAA
		public void OnTravelToVillageTutorialQuestStarted()
		{
			StoryModeEvents.Instance._onTravelToVillageTutorialQuestStartedEvent.Invoke();
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00004ABB File Offset: 0x00002CBB
		public static IMbEvent OnConfigChangedEvent
		{
			get
			{
				return StoryModeEvents.Instance._onConfigChanged;
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004AC7 File Offset: 0x00002CC7
		public void OnConfigChanged()
		{
			StoryModeEvents.Instance._onConfigChanged.Invoke();
		}

		// Token: 0x04000029 RID: 41
		private readonly MbEvent<MainStoryLineSide> _onMainStoryLineSideChosenEvent = new MbEvent<MainStoryLineSide>();

		// Token: 0x0400002A RID: 42
		private readonly MbEvent _onStoryModeTutorialEndedEvent = new MbEvent();

		// Token: 0x0400002B RID: 43
		private readonly MbEvent _onBannerPieceCollectedEvent = new MbEvent();

		// Token: 0x0400002C RID: 44
		private readonly MbEvent _onConspiracyActivatedEvent = new MbEvent();

		// Token: 0x0400002D RID: 45
		private readonly MbEvent _onTravelToVillageTutorialQuestStartedEvent = new MbEvent();

		// Token: 0x0400002E RID: 46
		private readonly MbEvent _onConfigChanged = new MbEvent();
	}
}
