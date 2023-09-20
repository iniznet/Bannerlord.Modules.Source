using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200004E RID: 78
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MissionStateChange : GameNetworkMessage
	{
		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060002BF RID: 703 RVA: 0x00005A41 File Offset: 0x00003C41
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x00005A49 File Offset: 0x00003C49
		public MissionLobbyComponent.MultiplayerGameState CurrentState { get; private set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x00005A52 File Offset: 0x00003C52
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x00005A5A File Offset: 0x00003C5A
		public float StateStartTimeInSeconds { get; private set; }

		// Token: 0x060002C3 RID: 707 RVA: 0x00005A63 File Offset: 0x00003C63
		public MissionStateChange(MissionLobbyComponent.MultiplayerGameState currentState, long stateStartTimeInTicks)
		{
			this.CurrentState = currentState;
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00005A80 File Offset: 0x00003C80
		public MissionStateChange()
		{
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00005A88 File Offset: 0x00003C88
		protected override bool OnRead()
		{
			bool flag = true;
			this.CurrentState = (MissionLobbyComponent.MultiplayerGameState)GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.MissionCurrentStateCompressionInfo, ref flag);
			if (this.CurrentState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			}
			return flag;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00005AC4 File Offset: 0x00003CC4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.CurrentState, CompressionMatchmaker.MissionCurrentStateCompressionInfo);
			if (this.CurrentState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00005AEE File Offset: 0x00003CEE
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00005AF6 File Offset: 0x00003CF6
		protected override string OnGetLogFormat()
		{
			return "Mission State has changed to: " + this.CurrentState;
		}
	}
}
