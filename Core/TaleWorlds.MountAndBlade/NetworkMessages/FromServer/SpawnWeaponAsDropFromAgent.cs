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
		public Agent Agent { get; private set; }

		public EquipmentIndex EquipmentIndex { get; private set; }

		public Vec3 Velocity { get; private set; }

		public Vec3 AngularVelocity { get; private set; }

		public Mission.WeaponSpawnFlags WeaponSpawnFlags { get; private set; }

		public int ForcedIndex { get; private set; }

		public SpawnWeaponAsDropFromAgent(Agent agent, EquipmentIndex equipmentIndex, Vec3 velocity, Vec3 angularVelocity, Mission.WeaponSpawnFlags weaponSpawnFlags, int forcedIndex)
		{
			this.Agent = agent;
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
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.EquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.WeaponSpawnFlags = (Mission.WeaponSpawnFlags)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo, ref flag);
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
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.WeaponSpawnFlags, CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo);
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
			object[] array = new object[6];
			array[0] = "Spawn Weapon from agent with index: ";
			int num = 1;
			Agent agent = this.Agent;
			array[num] = ((agent != null) ? agent.Index : (-1));
			array[2] = " from equipment index: ";
			array[3] = this.EquipmentIndex;
			array[4] = ", and with ID: ";
			array[5] = this.ForcedIndex;
			return string.Concat(array);
		}
	}
}
