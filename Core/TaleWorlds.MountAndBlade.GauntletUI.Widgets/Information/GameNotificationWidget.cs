using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	public class GameNotificationWidget : BrushWidget
	{
		public float RampUpInSeconds { get; set; }

		public float RampDownInSeconds { get; set; }

		public GameNotificationWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._textWidgetAlignmentDirty)
			{
				if (this.AnnouncerImageIdentifier.ImageTypeCode != 0)
				{
					this.TextWidget.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Left;
				}
				else
				{
					this.TextWidget.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Center;
				}
			}
			if (this.TotalTime > 0f && this._totalDt <= this.TotalTime)
			{
				if (this._totalDt <= this.RampUpInSeconds)
				{
					float num = Mathf.Lerp(0f, 1f, this._totalDt / this.RampUpInSeconds);
					this.SetGlobalAlphaRecursively(num);
				}
				else if (this._totalDt > this.RampUpInSeconds && this._totalDt < this.TotalTime - this.RampDownInSeconds)
				{
					this.SetGlobalAlphaRecursively(1f);
				}
				else if (this.TotalTime - this._totalDt < this.RampDownInSeconds)
				{
					float num2 = Mathf.Lerp(1f, 0f, 1f - (this.TotalTime - this._totalDt) / this.RampUpInSeconds);
					this.SetGlobalAlphaRecursively(num2);
				}
				else
				{
					this.SetGlobalAlphaRecursively(0f);
				}
				this._totalDt += dt;
			}
		}

		public ImageIdentifierWidget AnnouncerImageIdentifier
		{
			get
			{
				return this._announcerImageIdentifier;
			}
			set
			{
				if (this._announcerImageIdentifier != value)
				{
					this._announcerImageIdentifier = value;
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "AnnouncerImageIdentifier");
				}
			}
		}

		public int NotificationId
		{
			get
			{
				return this._notificationId;
			}
			set
			{
				if (this._notificationId != value)
				{
					this._notificationId = value;
					base.OnPropertyChanged(value, "NotificationId");
					this._textWidgetAlignmentDirty = true;
					this._totalDt = 0f;
				}
			}
		}

		public float TotalTime
		{
			get
			{
				return this._totalTime;
			}
			set
			{
				if (this._totalTime != value)
				{
					this._totalTime = value;
				}
			}
		}

		public RichTextWidget TextWidget
		{
			get
			{
				return this._textWidget;
			}
			set
			{
				if (this._textWidget != value)
				{
					this._textWidget = value;
					base.OnPropertyChanged<RichTextWidget>(value, "TextWidget");
				}
			}
		}

		private bool _textWidgetAlignmentDirty = true;

		private float _totalDt;

		private int _notificationId;

		private RichTextWidget _textWidget;

		private ImageIdentifierWidget _announcerImageIdentifier;

		private float _totalTime;
	}
}
