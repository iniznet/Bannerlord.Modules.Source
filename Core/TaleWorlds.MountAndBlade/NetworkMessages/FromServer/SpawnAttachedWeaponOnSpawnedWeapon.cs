using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnAttachedWeaponOnSpawnedWeapon : GameNetworkMessage
	{
		public MissionObjectId SpawnedWeaponId { get; private set; }

		public int AttachmentIndex { get; private set; }

		public int ForcedIndex { get; private set; }

		public SpawnAttachedWeaponOnSpawnedWeapon(MissionObjectId spawnedWeaponId, int attachmentIndex, int forcedIndex)
		{
			this.SpawnedWeaponId = spawnedWeaponId;
			this.AttachmentIndex = attachmentIndex;
			this.ForcedIndex = forcedIndex;
		}

		public SpawnAttachedWeaponOnSpawnedWeapon()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SpawnedWeaponId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.AttachmentIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.SpawnedWeaponId);
			GameNetworkMessage.WriteIntToPacket(this.AttachmentIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "SpawnAttachedWeaponOnSpawnedWeapon with Spawned Weapon ID: ", this.SpawnedWeaponId, " AttachmentIndex: ", this.AttachmentIndex, " Attached Weapon ID: ", this.ForcedIndex });
		}
	}
}
