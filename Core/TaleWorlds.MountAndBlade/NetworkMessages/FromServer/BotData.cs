using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000045 RID: 69
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BotData : GameNetworkMessage
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600024B RID: 587 RVA: 0x00004DCB File Offset: 0x00002FCB
		// (set) Token: 0x0600024C RID: 588 RVA: 0x00004DD3 File Offset: 0x00002FD3
		public BattleSideEnum Side { get; private set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600024D RID: 589 RVA: 0x00004DDC File Offset: 0x00002FDC
		// (set) Token: 0x0600024E RID: 590 RVA: 0x00004DE4 File Offset: 0x00002FE4
		public int KillCount { get; private set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600024F RID: 591 RVA: 0x00004DED File Offset: 0x00002FED
		// (set) Token: 0x06000250 RID: 592 RVA: 0x00004DF5 File Offset: 0x00002FF5
		public int AssistCount { get; private set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000251 RID: 593 RVA: 0x00004DFE File Offset: 0x00002FFE
		// (set) Token: 0x06000252 RID: 594 RVA: 0x00004E06 File Offset: 0x00003006
		public int DeathCount { get; private set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000253 RID: 595 RVA: 0x00004E0F File Offset: 0x0000300F
		// (set) Token: 0x06000254 RID: 596 RVA: 0x00004E17 File Offset: 0x00003017
		public int AliveBotCount { get; private set; }

		// Token: 0x06000255 RID: 597 RVA: 0x00004E20 File Offset: 0x00003020
		public BotData(BattleSideEnum side, int kill, int assist, int death, int alive)
		{
			this.Side = side;
			this.KillCount = kill;
			this.AssistCount = assist;
			this.DeathCount = death;
			this.AliveBotCount = alive;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00004E4D File Offset: 0x0000304D
		public BotData()
		{
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00004E58 File Offset: 0x00003058
		protected override bool OnRead()
		{
			bool flag = true;
			this.Side = (BattleSideEnum)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			this.KillCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.AssistCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.DeathCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.AliveBotCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00004EC4 File Offset: 0x000030C4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.Side, CompressionMission.TeamSideCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.KillCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AssistCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DeathCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AliveBotCount, CompressionMission.AgentCompressionInfo);
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00004F21 File Offset: 0x00003121
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00004F28 File Offset: 0x00003128
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "BOTS for side: ", this.Side, ", Kill: ", this.KillCount, " Death: ", this.DeathCount, " Assist: ", this.AssistCount, ", Alive: ", this.AliveBotCount });
		}
	}
}
