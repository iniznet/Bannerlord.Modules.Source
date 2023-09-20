using System;
using TaleWorlds.Diamond;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	// Token: 0x020000DD RID: 221
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class RequestMaxAllowedPriorityResponse : FunctionResult
	{
		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600032B RID: 811 RVA: 0x000043D1 File Offset: 0x000025D1
		// (set) Token: 0x0600032C RID: 812 RVA: 0x000043D9 File Offset: 0x000025D9
		public sbyte Priority { get; private set; }

		// Token: 0x0600032D RID: 813 RVA: 0x000043E2 File Offset: 0x000025E2
		public RequestMaxAllowedPriorityResponse(sbyte priority)
		{
			this.Priority = priority;
		}
	}
}
