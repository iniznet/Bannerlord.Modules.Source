using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000CE RID: 206
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleStartedMessage : Message
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x00004177 File Offset: 0x00002377
		// (set) Token: 0x060002F5 RID: 757 RVA: 0x0000417F File Offset: 0x0000237F
		public Dictionary<PlayerId, int> PlayerTeams { get; set; }

		// Token: 0x060002F6 RID: 758 RVA: 0x00004188 File Offset: 0x00002388
		public BattleStartedMessage(Dictionary<PlayerId, int> playerTeams)
		{
			this.PlayerTeams = playerTeams;
		}
	}
}
