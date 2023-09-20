using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200000F RID: 15
	public class ColorButtonWidget : ButtonWidget
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00003597 File Offset: 0x00001797
		public ColorButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000035A0 File Offset: 0x000017A0
		private void ApplyStringColorToBrush(string color)
		{
			Color color2 = Color.ConvertStringToColor(color);
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = color2;
				}
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00003638 File Offset: 0x00001838
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00003640 File Offset: 0x00001840
		[Editor(false)]
		public string ColorToApply
		{
			get
			{
				return this._colorToApply;
			}
			set
			{
				if (this._colorToApply != value)
				{
					this._colorToApply = value;
					base.OnPropertyChanged<string>(value, "ColorToApply");
					if (!string.IsNullOrEmpty(value))
					{
						this.ApplyStringColorToBrush(value);
					}
				}
			}
		}

		// Token: 0x04000049 RID: 73
		private string _colorToApply;
	}
}
