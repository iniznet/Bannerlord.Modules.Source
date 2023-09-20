using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000067 RID: 103
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AssignFormationToPlayer : GameNetworkMessage
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x000071C1 File Offset: 0x000053C1
		// (set) Token: 0x060003B3 RID: 947 RVA: 0x000071C9 File Offset: 0x000053C9
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x000071D2 File Offset: 0x000053D2
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x000071DA File Offset: 0x000053DA
		public FormationClass FormationClass { get; private set; }

		// Token: 0x060003B6 RID: 950 RVA: 0x000071E3 File Offset: 0x000053E3
		public AssignFormationToPlayer(NetworkCommunicator peer, FormationClass formationClass)
		{
			this.Peer = peer;
			this.FormationClass = formationClass;
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000071F9 File Offset: 0x000053F9
		public AssignFormationToPlayer()
		{
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00007204 File Offset: 0x00005404
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.FormationClass = (FormationClass)GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00007234 File Offset: 0x00005434
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket((int)this.FormationClass, CompressionOrder.FormationClassCompressionInfo);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00007251 File Offset: 0x00005451
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00007258 File Offset: 0x00005458
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Assign formation with index: ",
				(int)this.FormationClass,
				" to NetworkPeer with name: ",
				this.Peer.UserName,
				" and peer-index",
				this.Peer.Index,
				" and make him captain."
			});
		}
	}
}
