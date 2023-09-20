using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	public class MultiplayerScoreboardAnimatedFillBarWidget : FillBarWidget
	{
		public event MultiplayerScoreboardAnimatedFillBarWidget.FullFillFinishedHandler OnFullFillFinished;

		public MultiplayerScoreboardAnimatedFillBarWidget(UIContext context)
			: base(context)
		{
		}

		public void StartAnimation()
		{
			if (base.FillWidget == null || base.ChangeWidget == null || MathF.Abs(this.AnimationDuration) <= 1E-45f)
			{
				return;
			}
			float num = Mathf.Clamp(Mathf.Clamp((float)base.InitialAmount, 0f, (float)base.MaxAmount) / (float)base.MaxAmount, 0f, 1f);
			float num2 = Mathf.Clamp((float)(base.CurrentAmount - base.InitialAmount), (float)(-(float)base.MaxAmount), (float)base.MaxAmount);
			this._finalRatio = num + Mathf.Clamp(num2 / (float)base.MaxAmount, -1f, 1f);
			if (!this._isStarted)
			{
				base.Context.TwoDimensionContext.CreateSoundEvent(this._xpBarSoundEventName);
				base.Context.TwoDimensionContext.PlaySoundEvent(this._xpBarSoundEventName);
			}
			if (this.TimesOfFullFill > 0)
			{
				this._currentTargetRatioOfChange = 1f;
			}
			else
			{
				this._currentTargetRatioOfChange = this._finalRatio;
				this._inFinalFillState = true;
			}
			this._inFirstFillState = true;
			this._ratioOfChange = num;
			this._isStarted = true;
			this._timePassed = 0f;
			this._ratioOfChangePerTick = this.AnimationDuration / ((float)this._timesOfFullFill + this._finalRatio);
		}

		public void Reset()
		{
			this._timePassed = 0f;
			this._ratioOfChange = 0f;
			this._currentTargetRatioOfChange = 0f;
			this._ratioOfChangePerTick = 0f;
			this._inFirstFillState = true;
			this._isStarted = false;
			base.Context.TwoDimensionContext.StopAndRemoveSoundEvent(this._xpBarSoundEventName);
		}

		protected override void OnUpdate(float dt)
		{
			if (this.IsStartRequested && !this._isStarted && !this._inFinalFillState)
			{
				this.StartAnimation();
			}
			if (!this._isStarted)
			{
				return;
			}
			this._timePassed += dt;
			if (this._timePassed >= this.AnimationDelay)
			{
				this._ratioOfChange += dt / this._ratioOfChangePerTick;
				if (this._ratioOfChange > this._currentTargetRatioOfChange)
				{
					if (this._timesOfFullFill > 0)
					{
						this._currentTargetRatioOfChange = 1f;
						this._ratioOfChange = 0f;
						this._timesOfFullFill--;
						this._inFirstFillState = false;
						if (this._timesOfFullFill == 0)
						{
							this._currentTargetRatioOfChange = this._finalRatio;
							this._inFinalFillState = true;
						}
						MultiplayerScoreboardAnimatedFillBarWidget.FullFillFinishedHandler onFullFillFinished = this.OnFullFillFinished;
						if (onFullFillFinished == null)
						{
							return;
						}
						onFullFillFinished();
						return;
					}
					else if (this._inFinalFillState || this._timesOfFullFill == 0)
					{
						this._inFinalFillState = true;
						this._ratioOfChange = this._finalRatio;
						this._isStarted = false;
						base.Context.TwoDimensionContext.StopAndRemoveSoundEvent(this._xpBarSoundEventName);
						base.Context.TwoDimensionContext.PlaySound(this._xpBarStopSoundEventName);
					}
				}
			}
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (base.FillWidget != null)
			{
				float x = base.FillWidget.ParentWidget.Size.X;
				float num = Mathf.Clamp(Mathf.Clamp((float)base.InitialAmount, 0f, (float)base.MaxAmount) / (float)base.MaxAmount, 0f, 1f);
				base.FillWidget.ScaledSuggestedWidth = num * x;
				base.FillWidget.IsVisible = this._inFirstFillState;
				if (base.ChangeWidget != null)
				{
					if (this._ratioOfChange >= 0f)
					{
						base.ChangeWidget.ScaledSuggestedWidth = this._ratioOfChange * x;
						base.ChangeWidget.Color = new Color(1f, 1f, 1f, 1f);
					}
					if (base.DividerWidget != null)
					{
						if (this._ratioOfChange > 0f)
						{
							base.DividerWidget.ScaledPositionXOffset = base.FillWidget.ScaledSuggestedWidth - base.DividerWidget.Size.X;
						}
						base.DividerWidget.IsVisible = this._ratioOfChange != 0f;
					}
					if (this.ChangeOverlayWidget != null)
					{
						this.ChangeOverlayWidget.ScaledPositionXOffset = base.ChangeWidget.ScaledMarginLeft + base.ChangeWidget.ScaledSuggestedWidth - this.ChangeOverlayWidget.Size.X;
						this.ChangeOverlayWidget.IsVisible = this._ratioOfChange != 0f;
					}
				}
			}
		}

		[Editor(false)]
		public bool IsStartRequested
		{
			get
			{
				return this._isStartRequested;
			}
			set
			{
				if (value != this._isStartRequested)
				{
					this._isStartRequested = value;
					base.OnPropertyChanged(value, "IsStartRequested");
				}
			}
		}

		[Editor(false)]
		public float AnimationDelay
		{
			get
			{
				return this._animationDelay;
			}
			set
			{
				if (this._animationDelay != value)
				{
					this._animationDelay = value;
					base.OnPropertyChanged(value, "AnimationDelay");
				}
			}
		}

		[Editor(false)]
		public float AnimationDuration
		{
			get
			{
				return this._animationDuration;
			}
			set
			{
				if (this._animationDuration != value)
				{
					this._animationDuration = value;
					base.OnPropertyChanged(value, "AnimationDuration");
				}
			}
		}

		[Editor(false)]
		public int TimesOfFullFill
		{
			get
			{
				return this._timesOfFullFill;
			}
			set
			{
				if (this._timesOfFullFill != value)
				{
					this._timesOfFullFill = value;
					base.OnPropertyChanged(value, "TimesOfFullFill");
				}
			}
		}

		[Editor(false)]
		public Widget ChangeOverlayWidget
		{
			get
			{
				return this._changeOverlayWidget;
			}
			set
			{
				if (this._changeOverlayWidget != value)
				{
					this._changeOverlayWidget = value;
					base.OnPropertyChanged<Widget>(value, "ChangeOverlayWidget");
				}
			}
		}

		private float _currentTargetRatioOfChange;

		private float _finalRatio;

		private float _ratioOfChange;

		private bool _isStarted;

		private bool _inFinalFillState;

		private bool _inFirstFillState = true;

		private float _timePassed;

		private float _ratioOfChangePerTick;

		private string _xpBarSoundEventName = "multiplayer/xpbar";

		private string _xpBarStopSoundEventName = "multiplayer/xpbar_stop";

		private bool _isStartRequested;

		private float _animationDelay;

		private float _animationDuration;

		private Widget _changeOverlayWidget;

		private int _timesOfFullFill;

		public delegate void FullFillFinishedHandler();
	}
}
