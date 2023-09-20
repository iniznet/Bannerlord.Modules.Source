using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000082 RID: 130
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MakeAgentDead : GameNetworkMessage
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000535 RID: 1333 RVA: 0x00009E9F File Offset: 0x0000809F
		// (set) Token: 0x06000536 RID: 1334 RVA: 0x00009EA7 File Offset: 0x000080A7
		public Agent Agent { get; private set; }

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000537 RID: 1335 RVA: 0x00009EB0 File Offset: 0x000080B0
		// (set) Token: 0x06000538 RID: 1336 RVA: 0x00009EB8 File Offset: 0x000080B8
		public bool IsKilled { get; private set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000539 RID: 1337 RVA: 0x00009EC1 File Offset: 0x000080C1
		// (set) Token: 0x0600053A RID: 1338 RVA: 0x00009EC9 File Offset: 0x000080C9
		public ActionIndexValueCache ActionCodeIndex { get; private set; }

		// Token: 0x0600053B RID: 1339 RVA: 0x00009ED2 File Offset: 0x000080D2
		public MakeAgentDead(Agent agent, bool isKilled, ActionIndexValueCache actionCodeIndex)
		{
			this.Agent = agent;
			this.IsKilled = isKilled;
			this.ActionCodeIndex = actionCodeIndex;
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x00009EEF File Offset: 0x000080EF
		public MakeAgentDead()
		{
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x00009EF8 File Offset: 0x000080F8
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsKilled = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.ActionCodeIndex = new ActionIndexValueCache(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ActionCodeCompressionInfo, ref flag));
			return flag;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x00009F3C File Offset: 0x0000813C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteBoolToPacket(this.IsKilled);
			GameNetworkMessage.WriteIntToPacket(this.ActionCodeIndex.Index, CompressionBasic.ActionCodeCompressionInfo);
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00009F77 File Offset: 0x00008177
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00009F7C File Offset: 0x0000817C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Make Agent Dead on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
