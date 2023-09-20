using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class BehaviorSets
	{
		private static void AddBehaviorGroups(IAgent agent)
		{
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.AddBehaviorGroup<DailyBehaviorGroup>();
			agentNavigator.AddBehaviorGroup<InterruptingBehaviorGroup>();
			agentNavigator.AddBehaviorGroup<AlarmedBehaviorGroup>();
		}

		public static void AddQuestCharacterBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FleeBehavior>();
			behaviorGroup.AddBehavior<FightBehavior>();
		}

		public static void AddWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FleeBehavior>();
			behaviorGroup.AddBehavior<FightBehavior>();
		}

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

		public static void AddIndoorWandererBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>().SetOutdoorWandering(false);
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FleeBehavior>();
			behaviorGroup.AddBehavior<FightBehavior>();
		}

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

		public static void AddAmbushPlayerBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AlarmedBehaviorGroup behaviorGroup = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FightBehavior>();
			behaviorGroup.DisableCalmDown = true;
		}

		public static void AddStandGuardBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<StandGuardBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FightBehavior>();
			behaviorGroup.DisableCalmDown = true;
		}

		public static void AddPatrollingGuardBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<PatrollingGuardBehavior>();
			AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			behaviorGroup.AddBehavior<FightBehavior>();
			behaviorGroup.DisableCalmDown = true;
		}

		public static void AddCompanionBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<WalkingBehavior>().SetIndoorWandering(false);
			agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<FightBehavior>();
		}

		public static void AddBodyguardBehaviors(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			DailyBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			behaviorGroup.AddBehavior<WalkingBehavior>();
			behaviorGroup.AddBehavior<FollowAgentBehavior>().SetTargetAgent(Agent.Main);
			agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<FightBehavior>();
		}

		public static void AddFirstCompanionBehavior(IAgent agent)
		{
			BehaviorSets.AddBehaviorGroups(agent);
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<FightBehavior>();
		}
	}
}
