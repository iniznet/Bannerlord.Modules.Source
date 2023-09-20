using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000071 RID: 113
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ConsumeWeaponAmount : GameNetworkMessage
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x00008108 File Offset: 0x00006308
		// (set) Token: 0x06000433 RID: 1075 RVA: 0x00008110 File Offset: 0x00006310
		public MissionObject SpawnedItemEntity { get; private set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x00008119 File Offset: 0x00006319
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x00008121 File Offset: 0x00006321
		public short ConsumedAmount { get; private set; }

		// Token: 0x06000436 RID: 1078 RVA: 0x0000812A File Offset: 0x0000632A
		public ConsumeWeaponAmount(SpawnedItemEntity spawnedItemEntity, short consumedAmount)
		{
			this.SpawnedItemEntity = spawnedItemEntity;
			this.ConsumedAmount = consumedAmount;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00008140 File Offset: 0x00006340
		public ConsumeWeaponAmount()
		{
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00008148 File Offset: 0x00006348
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SpawnedItemEntity);
			GameNetworkMessage.WriteIntToPacket((int)this.ConsumedAmount, CompressionGeneric.ItemDataValueCompressionInfo);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00008168 File Offset: 0x00006368
		protected override bool OnRead()
		{
			bool flag = true;
			this.SpawnedItemEntity = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.ConsumedAmount = (short)GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.ItemDataValueCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00008198 File Offset: 0x00006398
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x000081A0 File Offset: 0x000063A0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Consumed ",
				this.ConsumedAmount,
				" from ",
				(this.SpawnedItemEntity as SpawnedItemEntity).WeaponCopy.GetModifiedItemName()
			});
		}
	}
}
