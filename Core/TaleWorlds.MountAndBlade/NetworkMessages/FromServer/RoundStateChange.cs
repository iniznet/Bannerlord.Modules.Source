using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200005E RID: 94
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundStateChange : GameNetworkMessage
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000346 RID: 838 RVA: 0x0000670F File Offset: 0x0000490F
		// (set) Token: 0x06000347 RID: 839 RVA: 0x00006717 File Offset: 0x00004917
		public MultiplayerRoundState RoundState { get; private set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000348 RID: 840 RVA: 0x00006720 File Offset: 0x00004920
		// (set) Token: 0x06000349 RID: 841 RVA: 0x00006728 File Offset: 0x00004928
		public float StateStartTimeInSeconds { get; private set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600034A RID: 842 RVA: 0x00006731 File Offset: 0x00004931
		// (set) Token: 0x0600034B RID: 843 RVA: 0x00006739 File Offset: 0x00004939
		public int RemainingTimeOnPreviousState { get; private set; }

		// Token: 0x0600034C RID: 844 RVA: 0x00006742 File Offset: 0x00004942
		public RoundStateChange(MultiplayerRoundState roundState, long stateStartTimeInTicks, int remainingTimeOnPreviousState)
		{
			this.RoundState = roundState;
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
			this.RemainingTimeOnPreviousState = remainingTimeOnPreviousState;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00006766 File Offset: 0x00004966
		public RoundStateChange()
		{
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00006770 File Offset: 0x00004970
		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundState = (MultiplayerRoundState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundStateCompressionInfo, ref flag);
			this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			this.RemainingTimeOnPreviousState = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RoundTimeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000067B6 File Offset: 0x000049B6
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.RoundState, CompressionMission.MissionRoundStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.RemainingTimeOnPreviousState, CompressionMission.RoundTimeCompressionInfo);
		}

		// Token: 0x06000350 RID: 848 RVA: 0x000067E8 File Offset: 0x000049E8
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x06000351 RID: 849 RVA: 0x000067F0 File Offset: 0x000049F0
		protected override string OnGetLogFormat()
		{
			return "Changing round state to: " + this.RoundState;
		}
	}
}
