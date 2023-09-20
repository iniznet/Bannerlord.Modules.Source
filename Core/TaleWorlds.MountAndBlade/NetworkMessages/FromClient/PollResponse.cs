using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000013 RID: 19
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class PollResponse : GameNetworkMessage
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00002B26 File Offset: 0x00000D26
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00002B2E File Offset: 0x00000D2E
		public bool Accepted { get; private set; }

		// Token: 0x0600008E RID: 142 RVA: 0x00002B37 File Offset: 0x00000D37
		public PollResponse(bool accepted)
		{
			this.Accepted = accepted;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00002B46 File Offset: 0x00000D46
		public PollResponse()
		{
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00002B50 File Offset: 0x00000D50
		protected override bool OnRead()
		{
			bool flag = true;
			this.Accepted = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00002B6D File Offset: 0x00000D6D
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.Accepted);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00002B7A File Offset: 0x00000D7A
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00002B82 File Offset: 0x00000D82
		protected override string OnGetLogFormat()
		{
			return "Receiving poll response: " + (this.Accepted ? "Accepted." : "Not accepted.");
		}
	}
}
