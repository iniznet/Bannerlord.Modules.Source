using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateBanner : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public string BannerCode { get; private set; }

		public CreateBanner(NetworkCommunicator peer, string bannerCode)
		{
			this.Peer = peer;
			this.BannerCode = bannerCode;
		}

		public CreateBanner()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Create banner for peer: ",
				this.Peer.UserName,
				", with index: ",
				this.Peer.Index
			});
		}
	}
}
