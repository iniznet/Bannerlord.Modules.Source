using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000DC RID: 220
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class PlayerFledBattleMessage : Message
	{
		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000328 RID: 808 RVA: 0x000043B1 File Offset: 0x000025B1
		// (set) Token: 0x06000329 RID: 809 RVA: 0x000043B9 File Offset: 0x000025B9
		public PlayerId PlayerId { get; private set; }

		// Token: 0x0600032A RID: 810 RVA: 0x000043C2 File Offset: 0x000025C2
		public PlayerFledBattleMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
