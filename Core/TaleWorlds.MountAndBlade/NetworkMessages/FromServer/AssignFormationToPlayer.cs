using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AssignFormationToPlayer : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public FormationClass FormationClass { get; private set; }

		public AssignFormationToPlayer(NetworkCommunicator peer, FormationClass formationClass)
		{
			this.Peer = peer;
			this.FormationClass = formationClass;
		}

		public AssignFormationToPlayer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.FormationClass = (FormationClass)GameNetworkMessage.ReadIntFromPacket(CompressionMission.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket((int)this.FormationClass, CompressionMission.FormationClassCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

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
