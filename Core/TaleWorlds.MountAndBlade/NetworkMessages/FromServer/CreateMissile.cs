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

		public int AgentIndex { get; private set; }

		public EquipmentIndex WeaponIndex { get; private set; }

		public MissionWeapon Weapon { get; private set; }

		public Vec3 Position { get; private set; }

		public Vec3 Direction { get; private set; }

		public float Speed { get; private set; }

		public Mat3 Orientation { get; private set; }

		public bool HasRigidBody { get; private set; }

		public MissionObjectId MissionObjectToIgnoreId { get; private set; }

		public bool IsPrimaryWeaponShot { get; private set; }

		public CreateMissile(int missileIndex, int agentIndex, EquipmentIndex weaponIndex, MissionWeapon weapon, Vec3 position, Vec3 direction, float speed, Mat3 orientation, bool hasRigidBody, MissionObjectId missionObjectToIgnoreId, bool isPrimaryWeaponShot)
		{
			this.MissileIndex = missileIndex;
			this.AgentIndex = agentIndex;
			this.WeaponIndex = weaponIndex;
			this.Weapon = weapon;
			this.Position = position;
			this.Direction = direction;
			this.Speed = speed;
			this.Orientation = orientation;
			this.HasRigidBody = hasRigidBody;
			this.MissionObjectToIgnoreId = missionObjectToIgnoreId;
			this.IsPrimaryWeaponShot = isPrimaryWeaponShot;
		}

		public CreateMissile()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissileIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissileCompressionInfo, ref flag);
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
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
				this.MissionObjectToIgnoreId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
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
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
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
				GameNetworkMessage.WriteMissionObjectIdToPacket((this.MissionObjectToIgnoreId.Id >= 0) ? this.MissionObjectToIgnoreId : MissionObjectId.Invalid);
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
			return string.Concat(new object[] { "Create a missile with index: ", this.MissileIndex, " on agent with agent-index: ", this.AgentIndex });
		}
	}
}
