using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000C7 RID: 199
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleCancelledDueToPlayerQuitMessage : Message
	{
		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060002CF RID: 719 RVA: 0x00003FAF File Offset: 0x000021AF
		// (set) Token: 0x060002D0 RID: 720 RVA: 0x00003FB7 File Offset: 0x000021B7
		public PlayerId PlayerId { get; private set; }

		// Token: 0x060002D1 RID: 721 RVA: 0x00003FC0 File Offset: 0x000021C0
		public BattleCancelledDueToPlayerQuitMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
