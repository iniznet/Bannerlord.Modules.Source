using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	// Token: 0x0200010A RID: 266
	public class MapBarTextWidget : TextWidget
	{
		// Token: 0x06000DA8 RID: 3496 RVA: 0x00026410 File Offset: 0x00024610
		public MapBarTextWidget(UIContext context)
			: base(context)
		{
			base.intPropertyChanged += this.TextPropertyChanged;
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x0002643C File Offset: 0x0002463C
		private void TextPropertyChanged(PropertyOwnerObject widget, string propertyName, int propertyValue)
		{
			if (propertyName == "IntText")
			{
				if (this._prevValue != -99)
				{
					if (propertyValue - this._prevValue > 0)
					{
						if (base.CurrentState == "Positive")
						{
							base.BrushRenderer.RestartAnimation();
						}
						else
						{
							this.SetState("Positive");
						}
					}
					else if (propertyValue - this._prevValue < 0)
					{
						if (base.CurrentState == "Negative")
						{
							base.BrushRenderer.RestartAnimation();
						}
						else
						{
							this.SetState("Negative");
						}
					}
				}
				this._prevValue = propertyValue;
			}
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06000DAA RID: 3498 RVA: 0x000264D5 File Offset: 0x000246D5
		// (set) Token: 0x06000DAB RID: 3499 RVA: 0x000264E0 File Offset: 0x000246E0
		[Editor(false)]
		public bool IsWarning
		{
			get
			{
				return this._isWarning;
			}
			set
			{
				if (value != this._isWarning)
				{
					this._isWarning = value;
					base.OnPropertyChanged(value, "IsWarning");
					base.ReadOnlyBrush.GetStyleOrDefault(base.CurrentState);
					Color color = Color.Black;
					if (value)
					{
						color = this.WarningColor;
					}
					else
					{
						color = this.NormalColor;
					}
					foreach (Style style in base.Brush.Styles)
					{
						style.FontColor = color;
					}
				}
			}
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x00026580 File Offset: 0x00024780
		// (set) Token: 0x06000DAD RID: 3501 RVA: 0x00026588 File Offset: 0x00024788
		[Editor(false)]
		public Color NormalColor
		{
			get
			{
				return this._normalColor;
			}
			set
			{
				if (value.Alpha != this._normalColor.Alpha || value.Blue != this._normalColor.Blue || value.Red != this._normalColor.Red || value.Green != this._normalColor.Green)
				{
					this._normalColor = value;
					base.OnPropertyChanged(value, "NormalColor");
				}
			}
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06000DAE RID: 3502 RVA: 0x000265F4 File Offset: 0x000247F4
		// (set) Token: 0x06000DAF RID: 3503 RVA: 0x000265FC File Offset: 0x000247FC
		[Editor(false)]
		public Color WarningColor
		{
			get
			{
				return this._warningColor;
			}
			set
			{
				if (value.Alpha != this._warningColor.Alpha || value.Blue != this._warningColor.Blue || value.Red != this._warningColor.Red || value.Green != this._warningColor.Green)
				{
					this._warningColor = value;
					base.OnPropertyChanged(value, "WarningColor");
				}
			}
		}

		// Token: 0x0400064C RID: 1612
		private int _prevValue = -99;

		// Token: 0x0400064D RID: 1613
		private bool _isWarning;

		// Token: 0x0400064E RID: 1614
		private Color _normalColor;

		// Token: 0x0400064F RID: 1615
		private Color _warningColor;
	}
}
