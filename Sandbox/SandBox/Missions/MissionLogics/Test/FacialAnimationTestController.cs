using System;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Test
{
	public class FacialAnimationTestController : MissionLogic
	{
		public override void AfterStart()
		{
			this.InitializeTeams(true);
			base.Mission.SetMissionMode(0, true);
			Agent agent = this.SpawnTestAgent(Game.Current.PlayerTroop, "sp_player", true);
			agent.Controller = 2;
			agent.SetActionChannel(0, ActionIndexCache.act_none, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			this.SpawnTestAgent("convo_test_normal3", "rider_male", false);
			this.SpawnTestAgent("convo_test_normal4", "rider_female", false);
			Agent agent2 = this.SpawnTestAgent("convo_test_normal3", "chair_male", true);
			StandingPoint standingPoint = base.Mission.Scene.FindEntityWithTag("chair_male").GetFirstScriptOfType<Chair>().StandingPoints[0];
			WorldFrame userFrameForAgent = standingPoint.GetUserFrameForAgent(agent2);
			agent2.TeleportToPosition(userFrameForAgent.Origin.GetGroundVec3());
			Agent agent3 = agent2;
			Vec2 vec = userFrameForAgent.Rotation.f.AsVec2;
			vec = vec.Normalized();
			agent3.SetMovementDirection(ref vec);
			agent2.UseGameObject(standingPoint, -1);
			Agent agent4 = this.SpawnTestAgent("convo_test_normal4", "chair_female", true);
			StandingPoint standingPoint2 = base.Mission.Scene.FindEntityWithTag("chair_female").GetFirstScriptOfType<Chair>().StandingPoints[0];
			WorldFrame userFrameForAgent2 = standingPoint2.GetUserFrameForAgent(agent4);
			agent4.TeleportToPosition(userFrameForAgent2.Origin.GetGroundVec3());
			Agent agent5 = agent4;
			vec = userFrameForAgent2.Rotation.f.AsVec2;
			vec = vec.Normalized();
			agent5.SetMovementDirection(ref vec);
			agent4.UseGameObject(standingPoint2, -1);
			this.SpawnTestAgent("convo_test_normal3", "male_1", true);
			this.SpawnTestAgent("convo_test_normal4", "female_1", true);
			this.SpawnTestAgent("convo_test_aggressive", "sp_1", true);
			this.SpawnTestAgent("convo_test_warrior", "sp_2", true);
			this.SpawnTestAgent("convo_test_confident1", "sp_3", true);
			this.SpawnTestAgent("convo_test_confident2", "sp_4", true);
			this.SpawnTestAgent("convo_test_weary", "sp_5", true);
			this.SpawnTestAgent("convo_test_nervous", "sp_6", true);
			this.SpawnTestAgent("convo_test_demure", "sp_7", true);
			this.SpawnTestAgent("convo_test_closed", "sp_8", true);
			this.SpawnTestAgent("convo_test_hip", "sp_9", true);
		}

		private Agent SpawnTestAgent(string characterId, string entityName, bool noHorse = true)
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(characterId);
			return this.SpawnTestAgent(@object, entityName, noHorse);
		}

		private Agent SpawnTestAgent(BasicCharacterObject character, string entityName, bool noHorse = true)
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag(entityName);
			AgentBuildData agentBuildData = new AgentBuildData(character).TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam).InitialFrameFromSpawnPointEntity(gameEntity)
				.NoHorses(noHorse);
			Agent agent = base.Mission.SpawnAgent(agentBuildData, false);
			if (agent.MountAgent == null && agent != Agent.Main)
			{
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(agentBuildData.AgentMonster, MBGlobals.GetActionSetWithSuffix(agent.Monster, agent.IsFemale, "_villager"), agent.Character.GetStepSize(), false);
				agent.SetActionSet(ref animationSystemData);
				agent.SetActionChannel(0, FacialAnimationTestController.act_stand_1, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			return agent;
		}

		private void InitializeTeams(bool isPlayerAttacker = true)
		{
			if (!Extensions.IsEmpty<Team>(base.Mission.Teams))
			{
				throw new MBIllegalValueException("Number of teams is not 0.");
			}
			base.Mission.Teams.Add(0, uint.MaxValue, uint.MaxValue, null, true, false, true);
			base.Mission.Teams.Add(1, uint.MaxValue, uint.MaxValue, null, true, false, true);
			base.Mission.Teams.Add(1, uint.MaxValue, uint.MaxValue, null, true, false, true);
			if (isPlayerAttacker)
			{
				base.Mission.PlayerTeam = base.Mission.AttackerTeam;
				return;
			}
			base.Mission.PlayerTeam = base.Mission.DefenderTeam;
		}

		private static readonly ActionIndexCache act_stand_1 = ActionIndexCache.Create("act_stand_1");
	}
}
