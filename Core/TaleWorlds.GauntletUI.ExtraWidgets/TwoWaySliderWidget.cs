using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x02000011 RID: 17
	public class TwoWaySliderWidget : SliderWidget
	{
		// Token: 0x060000FD RID: 253 RVA: 0x0000620B File Offset: 0x0000440B
		public TwoWaySliderWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006214 File Offset: 0x00004414
		protected override void OnValueIntChanged(int value)
		{
			base.OnValueIntChanged(value);
			if (this.ChangeFillWidget == null || base.MaxValueInt == 0)
			{
				return;
			}
			float num = base.Size.X / base._scaleToUse;
			float num2 = (float)this.BaseValueInt / base.MaxValueFloat * num;
			if (value < this.BaseValueInt)
			{
				this.ChangeFillWidget.SetState("Positive");
				this.ChangeFillWidget.SuggestedWidth = (float)(this.BaseValueInt - value) / base.MaxValueFloat * num;
				this.ChangeFillWidget.PositionXOffset = num2 - this.ChangeFillWidget.SuggestedWidth;
			}
			else if (value > this.BaseValueInt)
			{
				this.ChangeFillWidget.SetState("Negative");
				this.ChangeFillWidget.SuggestedWidth = (float)(value - this.BaseValueInt) / base.MaxValueFloat * num;
				this.ChangeFillWidget.PositionXOffset = num2;
			}
			else
			{
				this.ChangeFillWidget.SetState("Default");
				this.ChangeFillWidget.SuggestedWidth = 0f;
			}
			if (this._handleClicked || this._valueChangedByMouse || this._manuallyIncreased)
			{
				this._manuallyIncreased = false;
				base.OnPropertyChanged(base.ValueInt, "ValueInt");
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006341 File Offset: 0x00004541
		private void ChangeFillWidgetUpdated()
		{
			if (this.ChangeFillWidget != null)
			{
				this.ChangeFillWidget.AddState("Negative");
				this.ChangeFillWidget.AddState("Positive");
				this.ChangeFillWidget.HorizontalAlignment = HorizontalAlignment.Left;
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006377 File Offset: 0x00004577
		private void BaseValueIntUpdated()
		{
			this.OnValueIntChanged(base.ValueInt);
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00006385 File Offset: 0x00004585
		// (set) Token: 0x06000102 RID: 258 RVA: 0x0000638D File Offset: 0x0000458D
		[Editor(false)]
		public BrushWidget ChangeFillWidget
		{
			get
			{
				return this._changeFillWidget;
			}
			set
			{
				if (this._changeFillWidget != value)
				{
					this._changeFillWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "ChangeFillWidget");
					this.ChangeFillWidgetUpdated();
				}
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000103 RID: 259 RVA: 0x000063B1 File Offset: 0x000045B1
		// (set) Token: 0x06000104 RID: 260 RVA: 0x000063B9 File Offset: 0x000045B9
		[Editor(false)]
		public int BaseValueInt
		{
			get
			{
				return this._baseValueInt;
			}
			set
			{
				if (this._baseValueInt != value)
				{
					this._baseValueInt = value;
					base.OnPropertyChanged(value, "BaseValueInt");
					this.BaseValueIntUpdated();
				}
			}
		}

		// Token: 0x04000076 RID: 118
		protected bool _manuallyIncreased;

		// Token: 0x04000077 RID: 119
		private BrushWidget _changeFillWidget;

		// Token: 0x04000078 RID: 120
		private int _baseValueInt;
	}
}
