using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftedWeaponDesignResultListPanel : ListPanel
	{
		public CounterTextBrushWidget ChangeValueTextWidget { get; set; }

		public CounterTextBrushWidget ValueTextWidget { get; set; }

		public RichTextWidget GoldEffectorTextWidget { get; set; }

		public Brush PositiveChangeBrush { get; set; }

		public Brush NegativeChangeBrush { get; set; }

		public Brush NeutralBrush { get; set; }

		public float FadeInTimeIndexOffset { get; set; } = 2f;

		public float FadeInTime { get; set; } = 0.5f;

		public float CounterStartTime { get; set; } = 2f;

		private bool _hasChange
		{
			get
			{
				return this.ChangeAmount != 0f;
			}
		}

		private float _valueTextStartFadeInTime
		{
			get
			{
				return (float)base.GetSiblingIndex() * this.FadeInTimeIndexOffset;
			}
		}

		public CraftedWeaponDesignResultListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.ValueTextWidget.FloatTarget = this.InitValue;
				this.ValueTextWidget.ForceSetValue(this.InitValue);
				if (this._hasChange)
				{
					this.ValueTextWidget.Brush = ((this.ChangeAmount > 0f) ? this.PositiveChangeBrush : this.NegativeChangeBrush);
					this.ChangeValueTextWidget.Brush = ((this.ChangeAmount > 0f) ? this.PositiveChangeBrush : this.NegativeChangeBrush);
					this.ChangeValueTextWidget.IsVisible = true;
				}
				else
				{
					this.ChangeValueTextWidget.IsVisible = false;
					this.ValueTextWidget.Brush = this.NeutralBrush;
				}
				this.ChangeValueTextWidget.SetGlobalAlphaRecursively(0f);
				this.ValueTextWidget.SetGlobalAlphaRecursively(0f);
				this.ChangeValueTextWidget.ShowSign = true;
				if (this.InitValue == 0f)
				{
					this.LabelTextWidget.SetState(this._isExceedingBeneficial ? "Bonus" : "Penalty");
				}
				this._initialized = true;
			}
			if (this._totalTime > this._valueTextStartFadeInTime)
			{
				float num = (this._totalTime - this._valueTextStartFadeInTime) / this.FadeInTime;
				if (num >= 0f && num <= 1f)
				{
					float num2 = MathF.Lerp(0f, 1f, num, 1E-05f);
					if (num2 < 1f)
					{
						this.ValueTextWidget.SetGlobalAlphaRecursively(num2);
					}
				}
				if (this._hasChange && this._totalTime > this._valueTextStartFadeInTime + this.CounterStartTime)
				{
					this.ValueTextWidget.FloatTarget = this.InitValue + this.ChangeAmount;
					num = (this._totalTime - this._valueTextStartFadeInTime - this.FadeInTime) / this.FadeInTime;
					if (num >= 0f && num <= 1f)
					{
						float num3 = MathF.Lerp(0f, 1f, num, 1E-05f);
						if (num3 < 1f)
						{
							this.ChangeValueTextWidget.SetGlobalAlphaRecursively(num3);
						}
					}
					this.ChangeValueTextWidget.FloatTarget = this.ChangeAmount;
				}
			}
			this._totalTime += dt;
		}

		public RichTextWidget LabelTextWidget
		{
			get
			{
				return this._labelTextWidget;
			}
			set
			{
				if (this._labelTextWidget != value)
				{
					this._labelTextWidget = value;
				}
			}
		}

		public float InitValue
		{
			get
			{
				return this._initValue;
			}
			set
			{
				if (this._initValue != value)
				{
					this._initValue = value;
				}
			}
		}

		public float ChangeAmount
		{
			get
			{
				return this._changeAmount;
			}
			set
			{
				if (this._changeAmount != value)
				{
					this._changeAmount = value;
				}
			}
		}

		public bool IsExceedingBeneficial
		{
			get
			{
				return this._isExceedingBeneficial;
			}
			set
			{
				if (value != this._isExceedingBeneficial)
				{
					this._isExceedingBeneficial = value;
				}
			}
		}

		public float TargetValue
		{
			get
			{
				return this._targetValue;
			}
			set
			{
				if (value != this._targetValue)
				{
					this._targetValue = value;
				}
			}
		}

		public bool IsOrderResult
		{
			get
			{
				return this._isOrderResult;
			}
			set
			{
				if (value != this._isOrderResult)
				{
					this._isOrderResult = value;
				}
			}
		}

		private bool _initialized;

		private float _totalTime;

		private RichTextWidget _labelTextWidget;

		private float _initValue;

		private float _changeAmount;

		private float _targetValue;

		private bool _isExceedingBeneficial;

		private bool _isOrderResult;
	}
}
