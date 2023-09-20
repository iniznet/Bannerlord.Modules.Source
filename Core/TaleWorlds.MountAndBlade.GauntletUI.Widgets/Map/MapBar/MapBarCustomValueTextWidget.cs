using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	// Token: 0x02000108 RID: 264
	public class MapBarCustomValueTextWidget : TextWidget
	{
		// Token: 0x06000D94 RID: 3476 RVA: 0x000260C7 File Offset: 0x000242C7
		public MapBarCustomValueTextWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x000260D8 File Offset: 0x000242D8
		private void RefreshTextAnimation(int valueDifference)
		{
			if (valueDifference > 0)
			{
				if (base.CurrentState == "Positive")
				{
					base.BrushRenderer.RestartAnimation();
					return;
				}
				this.SetState("Positive");
				return;
			}
			else
			{
				if (valueDifference >= 0)
				{
					Debug.FailedAssert("Value change in party label cannot be 0", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Map\\MapBar\\MapBarCustomValueTextWidget.cs", "RefreshTextAnimation", 40);
					return;
				}
				if (base.CurrentState == "Negative")
				{
					base.BrushRenderer.RestartAnimation();
					return;
				}
				this.SetState("Negative");
				return;
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06000D96 RID: 3478 RVA: 0x00026157 File Offset: 0x00024357
		// (set) Token: 0x06000D97 RID: 3479 RVA: 0x0002615F File Offset: 0x0002435F
		[Editor(false)]
		public int ValueAsInt
		{
			get
			{
				return this._totalTroops;
			}
			set
			{
				if (value != this._totalTroops)
				{
					this.RefreshTextAnimation(value - this._totalTroops);
					this._totalTroops = value;
					base.OnPropertyChanged(value, "ValueAsInt");
				}
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06000D98 RID: 3480 RVA: 0x0002618B File Offset: 0x0002438B
		// (set) Token: 0x06000D99 RID: 3481 RVA: 0x00026194 File Offset: 0x00024394
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

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06000D9A RID: 3482 RVA: 0x00026234 File Offset: 0x00024434
		// (set) Token: 0x06000D9B RID: 3483 RVA: 0x0002623C File Offset: 0x0002443C
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

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06000D9C RID: 3484 RVA: 0x000262A8 File Offset: 0x000244A8
		// (set) Token: 0x06000D9D RID: 3485 RVA: 0x000262B0 File Offset: 0x000244B0
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

		// Token: 0x04000643 RID: 1603
		private bool _isWarning;

		// Token: 0x04000644 RID: 1604
		private Color _normalColor;

		// Token: 0x04000645 RID: 1605
		private Color _warningColor;

		// Token: 0x04000646 RID: 1606
		private int _totalTroops;
	}
}
