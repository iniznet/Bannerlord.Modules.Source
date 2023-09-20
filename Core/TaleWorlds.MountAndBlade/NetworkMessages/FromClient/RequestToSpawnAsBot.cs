using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000029 RID: 41
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestToSpawnAsBot : GameNetworkMessage
	{
		// Token: 0x06000145 RID: 325 RVA: 0x00003867 File Offset: 0x00001A67
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000386A File Offset: 0x00001A6A
		protected override void OnWrite()
		{
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000386C File Offset: 0x00001A6C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00003874 File Offset: 0x00001A74
		protected override string OnGetLogFormat()
		{
			return "Request to spawn as a bot";
		}
	}
}
