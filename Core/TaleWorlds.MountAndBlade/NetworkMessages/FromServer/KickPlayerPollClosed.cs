using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class KickPlayerPollClosed : GameNetworkMessage
	{
		public NetworkCommunicator PlayerPeer { get; private set; }

		public bool Accepted { get; private set; }

		public KickPlayerPollClosed(NetworkCommunicator playerPeer, bool accepted)
		{
			this.PlayerPeer = playerPeer;
			this.Accepted = accepted;
		}

		public KickPlayerPollClosed()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Accepted = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
			GameNetworkMessage.WriteBoolToPacket(this.Accepted);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			string[] array = new string[5];
			array[0] = "Poll is closed. ";
			int num = 1;
			NetworkCommunicator playerPeer = this.PlayerPeer;
			array[num] = ((playerPeer != null) ? playerPeer.UserName : null);
			array[2] = " is ";
			array[3] = (this.Accepted ? "" : "not ");
			array[4] = "kicked.";
			return string.Concat(array);
		}
	}
}
