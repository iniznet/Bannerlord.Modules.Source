using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Notification
{
	// Token: 0x02000101 RID: 257
	public class MapNotificationItemWidget : Widget
	{
		// Token: 0x06000D4C RID: 3404 RVA: 0x0002527B File Offset: 0x0002347B
		public MapNotificationItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x00025290 File Offset: 0x00023490
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._imageDetermined)
			{
				this.NotificationRingImageWidget.RegisterBrushStatesOfWidget();
				this.NotificationRingImageWidget.SetState(this.NotificationType);
				this._imageDetermined = true;
			}
			if (!this._sizeDetermined && this.NotificationDescriptionText != null)
			{
				this.DetermineSize();
			}
			bool flag = this._ringHoverBegan || this._extensionHoverBegan || this._removeHoverBegan;
			this._isExtended = flag;
			if (this.RemoveButtonVisualWidget != null)
			{
				this.RemoveButtonVisualWidget.IsVisible = this._isExtended && base.EventManager.IsControllerActive;
			}
			this.NotificationRingWidget.IsEnabled = !this._removeInitiated;
			this.NotificationExtensionWidget.IsEnabled = !this._removeInitiated;
			this.RemoveNotificationButtonWidget.IsVisible = flag && !this.IsInspectionForced;
			this.NotificationTextContainerWidget.IsVisible = flag;
			this.RefreshHorizontalPositioning(dt, flag);
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x00025388 File Offset: 0x00023588
		private void DetermineSize()
		{
			if (this.NotificationDescriptionText.Size.Y > base.Size.Y - 45f * base._scaleToUse)
			{
				this.NotificationExtensionWidget.Sprite = this.ExtendedWidthSprite;
				this.NotificationExtensionWidget.SuggestedWidth = this.ExtendedWidth;
			}
			else
			{
				this.NotificationExtensionWidget.Sprite = this.DefaultWidthSprite;
			}
			this._sizeDetermined = true;
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x000253FC File Offset: 0x000235FC
		private void RefreshHorizontalPositioning(float dt, bool shouldExtend)
		{
			float num = this.NotificationExtensionWidget.Size.X - this.NotificationRingWidget.Size.X + 20f * base._scaleToUse;
			float num2 = -(this.NotificationExtensionWidget.Size.X - (this.NotificationExtensionWidget.Size.X - this.NotificationRingWidget.Size.X)) + 35f * base._scaleToUse;
			float num3 = (shouldExtend ? num2 : num);
			this.NotificationExtensionWidget.ScaledPositionXOffset = this.LocalLerp(this.NotificationExtensionWidget.ScaledPositionXOffset, num3, dt * 18f);
			float num4 = 0f;
			if (this._removeInitiated)
			{
				num4 = this.NotificationRingWidget.Size.X;
			}
			else if (!base.IsVisible)
			{
				num4 = this.NotificationRingWidget.Size.X;
			}
			base.ScaledPositionXOffset = this.LocalLerp(base.ScaledPositionXOffset, num4, dt * 18f);
			if (this._removeInitiated && MathF.Abs(base.ScaledPositionXOffset - num4) < 0.7f)
			{
				base.EventFired("OnRemove", Array.Empty<object>());
			}
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x00025525 File Offset: 0x00023725
		private void OnRemoveClick(Widget button)
		{
			if (!this.IsInspectionForced)
			{
				this._removeInitiated = true;
			}
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x00025536 File Offset: 0x00023736
		private void OnInspectionClick(Widget button)
		{
			base.EventFired("OnInspection", Array.Empty<object>());
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06000D52 RID: 3410 RVA: 0x00025548 File Offset: 0x00023748
		// (set) Token: 0x06000D53 RID: 3411 RVA: 0x00025550 File Offset: 0x00023750
		[Editor(false)]
		public bool IsFocusItem
		{
			get
			{
				return this._isFocusItem;
			}
			set
			{
				if (value != this._isFocusItem)
				{
					this._isFocusItem = value;
					base.OnPropertyChanged(value, "IsFocusItem");
				}
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06000D54 RID: 3412 RVA: 0x0002556E File Offset: 0x0002376E
		// (set) Token: 0x06000D55 RID: 3413 RVA: 0x00025576 File Offset: 0x00023776
		[Editor(false)]
		public float DefaultWidth
		{
			get
			{
				return this._defaultWidth;
			}
			set
			{
				if (value != this._defaultWidth)
				{
					this._defaultWidth = value;
					base.OnPropertyChanged(value, "DefaultWidth");
				}
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06000D56 RID: 3414 RVA: 0x00025594 File Offset: 0x00023794
		// (set) Token: 0x06000D57 RID: 3415 RVA: 0x0002559C File Offset: 0x0002379C
		[Editor(false)]
		public float ExtendedWidth
		{
			get
			{
				return this._extendedWidth;
			}
			set
			{
				if (value != this._extendedWidth)
				{
					this._extendedWidth = value;
					base.OnPropertyChanged(value, "ExtendedWidth");
				}
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06000D58 RID: 3416 RVA: 0x000255BA File Offset: 0x000237BA
		// (set) Token: 0x06000D59 RID: 3417 RVA: 0x000255C4 File Offset: 0x000237C4
		[Editor(false)]
		public ButtonWidget RemoveNotificationButtonWidget
		{
			get
			{
				return this._removeNotificationButtonWidget;
			}
			set
			{
				if (this._removeNotificationButtonWidget != value)
				{
					this._removeNotificationButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "RemoveNotificationButtonWidget");
					value.ClickEventHandlers.Add(new Action<Widget>(this.OnRemoveClick));
					value.boolPropertyChanged += this.RemoveButtonWidgetPropertyChanged;
				}
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06000D5A RID: 3418 RVA: 0x00025616 File Offset: 0x00023816
		// (set) Token: 0x06000D5B RID: 3419 RVA: 0x0002561E File Offset: 0x0002381E
		[Editor(false)]
		public Widget NotificationRingImageWidget
		{
			get
			{
				return this._notificationRingImageWidget;
			}
			set
			{
				if (this._notificationRingImageWidget != value)
				{
					this._notificationRingImageWidget = value;
					base.OnPropertyChanged<Widget>(value, "NotificationRingImageWidget");
				}
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06000D5C RID: 3420 RVA: 0x0002563C File Offset: 0x0002383C
		// (set) Token: 0x06000D5D RID: 3421 RVA: 0x00025644 File Offset: 0x00023844
		[Editor(false)]
		public bool IsInspectionForced
		{
			get
			{
				return this._isInspectionForced;
			}
			set
			{
				if (this._isInspectionForced != value)
				{
					this._isInspectionForced = value;
					base.OnPropertyChanged(value, "IsInspectionForced");
				}
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06000D5E RID: 3422 RVA: 0x00025662 File Offset: 0x00023862
		// (set) Token: 0x06000D5F RID: 3423 RVA: 0x0002566A File Offset: 0x0002386A
		[Editor(false)]
		public string NotificationType
		{
			get
			{
				return this._notificationType;
			}
			set
			{
				if (this._notificationType != value)
				{
					this._notificationType = value;
					base.OnPropertyChanged<string>(value, "NotificationType");
				}
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06000D60 RID: 3424 RVA: 0x0002568D File Offset: 0x0002388D
		// (set) Token: 0x06000D61 RID: 3425 RVA: 0x00025695 File Offset: 0x00023895
		[Editor(false)]
		public Sprite DefaultWidthSprite
		{
			get
			{
				return this._defaultWidthSprite;
			}
			set
			{
				if (this._defaultWidthSprite != value)
				{
					this._defaultWidthSprite = value;
					base.OnPropertyChanged<Sprite>(value, "DefaultWidthSprite");
				}
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06000D62 RID: 3426 RVA: 0x000256B3 File Offset: 0x000238B3
		// (set) Token: 0x06000D63 RID: 3427 RVA: 0x000256BB File Offset: 0x000238BB
		[Editor(false)]
		public Sprite ExtendedWidthSprite
		{
			get
			{
				return this._extendedWidthSprite;
			}
			set
			{
				if (this._extendedWidthSprite != value)
				{
					this._extendedWidthSprite = value;
					base.OnPropertyChanged<Sprite>(value, "ExtendedWidthSprite");
				}
			}
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x000256D9 File Offset: 0x000238D9
		private void RemoveButtonWidgetPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				this._removeHoverBegan = propertyValue;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06000D65 RID: 3429 RVA: 0x000256EF File Offset: 0x000238EF
		// (set) Token: 0x06000D66 RID: 3430 RVA: 0x000256F8 File Offset: 0x000238F8
		[Editor(false)]
		public Widget NotificationRingWidget
		{
			get
			{
				return this._notificationRingWidget;
			}
			set
			{
				if (this._notificationRingWidget != value)
				{
					this._notificationRingWidget = value;
					base.OnPropertyChanged<Widget>(value, "NotificationRingWidget");
					value.boolPropertyChanged += this.RingWidgetOnPropertyChanged;
					value.EventFire += this.InspectionWidgetsEventFire;
				}
			}
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x00025745 File Offset: 0x00023945
		private void RingWidgetOnPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				this._ringHoverBegan = propertyValue;
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06000D68 RID: 3432 RVA: 0x0002575B File Offset: 0x0002395B
		// (set) Token: 0x06000D69 RID: 3433 RVA: 0x00025764 File Offset: 0x00023964
		[Editor(false)]
		public Widget NotificationExtensionWidget
		{
			get
			{
				return this._notificationExtensionWidget;
			}
			set
			{
				if (this._notificationExtensionWidget != value)
				{
					this._notificationExtensionWidget = value;
					base.OnPropertyChanged<Widget>(value, "NotificationExtensionWidget");
					value.boolPropertyChanged += this.ExtensionWidgetOnPropertyChanged;
					value.EventFire += this.InspectionWidgetsEventFire;
				}
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06000D6A RID: 3434 RVA: 0x000257B1 File Offset: 0x000239B1
		// (set) Token: 0x06000D6B RID: 3435 RVA: 0x000257B9 File Offset: 0x000239B9
		[Editor(false)]
		public Widget NotificationTextContainerWidget
		{
			get
			{
				return this._notificationTextContainerWidget;
			}
			set
			{
				if (this._notificationTextContainerWidget != value)
				{
					this._notificationTextContainerWidget = value;
					base.OnPropertyChanged<Widget>(value, "NotificationTextContainerWidget");
				}
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06000D6C RID: 3436 RVA: 0x000257D7 File Offset: 0x000239D7
		// (set) Token: 0x06000D6D RID: 3437 RVA: 0x000257DF File Offset: 0x000239DF
		[Editor(false)]
		public RichTextWidget NotificationDescriptionText
		{
			get
			{
				return this._notificationDescriptionText;
			}
			set
			{
				if (this._notificationDescriptionText != value)
				{
					this._notificationDescriptionText = value;
					base.OnPropertyChanged<RichTextWidget>(value, "NotificationDescriptionText");
				}
			}
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06000D6E RID: 3438 RVA: 0x000257FD File Offset: 0x000239FD
		// (set) Token: 0x06000D6F RID: 3439 RVA: 0x00025805 File Offset: 0x00023A05
		[Editor(false)]
		public InputKeyVisualWidget RemoveButtonVisualWidget
		{
			get
			{
				return this._removeButtonVisualWidget;
			}
			set
			{
				if (this._removeButtonVisualWidget != value)
				{
					this._removeButtonVisualWidget = value;
					base.OnPropertyChanged<InputKeyVisualWidget>(value, "RemoveButtonVisualWidget");
				}
			}
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x00025823 File Offset: 0x00023A23
		private void InspectionWidgetsEventFire(Widget widget, string eventName, object[] eventParameters)
		{
			if (eventName == "MouseUp")
			{
				this.OnInspectionClick(widget);
				return;
			}
			if (eventName == "MouseAlternateUp")
			{
				this.OnRemoveClick(this);
			}
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x0002584E File Offset: 0x00023A4E
		private void ExtensionWidgetOnPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				this._extensionHoverBegan = propertyValue;
			}
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x00025864 File Offset: 0x00023A64
		private float LocalLerp(float start, float end, float delta)
		{
			if (MathF.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		// Token: 0x04000621 RID: 1569
		private bool _ringHoverBegan;

		// Token: 0x04000622 RID: 1570
		private bool _extensionHoverBegan;

		// Token: 0x04000623 RID: 1571
		private bool _removeHoverBegan;

		// Token: 0x04000624 RID: 1572
		private bool _removeInitiated;

		// Token: 0x04000625 RID: 1573
		private bool _imageDetermined;

		// Token: 0x04000626 RID: 1574
		private bool _sizeDetermined;

		// Token: 0x04000627 RID: 1575
		private bool _isExtended;

		// Token: 0x04000628 RID: 1576
		private bool _isFocusItem;

		// Token: 0x04000629 RID: 1577
		private float _defaultWidth;

		// Token: 0x0400062A RID: 1578
		private float _extendedWidth;

		// Token: 0x0400062B RID: 1579
		private bool _isInspectionForced;

		// Token: 0x0400062C RID: 1580
		private string _notificationType = "Default";

		// Token: 0x0400062D RID: 1581
		private Sprite _defaultWidthSprite;

		// Token: 0x0400062E RID: 1582
		private Sprite _extendedWidthSprite;

		// Token: 0x0400062F RID: 1583
		private Widget _notificationRingWidget;

		// Token: 0x04000630 RID: 1584
		private Widget _notificationRingImageWidget;

		// Token: 0x04000631 RID: 1585
		private Widget _notificationExtensionWidget;

		// Token: 0x04000632 RID: 1586
		private Widget _notificationTextContainerWidget;

		// Token: 0x04000633 RID: 1587
		private ButtonWidget _removeNotificationButtonWidget;

		// Token: 0x04000634 RID: 1588
		private RichTextWidget _notificationDescriptionText;

		// Token: 0x04000635 RID: 1589
		private InputKeyVisualWidget _removeButtonVisualWidget;
	}
}
