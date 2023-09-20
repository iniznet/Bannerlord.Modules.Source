using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	// Token: 0x02000054 RID: 84
	public class QuestProgressVisualWidget : Widget
	{
		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x0000DC79 File Offset: 0x0000BE79
		// (set) Token: 0x06000466 RID: 1126 RVA: 0x0000DC81 File Offset: 0x0000BE81
		public Widget BarWidget { get; set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x0000DC8A File Offset: 0x0000BE8A
		// (set) Token: 0x06000468 RID: 1128 RVA: 0x0000DC92 File Offset: 0x0000BE92
		public Widget SliderWidget { get; set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x0000DC9B File Offset: 0x0000BE9B
		// (set) Token: 0x0600046A RID: 1130 RVA: 0x0000DCA3 File Offset: 0x0000BEA3
		public Widget CheckboxVisualWidget { get; set; }

		// Token: 0x0600046B RID: 1131 RVA: 0x0000DCAC File Offset: 0x0000BEAC
		public QuestProgressVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0000DCB8 File Offset: 0x0000BEB8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				bool flag = this.CurrentProgress >= this.TargetProgress;
				base.IsVisible = !flag && this.IsValid;
				this.CheckboxVisualWidget.IsVisible = flag && this.IsValid;
				this.BarWidget.IsVisible = false;
				this.SliderWidget.IsVisible = false;
				if (base.IsVisible)
				{
					if (this.TargetProgress < 20)
					{
						for (int i = 0; i < this.TargetProgress; i++)
						{
							BrushWidget brushWidget = new BrushWidget(base.Context)
							{
								WidthSizePolicy = SizePolicy.Fixed,
								SuggestedWidth = this.ProgressStoneWidth,
								HeightSizePolicy = SizePolicy.Fixed,
								SuggestedHeight = this.ProgressStoneHeight,
								MarginRight = (float)this.HorizontalSpacingBetweenStones / 2f,
								MarginLeft = (float)this.HorizontalSpacingBetweenStones / 2f,
								IsEnabled = false
							};
							if (i < this.CurrentProgress)
							{
								brushWidget.Brush = base.Context.GetBrush("StageTask.ProgressStone");
								brushWidget.Brush.AlphaFactor = 0.8f;
							}
							this.BarWidget.AddChild(brushWidget);
						}
						this.BarWidget.IsVisible = true;
					}
					else if (this.TargetProgress >= 20)
					{
						this.SliderWidget.IsVisible = true;
						this.SliderWidget.IsDisabled = true;
					}
				}
				this._initialized = true;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x0000DE2A File Offset: 0x0000C02A
		// (set) Token: 0x0600046E RID: 1134 RVA: 0x0000DE32 File Offset: 0x0000C032
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
			set
			{
				if (this._isValid != value)
				{
					this._isValid = value;
				}
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x0000DE44 File Offset: 0x0000C044
		// (set) Token: 0x06000470 RID: 1136 RVA: 0x0000DE4C File Offset: 0x0000C04C
		public float ProgressStoneWidth
		{
			get
			{
				return this._progressStoneWidth;
			}
			set
			{
				if (this._progressStoneWidth != value)
				{
					this._progressStoneWidth = value;
				}
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x0000DE5E File Offset: 0x0000C05E
		// (set) Token: 0x06000472 RID: 1138 RVA: 0x0000DE66 File Offset: 0x0000C066
		public float ProgressStoneHeight
		{
			get
			{
				return this._progressStoneHeight;
			}
			set
			{
				if (this._progressStoneHeight != value)
				{
					this._progressStoneHeight = value;
				}
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x0000DE78 File Offset: 0x0000C078
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x0000DE80 File Offset: 0x0000C080
		public int CurrentProgress
		{
			get
			{
				return this._currentProgress;
			}
			set
			{
				if (this._currentProgress != value)
				{
					this._currentProgress = value;
				}
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x0000DE92 File Offset: 0x0000C092
		// (set) Token: 0x06000476 RID: 1142 RVA: 0x0000DE9A File Offset: 0x0000C09A
		public int TargetProgress
		{
			get
			{
				return this._targetProgress;
			}
			set
			{
				if (this._targetProgress != value)
				{
					this._targetProgress = value;
				}
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000477 RID: 1143 RVA: 0x0000DEAC File Offset: 0x0000C0AC
		// (set) Token: 0x06000478 RID: 1144 RVA: 0x0000DEB4 File Offset: 0x0000C0B4
		public int HorizontalSpacingBetweenStones
		{
			get
			{
				return this._horizontalSpacingBetweenStones;
			}
			set
			{
				if (this._horizontalSpacingBetweenStones != value)
				{
					this._horizontalSpacingBetweenStones = value;
				}
			}
		}

		// Token: 0x040001E9 RID: 489
		private bool _initialized;

		// Token: 0x040001ED RID: 493
		private int _currentProgress;

		// Token: 0x040001EE RID: 494
		private int _targetProgress;

		// Token: 0x040001EF RID: 495
		private float _progressStoneWidth;

		// Token: 0x040001F0 RID: 496
		private float _progressStoneHeight;

		// Token: 0x040001F1 RID: 497
		private int _horizontalSpacingBetweenStones;

		// Token: 0x040001F2 RID: 498
		private bool _isValid;
	}
}
