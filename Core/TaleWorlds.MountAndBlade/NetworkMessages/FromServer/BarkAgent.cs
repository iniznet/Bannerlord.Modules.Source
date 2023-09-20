using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200006B RID: 107
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BarkAgent : GameNetworkMessage
	{
		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x000077BC File Offset: 0x000059BC
		// (set) Token: 0x060003E5 RID: 997 RVA: 0x000077C4 File Offset: 0x000059C4
		public Agent Agent { get; private set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x000077CD File Offset: 0x000059CD
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x000077D5 File Offset: 0x000059D5
		public int IndexOfBark { get; private set; }

		// Token: 0x060003E8 RID: 1000 RVA: 0x000077DE File Offset: 0x000059DE
		public BarkAgent(Agent agent, int indexOfBark)
		{
			this.Agent = agent;
			this.IndexOfBark = indexOfBark;
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x000077F4 File Offset: 0x000059F4
		public BarkAgent()
		{
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x000077FC File Offset: 0x000059FC
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IndexOfBark = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BarkIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0000782C File Offset: 0x00005A2C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.IndexOfBark, CompressionMission.BarkIndexCompressionInfo);
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00007849 File Offset: 0x00005A49
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0000784D File Offset: 0x00005A4D
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"FromServer.BarkAgent: ",
				this.Agent.Index,
				", ",
				this.IndexOfBark
			});
		}
	}
}
