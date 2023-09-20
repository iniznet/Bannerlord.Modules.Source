using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200008C RID: 140
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentIsPlayer : GameNetworkMessage
	{
		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x0000A91E File Offset: 0x00008B1E
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x0000A926 File Offset: 0x00008B26
		public Agent Agent { get; private set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x0000A92F File Offset: 0x00008B2F
		// (set) Token: 0x060005AA RID: 1450 RVA: 0x0000A937 File Offset: 0x00008B37
		public bool IsPlayer { get; private set; }

		// Token: 0x060005AB RID: 1451 RVA: 0x0000A940 File Offset: 0x00008B40
		public SetAgentIsPlayer(Agent agent, bool isPlayer)
		{
			this.Agent = agent;
			this.IsPlayer = isPlayer;
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x0000A956 File Offset: 0x00008B56
		public SetAgentIsPlayer()
		{
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0000A960 File Offset: 0x00008B60
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x0000A98B File Offset: 0x00008B8B
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayer);
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0000A9A3 File Offset: 0x00008BA3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0000A9AC File Offset: 0x00008BAC
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Controller is player on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index,
				this.IsPlayer ? " - TRUE." : " - FALSE."
			});
		}
	}
}
