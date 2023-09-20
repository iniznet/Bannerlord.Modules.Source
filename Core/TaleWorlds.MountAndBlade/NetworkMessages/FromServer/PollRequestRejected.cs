using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000058 RID: 88
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PollRequestRejected : GameNetworkMessage
	{
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000312 RID: 786 RVA: 0x000062D9 File Offset: 0x000044D9
		// (set) Token: 0x06000313 RID: 787 RVA: 0x000062E1 File Offset: 0x000044E1
		public int Reason { get; private set; }

		// Token: 0x06000314 RID: 788 RVA: 0x000062EA File Offset: 0x000044EA
		public PollRequestRejected(int reason)
		{
			this.Reason = reason;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x000062F9 File Offset: 0x000044F9
		public PollRequestRejected()
		{
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00006304 File Offset: 0x00004504
		protected override bool OnRead()
		{
			bool flag = true;
			this.Reason = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerPollRejectReasonCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00006326 File Offset: 0x00004526
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.Reason, CompressionMission.MultiplayerPollRejectReasonCompressionInfo);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00006338 File Offset: 0x00004538
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00006340 File Offset: 0x00004540
		protected override string OnGetLogFormat()
		{
			return "Poll request rejected (" + (MultiplayerPollRejectReason)this.Reason + ")";
		}
	}
}
