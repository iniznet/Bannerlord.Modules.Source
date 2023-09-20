using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ChangeCulture : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public BasicCultureObject Culture { get; private set; }

		public ChangeCulture()
		{
		}

		public ChangeCulture(MissionPeer peer, BasicCultureObject culture)
		{
			this.Peer = peer.GetNetworkPeer();
			this.Culture = culture;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Culture, CompressionBasic.GUIDCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Culture = (BasicCultureObject)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Requested culture: " + this.Culture.Name;
		}
	}
}
