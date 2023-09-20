using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A9 RID: 169
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSpawnedFormationCount : GameNetworkMessage
	{
		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x0000C956 File Offset: 0x0000AB56
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x0000C95E File Offset: 0x0000AB5E
		public int NumOfFormationsTeamOne { get; private set; }

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x0000C967 File Offset: 0x0000AB67
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x0000C96F File Offset: 0x0000AB6F
		public int NumOfFormationsTeamTwo { get; private set; }

		// Token: 0x060006DE RID: 1758 RVA: 0x0000C978 File Offset: 0x0000AB78
		public SetSpawnedFormationCount(int numFormationsTeamOne, int numFormationsTeamTwo)
		{
			this.NumOfFormationsTeamOne = numFormationsTeamOne;
			this.NumOfFormationsTeamTwo = numFormationsTeamTwo;
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x0000C98E File Offset: 0x0000AB8E
		public SetSpawnedFormationCount()
		{
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x0000C998 File Offset: 0x0000AB98
		protected override bool OnRead()
		{
			bool flag = true;
			this.NumOfFormationsTeamOne = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			this.NumOfFormationsTeamTwo = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x0000C9CC File Offset: 0x0000ABCC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.NumOfFormationsTeamOne, CompressionOrder.FormationClassCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.NumOfFormationsTeamTwo, CompressionOrder.FormationClassCompressionInfo);
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x0000C9EE File Offset: 0x0000ABEE
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x0000C9F2 File Offset: 0x0000ABF2
		protected override string OnGetLogFormat()
		{
			return "Syncing formation count";
		}
	}
}
