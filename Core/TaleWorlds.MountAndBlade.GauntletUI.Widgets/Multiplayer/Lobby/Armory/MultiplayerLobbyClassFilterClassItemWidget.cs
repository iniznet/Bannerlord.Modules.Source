using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerLobbyClassFilterClassItemWidget : ToggleStateButtonWidget
	{
		public MultiplayerLobbyClassFilterClassItemWidget(UIContext context)
			: base(context)
		{
		}

		private void SetFactionColor()
		{
			if (this.FactionColorWidget == null)
			{
				return;
			}
			this.FactionColorWidget.Color = this.CultureColor;
		}

		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.TroopType) || this._iconWidget == null)
			{
				return;
			}
			this.IconWidget.Sprite = base.Context.SpriteData.GetSprite("General\\compass\\" + this.TroopType);
		}

		[Editor(false)]
		public Widget FactionColorWidget
		{
			get
			{
				return this._factionColorWidget;
			}
			set
			{
				if (this._factionColorWidget != value)
				{
					this._factionColorWidget = value;
					base.OnPropertyChanged<Widget>(value, "FactionColorWidget");
					this.SetFactionColor();
				}
			}
		}

		[Editor(false)]
		public Color CultureColor
		{
			get
			{
				return this._cultureColor;
			}
			set
			{
				if (this._cultureColor != value)
				{
					this._cultureColor = value;
					base.OnPropertyChanged(value, "CultureColor");
					this.SetFactionColor();
				}
			}
		}

		[DataSourceProperty]
		public string TroopType
		{
			get
			{
				return this._troopType;
			}
			set
			{
				if (value != this._troopType)
				{
					this._troopType = value;
					base.OnPropertyChanged<string>(value, "TroopType");
					this.UpdateIcon();
				}
			}
		}

		[DataSourceProperty]
		public Widget IconWidget
		{
			get
			{
				return this._iconWidget;
			}
			set
			{
				if (value != this._iconWidget)
				{
					this._iconWidget = value;
					base.OnPropertyChanged<Widget>(value, "IconWidget");
					this.UpdateIcon();
				}
			}
		}

		private Widget _factionColorWidget;

		private Color _cultureColor;

		private string _troopType;

		private Widget _iconWidget;
	}
}
