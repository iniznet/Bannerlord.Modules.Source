using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnWeaponAsDropFromAgent : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public EquipmentIndex EquipmentIndex { get; private set; }

		public Vec3 Velocity { get; private set; }

		public Vec3 AngularVelocity { get; private set; }

		public Mission.WeaponSpawnFlags WeaponSpawnFlags { get; private set; }

		public int ForcedIndex { get; private set; }

		public SpawnWeaponAsDropFromAgent(int agentIndex, EquipmentIndex equipmentIndex, Vec3 velocity, Vec3 angularVelocity, Mission.WeaponSpawnFlags weaponSpawnFlags, int forcedIndex)
		{
			this.AgentIndex = agentIndex;
			this.EquipmentIndex = equipmentIndex;
			this.Velocity = velocity;
			this.AngularVelocity = angularVelocity;
			this.WeaponSpawnFlags = weaponSpawnFlags;
			this.ForcedIndex = forcedIndex;
		}

		public SpawnWeaponAsDropFromAgent()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.EquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.WeaponSpawnFlags = (Mission.WeaponSpawnFlags)GameNetworkMessage.ReadUintFromPacket(CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo, ref flag);
			if (this.WeaponSpawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithPhysics))
			{
				this.Velocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemVelocityCompressionInfo, ref flag);
				this.AngularVelocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemAngularVelocityCompressionInfo, ref flag);
			}
			else
			{
				this.Velocity = Vec3.Zero;
				this.AngularVelocity = Vec3.Zero;
			}
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket((int)this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteUintToPacket((uint)this.WeaponSpawnFlags, CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo);
			if (this.WeaponSpawnFlags.HasAnyFlag(Mission.WeaponSpawnFlags.WithPhysics))
			{
				GameNetworkMessage.WriteVec3ToPacket(this.Velocity, CompressionMission.SpawnedItemVelocityCompressionInfo);
				GameNetworkMessage.WriteVec3ToPacket(this.AngularVelocity, CompressionMission.SpawnedItemAngularVelocityCompressionInfo);
			}
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Spawn Weapon from agent with agent-index: ", this.AgentIndex, " from equipment index: ", this.EquipmentIndex, ", and with ID: ", this.ForcedIndex });
		}
	}
}
