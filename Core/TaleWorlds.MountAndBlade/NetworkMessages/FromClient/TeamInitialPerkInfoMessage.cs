using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200000E RID: 14
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class TeamInitialPerkInfoMessage : GameNetworkMessage
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600005C RID: 92 RVA: 0x000027AC File Offset: 0x000009AC
		// (set) Token: 0x0600005D RID: 93 RVA: 0x000027B4 File Offset: 0x000009B4
		public int[] Perks { get; private set; }

		// Token: 0x0600005E RID: 94 RVA: 0x000027BD File Offset: 0x000009BD
		public TeamInitialPerkInfoMessage(int[] perks)
		{
			this.Perks = perks;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000027CC File Offset: 0x000009CC
		public TeamInitialPerkInfoMessage()
		{
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000027D4 File Offset: 0x000009D4
		protected override bool OnRead()
		{
			bool flag = true;
			this.Perks = new int[3];
			for (int i = 0; i < 3; i++)
			{
				this.Perks[i] = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkIndexCompressionInfo, ref flag);
			}
			return flag;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002810 File Offset: 0x00000A10
		protected override void OnWrite()
		{
			for (int i = 0; i < 3; i++)
			{
				GameNetworkMessage.WriteIntToPacket(this.Perks[i], CompressionMission.PerkIndexCompressionInfo);
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000283B File Offset: 0x00000A3B
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Equipment;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002840 File Offset: 0x00000A40
		protected override string OnGetLogFormat()
		{
			return "TeamInitialPerkInfoMessage";
		}
	}
}
