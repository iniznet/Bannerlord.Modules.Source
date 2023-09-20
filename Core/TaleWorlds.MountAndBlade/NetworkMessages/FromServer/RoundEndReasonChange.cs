using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200005D RID: 93
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundEndReasonChange : GameNetworkMessage
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600033E RID: 830 RVA: 0x00006677 File Offset: 0x00004877
		// (set) Token: 0x0600033F RID: 831 RVA: 0x0000667F File Offset: 0x0000487F
		public RoundEndReason RoundEndReason { get; private set; }

		// Token: 0x06000340 RID: 832 RVA: 0x00006688 File Offset: 0x00004888
		public RoundEndReasonChange()
		{
			this.RoundEndReason = RoundEndReason.Invalid;
		}

		// Token: 0x06000341 RID: 833 RVA: 0x00006697 File Offset: 0x00004897
		public RoundEndReasonChange(RoundEndReason roundEndReason)
		{
			this.RoundEndReason = roundEndReason;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x000066A6 File Offset: 0x000048A6
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.RoundEndReason, CompressionMission.RoundEndReasonCompressionInfo);
		}

		// Token: 0x06000343 RID: 835 RVA: 0x000066B8 File Offset: 0x000048B8
		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundEndReason = (RoundEndReason)GameNetworkMessage.ReadIntFromPacket(CompressionMission.RoundEndReasonCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000344 RID: 836 RVA: 0x000066DA File Offset: 0x000048DA
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x000066E4 File Offset: 0x000048E4
		protected override string OnGetLogFormat()
		{
			return "Change round end reason to: " + this.RoundEndReason.ToString();
		}
	}
}
