using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200005C RID: 92
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundCountChange : GameNetworkMessage
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000336 RID: 822 RVA: 0x000065FA File Offset: 0x000047FA
		// (set) Token: 0x06000337 RID: 823 RVA: 0x00006602 File Offset: 0x00004802
		public int RoundCount { get; private set; }

		// Token: 0x06000338 RID: 824 RVA: 0x0000660B File Offset: 0x0000480B
		public RoundCountChange(int roundCount)
		{
			this.RoundCount = roundCount;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000661A File Offset: 0x0000481A
		public RoundCountChange()
		{
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00006624 File Offset: 0x00004824
		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundCountCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00006646 File Offset: 0x00004846
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.RoundCount, CompressionMission.MissionRoundCountCompressionInfo);
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00006658 File Offset: 0x00004858
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00006660 File Offset: 0x00004860
		protected override string OnGetLogFormat()
		{
			return "Change round count to: " + this.RoundCount;
		}
	}
}
