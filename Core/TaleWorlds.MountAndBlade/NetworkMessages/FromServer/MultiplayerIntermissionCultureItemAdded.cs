using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200004F RID: 79
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionCultureItemAdded : GameNetworkMessage
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x00005B0D File Offset: 0x00003D0D
		// (set) Token: 0x060002CA RID: 714 RVA: 0x00005B15 File Offset: 0x00003D15
		public string CultureId { get; private set; }

		// Token: 0x060002CB RID: 715 RVA: 0x00005B1E File Offset: 0x00003D1E
		public MultiplayerIntermissionCultureItemAdded()
		{
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00005B26 File Offset: 0x00003D26
		public MultiplayerIntermissionCultureItemAdded(string cultureId)
		{
			this.CultureId = cultureId;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00005B38 File Offset: 0x00003D38
		protected override bool OnRead()
		{
			bool flag = true;
			this.CultureId = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00005B55 File Offset: 0x00003D55
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.CultureId);
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00005B62 File Offset: 0x00003D62
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00005B6A File Offset: 0x00003D6A
		protected override string OnGetLogFormat()
		{
			return "Adding culture for voting with id: " + this.CultureId + ".";
		}
	}
}
