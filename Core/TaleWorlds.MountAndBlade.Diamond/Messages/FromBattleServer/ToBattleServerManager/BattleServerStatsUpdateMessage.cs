using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000CD RID: 205
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleServerStatsUpdateMessage : Message
	{
		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000413F File Offset: 0x0000233F
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x00004147 File Offset: 0x00002347
		public BattleResult BattleResult { get; private set; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x00004150 File Offset: 0x00002350
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x00004158 File Offset: 0x00002358
		public Dictionary<int, int> TeamScores { get; private set; }

		// Token: 0x060002F3 RID: 755 RVA: 0x00004161 File Offset: 0x00002361
		public BattleServerStatsUpdateMessage(BattleResult battleResult, Dictionary<int, int> teamScores)
		{
			this.BattleResult = battleResult;
			this.TeamScores = teamScores;
		}
	}
}
