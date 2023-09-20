using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class DuelResponse : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public bool Accepted { get; private set; }

		public DuelResponse(NetworkCommunicator peer, bool accepted)
		{
			this.Peer = peer;
			this.Accepted = accepted;
		}

		public DuelResponse()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Accepted = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteBoolToPacket(this.Accepted);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "Duel Response: " + (this.Accepted ? " Accepted" : " Not accepted");
		}
	}
}
