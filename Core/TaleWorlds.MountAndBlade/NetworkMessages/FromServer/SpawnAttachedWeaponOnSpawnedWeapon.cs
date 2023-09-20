using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B2 RID: 178
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnAttachedWeaponOnSpawnedWeapon : GameNetworkMessage
	{
		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0000D3E1 File Offset: 0x0000B5E1
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x0000D3E9 File Offset: 0x0000B5E9
		public SpawnedItemEntity SpawnedWeapon { get; private set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x0000D3F2 File Offset: 0x0000B5F2
		// (set) Token: 0x06000749 RID: 1865 RVA: 0x0000D3FA File Offset: 0x0000B5FA
		public int AttachmentIndex { get; private set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x0000D403 File Offset: 0x0000B603
		// (set) Token: 0x0600074B RID: 1867 RVA: 0x0000D40B File Offset: 0x0000B60B
		public int ForcedIndex { get; private set; }

		// Token: 0x0600074C RID: 1868 RVA: 0x0000D414 File Offset: 0x0000B614
		public SpawnAttachedWeaponOnSpawnedWeapon(SpawnedItemEntity spawnedWeapon, int attachmentIndex, int forcedIndex)
		{
			this.SpawnedWeapon = spawnedWeapon;
			this.AttachmentIndex = attachmentIndex;
			this.ForcedIndex = forcedIndex;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0000D431 File Offset: 0x0000B631
		public SpawnAttachedWeaponOnSpawnedWeapon()
		{
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0000D43C File Offset: 0x0000B63C
		protected override bool OnRead()
		{
			bool flag = true;
			this.SpawnedWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SpawnedItemEntity;
			this.AttachmentIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x0000D482 File Offset: 0x0000B682
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SpawnedWeapon);
			GameNetworkMessage.WriteIntToPacket(this.AttachmentIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x0000D4AF File Offset: 0x0000B6AF
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x0000D4B4 File Offset: 0x0000B6B4
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
