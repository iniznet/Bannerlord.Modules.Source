using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000022 RID: 34
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class CancelCheering : GameNetworkMessage
	{
		// Token: 0x06000114 RID: 276 RVA: 0x000035EF File Offset: 0x000017EF
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000035F2 File Offset: 0x000017F2
		protected override void OnWrite()
		{
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000035F4 File Offset: 0x000017F4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000035F8 File Offset: 0x000017F8
		protected override string OnGetLogFormat()
		{
			return "FromClient.CancelCheering";
		}
	}
}
