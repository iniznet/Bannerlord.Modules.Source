using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200000F RID: 15
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ChangeGamePoll : GameNetworkMessage
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002847 File Offset: 0x00000A47
		// (set) Token: 0x06000065 RID: 101 RVA: 0x0000284F File Offset: 0x00000A4F
		public string GameType { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002858 File Offset: 0x00000A58
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00002860 File Offset: 0x00000A60
		public string Map { get; private set; }

		// Token: 0x06000068 RID: 104 RVA: 0x00002869 File Offset: 0x00000A69
		public ChangeGamePoll(string gameType, string map)
		{
			this.GameType = gameType;
			this.Map = map;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000287F File Offset: 0x00000A7F
		public ChangeGamePoll()
		{
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002888 File Offset: 0x00000A88
		protected override bool OnRead()
		{
			bool flag = true;
			this.GameType = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Map = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000028B2 File Offset: 0x00000AB2
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.GameType);
			GameNetworkMessage.WriteStringToPacket(this.Map);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000028CA File Offset: 0x00000ACA
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000028D2 File Offset: 0x00000AD2
		protected override string OnGetLogFormat()
		{
			return "Poll Requested: Change Map to: " + this.Map + " and GameType to: " + this.GameType;
		}
	}
}
