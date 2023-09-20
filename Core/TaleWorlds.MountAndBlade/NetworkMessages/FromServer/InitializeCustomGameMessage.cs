using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200007F RID: 127
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class InitializeCustomGameMessage : GameNetworkMessage
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x00009BC0 File Offset: 0x00007DC0
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x00009BC8 File Offset: 0x00007DC8
		public bool InMission { get; private set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x00009BD1 File Offset: 0x00007DD1
		// (set) Token: 0x06000512 RID: 1298 RVA: 0x00009BD9 File Offset: 0x00007DD9
		public string GameType { get; private set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x00009BE2 File Offset: 0x00007DE2
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x00009BEA File Offset: 0x00007DEA
		public string Map { get; private set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x00009BF3 File Offset: 0x00007DF3
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x00009BFB File Offset: 0x00007DFB
		public int BattleIndex { get; private set; }

		// Token: 0x06000517 RID: 1303 RVA: 0x00009C04 File Offset: 0x00007E04
		public InitializeCustomGameMessage(bool inMission, string gameType, string map, int battleIndex)
		{
			this.InMission = inMission;
			this.GameType = gameType;
			this.Map = map;
			this.BattleIndex = battleIndex;
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x00009C29 File Offset: 0x00007E29
		public InitializeCustomGameMessage()
		{
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00009C34 File Offset: 0x00007E34
		protected override bool OnRead()
		{
			bool flag = true;
			this.InMission = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.GameType = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Map = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.BattleIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AutomatedBattleIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00009C7D File Offset: 0x00007E7D
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.InMission);
			GameNetworkMessage.WriteStringToPacket(this.GameType);
			GameNetworkMessage.WriteStringToPacket(this.Map);
			GameNetworkMessage.WriteIntToPacket(this.BattleIndex, CompressionMission.AutomatedBattleIndexCompressionInfo);
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x00009CB0 File Offset: 0x00007EB0
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00009CB8 File Offset: 0x00007EB8
		protected override string OnGetLogFormat()
		{
			return "Initialize Custom Game";
		}
	}
}
