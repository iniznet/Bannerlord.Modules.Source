using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class EquipWeaponFromSpawnedItemEntity : GameNetworkMessage
	{
		public MissionObjectId SpawnedItemEntityId { get; private set; }

		public EquipmentIndex SlotIndex { get; private set; }

		public int AgentIndex { get; private set; }

		public bool RemoveWeapon { get; private set; }

		public EquipWeaponFromSpawnedItemEntity(int agentIndex, EquipmentIndex slot, MissionObjectId spawnedItemEntityId, bool removeWeapon)
		{
			this.AgentIndex = agentIndex;
			this.SlotIndex = slot;
			this.SpawnedItemEntityId = spawnedItemEntityId;
			this.RemoveWeapon = removeWeapon;
		}

		public EquipWeaponFromSpawnedItemEntity()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteMissionObjectIdToPacket((this.SpawnedItemEntityId.Id >= 0) ? this.SpawnedItemEntityId : MissionObjectId.Invalid);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.RemoveWeapon);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.SpawnedItemEntityId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.RemoveWeapon = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"EquipWeaponFromSpawnedItemEntity with missionObjectId: ",
				this.SpawnedItemEntityId,
				" to SlotIndex: ",
				this.SlotIndex,
				" on agent-index: ",
				this.AgentIndex,
				" RemoveWeapon: ",
				this.RemoveWeapon.ToString()
			});
		}
	}
}
