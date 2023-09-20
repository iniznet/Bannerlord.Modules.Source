using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelPreparationStartedForTheFirstTime : GameNetworkMessage
	{
		public NetworkCommunicator RequesterPeer { get; private set; }

		public NetworkCommunicator RequesteePeer { get; private set; }

		public int AreaIndex { get; private set; }

		public DuelPreparationStartedForTheFirstTime(NetworkCommunicator requesterPeer, NetworkCommunicator requesteePeer, int areaIndex)
		{
			this.RequesterPeer = requesterPeer;
			this.RequesteePeer = requesteePeer;
			this.AreaIndex = areaIndex;
		}

		public DuelPreparationStartedForTheFirstTime()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RequesterPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.RequesteePeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.AreaIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.DuelAreaIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequesterPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequesteePeer);
			GameNetworkMessage.WriteIntToPacket(this.AreaIndex, CompressionMission.DuelAreaIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Duel started between agent with name: ",
				this.RequesteePeer.UserName,
				" and index: ",
				this.RequesteePeer.Index,
				" and agent with name: ",
				this.RequesterPeer.UserName,
				" and index: ",
				this.RequesterPeer.Index
			});
		}
	}
}
