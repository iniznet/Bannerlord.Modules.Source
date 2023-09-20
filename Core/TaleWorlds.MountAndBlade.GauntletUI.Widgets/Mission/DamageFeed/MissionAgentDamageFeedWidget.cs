using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.DamageFeed
{
	public class MissionAgentDamageFeedWidget : Widget
	{
		public MissionAgentDamageFeedWidget(UIContext context)
			: base(context)
		{
			this._feedItemQueue = new Queue<MissionAgentDamageFeedItemWidget>();
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			MissionAgentDamageFeedItemWidget missionAgentDamageFeedItemWidget = (MissionAgentDamageFeedItemWidget)child;
			this._feedItemQueue.Enqueue(missionAgentDamageFeedItemWidget);
			this.UpdateSpeedModifiers();
		}

		protected override void OnChildRemoved(Widget child)
		{
			this._activeFeedItem = null;
			base.OnChildRemoved(child);
		}

		protected override void OnLateUpdate(float dt)
		{
			if (this._activeFeedItem == null && this._feedItemQueue.Count > 0)
			{
				MissionAgentDamageFeedItemWidget missionAgentDamageFeedItemWidget = this._feedItemQueue.Dequeue();
				this._activeFeedItem = missionAgentDamageFeedItemWidget;
				this._activeFeedItem.ShowFeed();
			}
		}

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

		private int _speedUpWidgetLimit = 1;

		private readonly Queue<MissionAgentDamageFeedItemWidget> _feedItemQueue;

		private MissionAgentDamageFeedItemWidget _activeFeedItem;
	}
}
