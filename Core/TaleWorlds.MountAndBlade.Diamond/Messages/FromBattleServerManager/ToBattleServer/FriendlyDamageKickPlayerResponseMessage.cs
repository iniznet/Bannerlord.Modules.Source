using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000D9 RID: 217
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class FriendlyDamageKickPlayerResponseMessage : Message
	{
		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000319 RID: 793 RVA: 0x00004308 File Offset: 0x00002508
		// (set) Token: 0x0600031A RID: 794 RVA: 0x00004310 File Offset: 0x00002510
		public PlayerId PlayerId { get; private set; }

		// Token: 0x0600031B RID: 795 RVA: 0x00004319 File Offset: 0x00002519
		public FriendlyDamageKickPlayerResponseMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
