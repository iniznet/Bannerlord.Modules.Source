using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200007D RID: 125
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FormationWipedMessage : GameNetworkMessage
	{
		// Token: 0x060004ED RID: 1261 RVA: 0x0000969E File Offset: 0x0000789E
		protected override void OnWrite()
		{
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x000096A0 File Offset: 0x000078A0
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x000096A3 File Offset: 0x000078A3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x000096AB File Offset: 0x000078AB
		protected override string OnGetLogFormat()
		{
			return "FormationWipedMessage";
		}
	}
}
