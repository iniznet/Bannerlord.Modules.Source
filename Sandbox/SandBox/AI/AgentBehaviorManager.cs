using System;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SandBox.AI
{
	// Token: 0x020000CA RID: 202
	public class AgentBehaviorManager : IAgentBehaviorManager
	{
		// Token: 0x06000C03 RID: 3075 RVA: 0x0005F0D8 File Offset: 0x0005D2D8
		public void AddQuestCharacterBehaviors(IAgent agent)
		{
			BehaviorSets.AddQuestCharacterBehaviors(agent);
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0005F0E0 File Offset: 0x0005D2E0
		void IAgentBehaviorManager.AddWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddWandererBehaviors(agent);
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0005F0E8 File Offset: 0x0005D2E8
		void IAgentBehaviorManager.AddOutdoorWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddOutdoorWandererBehaviors(agent);
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0005F0F0 File Offset: 0x0005D2F0
		void IAgentBehaviorManager.AddIndoorWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddIndoorWandererBehaviors(agent);
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0005F0F8 File Offset: 0x0005D2F8
		void IAgentBehaviorManager.AddFixedCharacterBehaviors(IAgent agent)
		{
			BehaviorSets.AddFixedCharacterBehaviors(agent);
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0005F100 File Offset: 0x0005D300
		void IAgentBehaviorManager.AddAmbushPlayerBehaviors(IAgent agent)
		{
			BehaviorSets.AddAmbushPlayerBehaviors(agent);
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0005F108 File Offset: 0x0005D308
		void IAgentBehaviorManager.AddStandGuardBehaviors(IAgent agent)
		{
			BehaviorSets.AddStandGuardBehaviors(agent);
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0005F110 File Offset: 0x0005D310
		void IAgentBehaviorManager.AddPatrollingGuardBehaviors(IAgent agent)
		{
			BehaviorSets.AddPatrollingGuardBehaviors(agent);
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0005F118 File Offset: 0x0005D318
		void IAgentBehaviorManager.AddCompanionBehaviors(IAgent agent)
		{
			BehaviorSets.AddCompanionBehaviors(agent);
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0005F120 File Offset: 0x0005D320
		void IAgentBehaviorManager.AddBodyguardBehaviors(IAgent agent)
		{
			BehaviorSets.AddBodyguardBehaviors(agent);
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0005F128 File Offset: 0x0005D328
		public void AddFirstCompanionBehavior(IAgent agent)
		{
			BehaviorSets.AddFirstCompanionBehavior(agent);
		}
	}
}
