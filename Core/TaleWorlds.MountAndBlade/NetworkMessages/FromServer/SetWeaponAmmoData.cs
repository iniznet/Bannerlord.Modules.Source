using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000AD RID: 173
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWeaponAmmoData : GameNetworkMessage
	{
		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000702 RID: 1794 RVA: 0x0000CCF9 File Offset: 0x0000AEF9
		// (set) Token: 0x06000703 RID: 1795 RVA: 0x0000CD01 File Offset: 0x0000AF01
		public Agent Agent { get; private set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000704 RID: 1796 RVA: 0x0000CD0A File Offset: 0x0000AF0A
		// (set) Token: 0x06000705 RID: 1797 RVA: 0x0000CD12 File Offset: 0x0000AF12
		public EquipmentIndex WeaponEquipmentIndex { get; private set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000706 RID: 1798 RVA: 0x0000CD1B File Offset: 0x0000AF1B
		// (set) Token: 0x06000707 RID: 1799 RVA: 0x0000CD23 File Offset: 0x0000AF23
		public EquipmentIndex AmmoEquipmentIndex { get; private set; }

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000708 RID: 1800 RVA: 0x0000CD2C File Offset: 0x0000AF2C
		// (set) Token: 0x06000709 RID: 1801 RVA: 0x0000CD34 File Offset: 0x0000AF34
		public short Ammo { get; private set; }

		// Token: 0x0600070A RID: 1802 RVA: 0x0000CD3D File Offset: 0x0000AF3D
		public SetWeaponAmmoData(Agent agent, EquipmentIndex weaponEquipmentIndex, EquipmentIndex ammoEquipmentIndex, short ammo)
		{
			this.Agent = agent;
			this.WeaponEquipmentIndex = weaponEquipmentIndex;
			this.AmmoEquipmentIndex = ammoEquipmentIndex;
			this.Ammo = ammo;
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x0000CD62 File Offset: 0x0000AF62
		public SetWeaponAmmoData()
		{
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0000CD6C File Offset: 0x0000AF6C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.WeaponEquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.AmmoEquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			this.Ammo = (short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemDataCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0000CDC1 File Offset: 0x0000AFC1
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.WeaponEquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.AmmoEquipmentIndex, CompressionMission.WieldSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.Ammo, CompressionMission.ItemDataCompressionInfo);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0000CDFE File Offset: 0x0000AFFE
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0000CE04 File Offset: 0x0000B004
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set ammo: ",
				this.Ammo,
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
