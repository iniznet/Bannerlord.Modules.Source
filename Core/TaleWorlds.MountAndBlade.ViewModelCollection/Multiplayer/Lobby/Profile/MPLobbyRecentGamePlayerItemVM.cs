using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x02000070 RID: 112
	public class MPLobbyRecentGamePlayerItemVM : MPLobbyPlayerBaseVM
	{
		// Token: 0x06000A6A RID: 2666 RVA: 0x000257D8 File Offset: 0x000239D8
		public MPLobbyRecentGamePlayerItemVM(PlayerId playerId, MatchInfo matchOfThePlayer, Action<MPLobbyRecentGamePlayerItemVM> onActivatePlayerActions)
			: base(playerId, "", null, null)
		{
			this.MatchOfThePlayer = matchOfThePlayer;
			this._onActivatePlayerActions = onActivatePlayerActions;
			PlayerInfo playerInfo = this.MatchOfThePlayer.Players.FirstOrDefault((PlayerInfo p) => p.PlayerId == playerId.ToString());
			if (playerInfo != null)
			{
				this.KillCount = playerInfo.Kill;
				this.DeathCount = playerInfo.Death;
				this.AssistCount = playerInfo.Assist;
			}
			this.RefreshValues();
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x0002585D File Offset: 0x00023A5D
		private void ExecuteActivatePlayerActions()
		{
			Action<MPLobbyRecentGamePlayerItemVM> onActivatePlayerActions = this._onActivatePlayerActions;
			if (onActivatePlayerActions == null)
			{
				return;
			}
			onActivatePlayerActions(this);
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00025870 File Offset: 0x00023A70
		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000A6D RID: 2669 RVA: 0x00025878 File Offset: 0x00023A78
		// (set) Token: 0x06000A6E RID: 2670 RVA: 0x00025880 File Offset: 0x00023A80
		[DataSourceProperty]
		public int KillCount
		{
			get
			{
				return this._killCount;
			}
			set
			{
				if (value != this._killCount)
				{
					this._killCount = value;
					base.OnPropertyChangedWithValue(value, "KillCount");
				}
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000A6F RID: 2671 RVA: 0x0002589E File Offset: 0x00023A9E
		// (set) Token: 0x06000A70 RID: 2672 RVA: 0x000258A6 File Offset: 0x00023AA6
		[DataSourceProperty]
		public int DeathCount
		{
			get
			{
				return this._deathCount;
			}
			set
			{
				if (value != this._deathCount)
				{
					this._deathCount = value;
					base.OnPropertyChangedWithValue(value, "DeathCount");
				}
			}
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000A71 RID: 2673 RVA: 0x000258C4 File Offset: 0x00023AC4
		// (set) Token: 0x06000A72 RID: 2674 RVA: 0x000258CC File Offset: 0x00023ACC
		[DataSourceProperty]
		public int AssistCount
		{
			get
			{
				return this._assistCount;
			}
			set
			{
				if (value != this._assistCount)
				{
					this._assistCount = value;
					base.OnPropertyChangedWithValue(value, "AssistCount");
				}
			}
		}

		// Token: 0x0400050F RID: 1295
		public readonly MatchInfo MatchOfThePlayer;

		// Token: 0x04000510 RID: 1296
		private readonly Action<MPLobbyRecentGamePlayerItemVM> _onActivatePlayerActions;

		// Token: 0x04000511 RID: 1297
		private int _killCount;

		// Token: 0x04000512 RID: 1298
		private int _deathCount;

		// Token: 0x04000513 RID: 1299
		private int _assistCount;
	}
}
