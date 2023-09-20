using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200003D RID: 61
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelEnded : GameNetworkMessage
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000463A File Offset: 0x0000283A
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x00004642 File Offset: 0x00002842
		public NetworkCommunicator WinnerPeer { get; private set; }

		// Token: 0x060001F9 RID: 505 RVA: 0x0000464B File Offset: 0x0000284B
		public DuelEnded(NetworkCommunicator winnerPeer)
		{
			this.WinnerPeer = winnerPeer;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000465A File Offset: 0x0000285A
		public DuelEnded()
		{
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00004664 File Offset: 0x00002864
		protected override bool OnRead()
		{
			bool flag = true;
			this.WinnerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00004682 File Offset: 0x00002882
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.WinnerPeer);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000468F File Offset: 0x0000288F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00004697 File Offset: 0x00002897
		protected override string OnGetLogFormat()
		{
			return this.WinnerPeer.UserName + "has won the duel";
		}
	}
}
