using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWeaponAmmoData : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public EquipmentIndex WeaponEquipmentIndex { get; private set; }

		public EquipmentIndex AmmoEquipmentIndex { get; private set; }

		public short Ammo { get; private set; }

		public SetWeaponAmmoData(int agentIndex, EquipmentIndex weaponEquipmentIndex, EquipmentIndex ammoEquipmentIndex, short ammo)
		{
			this.AgentIndex = agentIndex;
			this.WeaponEquipmentIndex = weaponEquipmentIndex;
			this.AmmoEquipmentIndex = ammoEquipmentIndex;
			this.Ammo = ammo;
		}

		public SetWeaponAmmoData()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.WeaponEquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.AmmoEquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			this.Ammo = (short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemDataCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket((int)this.WeaponEquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.AmmoEquipmentIndex, CompressionMission.WieldSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.Ammo, CompressionMission.ItemDataCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set ammo: ", this.Ammo, " for weapon with EquipmentIndex: ", this.WeaponEquipmentIndex, " on Agent with agent-index: ", this.AgentIndex });
		}
	}
}
