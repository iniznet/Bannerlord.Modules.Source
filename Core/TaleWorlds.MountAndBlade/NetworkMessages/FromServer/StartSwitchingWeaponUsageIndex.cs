using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StartSwitchingWeaponUsageIndex : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public EquipmentIndex EquipmentIndex { get; private set; }

		public int UsageIndex { get; private set; }

		public Agent.UsageDirection CurrentMovementFlagUsageDirection { get; private set; }

		public StartSwitchingWeaponUsageIndex(Agent agent, EquipmentIndex equipmentIndex, int usageIndex, Agent.UsageDirection currentMovementFlagUsageDirection)
		{
			this.Agent = agent;
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
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.EquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.UsageIndex = (int)((short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag));
			this.CurrentMovementFlagUsageDirection = (Agent.UsageDirection)GameNetworkMessage.ReadIntFromPacket(CompressionMission.UsageDirectionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
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
			return string.Concat(new object[]
			{
				"StartSwitchingWeaponUsageIndex: ",
				this.UsageIndex,
				" for weapon with EquipmentIndex: ",
				this.EquipmentIndex,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
