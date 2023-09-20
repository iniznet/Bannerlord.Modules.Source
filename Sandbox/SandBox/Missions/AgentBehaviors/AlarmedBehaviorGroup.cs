using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	// Token: 0x0200006C RID: 108
	public class AlarmedBehaviorGroup : AgentBehaviorGroup
	{
		// Token: 0x060004AF RID: 1199 RVA: 0x00021A28 File Offset: 0x0001FC28
		public AlarmedBehaviorGroup(AgentNavigator navigator, Mission mission)
			: base(navigator, mission)
		{
			this._alarmedTimer = new BasicMissionTimer();
			this._checkCalmDownTimer = new BasicMissionTimer();
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._missionFightHandler = base.Mission.GetMissionBehavior<MissionFightHandler>();
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00021A78 File Offset: 0x0001FC78
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

		// Token: 0x060004B1 RID: 1201 RVA: 0x00021B2C File Offset: 0x0001FD2C
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

		// Token: 0x060004B2 RID: 1202 RVA: 0x00021B88 File Offset: 0x0001FD88
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

		// Token: 0x060004B3 RID: 1203 RVA: 0x00021C98 File Offset: 0x0001FE98
		private bool IsNearDanger()
		{
			float num;
			Agent closestAlarmSource = this.GetClosestAlarmSource(out num);
			return closestAlarmSource != null && (num < 225f || this.Navigator.CanSeeAgent(closestAlarmSource));
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00021CCC File Offset: 0x0001FECC
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

		// Token: 0x060004B5 RID: 1205 RVA: 0x00021D64 File Offset: 0x0001FF64
		public static void AlarmAgent(Agent agent)
		{
			agent.SetWatchState(2);
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00021D70 File Offset: 0x0001FF70
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

		// Token: 0x060004B7 RID: 1207 RVA: 0x00021E78 File Offset: 0x00020078
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

		// Token: 0x060004B8 RID: 1208 RVA: 0x00021F84 File Offset: 0x00020184
		public override void ForceThink(float inSeconds)
		{
		}

		// Token: 0x0400023E RID: 574
		private static readonly ActionIndexCache act_scared_to_normal_1 = ActionIndexCache.Create("act_scared_to_normal_1");

		// Token: 0x0400023F RID: 575
		public const float SafetyDistance = 15f;

		// Token: 0x04000240 RID: 576
		public const float SafetyDistanceSquared = 225f;

		// Token: 0x04000241 RID: 577
		private readonly MissionAgentHandler _missionAgentHandler;

		// Token: 0x04000242 RID: 578
		private readonly MissionFightHandler _missionFightHandler;

		// Token: 0x04000243 RID: 579
		public bool DisableCalmDown;

		// Token: 0x04000244 RID: 580
		private readonly BasicMissionTimer _alarmedTimer;

		// Token: 0x04000245 RID: 581
		private readonly BasicMissionTimer _checkCalmDownTimer;

		// Token: 0x04000246 RID: 582
		private bool _isCalmingDown;
	}
}
