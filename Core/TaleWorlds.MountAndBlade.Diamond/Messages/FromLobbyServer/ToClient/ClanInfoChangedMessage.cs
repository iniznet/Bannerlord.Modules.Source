using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000012 RID: 18
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanInfoChangedMessage : Message
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600003D RID: 61 RVA: 0x000022D2 File Offset: 0x000004D2
		// (set) Token: 0x0600003E RID: 62 RVA: 0x000022DA File Offset: 0x000004DA
		public ClanHomeInfo ClanHomeInfo { get; private set; }

		// Token: 0x0600003F RID: 63 RVA: 0x000022E3 File Offset: 0x000004E3
		public ClanInfoChangedMessage(ClanHomeInfo clanHomeInfo)
		{
			this.ClanHomeInfo = clanHomeInfo;
		}
	}
}
