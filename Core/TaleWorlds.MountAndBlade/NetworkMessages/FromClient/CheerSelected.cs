using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000023 RID: 35
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class CheerSelected : GameNetworkMessage
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000118 RID: 280 RVA: 0x000035FF File Offset: 0x000017FF
		// (set) Token: 0x06000119 RID: 281 RVA: 0x00003607 File Offset: 0x00001807
		public int IndexOfCheer { get; private set; }

		// Token: 0x0600011A RID: 282 RVA: 0x00003610 File Offset: 0x00001810
		public CheerSelected(int indexOfCheer)
		{
			this.IndexOfCheer = indexOfCheer;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000361F File Offset: 0x0000181F
		public CheerSelected()
		{
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00003628 File Offset: 0x00001828
		protected override bool OnRead()
		{
			bool flag = true;
			this.IndexOfCheer = GameNetworkMessage.ReadIntFromPacket(CompressionMission.CheerIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000364A File Offset: 0x0000184A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.IndexOfCheer, CompressionMission.CheerIndexCompressionInfo);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000365C File Offset: 0x0000185C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00003660 File Offset: 0x00001860
		protected override string OnGetLogFormat()
		{
			return "FromClient.CheerSelected: " + this.IndexOfCheer;
		}
	}
}
