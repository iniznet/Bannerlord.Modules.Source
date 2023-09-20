using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200002C RID: 44
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SelectAllSiegeWeapons : GameNetworkMessage
	{
		// Token: 0x06000159 RID: 345 RVA: 0x00003987 File Offset: 0x00001B87
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000398A File Offset: 0x00001B8A
		protected override void OnWrite()
		{
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000398C File Offset: 0x00001B8C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00003994 File Offset: 0x00001B94
		protected override string OnGetLogFormat()
		{
			return "Select all siege weapons.";
		}
	}
}
