using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Matchmaking
{
	// Token: 0x020000A0 RID: 160
	public class MultiplayerLobbyMatchmakingRegionConnectionQualityTextWidget : TextWidget
	{
		// Token: 0x0600086D RID: 2157 RVA: 0x000186E1 File Offset: 0x000168E1
		public MultiplayerLobbyMatchmakingRegionConnectionQualityTextWidget(UIContext context)
			: base(context)
		{
			base.AddState("PoorQuality");
			base.AddState("AverageQuality");
			base.AddState("GoodQuality");
			this.ConnectionQualityLevelUpdated();
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x00018714 File Offset: 0x00016914
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

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x0600086F RID: 2159 RVA: 0x0001876B File Offset: 0x0001696B
		// (set) Token: 0x06000870 RID: 2160 RVA: 0x00018773 File Offset: 0x00016973
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

		// Token: 0x040003DA RID: 986
		private int _connectionQualityLevel;
	}
}
