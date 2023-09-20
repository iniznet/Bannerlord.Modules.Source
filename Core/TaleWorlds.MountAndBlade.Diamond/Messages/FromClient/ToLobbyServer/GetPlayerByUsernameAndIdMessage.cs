using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000099 RID: 153
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerByUsernameAndIdMessage : Message
	{
		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600022D RID: 557 RVA: 0x00003899 File Offset: 0x00001A99
		// (set) Token: 0x0600022E RID: 558 RVA: 0x000038A1 File Offset: 0x00001AA1
		public string Username { get; private set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600022F RID: 559 RVA: 0x000038AA File Offset: 0x00001AAA
		// (set) Token: 0x06000230 RID: 560 RVA: 0x000038B2 File Offset: 0x00001AB2
		public int UserId { get; private set; }

		// Token: 0x06000231 RID: 561 RVA: 0x000038BB File Offset: 0x00001ABB
		public GetPlayerByUsernameAndIdMessage(string username, int userId)
		{
			this.Username = username;
			this.UserId = userId;
		}
	}
}
