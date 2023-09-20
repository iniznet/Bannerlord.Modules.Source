using System;
using TaleWorlds.Diamond;
using TaleWorlds.Library;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	// Token: 0x020000CC RID: 204
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleServerReadyMessage : LoginMessage
	{
		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x00004089 File Offset: 0x00002289
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x00004091 File Offset: 0x00002291
		public ApplicationVersion ApplicationVersion { get; private set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0000409A File Offset: 0x0000229A
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x000040A2 File Offset: 0x000022A2
		public string AssignedAddress { get; private set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x000040AB File Offset: 0x000022AB
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x000040B3 File Offset: 0x000022B3
		public ushort AssignedPort { get; private set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x000040BC File Offset: 0x000022BC
		// (set) Token: 0x060002E7 RID: 743 RVA: 0x000040C4 File Offset: 0x000022C4
		public string Region { get; private set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x000040CD File Offset: 0x000022CD
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x000040D5 File Offset: 0x000022D5
		public sbyte Priority { get; private set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060002EA RID: 746 RVA: 0x000040DE File Offset: 0x000022DE
		// (set) Token: 0x060002EB RID: 747 RVA: 0x000040E6 File Offset: 0x000022E6
		public string Password { get; private set; }

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060002EC RID: 748 RVA: 0x000040EF File Offset: 0x000022EF
		// (set) Token: 0x060002ED RID: 749 RVA: 0x000040F7 File Offset: 0x000022F7
		public string GameType { get; private set; }

		// Token: 0x060002EE RID: 750 RVA: 0x00004100 File Offset: 0x00002300
		public BattleServerReadyMessage(PeerId peerId, ApplicationVersion applicationVersion, string assignedAddress, ushort assignedPort, string region, sbyte priority, string password, string gameType)
			: base(peerId)
		{
			this.ApplicationVersion = applicationVersion;
			this.AssignedAddress = assignedAddress;
			this.AssignedPort = assignedPort;
			this.Region = region;
			this.Priority = priority;
			this.Password = password;
			this.GameType = gameType;
		}
	}
}
