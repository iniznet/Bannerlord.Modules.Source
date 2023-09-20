using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRoundMVP : GameNetworkMessage
	{
		public NetworkCommunicator MVPPeer { get; private set; }

		public SetRoundMVP(NetworkCommunicator mvpPeer, int mvpCount)
		{
			this.MVPPeer = mvpPeer;
			this.MVPCount = mvpCount;
		}

		public SetRoundMVP()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MVPPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.MVPCount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.RoundTotalCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.MVPPeer);
			GameNetworkMessage.WriteIntToPacket(this.MVPCount, CompressionBasic.RoundTotalCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission | MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "MVP selected as: " + this.MVPPeer.UserName + ".";
		}

		public int MVPCount;
	}
}
