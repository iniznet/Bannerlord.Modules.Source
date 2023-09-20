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
	public class HideoutMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
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

		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = false;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleAgentLogic = base.Mission.GetMissionBehavior<BattleAgentLogic>();
			this._battleEndLogic = base.Mission.GetMissionBehavior<BattleEndLogic>();
			this._battleEndLogic.ChangeCanCheckForEndCondition(false);
			this._agentVictoryLogic = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
			base.Mission.IsMainAgentObjectInteractionEnabled = false;
		}

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

		public void StartSpawner(BattleSideEnum side)
		{
			this._missionSides[side].SetSpawnTroops(true);
		}

		public void StopSpawner(BattleSideEnum side)
		{
			this._missionSides[side].SetSpawnTroops(false);
		}

		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return this._missionSides[side].TroopSpawningActive;
		}

		public float GetReinforcementInterval()
		{
			return 0f;
		}

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

		private void DecideMissionState()
		{
			HideoutMissionController.MissionSide missionSide = this._missionSides[0];
			this._hideoutMissionState = ((!missionSide.IsPlayerSide) ? HideoutMissionController.HideoutMissionState.InitialFightBeforeBossFight : HideoutMissionController.HideoutMissionState.WithoutBossFight);
		}

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

		private void StartCutScene()
		{
			List<Agent> list = base.Mission.Agents.Where((Agent x) => x.IsActive() && x.Team == base.Mission.PlayerTeam && x.IsHuman && x.IsAIControlled).ToList<Agent>();
			List<Agent> list2 = base.Mission.Agents.Where((Agent x) => x.IsActive() && x.Team == this._enemyTeam && x.IsHuman && x.IsAIControlled && x != this._bossAgent).ToList<Agent>();
			base.Mission.GetMissionBehavior<HideoutCinematicController>().StartCinematic(Agent.Main, list, this._bossAgent, list2, new HideoutCinematicController.OnHideoutCinematicFinished(this.OnCutSceneOver), 0.25f, 0.20943952f, 0.4f, 0.2f, 8f);
		}

		private void OnCutSceneOver()
		{
			Mission.Current.SetMissionMode(this._oldMissionMode, false);
			this._hideoutMissionState = HideoutMissionController.HideoutMissionState.ConversationBetweenLeaders;
			MissionConversationLogic missionBehavior = base.Mission.GetMissionBehavior<MissionConversationLogic>();
			missionBehavior.DisableStartConversation(false);
			missionBehavior.StartConversation(this._bossAgent, false, false);
		}

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

		private const int FirstPhaseEndInSeconds = 4;

		private readonly List<CommonAreaMarker> _areaMarkers;

		private readonly List<PatrolArea> _patrolAreas;

		private readonly Dictionary<Agent, HideoutMissionController.UsedObject> _defenderAgentObjects;

		private readonly HideoutMissionController.MissionSide[] _missionSides;

		private List<Agent> _duelPhaseAllyAgents;

		private List<Agent> _duelPhaseBanditAgents;

		private BattleAgentLogic _battleAgentLogic;

		private BattleEndLogic _battleEndLogic;

		private AgentVictoryLogic _agentVictoryLogic;

		private HideoutMissionController.HideoutMissionState _hideoutMissionState;

		private Agent _bossAgent;

		private Team _enemyTeam;

		private Timer _firstPhaseEndTimer;

		private bool _troopsInitialized;

		private bool _isMissionInitialized;

		private bool _battleResolved;

		private int _firstPhaseEnemyTroopCount;

		private int _firstPhasePlayerSideTroopCount;

		private MissionMode _oldMissionMode;

		private class MissionSide
		{
			public bool TroopSpawningActive { get; private set; }

			public int NumberOfActiveTroops
			{
				get
				{
					return this._numberOfSpawnedTroops - this._troopSupplier.NumRemovedTroops;
				}
			}

			public int NumberOfTroopsNotSupplied
			{
				get
				{
					return this._troopSupplier.NumTroopsNotSupplied;
				}
			}

			public MissionSide(BattleSideEnum side, IMissionTroopSupplier troopSupplier, bool isPlayerSide)
			{
				this._side = side;
				this.IsPlayerSide = isPlayerSide;
				this._troopSupplier = troopSupplier;
			}

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

			public void SetSpawnTroops(bool spawnTroops)
			{
				this.TroopSpawningActive = spawnTroops;
			}

			private readonly BattleSideEnum _side;

			private readonly IMissionTroopSupplier _troopSupplier;

			public readonly bool IsPlayerSide;

			private int _numberOfSpawnedTroops;
		}

		private class UsedObject
		{
			public UsedObject(UsableMachine machine, bool isMachineAITicked)
			{
				this.Machine = machine;
				this.MachineAI = machine.CreateAIBehaviorObject();
				this.IsMachineAITicked = isMachineAITicked;
			}

			public readonly UsableMachine Machine;

			public readonly UsableMachineAIBase MachineAI;

			public bool IsMachineAITicked;
		}

		private enum HideoutMissionState
		{
			NotDecided,
			WithoutBossFight,
			InitialFightBeforeBossFight,
			CutSceneBeforeBossFight,
			ConversationBetweenLeaders,
			BossFightWithDuel,
			BossFightWithAll
		}
	}
}
