using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B3 RID: 179
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnWeaponAsDropFromAgent : GameNetworkMessage
	{
		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x0000D518 File Offset: 0x0000B718
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x0000D520 File Offset: 0x0000B720
		public Agent Agent { get; private set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x0000D529 File Offset: 0x0000B729
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x0000D531 File Offset: 0x0000B731
		public EquipmentIndex EquipmentIndex { get; private set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000756 RID: 1878 RVA: 0x0000D53A File Offset: 0x0000B73A
		// (set) Token: 0x06000757 RID: 1879 RVA: 0x0000D542 File Offset: 0x0000B742
		public Vec3 Velocity { get; private set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000758 RID: 1880 RVA: 0x0000D54B File Offset: 0x0000B74B
		// (set) Token: 0x06000759 RID: 1881 RVA: 0x0000D553 File Offset: 0x0000B753
		public Vec3 AngularVelocity { get; private set; }

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x0000D55C File Offset: 0x0000B75C
		// (set) Token: 0x0600075B RID: 1883 RVA: 0x0000D564 File Offset: 0x0000B764
		public Mission.WeaponSpawnFlags WeaponSpawnFlags { get; private set; }

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x0000D56D File Offset: 0x0000B76D
		// (set) Token: 0x0600075D RID: 1885 RVA: 0x0000D575 File Offset: 0x0000B775
		public int ForcedIndex { get; private set; }

		// Token: 0x0600075E RID: 1886 RVA: 0x0000D57E File Offset: 0x0000B77E
		public SpawnWeaponAsDropFromAgent(Agent agent, EquipmentIndex equipmentIndex, Vec3 velocity, Vec3 angularVelocity, Mission.WeaponSpawnFlags weaponSpawnFlags, int forcedIndex)
		{
			this.Agent = agent;
			this.EquipmentIndex = equipmentIndex;
			this.Velocity = velocity;
			this.AngularVelocity = angularVelocity;
			this.WeaponSpawnFlags = weaponSpawnFlags;
			this.ForcedIndex = forcedIndex;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x0000D5B3 File Offset: 0x0000B7B3
		public SpawnWeaponAsDropFromAgent()
		{
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0000D5BC File Offset: 0x0000B7BC
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

		// Token: 0x06000761 RID: 1889 RVA: 0x0000D65C File Offset: 0x0000B85C
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

		// Token: 0x06000762 RID: 1890 RVA: 0x0000D6D2 File Offset: 0x0000B8D2
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x0000D6D8 File Offset: 0x0000B8D8
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
