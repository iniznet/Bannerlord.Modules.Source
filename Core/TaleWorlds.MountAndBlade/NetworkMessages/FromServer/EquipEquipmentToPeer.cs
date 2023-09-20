using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class EquipEquipmentToPeer : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public Equipment Equipment { get; private set; }

		public EquipEquipmentToPeer(NetworkCommunicator peer, Equipment equipment)
		{
			this.Peer = peer;
			this.Equipment = new Equipment();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				this.Equipment[equipmentIndex] = equipment.GetEquipmentFromSlot(equipmentIndex);
			}
		}

		public EquipEquipmentToPeer()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			if (flag)
			{
				this.Equipment = new Equipment();
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
				{
					if (flag)
					{
						this.Equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref flag));
					}
				}
			}
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				ModuleNetworkData.WriteItemReferenceToPacket(this.Equipment.GetEquipmentFromSlot(equipmentIndex));
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Equip equipment to peer: ",
				this.Peer.UserName,
				" with peer-index:",
				this.Peer.Index
			});
		}
	}
}
