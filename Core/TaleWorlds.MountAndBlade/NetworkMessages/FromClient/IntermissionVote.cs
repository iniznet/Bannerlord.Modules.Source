using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000010 RID: 16
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class IntermissionVote : GameNetworkMessage
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000028EF File Offset: 0x00000AEF
		// (set) Token: 0x0600006F RID: 111 RVA: 0x000028F7 File Offset: 0x00000AF7
		public int VoteCount { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00002900 File Offset: 0x00000B00
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00002908 File Offset: 0x00000B08
		public string ItemID { get; private set; }

		// Token: 0x06000072 RID: 114 RVA: 0x00002911 File Offset: 0x00000B11
		public IntermissionVote(string itemID, int voteCount)
		{
			this.VoteCount = voteCount;
			this.ItemID = itemID;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002927 File Offset: 0x00000B27
		public IntermissionVote()
		{
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002930 File Offset: 0x00000B30
		protected override bool OnRead()
		{
			bool flag = true;
			this.ItemID = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.VoteCount = GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(-1, 1, true), ref flag);
			return flag;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002962 File Offset: 0x00000B62
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.ItemID);
			GameNetworkMessage.WriteIntToPacket(this.VoteCount, new CompressionInfo.Integer(-1, 1, true));
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00002982 File Offset: 0x00000B82
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000298A File Offset: 0x00000B8A
		protected override string OnGetLogFormat()
		{
			return string.Format("Intermission vote casted for item with ID: {0} with count: {1}.", this.ItemID, this.VoteCount);
		}
	}
}
