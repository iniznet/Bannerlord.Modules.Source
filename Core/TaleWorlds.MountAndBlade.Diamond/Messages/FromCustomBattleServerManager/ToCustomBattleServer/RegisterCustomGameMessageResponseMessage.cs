using System;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	// Token: 0x02000065 RID: 101
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[Serializable]
	public class RegisterCustomGameMessageResponseMessage : FunctionResult
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600019A RID: 410 RVA: 0x00003270 File Offset: 0x00001470
		// (set) Token: 0x0600019B RID: 411 RVA: 0x00003278 File Offset: 0x00001478
		public bool ShouldReportActivities { get; private set; }

		// Token: 0x0600019C RID: 412 RVA: 0x00003281 File Offset: 0x00001481
		public RegisterCustomGameMessageResponseMessage(bool shouldReportActivities)
		{
			this.ShouldReportActivities = shouldReportActivities;
		}
	}
}
