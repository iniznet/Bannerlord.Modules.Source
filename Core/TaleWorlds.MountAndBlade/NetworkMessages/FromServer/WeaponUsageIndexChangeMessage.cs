using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class WeaponUsageIndexChangeMessage : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public EquipmentIndex SlotIndex { get; private set; }

		public int UsageIndex { get; private set; }

		public WeaponUsageIndexChangeMessage()
		{
		}

		public WeaponUsageIndexChangeMessage(int agentIndex, EquipmentIndex slotIndex, int usageIndex)
		{
			this.AgentIndex = agentIndex;
			this.SlotIndex = slotIndex;
			this.UsageIndex = usageIndex;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.UsageIndex = (int)((short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag));
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.UsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set Weapon Usage Index: ", this.UsageIndex, " for weapon with EquipmentIndex: ", this.SlotIndex, " on Agent with agent-index: ", this.AgentIndex });
		}
	}
}
