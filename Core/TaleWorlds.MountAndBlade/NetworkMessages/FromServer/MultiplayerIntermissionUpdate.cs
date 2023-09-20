using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000053 RID: 83
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionUpdate : GameNetworkMessage
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060002ED RID: 749 RVA: 0x00005D78 File Offset: 0x00003F78
		// (set) Token: 0x060002EE RID: 750 RVA: 0x00005D80 File Offset: 0x00003F80
		public MultiplayerIntermissionState IntermissionState { get; private set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060002EF RID: 751 RVA: 0x00005D89 File Offset: 0x00003F89
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x00005D91 File Offset: 0x00003F91
		public float IntermissionTimer { get; private set; }

		// Token: 0x060002F1 RID: 753 RVA: 0x00005D9A File Offset: 0x00003F9A
		public MultiplayerIntermissionUpdate()
		{
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00005DA2 File Offset: 0x00003FA2
		public MultiplayerIntermissionUpdate(MultiplayerIntermissionState intermissionState, float intermissionTimer)
		{
			this.IntermissionState = intermissionState;
			this.IntermissionTimer = intermissionTimer;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00005DB8 File Offset: 0x00003FB8
		protected override bool OnRead()
		{
			bool flag = true;
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionStateCompressionInfo, ref flag);
			this.IntermissionState = (MultiplayerIntermissionState)num;
			this.IntermissionTimer = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.IntermissionTimerCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00005DEE File Offset: 0x00003FEE
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.IntermissionState, CompressionBasic.IntermissionStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.IntermissionTimer, CompressionBasic.IntermissionTimerCompressionInfo);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00005E10 File Offset: 0x00004010
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00005E18 File Offset: 0x00004018
		protected override string OnGetLogFormat()
		{
			return "Receiving runtime intermission state.";
		}
	}
}
