using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200005F RID: 95
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundWinnerChange : GameNetworkMessage
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000352 RID: 850 RVA: 0x00006807 File Offset: 0x00004A07
		// (set) Token: 0x06000353 RID: 851 RVA: 0x0000680F File Offset: 0x00004A0F
		public BattleSideEnum RoundWinner { get; private set; }

		// Token: 0x06000354 RID: 852 RVA: 0x00006818 File Offset: 0x00004A18
		public RoundWinnerChange(BattleSideEnum roundWinner)
		{
			this.RoundWinner = roundWinner;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00006827 File Offset: 0x00004A27
		public RoundWinnerChange()
		{
			this.RoundWinner = BattleSideEnum.None;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00006838 File Offset: 0x00004A38
		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundWinner = (BattleSideEnum)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0000685A File Offset: 0x00004A5A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.RoundWinner, CompressionMission.TeamSideCompressionInfo);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0000686C File Offset: 0x00004A6C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00006874 File Offset: 0x00004A74
		protected override string OnGetLogFormat()
		{
			return "Change round winner to: " + this.RoundWinner.ToString();
		}
	}
}
