using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerLobbyArmoryCosmeticTierVisualBrushWidget : BrushWidget
	{
		public MultiplayerLobbyArmoryCosmeticTierVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateVisual()
		{
			switch (this._rarity)
			{
			case 0:
			case 1:
				this.SetState("Common");
				return;
			case 2:
				this.SetState("Rare");
				return;
			case 3:
				this.SetState("Unique");
				return;
			default:
				return;
			}
		}

		[Editor(false)]
		public int Rarity
		{
			get
			{
				return this._rarity;
			}
			set
			{
				if (this._rarity != value)
				{
					this._rarity = value;
					base.OnPropertyChanged(value, "Rarity");
					this.UpdateVisual();
				}
			}
		}

		private int _rarity = -1;
	}
}
