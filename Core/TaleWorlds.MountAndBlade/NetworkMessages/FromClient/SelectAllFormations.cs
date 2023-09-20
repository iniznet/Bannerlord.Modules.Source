using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200002B RID: 43
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SelectAllFormations : GameNetworkMessage
	{
		// Token: 0x06000154 RID: 340 RVA: 0x0000396E File Offset: 0x00001B6E
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00003971 File Offset: 0x00001B71
		protected override void OnWrite()
		{
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00003973 File Offset: 0x00001B73
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00003978 File Offset: 0x00001B78
		protected override string OnGetLogFormat()
		{
			return "Select all formations";
		}
	}
}
