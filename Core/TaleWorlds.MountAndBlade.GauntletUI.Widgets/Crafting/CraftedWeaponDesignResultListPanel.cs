using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000141 RID: 321
	public class CraftedWeaponDesignResultListPanel : ListPanel
	{
		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x060010CF RID: 4303 RVA: 0x0002EFE6 File Offset: 0x0002D1E6
		// (set) Token: 0x060010D0 RID: 4304 RVA: 0x0002EFEE File Offset: 0x0002D1EE
		public CounterTextBrushWidget ChangeValueTextWidget { get; set; }

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x060010D1 RID: 4305 RVA: 0x0002EFF7 File Offset: 0x0002D1F7
		// (set) Token: 0x060010D2 RID: 4306 RVA: 0x0002EFFF File Offset: 0x0002D1FF
		public CounterTextBrushWidget ValueTextWidget { get; set; }

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x060010D3 RID: 4307 RVA: 0x0002F008 File Offset: 0x0002D208
		// (set) Token: 0x060010D4 RID: 4308 RVA: 0x0002F010 File Offset: 0x0002D210
		public RichTextWidget GoldEffectorTextWidget { get; set; }

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x0002F019 File Offset: 0x0002D219
		// (set) Token: 0x060010D6 RID: 4310 RVA: 0x0002F021 File Offset: 0x0002D221
		public Brush PositiveChangeBrush { get; set; }

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x060010D7 RID: 4311 RVA: 0x0002F02A File Offset: 0x0002D22A
		// (set) Token: 0x060010D8 RID: 4312 RVA: 0x0002F032 File Offset: 0x0002D232
		public Brush NegativeChangeBrush { get; set; }

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x060010D9 RID: 4313 RVA: 0x0002F03B File Offset: 0x0002D23B
		// (set) Token: 0x060010DA RID: 4314 RVA: 0x0002F043 File Offset: 0x0002D243
		public Brush NeutralBrush { get; set; }

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x060010DB RID: 4315 RVA: 0x0002F04C File Offset: 0x0002D24C
		// (set) Token: 0x060010DC RID: 4316 RVA: 0x0002F054 File Offset: 0x0002D254
		public float FadeInTimeIndexOffset { get; set; } = 2f;

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x060010DD RID: 4317 RVA: 0x0002F05D File Offset: 0x0002D25D
		// (set) Token: 0x060010DE RID: 4318 RVA: 0x0002F065 File Offset: 0x0002D265
		public float FadeInTime { get; set; } = 0.5f;

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x060010DF RID: 4319 RVA: 0x0002F06E File Offset: 0x0002D26E
		// (set) Token: 0x060010E0 RID: 4320 RVA: 0x0002F076 File Offset: 0x0002D276
		public float CounterStartTime { get; set; } = 2f;

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x060010E1 RID: 4321 RVA: 0x0002F07F File Offset: 0x0002D27F
		private bool _hasChange
		{
			get
			{
				return this.ChangeAmount != 0f;
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x060010E2 RID: 4322 RVA: 0x0002F091 File Offset: 0x0002D291
		private float _valueTextStartFadeInTime
		{
			get
			{
				return (float)base.GetSiblingIndex() * this.FadeInTimeIndexOffset;
			}
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x0002F0A1 File Offset: 0x0002D2A1
		public CraftedWeaponDesignResultListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x0002F0CC File Offset: 0x0002D2CC
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

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x060010E5 RID: 4325 RVA: 0x0002F2F8 File Offset: 0x0002D4F8
		// (set) Token: 0x060010E6 RID: 4326 RVA: 0x0002F300 File Offset: 0x0002D500
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

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x060010E7 RID: 4327 RVA: 0x0002F312 File Offset: 0x0002D512
		// (set) Token: 0x060010E8 RID: 4328 RVA: 0x0002F31A File Offset: 0x0002D51A
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

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x060010E9 RID: 4329 RVA: 0x0002F32C File Offset: 0x0002D52C
		// (set) Token: 0x060010EA RID: 4330 RVA: 0x0002F334 File Offset: 0x0002D534
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

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x0002F346 File Offset: 0x0002D546
		// (set) Token: 0x060010EC RID: 4332 RVA: 0x0002F34E File Offset: 0x0002D54E
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

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x060010ED RID: 4333 RVA: 0x0002F360 File Offset: 0x0002D560
		// (set) Token: 0x060010EE RID: 4334 RVA: 0x0002F368 File Offset: 0x0002D568
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

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x0002F37A File Offset: 0x0002D57A
		// (set) Token: 0x060010F0 RID: 4336 RVA: 0x0002F382 File Offset: 0x0002D582
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

		// Token: 0x040007BF RID: 1983
		private bool _initialized;

		// Token: 0x040007C0 RID: 1984
		private float _totalTime;

		// Token: 0x040007C1 RID: 1985
		private RichTextWidget _labelTextWidget;

		// Token: 0x040007C2 RID: 1986
		private float _initValue;

		// Token: 0x040007C3 RID: 1987
		private float _changeAmount;

		// Token: 0x040007C4 RID: 1988
		private float _targetValue;

		// Token: 0x040007C5 RID: 1989
		private bool _isExceedingBeneficial;

		// Token: 0x040007C6 RID: 1990
		private bool _isOrderResult;
	}
}
