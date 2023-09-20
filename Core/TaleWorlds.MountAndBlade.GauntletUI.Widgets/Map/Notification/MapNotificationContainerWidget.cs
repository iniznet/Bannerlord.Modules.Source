using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Notification
{
	// Token: 0x02000100 RID: 256
	public class MapNotificationContainerWidget : Widget
	{
		// Token: 0x06000D3D RID: 3389 RVA: 0x00024F50 File Offset: 0x00023150
		public MapNotificationContainerWidget(UIContext context)
			: base(context)
		{
			this._newChildren = new List<Widget>();
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x00024F6C File Offset: 0x0002316C
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

		// Token: 0x06000D3F RID: 3391 RVA: 0x00025048 File Offset: 0x00023248
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

		// Token: 0x06000D40 RID: 3392 RVA: 0x0002509F File Offset: 0x0002329F
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this._newChildren.Add(child);
		}

		// Token: 0x06000D41 RID: 3393 RVA: 0x000250B4 File Offset: 0x000232B4
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

		// Token: 0x06000D42 RID: 3394 RVA: 0x000250EC File Offset: 0x000232EC
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

		// Token: 0x06000D43 RID: 3395 RVA: 0x00025138 File Offset: 0x00023338
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

		// Token: 0x06000D44 RID: 3396 RVA: 0x000251B2 File Offset: 0x000233B2
		private float DetermineChildTargetYOffset(Widget child, int childIndex)
		{
			if (childIndex < this.MaxAmountOfNotificationsToShow)
			{
				return -child.Size.Y * (float)childIndex * base._inverseScaleToUse;
			}
			return -child.Size.Y * (float)this.MaxAmountOfNotificationsToShow * base._inverseScaleToUse;
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06000D45 RID: 3397 RVA: 0x000251EF File Offset: 0x000233EF
		// (set) Token: 0x06000D46 RID: 3398 RVA: 0x000251F7 File Offset: 0x000233F7
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

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06000D47 RID: 3399 RVA: 0x00025215 File Offset: 0x00023415
		// (set) Token: 0x06000D48 RID: 3400 RVA: 0x0002521D File Offset: 0x0002341D
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

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06000D49 RID: 3401 RVA: 0x0002523B File Offset: 0x0002343B
		// (set) Token: 0x06000D4A RID: 3402 RVA: 0x00025243 File Offset: 0x00023443
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

		// Token: 0x06000D4B RID: 3403 RVA: 0x00025261 File Offset: 0x00023461
		private float LocalLerp(float start, float end, float delta)
		{
			if (MathF.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		// Token: 0x0400061D RID: 1565
		private List<Widget> _newChildren;

		// Token: 0x0400061E RID: 1566
		private TextWidget _moreTextWidget;

		// Token: 0x0400061F RID: 1567
		private BrushWidget _moreTextWidgetContainer;

		// Token: 0x04000620 RID: 1568
		private int _maxAmountOfNotificationsToShow = 5;
	}
}
