using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000AF RID: 175
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWeaponReloadPhase : GameNetworkMessage
	{
		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600071C RID: 1820 RVA: 0x0000CFC1 File Offset: 0x0000B1C1
		// (set) Token: 0x0600071D RID: 1821 RVA: 0x0000CFC9 File Offset: 0x0000B1C9
		public Agent Agent { get; private set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x0000CFD2 File Offset: 0x0000B1D2
		// (set) Token: 0x0600071F RID: 1823 RVA: 0x0000CFDA File Offset: 0x0000B1DA
		public EquipmentIndex EquipmentIndex { get; private set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000720 RID: 1824 RVA: 0x0000CFE3 File Offset: 0x0000B1E3
		// (set) Token: 0x06000721 RID: 1825 RVA: 0x0000CFEB File Offset: 0x0000B1EB
		public short ReloadPhase { get; private set; }

		// Token: 0x06000722 RID: 1826 RVA: 0x0000CFF4 File Offset: 0x0000B1F4
		public SetWeaponReloadPhase(Agent agent, EquipmentIndex equipmentIndex, short reloadPhase)
		{
			this.Agent = agent;
			this.EquipmentIndex = equipmentIndex;
			this.ReloadPhase = reloadPhase;
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x0000D011 File Offset: 0x0000B211
		public SetWeaponReloadPhase()
		{
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x0000D01C File Offset: 0x0000B21C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.EquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.ReloadPhase = (short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponReloadPhaseCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x0000D05F File Offset: 0x0000B25F
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.ReloadPhase, CompressionMission.WeaponReloadPhaseCompressionInfo);
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0000D08C File Offset: 0x0000B28C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0000D094 File Offset: 0x0000B294
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Reload Phase: ",
				this.ReloadPhase,
				" for weapon with EquipmentIndex: ",
				this.EquipmentIndex,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
