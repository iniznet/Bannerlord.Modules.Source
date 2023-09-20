using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x0200005B RID: 91
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleFinishedMessage : Message
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00002EDC File Offset: 0x000010DC
		public BattleResult BattleResult { get; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00002EE4 File Offset: 0x000010E4
		public Dictionary<int, int> TeamScores { get; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00002EEC File Offset: 0x000010EC
		public Dictionary<PlayerId, int> PlayerScores { get; }

		// Token: 0x06000151 RID: 337 RVA: 0x00002EF4 File Offset: 0x000010F4
		public CustomBattleFinishedMessage(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this.BattleResult = battleResult;
			this.TeamScores = teamScores;
			this.PlayerScores = playerScores;
		}
	}
}
