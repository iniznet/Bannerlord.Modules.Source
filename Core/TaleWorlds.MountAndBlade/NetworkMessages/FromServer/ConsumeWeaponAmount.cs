using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ConsumeWeaponAmount : GameNetworkMessage
	{
		public MissionObjectId SpawnedItemEntityId { get; private set; }

		public short ConsumedAmount { get; private set; }

		public ConsumeWeaponAmount(MissionObjectId spawnedItemEntityId, short consumedAmount)
		{
			this.SpawnedItemEntityId = spawnedItemEntityId;
			this.ConsumedAmount = consumedAmount;
		}

		public ConsumeWeaponAmount()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.SpawnedItemEntityId);
			GameNetworkMessage.WriteIntToPacket((int)this.ConsumedAmount, CompressionBasic.ItemDataValueCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SpawnedItemEntityId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.ConsumedAmount = (short)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ItemDataValueCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Consumed ", this.ConsumedAmount, " from ", this.SpawnedItemEntityId });
		}
	}
}
