using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class CreateBanner : GameNetworkMessage
	{
		public string BannerCode { get; private set; }

		public CreateBanner(string bannerCode)
		{
			this.BannerCode = bannerCode;
		}

		public CreateBanner()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Clients has updated his banner";
		}
	}
}
