using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B5 RID: 181
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RemoveFriendMessage : Message
	{
		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000294 RID: 660 RVA: 0x00003D1C File Offset: 0x00001F1C
		// (set) Token: 0x06000295 RID: 661 RVA: 0x00003D24 File Offset: 0x00001F24
		public PlayerId FriendId { get; private set; }

		// Token: 0x06000296 RID: 662 RVA: 0x00003D2D File Offset: 0x00001F2D
		public RemoveFriendMessage(PlayerId friendId)
		{
			this.FriendId = friendId;
		}
	}
}
