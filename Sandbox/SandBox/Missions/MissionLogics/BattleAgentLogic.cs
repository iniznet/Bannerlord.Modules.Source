using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000035 RID: 53
	public class BattleAgentLogic : MissionLogic
	{
		// Token: 0x06000271 RID: 625 RVA: 0x000108B3 File Offset: 0x0000EAB3
		public override void AfterStart()
		{
			this._battleObserverMissionLogic = Mission.Current.GetMissionBehavior<BattleObserverMissionLogic>();
			this.CheckPerkEffectsOnTeams();
		}

		// Token: 0x06000272 RID: 626 RVA: 0x000108CC File Offset: 0x0000EACC
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.Character != null && agent.Origin != null)
			{
				PartyBase partyBase = (PartyBase)agent.Origin.BattleCombatant;
				CharacterObject characterObject = (CharacterObject)agent.Character;
				if (partyBase != null)
				{
					this._troopUpgradeTracker.AddTrackedTroop(partyBase, characterObject);
				}
			}
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00010918 File Offset: 0x0000EB18
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			if (affectedAgent.Character != null && affectorAgent != null && affectorAgent.Character != null && affectedAgent.State == 1)
			{
				bool flag = affectedAgent.Health - (float)blow.InflictedDamage < 1f;
				bool flag2 = false;
				if (affectedAgent.Team != null && affectorAgent.Team != null)
				{
					flag2 = affectedAgent.Team.Side == affectorAgent.Team.Side;
				}
				IAgentOriginBase origin = affectorAgent.Origin;
				BasicCharacterObject character = affectedAgent.Character;
				Formation formation = affectorAgent.Formation;
				BasicCharacterObject basicCharacterObject;
				if (formation == null)
				{
					basicCharacterObject = null;
				}
				else
				{
					Agent captain = formation.Captain;
					basicCharacterObject = ((captain != null) ? captain.Character : null);
				}
				int inflictedDamage = blow.InflictedDamage;
				bool flag3 = flag;
				bool flag4 = flag2;
				MissionWeapon missionWeapon = attackerWeapon;
				origin.OnScoreHit(character, basicCharacterObject, inflictedDamage, flag3, flag4, missionWeapon.CurrentUsageItem);
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x000109D8 File Offset: 0x0000EBD8
		public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
		{
			if (prevTeam != null && newTeam != null && prevTeam != newTeam)
			{
				BattleObserverMissionLogic battleObserverMissionLogic = this._battleObserverMissionLogic;
				if (battleObserverMissionLogic == null)
				{
					return;
				}
				IBattleObserver battleObserver = battleObserverMissionLogic.BattleObserver;
				if (battleObserver == null)
				{
					return;
				}
				battleObserver.TroopSideChanged((prevTeam != null) ? prevTeam.Side : (-1), (newTeam != null) ? newTeam.Side : (-1), (PartyBase)agent.Origin.BattleCombatant, agent.Character);
			}
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00010A38 File Offset: 0x0000EC38
		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent == null)
			{
				return;
			}
			if (affectorAgent.IsMount && affectorAgent.RiderAgent != null)
			{
				affectorAgent = affectorAgent.RiderAgent;
			}
			if (affectorAgent.Character == null || affectedAgent.Character == null)
			{
				return;
			}
			float num = (float)blow.InflictedDamage;
			if (num > affectedAgent.HealthLimit)
			{
				num = affectedAgent.HealthLimit;
			}
			float num2 = num / affectedAgent.HealthLimit;
			this.EnemyHitReward(affectedAgent, affectorAgent, blow.MovementSpeedDamageModifier, shotDifficulty, isSiegeEngineHit, attackerWeapon, blow.AttackType, 0.5f * num2, num);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00010AB8 File Offset: 0x0000ECB8
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (base.Mission.Mode != 6)
			{
				if (affectorAgent == null && affectedAgent.IsMount && agentState == 2)
				{
					return;
				}
				CharacterObject characterObject = (CharacterObject)affectedAgent.Character;
				CharacterObject characterObject2 = (CharacterObject)((affectorAgent != null) ? affectorAgent.Character : null);
				if (affectedAgent.Origin != null)
				{
					PartyBase partyBase = (PartyBase)affectedAgent.Origin.BattleCombatant;
					if (agentState == 3)
					{
						affectedAgent.Origin.SetWounded();
					}
					else if (agentState == 4)
					{
						affectedAgent.Origin.SetKilled();
						Hero hero = (affectedAgent.IsHuman ? characterObject.HeroObject : null);
						Hero hero2 = ((affectorAgent == null) ? null : (affectorAgent.IsHuman ? characterObject2.HeroObject : null));
						if (hero != null && hero2 != null)
						{
							CampaignEventDispatcher.Instance.OnCharacterDefeated(hero2, hero);
						}
						if (partyBase != null)
						{
							this.CheckUpgrade(affectedAgent.Team.Side, partyBase, characterObject);
						}
					}
					else
					{
						affectedAgent.Origin.SetRouted();
					}
				}
				if (affectedAgent.Origin == null || affectorAgent == null || affectorAgent.Origin == null || affectorAgent.Character == null || agentState != 4)
				{
				}
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00010BC5 File Offset: 0x0000EDC5
		public override void OnAgentFleeing(Agent affectedAgent)
		{
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00010BC7 File Offset: 0x0000EDC7
		public override void OnMissionTick(float dt)
		{
			this.UpdateMorale();
			if (this._nextMoraleCheckTime.IsPast)
			{
				this._nextMoraleCheckTime = MissionTime.SecondsFromNow(10f);
			}
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00010BEC File Offset: 0x0000EDEC
		private void CheckPerkEffectsOnTeams()
		{
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00010BEE File Offset: 0x0000EDEE
		private void UpdateMorale()
		{
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00010BF0 File Offset: 0x0000EDF0
		private void EnemyHitReward(Agent affectedAgent, Agent affectorAgent, float lastSpeedBonus, float lastShotDifficulty, bool isSiegeEngineHit, WeaponComponentData lastAttackerWeapon, AgentAttackType attackType, float hitpointRatio, float damageAmount)
		{
			CharacterObject characterObject = (CharacterObject)affectedAgent.Character;
			CharacterObject characterObject2 = (CharacterObject)affectorAgent.Character;
			if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null && affectorAgent.Team != null && affectorAgent.Team.IsValid && affectedAgent.Team != null && affectedAgent.Team.IsValid)
			{
				PartyBase partyBase = (PartyBase)affectorAgent.Origin.BattleCombatant;
				Hero captain = BattleAgentLogic.GetCaptain(affectorAgent);
				Hero hero = ((affectorAgent.Team.Leader != null && affectorAgent.Team.Leader.Character.IsHero) ? ((CharacterObject)affectorAgent.Team.Leader.Character).HeroObject : null);
				bool flag = affectorAgent.Team.Side == affectedAgent.Team.Side;
				bool flag2 = affectorAgent.MountAgent != null;
				bool flag3 = flag2 && attackType == 3;
				SkillLevelingManager.OnCombatHit(characterObject2, characterObject, (captain != null) ? captain.CharacterObject : null, hero, lastSpeedBonus, lastShotDifficulty, lastAttackerWeapon, hitpointRatio, 0, flag2, flag, hero != null && affectorAgent.Character != hero.CharacterObject && (hero != Hero.MainHero || affectorAgent.Formation == null || !affectorAgent.Formation.IsAIControlled), damageAmount, affectedAgent.Health < 1f, isSiegeEngineHit, flag3);
				BattleObserverMissionLogic battleObserverMissionLogic = this._battleObserverMissionLogic;
				if (((battleObserverMissionLogic != null) ? battleObserverMissionLogic.BattleObserver : null) != null && affectorAgent.Character != null)
				{
					if (affectorAgent.Character.IsHero)
					{
						Hero heroObject = characterObject2.HeroObject;
						using (IEnumerator<SkillObject> enumerator = this._troopUpgradeTracker.CheckSkillUpgrades(heroObject).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								SkillObject skillObject = enumerator.Current;
								this._battleObserverMissionLogic.BattleObserver.HeroSkillIncreased(affectorAgent.Team.Side, partyBase, characterObject2, skillObject);
							}
							return;
						}
					}
					this.CheckUpgrade(affectorAgent.Team.Side, partyBase, characterObject2);
				}
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00010E10 File Offset: 0x0000F010
		private static Hero GetCaptain(Agent affectorAgent)
		{
			Hero hero = null;
			if (affectorAgent.Formation != null)
			{
				Agent captain = affectorAgent.Formation.Captain;
				if (captain != null)
				{
					float captainRadius = Campaign.Current.Models.CombatXpModel.CaptainRadius;
					if (captain.Position.Distance(affectorAgent.Position) < captainRadius)
					{
						hero = ((CharacterObject)captain.Character).HeroObject;
					}
				}
			}
			return hero;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00010E74 File Offset: 0x0000F074
		private void CheckUpgrade(BattleSideEnum side, PartyBase party, CharacterObject character)
		{
			BattleObserverMissionLogic battleObserverMissionLogic = this._battleObserverMissionLogic;
			if (((battleObserverMissionLogic != null) ? battleObserverMissionLogic.BattleObserver : null) != null)
			{
				int num = this._troopUpgradeTracker.CheckUpgradedCount(party, character);
				if (num != 0)
				{
					this._battleObserverMissionLogic.BattleObserver.TroopNumberChanged(side, party, character, 0, 0, 0, 0, 0, num);
				}
			}
		}

		// Token: 0x04000140 RID: 320
		private BattleObserverMissionLogic _battleObserverMissionLogic;

		// Token: 0x04000141 RID: 321
		private TroopUpgradeTracker _troopUpgradeTracker = new TroopUpgradeTracker();

		// Token: 0x04000142 RID: 322
		private const float XpShareForKill = 0.5f;

		// Token: 0x04000143 RID: 323
		private const float XpShareForDamage = 0.5f;

		// Token: 0x04000144 RID: 324
		private MissionTime _nextMoraleCheckTime;
	}
}
