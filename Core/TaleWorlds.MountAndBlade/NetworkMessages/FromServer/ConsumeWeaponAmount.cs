using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ConsumeWeaponAmount : GameNetworkMessage
	{
		public MissionObject SpawnedItemEntity { get; private set; }

		public short ConsumedAmount { get; private set; }

		public ConsumeWeaponAmount(SpawnedItemEntity spawnedItemEntity, short consumedAmount)
		{
			this.SpawnedItemEntity = spawnedItemEntity;
			this.ConsumedAmount = consumedAmount;
		}

		public ConsumeWeaponAmount()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SpawnedItemEntity);
			GameNetworkMessage.WriteIntToPacket((int)this.ConsumedAmount, CompressionGeneric.ItemDataValueCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SpawnedItemEntity = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.ConsumedAmount = (short)GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.ItemDataValueCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

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
