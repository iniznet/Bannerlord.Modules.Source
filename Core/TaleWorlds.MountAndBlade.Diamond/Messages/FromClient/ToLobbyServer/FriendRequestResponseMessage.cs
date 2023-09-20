using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200008C RID: 140
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class FriendRequestResponseMessage : Message
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000211 RID: 529 RVA: 0x00003779 File Offset: 0x00001979
		// (set) Token: 0x06000212 RID: 530 RVA: 0x00003781 File Offset: 0x00001981
		public PlayerId PlayerId { get; private set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000378A File Offset: 0x0000198A
		// (set) Token: 0x06000214 RID: 532 RVA: 0x00003792 File Offset: 0x00001992
		public bool DontUseNameForUnknownPlayer { get; private set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000215 RID: 533 RVA: 0x0000379B File Offset: 0x0000199B
		// (set) Token: 0x06000216 RID: 534 RVA: 0x000037A3 File Offset: 0x000019A3
		public bool IsAccepted { get; private set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000217 RID: 535 RVA: 0x000037AC File Offset: 0x000019AC
		// (set) Token: 0x06000218 RID: 536 RVA: 0x000037B4 File Offset: 0x000019B4
		public bool IsBlocked { get; private set; }

		// Token: 0x06000219 RID: 537 RVA: 0x000037BD File Offset: 0x000019BD
		public FriendRequestResponseMessage(PlayerId playerId, bool dontUseNameForUnknownPlayer, bool isAccepted, bool isBlocked)
		{
			this.PlayerId = playerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
			this.IsAccepted = isAccepted;
			this.IsBlocked = isBlocked;
		}
	}
}
