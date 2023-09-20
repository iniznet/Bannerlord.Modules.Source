using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000024 RID: 36
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ClearSelectedFormations : GameNetworkMessage
	{
		// Token: 0x06000121 RID: 289 RVA: 0x0000367F File Offset: 0x0000187F
		protected override bool OnRead()
		{
			return true;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00003682 File Offset: 0x00001882
		protected override void OnWrite()
		{
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00003684 File Offset: 0x00001884
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Formations;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00003689 File Offset: 0x00001889
		protected override string OnGetLogFormat()
		{
			return "Clear Selected Formations";
		}
	}
}
