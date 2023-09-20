using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000021 RID: 33
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class BarkSelected : GameNetworkMessage
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600010B RID: 267 RVA: 0x0000356F File Offset: 0x0000176F
		// (set) Token: 0x0600010C RID: 268 RVA: 0x00003577 File Offset: 0x00001777
		public int IndexOfBark { get; private set; }

		// Token: 0x0600010D RID: 269 RVA: 0x00003580 File Offset: 0x00001780
		public BarkSelected(int indexOfBark)
		{
			this.IndexOfBark = indexOfBark;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000358F File Offset: 0x0000178F
		public BarkSelected()
		{
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00003598 File Offset: 0x00001798
		protected override bool OnRead()
		{
			bool flag = true;
			this.IndexOfBark = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BarkIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000035BA File Offset: 0x000017BA
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.IndexOfBark, CompressionMission.BarkIndexCompressionInfo);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000035CC File Offset: 0x000017CC
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000035D0 File Offset: 0x000017D0
		protected override string OnGetLogFormat()
		{
			return "FromClient.BarkSelected: " + this.IndexOfBark;
		}
	}
}
