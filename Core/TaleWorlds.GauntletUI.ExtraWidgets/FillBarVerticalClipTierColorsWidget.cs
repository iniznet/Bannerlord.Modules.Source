using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x02000007 RID: 7
	public class FillBarVerticalClipTierColorsWidget : FillBarVerticalWidget
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00002D5C File Offset: 0x00000F5C
		public FillBarVerticalClipTierColorsWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002D88 File Offset: 0x00000F88
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			float num = (float)base.InitialAmount / base.MaxAmountAsFloat;
			Color color = new Color(0f, 0f, 0f, 0f);
			if (num == 1f)
			{
				base.FillWidget.Color = Color.ConvertStringToColor(this.MaxedColor);
				return;
			}
			float num2 = this._maxThreshold;
			float num3 = this._maxThreshold;
			Color color2 = Color.ConvertStringToColor(this.MaxedColor);
			Color color3 = Color.ConvertStringToColor(this.MaxedColor);
			if (num >= this._highThreshold && num < this._maxThreshold)
			{
				num2 = this._highThreshold;
				num3 = this._maxThreshold;
				color2 = Color.ConvertStringToColor(this.HighColor);
				color3 = Color.ConvertStringToColor(this.MaxedColor);
			}
			else if (num >= this._mediumThreshold && num < this._highThreshold)
			{
				num2 = this._mediumThreshold;
				num3 = this._highThreshold;
				color2 = Color.ConvertStringToColor(this.MediumColor);
				color3 = Color.ConvertStringToColor(this.HighColor);
			}
			else if (num >= this._lowThreshold && num < this._mediumThreshold)
			{
				num2 = this._lowThreshold;
				num3 = this._mediumThreshold;
				color2 = Color.ConvertStringToColor(this.LowColor);
				color3 = Color.ConvertStringToColor(this.MediumColor);
			}
			float num4 = (num - num2) / (num3 - num2);
			color = Color.Lerp(color2, color3, num4);
			base.FillWidget.Color = color;
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002EE1 File Offset: 0x000010E1
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002EE9 File Offset: 0x000010E9
		[Editor(false)]
		public string MaxedColor
		{
			get
			{
				return this._maxedColor;
			}
			set
			{
				if (value != this._maxedColor)
				{
					this._maxedColor = value;
					base.OnPropertyChanged<string>(value, "MaxedColor");
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002F0C File Offset: 0x0000110C
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002F14 File Offset: 0x00001114
		[Editor(false)]
		public string HighColor
		{
			get
			{
				return this._highColor;
			}
			set
			{
				if (value != this._highColor)
				{
					this._highColor = value;
					base.OnPropertyChanged<string>(value, "HighColor");
				}
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002F37 File Offset: 0x00001137
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00002F3F File Offset: 0x0000113F
		[Editor(false)]
		public string MediumColor
		{
			get
			{
				return this._mediumColor;
			}
			set
			{
				if (value != this._mediumColor)
				{
					this._mediumColor = value;
					base.OnPropertyChanged<string>(value, "MediumColor");
				}
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002F62 File Offset: 0x00001162
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00002F6A File Offset: 0x0000116A
		[Editor(false)]
		public string LowColor
		{
			get
			{
				return this._lowColor;
			}
			set
			{
				if (value != this._lowColor)
				{
					this._lowColor = value;
					base.OnPropertyChanged<string>(value, "LowColor");
				}
			}
		}

		// Token: 0x04000024 RID: 36
		private readonly float _maxThreshold = 1f;

		// Token: 0x04000025 RID: 37
		private readonly float _highThreshold = 0.6f;

		// Token: 0x04000026 RID: 38
		private readonly float _mediumThreshold = 0.35f;

		// Token: 0x04000027 RID: 39
		private readonly float _lowThreshold;

		// Token: 0x04000028 RID: 40
		private string _maxedColor;

		// Token: 0x04000029 RID: 41
		private string _highColor;

		// Token: 0x0400002A RID: 42
		private string _mediumColor;

		// Token: 0x0400002B RID: 43
		private string _lowColor;
	}
}
