using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000087 RID: 135
	public interface IAgentBehaviorManager
	{
		// Token: 0x06001064 RID: 4196
		void AddQuestCharacterBehaviors(IAgent agent);

		// Token: 0x06001065 RID: 4197
		void AddWandererBehaviors(IAgent agent);

		// Token: 0x06001066 RID: 4198
		void AddOutdoorWandererBehaviors(IAgent agent);

		// Token: 0x06001067 RID: 4199
		void AddIndoorWandererBehaviors(IAgent agent);

		// Token: 0x06001068 RID: 4200
		void AddFixedCharacterBehaviors(IAgent agent);

		// Token: 0x06001069 RID: 4201
		void AddAmbushPlayerBehaviors(IAgent agent);

		// Token: 0x0600106A RID: 4202
		void AddStandGuardBehaviors(IAgent agent);

		// Token: 0x0600106B RID: 4203
		void AddPatrollingGuardBehaviors(IAgent agent);

		// Token: 0x0600106C RID: 4204
		void AddCompanionBehaviors(IAgent agent);

		// Token: 0x0600106D RID: 4205
		void AddBodyguardBehaviors(IAgent agent);

		// Token: 0x0600106E RID: 4206
		void AddFirstCompanionBehavior(IAgent agent);
	}
}
