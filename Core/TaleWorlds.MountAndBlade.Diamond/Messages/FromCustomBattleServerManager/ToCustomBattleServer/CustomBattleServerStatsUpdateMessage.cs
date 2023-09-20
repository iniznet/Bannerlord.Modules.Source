using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	// Token: 0x02000068 RID: 104
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerStatsUpdateMessage : Message
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x000032D0 File Offset: 0x000014D0
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x000032D8 File Offset: 0x000014D8
		public BattleResult BattleResult { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x000032E1 File Offset: 0x000014E1
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x000032E9 File Offset: 0x000014E9
		public Dictionary<int, int> TeamScores { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x000032F2 File Offset: 0x000014F2
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x000032FA File Offset: 0x000014FA
		public Dictionary<PlayerId, int> PlayerScores { get; set; }

		// Token: 0x060001A9 RID: 425 RVA: 0x00003303 File Offset: 0x00001503
		public CustomBattleServerStatsUpdateMessage(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this.BattleResult = battleResult;
			this.TeamScores = teamScores;
			this.PlayerScores = playerScores;
		}
	}
}
