using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000056 RID: 86
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PollCancelled : GameNetworkMessage
	{
		// Token: 0x06000304 RID: 772 RVA: 0x0000621F File Offset: 0x0000441F
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00006222 File Offset: 0x00004422
		protected override void OnWrite()
		{
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00006224 File Offset: 0x00004424
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0000622C File Offset: 0x0000442C
		protected override string OnGetLogFormat()
		{
			return "Poll cancelled.";
		}
	}
}
