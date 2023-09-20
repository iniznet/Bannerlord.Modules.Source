using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class EquipWeaponFromSpawnedItemEntity : GameNetworkMessage
	{
		public SpawnedItemEntity SpawnedItemEntity { get; private set; }

		public EquipmentIndex SlotIndex { get; private set; }

		public Agent Agent { get; private set; }

		public bool RemoveWeapon { get; private set; }

		public EquipWeaponFromSpawnedItemEntity(Agent a, EquipmentIndex slot, SpawnedItemEntity spawnedItemEntity, bool removeWeapon)
		{
			this.Agent = a;
			this.SlotIndex = slot;
			this.SpawnedItemEntity = spawnedItemEntity;
			this.RemoveWeapon = removeWeapon;
		}

		public EquipWeaponFromSpawnedItemEntity()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SpawnedItemEntity);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.RemoveWeapon);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.SpawnedItemEntity = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SpawnedItemEntity;
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
				"EquipWeaponFromSpawnedItemEntity with entity name: ",
				(this.SpawnedItemEntity != null) ? ((this.SpawnedItemEntity.GameEntity != null) ? this.SpawnedItemEntity.GameEntity.Name : "null entity") : "null spawned item",
				" to SlotIndex: ",
				this.SlotIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index,
				" RemoveWeapon: ",
				this.RemoveWeapon.ToString()
			});
		}
	}
}
