using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnAttachedWeaponOnSpawnedWeapon : GameNetworkMessage
	{
		public SpawnedItemEntity SpawnedWeapon { get; private set; }

		public int AttachmentIndex { get; private set; }

		public int ForcedIndex { get; private set; }

		public SpawnAttachedWeaponOnSpawnedWeapon(SpawnedItemEntity spawnedWeapon, int attachmentIndex, int forcedIndex)
		{
			this.SpawnedWeapon = spawnedWeapon;
			this.AttachmentIndex = attachmentIndex;
			this.ForcedIndex = forcedIndex;
		}

		public SpawnAttachedWeaponOnSpawnedWeapon()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SpawnedWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SpawnedItemEntity;
			this.AttachmentIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SpawnedWeapon);
			GameNetworkMessage.WriteIntToPacket(this.AttachmentIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"SpawnAttachedWeaponOnSpawnedWeapon with Spawned Weapon ID: ",
				this.SpawnedWeapon.Id.Id,
				" AttachmentIndex: ",
				this.AttachmentIndex,
				" Attached Weapon ID: ",
				this.ForcedIndex
			});
		}
	}
}
