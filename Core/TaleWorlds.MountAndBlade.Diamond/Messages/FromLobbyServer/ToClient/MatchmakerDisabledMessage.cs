using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200003F RID: 63
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class MatchmakerDisabledMessage : Message
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00002A84 File Offset: 0x00000C84
		public int RemainingTime { get; }

		// Token: 0x060000EE RID: 238 RVA: 0x00002A8C File Offset: 0x00000C8C
		public MatchmakerDisabledMessage(int remainingTime)
		{
			this.RemainingTime = remainingTime;
		}
	}
}
