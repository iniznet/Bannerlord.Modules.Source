using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Matchmaking
{
	public class MultiplayerLobbyMatchmakingRegionConnectionQualityTextWidget : TextWidget
	{
		public MultiplayerLobbyMatchmakingRegionConnectionQualityTextWidget(UIContext context)
			: base(context)
		{
			base.AddState("PoorQuality");
			base.AddState("AverageQuality");
			base.AddState("GoodQuality");
			this.ConnectionQualityLevelUpdated();
		}

		private void ConnectionQualityLevelUpdated()
		{
			switch (this.ConnectionQualityLevel)
			{
			case 0:
				this.SetState("PoorQuality");
				return;
			case 1:
				this.SetState("AverageQuality");
				return;
			case 2:
				this.SetState("GoodQuality");
				return;
			default:
				this.SetState("Default");
				return;
			}
		}

		[Editor(false)]
		public int ConnectionQualityLevel
		{
			get
			{
				return this._connectionQualityLevel;
			}
			set
			{
				if (this._connectionQualityLevel != value)
				{
					this._connectionQualityLevel = value;
					base.OnPropertyChanged(value, "ConnectionQualityLevel");
					this.ConnectionQualityLevelUpdated();
				}
			}
		}

		private int _connectionQualityLevel;
	}
}
