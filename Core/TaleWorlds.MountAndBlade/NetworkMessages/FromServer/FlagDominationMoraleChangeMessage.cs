using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200003A RID: 58
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagDominationMoraleChangeMessage : GameNetworkMessage
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x000042D4 File Offset: 0x000024D4
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x000042DC File Offset: 0x000024DC
		public float Morale { get; private set; }

		// Token: 0x060001D9 RID: 473 RVA: 0x000042E5 File Offset: 0x000024E5
		public FlagDominationMoraleChangeMessage()
		{
		}

		// Token: 0x060001DA RID: 474 RVA: 0x000042ED File Offset: 0x000024ED
		public FlagDominationMoraleChangeMessage(float morale)
		{
			this.Morale = morale;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x000042FC File Offset: 0x000024FC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteFloatToPacket(this.Morale, CompressionMission.FlagDominationMoraleCompressionInfo);
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00004310 File Offset: 0x00002510
		protected override bool OnRead()
		{
			bool flag = true;
			this.Morale = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagDominationMoraleCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00004332 File Offset: 0x00002532
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000433A File Offset: 0x0000253A
		protected override string OnGetLogFormat()
		{
			return "Morale synched: " + this.Morale;
		}
	}
}
