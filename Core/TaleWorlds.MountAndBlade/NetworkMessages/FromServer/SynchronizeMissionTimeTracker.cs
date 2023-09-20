using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200005A RID: 90
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SynchronizeMissionTimeTracker : GameNetworkMessage
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000324 RID: 804 RVA: 0x00006440 File Offset: 0x00004640
		// (set) Token: 0x06000325 RID: 805 RVA: 0x00006448 File Offset: 0x00004648
		public float CurrentTime { get; private set; }

		// Token: 0x06000326 RID: 806 RVA: 0x00006451 File Offset: 0x00004651
		public SynchronizeMissionTimeTracker(float currentTime)
		{
			this.CurrentTime = currentTime;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00006460 File Offset: 0x00004660
		public SynchronizeMissionTimeTracker()
		{
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00006468 File Offset: 0x00004668
		protected override bool OnRead()
		{
			bool flag = true;
			this.CurrentTime = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000648A File Offset: 0x0000468A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteFloatToPacket(this.CurrentTime, CompressionMatchmaker.MissionTimeCompressionInfo);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000649C File Offset: 0x0000469C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x000064A4 File Offset: 0x000046A4
		protected override string OnGetLogFormat()
		{
			return this.CurrentTime + " seconds have elapsed since the start of the mission.";
		}
	}
}
