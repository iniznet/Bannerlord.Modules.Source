using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200007B RID: 123
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeUsernameMessage : Message
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x00003515 File Offset: 0x00001715
		public string Username { get; }

		// Token: 0x060001DA RID: 474 RVA: 0x0000351D File Offset: 0x0000171D
		public ChangeUsernameMessage(string username)
		{
			this.Username = username;
		}
	}
}
