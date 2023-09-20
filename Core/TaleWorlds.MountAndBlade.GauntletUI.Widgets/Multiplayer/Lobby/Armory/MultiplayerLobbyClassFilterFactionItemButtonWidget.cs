using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerLobbyClassFilterFactionItemButtonWidget : ButtonWidget
	{
		public MultiplayerLobbyClassFilterFactionItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void OnCultureChanged()
		{
			if (this.Culture == null)
			{
				return;
			}
			string text = this.BaseBrushName + "." + this.Culture[0].ToString().ToUpper() + this.Culture.Substring(1).ToLower();
			base.Brush = base.Context.BrushFactory.GetBrush(text);
		}

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

		private string _baseBrushName = "MPLobby.ClassFilter.FactionButton";

		private string _culture;
	}
}
