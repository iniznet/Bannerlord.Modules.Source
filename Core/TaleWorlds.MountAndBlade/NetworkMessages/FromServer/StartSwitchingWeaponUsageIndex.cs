using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B5 RID: 181
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StartSwitchingWeaponUsageIndex : GameNetworkMessage
	{
		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000778 RID: 1912 RVA: 0x0000D92C File Offset: 0x0000BB2C
		// (set) Token: 0x06000779 RID: 1913 RVA: 0x0000D934 File Offset: 0x0000BB34
		public Agent Agent { get; private set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x0000D93D File Offset: 0x0000BB3D
		// (set) Token: 0x0600077B RID: 1915 RVA: 0x0000D945 File Offset: 0x0000BB45
		public EquipmentIndex EquipmentIndex { get; private set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x0600077C RID: 1916 RVA: 0x0000D94E File Offset: 0x0000BB4E
		// (set) Token: 0x0600077D RID: 1917 RVA: 0x0000D956 File Offset: 0x0000BB56
		public int UsageIndex { get; private set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x0000D95F File Offset: 0x0000BB5F
		// (set) Token: 0x0600077F RID: 1919 RVA: 0x0000D967 File Offset: 0x0000BB67
		public Agent.UsageDirection CurrentMovementFlagUsageDirection { get; private set; }

		// Token: 0x06000780 RID: 1920 RVA: 0x0000D970 File Offset: 0x0000BB70
		public StartSwitchingWeaponUsageIndex(Agent agent, EquipmentIndex equipmentIndex, int usageIndex, Agent.UsageDirection currentMovementFlagUsageDirection)
		{
			this.Agent = agent;
			this.EquipmentIndex = equipmentIndex;
			this.UsageIndex = usageIndex;
			this.CurrentMovementFlagUsageDirection = currentMovementFlagUsageDirection;
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x0000D995 File Offset: 0x0000BB95
		public StartSwitchingWeaponUsageIndex()
		{
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x0000D9A0 File Offset: 0x0000BBA0
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.EquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.UsageIndex = (int)((short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag));
			this.CurrentMovementFlagUsageDirection = (Agent.UsageDirection)GameNetworkMessage.ReadIntFromPacket(CompressionMission.UsageDirectionCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0000D9F5 File Offset: 0x0000BBF5
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.UsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.CurrentMovementFlagUsageDirection, CompressionMission.UsageDirectionCompressionInfo);
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0000DA32 File Offset: 0x0000BC32
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0000DA38 File Offset: 0x0000BC38
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"StartSwitchingWeaponUsageIndex: ",
				this.UsageIndex,
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
