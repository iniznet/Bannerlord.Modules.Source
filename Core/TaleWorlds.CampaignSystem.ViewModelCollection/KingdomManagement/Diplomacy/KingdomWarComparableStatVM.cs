using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	// Token: 0x0200005F RID: 95
	public class KingdomWarComparableStatVM : ViewModel
	{
		// Token: 0x06000833 RID: 2099 RVA: 0x00022F94 File Offset: 0x00021194
		public KingdomWarComparableStatVM(int faction1Stat, int faction2Stat, TextObject name, string faction1Color, string faction2Color, int defaultRange, BasicTooltipViewModel faction1Hint = null, BasicTooltipViewModel faction2Hint = null)
		{
			int num = MathF.Max(MathF.Max(faction1Stat, faction2Stat), defaultRange);
			if (num == 0)
			{
				num = 1;
			}
			this.Faction1Color = faction1Color;
			this.Faction2Color = faction2Color;
			this.Faction1Value = faction1Stat;
			this.Faction2Value = faction2Stat;
			this._defaultRange = defaultRange;
			this.Faction1Percentage = MathF.Round((float)faction1Stat / (float)num * 100f);
			this.Faction2Percentage = MathF.Round((float)faction2Stat / (float)num * 100f);
			this._nameObj = name;
			this.Faction1Hint = faction1Hint;
			this.Faction2Hint = faction2Hint;
			this.RefreshValues();
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0002302A File Offset: 0x0002122A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameObj.ToString();
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000835 RID: 2101 RVA: 0x00023043 File Offset: 0x00021243
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x0002304B File Offset: 0x0002124B
		[DataSourceProperty]
		public BasicTooltipViewModel Faction1Hint
		{
			get
			{
				return this._faction1Hint;
			}
			set
			{
				if (value != this._faction1Hint)
				{
					this._faction1Hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Faction1Hint");
				}
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000837 RID: 2103 RVA: 0x00023069 File Offset: 0x00021269
		// (set) Token: 0x06000838 RID: 2104 RVA: 0x00023071 File Offset: 0x00021271
		[DataSourceProperty]
		public BasicTooltipViewModel Faction2Hint
		{
			get
			{
				return this._faction2Hint;
			}
			set
			{
				if (value != this._faction2Hint)
				{
					this._faction2Hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Faction2Hint");
				}
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x0002308F File Offset: 0x0002128F
		// (set) Token: 0x0600083A RID: 2106 RVA: 0x00023097 File Offset: 0x00021297
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x000230BA File Offset: 0x000212BA
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x000230C2 File Offset: 0x000212C2
		[DataSourceProperty]
		public string Faction1Color
		{
			get
			{
				return this._faction1Color;
			}
			set
			{
				if (value != this._faction1Color)
				{
					this._faction1Color = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction1Color");
				}
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x0600083D RID: 2109 RVA: 0x000230E5 File Offset: 0x000212E5
		// (set) Token: 0x0600083E RID: 2110 RVA: 0x000230ED File Offset: 0x000212ED
		[DataSourceProperty]
		public string Faction2Color
		{
			get
			{
				return this._faction2Color;
			}
			set
			{
				if (value != this._faction2Color)
				{
					this._faction2Color = value;
					base.OnPropertyChangedWithValue<string>(value, "Faction2Color");
				}
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x0600083F RID: 2111 RVA: 0x00023110 File Offset: 0x00021310
		// (set) Token: 0x06000840 RID: 2112 RVA: 0x00023118 File Offset: 0x00021318
		[DataSourceProperty]
		public int Faction1Percentage
		{
			get
			{
				return this._faction1Percentage;
			}
			set
			{
				if (value != this._faction1Percentage)
				{
					this._faction1Percentage = value;
					base.OnPropertyChangedWithValue(value, "Faction1Percentage");
				}
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000841 RID: 2113 RVA: 0x00023136 File Offset: 0x00021336
		// (set) Token: 0x06000842 RID: 2114 RVA: 0x0002313E File Offset: 0x0002133E
		[DataSourceProperty]
		public int Faction1Value
		{
			get
			{
				return this._faction1Value;
			}
			set
			{
				if (value != this._faction1Value)
				{
					this._faction1Value = value;
					base.OnPropertyChangedWithValue(value, "Faction1Value");
				}
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000843 RID: 2115 RVA: 0x0002315C File Offset: 0x0002135C
		// (set) Token: 0x06000844 RID: 2116 RVA: 0x00023164 File Offset: 0x00021364
		[DataSourceProperty]
		public int Faction2Percentage
		{
			get
			{
				return this._faction2Percentage;
			}
			set
			{
				if (value != this._faction2Percentage)
				{
					this._faction2Percentage = value;
					base.OnPropertyChangedWithValue(value, "Faction2Percentage");
				}
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000845 RID: 2117 RVA: 0x00023182 File Offset: 0x00021382
		// (set) Token: 0x06000846 RID: 2118 RVA: 0x0002318A File Offset: 0x0002138A
		[DataSourceProperty]
		public int Faction2Value
		{
			get
			{
				return this._faction2Value;
			}
			set
			{
				if (value != this._faction2Value)
				{
					this._faction2Value = value;
					base.OnPropertyChangedWithValue(value, "Faction2Value");
				}
			}
		}

		// Token: 0x040003A7 RID: 935
		private TextObject _nameObj;

		// Token: 0x040003A8 RID: 936
		private int _defaultRange;

		// Token: 0x040003A9 RID: 937
		private BasicTooltipViewModel _faction1Hint;

		// Token: 0x040003AA RID: 938
		private BasicTooltipViewModel _faction2Hint;

		// Token: 0x040003AB RID: 939
		private string _name;

		// Token: 0x040003AC RID: 940
		private string _faction1Color;

		// Token: 0x040003AD RID: 941
		private string _faction2Color;

		// Token: 0x040003AE RID: 942
		private int _faction1Percentage;

		// Token: 0x040003AF RID: 943
		private int _faction1Value;

		// Token: 0x040003B0 RID: 944
		private int _faction2Percentage;

		// Token: 0x040003B1 RID: 945
		private int _faction2Value;
	}
}
