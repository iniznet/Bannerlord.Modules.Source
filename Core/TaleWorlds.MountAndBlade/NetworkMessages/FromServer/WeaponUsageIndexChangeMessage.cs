using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class WeaponUsageIndexChangeMessage : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public EquipmentIndex SlotIndex { get; private set; }

		public int UsageIndex { get; private set; }

		public WeaponUsageIndexChangeMessage()
		{
		}

		public WeaponUsageIndexChangeMessage(Agent agent, EquipmentIndex slotIndex, int usageIndex)
		{
			this.Agent = agent;
			this.SlotIndex = slotIndex;
			this.UsageIndex = usageIndex;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.UsageIndex = (int)((short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag));
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.UsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			object[] array = new object[8];
			array[0] = "Set Weapon Usage Index: ";
			array[1] = this.UsageIndex;
			array[2] = " for weapon with EquipmentIndex: ";
			array[3] = this.SlotIndex;
			array[4] = " on Agent with name: ";
			array[5] = ((this.Agent != null) ? this.Agent.Name : "null agent");
			array[6] = " and agent-index: ";
			int num = 7;
			Agent agent = this.Agent;
			array[num] = ((agent != null) ? agent.Index : (-1));
			return string.Concat(array);
		}
	}
}
