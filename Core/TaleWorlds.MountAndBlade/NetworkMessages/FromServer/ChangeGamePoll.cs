using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000048 RID: 72
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class ChangeGamePoll : GameNetworkMessage
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000271 RID: 625 RVA: 0x0000519D File Offset: 0x0000339D
		// (set) Token: 0x06000272 RID: 626 RVA: 0x000051A5 File Offset: 0x000033A5
		public string GameType { get; private set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000273 RID: 627 RVA: 0x000051AE File Offset: 0x000033AE
		// (set) Token: 0x06000274 RID: 628 RVA: 0x000051B6 File Offset: 0x000033B6
		public string Map { get; private set; }

		// Token: 0x06000275 RID: 629 RVA: 0x000051BF File Offset: 0x000033BF
		public ChangeGamePoll(string gameType, string map)
		{
			this.GameType = gameType;
			this.Map = map;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000051D5 File Offset: 0x000033D5
		public ChangeGamePoll()
		{
		}

		// Token: 0x06000277 RID: 631 RVA: 0x000051E0 File Offset: 0x000033E0
		protected override bool OnRead()
		{
			bool flag = true;
			this.GameType = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Map = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000520A File Offset: 0x0000340A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.GameType);
			GameNetworkMessage.WriteStringToPacket(this.Map);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00005222 File Offset: 0x00003422
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000522A File Offset: 0x0000342A
		protected override string OnGetLogFormat()
		{
			return "Poll started: Change Map to: " + this.Map + " and GameType to: " + this.GameType;
		}
	}
}
