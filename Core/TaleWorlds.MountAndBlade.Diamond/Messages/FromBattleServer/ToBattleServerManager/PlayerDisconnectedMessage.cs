using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000D2 RID: 210
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class PlayerDisconnectedMessage : Message
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000302 RID: 770 RVA: 0x0000420F File Offset: 0x0000240F
		// (set) Token: 0x06000303 RID: 771 RVA: 0x00004217 File Offset: 0x00002417
		public PlayerId PlayerId { get; private set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000304 RID: 772 RVA: 0x00004220 File Offset: 0x00002420
		// (set) Token: 0x06000305 RID: 773 RVA: 0x00004228 File Offset: 0x00002428
		public DisconnectType Type { get; private set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000306 RID: 774 RVA: 0x00004231 File Offset: 0x00002431
		// (set) Token: 0x06000307 RID: 775 RVA: 0x00004239 File Offset: 0x00002439
		public bool IsAllowedLeave { get; private set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000308 RID: 776 RVA: 0x00004242 File Offset: 0x00002442
		// (set) Token: 0x06000309 RID: 777 RVA: 0x0000424A File Offset: 0x0000244A
		public BattleResult BattleResult { get; private set; }

		// Token: 0x0600030A RID: 778 RVA: 0x00004253 File Offset: 0x00002453
		public PlayerDisconnectedMessage(PlayerId playerId, DisconnectType type, bool isAllowedLeave, BattleResult battleResult)
		{
			this.PlayerId = playerId;
			this.Type = type;
			this.IsAllowedLeave = isAllowedLeave;
			this.BattleResult = battleResult;
		}
	}
}
