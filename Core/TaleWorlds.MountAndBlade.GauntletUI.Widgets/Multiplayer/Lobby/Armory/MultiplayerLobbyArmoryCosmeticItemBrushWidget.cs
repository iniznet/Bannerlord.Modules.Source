using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerLobbyArmoryCosmeticItemBrushWidget : BrushWidget
	{
		public MultiplayerLobbyArmoryCosmeticItemBrushWidget(UIContext context)
			: base(context)
		{
		}

		public override void SetState(string stateName)
		{
		}

		private void OnUsageChanged()
		{
			base.SetState(this.IsUsed ? "Selected" : "Default");
		}

		private void OnRarityChanged()
		{
			switch (this.Rarity)
			{
			case 0:
			case 1:
				base.Brush = base.Context.GetBrush("MPLobby.Armory.CosmeticButton.Common");
				return;
			case 2:
				base.Brush = base.Context.GetBrush("MPLobby.Armory.CosmeticButton.Rare");
				return;
			case 3:
				base.Brush = base.Context.GetBrush("MPLobby.Armory.CosmeticButton.Unique");
				return;
			default:
				return;
			}
		}

		[Editor(false)]
		public bool IsUsed
		{
			get
			{
				return this._isUsed;
			}
			set
			{
				if (value != this._isUsed)
				{
					this._isUsed = value;
					base.OnPropertyChanged(value, "IsUsed");
					this.OnUsageChanged();
				}
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
				if (value != this._rarity)
				{
					this._rarity = value;
					base.OnPropertyChanged(value, "Rarity");
					this.OnRarityChanged();
				}
			}
		}

		private const string BaseBrushName = "MPLobby.Armory.CosmeticButton";

		private bool _isUsed;

		private int _rarity;
	}
}
