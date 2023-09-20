using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Notification
{
	public class MapNotificationItemWidget : BrushWidget
	{
		public MapNotificationItemWidget(UIContext context)
			: base(context)
		{
		}

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

		private void OnRemoveClick(Widget button)
		{
			if (!this.IsInspectionForced)
			{
				this._removeInitiated = true;
				base.EventFired("OnRemoveBegin", Array.Empty<object>());
			}
		}

		private void OnInspectionClick(Widget button)
		{
			base.EventFired("OnInspection", Array.Empty<object>());
		}

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

		private void RemoveButtonWidgetPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				this._removeHoverBegan = propertyValue;
			}
		}

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

		private void RingWidgetOnPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				this._ringHoverBegan = propertyValue;
			}
		}

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

		private void ExtensionWidgetOnPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				this._extensionHoverBegan = propertyValue;
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

		private bool _ringHoverBegan;

		private bool _extensionHoverBegan;

		private bool _removeHoverBegan;

		private bool _removeInitiated;

		private bool _imageDetermined;

		private bool _sizeDetermined;

		private bool _isExtended;

		private bool _isFocusItem;

		private float _defaultWidth;

		private float _extendedWidth;

		private bool _isInspectionForced;

		private string _notificationType = "Default";

		private Sprite _defaultWidthSprite;

		private Sprite _extendedWidthSprite;

		private Widget _notificationRingWidget;

		private Widget _notificationRingImageWidget;

		private Widget _notificationExtensionWidget;

		private Widget _notificationTextContainerWidget;

		private ButtonWidget _removeNotificationButtonWidget;

		private RichTextWidget _notificationDescriptionText;

		private InputKeyVisualWidget _removeButtonVisualWidget;
	}
}
