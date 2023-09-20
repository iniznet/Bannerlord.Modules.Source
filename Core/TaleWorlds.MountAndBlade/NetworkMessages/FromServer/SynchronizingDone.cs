using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SynchronizingDone : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public bool Synchronized { get; private set; }

		public SynchronizingDone(NetworkCommunicator peer, bool synchronized)
		{
			this.Peer = peer;
			this.Synchronized = synchronized;
		}

		public SynchronizingDone()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Synchronized = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteBoolToPacket(this.Synchronized);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		protected override string OnGetLogFormat()
		{
			string text = string.Concat(new object[]
			{
				"peer with name: ",
				this.Peer.UserName,
				", and index: ",
				this.Peer.Index
			});
			if (!this.Synchronized)
			{
				return "Synchronized: FALSE for " + text + " (Peer will not receive broadcasted messages)";
			}
			return "Synchronized: TRUE for " + text + " (received all initial data from the server and will now receive broadcasted messages)";
		}
	}
}
