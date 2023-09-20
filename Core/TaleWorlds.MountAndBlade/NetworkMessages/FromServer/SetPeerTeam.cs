using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetPeerTeam : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public Team Team { get; private set; }

		public SetPeerTeam(NetworkCommunicator peer, Team team)
		{
			this.Peer = peer;
			this.Team = team;
		}

		public SetPeerTeam()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
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
				this.Team,
				" of NetworkPeer with name: ",
				this.Peer.UserName,
				" and peer-index",
				this.Peer.Index
			});
		}
	}
}
