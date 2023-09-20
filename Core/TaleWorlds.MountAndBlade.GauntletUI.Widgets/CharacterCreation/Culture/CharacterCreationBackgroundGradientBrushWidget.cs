using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Culture
{
	// Token: 0x02000169 RID: 361
	public class CharacterCreationBackgroundGradientBrushWidget : BrushWidget
	{
		// Token: 0x0600128D RID: 4749 RVA: 0x000333B1 File Offset: 0x000315B1
		public CharacterCreationBackgroundGradientBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x000333BC File Offset: 0x000315BC
		private void SetCultureBackground(string value)
		{
			string factionColorCode = WidgetsMultiplayerHelper.GetFactionColorCode(value, false);
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = Color.ConvertStringToColor(factionColorCode);
				}
			}
		}

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x0600128F RID: 4751 RVA: 0x00033458 File Offset: 0x00031658
		// (set) Token: 0x06001290 RID: 4752 RVA: 0x00033460 File Offset: 0x00031660
		[Editor(false)]
		public string CurrentCultureId
		{
			get
			{
				return this._currentCultureId;
			}
			set
			{
				if (this._currentCultureId != value)
				{
					this._currentCultureId = value;
					base.OnPropertyChanged<string>(value, "CurrentCultureId");
					this.SetCultureBackground(value);
				}
			}
		}

		// Token: 0x0400087E RID: 2174
		private string _currentCultureId;
	}
}
