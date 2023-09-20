using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200004E RID: 78
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class RecentPlayerStatusesMessage : Message
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00002D0A File Offset: 0x00000F0A
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00002D12 File Offset: 0x00000F12
		public FriendInfo[] Friends { get; private set; }

		// Token: 0x06000126 RID: 294 RVA: 0x00002D1B File Offset: 0x00000F1B
		public RecentPlayerStatusesMessage(FriendInfo[] friends)
		{
			this.Friends = friends;
		}
	}
}
