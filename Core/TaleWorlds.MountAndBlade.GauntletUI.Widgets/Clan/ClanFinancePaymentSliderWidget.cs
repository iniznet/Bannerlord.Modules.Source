using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	public class ClanFinancePaymentSliderWidget : SliderWidget
	{
		public ClanFinancePaymentSliderWidget(UIContext context)
			: base(context)
		{
		}

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

		private Widget _initialFillWidget;

		private Widget _newIncreaseFillWidget;

		private Widget _newDecreaseFillWidget;

		private Widget _currentRatioIndicatorWidget;

		private int _currentSize;

		private int _targetSize;

		private int _sizeLimit;
	}
}
