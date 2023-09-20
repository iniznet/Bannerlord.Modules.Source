using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200000C RID: 12
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestPerkChange : GameNetworkMessage
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004A RID: 74 RVA: 0x0000265D File Offset: 0x0000085D
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002665 File Offset: 0x00000865
		public int PerkListIndex { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004C RID: 76 RVA: 0x0000266E File Offset: 0x0000086E
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002676 File Offset: 0x00000876
		public int PerkIndex { get; private set; }

		// Token: 0x0600004E RID: 78 RVA: 0x0000267F File Offset: 0x0000087F
		public RequestPerkChange(int perkListIndex, int perkIndex)
		{
			this.PerkListIndex = perkListIndex;
			this.PerkIndex = perkIndex;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002695 File Offset: 0x00000895
		public RequestPerkChange()
		{
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000026A0 File Offset: 0x000008A0
		protected override bool OnRead()
		{
			bool flag = true;
			this.PerkListIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkListIndexCompressionInfo, ref flag);
			this.PerkIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000026D4 File Offset: 0x000008D4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.PerkListIndex, CompressionMission.PerkListIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.PerkIndex, CompressionMission.PerkIndexCompressionInfo);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000026F6 File Offset: 0x000008F6
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000026FB File Offset: 0x000008FB
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Requesting perk selection in list ", this.PerkListIndex, " change to ", this.PerkIndex });
		}
	}
}
