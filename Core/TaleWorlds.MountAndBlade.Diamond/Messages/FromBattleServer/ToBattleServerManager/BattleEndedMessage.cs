using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000C8 RID: 200
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleEndedMessage : Message
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x00003FCF File Offset: 0x000021CF
		public BattleResult BattleResult { get; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x00003FD7 File Offset: 0x000021D7
		public GameLog[] GameLogs { get; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x00003FDF File Offset: 0x000021DF
		public Dictionary<ValueTuple<PlayerId, string, string>, int> BadgeDataDictionary { get; }

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x00003FE7 File Offset: 0x000021E7
		public Dictionary<int, int> TeamScores { get; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x00003FEF File Offset: 0x000021EF
		public Dictionary<PlayerId, int> PlayerScores { get; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x00003FF7 File Offset: 0x000021F7
		public int GameTime { get; }

		// Token: 0x060002D8 RID: 728 RVA: 0x00003FFF File Offset: 0x000021FF
		public BattleEndedMessage(BattleResult battleResult, GameLog[] gameLogs, Dictionary<ValueTuple<PlayerId, string, string>, int> badgeDataDictionary, int gameTime, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this.BattleResult = battleResult;
			this.GameLogs = gameLogs;
			this.BadgeDataDictionary = badgeDataDictionary;
			this.TeamScores = teamScores;
			this.PlayerScores = playerScores;
			this.GameTime = gameTime;
		}
	}
}
