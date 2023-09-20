using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000028 RID: 40
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestStopUsingObject : GameNetworkMessage
	{
		// Token: 0x06000140 RID: 320 RVA: 0x0000384B File Offset: 0x00001A4B
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000384E File Offset: 0x00001A4E
		protected override void OnWrite()
		{
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00003850 File Offset: 0x00001A50
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00003858 File Offset: 0x00001A58
		protected override string OnGetLogFormat()
		{
			return "Request to stop using UsableMissionObject";
		}
	}
}
