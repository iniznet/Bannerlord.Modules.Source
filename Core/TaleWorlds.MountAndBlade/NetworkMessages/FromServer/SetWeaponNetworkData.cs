using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWeaponNetworkData : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public EquipmentIndex WeaponEquipmentIndex { get; private set; }

		public short DataValue { get; private set; }

		public SetWeaponNetworkData(Agent agent, EquipmentIndex weaponEquipmentIndex, short dataValue)
		{
			this.Agent = agent;
			this.WeaponEquipmentIndex = weaponEquipmentIndex;
			this.DataValue = dataValue;
		}

		public SetWeaponNetworkData()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.WeaponEquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.DataValue = (short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemDataCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.WeaponEquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.DataValue, CompressionMission.ItemDataCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Network data: ",
				this.DataValue,
				" for weapon with EquipmentIndex: ",
				this.WeaponEquipmentIndex,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
