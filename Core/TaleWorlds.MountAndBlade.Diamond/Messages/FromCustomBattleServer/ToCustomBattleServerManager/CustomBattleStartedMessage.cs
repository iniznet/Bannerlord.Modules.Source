using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x0200005E RID: 94
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleStartedMessage : Message
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00002FB1 File Offset: 0x000011B1
		// (set) Token: 0x06000160 RID: 352 RVA: 0x00002FB9 File Offset: 0x000011B9
		public string GameType { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00002FC2 File Offset: 0x000011C2
		// (set) Token: 0x06000162 RID: 354 RVA: 0x00002FCA File Offset: 0x000011CA
		public Dictionary<PlayerId, int> PlayerTeams { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00002FD3 File Offset: 0x000011D3
		// (set) Token: 0x06000164 RID: 356 RVA: 0x00002FDB File Offset: 0x000011DB
		public List<string> FactionNames { get; set; }

		// Token: 0x06000165 RID: 357 RVA: 0x00002FE4 File Offset: 0x000011E4
		public CustomBattleStartedMessage(string gameType, Dictionary<PlayerId, int> playerTeams, List<string> factionNames)
		{
			this.GameType = gameType;
			this.PlayerTeams = playerTeams;
			this.FactionNames = factionNames;
		}
	}
}
