using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200002F RID: 47
	[Serializable]
	public class GetPlayerStatsMessageResult : FunctionResult
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000026D5 File Offset: 0x000008D5
		// (set) Token: 0x0600009C RID: 156 RVA: 0x000026DD File Offset: 0x000008DD
		public PlayerStatsBase[] PlayerStats { get; private set; }

		// Token: 0x0600009D RID: 157 RVA: 0x000026E6 File Offset: 0x000008E6
		public GetPlayerStatsMessageResult(PlayerStatsBase[] playerStats)
		{
			this.PlayerStats = playerStats;
		}
	}
}
