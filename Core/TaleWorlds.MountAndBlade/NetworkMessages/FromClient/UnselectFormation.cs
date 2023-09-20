using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000032 RID: 50
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class UnselectFormation : GameNetworkMessage
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00003D01 File Offset: 0x00001F01
		// (set) Token: 0x0600018C RID: 396 RVA: 0x00003D09 File Offset: 0x00001F09
		public int FormationIndex { get; private set; }

		// Token: 0x0600018D RID: 397 RVA: 0x00003D12 File Offset: 0x00001F12
		public UnselectFormation(int formationIndex)
		{
			this.FormationIndex = formationIndex;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00003D21 File Offset: 0x00001F21
		public UnselectFormation()
		{
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00003D2C File Offset: 0x00001F2C
		protected override bool OnRead()
		{
			bool flag = true;
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00003D4E File Offset: 0x00001F4E
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00003D60 File Offset: 0x00001F60
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00003D65 File Offset: 0x00001F65
		protected override string OnGetLogFormat()
		{
			return "Deselect Formation with index: " + this.FormationIndex;
		}
	}
}
