using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B0 RID: 176
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWieldedItemIndex : GameNetworkMessage
	{
		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000728 RID: 1832 RVA: 0x0000D109 File Offset: 0x0000B309
		// (set) Token: 0x06000729 RID: 1833 RVA: 0x0000D111 File Offset: 0x0000B311
		public Agent Agent { get; private set; }

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600072A RID: 1834 RVA: 0x0000D11A File Offset: 0x0000B31A
		// (set) Token: 0x0600072B RID: 1835 RVA: 0x0000D122 File Offset: 0x0000B322
		public bool IsLeftHand { get; private set; }

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600072C RID: 1836 RVA: 0x0000D12B File Offset: 0x0000B32B
		// (set) Token: 0x0600072D RID: 1837 RVA: 0x0000D133 File Offset: 0x0000B333
		public bool IsWieldedInstantly { get; private set; }

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600072E RID: 1838 RVA: 0x0000D13C File Offset: 0x0000B33C
		// (set) Token: 0x0600072F RID: 1839 RVA: 0x0000D144 File Offset: 0x0000B344
		public bool IsWieldedOnSpawn { get; private set; }

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000730 RID: 1840 RVA: 0x0000D14D File Offset: 0x0000B34D
		// (set) Token: 0x06000731 RID: 1841 RVA: 0x0000D155 File Offset: 0x0000B355
		public EquipmentIndex WieldedItemIndex { get; private set; }

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000732 RID: 1842 RVA: 0x0000D15E File Offset: 0x0000B35E
		// (set) Token: 0x06000733 RID: 1843 RVA: 0x0000D166 File Offset: 0x0000B366
		public int MainHandCurrentUsageIndex { get; private set; }

		// Token: 0x06000734 RID: 1844 RVA: 0x0000D16F File Offset: 0x0000B36F
		public SetWieldedItemIndex(Agent agent, bool isLeftHand, bool isWieldedInstantly, bool isWieldedOnSpawn, EquipmentIndex wieldedItemIndex, int mainHandCurUsageIndex)
		{
			this.Agent = agent;
			this.IsLeftHand = isLeftHand;
			this.IsWieldedInstantly = isWieldedInstantly;
			this.IsWieldedOnSpawn = isWieldedOnSpawn;
			this.WieldedItemIndex = wieldedItemIndex;
			this.MainHandCurrentUsageIndex = mainHandCurUsageIndex;
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x0000D1A4 File Offset: 0x0000B3A4
		public SetWieldedItemIndex()
		{
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x0000D1AC File Offset: 0x0000B3AC
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsLeftHand = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsWieldedInstantly = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsWieldedOnSpawn = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.WieldedItemIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			this.MainHandCurrentUsageIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0000D218 File Offset: 0x0000B418
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteBoolToPacket(this.IsLeftHand);
			GameNetworkMessage.WriteBoolToPacket(this.IsWieldedInstantly);
			GameNetworkMessage.WriteBoolToPacket(this.IsWieldedOnSpawn);
			GameNetworkMessage.WriteIntToPacket((int)this.WieldedItemIndex, CompressionMission.WieldSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.MainHandCurrentUsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0000D271 File Offset: 0x0000B471
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x0000D278 File Offset: 0x0000B478
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Wielded Item Index to: ",
				this.WieldedItemIndex,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
