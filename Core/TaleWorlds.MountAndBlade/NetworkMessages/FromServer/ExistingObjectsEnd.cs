using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200007B RID: 123
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ExistingObjectsEnd : GameNetworkMessage
	{
		// Token: 0x060004DE RID: 1246 RVA: 0x000095EA File Offset: 0x000077EA
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x000095ED File Offset: 0x000077ED
		protected override void OnWrite()
		{
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x000095EF File Offset: 0x000077EF
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x000095F3 File Offset: 0x000077F3
		protected override string OnGetLogFormat()
		{
			return "Finished receiving existing objects";
		}
	}
}
