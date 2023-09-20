using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	// Token: 0x020000D7 RID: 215
	public class CraftingListPropertyItem : ViewModel
	{
		// Token: 0x060013E1 RID: 5089 RVA: 0x0004BC9C File Offset: 0x00049E9C
		public CraftingListPropertyItem(TextObject description, float maxValue, float value, float targetValue, CraftingTemplate.CraftingStatTypes propertyType, bool isAlternativeUsageProperty = false)
		{
			this.Description = description;
			this.PropertyMaxValue = maxValue;
			this.PropertyValue = value;
			this.TargetValue = targetValue;
			this.IsAlternativeUsageProperty = isAlternativeUsageProperty;
			this.Type = propertyType;
			this.RefreshValues();
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0004BCF0 File Offset: 0x00049EF0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.HasValidTarget = this.TargetValue > float.Epsilon;
			this.HasValidValue = this.PropertyValue > float.Epsilon;
			TextObject description = this.Description;
			this.PropertyLbl = ((description != null) ? description.ToString() : null);
			this.IsExceedingBeneficial = this.CheckIfExceedingIsBeneficial();
			this.SeparatorText = new TextObject("{=dB6cFDmz}/", null).ToString();
			bool flag = this.CheckIfUsingIntegerValue(this.PropertyValue, this.Type);
			this.PropertyValueText = (flag ? this.PropertyValue.ToString("F0") : ((this.PropertyValue < 10f) ? this.PropertyValue.ToString("F2") : this.PropertyValue.ToString("F1")));
			if (this.HasValidTarget)
			{
				this.TargetValueText = (flag ? this.TargetValue.ToString("F0") : ((this.TargetValue < 10f) ? this.TargetValue.ToString("F2") : this.TargetValue.ToString("F1")));
			}
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x0004BE24 File Offset: 0x0004A024
		private bool CheckIfExceedingIsBeneficial()
		{
			return this.Type > CraftingTemplate.CraftingStatTypes.Weight;
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x0004BE30 File Offset: 0x0004A030
		private bool CheckIfUsingIntegerValue(float propertyValue, CraftingTemplate.CraftingStatTypes type)
		{
			bool flag = type == CraftingTemplate.CraftingStatTypes.StackAmount;
			bool flag2 = propertyValue >= 100f;
			return flag || flag2;
		}

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x060013E5 RID: 5093 RVA: 0x0004BE50 File Offset: 0x0004A050
		// (set) Token: 0x060013E6 RID: 5094 RVA: 0x0004BE58 File Offset: 0x0004A058
		[DataSourceProperty]
		public bool IsValidForUsage
		{
			get
			{
				return this._showStats;
			}
			set
			{
				if (value != this._showStats)
				{
					this._showStats = value;
					base.OnPropertyChangedWithValue(value, "IsValidForUsage");
				}
			}
		}

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x060013E7 RID: 5095 RVA: 0x0004BE76 File Offset: 0x0004A076
		// (set) Token: 0x060013E8 RID: 5096 RVA: 0x0004BE7E File Offset: 0x0004A07E
		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue(value, "IsExceedingBeneficial");
				}
			}
		}

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x060013E9 RID: 5097 RVA: 0x0004BE9C File Offset: 0x0004A09C
		// (set) Token: 0x060013EA RID: 5098 RVA: 0x0004BEA4 File Offset: 0x0004A0A4
		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue(value, "HasValidTarget");
				}
			}
		}

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x060013EB RID: 5099 RVA: 0x0004BEC2 File Offset: 0x0004A0C2
		// (set) Token: 0x060013EC RID: 5100 RVA: 0x0004BECA File Offset: 0x0004A0CA
		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue(value, "HasValidValue");
				}
			}
		}

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x060013ED RID: 5101 RVA: 0x0004BEE8 File Offset: 0x0004A0E8
		// (set) Token: 0x060013EE RID: 5102 RVA: 0x0004BEF0 File Offset: 0x0004A0F0
		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue(value, "TargetValue");
				}
			}
		}

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x060013EF RID: 5103 RVA: 0x0004BF0E File Offset: 0x0004A10E
		// (set) Token: 0x060013F0 RID: 5104 RVA: 0x0004BF16 File Offset: 0x0004A116
		[DataSourceProperty]
		public string TargetValueText
		{
			get
			{
				return this._targetValueText;
			}
			set
			{
				if (value != this._targetValueText)
				{
					this._targetValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "TargetValueText");
				}
			}
		}

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x060013F1 RID: 5105 RVA: 0x0004BF39 File Offset: 0x0004A139
		// (set) Token: 0x060013F2 RID: 5106 RVA: 0x0004BF41 File Offset: 0x0004A141
		[DataSourceProperty]
		public bool IsAlternativeUsageProperty
		{
			get
			{
				return this._isAlternativeUsageProperty;
			}
			set
			{
				if (this._isAlternativeUsageProperty != value)
				{
					this._isAlternativeUsageProperty = value;
					base.OnPropertyChangedWithValue(value, "IsAlternativeUsageProperty");
				}
			}
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0004BF5F File Offset: 0x0004A15F
		// (set) Token: 0x060013F4 RID: 5108 RVA: 0x0004BF67 File Offset: 0x0004A167
		[DataSourceProperty]
		public string PropertyLbl
		{
			get
			{
				return this._propertyLbl;
			}
			set
			{
				if (value != this._propertyLbl)
				{
					this._propertyLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "PropertyLbl");
				}
			}
		}

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x060013F5 RID: 5109 RVA: 0x0004BF8A File Offset: 0x0004A18A
		// (set) Token: 0x060013F6 RID: 5110 RVA: 0x0004BF92 File Offset: 0x0004A192
		[DataSourceProperty]
		public float PropertyValue
		{
			get
			{
				return this._propertyValue;
			}
			set
			{
				if (value == 0f || value != this._propertyValue)
				{
					this._propertyValue = value;
					base.OnPropertyChangedWithValue(value, "PropertyValue");
				}
			}
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x060013F7 RID: 5111 RVA: 0x0004BFB8 File Offset: 0x0004A1B8
		// (set) Token: 0x060013F8 RID: 5112 RVA: 0x0004BFC0 File Offset: 0x0004A1C0
		[DataSourceProperty]
		public float PropertyMaxValue
		{
			get
			{
				return this._propertyMaxValue;
			}
			set
			{
				if (value != this._propertyMaxValue)
				{
					this._propertyMaxValue = value;
					base.OnPropertyChangedWithValue(value, "PropertyMaxValue");
				}
			}
		}

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x060013F9 RID: 5113 RVA: 0x0004BFDE File Offset: 0x0004A1DE
		// (set) Token: 0x060013FA RID: 5114 RVA: 0x0004BFE6 File Offset: 0x0004A1E6
		[DataSourceProperty]
		public string PropertyValueText
		{
			get
			{
				return this._propertyValueText;
			}
			set
			{
				if (this._propertyValueText != value)
				{
					this._propertyValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "PropertyValueText");
				}
			}
		}

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x060013FB RID: 5115 RVA: 0x0004C009 File Offset: 0x0004A209
		// (set) Token: 0x060013FC RID: 5116 RVA: 0x0004C011 File Offset: 0x0004A211
		[DataSourceProperty]
		public string SeparatorText
		{
			get
			{
				return this._separatorText;
			}
			set
			{
				if (value != this._separatorText)
				{
					this._separatorText = value;
					base.OnPropertyChangedWithValue<string>(value, "SeparatorText");
				}
			}
		}

		// Token: 0x04000943 RID: 2371
		public readonly TextObject Description;

		// Token: 0x04000944 RID: 2372
		public readonly CraftingTemplate.CraftingStatTypes Type;

		// Token: 0x04000945 RID: 2373
		private bool _showStats;

		// Token: 0x04000946 RID: 2374
		private bool _isExceedingBeneficial;

		// Token: 0x04000947 RID: 2375
		private bool _hasValidTarget;

		// Token: 0x04000948 RID: 2376
		private bool _hasValidValue;

		// Token: 0x04000949 RID: 2377
		private float _targetValue;

		// Token: 0x0400094A RID: 2378
		private string _targetValueText;

		// Token: 0x0400094B RID: 2379
		private string _propertyLbl;

		// Token: 0x0400094C RID: 2380
		private float _propertyValue;

		// Token: 0x0400094D RID: 2381
		private float _propertyMaxValue = -1f;

		// Token: 0x0400094E RID: 2382
		private string _propertyValueText;

		// Token: 0x0400094F RID: 2383
		public bool _isAlternativeUsageProperty;

		// Token: 0x04000950 RID: 2384
		private string _separatorText;
	}
}
