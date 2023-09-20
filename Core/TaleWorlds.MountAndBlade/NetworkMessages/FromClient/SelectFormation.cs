using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200002D RID: 45
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SelectFormation : GameNetworkMessage
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600015D RID: 349 RVA: 0x0000399B File Offset: 0x00001B9B
		// (set) Token: 0x0600015E RID: 350 RVA: 0x000039A3 File Offset: 0x00001BA3
		public int FormationIndex { get; private set; }

		// Token: 0x0600015F RID: 351 RVA: 0x000039AC File Offset: 0x00001BAC
		public SelectFormation(int formationIndex)
		{
			this.FormationIndex = formationIndex;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000039BB File Offset: 0x00001BBB
		public SelectFormation()
		{
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000039C4 File Offset: 0x00001BC4
		protected override bool OnRead()
		{
			bool flag = true;
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000039E6 File Offset: 0x00001BE6
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000039F8 File Offset: 0x00001BF8
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000039FD File Offset: 0x00001BFD
		protected override string OnGetLogFormat()
		{
			return "Select Formation with ID: " + this.FormationIndex;
		}
	}
}
