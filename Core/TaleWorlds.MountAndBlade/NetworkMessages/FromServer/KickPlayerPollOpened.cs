using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class KickPlayerPollOpened : GameNetworkMessage
	{
		public NetworkCommunicator InitiatorPeer { get; private set; }

		public NetworkCommunicator PlayerPeer { get; private set; }

		public bool BanPlayer { get; private set; }

		public KickPlayerPollOpened(NetworkCommunicator initiatorPeer, NetworkCommunicator playerPeer, bool banPlayer)
		{
			this.InitiatorPeer = initiatorPeer;
			this.PlayerPeer = playerPeer;
			this.BanPlayer = banPlayer;
		}

		public KickPlayerPollOpened()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.InitiatorPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BanPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.InitiatorPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
			GameNetworkMessage.WriteBoolToPacket(this.BanPlayer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			string[] array = new string[5];
			int num = 0;
			NetworkCommunicator initiatorPeer = this.InitiatorPeer;
			array[num] = ((initiatorPeer != null) ? initiatorPeer.UserName : null);
			array[1] = " wants to start poll to kick";
			array[2] = (this.BanPlayer ? " and ban" : "");
			array[3] = " player: ";
			int num2 = 4;
			NetworkCommunicator playerPeer = this.PlayerPeer;
			array[num2] = ((playerPeer != null) ? playerPeer.UserName : null);
			return string.Concat(array);
		}
	}
}
