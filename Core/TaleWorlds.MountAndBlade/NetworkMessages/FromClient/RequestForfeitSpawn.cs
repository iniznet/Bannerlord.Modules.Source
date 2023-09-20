using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000015 RID: 21
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestForfeitSpawn : GameNetworkMessage
	{
		// Token: 0x0600009D RID: 157 RVA: 0x00002C2F File Offset: 0x00000E2F
		protected override void OnWrite()
		{
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00002C31 File Offset: 0x00000E31
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00002C34 File Offset: 0x00000E34
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00002C3C File Offset: 0x00000E3C
		protected override string OnGetLogFormat()
		{
			return "Someone has requested to forfeit spawning.";
		}
	}
}
