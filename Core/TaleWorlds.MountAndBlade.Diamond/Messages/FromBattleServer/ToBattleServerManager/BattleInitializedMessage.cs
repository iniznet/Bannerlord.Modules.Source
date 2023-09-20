using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000CA RID: 202
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleInitializedMessage : Message
	{
		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060002DA RID: 730 RVA: 0x0000403C File Offset: 0x0000223C
		public string GameType { get; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060002DB RID: 731 RVA: 0x00004044 File Offset: 0x00002244
		public List<PlayerId> AssignedPlayers { get; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060002DC RID: 732 RVA: 0x0000404C File Offset: 0x0000224C
		public string Faction1 { get; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060002DD RID: 733 RVA: 0x00004054 File Offset: 0x00002254
		public string Faction2 { get; }

		// Token: 0x060002DE RID: 734 RVA: 0x0000405C File Offset: 0x0000225C
		public BattleInitializedMessage(string gameType, List<PlayerId> assignedPlayers, string faction1, string faction2)
		{
			this.GameType = gameType;
			this.AssignedPlayers = assignedPlayers;
			this.Faction1 = faction1;
			this.Faction2 = faction2;
		}
	}
}
