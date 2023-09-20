using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000047 RID: 71
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ChangeCulture : GameNetworkMessage
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000267 RID: 615 RVA: 0x000050E2 File Offset: 0x000032E2
		// (set) Token: 0x06000268 RID: 616 RVA: 0x000050EA File Offset: 0x000032EA
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000269 RID: 617 RVA: 0x000050F3 File Offset: 0x000032F3
		// (set) Token: 0x0600026A RID: 618 RVA: 0x000050FB File Offset: 0x000032FB
		public BasicCultureObject Culture { get; private set; }

		// Token: 0x0600026B RID: 619 RVA: 0x00005104 File Offset: 0x00003304
		public ChangeCulture()
		{
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000510C File Offset: 0x0000330C
		public ChangeCulture(MissionPeer peer, BasicCultureObject culture)
		{
			this.Peer = peer.GetNetworkPeer();
			this.Culture = culture;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00005127 File Offset: 0x00003327
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Culture, CompressionBasic.GUIDCompressionInfo);
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00005144 File Offset: 0x00003344
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Culture = (BasicCultureObject)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000517E File Offset: 0x0000337E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00005186 File Offset: 0x00003386
		protected override string OnGetLogFormat()
		{
			return "Requested culture: " + this.Culture.Name;
		}
	}
}
