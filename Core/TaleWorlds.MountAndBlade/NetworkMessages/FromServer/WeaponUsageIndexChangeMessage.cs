using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C0 RID: 192
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class WeaponUsageIndexChangeMessage : GameNetworkMessage
	{
		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060007F2 RID: 2034 RVA: 0x0000E567 File Offset: 0x0000C767
		// (set) Token: 0x060007F3 RID: 2035 RVA: 0x0000E56F File Offset: 0x0000C76F
		public Agent Agent { get; private set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060007F4 RID: 2036 RVA: 0x0000E578 File Offset: 0x0000C778
		// (set) Token: 0x060007F5 RID: 2037 RVA: 0x0000E580 File Offset: 0x0000C780
		public EquipmentIndex SlotIndex { get; private set; }

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060007F6 RID: 2038 RVA: 0x0000E589 File Offset: 0x0000C789
		// (set) Token: 0x060007F7 RID: 2039 RVA: 0x0000E591 File Offset: 0x0000C791
		public int UsageIndex { get; private set; }

		// Token: 0x060007F8 RID: 2040 RVA: 0x0000E59A File Offset: 0x0000C79A
		public WeaponUsageIndexChangeMessage()
		{
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0000E5A2 File Offset: 0x0000C7A2
		public WeaponUsageIndexChangeMessage(Agent agent, EquipmentIndex slotIndex, int usageIndex)
		{
			this.Agent = agent;
			this.SlotIndex = slotIndex;
			this.UsageIndex = usageIndex;
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0000E5C0 File Offset: 0x0000C7C0
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.UsageIndex = (int)((short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag));
			return flag;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0000E603 File Offset: 0x0000C803
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.UsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0000E630 File Offset: 0x0000C830
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x0000E638 File Offset: 0x0000C838
		protected override string OnGetLogFormat()
		{
			object[] array = new object[8];
			array[0] = "Set Weapon Usage Index: ";
			array[1] = this.UsageIndex;
			array[2] = " for weapon with EquipmentIndex: ";
			array[3] = this.SlotIndex;
			array[4] = " on Agent with name: ";
			array[5] = ((this.Agent != null) ? this.Agent.Name : "null agent");
			array[6] = " and agent-index: ";
			int num = 7;
			Agent agent = this.Agent;
			array[num] = ((agent != null) ? agent.Index : (-1));
			return string.Concat(array);
		}
	}
}
