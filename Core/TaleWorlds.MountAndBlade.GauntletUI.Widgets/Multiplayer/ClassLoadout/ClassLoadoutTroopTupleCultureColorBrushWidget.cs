using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000B8 RID: 184
	public class ClassLoadoutTroopTupleCultureColorBrushWidget : BrushWidget
	{
		// Token: 0x06000983 RID: 2435 RVA: 0x0001B397 File Offset: 0x00019597
		public ClassLoadoutTroopTupleCultureColorBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0001B3A0 File Offset: 0x000195A0
		private void UpdateColor()
		{
			if (string.IsNullOrEmpty(this.FactionCode))
			{
				return;
			}
			string factionColorCode = WidgetsMultiplayerHelper.GetFactionColorCode(this.FactionCode.ToLower(), this.UseSecondary);
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = Color.ConvertStringToColor(factionColorCode);
				}
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000985 RID: 2437 RVA: 0x0001B45C File Offset: 0x0001965C
		// (set) Token: 0x06000986 RID: 2438 RVA: 0x0001B464 File Offset: 0x00019664
		public string FactionCode
		{
			get
			{
				return this._factionCode;
			}
			set
			{
				if (value != this._factionCode)
				{
					this._factionCode = value;
					base.OnPropertyChanged<string>(value, "FactionCode");
					this.UpdateColor();
				}
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000987 RID: 2439 RVA: 0x0001B48D File Offset: 0x0001968D
		// (set) Token: 0x06000988 RID: 2440 RVA: 0x0001B495 File Offset: 0x00019695
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChanged(value, "UseSecondary");
					this.UpdateColor();
				}
			}
		}

		// Token: 0x04000460 RID: 1120
		private string _factionCode;

		// Token: 0x04000461 RID: 1121
		private bool _useSecondary;
	}
}
