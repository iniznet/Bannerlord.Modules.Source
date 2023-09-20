using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200007A RID: 122
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ExistingObjectsBegin : GameNetworkMessage
	{
		// Token: 0x060004D9 RID: 1241 RVA: 0x000095D2 File Offset: 0x000077D2
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x000095D5 File Offset: 0x000077D5
		protected override void OnWrite()
		{
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x000095D7 File Offset: 0x000077D7
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x000095DB File Offset: 0x000077DB
		protected override string OnGetLogFormat()
		{
			return "Started receiving existing objects";
		}
	}
}
