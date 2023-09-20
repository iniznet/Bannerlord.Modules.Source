using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000041 RID: 65
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelRoundEnded : GameNetworkMessage
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00004A1C File Offset: 0x00002C1C
		// (set) Token: 0x06000226 RID: 550 RVA: 0x00004A24 File Offset: 0x00002C24
		public NetworkCommunicator WinnerPeer { get; private set; }

		// Token: 0x06000227 RID: 551 RVA: 0x00004A2D File Offset: 0x00002C2D
		public DuelRoundEnded(NetworkCommunicator winnerPeer)
		{
			this.WinnerPeer = winnerPeer;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00004A3C File Offset: 0x00002C3C
		public DuelRoundEnded()
		{
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00004A44 File Offset: 0x00002C44
		protected override bool OnRead()
		{
			bool flag = true;
			this.WinnerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00004A62 File Offset: 0x00002C62
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.WinnerPeer);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00004A6F File Offset: 0x00002C6F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00004A77 File Offset: 0x00002C77
		protected override string OnGetLogFormat()
		{
			return this.WinnerPeer.UserName + "has won the duel against round.";
		}
	}
}
