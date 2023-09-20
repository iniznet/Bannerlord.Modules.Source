using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000DB RID: 219
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class PlayerDisconnectedFromLobbyMessage : Message
	{
		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000325 RID: 805 RVA: 0x00004391 File Offset: 0x00002591
		// (set) Token: 0x06000326 RID: 806 RVA: 0x00004399 File Offset: 0x00002599
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000327 RID: 807 RVA: 0x000043A2 File Offset: 0x000025A2
		public PlayerDisconnectedFromLobbyMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
