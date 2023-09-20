﻿using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.General
{
	public class SingleplayerGeneralKillFeedWidget : Widget
	{
		public float VerticalPaddingAmount { get; set; } = 3f;

		public SingleplayerGeneralKillFeedWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._normalWidgetHeight <= 0f && base.ChildCount > 1)
			{
				this._normalWidgetHeight = base.GetChild(0).ScaledSuggestedHeight * base._inverseScaleToUse;
			}
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				child.PositionYOffset = Mathf.Lerp(child.PositionYOffset, this.GetVerticalPositionOfChildByIndex(i, base.ChildCount), 0.35f);
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
			return (this._normalWidgetHeight + this.VerticalPaddingAmount) * (float)(numOfTotalChild - 1 - indexOfChild);
		}

		private void UpdateSpeedModifiers()
		{
			if (base.ChildCount > this._speedUpWidgetLimit)
			{
				float num = (float)(base.ChildCount - this._speedUpWidgetLimit) / 20f + 1f;
				for (int i = 0; i < base.ChildCount - this._speedUpWidgetLimit; i++)
				{
					(base.GetChild(i) as SingleplayerGeneralKillFeedItemWidget).SetSpeedModifier(num);
				}
			}
		}

		private float _normalWidgetHeight;

		private int _speedUpWidgetLimit = 10;
	}
}
