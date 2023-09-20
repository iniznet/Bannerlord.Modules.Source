using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StartSwitchingWeaponUsageIndex : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public EquipmentIndex EquipmentIndex { get; private set; }

		public int UsageIndex { get; private set; }

		public Agent.UsageDirection CurrentMovementFlagUsageDirection { get; private set; }

		public StartSwitchingWeaponUsageIndex(int agentIndex, EquipmentIndex equipmentIndex, int usageIndex, Agent.UsageDirection currentMovementFlagUsageDirection)
		{
			this.AgentIndex = agentIndex;
			this.EquipmentIndex = equipmentIndex;
			this.UsageIndex = usageIndex;
			this.CurrentMovementFlagUsageDirection = currentMovementFlagUsageDirection;
		}

		public StartSwitchingWeaponUsageIndex()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.EquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.UsageIndex = (int)((short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag));
			this.CurrentMovementFlagUsageDirection = (Agent.UsageDirection)GameNetworkMessage.ReadIntFromPacket(CompressionMission.UsageDirectionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket((int)this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.UsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.CurrentMovementFlagUsageDirection, CompressionMission.UsageDirectionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "StartSwitchingWeaponUsageIndex: ", this.UsageIndex, " for weapon with EquipmentIndex: ", this.EquipmentIndex, " on Agent with agent-index: ", this.AgentIndex });
		}
	}
}
