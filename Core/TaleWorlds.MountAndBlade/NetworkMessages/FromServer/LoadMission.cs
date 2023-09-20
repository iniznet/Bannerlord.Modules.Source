using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000081 RID: 129
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class LoadMission : GameNetworkMessage
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x00009DD2 File Offset: 0x00007FD2
		// (set) Token: 0x0600052A RID: 1322 RVA: 0x00009DDA File Offset: 0x00007FDA
		public string GameType { get; private set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600052B RID: 1323 RVA: 0x00009DE3 File Offset: 0x00007FE3
		// (set) Token: 0x0600052C RID: 1324 RVA: 0x00009DEB File Offset: 0x00007FEB
		public string Map { get; private set; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600052D RID: 1325 RVA: 0x00009DF4 File Offset: 0x00007FF4
		// (set) Token: 0x0600052E RID: 1326 RVA: 0x00009DFC File Offset: 0x00007FFC
		public int BattleIndex { get; private set; }

		// Token: 0x0600052F RID: 1327 RVA: 0x00009E05 File Offset: 0x00008005
		public LoadMission(string gameType, string map, int battleIndex)
		{
			this.GameType = gameType;
			this.Map = map;
			this.BattleIndex = battleIndex;
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x00009E22 File Offset: 0x00008022
		public LoadMission()
		{
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x00009E2C File Offset: 0x0000802C
		protected override bool OnRead()
		{
			bool flag = true;
			this.GameType = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Map = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.BattleIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AutomatedBattleIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00009E68 File Offset: 0x00008068
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.GameType);
			GameNetworkMessage.WriteStringToPacket(this.Map);
			GameNetworkMessage.WriteIntToPacket(this.BattleIndex, CompressionMission.AutomatedBattleIndexCompressionInfo);
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00009E90 File Offset: 0x00008090
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00009E98 File Offset: 0x00008098
		protected override string OnGetLogFormat()
		{
			return "Load Mission";
		}
	}
}
