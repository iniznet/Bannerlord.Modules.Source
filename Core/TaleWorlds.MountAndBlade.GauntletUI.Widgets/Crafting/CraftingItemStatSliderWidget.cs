using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000143 RID: 323
	public class CraftingItemStatSliderWidget : SliderWidget
	{
		// Token: 0x060010F3 RID: 4339 RVA: 0x0002F3F2 File Offset: 0x0002D5F2
		public CraftingItemStatSliderWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x0002F3FC File Offset: 0x0002D5FC
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

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x060010F5 RID: 4341 RVA: 0x0002F599 File Offset: 0x0002D799
		// (set) Token: 0x060010F6 RID: 4342 RVA: 0x0002F5A1 File Offset: 0x0002D7A1
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

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x060010F7 RID: 4343 RVA: 0x0002F5B3 File Offset: 0x0002D7B3
		// (set) Token: 0x060010F8 RID: 4344 RVA: 0x0002F5BB File Offset: 0x0002D7BB
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

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x060010F9 RID: 4345 RVA: 0x0002F5CD File Offset: 0x0002D7CD
		// (set) Token: 0x060010FA RID: 4346 RVA: 0x0002F5D5 File Offset: 0x0002D7D5
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

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x060010FB RID: 4347 RVA: 0x0002F5E7 File Offset: 0x0002D7E7
		// (set) Token: 0x060010FC RID: 4348 RVA: 0x0002F5EF File Offset: 0x0002D7EF
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

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x060010FD RID: 4349 RVA: 0x0002F601 File Offset: 0x0002D801
		// (set) Token: 0x060010FE RID: 4350 RVA: 0x0002F609 File Offset: 0x0002D809
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

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x060010FF RID: 4351 RVA: 0x0002F61B File Offset: 0x0002D81B
		// (set) Token: 0x06001100 RID: 4352 RVA: 0x0002F623 File Offset: 0x0002D823
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

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06001101 RID: 4353 RVA: 0x0002F635 File Offset: 0x0002D835
		// (set) Token: 0x06001102 RID: 4354 RVA: 0x0002F63D File Offset: 0x0002D83D
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

		// Token: 0x040007C9 RID: 1993
		private bool _hasValidTarget;

		// Token: 0x040007CA RID: 1994
		private bool _hasValidValue;

		// Token: 0x040007CB RID: 1995
		private bool _isExceedingBeneficial;

		// Token: 0x040007CC RID: 1996
		private float _targetValue;

		// Token: 0x040007CD RID: 1997
		private BrushWidget _targetFill;

		// Token: 0x040007CE RID: 1998
		private TextWidget _valueText;

		// Token: 0x040007CF RID: 1999
		private TextWidget _labelTextWidget;
	}
}
