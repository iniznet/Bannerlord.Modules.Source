using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestChangeCharacterMessage : GameNetworkMessage
	{
		public NetworkCommunicator NetworkPeer { get; private set; }

		public RequestChangeCharacterMessage(NetworkCommunicator networkPeer)
		{
			this.NetworkPeer = networkPeer;
		}

		public RequestChangeCharacterMessage()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.NetworkPeer);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.NetworkPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return this.NetworkPeer.UserName + " has requested to change character.";
		}
	}
}
