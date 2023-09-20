using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelSessionStarted : GameNetworkMessage
	{
		public NetworkCommunicator RequesterPeer { get; private set; }

		public NetworkCommunicator RequestedPeer { get; private set; }

		public DuelSessionStarted(NetworkCommunicator requesterPeer, NetworkCommunicator requestedPeer)
		{
			this.RequesterPeer = requesterPeer;
			this.RequestedPeer = requestedPeer;
		}

		public DuelSessionStarted()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RequesterPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.RequestedPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequesterPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequestedPeer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Duel session started between agent with name: ",
				this.RequestedPeer.UserName,
				" and index: ",
				this.RequestedPeer.Index,
				" and agent with name: ",
				this.RequesterPeer.UserName,
				" and index: ",
				this.RequesterPeer.Index
			});
		}
	}
}
