using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	public class QuestProgressVisualWidget : Widget
	{
		public Widget BarWidget { get; set; }

		public Widget SliderWidget { get; set; }

		public Widget CheckboxVisualWidget { get; set; }

		public QuestProgressVisualWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _initialized;

		private int _currentProgress;

		private int _targetProgress;

		private float _progressStoneWidth;

		private float _progressStoneHeight;

		private int _horizontalSpacingBetweenStones;

		private bool _isValid;
	}
}
