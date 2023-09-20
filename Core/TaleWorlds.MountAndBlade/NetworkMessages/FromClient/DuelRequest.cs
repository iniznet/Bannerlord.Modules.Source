using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000008 RID: 8
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class DuelRequest : GameNetworkMessage
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002425 File Offset: 0x00000625
		// (set) Token: 0x06000029 RID: 41 RVA: 0x0000242D File Offset: 0x0000062D
		public Agent RequestedAgent { get; private set; }

		// Token: 0x0600002A RID: 42 RVA: 0x00002436 File Offset: 0x00000636
		public DuelRequest(Agent requestedAgent)
		{
			this.RequestedAgent = requestedAgent;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002445 File Offset: 0x00000645
		public DuelRequest()
		{
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002450 File Offset: 0x00000650
		protected override bool OnRead()
		{
			bool flag = true;
			this.RequestedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000246E File Offset: 0x0000066E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.RequestedAgent);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000247B File Offset: 0x0000067B
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002483 File Offset: 0x00000683
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Duel requested from agent with name: ",
				this.RequestedAgent.Name,
				" and index: ",
				this.RequestedAgent.Index
			});
		}
	}
}
