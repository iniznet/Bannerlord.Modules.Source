using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateMissile : GameNetworkMessage
	{
		public int MissileIndex { get; private set; }

		public Agent Agent { get; private set; }

		public EquipmentIndex WeaponIndex { get; private set; }

		public MissionWeapon Weapon { get; private set; }

		public Vec3 Position { get; private set; }

		public Vec3 Direction { get; private set; }

		public float Speed { get; private set; }

		public Mat3 Orientation { get; private set; }

		public bool HasRigidBody { get; private set; }

		public MissionObject MissionObjectToIgnore { get; private set; }

		public bool IsPrimaryWeaponShot { get; private set; }

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

		public CreateMissile()
		{
		}

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

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

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
