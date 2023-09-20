using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetPeerTeam : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public int TeamIndex { get; private set; }

		public SetPeerTeam(NetworkCommunicator peer, int teamIndex)
		{
			this.Peer = peer;
			this.TeamIndex = teamIndex;
		}

		public SetPeerTeam()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.TeamIndex = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteTeamIndexToPacket(this.TeamIndex);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Team: ",
				this.TeamIndex,
				" of NetworkPeer with name: ",
				this.Peer.UserName,
				" and peer-index",
				this.Peer.Index
			});
		}
	}
}
