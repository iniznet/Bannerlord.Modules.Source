using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200000E RID: 14
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ClanCreationRequestAnsweredMessage : Message
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002221 File Offset: 0x00000421
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002229 File Offset: 0x00000429
		public PlayerId PlayerId { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002232 File Offset: 0x00000432
		// (set) Token: 0x06000030 RID: 48 RVA: 0x0000223A File Offset: 0x0000043A
		public ClanCreationAnswer ClanCreationAnswer { get; private set; }

		// Token: 0x06000031 RID: 49 RVA: 0x00002243 File Offset: 0x00000443
		public ClanCreationRequestAnsweredMessage(PlayerId playerId, ClanCreationAnswer clanCreationAnswer)
		{
			this.PlayerId = playerId;
			this.ClanCreationAnswer = clanCreationAnswer;
		}
	}
}
