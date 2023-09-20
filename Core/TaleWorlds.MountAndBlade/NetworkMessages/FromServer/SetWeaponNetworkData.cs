using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000AE RID: 174
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWeaponNetworkData : GameNetworkMessage
	{
		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000710 RID: 1808 RVA: 0x0000CE79 File Offset: 0x0000B079
		// (set) Token: 0x06000711 RID: 1809 RVA: 0x0000CE81 File Offset: 0x0000B081
		public Agent Agent { get; private set; }

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000712 RID: 1810 RVA: 0x0000CE8A File Offset: 0x0000B08A
		// (set) Token: 0x06000713 RID: 1811 RVA: 0x0000CE92 File Offset: 0x0000B092
		public EquipmentIndex WeaponEquipmentIndex { get; private set; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000714 RID: 1812 RVA: 0x0000CE9B File Offset: 0x0000B09B
		// (set) Token: 0x06000715 RID: 1813 RVA: 0x0000CEA3 File Offset: 0x0000B0A3
		public short DataValue { get; private set; }

		// Token: 0x06000716 RID: 1814 RVA: 0x0000CEAC File Offset: 0x0000B0AC
		public SetWeaponNetworkData(Agent agent, EquipmentIndex weaponEquipmentIndex, short dataValue)
		{
			this.Agent = agent;
			this.WeaponEquipmentIndex = weaponEquipmentIndex;
			this.DataValue = dataValue;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x0000CEC9 File Offset: 0x0000B0C9
		public SetWeaponNetworkData()
		{
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0000CED4 File Offset: 0x0000B0D4
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.WeaponEquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.DataValue = (short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemDataCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0000CF17 File Offset: 0x0000B117
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.WeaponEquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.DataValue, CompressionMission.ItemDataCompressionInfo);
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0000CF44 File Offset: 0x0000B144
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x0000CF4C File Offset: 0x0000B14C
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
