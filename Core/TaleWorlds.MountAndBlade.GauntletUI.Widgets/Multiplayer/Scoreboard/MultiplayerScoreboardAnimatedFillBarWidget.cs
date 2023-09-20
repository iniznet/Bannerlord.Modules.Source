using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	// Token: 0x02000084 RID: 132
	public class MultiplayerScoreboardAnimatedFillBarWidget : FillBarWidget
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000711 RID: 1809 RVA: 0x00014EEC File Offset: 0x000130EC
		// (remove) Token: 0x06000712 RID: 1810 RVA: 0x00014F24 File Offset: 0x00013124
		public event MultiplayerScoreboardAnimatedFillBarWidget.FullFillFinishedHandler OnFullFillFinished;

		// Token: 0x06000713 RID: 1811 RVA: 0x00014F59 File Offset: 0x00013159
		public MultiplayerScoreboardAnimatedFillBarWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x00014F80 File Offset: 0x00013180
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

		// Token: 0x06000715 RID: 1813 RVA: 0x000150BC File Offset: 0x000132BC
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

		// Token: 0x06000716 RID: 1814 RVA: 0x0001511C File Offset: 0x0001331C
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

		// Token: 0x06000717 RID: 1815 RVA: 0x0001524C File Offset: 0x0001344C
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

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000718 RID: 1816 RVA: 0x000153C5 File Offset: 0x000135C5
		// (set) Token: 0x06000719 RID: 1817 RVA: 0x000153CD File Offset: 0x000135CD
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

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x0600071A RID: 1818 RVA: 0x000153EB File Offset: 0x000135EB
		// (set) Token: 0x0600071B RID: 1819 RVA: 0x000153F3 File Offset: 0x000135F3
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

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x0600071C RID: 1820 RVA: 0x00015411 File Offset: 0x00013611
		// (set) Token: 0x0600071D RID: 1821 RVA: 0x00015419 File Offset: 0x00013619
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

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x00015437 File Offset: 0x00013637
		// (set) Token: 0x0600071F RID: 1823 RVA: 0x0001543F File Offset: 0x0001363F
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

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000720 RID: 1824 RVA: 0x0001545D File Offset: 0x0001365D
		// (set) Token: 0x06000721 RID: 1825 RVA: 0x00015465 File Offset: 0x00013665
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

		// Token: 0x04000323 RID: 803
		private float _currentTargetRatioOfChange;

		// Token: 0x04000324 RID: 804
		private float _finalRatio;

		// Token: 0x04000325 RID: 805
		private float _ratioOfChange;

		// Token: 0x04000326 RID: 806
		private bool _isStarted;

		// Token: 0x04000327 RID: 807
		private bool _inFinalFillState;

		// Token: 0x04000328 RID: 808
		private bool _inFirstFillState = true;

		// Token: 0x04000329 RID: 809
		private float _timePassed;

		// Token: 0x0400032A RID: 810
		private float _ratioOfChangePerTick;

		// Token: 0x0400032B RID: 811
		private string _xpBarSoundEventName = "multiplayer/xpbar";

		// Token: 0x0400032C RID: 812
		private string _xpBarStopSoundEventName = "multiplayer/xpbar_stop";

		// Token: 0x0400032E RID: 814
		private bool _isStartRequested;

		// Token: 0x0400032F RID: 815
		private float _animationDelay;

		// Token: 0x04000330 RID: 816
		private float _animationDuration;

		// Token: 0x04000331 RID: 817
		private Widget _changeOverlayWidget;

		// Token: 0x04000332 RID: 818
		private int _timesOfFullFill;

		// Token: 0x0200018B RID: 395
		// (Invoke) Token: 0x06001300 RID: 4864
		public delegate void FullFillFinishedHandler();
	}
}
