using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000089 RID: 137
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ReplaceBotWithPlayer : GameNetworkMessage
	{
		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x0000A4A7 File Offset: 0x000086A7
		// (set) Token: 0x0600057C RID: 1404 RVA: 0x0000A4AF File Offset: 0x000086AF
		public Agent BotAgent { get; private set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x0600057D RID: 1405 RVA: 0x0000A4B8 File Offset: 0x000086B8
		// (set) Token: 0x0600057E RID: 1406 RVA: 0x0000A4C0 File Offset: 0x000086C0
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x0600057F RID: 1407 RVA: 0x0000A4C9 File Offset: 0x000086C9
		// (set) Token: 0x06000580 RID: 1408 RVA: 0x0000A4D1 File Offset: 0x000086D1
		public int Health { get; private set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000581 RID: 1409 RVA: 0x0000A4DA File Offset: 0x000086DA
		// (set) Token: 0x06000582 RID: 1410 RVA: 0x0000A4E2 File Offset: 0x000086E2
		public int MountHealth { get; private set; }

		// Token: 0x06000583 RID: 1411 RVA: 0x0000A4EC File Offset: 0x000086EC
		public ReplaceBotWithPlayer(NetworkCommunicator peer, Agent botAgent)
		{
			this.Peer = peer;
			this.BotAgent = botAgent;
			this.Health = MathF.Ceiling(botAgent.Health);
			Agent mountAgent = this.BotAgent.MountAgent;
			this.MountHealth = MathF.Ceiling((mountAgent != null) ? mountAgent.Health : (-1f));
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x0000A544 File Offset: 0x00008744
		public ReplaceBotWithPlayer()
		{
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0000A54C File Offset: 0x0000874C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BotAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.Health = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			this.MountHealth = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x0000A59C File Offset: 0x0000879C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.BotAgent);
			GameNetworkMessage.WriteIntToPacket(this.Health, CompressionMission.AgentHealthCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.MountHealth, CompressionMission.AgentHealthCompressionInfo);
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x0000A5D4 File Offset: 0x000087D4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0000A5DC File Offset: 0x000087DC
		protected override string OnGetLogFormat()
		{
			return "Replace a bot with a player";
		}
	}
}
