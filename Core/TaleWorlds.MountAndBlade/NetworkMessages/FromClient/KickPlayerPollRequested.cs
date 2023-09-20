using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class KickPlayerPollRequested : GameNetworkMessage
	{
		public NetworkCommunicator PlayerPeer { get; private set; }

		public bool BanPlayer { get; private set; }

		public KickPlayerPollRequested(NetworkCommunicator playerPeer, bool banPlayer)
		{
			this.PlayerPeer = playerPeer;
			this.BanPlayer = banPlayer;
		}

		public KickPlayerPollRequested()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			this.BanPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
			GameNetworkMessage.WriteBoolToPacket(this.BanPlayer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			string text = "Requested to start poll to kick";
			string text2 = (this.BanPlayer ? " and ban" : "");
			string text3 = " player: ";
			NetworkCommunicator playerPeer = this.PlayerPeer;
			return text + text2 + text3 + ((playerPeer != null) ? playerPeer.UserName : null);
		}
	}
}
