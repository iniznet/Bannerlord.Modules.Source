using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A1 RID: 161
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetPeerTeam : GameNetworkMessage
	{
		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0000C212 File Offset: 0x0000A412
		// (set) Token: 0x0600068F RID: 1679 RVA: 0x0000C21A File Offset: 0x0000A41A
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x0000C223 File Offset: 0x0000A423
		// (set) Token: 0x06000691 RID: 1681 RVA: 0x0000C22B File Offset: 0x0000A42B
		public Team Team { get; private set; }

		// Token: 0x06000692 RID: 1682 RVA: 0x0000C234 File Offset: 0x0000A434
		public SetPeerTeam(NetworkCommunicator peer, Team team)
		{
			this.Peer = peer;
			this.Team = team;
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0000C24A File Offset: 0x0000A44A
		public SetPeerTeam()
		{
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0000C254 File Offset: 0x0000A454
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0000C27F File Offset: 0x0000A47F
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0000C297 File Offset: 0x0000A497
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0000C29C File Offset: 0x0000A49C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Team: ",
				this.Team,
				" of NetworkPeer with name: ",
				this.Peer.UserName,
				" and peer-index",
				this.Peer.Index
			});
		}
	}
}
