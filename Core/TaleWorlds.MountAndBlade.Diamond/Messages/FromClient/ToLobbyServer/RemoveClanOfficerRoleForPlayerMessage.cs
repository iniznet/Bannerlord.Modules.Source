using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B4 RID: 180
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RemoveClanOfficerRoleForPlayerMessage : Message
	{
		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000291 RID: 657 RVA: 0x00003CFC File Offset: 0x00001EFC
		// (set) Token: 0x06000292 RID: 658 RVA: 0x00003D04 File Offset: 0x00001F04
		public PlayerId RemovedOfficerId { get; private set; }

		// Token: 0x06000293 RID: 659 RVA: 0x00003D0D File Offset: 0x00001F0D
		public RemoveClanOfficerRoleForPlayerMessage(PlayerId removedOfficerId)
		{
			this.RemovedOfficerId = removedOfficerId;
		}
	}
}
