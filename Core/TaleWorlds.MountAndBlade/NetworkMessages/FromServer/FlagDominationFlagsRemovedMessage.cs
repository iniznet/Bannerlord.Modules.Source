using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000039 RID: 57
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagDominationFlagsRemovedMessage : GameNetworkMessage
	{
		// Token: 0x060001D3 RID: 467 RVA: 0x000042C0 File Offset: 0x000024C0
		protected override void OnWrite()
		{
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x000042C2 File Offset: 0x000024C2
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x000042C5 File Offset: 0x000024C5
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x000042CD File Offset: 0x000024CD
		protected override string OnGetLogFormat()
		{
			return "Flags got removed.";
		}
	}
}
