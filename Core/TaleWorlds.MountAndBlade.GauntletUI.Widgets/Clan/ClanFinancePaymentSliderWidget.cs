using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	// Token: 0x02000152 RID: 338
	public class ClanFinancePaymentSliderWidget : SliderWidget
	{
		// Token: 0x06001189 RID: 4489 RVA: 0x00030596 File Offset: 0x0002E796
		public ClanFinancePaymentSliderWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x000305A0 File Offset: 0x0002E7A0
		protected override void OnLateUpdate(float dt)
		{
			this.CurrentRatioIndicatorWidget.ScaledPositionXOffset = Mathf.Clamp(base.Size.X * ((float)this.CurrentSize / (float)this.SizeLimit) - this.CurrentRatioIndicatorWidget.Size.X / 2f, 0f, base.Size.X);
			this.InitialFillWidget.ScaledPositionXOffset = this.CurrentRatioIndicatorWidget.PositionXOffset * base._scaleToUse + this.CurrentRatioIndicatorWidget.Size.X / 2f;
			this.InitialFillWidget.ScaledSuggestedWidth = base.Size.X - this.CurrentRatioIndicatorWidget.PositionXOffset * base._scaleToUse - this.CurrentRatioIndicatorWidget.Size.X / 2f;
			if (base.Handle.PositionXOffset > this.CurrentRatioIndicatorWidget.PositionXOffset)
			{
				this.NewIncreaseFillWidget.ScaledPositionXOffset = this.CurrentRatioIndicatorWidget.PositionXOffset * base._scaleToUse + this.CurrentRatioIndicatorWidget.Size.X / 2f;
				this.NewIncreaseFillWidget.ScaledSuggestedWidth = Mathf.Clamp((base.Handle.PositionXOffset - this.CurrentRatioIndicatorWidget.PositionXOffset) * base._scaleToUse, 0f, base.Size.X);
				this.NewDecreaseFillWidget.ScaledSuggestedWidth = 0f;
			}
			else if (base.Handle.PositionXOffset < this.CurrentRatioIndicatorWidget.PositionXOffset)
			{
				this.NewDecreaseFillWidget.ScaledPositionXOffset = base.Handle.PositionXOffset * base._scaleToUse + base.Handle.Size.X / 2f;
				this.NewDecreaseFillWidget.ScaledSuggestedWidth = Mathf.Clamp(this.CurrentRatioIndicatorWidget.PositionXOffset * base._scaleToUse + this.CurrentRatioIndicatorWidget.Size.X / 2f - (base.Handle.PositionXOffset * base._scaleToUse + base.Handle.Size.X / 2f), 0f, base.Size.X);
				this.NewIncreaseFillWidget.ScaledSuggestedWidth = 0f;
			}
			else
			{
				this.NewIncreaseFillWidget.ScaledSuggestedWidth = 0f;
				this.NewDecreaseFillWidget.ScaledSuggestedWidth = 0f;
			}
			base.OnLateUpdate(dt);
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x0600118B RID: 4491 RVA: 0x00030810 File Offset: 0x0002EA10
		// (set) Token: 0x0600118C RID: 4492 RVA: 0x00030818 File Offset: 0x0002EA18
		[Editor(false)]
		public Widget InitialFillWidget
		{
			get
			{
				return this._initialFillWidget;
			}
			set
			{
				if (this._initialFillWidget != value)
				{
					this._initialFillWidget = value;
				}
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x0600118D RID: 4493 RVA: 0x0003082A File Offset: 0x0002EA2A
		// (set) Token: 0x0600118E RID: 4494 RVA: 0x00030832 File Offset: 0x0002EA32
		[Editor(false)]
		public Widget NewIncreaseFillWidget
		{
			get
			{
				return this._newIncreaseFillWidget;
			}
			set
			{
				if (this._newIncreaseFillWidget != value)
				{
					this._newIncreaseFillWidget = value;
				}
			}
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x0600118F RID: 4495 RVA: 0x00030844 File Offset: 0x0002EA44
		// (set) Token: 0x06001190 RID: 4496 RVA: 0x0003084C File Offset: 0x0002EA4C
		[Editor(false)]
		public Widget NewDecreaseFillWidget
		{
			get
			{
				return this._newDecreaseFillWidget;
			}
			set
			{
				if (this._newDecreaseFillWidget != value)
				{
					this._newDecreaseFillWidget = value;
				}
			}
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06001191 RID: 4497 RVA: 0x0003085E File Offset: 0x0002EA5E
		// (set) Token: 0x06001192 RID: 4498 RVA: 0x00030866 File Offset: 0x0002EA66
		[Editor(false)]
		public Widget CurrentRatioIndicatorWidget
		{
			get
			{
				return this._currentRatioIndicatorWidget;
			}
			set
			{
				if (this._currentRatioIndicatorWidget != value)
				{
					this._currentRatioIndicatorWidget = value;
				}
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06001193 RID: 4499 RVA: 0x00030878 File Offset: 0x0002EA78
		// (set) Token: 0x06001194 RID: 4500 RVA: 0x00030880 File Offset: 0x0002EA80
		[Editor(false)]
		public int CurrentSize
		{
			get
			{
				return this._currentSize;
			}
			set
			{
				if (this._currentSize != value)
				{
					this._currentSize = value;
				}
			}
		}

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x06001195 RID: 4501 RVA: 0x00030892 File Offset: 0x0002EA92
		// (set) Token: 0x06001196 RID: 4502 RVA: 0x0003089A File Offset: 0x0002EA9A
		[Editor(false)]
		public int TargetSize
		{
			get
			{
				return this._targetSize;
			}
			set
			{
				if (this._targetSize != value)
				{
					this._targetSize = value;
				}
			}
		}

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06001197 RID: 4503 RVA: 0x000308AC File Offset: 0x0002EAAC
		// (set) Token: 0x06001198 RID: 4504 RVA: 0x000308B4 File Offset: 0x0002EAB4
		[Editor(false)]
		public int SizeLimit
		{
			get
			{
				return this._sizeLimit;
			}
			set
			{
				if (this._sizeLimit != value)
				{
					this._sizeLimit = value;
				}
			}
		}

		// Token: 0x04000808 RID: 2056
		private Widget _initialFillWidget;

		// Token: 0x04000809 RID: 2057
		private Widget _newIncreaseFillWidget;

		// Token: 0x0400080A RID: 2058
		private Widget _newDecreaseFillWidget;

		// Token: 0x0400080B RID: 2059
		private Widget _currentRatioIndicatorWidget;

		// Token: 0x0400080C RID: 2060
		private int _currentSize;

		// Token: 0x0400080D RID: 2061
		private int _targetSize;

		// Token: 0x0400080E RID: 2062
		private int _sizeLimit;
	}
}
