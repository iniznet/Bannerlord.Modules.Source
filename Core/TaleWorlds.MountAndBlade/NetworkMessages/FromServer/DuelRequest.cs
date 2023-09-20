using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000040 RID: 64
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelRequest : GameNetworkMessage
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00004916 File Offset: 0x00002B16
		// (set) Token: 0x0600021A RID: 538 RVA: 0x0000491E File Offset: 0x00002B1E
		public Agent RequesterAgent { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600021B RID: 539 RVA: 0x00004927 File Offset: 0x00002B27
		// (set) Token: 0x0600021C RID: 540 RVA: 0x0000492F File Offset: 0x00002B2F
		public Agent RequestedAgent { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600021D RID: 541 RVA: 0x00004938 File Offset: 0x00002B38
		// (set) Token: 0x0600021E RID: 542 RVA: 0x00004940 File Offset: 0x00002B40
		public TroopType SelectedAreaTroopType { get; private set; }

		// Token: 0x0600021F RID: 543 RVA: 0x00004949 File Offset: 0x00002B49
		public DuelRequest(Agent requesterAgent, Agent requestedAgent, TroopType selectedAreaTroopType)
		{
			this.RequesterAgent = requesterAgent;
			this.RequestedAgent = requestedAgent;
			this.SelectedAreaTroopType = selectedAreaTroopType;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00004966 File Offset: 0x00002B66
		public DuelRequest()
		{
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00004970 File Offset: 0x00002B70
		protected override bool OnRead()
		{
			bool flag = true;
			this.RequesterAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.RequestedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SelectedAreaTroopType = (TroopType)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.TroopTypeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x000049AE File Offset: 0x00002BAE
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.RequesterAgent);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.RequestedAgent);
			GameNetworkMessage.WriteIntToPacket((int)this.SelectedAreaTroopType, CompressionBasic.TroopTypeCompressionInfo);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x000049D6 File Offset: 0x00002BD6
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x000049DE File Offset: 0x00002BDE
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Request duel from agent with name: ",
				this.RequestedAgent.Name,
				" and index: ",
				this.RequestedAgent.Index
			});
		}
	}
}
