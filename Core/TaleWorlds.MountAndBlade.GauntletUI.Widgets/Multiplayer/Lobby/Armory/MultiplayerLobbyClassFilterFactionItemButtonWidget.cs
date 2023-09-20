using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	// Token: 0x020000AB RID: 171
	public class MultiplayerLobbyClassFilterFactionItemButtonWidget : ButtonWidget
	{
		// Token: 0x060008D0 RID: 2256 RVA: 0x00019404 File Offset: 0x00017604
		public MultiplayerLobbyClassFilterFactionItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00019418 File Offset: 0x00017618
		private void OnCultureChanged()
		{
			if (this.Culture == null)
			{
				return;
			}
			string text = this.BaseBrushName + "." + this.Culture[0].ToString().ToUpper() + this.Culture.Substring(1).ToLower();
			base.Brush = base.Context.BrushFactory.GetBrush(text);
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060008D2 RID: 2258 RVA: 0x00019480 File Offset: 0x00017680
		// (set) Token: 0x060008D3 RID: 2259 RVA: 0x00019488 File Offset: 0x00017688
		[Editor(false)]
		public string BaseBrushName
		{
			get
			{
				return this._baseBrushName;
			}
			set
			{
				if (value != this._baseBrushName)
				{
					this._baseBrushName = value;
					base.OnPropertyChanged<string>(value, "BaseBrushName");
					this.OnCultureChanged();
				}
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060008D4 RID: 2260 RVA: 0x000194B1 File Offset: 0x000176B1
		// (set) Token: 0x060008D5 RID: 2261 RVA: 0x000194B9 File Offset: 0x000176B9
		[Editor(false)]
		public string Culture
		{
			get
			{
				return this._culture;
			}
			set
			{
				if (this._culture != value)
				{
					this._culture = value;
					base.OnPropertyChanged<string>(value, "Culture");
					this.OnCultureChanged();
				}
			}
		}

		// Token: 0x04000402 RID: 1026
		private string _baseBrushName = "MPLobby.ClassFilter.FactionButton";

		// Token: 0x04000403 RID: 1027
		private string _culture;
	}
}
