using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class KickPlayer : GameNetworkMessage
	{
		public NetworkCommunicator PlayerPeer { get; private set; }

		public bool BanPlayer { get; private set; }

		public KickPlayer(NetworkCommunicator playerPeer, bool banPlayer)
		{
			this.PlayerPeer = playerPeer;
			this.BanPlayer = banPlayer;
		}

		public KickPlayer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
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
			return "Requested to kick" + (this.BanPlayer ? " and ban" : "") + " player: " + this.PlayerPeer.UserName;
		}
	}
}
