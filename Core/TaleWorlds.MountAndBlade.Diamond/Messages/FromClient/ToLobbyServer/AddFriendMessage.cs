using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000070 RID: 112
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AddFriendMessage : Message
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060001BC RID: 444 RVA: 0x000033DF File Offset: 0x000015DF
		// (set) Token: 0x060001BD RID: 445 RVA: 0x000033E7 File Offset: 0x000015E7
		public PlayerId FriendId { get; private set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060001BE RID: 446 RVA: 0x000033F0 File Offset: 0x000015F0
		public bool DontUseNameForUnknownPlayer { get; }

		// Token: 0x060001BF RID: 447 RVA: 0x000033F8 File Offset: 0x000015F8
		public AddFriendMessage(PlayerId friendId, bool dontUseNameForUnknownPlayer)
		{
			this.FriendId = friendId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
