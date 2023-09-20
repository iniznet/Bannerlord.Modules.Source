using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnWeaponWithNewEntity : GameNetworkMessage
	{
		public MissionWeapon Weapon { get; private set; }

		public Mission.WeaponSpawnFlags WeaponSpawnFlags { get; private set; }

		public int ForcedIndex { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public MissionObjectId ParentMissionObjectId { get; private set; }

		public bool IsVisible { get; private set; }

		public bool HasLifeTime { get; private set; }

		public SpawnWeaponWithNewEntity(MissionWeapon weapon, Mission.WeaponSpawnFlags weaponSpawnFlags, int forcedIndex, MatrixFrame frame, MissionObjectId parentMissionObjectId, bool isVisible, bool hasLifeTime)
		{
			this.Weapon = weapon;
			this.WeaponSpawnFlags = weaponSpawnFlags;
			this.ForcedIndex = forcedIndex;
			this.Frame = frame;
			this.ParentMissionObjectId = parentMissionObjectId;
			this.IsVisible = isVisible;
			this.HasLifeTime = hasLifeTime;
		}

		public SpawnWeaponWithNewEntity()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			this.WeaponSpawnFlags = (Mission.WeaponSpawnFlags)GameNetworkMessage.ReadUintFromPacket(CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			this.ParentMissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.IsVisible = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.HasLifeTime = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
			GameNetworkMessage.WriteUintToPacket((uint)this.WeaponSpawnFlags, CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
			GameNetworkMessage.WriteMissionObjectIdToPacket((this.ParentMissionObjectId.Id >= 0) ? this.ParentMissionObjectId : MissionObjectId.Invalid);
			GameNetworkMessage.WriteBoolToPacket(this.IsVisible);
			GameNetworkMessage.WriteBoolToPacket(this.HasLifeTime);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Spawn Weapon with name: ",
				this.Weapon.Item.Name,
				", and with ID: ",
				this.ForcedIndex
			});
		}
	}
}
