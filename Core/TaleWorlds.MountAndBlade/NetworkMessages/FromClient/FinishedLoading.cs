using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000027 RID: 39
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class FinishedLoading : GameNetworkMessage
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000137 RID: 311 RVA: 0x000037DB File Offset: 0x000019DB
		// (set) Token: 0x06000138 RID: 312 RVA: 0x000037E3 File Offset: 0x000019E3
		public int BattleIndex { get; private set; }

		// Token: 0x06000139 RID: 313 RVA: 0x000037EC File Offset: 0x000019EC
		public FinishedLoading()
		{
		}

		// Token: 0x0600013A RID: 314 RVA: 0x000037F4 File Offset: 0x000019F4
		public FinishedLoading(int battleIndex)
		{
			this.BattleIndex = battleIndex;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00003804 File Offset: 0x00001A04
		protected override bool OnRead()
		{
			bool flag = true;
			this.BattleIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AutomatedBattleIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00003826 File Offset: 0x00001A26
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.BattleIndex, CompressionMission.AutomatedBattleIndexCompressionInfo);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00003838 File Offset: 0x00001A38
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000383C File Offset: 0x00001A3C
		protected override string OnGetLogFormat()
		{
			return "Finished Loading";
		}
	}
}
