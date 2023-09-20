using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestPerkChange : GameNetworkMessage
	{
		public int PerkListIndex { get; private set; }

		public int PerkIndex { get; private set; }

		public RequestPerkChange(int perkListIndex, int perkIndex)
		{
			this.PerkListIndex = perkListIndex;
			this.PerkIndex = perkIndex;
		}

		public RequestPerkChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.PerkListIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkListIndexCompressionInfo, ref flag);
			this.PerkIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.PerkListIndex, CompressionMission.PerkListIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.PerkIndex, CompressionMission.PerkIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Requesting perk selection in list ", this.PerkListIndex, " change to ", this.PerkIndex });
		}
	}
}
