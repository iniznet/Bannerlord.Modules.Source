using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000077 RID: 119
	public interface IAgent
	{
		// Token: 0x17000278 RID: 632
		// (get) Token: 0x0600076A RID: 1898
		BasicCharacterObject Character { get; }

		// Token: 0x0600076B RID: 1899
		bool IsEnemyOf(IAgent agent);

		// Token: 0x0600076C RID: 1900
		bool IsFriendOf(IAgent agent);

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x0600076D RID: 1901
		AgentState State { get; }

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x0600076E RID: 1902
		IMissionTeam Team { get; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x0600076F RID: 1903
		IAgentOriginBase Origin { get; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000770 RID: 1904
		float Age { get; }

		// Token: 0x06000771 RID: 1905
		bool IsActive();

		// Token: 0x06000772 RID: 1906
		void SetAsConversationAgent(bool set);
	}
}
