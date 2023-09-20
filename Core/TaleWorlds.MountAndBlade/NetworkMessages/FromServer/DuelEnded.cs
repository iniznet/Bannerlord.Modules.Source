using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelEnded : GameNetworkMessage
	{
		public NetworkCommunicator WinnerPeer { get; private set; }

		public DuelEnded(NetworkCommunicator winnerPeer)
		{
			this.WinnerPeer = winnerPeer;
		}

		public DuelEnded()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.WinnerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.WinnerPeer);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return this.WinnerPeer.UserName + "has won the duel";
		}
	}
}
