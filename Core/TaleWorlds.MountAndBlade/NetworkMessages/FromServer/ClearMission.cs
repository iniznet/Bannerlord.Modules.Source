using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200006F RID: 111
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ClearMission : GameNetworkMessage
	{
		// Token: 0x06000409 RID: 1033 RVA: 0x00007AB5 File Offset: 0x00005CB5
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00007AB8 File Offset: 0x00005CB8
		protected override void OnWrite()
		{
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00007ABA File Offset: 0x00005CBA
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00007AC2 File Offset: 0x00005CC2
		protected override string OnGetLogFormat()
		{
			return "Clear Mission";
		}
	}
}
