using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	// Token: 0x02000128 RID: 296
	public class GameNotificationWidget : BrushWidget
	{
		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06000F6F RID: 3951 RVA: 0x0002B1F9 File Offset: 0x000293F9
		// (set) Token: 0x06000F70 RID: 3952 RVA: 0x0002B201 File Offset: 0x00029401
		public float RampUpInSeconds { get; set; }

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06000F71 RID: 3953 RVA: 0x0002B20A File Offset: 0x0002940A
		// (set) Token: 0x06000F72 RID: 3954 RVA: 0x0002B212 File Offset: 0x00029412
		public float RampDownInSeconds { get; set; }

		// Token: 0x06000F73 RID: 3955 RVA: 0x0002B21B File Offset: 0x0002941B
		public GameNotificationWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x0002B22C File Offset: 0x0002942C
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

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06000F75 RID: 3957 RVA: 0x0002B362 File Offset: 0x00029562
		// (set) Token: 0x06000F76 RID: 3958 RVA: 0x0002B36A File Offset: 0x0002956A
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

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06000F77 RID: 3959 RVA: 0x0002B388 File Offset: 0x00029588
		// (set) Token: 0x06000F78 RID: 3960 RVA: 0x0002B390 File Offset: 0x00029590
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

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06000F79 RID: 3961 RVA: 0x0002B3C0 File Offset: 0x000295C0
		// (set) Token: 0x06000F7A RID: 3962 RVA: 0x0002B3C8 File Offset: 0x000295C8
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

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06000F7B RID: 3963 RVA: 0x0002B3DA File Offset: 0x000295DA
		// (set) Token: 0x06000F7C RID: 3964 RVA: 0x0002B3E2 File Offset: 0x000295E2
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

		// Token: 0x0400070C RID: 1804
		private bool _textWidgetAlignmentDirty = true;

		// Token: 0x0400070D RID: 1805
		private float _totalDt;

		// Token: 0x04000710 RID: 1808
		private int _notificationId;

		// Token: 0x04000711 RID: 1809
		private RichTextWidget _textWidget;

		// Token: 0x04000712 RID: 1810
		private ImageIdentifierWidget _announcerImageIdentifier;

		// Token: 0x04000713 RID: 1811
		private float _totalTime;
	}
}
