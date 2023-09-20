using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.DamageFeed
{
	// Token: 0x020000E5 RID: 229
	public class MissionAgentDamageFeedWidget : Widget
	{
		// Token: 0x06000BF7 RID: 3063 RVA: 0x000217BA File Offset: 0x0001F9BA
		public MissionAgentDamageFeedWidget(UIContext context)
			: base(context)
		{
			this._feedItemQueue = new Queue<MissionAgentDamageFeedItemWidget>();
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x000217D8 File Offset: 0x0001F9D8
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			MissionAgentDamageFeedItemWidget missionAgentDamageFeedItemWidget = (MissionAgentDamageFeedItemWidget)child;
			this._feedItemQueue.Enqueue(missionAgentDamageFeedItemWidget);
			this.UpdateSpeedModifiers();
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x00021805 File Offset: 0x0001FA05
		protected override void OnChildRemoved(Widget child)
		{
			this._activeFeedItem = null;
			base.OnChildRemoved(child);
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x00021818 File Offset: 0x0001FA18
		protected override void OnLateUpdate(float dt)
		{
			if (this._activeFeedItem == null && this._feedItemQueue.Count > 0)
			{
				MissionAgentDamageFeedItemWidget missionAgentDamageFeedItemWidget = this._feedItemQueue.Dequeue();
				this._activeFeedItem = missionAgentDamageFeedItemWidget;
				this._activeFeedItem.ShowFeed();
			}
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x0002185C File Offset: 0x0001FA5C
		private void UpdateSpeedModifiers()
		{
			if (base.ChildCount > this._speedUpWidgetLimit)
			{
				float num = (float)(base.ChildCount - this._speedUpWidgetLimit) / 3f + 1f;
				for (int i = 0; i < base.ChildCount - this._speedUpWidgetLimit; i++)
				{
					((MissionAgentDamageFeedItemWidget)base.GetChild(i)).SetSpeedModifier(num);
				}
			}
		}

		// Token: 0x04000586 RID: 1414
		private int _speedUpWidgetLimit = 1;

		// Token: 0x04000587 RID: 1415
		private readonly Queue<MissionAgentDamageFeedItemWidget> _feedItemQueue;

		// Token: 0x04000588 RID: 1416
		private MissionAgentDamageFeedItemWidget _activeFeedItem;
	}
}
