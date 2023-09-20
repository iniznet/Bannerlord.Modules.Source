using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x0200005F RID: 95
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class PlayerDisconnectedMessage : Message
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000166 RID: 358 RVA: 0x00003001 File Offset: 0x00001201
		// (set) Token: 0x06000167 RID: 359 RVA: 0x00003009 File Offset: 0x00001209
		public PlayerId PlayerId { get; private set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000168 RID: 360 RVA: 0x00003012 File Offset: 0x00001212
		// (set) Token: 0x06000169 RID: 361 RVA: 0x0000301A File Offset: 0x0000121A
		public DisconnectType Type { get; private set; }

		// Token: 0x0600016A RID: 362 RVA: 0x00003023 File Offset: 0x00001223
		public PlayerDisconnectedMessage(PlayerId playerId, DisconnectType type)
		{
			this.PlayerId = playerId;
			this.Type = type;
		}
	}
}
