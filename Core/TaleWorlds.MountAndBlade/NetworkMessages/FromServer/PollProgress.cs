using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000057 RID: 87
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PollProgress : GameNetworkMessage
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000308 RID: 776 RVA: 0x00006233 File Offset: 0x00004433
		// (set) Token: 0x06000309 RID: 777 RVA: 0x0000623B File Offset: 0x0000443B
		public int VotesAccepted { get; private set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600030A RID: 778 RVA: 0x00006244 File Offset: 0x00004444
		// (set) Token: 0x0600030B RID: 779 RVA: 0x0000624C File Offset: 0x0000444C
		public int VotesRejected { get; private set; }

		// Token: 0x0600030C RID: 780 RVA: 0x00006255 File Offset: 0x00004455
		public PollProgress(int votesAccepted, int votesRejected)
		{
			this.VotesAccepted = votesAccepted;
			this.VotesRejected = votesRejected;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000626B File Offset: 0x0000446B
		public PollProgress()
		{
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00006274 File Offset: 0x00004474
		protected override bool OnRead()
		{
			bool flag = true;
			this.VotesAccepted = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.VotesRejected = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x000062A8 File Offset: 0x000044A8
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.VotesAccepted, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.VotesRejected, CompressionBasic.PlayerCompressionInfo);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x000062CA File Offset: 0x000044CA
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x000062D2 File Offset: 0x000044D2
		protected override string OnGetLogFormat()
		{
			return "Update on the voting progress.";
		}
	}
}
