using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x0200006D RID: 109
	public class BehaviorSets
	{
		// Token: 0x060004BA RID: 1210 RVA: 0x00021F97 File Offset: 0x00020197
		private static void AddBehaviorGroups(IAgent agent)
		{
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.AddBehaviorGroup<DailyBehaviorGroup>();
			agentNavigator.AddBehaviorGroup<InterruptingBehaviorGroup>();
			agentNavigator.AddBehaviorGroup<AlarmedBehaviorGroup>();
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x00021FBD File Offset: 0x000201BD
		public static void AddQuestCharacterBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FleeBehavior>();
			behaviorGroup.AddBehavior<FightBehavior>();
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x00021FF3 File Offset: 0x000201F3
		public static void AddWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FleeBehavior>();
			behaviorGroup.AddBehavior<FightBehavior>();
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0002202C File Offset: 0x0002022C
		public static void AddOutdoorWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			DailyBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			behaviorGroup.AddBehavior<WalkingBehavior>().SetIndoorWandering(false);
			behaviorGroup.AddBehavior<ChangeLocationBehavior>();
			AlarmedBehaviorGroup behaviorGroup2 = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup2.AddBehavior<FleeBehavior>();
			behaviorGroup2.AddBehavior<FightBehavior>();
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x00022079 File Offset: 0x00020279
		public static void AddIndoorWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>().SetOutdoorWandering(false);
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FleeBehavior>();
			behaviorGroup.AddBehavior<FightBehavior>();
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x000220B4 File Offset: 0x000202B4
		public static void AddFixedCharacterBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			WalkingBehavior walkingBehavior = agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>();
			walkingBehavior.SetIndoorWandering(false);
			walkingBehavior.SetOutdoorWandering(false);
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FleeBehavior>();
			behaviorGroup.AddBehavior<FightBehavior>();
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x00022101 File Offset: 0x00020301
		public static void AddAmbushPlayerBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AlarmedBehaviorGroup behaviorGroup = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FightBehavior>();
			behaviorGroup.DisableCalmDown = true;
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0002212B File Offset: 0x0002032B
		public static void AddStandGuardBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<StandGuardBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FightBehavior>();
			behaviorGroup.DisableCalmDown = true;
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x00022161 File Offset: 0x00020361
		public static void AddPatrollingGuardBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<PatrollingGuardBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FightBehavior>();
			behaviorGroup.DisableCalmDown = true;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00022197 File Offset: 0x00020397
		public static void AddCompanionBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>().SetIndoorWandering(false);
			agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<FightBehavior>();
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x000221CB File Offset: 0x000203CB
		public static void AddBodyguardBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			DailyBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			behaviorGroup.AddBehavior<WalkingBehavior>();
			behaviorGroup.AddBehavior<FollowAgentBehavior>().SetTargetAgent(Agent.Main);
			agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<FightBehavior>();
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0002220A File Offset: 0x0002040A
		public static void AddFirstCompanionBehavior(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<FightBehavior>();
		}
	}
}
