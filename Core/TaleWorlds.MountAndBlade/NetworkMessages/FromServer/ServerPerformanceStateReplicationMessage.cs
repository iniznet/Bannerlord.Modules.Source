using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C5 RID: 197
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ServerPerformanceStateReplicationMessage : GameNetworkMessage
	{
		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x0000E880 File Offset: 0x0000CA80
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x0000E888 File Offset: 0x0000CA88
		internal ServerPerformanceState ServerPerformanceProblemState { get; private set; }

		// Token: 0x0600081C RID: 2076 RVA: 0x0000E891 File Offset: 0x0000CA91
		public ServerPerformanceStateReplicationMessage()
		{
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0000E899 File Offset: 0x0000CA99
		internal ServerPerformanceStateReplicationMessage(ServerPerformanceState serverPerformanceProblemState)
		{
			this.ServerPerformanceProblemState = serverPerformanceProblemState;
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x0000E8A8 File Offset: 0x0000CAA8
		protected override bool OnRead()
		{
			bool flag = true;
			this.ServerPerformanceProblemState = (ServerPerformanceState)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ServerPerformanceStateCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0000E8CA File Offset: 0x0000CACA
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.ServerPerformanceProblemState, CompressionBasic.ServerPerformanceStateCompressionInfo);
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0000E8DC File Offset: 0x0000CADC
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0000E8E4 File Offset: 0x0000CAE4
		protected override string OnGetLogFormat()
		{
			return "ServerPerformanceStateReplicationMessage";
		}
	}
}
