using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Conversation.MissionLogics;
using SandBox.Objects.AnimationPoints;
using SandBox.Objects.AreaMarkers;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200003C RID: 60
	public class HideoutMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x060002E1 RID: 737 RVA: 0x00012F0C File Offset: 0x0001110C
		public HideoutMissionController(IMissionTroopSupplier[] suppliers, BattleSideEnum playerSide, int firstPhaseEnemyTroopCount, int firstPhasePlayerSideTroopCount)
		{
			this._areaMarkers = new List<CommonAreaMarker>();
			this._patrolAreas = new List<PatrolArea>();
			this._defenderAgentObjects = new Dictionary<Agent, HideoutMissionController.UsedObject>();
			this._firstPhaseEnemyTroopCount = firstPhaseEnemyTroopCount;
			this._firstPhasePlayerSideTroopCount = firstPhasePlayerSideTroopCount;
			this._missionSides = new HideoutMissionController.MissionSide[2];
			for (int i = 0; i < 2; i++)
			{
				IMissionTroopSupplier missionTroopSupplier = suppliers[i];
				bool flag = i == playerSide;
				this._missionSides[i] = new HideoutMissionController.MissionSide(i, missionTroopSupplier, flag);
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00012F80 File Offset: 0x00011180
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = false;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00012F94 File Offset: 0x00011194
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleAgentLogic = base.Mission.GetMissionBehavior<BattleAgentLogic>();
			this._battleEndLogic = base.Mission.GetMissionBehavior<BattleEndLogic>();
			this._battleEndLogic.ChangeCanCheckForEndCondition(false);
			this._agentVictoryLogic = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
			base.Mission.IsMainAgentObjectInteractionEnabled = false;
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00012FF4 File Offset: 0x000111F4
		public override void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			if (usedObject != null && usedObject is AnimationPoint && userAgent.IsActive() && userAgent.IsAIControlled && userAgent.CurrentWatchState == null)
			{
				PatrolArea firstScriptOfType = usedObject.GameEntity.Parent.GetFirstScriptOfType<PatrolArea>();
				if (firstScriptOfType == null)
				{
					return;
				}
				firstScriptOfType.AddAgent(userAgent, -1);
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00013040 File Offset: 0x00011240
		public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
		{
			bool flag2 = flag == 2;
			if (flag2 || flag == 1)
			{
				if (agent.IsUsingGameObject)
				{
					agent.StopUsingGameObject(true, 1);
				}
				else
				{
					agent.DisableScriptedMovement();
					if (agent.IsAIControlled && AgentComponentExtensions.AIMoveToGameObjectIsEnabled(agent))
					{
						AgentComponentExtensions.AIMoveToGameObjectDisable(agent);
						Formation formation = agent.Formation;
						if (formation != null)
						{
							formation.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(agent);
						}
					}
				}
				this._defenderAgentObjects[agent].IsMachineAITicked = false;
			}
			else if (flag == null)
			{
				this._defenderAgentObjects[agent].IsMachineAITicked = true;
				agent.TryToSheathWeaponInHand(0, 0);
				this._defenderAgentObjects[agent].Machine.AddAgent(agent, -1);
			}
			if (flag2)
			{
				agent.SetWantsToYell();
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x000130F4 File Offset: 0x000112F4
		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this.InitializeMission();
				this._isMissionInitialized = true;
				return;
			}
			if (!this._troopsInitialized)
			{
				this._troopsInitialized = true;
				foreach (Agent agent in base.Mission.Agents)
				{
					this._battleAgentLogic.OnAgentBuild(agent, null);
				}
			}
			this.UsedObjectTick(dt);
			if (!this._battleResolved)
			{
				this.CheckBattleResolved();
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0001318C File Offset: 0x0001138C
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.BossFightWithDuel)
			{
				using (List<Agent>.Enumerator enumerator = base.Mission.Agents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Agent agent = enumerator.Current;
						if (agent != affectedAgent && agent != affectorAgent && agent.IsActive() && agent.GetLookAgent() == affectedAgent)
						{
							agent.SetLookAgent(null);
						}
					}
					return;
				}
			}
			if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.InitialFightBeforeBossFight && affectedAgent.IsMainAgent)
			{
				base.Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
				affectedAgent.Formation = null;
				base.Mission.PlayerTeam.PlayerOrderController.SetOrder(10);
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0001324C File Offset: 0x0001144C
		private void InitializeMission()
		{
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			base.Mission.SetMissionMode(4, true);
			this._areaMarkers.AddRange(from area in MBExtensions.FindAllWithType<CommonAreaMarker>(base.Mission.ActiveMissionObjects)
				orderby area.AreaIndex
				select area);
			this._patrolAreas.AddRange(from area in MBExtensions.FindAllWithType<PatrolArea>(base.Mission.ActiveMissionObjects)
				orderby area.AreaIndex
				select area);
			this.DecideMissionState();
			base.Mission.MakeDefaultDeploymentPlans();
			for (int i = 0; i < 2; i++)
			{
				int num;
				if (this._missionSides[i].IsPlayerSide)
				{
					num = this._firstPhasePlayerSideTroopCount;
				}
				else
				{
					if (this._missionSides[i].NumberOfTroopsNotSupplied <= this._firstPhaseEnemyTroopCount)
					{
						Debug.FailedAssert("_missionSides[i].NumberOfTroopsNotSupplied <= _firstPhaseEnemyTroopCount", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\HideoutMissionController.cs", "InitializeMission", 448);
						this._firstPhaseEnemyTroopCount = (int)((float)this._missionSides[i].NumberOfTroopsNotSupplied * 0.7f);
					}
					num = ((this._hideoutMissionState == HideoutMissionController.HideoutMissionState.InitialFightBeforeBossFight) ? this._firstPhaseEnemyTroopCount : this._missionSides[i].NumberOfTroopsNotSupplied);
				}
				this._missionSides[i].SpawnTroops(this._areaMarkers, this._patrolAreas, this._defenderAgentObjects, num);
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000133B8 File Offset: 0x000115B8
		private void UsedObjectTick(float dt)
		{
			foreach (KeyValuePair<Agent, HideoutMissionController.UsedObject> keyValuePair in this._defenderAgentObjects)
			{
				if (keyValuePair.Value.IsMachineAITicked)
				{
					keyValuePair.Value.MachineAI.Tick(keyValuePair.Key, null, null, dt);
				}
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00013430 File Offset: 0x00011630
		protected override void OnEndMission()
		{
			int num = 0;
			if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.BossFightWithDuel)
			{
				if (Agent.Main == null || !Agent.Main.IsActive())
				{
					List<Agent> duelPhaseAllyAgents = this._duelPhaseAllyAgents;
					num = ((duelPhaseAllyAgents != null) ? duelPhaseAllyAgents.Count : 0);
				}
				else if (this._bossAgent == null || !this._bossAgent.IsActive())
				{
					PlayerEncounter.EnemySurrender = true;
				}
			}
			if (MobileParty.MainParty.MemberRoster.TotalHealthyCount <= num && MapEvent.PlayerMapEvent.BattleState == null)
			{
				MapEvent.PlayerMapEvent.SetOverrideWinner(0);
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x000134B4 File Offset: 0x000116B4
		private void CheckBattleResolved()
		{
			if (this._hideoutMissionState != HideoutMissionController.HideoutMissionState.CutSceneBeforeBossFight && this._hideoutMissionState != HideoutMissionController.HideoutMissionState.ConversationBetweenLeaders)
			{
				if (this.IsSideDepleted(1))
				{
					if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.BossFightWithDuel)
					{
						this.OnDuelOver(0);
					}
					this._battleEndLogic.ChangeCanCheckForEndCondition(true);
					this._battleResolved = true;
					return;
				}
				if (this.IsSideDepleted(0))
				{
					if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.InitialFightBeforeBossFight)
					{
						if (this._firstPhaseEndTimer == null)
						{
							this._firstPhaseEndTimer = new Timer(base.Mission.CurrentTime, 4f, true);
							this._oldMissionMode = Mission.Current.Mode;
							Mission.Current.SetMissionMode(9, false);
							return;
						}
						if (this._firstPhaseEndTimer.Check(base.Mission.CurrentTime))
						{
							this.StartBossFight();
							return;
						}
					}
					else
					{
						if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.BossFightWithDuel)
						{
							this.OnDuelOver(1);
						}
						this._battleEndLogic.ChangeCanCheckForEndCondition(true);
						MapEvent.PlayerMapEvent.SetOverrideWinner(1);
						this._battleResolved = true;
					}
				}
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x000135AA File Offset: 0x000117AA
		public void StartSpawner(BattleSideEnum side)
		{
			this._missionSides[side].SetSpawnTroops(true);
		}

		// Token: 0x060002ED RID: 749 RVA: 0x000135BA File Offset: 0x000117BA
		public void StopSpawner(BattleSideEnum side)
		{
			this._missionSides[side].SetSpawnTroops(false);
		}

		// Token: 0x060002EE RID: 750 RVA: 0x000135CA File Offset: 0x000117CA
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return this._missionSides[side].TroopSpawningActive;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x000135D9 File Offset: 0x000117D9
		public float GetReinforcementInterval()
		{
			return 0f;
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x000135E0 File Offset: 0x000117E0
		public unsafe bool IsSideDepleted(BattleSideEnum side)
		{
			bool flag = this._missionSides[side].NumberOfActiveTroops == 0;
			if (!flag)
			{
				if ((Agent.Main == null || !Agent.Main.IsActive()) && side == 1)
				{
					if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.BossFightWithDuel || this._hideoutMissionState == HideoutMissionController.HideoutMissionState.InitialFightBeforeBossFight)
					{
						flag = true;
					}
					else if (this._hideoutMissionState == HideoutMissionController.HideoutMissionState.WithoutBossFight || this._hideoutMissionState == HideoutMissionController.HideoutMissionState.BossFightWithAll)
					{
						bool flag2 = base.Mission.Teams.Attacker.FormationsIncludingEmpty.Any(delegate(Formation f)
						{
							if (f.CountOfUnits > 0)
							{
								MovementOrder movementOrder = *f.GetReadonlyMovementOrderReference();
								return movementOrder.OrderType == 4;
							}
							return false;
						});
						bool flag3 = base.Mission.Teams.Defender.ActiveAgents.Any((Agent t) => t.CurrentWatchState == 2);
						flag = !flag2 && !flag3;
					}
				}
				else if (side == null && this._hideoutMissionState == HideoutMissionController.HideoutMissionState.BossFightWithDuel && (this._bossAgent == null || !this._bossAgent.IsActive()))
				{
					flag = true;
				}
			}
			else if (side == null && this._hideoutMissionState == HideoutMissionController.HideoutMissionState.InitialFightBeforeBossFight && (Agent.Main == null || !Agent.Main.IsActive()))
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00013718 File Offset: 0x00011918
		private void DecideMissionState()
		{
			HideoutMissionController.MissionSide missionSide = this._missionSides[0];
			this._hideoutMissionState = ((!missionSide.IsPlayerSide) ? HideoutMissionController.HideoutMissionState.InitialFightBeforeBossFight : HideoutMissionController.HideoutMissionState.WithoutBossFight);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00013740 File Offset: 0x00011940
		private void StartBossFight()
		{
			this._hideoutMissionState = HideoutMissionController.HideoutMissionState.CutSceneBeforeBossFight;
			this._enemyTeam = base.Mission.PlayerEnemyTeam;
			this.SpawnBossAndBodyguards();
			base.Mission.PlayerTeam.SetIsEnemyOf(this._enemyTeam, false);
			this.SetWatchStateOfAIAgents(0);
			if (Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(false, 1);
			}
			this.StartCutScene();
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x000137A8 File Offset: 0x000119A8
		private void SetWatchStateOfAIAgents(Agent.WatchState state)
		{
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.IsAIControlled)
				{
					agent.SetWatchState(state);
				}
			}
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00013808 File Offset: 0x00011A08
		private void SpawnBossAndBodyguards()
		{
			HideoutMissionController.MissionSide missionSide = this._missionSides[0];
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = Agent.Main.Position + Agent.Main.LookDirection * -3f;
			missionSide.SpawnRemainingTroopsForBossFight(new List<MatrixFrame> { identity }, missionSide.NumberOfTroopsNotSupplied);
			this._bossAgent = this.SelectBossAgent();
			this._bossAgent.WieldInitialWeapons(2);
			foreach (Agent agent in this._enemyTeam.ActiveAgents)
			{
				if (agent != this._bossAgent)
				{
					agent.WieldInitialWeapons(3);
				}
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x000138D4 File Offset: 0x00011AD4
		private Agent SelectBossAgent()
		{
			Agent agent = null;
			Agent agent2 = null;
			foreach (Agent agent3 in base.Mission.Agents)
			{
				if (agent3.Team == this._enemyTeam && agent3.IsHuman)
				{
					if (agent3.IsHero)
					{
						agent = agent3;
						break;
					}
					if (agent3.Character.Culture.IsBandit)
					{
						CultureObject cultureObject = agent3.Character.Culture as CultureObject;
						if (((cultureObject != null) ? cultureObject.BanditBoss : null) != null && ((CultureObject)agent3.Character.Culture).BanditBoss == agent3.Character)
						{
							agent = agent3;
						}
					}
					if (agent2 == null || agent3.Character.Level > agent2.Character.Level)
					{
						agent2 = agent3;
					}
				}
			}
			agent = agent ?? agent2;
			return agent;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x000139CC File Offset: 0x00011BCC
		private void StartCutScene()
		{
			List<Agent> list = base.Mission.Agents.Where((Agent x) => x.IsActive() && x.Team == base.Mission.PlayerTeam && x.IsHuman && x.IsAIControlled).ToList<Agent>();
			List<Agent> list2 = base.Mission.Agents.Where((Agent x) => x.IsActive() && x.Team == this._enemyTeam && x.IsHuman && x.IsAIControlled && x != this._bossAgent).ToList<Agent>();
			base.Mission.GetMissionBehavior<HideoutCinematicController>().StartCinematic(Agent.Main, list, this._bossAgent, list2, new HideoutCinematicController.OnHideoutCinematicFinished(this.OnCutSceneOver), 0.25f, 0.20943952f, 0.4f, 0.2f, 8f);
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00013A5F File Offset: 0x00011C5F
		private void OnCutSceneOver()
		{
			Mission.Current.SetMissionMode(this._oldMissionMode, false);
			this._hideoutMissionState = HideoutMissionController.HideoutMissionState.ConversationBetweenLeaders;
			MissionConversationLogic missionBehavior = base.Mission.GetMissionBehavior<MissionConversationLogic>();
			missionBehavior.DisableStartConversation(false);
			missionBehavior.StartConversation(this._bossAgent, false, false);
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00013A98 File Offset: 0x00011C98
		private void OnDuelOver(BattleSideEnum winnerSide)
		{
			AgentVictoryLogic missionBehavior = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.SetCheerActionGroup(3);
			}
			if (missionBehavior != null)
			{
				missionBehavior.SetCheerReactionTimerSettings(0.25f, 3f);
			}
			if (winnerSide == 1 && this._duelPhaseAllyAgents != null)
			{
				using (List<Agent>.Enumerator enumerator = this._duelPhaseAllyAgents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Agent agent = enumerator.Current;
						if (agent.State == 1)
						{
							agent.SetTeam(base.Mission.PlayerTeam, true);
							agent.SetWatchState(2);
						}
					}
					return;
				}
			}
			if (winnerSide == null && this._duelPhaseBanditAgents != null)
			{
				foreach (Agent agent2 in this._duelPhaseBanditAgents)
				{
					if (agent2.State == 1)
					{
						agent2.SetTeam(this._enemyTeam, true);
						agent2.SetWatchState(2);
					}
				}
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00013BA4 File Offset: 0x00011DA4
		public static void StartBossFightDuelMode()
		{
			Mission mission = Mission.Current;
			HideoutMissionController hideoutMissionController = ((mission != null) ? mission.GetMissionBehavior<HideoutMissionController>() : null);
			if (hideoutMissionController == null)
			{
				return;
			}
			hideoutMissionController.StartBossFightDuelModeInternal();
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00013BC4 File Offset: 0x00011DC4
		private void StartBossFightDuelModeInternal()
		{
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			base.Mission.PlayerTeam.SetIsEnemyOf(this._enemyTeam, true);
			this._duelPhaseAllyAgents = base.Mission.Agents.Where((Agent x) => x.IsActive() && x.Team == base.Mission.PlayerTeam && x.IsHuman && x.IsAIControlled && x != Agent.Main).ToList<Agent>();
			this._duelPhaseBanditAgents = base.Mission.Agents.Where((Agent x) => x.IsActive() && x.Team == this._enemyTeam && x.IsHuman && x.IsAIControlled && x != this._bossAgent).ToList<Agent>();
			foreach (Agent agent in this._duelPhaseAllyAgents)
			{
				agent.SetTeam(Team.Invalid, true);
				WorldPosition worldPosition = agent.GetWorldPosition();
				agent.SetScriptedPosition(ref worldPosition, false, 0);
				agent.SetLookAgent(Agent.Main);
			}
			foreach (Agent agent2 in this._duelPhaseBanditAgents)
			{
				agent2.SetTeam(Team.Invalid, true);
				WorldPosition worldPosition2 = agent2.GetWorldPosition();
				agent2.SetScriptedPosition(ref worldPosition2, false, 0);
				agent2.SetLookAgent(this._bossAgent);
			}
			this._bossAgent.SetWatchState(2);
			this._hideoutMissionState = HideoutMissionController.HideoutMissionState.BossFightWithDuel;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00013D24 File Offset: 0x00011F24
		public static void StartBossFightBattleMode()
		{
			Mission mission = Mission.Current;
			HideoutMissionController hideoutMissionController = ((mission != null) ? mission.GetMissionBehavior<HideoutMissionController>() : null);
			if (hideoutMissionController == null)
			{
				return;
			}
			hideoutMissionController.StartBossFightBattleModeInternal();
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00013D44 File Offset: 0x00011F44
		private void StartBossFightBattleModeInternal()
		{
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			base.Mission.PlayerTeam.SetIsEnemyOf(this._enemyTeam, true);
			this.SetWatchStateOfAIAgents(2);
			this._hideoutMissionState = HideoutMissionController.HideoutMissionState.BossFightWithAll;
			foreach (Formation formation in base.Mission.PlayerTeam.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
					formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
				}
			}
		}

		// Token: 0x04000178 RID: 376
		private const int FirstPhaseEndInSeconds = 4;

		// Token: 0x04000179 RID: 377
		private readonly List<CommonAreaMarker> _areaMarkers;

		// Token: 0x0400017A RID: 378
		private readonly List<PatrolArea> _patrolAreas;

		// Token: 0x0400017B RID: 379
		private readonly Dictionary<Agent, HideoutMissionController.UsedObject> _defenderAgentObjects;

		// Token: 0x0400017C RID: 380
		private readonly HideoutMissionController.MissionSide[] _missionSides;

		// Token: 0x0400017D RID: 381
		private List<Agent> _duelPhaseAllyAgents;

		// Token: 0x0400017E RID: 382
		private List<Agent> _duelPhaseBanditAgents;

		// Token: 0x0400017F RID: 383
		private BattleAgentLogic _battleAgentLogic;

		// Token: 0x04000180 RID: 384
		private BattleEndLogic _battleEndLogic;

		// Token: 0x04000181 RID: 385
		private AgentVictoryLogic _agentVictoryLogic;

		// Token: 0x04000182 RID: 386
		private HideoutMissionController.HideoutMissionState _hideoutMissionState;

		// Token: 0x04000183 RID: 387
		private Agent _bossAgent;

		// Token: 0x04000184 RID: 388
		private Team _enemyTeam;

		// Token: 0x04000185 RID: 389
		private Timer _firstPhaseEndTimer;

		// Token: 0x04000186 RID: 390
		private bool _troopsInitialized;

		// Token: 0x04000187 RID: 391
		private bool _isMissionInitialized;

		// Token: 0x04000188 RID: 392
		private bool _battleResolved;

		// Token: 0x04000189 RID: 393
		private int _firstPhaseEnemyTroopCount;

		// Token: 0x0400018A RID: 394
		private int _firstPhasePlayerSideTroopCount;

		// Token: 0x0400018B RID: 395
		private MissionMode _oldMissionMode;

		// Token: 0x02000118 RID: 280
		private class MissionSide
		{
			// Token: 0x170000E8 RID: 232
			// (get) Token: 0x06000CCE RID: 3278 RVA: 0x00061FCD File Offset: 0x000601CD
			// (set) Token: 0x06000CCF RID: 3279 RVA: 0x00061FD5 File Offset: 0x000601D5
			public bool TroopSpawningActive { get; private set; }

			// Token: 0x170000E9 RID: 233
			// (get) Token: 0x06000CD0 RID: 3280 RVA: 0x00061FDE File Offset: 0x000601DE
			public int NumberOfActiveTroops
			{
				get
				{
					return this._numberOfSpawnedTroops - this._troopSupplier.NumRemovedTroops;
				}
			}

			// Token: 0x170000EA RID: 234
			// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x00061FF2 File Offset: 0x000601F2
			public int NumberOfTroopsNotSupplied
			{
				get
				{
					return this._troopSupplier.NumTroopsNotSupplied;
				}
			}

			// Token: 0x06000CD2 RID: 3282 RVA: 0x00061FFF File Offset: 0x000601FF
			public MissionSide(BattleSideEnum side, IMissionTroopSupplier troopSupplier, bool isPlayerSide)
			{
				this._side = side;
				this.IsPlayerSide = isPlayerSide;
				this._troopSupplier = troopSupplier;
			}

			// Token: 0x06000CD3 RID: 3283 RVA: 0x0006201C File Offset: 0x0006021C
			public void SpawnTroops(List<CommonAreaMarker> areaMarkers, List<PatrolArea> patrolAreas, Dictionary<Agent, HideoutMissionController.UsedObject> defenderAgentObjects, int spawnCount)
			{
				int num = 0;
				bool flag = false;
				List<StandingPoint> list = new List<StandingPoint>();
				foreach (CommonAreaMarker commonAreaMarker in areaMarkers)
				{
					foreach (UsableMachine usableMachine in commonAreaMarker.GetUsableMachinesInRange(null))
					{
						list.AddRange(usableMachine.StandingPoints);
					}
				}
				List<IAgentOriginBase> list2 = this._troopSupplier.SupplyTroops(spawnCount).ToList<IAgentOriginBase>();
				for (int i = 0; i < list2.Count; i++)
				{
					if (1 == this._side)
					{
						Mission.Current.SpawnTroop(list2[i], true, true, false, false, 0, 0, true, true, true, null, null, null, null, 10, false);
						this._numberOfSpawnedTroops++;
					}
					else if (areaMarkers.Count > num)
					{
						StandingPoint standingPoint = null;
						int num2 = list2.Count - i;
						if (num2 < list.Count / 2 && num2 < 4)
						{
							flag = true;
						}
						if (!flag)
						{
							Extensions.Shuffle<StandingPoint>(list);
							standingPoint = list.FirstOrDefault((StandingPoint point) => !point.IsDeactivated && !point.IsDisabled && !point.HasUser);
						}
						else
						{
							IEnumerable<PatrolArea> enumerable = patrolAreas.Where((PatrolArea area) => area.StandingPoints.All((StandingPoint point) => !point.HasUser && !point.HasAIMovingTo));
							if (!Extensions.IsEmpty<PatrolArea>(enumerable))
							{
								standingPoint = enumerable.First<PatrolArea>().StandingPoints[0];
							}
						}
						if (standingPoint != null && !standingPoint.IsDisabled)
						{
							MatrixFrame globalFrame = standingPoint.GameEntity.GetGlobalFrame();
							globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
							Agent agent = Mission.Current.SpawnTroop(list2[i], false, false, false, false, 0, 0, false, false, false, new Vec3?(globalFrame.origin), new Vec2?(globalFrame.rotation.f.AsVec2.Normalized()), "_hideout_bandit", null, 10, false);
							this.InitializeBanditAgent(agent, standingPoint, flag, defenderAgentObjects);
							this._numberOfSpawnedTroops++;
							int groupId = ((AnimationPoint)standingPoint).GroupId;
							if (flag)
							{
								goto IL_291;
							}
							using (List<StandingPoint>.Enumerator enumerator3 = standingPoint.GameEntity.Parent.GetFirstScriptOfType<UsableMachine>().StandingPoints.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									StandingPoint standingPoint2 = enumerator3.Current;
									int groupId2 = ((AnimationPoint)standingPoint2).GroupId;
									if (groupId == groupId2 && standingPoint2 != standingPoint)
									{
										standingPoint2.SetDisabledAndMakeInvisible(false);
									}
								}
								goto IL_291;
							}
						}
						num++;
					}
					IL_291:;
				}
				foreach (Formation formation in Mission.Current.AttackerTeam.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						formation.SetMovementOrder(MovementOrder.MovementOrderMove(formation.QuerySystem.MedianPosition));
					}
					formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
					if (Mission.Current.AttackerTeam == Mission.Current.PlayerTeam)
					{
						formation.PlayerOwner = Mission.Current.MainAgent;
					}
				}
			}

			// Token: 0x06000CD4 RID: 3284 RVA: 0x00062390 File Offset: 0x00060590
			public void SpawnRemainingTroopsForBossFight(List<MatrixFrame> spawnFrames, int spawnCount)
			{
				List<IAgentOriginBase> list = this._troopSupplier.SupplyTroops(spawnCount).ToList<IAgentOriginBase>();
				for (int i = 0; i < list.Count; i++)
				{
					MatrixFrame matrixFrame = spawnFrames.FirstOrDefault<MatrixFrame>();
					matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
					Agent agent = Mission.Current.SpawnTroop(list[i], false, false, false, false, 0, 0, false, false, false, new Vec3?(matrixFrame.origin), new Vec2?(matrixFrame.rotation.f.AsVec2.Normalized()), "_hideout_bandit", null, 10, false);
					AgentFlag agentFlags = agent.GetAgentFlags();
					if (Extensions.HasAnyFlag<AgentFlag>(agentFlags, 1048576))
					{
						agent.SetAgentFlags(agentFlags & -1048577);
					}
					this._numberOfSpawnedTroops++;
				}
				foreach (Formation formation in Mission.Current.AttackerTeam.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						formation.SetMovementOrder(MovementOrder.MovementOrderMove(formation.QuerySystem.MedianPosition));
					}
					formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
					if (Mission.Current.AttackerTeam == Mission.Current.PlayerTeam)
					{
						formation.PlayerOwner = Mission.Current.MainAgent;
					}
				}
			}

			// Token: 0x06000CD5 RID: 3285 RVA: 0x000624F8 File Offset: 0x000606F8
			private void InitializeBanditAgent(Agent agent, StandingPoint spawnPoint, bool isPatrolling, Dictionary<Agent, HideoutMissionController.UsedObject> defenderAgentObjects)
			{
				UsableMachine usableMachine = (isPatrolling ? spawnPoint.GameEntity.Parent.GetScriptComponents<PatrolArea>().FirstOrDefault<PatrolArea>() : spawnPoint.GameEntity.Parent.GetScriptComponents<UsableMachine>().FirstOrDefault<UsableMachine>());
				if (isPatrolling)
				{
					usableMachine.AddAgent(agent, -1);
					agent.WieldInitialWeapons(2);
				}
				else
				{
					agent.UseGameObject(spawnPoint, -1);
				}
				defenderAgentObjects.Add(agent, new HideoutMissionController.UsedObject(usableMachine, isPatrolling));
				AgentFlag agentFlags = agent.GetAgentFlags();
				agent.SetAgentFlags((agentFlags | 65536) & -1048577);
				agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
				this.SimulateTick(agent);
			}

			// Token: 0x06000CD6 RID: 3286 RVA: 0x00062590 File Offset: 0x00060790
			private void SimulateTick(Agent agent)
			{
				int num = MBRandom.RandomInt(1, 20);
				for (int i = 0; i < num; i++)
				{
					if (agent.IsUsingGameObject)
					{
						agent.CurrentlyUsedGameObject.SimulateTick(0.1f);
					}
				}
			}

			// Token: 0x06000CD7 RID: 3287 RVA: 0x000625CA File Offset: 0x000607CA
			public void SetSpawnTroops(bool spawnTroops)
			{
				this.TroopSpawningActive = spawnTroops;
			}

			// Token: 0x04000566 RID: 1382
			private readonly BattleSideEnum _side;

			// Token: 0x04000567 RID: 1383
			private readonly IMissionTroopSupplier _troopSupplier;

			// Token: 0x04000568 RID: 1384
			public readonly bool IsPlayerSide;

			// Token: 0x0400056A RID: 1386
			private int _numberOfSpawnedTroops;
		}

		// Token: 0x02000119 RID: 281
		private class UsedObject
		{
			// Token: 0x06000CD8 RID: 3288 RVA: 0x000625D3 File Offset: 0x000607D3
			public UsedObject(UsableMachine machine, bool isMachineAITicked)
			{
				this.Machine = machine;
				this.MachineAI = machine.CreateAIBehaviorObject();
				this.IsMachineAITicked = isMachineAITicked;
			}

			// Token: 0x0400056B RID: 1387
			public readonly UsableMachine Machine;

			// Token: 0x0400056C RID: 1388
			public readonly UsableMachineAIBase MachineAI;

			// Token: 0x0400056D RID: 1389
			public bool IsMachineAITicked;
		}

		// Token: 0x0200011A RID: 282
		private enum HideoutMissionState
		{
			// Token: 0x0400056F RID: 1391
			NotDecided,
			// Token: 0x04000570 RID: 1392
			WithoutBossFight,
			// Token: 0x04000571 RID: 1393
			InitialFightBeforeBossFight,
			// Token: 0x04000572 RID: 1394
			CutSceneBeforeBossFight,
			// Token: 0x04000573 RID: 1395
			ConversationBetweenLeaders,
			// Token: 0x04000574 RID: 1396
			BossFightWithDuel,
			// Token: 0x04000575 RID: 1397
			BossFightWithAll
		}
	}
}
