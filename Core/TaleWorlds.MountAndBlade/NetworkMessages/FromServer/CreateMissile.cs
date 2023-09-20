using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000076 RID: 118
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateMissile : GameNetworkMessage
	{
		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x00008D6E File Offset: 0x00006F6E
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x00008D76 File Offset: 0x00006F76
		public int MissileIndex { get; private set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x00008D7F File Offset: 0x00006F7F
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x00008D87 File Offset: 0x00006F87
		public Agent Agent { get; private set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x00008D90 File Offset: 0x00006F90
		// (set) Token: 0x06000499 RID: 1177 RVA: 0x00008D98 File Offset: 0x00006F98
		public EquipmentIndex WeaponIndex { get; private set; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x00008DA1 File Offset: 0x00006FA1
		// (set) Token: 0x0600049B RID: 1179 RVA: 0x00008DA9 File Offset: 0x00006FA9
		public MissionWeapon Weapon { get; private set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x00008DB2 File Offset: 0x00006FB2
		// (set) Token: 0x0600049D RID: 1181 RVA: 0x00008DBA File Offset: 0x00006FBA
		public Vec3 Position { get; private set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600049E RID: 1182 RVA: 0x00008DC3 File Offset: 0x00006FC3
		// (set) Token: 0x0600049F RID: 1183 RVA: 0x00008DCB File Offset: 0x00006FCB
		public Vec3 Direction { get; private set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x00008DD4 File Offset: 0x00006FD4
		// (set) Token: 0x060004A1 RID: 1185 RVA: 0x00008DDC File Offset: 0x00006FDC
		public float Speed { get; private set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060004A2 RID: 1186 RVA: 0x00008DE5 File Offset: 0x00006FE5
		// (set) Token: 0x060004A3 RID: 1187 RVA: 0x00008DED File Offset: 0x00006FED
		public Mat3 Orientation { get; private set; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060004A4 RID: 1188 RVA: 0x00008DF6 File Offset: 0x00006FF6
		// (set) Token: 0x060004A5 RID: 1189 RVA: 0x00008DFE File Offset: 0x00006FFE
		public bool HasRigidBody { get; private set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060004A6 RID: 1190 RVA: 0x00008E07 File Offset: 0x00007007
		// (set) Token: 0x060004A7 RID: 1191 RVA: 0x00008E0F File Offset: 0x0000700F
		public MissionObject MissionObjectToIgnore { get; private set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x00008E18 File Offset: 0x00007018
		// (set) Token: 0x060004A9 RID: 1193 RVA: 0x00008E20 File Offset: 0x00007020
		public bool IsPrimaryWeaponShot { get; private set; }

		// Token: 0x060004AA RID: 1194 RVA: 0x00008E2C File Offset: 0x0000702C
		public CreateMissile(int missileIndex, Agent agent, EquipmentIndex weaponIndex, MissionWeapon weapon, Vec3 position, Vec3 direction, float speed, Mat3 orientation, bool hasRigidBody, MissionObject missionObjectToIgnore, bool isPrimaryWeaponShot)
		{
			this.MissileIndex = missileIndex;
			this.Agent = agent;
			this.WeaponIndex = weaponIndex;
			this.Weapon = weapon;
			this.Position = position;
			this.Direction = direction;
			this.Speed = speed;
			this.Orientation = orientation;
			this.HasRigidBody = hasRigidBody;
			this.MissionObjectToIgnore = missionObjectToIgnore;
			this.IsPrimaryWeaponShot = isPrimaryWeaponShot;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00008E94 File Offset: 0x00007094
		public CreateMissile()
		{
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00008E9C File Offset: 0x0000709C
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissileIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissileCompressionInfo, ref flag);
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.WeaponIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			if (this.WeaponIndex == EquipmentIndex.None)
			{
				this.Weapon = ModuleNetworkData.ReadMissileWeaponReferenceFromPacket(Game.Current.ObjectManager, ref flag);
			}
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.MissileSpeedCompressionInfo, ref flag);
			this.HasRigidBody = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			if (this.HasRigidBody)
			{
				this.Orientation = GameNetworkMessage.ReadRotationMatrixFromPacket(ref flag);
				this.MissionObjectToIgnore = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			}
			else
			{
				Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
				this.Orientation = new Mat3(Vec3.Side, vec, Vec3.Up);
				this.Orientation.Orthonormalize();
			}
			this.IsPrimaryWeaponShot = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00008FA4 File Offset: 0x000071A4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.MissileIndex, CompressionMission.MissileCompressionInfo);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.WeaponIndex, CompressionMission.WieldSlotCompressionInfo);
			if (this.WeaponIndex == EquipmentIndex.None)
			{
				ModuleNetworkData.WriteMissileWeaponReferenceToPacket(this.Weapon);
			}
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionMission.MissileSpeedCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.HasRigidBody);
			if (this.HasRigidBody)
			{
				GameNetworkMessage.WriteRotationMatrixToPacket(this.Orientation);
				GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObjectToIgnore);
			}
			else
			{
				GameNetworkMessage.WriteVec3ToPacket(this.Orientation.f, CompressionBasic.UnitVectorCompressionInfo);
			}
			GameNetworkMessage.WriteBoolToPacket(this.IsPrimaryWeaponShot);
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0000906B File Offset: 0x0000726B
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00009074 File Offset: 0x00007274
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Create a missile with index: ",
				this.MissileIndex,
				" on agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
