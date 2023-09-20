using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingItemStatSliderWidget : SliderWidget
	{
		public CraftingItemStatSliderWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			float num = 1f;
			float x = base.SliderArea.Size.X;
			if (MathF.Abs(base.MaxValueFloat - base.MinValueFloat) > 1E-45f)
			{
				num = (base.ValueFloat - base.MinValueFloat) / (base.MaxValueFloat - base.MinValueFloat) * x;
				if (base.ReverseDirection)
				{
					num = 1f - num;
				}
			}
			if (this.HasValidTarget && this.TargetFill != null && base.Handle != null && this.ValueText != null)
			{
				float num2 = base.SliderArea.Size.X / base.MaxValueFloat * this.TargetValue;
				int num3 = MathF.Ceiling(MathF.Min(num, num2));
				int num4 = MathF.Floor(MathF.Max(num, num2));
				base.Filler.ScaledSuggestedWidth = (float)num3;
				this.TargetFill.ScaledPositionXOffset = (float)num3;
				this.TargetFill.ScaledSuggestedWidth = (float)(num4 - num3);
				base.Handle.ScaledPositionXOffset = num2 - base.Handle.Size.X / 2f;
				string text = ((this.IsExceedingBeneficial ? (base.ValueFloat >= this.TargetValue) : (base.ValueFloat <= this.TargetValue)) ? "Bonus" : "Penalty");
				this.TargetFill.SetState(text);
				this.ValueText.SetState(text);
				if (!this.HasValidValue)
				{
					this.LabelTextWidget.SetState(text);
					return;
				}
			}
			else
			{
				base.Filler.ScaledSuggestedWidth = num;
			}
		}

		[Editor(false)]
		public TextWidget ValueText
		{
			get
			{
				return this._valueText;
			}
			set
			{
				if (value != this._valueText)
				{
					this._valueText = value;
				}
			}
		}

		[Editor(false)]
		public TextWidget LabelTextWidget
		{
			get
			{
				return this._labelTextWidget;
			}
			set
			{
				if (value != this._labelTextWidget)
				{
					this._labelTextWidget = value;
				}
			}
		}

		[Editor(false)]
		public bool HasValidTarget
		{
			get
			{
				return this._hasValidTarget;
			}
			set
			{
				if (value != this._hasValidTarget)
				{
					this._hasValidTarget = value;
				}
			}
		}

		[Editor(false)]
		public bool HasValidValue
		{
			get
			{
				return this._hasValidValue;
			}
			set
			{
				if (value != this._hasValidValue)
				{
					this._hasValidValue = value;
				}
			}
		}

		[Editor(false)]
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

		[Editor(false)]
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

		[Editor(false)]
		public BrushWidget TargetFill
		{
			get
			{
				return this._targetFill;
			}
			set
			{
				if (value != this._targetFill)
				{
					this._targetFill = value;
				}
			}
		}

		private bool _hasValidTarget;

		private bool _hasValidValue;

		private bool _isExceedingBeneficial;

		private float _targetValue;

		private BrushWidget _targetFill;

		private TextWidget _valueText;

		private TextWidget _labelTextWidget;
	}
}
