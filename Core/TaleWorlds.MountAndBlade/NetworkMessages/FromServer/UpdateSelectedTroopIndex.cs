using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UpdateSelectedTroopIndex : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public int SelectedTroopIndex { get; private set; }

		public UpdateSelectedTroopIndex(NetworkCommunicator peer, int selectedTroopIndex)
		{
			this.Peer = peer;
			this.SelectedTroopIndex = selectedTroopIndex;
		}

		public UpdateSelectedTroopIndex()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.SelectedTroopIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SelectedTroopIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.SelectedTroopIndex, CompressionMission.SelectedTroopIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Update SelectedTroopIndex to: ",
				this.SelectedTroopIndex,
				", on peer: ",
				this.Peer.UserName,
				" with peer-index:",
				this.Peer.Index
			});
		}
	}
}
