using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Notification
{
	public class MapNotificationContainerWidget : Widget
	{
		public MapNotificationContainerWidget(UIContext context)
			: base(context)
		{
			this._newChildren = new List<Widget>();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._newChildren.Count > 0)
			{
				foreach (Widget widget in this._newChildren)
				{
					widget.PositionYOffset = this.DetermineChildTargetYOffset(widget, base.GetChildIndex(widget));
				}
				this.DetermineChildrenVisibility();
				this.DetermineMoreTextStatus();
				this.DetermineNavigationIndicies();
				this._newChildren.Clear();
			}
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				if (i < this.MaxAmountOfNotificationsToShow)
				{
					float num = this.DetermineChildTargetYOffset(child, i);
					child.PositionYOffset = this.LocalLerp(child.PositionYOffset, num, dt * 18f);
				}
			}
		}

		private void DetermineNavigationIndicies()
		{
			for (int i = 0; i < base.ChildCount; i++)
			{
				MapNotificationItemWidget mapNotificationItemWidget = base.GetChild(i) as MapNotificationItemWidget;
				if (i < this.MaxAmountOfNotificationsToShow)
				{
					mapNotificationItemWidget.NotificationRingWidget.GamepadNavigationIndex = base.ChildCount - 1 - i;
				}
				else
				{
					mapNotificationItemWidget.NotificationRingWidget.GamepadNavigationIndex = -1;
				}
			}
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this._newChildren.Add(child);
		}

		protected override void OnAfterChildRemoved(Widget child)
		{
			base.OnAfterChildRemoved(child);
			if (this._newChildren.Contains(child))
			{
				this._newChildren.Remove(child);
			}
			this.DetermineChildrenVisibility();
			this.DetermineMoreTextStatus();
			this.DetermineNavigationIndicies();
		}

		private void DetermineChildrenVisibility()
		{
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				bool isVisible = child.IsVisible;
				child.IsVisible = i < this.MaxAmountOfNotificationsToShow;
				if (!isVisible)
				{
					child.PositionYOffset = this.DetermineChildTargetYOffset(child, i);
				}
			}
		}

		private void DetermineMoreTextStatus()
		{
			this.MoreTextWidgetContainer.IsVisible = base.ChildCount > this.MaxAmountOfNotificationsToShow;
			if (this.MoreTextWidgetContainer.IsVisible)
			{
				this.MoreTextWidget.Text = "+" + (base.ChildCount - this.MaxAmountOfNotificationsToShow);
				this.MoreTextWidgetContainer.BrushRenderer.RestartAnimation();
				this.MoreTextWidget.BrushRenderer.RestartAnimation();
			}
		}

		private float DetermineChildTargetYOffset(Widget child, int childIndex)
		{
			if (childIndex < this.MaxAmountOfNotificationsToShow)
			{
				return -child.Size.Y * (float)childIndex * base._inverseScaleToUse;
			}
			return -child.Size.Y * (float)this.MaxAmountOfNotificationsToShow * base._inverseScaleToUse;
		}

		[Editor(false)]
		public BrushWidget MoreTextWidgetContainer
		{
			get
			{
				return this._moreTextWidgetContainer;
			}
			set
			{
				if (this._moreTextWidgetContainer != value)
				{
					this._moreTextWidgetContainer = value;
					base.OnPropertyChanged<BrushWidget>(value, "MoreTextWidgetContainer");
				}
			}
		}

		[Editor(false)]
		public TextWidget MoreTextWidget
		{
			get
			{
				return this._moreTextWidget;
			}
			set
			{
				if (this._moreTextWidget != value)
				{
					this._moreTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "MoreTextWidget");
				}
			}
		}

		[Editor(false)]
		public int MaxAmountOfNotificationsToShow
		{
			get
			{
				return this._maxAmountOfNotificationsToShow;
			}
			set
			{
				if (this._maxAmountOfNotificationsToShow != value)
				{
					this._maxAmountOfNotificationsToShow = value;
					base.OnPropertyChanged(value, "MaxAmountOfNotificationsToShow");
				}
			}
		}

		private float LocalLerp(float start, float end, float delta)
		{
			if (MathF.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		private List<Widget> _newChildren;

		private TextWidget _moreTextWidget;

		private BrushWidget _moreTextWidgetContainer;

		private int _maxAmountOfNotificationsToShow = 5;
	}
}
