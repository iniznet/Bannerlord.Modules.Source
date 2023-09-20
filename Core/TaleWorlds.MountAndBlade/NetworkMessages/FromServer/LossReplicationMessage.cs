using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C2 RID: 194
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class LossReplicationMessage : GameNetworkMessage
	{
		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000808 RID: 2056 RVA: 0x0000E760 File Offset: 0x0000C960
		// (set) Token: 0x06000809 RID: 2057 RVA: 0x0000E768 File Offset: 0x0000C968
		internal int LossValue { get; private set; }

		// Token: 0x0600080A RID: 2058 RVA: 0x0000E771 File Offset: 0x0000C971
		public LossReplicationMessage()
		{
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0000E779 File Offset: 0x0000C979
		internal LossReplicationMessage(int lossValue)
		{
			this.LossValue = lossValue;
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0000E788 File Offset: 0x0000C988
		protected override bool OnRead()
		{
			bool flag = true;
			this.LossValue = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.LossValueCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0000E7AA File Offset: 0x0000C9AA
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.LossValue, CompressionBasic.LossValueCompressionInfo);
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0000E7BC File Offset: 0x0000C9BC
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0000E7C4 File Offset: 0x0000C9C4
		protected override string OnGetLogFormat()
		{
			return "LossReplicationMessage";
		}
	}
}
