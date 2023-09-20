using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000078 RID: 120
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class EquipWeaponFromSpawnedItemEntity : GameNetworkMessage
	{
		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x0000929D File Offset: 0x0000749D
		// (set) Token: 0x060004BF RID: 1215 RVA: 0x000092A5 File Offset: 0x000074A5
		public SpawnedItemEntity SpawnedItemEntity { get; private set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060004C0 RID: 1216 RVA: 0x000092AE File Offset: 0x000074AE
		// (set) Token: 0x060004C1 RID: 1217 RVA: 0x000092B6 File Offset: 0x000074B6
		public EquipmentIndex SlotIndex { get; private set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060004C2 RID: 1218 RVA: 0x000092BF File Offset: 0x000074BF
		// (set) Token: 0x060004C3 RID: 1219 RVA: 0x000092C7 File Offset: 0x000074C7
		public Agent Agent { get; private set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060004C4 RID: 1220 RVA: 0x000092D0 File Offset: 0x000074D0
		// (set) Token: 0x060004C5 RID: 1221 RVA: 0x000092D8 File Offset: 0x000074D8
		public bool RemoveWeapon { get; private set; }

		// Token: 0x060004C6 RID: 1222 RVA: 0x000092E1 File Offset: 0x000074E1
		public EquipWeaponFromSpawnedItemEntity(Agent a, EquipmentIndex slot, SpawnedItemEntity spawnedItemEntity, bool removeWeapon)
		{
			this.Agent = a;
			this.SlotIndex = slot;
			this.SpawnedItemEntity = spawnedItemEntity;
			this.RemoveWeapon = removeWeapon;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x00009306 File Offset: 0x00007506
		public EquipWeaponFromSpawnedItemEntity()
		{
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0000930E File Offset: 0x0000750E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SpawnedItemEntity);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.RemoveWeapon);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00009344 File Offset: 0x00007544
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.SpawnedItemEntity = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SpawnedItemEntity;
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.RemoveWeapon = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x00009393 File Offset: 0x00007593
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0000939C File Offset: 0x0000759C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"EquipWeaponFromSpawnedItemEntity with entity name: ",
				(this.SpawnedItemEntity != null) ? ((this.SpawnedItemEntity.GameEntity != null) ? this.SpawnedItemEntity.GameEntity.Name : "null entity") : "null spawned item",
				" to SlotIndex: ",
				this.SlotIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index,
				" RemoveWeapon: ",
				this.RemoveWeapon.ToString()
			});
		}
	}
}
