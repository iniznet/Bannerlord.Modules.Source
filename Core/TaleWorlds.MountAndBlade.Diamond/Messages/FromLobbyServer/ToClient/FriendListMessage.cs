using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200001F RID: 31
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class FriendListMessage : Message
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000024EA File Offset: 0x000006EA
		// (set) Token: 0x0600006F RID: 111 RVA: 0x000024F2 File Offset: 0x000006F2
		public FriendInfo[] Friends { get; private set; }

		// Token: 0x06000070 RID: 112 RVA: 0x000024FB File Offset: 0x000006FB
		public FriendListMessage(FriendInfo[] friends)
		{
			this.Friends = friends;
		}
	}
}
