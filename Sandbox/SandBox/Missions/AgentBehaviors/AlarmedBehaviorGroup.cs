using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class AlarmedBehaviorGroup : AgentBehaviorGroup
	{
		public AlarmedBehaviorGroup(AgentNavigator navigator, Mission mission)
			: base(navigator, mission)
		{
			this._alarmedTimer = new BasicMissionTimer();
			this._checkCalmDownTimer = new BasicMissionTimer();
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._missionFightHandler = base.Mission.GetMissionBehavior<MissionFightHandler>();
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (base.ScriptedBehavior != null)
			{
				if (!base.ScriptedBehavior.IsActive)
				{
					base.DisableAllBehaviors();
					base.ScriptedBehavior.IsActive = true;
				}
			}
			else
			{
				float num = 0f;
				int num2 = -1;
				for (int i = 0; i < this.Behaviors.Count; i++)
				{
					float availability = this.Behaviors[i].GetAvailability(isSimulation);
					if (availability > num)
					{
						num = availability;
						num2 = i;
					}
				}
				if (num > 0f && num2 != -1 && !this.Behaviors[num2].IsActive)
				{
					base.DisableAllBehaviors();
					this.Behaviors[num2].IsActive = true;
				}
			}
			this.TickActiveBehaviors(dt, isSimulation);
		}

		private void TickActiveBehaviors(float dt, bool isSimulation)
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior.IsActive)
				{
					agentBehavior.Tick(dt, isSimulation);
				}
			}
		}

		public override float GetScore(bool isSimulation)
		{
			if (base.OwnerAgent.CurrentWatchState == 2)
			{
				if (!this.DisableCalmDown && this._alarmedTimer.ElapsedTime > 10f && this._checkCalmDownTimer.ElapsedTime > 1f)
				{
					if (!this._isCalmingDown)
					{
						this._checkCalmDownTimer.Reset();
						if (!this.IsNearDanger())
						{
							this._isCalmingDown = true;
							base.OwnerAgent.DisableScriptedMovement();
							base.OwnerAgent.SetActionChannel(0, AlarmedBehaviorGroup.act_scared_to_normal_1, false, 0UL, 0f, 1f, -0.2f, 0.4f, MBRandom.RandomFloat, false, -0.2f, 0, true);
						}
					}
					else if (!base.OwnerAgent.ActionSet.AreActionsAlternatives(base.OwnerAgent.GetCurrentActionValue(0), AlarmedBehaviorGroup.act_scared_to_normal_1))
					{
						this._isCalmingDown = false;
						return 0f;
					}
				}
				return 1f;
			}
			if (this.IsNearDanger())
			{
				AlarmedBehaviorGroup.AlarmAgent(base.OwnerAgent);
				return 1f;
			}
			return 0f;
		}

		private bool IsNearDanger()
		{
			float num;
			Agent closestAlarmSource = this.GetClosestAlarmSource(out num);
			return closestAlarmSource != null && (num < 225f || this.Navigator.CanSeeAgent(closestAlarmSource));
		}

		public Agent GetClosestAlarmSource(out float distanceSquared)
		{
			distanceSquared = float.MaxValue;
			if (this._missionFightHandler == null || !this._missionFightHandler.IsThereActiveFight())
			{
				return null;
			}
			Agent agent = null;
			foreach (Agent agent2 in this._missionFightHandler.GetDangerSources(base.OwnerAgent))
			{
				float num = agent2.Position.DistanceSquared(base.OwnerAgent.Position);
				if (num < distanceSquared)
				{
					distanceSquared = num;
					agent = agent2;
				}
			}
			return agent;
		}

		public static void AlarmAgent(Agent agent)
		{
			agent.SetWatchState(2);
		}

		protected override void OnActivate()
		{
			TextObject textObject = new TextObject("{=!}{p0} {p1} activate alarmed behavior group.", null);
			textObject.SetTextVariable("p0", base.OwnerAgent.Name);
			textObject.SetTextVariable("p1", base.OwnerAgent.Index);
			this._isCalmingDown = false;
			this._alarmedTimer.Reset();
			this._checkCalmDownTimer.Reset();
			base.OwnerAgent.DisableScriptedMovement();
			base.OwnerAgent.ClearTargetFrame();
			this.Navigator.SetItemsVisibility(false);
			LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(base.OwnerAgent.Origin);
			if (locationCharacter.ActionSetCode != locationCharacter.AlarmedActionSetCode)
			{
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(locationCharacter.GetAgentBuildData().AgentMonster, MBGlobals.GetActionSet(locationCharacter.AlarmedActionSetCode), locationCharacter.Character.GetStepSize(), false);
				base.OwnerAgent.SetActionSet(ref animationSystemData);
			}
			if (this.Navigator.MemberOfAlley != null || MissionFightHandler.IsAgentAggressive(base.OwnerAgent))
			{
				this.DisableCalmDown = true;
			}
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._isCalmingDown = false;
			if (base.OwnerAgent.IsActive())
			{
				base.OwnerAgent.TryToSheathWeaponInHand(1, 3);
				base.OwnerAgent.TryToSheathWeaponInHand(0, 3);
				if (base.OwnerAgent.Team.IsValid && base.OwnerAgent.Team == base.Mission.PlayerEnemyTeam)
				{
					base.OwnerAgent.SetTeam(new Team(MBTeam.InvalidTeam, -1, null, uint.MaxValue, uint.MaxValue, null), true);
				}
				base.OwnerAgent.SetWatchState(0);
				base.OwnerAgent.ResetLookAgent();
				base.OwnerAgent.SetActionChannel(0, ActionIndexCache.act_none, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				base.OwnerAgent.SetActionChannel(1, ActionIndexCache.act_none, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		public override void ForceThink(float inSeconds)
		{
		}

		private static readonly ActionIndexCache act_scared_to_normal_1 = ActionIndexCache.Create("act_scared_to_normal_1");

		public const float SafetyDistance = 15f;

		public const float SafetyDistanceSquared = 225f;

		private readonly MissionAgentHandler _missionAgentHandler;

		private readonly MissionFightHandler _missionFightHandler;

		public bool DisableCalmDown;

		private readonly BasicMissionTimer _alarmedTimer;

		private readonly BasicMissionTimer _checkCalmDownTimer;

		private bool _isCalmingDown;
	}
}
