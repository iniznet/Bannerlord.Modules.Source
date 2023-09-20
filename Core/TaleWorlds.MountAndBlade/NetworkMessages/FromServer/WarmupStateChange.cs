using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000BF RID: 191
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class WarmupStateChange : GameNetworkMessage
	{
		// Token: 0x170001CA RID: 458
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x0000E4A8 File Offset: 0x0000C6A8
		// (set) Token: 0x060007E9 RID: 2025 RVA: 0x0000E4B0 File Offset: 0x0000C6B0
		public MultiplayerWarmupComponent.WarmupStates WarmupState { get; private set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x0000E4B9 File Offset: 0x0000C6B9
		// (set) Token: 0x060007EB RID: 2027 RVA: 0x0000E4C1 File Offset: 0x0000C6C1
		public float StateStartTimeInSeconds { get; private set; }

		// Token: 0x060007EC RID: 2028 RVA: 0x0000E4CA File Offset: 0x0000C6CA
		public WarmupStateChange(MultiplayerWarmupComponent.WarmupStates warmupState, long stateStartTimeInTicks)
		{
			this.WarmupState = warmupState;
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0000E4E7 File Offset: 0x0000C6E7
		public WarmupStateChange()
		{
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0000E4EF File Offset: 0x0000C6EF
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.WarmupState, CompressionMission.MissionRoundStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0000E514 File Offset: 0x0000C714
		protected override bool OnRead()
		{
			bool flag = true;
			this.WarmupState = (MultiplayerWarmupComponent.WarmupStates)GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundStateCompressionInfo, ref flag);
			this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0000E548 File Offset: 0x0000C748
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0000E550 File Offset: 0x0000C750
		protected override string OnGetLogFormat()
		{
			return "Warmup state set to " + this.WarmupState;
		}
	}
}
