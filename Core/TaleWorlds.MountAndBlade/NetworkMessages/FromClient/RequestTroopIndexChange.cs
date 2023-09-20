using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestTroopIndexChange : GameNetworkMessage
	{
		public int SelectedTroopIndex { get; private set; }

		public RequestTroopIndexChange(int selectedTroopIndex)
		{
			this.SelectedTroopIndex = selectedTroopIndex;
		}

		public RequestTroopIndexChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SelectedTroopIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SelectedTroopIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.SelectedTroopIndex, CompressionMission.SelectedTroopIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		protected override string OnGetLogFormat()
		{
			return "Requesting selected troop change to " + this.SelectedTroopIndex;
		}
	}
}
