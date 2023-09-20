using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.Personal
{
	public class SingleplayerPersonalKillFeedWidget : Widget
	{
		public SingleplayerPersonalKillFeedWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._normalWidgetHeight == -1f && base.ChildCount > 1)
			{
				this._normalWidgetHeight = base.GetChild(0).ScaledSuggestedHeight * base._inverseScaleToUse;
			}
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				child.PositionYOffset = Mathf.Lerp(child.PositionYOffset, this.GetVerticalPositionOfChildByIndex(i, base.ChildCount), 0.2f);
			}
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.PositionYOffset = this.GetVerticalPositionOfChildByIndex(child.GetSiblingIndex(), base.ChildCount);
			this.UpdateSpeedModifiers();
		}

		private float GetVerticalPositionOfChildByIndex(int indexOfChild, int numOfTotalChild)
		{
			return -(this._normalWidgetHeight * (float)indexOfChild);
		}

		private void UpdateSpeedModifiers()
		{
			if (base.ChildCount > this._speedUpWidgetLimit)
			{
				float num = (float)(base.ChildCount - this._speedUpWidgetLimit) / 3f + 1f;
				for (int i = 0; i < base.ChildCount - this._speedUpWidgetLimit; i++)
				{
					(base.GetChild(i) as SingleplayerPersonalKillFeedItemWidget).SetSpeedModifier(num);
				}
			}
		}

		private float _normalWidgetHeight = -1f;

		private int _speedUpWidgetLimit = 3;
	}
}
