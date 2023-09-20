using System;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000D7 RID: 215
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class BattleReadyResponseMessage : FunctionResult
	{
		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000315 RID: 789 RVA: 0x000042E0 File Offset: 0x000024E0
		// (set) Token: 0x06000316 RID: 790 RVA: 0x000042E8 File Offset: 0x000024E8
		public bool ShouldReportActivities { get; private set; }

		// Token: 0x06000317 RID: 791 RVA: 0x000042F1 File Offset: 0x000024F1
		public BattleReadyResponseMessage(bool shouldReportActivities)
		{
			this.ShouldReportActivities = shouldReportActivities;
		}
	}
}
