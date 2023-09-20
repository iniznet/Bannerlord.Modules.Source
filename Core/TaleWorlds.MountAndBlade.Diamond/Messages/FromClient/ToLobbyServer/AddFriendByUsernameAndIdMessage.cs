using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200006F RID: 111
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AddFriendByUsernameAndIdMessage : Message
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x00003398 File Offset: 0x00001598
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x000033A0 File Offset: 0x000015A0
		public string Username { get; private set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x000033A9 File Offset: 0x000015A9
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x000033B1 File Offset: 0x000015B1
		public int UserId { get; private set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060001BA RID: 442 RVA: 0x000033BA File Offset: 0x000015BA
		public bool DontUseNameForUnknownPlayer { get; }

		// Token: 0x060001BB RID: 443 RVA: 0x000033C2 File Offset: 0x000015C2
		public AddFriendByUsernameAndIdMessage(string username, int userId, bool dontUseNameForUnknownPlayer)
		{
			this.Username = username;
			this.UserId = userId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
