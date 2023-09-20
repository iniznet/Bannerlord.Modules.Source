using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000043 RID: 67
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class EquipEquipmentToPeer : GameNetworkMessage
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000237 RID: 567 RVA: 0x00004B96 File Offset: 0x00002D96
		// (set) Token: 0x06000238 RID: 568 RVA: 0x00004B9E File Offset: 0x00002D9E
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000239 RID: 569 RVA: 0x00004BA7 File Offset: 0x00002DA7
		// (set) Token: 0x0600023A RID: 570 RVA: 0x00004BAF File Offset: 0x00002DAF
		public Equipment Equipment { get; private set; }

		// Token: 0x0600023B RID: 571 RVA: 0x00004BB8 File Offset: 0x00002DB8
		public EquipEquipmentToPeer(NetworkCommunicator peer, Equipment equipment)
		{
			this.Peer = peer;
			this.Equipment = new Equipment();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				this.Equipment[equipmentIndex] = equipment.GetEquipmentFromSlot(equipmentIndex);
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00004BFD File Offset: 0x00002DFD
		public EquipEquipmentToPeer()
		{
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00004C08 File Offset: 0x00002E08
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

		// Token: 0x0600023E RID: 574 RVA: 0x00004C5C File Offset: 0x00002E5C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				ModuleNetworkData.WriteItemReferenceToPacket(this.Equipment.GetEquipmentFromSlot(equipmentIndex));
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00004C92 File Offset: 0x00002E92
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00004C97 File Offset: 0x00002E97
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
