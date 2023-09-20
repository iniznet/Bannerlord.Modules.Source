using System;
using System.Collections.Generic;
using SandBox.Objects.Cinematics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200003B RID: 59
	public class HideoutCinematicController : MissionLogic
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060002CA RID: 714 RVA: 0x00012372 File Offset: 0x00010572
		// (set) Token: 0x060002CB RID: 715 RVA: 0x0001237A File Offset: 0x0001057A
		public HideoutCinematicController.HideoutCinematicState State { get; private set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060002CC RID: 716 RVA: 0x00012383 File Offset: 0x00010583
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0001238B File Offset: 0x0001058B
		public bool InStateTransition { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00012394 File Offset: 0x00010594
		public bool IsCinematicActive
		{
			get
			{
				return this.State > HideoutCinematicController.HideoutCinematicState.None;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0001239F File Offset: 0x0001059F
		public float CinematicDuration
		{
			get
			{
				return this._cinematicDuration;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x000123A7 File Offset: 0x000105A7
		public float TransitionDuration
		{
			get
			{
				return this._transitionDuration;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x000123AF File Offset: 0x000105AF
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x000123B4 File Offset: 0x000105B4
		public HideoutCinematicController()
		{
			this.State = HideoutCinematicController.HideoutCinematicState.None;
			this._cinematicFinishedCallback = null;
			this._transitionCallback = null;
			this._stateChangedCallback = null;
			this.InStateTransition = false;
			this._isBehaviorInit = false;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00012433 File Offset: 0x00010633
		public void SetStateTransitionCallback(HideoutCinematicController.OnHideoutCinematicStateChanged onStateChanged, HideoutCinematicController.OnHideoutCinematicTransition onTransition)
		{
			this._stateChangedCallback = onStateChanged;
			this._transitionCallback = onTransition;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00012444 File Offset: 0x00010644
		public void StartCinematic(Agent playerAgent, List<Agent> allyAgents, Agent bossAgent, List<Agent> banditAgents, HideoutCinematicController.OnHideoutCinematicFinished cinematicFinishedCallback, float placementPerturbation = 0.25f, float placementAngle = 0.20943952f, float transitionDuration = 0.4f, float stateDuration = 0.2f, float cinematicDuration = 8f)
		{
			if (this._isBehaviorInit && this.State == HideoutCinematicController.HideoutCinematicState.None)
			{
				this._cinematicFinishedCallback = cinematicFinishedCallback;
				this.ComputeAgentFrames(playerAgent, allyAgents, bossAgent, banditAgents, placementPerturbation, placementAngle);
				this._preCinematicPhase = HideoutCinematicController.HideoutPreCinematicPhase.InitializeFormations;
				this._postCinematicPhase = HideoutCinematicController.HideoutPostCinematicPhase.MoveAgents;
				this._transitionDuration = transitionDuration;
				this._stateDuration = stateDuration;
				this._cinematicDuration = cinematicDuration;
				this._remainingCinematicDuration = this._cinematicDuration;
				this.BeginStateTransition(HideoutCinematicController.HideoutCinematicState.PreCinematic);
				return;
			}
			if (!this._isBehaviorInit)
			{
				Debug.FailedAssert("Hideout cinematic controller is not initialized.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\HideoutCinematicController.cs", "StartCinematic", 172);
				return;
			}
			if (this.State != HideoutCinematicController.HideoutCinematicState.None)
			{
				Debug.FailedAssert("There is already an ongoing cinematic.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\HideoutCinematicController.cs", "StartCinematic", 176);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000124F8 File Offset: 0x000106F8
		public void GetBossStandingEyePosition(out Vec3 eyePosition)
		{
			Agent agent = this._bossAgentInfo.Agent;
			if (((agent != null) ? agent.Monster : null) != null)
			{
				eyePosition = this._bossAgentInfo.InitialFrame.origin + Vec3.Up * (this._bossAgentInfo.Agent.AgentScale * this._bossAgentInfo.Agent.Monster.StandingEyeHeight);
				return;
			}
			eyePosition = Vec3.Zero;
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\HideoutCinematicController.cs", "GetBossStandingEyePosition", 189);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00012590 File Offset: 0x00010790
		public void GetPlayerStandingEyePosition(out Vec3 eyePosition)
		{
			Agent agent = this._playerAgentInfo.Agent;
			if (((agent != null) ? agent.Monster : null) != null)
			{
				eyePosition = this._playerAgentInfo.InitialFrame.origin + Vec3.Up * (this._playerAgentInfo.Agent.AgentScale * this._playerAgentInfo.Agent.Monster.StandingEyeHeight);
				return;
			}
			eyePosition = Vec3.Zero;
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\HideoutCinematicController.cs", "GetPlayerStandingEyePosition", 202);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00012628 File Offset: 0x00010828
		public void GetScenePrefabParameters(out float innerRadius, out float outerRadius, out float walkDistance)
		{
			innerRadius = 0f;
			outerRadius = 0f;
			walkDistance = 0f;
			if (this._hideoutBossFightBehavior != null)
			{
				innerRadius = this._hideoutBossFightBehavior.InnerRadius;
				outerRadius = this._hideoutBossFightBehavior.OuterRadius;
				walkDistance = this._hideoutBossFightBehavior.WalkDistance;
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001267C File Offset: 0x0001087C
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("hideout_boss_fight");
			this._hideoutBossFightBehavior = ((gameEntity != null) ? gameEntity.GetFirstScriptOfType<HideoutBossFightBehavior>() : null);
			this._isBehaviorInit = gameEntity != null && this._hideoutBossFightBehavior != null;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x000126D4 File Offset: 0x000108D4
		public override void OnMissionTick(float dt)
		{
			if (this._isBehaviorInit && this.IsCinematicActive)
			{
				if (this.InStateTransition)
				{
					this.TickStateTransition(dt);
					return;
				}
				switch (this.State)
				{
				case HideoutCinematicController.HideoutCinematicState.PreCinematic:
					if (this.TickPreCinematic(dt))
					{
						this.BeginStateTransition(HideoutCinematicController.HideoutCinematicState.Cinematic);
						return;
					}
					break;
				case HideoutCinematicController.HideoutCinematicState.Cinematic:
					if (this.TickCinematic(dt))
					{
						this.BeginStateTransition(HideoutCinematicController.HideoutCinematicState.PostCinematic);
						return;
					}
					break;
				case HideoutCinematicController.HideoutCinematicState.PostCinematic:
					if (this.TickPostCinematic(dt))
					{
						this.BeginStateTransition(HideoutCinematicController.HideoutCinematicState.Completed);
						return;
					}
					break;
				case HideoutCinematicController.HideoutCinematicState.Completed:
				{
					HideoutCinematicController.OnHideoutCinematicFinished cinematicFinishedCallback = this._cinematicFinishedCallback;
					if (cinematicFinishedCallback != null)
					{
						cinematicFinishedCallback();
					}
					this.State = HideoutCinematicController.HideoutCinematicState.None;
					break;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00012770 File Offset: 0x00010970
		private void TickStateTransition(float dt)
		{
			this._remainingTransitionDuration -= dt;
			if (this._remainingTransitionDuration <= 0f)
			{
				this.InStateTransition = false;
				HideoutCinematicController.OnHideoutCinematicStateChanged stateChangedCallback = this._stateChangedCallback;
				if (stateChangedCallback != null)
				{
					stateChangedCallback(this.State);
				}
				this._remainingStateDuration = this._stateDuration;
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x000127C4 File Offset: 0x000109C4
		private bool TickPreCinematic(float dt)
		{
			Scene scene = base.Mission.Scene;
			this._remainingStateDuration -= dt;
			switch (this._preCinematicPhase)
			{
			case HideoutCinematicController.HideoutPreCinematicPhase.InitializeFormations:
			{
				this._playerAgentInfo.Agent.Controller = 1;
				bool isTeleportingAgents = base.Mission.IsTeleportingAgents;
				base.Mission.IsTeleportingAgents = true;
				foreach (Formation formation in base.Mission.Teams.Attacker.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						MatrixFrame matrixFrame;
						this._hideoutBossFightBehavior.GetAllyInitialFormationFrame(out matrixFrame);
						WorldPosition worldPosition;
						worldPosition..ctor(scene, matrixFrame.origin);
						formation.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
					}
				}
				foreach (Formation formation2 in base.Mission.Teams.Defender.FormationsIncludingEmpty)
				{
					if (formation2.CountOfUnits > 0)
					{
						MatrixFrame matrixFrame2;
						this._hideoutBossFightBehavior.GetBanditInitialFormationFrame(out matrixFrame2);
						WorldPosition worldPosition2;
						worldPosition2..ctor(scene, matrixFrame2.origin);
						formation2.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition2));
					}
				}
				base.Mission.IsTeleportingAgents = isTeleportingAgents;
				this._preCinematicPhase = HideoutCinematicController.HideoutPreCinematicPhase.StopFormations;
				break;
			}
			case HideoutCinematicController.HideoutPreCinematicPhase.StopFormations:
				foreach (Formation formation3 in base.Mission.Teams.Attacker.FormationsIncludingEmpty)
				{
					if (formation3.CountOfUnits > 0)
					{
						formation3.SetMovementOrder(MovementOrder.MovementOrderStop);
					}
				}
				foreach (Formation formation4 in base.Mission.Teams.Defender.FormationsIncludingEmpty)
				{
					if (formation4.CountOfUnits > 0)
					{
						formation4.SetMovementOrder(MovementOrder.MovementOrderStop);
					}
				}
				this._preCinematicPhase = HideoutCinematicController.HideoutPreCinematicPhase.InitializeAgents;
				break;
			case HideoutCinematicController.HideoutPreCinematicPhase.InitializeAgents:
				this._cachedAgentFormations = new List<Formation>();
				foreach (HideoutCinematicController.HideoutCinematicAgentInfo hideoutCinematicAgentInfo in this._hideoutAgentsInfo)
				{
					Agent agent = hideoutCinematicAgentInfo.Agent;
					this._cachedAgentFormations.Add(agent.Formation);
					agent.Formation = null;
					MatrixFrame initialFrame = hideoutCinematicAgentInfo.InitialFrame;
					WorldPosition worldPosition3;
					worldPosition3..ctor(scene, initialFrame.origin);
					agent.TeleportToPosition(worldPosition3.GetGroundVec3());
					Agent agent2 = agent;
					Vec2 vec = initialFrame.rotation.f.AsVec2;
					vec = vec.Normalized();
					agent2.SetMovementDirection(ref vec);
				}
				this._preCinematicPhase = HideoutCinematicController.HideoutPreCinematicPhase.MoveAgents;
				break;
			case HideoutCinematicController.HideoutPreCinematicPhase.MoveAgents:
				foreach (HideoutCinematicController.HideoutCinematicAgentInfo hideoutCinematicAgentInfo2 in this._hideoutAgentsInfo)
				{
					Agent agent3 = hideoutCinematicAgentInfo2.Agent;
					MatrixFrame targetFrame = hideoutCinematicAgentInfo2.TargetFrame;
					WorldPosition worldPosition4;
					worldPosition4..ctor(scene, targetFrame.origin);
					agent3.SetMaximumSpeedLimit(0.65f, false);
					Agent agent4 = agent3;
					Vec2 vec = targetFrame.rotation.f.AsVec2;
					agent4.SetScriptedPositionAndDirection(ref worldPosition4, vec.RotationInRadians, true, 0);
				}
				this._preCinematicPhase = HideoutCinematicController.HideoutPreCinematicPhase.Completed;
				break;
			}
			return this._preCinematicPhase == HideoutCinematicController.HideoutPreCinematicPhase.Completed && this._remainingStateDuration <= 0f;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00012B8C File Offset: 0x00010D8C
		private bool TickCinematic(float dt)
		{
			this._remainingCinematicDuration -= dt;
			this._remainingStateDuration -= dt;
			return this._remainingCinematicDuration <= 0f && this._remainingStateDuration <= 0f;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00012BC8 File Offset: 0x00010DC8
		private bool TickPostCinematic(float dt)
		{
			this._remainingStateDuration -= dt;
			HideoutCinematicController.HideoutPostCinematicPhase postCinematicPhase = this._postCinematicPhase;
			if (postCinematicPhase != HideoutCinematicController.HideoutPostCinematicPhase.MoveAgents)
			{
				if (postCinematicPhase == HideoutCinematicController.HideoutPostCinematicPhase.FinalizeAgents)
				{
					foreach (HideoutCinematicController.HideoutCinematicAgentInfo hideoutCinematicAgentInfo in this._hideoutAgentsInfo)
					{
						Agent agent = hideoutCinematicAgentInfo.Agent;
						agent.DisableScriptedMovement();
						agent.SetMaximumSpeedLimit(-1f, false);
					}
					this._postCinematicPhase = HideoutCinematicController.HideoutPostCinematicPhase.Completed;
				}
			}
			else
			{
				int num = 0;
				foreach (HideoutCinematicController.HideoutCinematicAgentInfo hideoutCinematicAgentInfo2 in this._hideoutAgentsInfo)
				{
					Agent agent2 = hideoutCinematicAgentInfo2.Agent;
					if (!hideoutCinematicAgentInfo2.HasReachedTarget(0.5f))
					{
						MatrixFrame targetFrame = hideoutCinematicAgentInfo2.TargetFrame;
						WorldPosition worldPosition;
						worldPosition..ctor(base.Mission.Scene, targetFrame.origin);
						agent2.TeleportToPosition(worldPosition.GetGroundVec3());
						Agent agent3 = agent2;
						Vec2 vec = targetFrame.rotation.f.AsVec2;
						vec = vec.Normalized();
						agent3.SetMovementDirection(ref vec);
					}
					agent2.Formation = this._cachedAgentFormations[num];
					num++;
				}
				this._postCinematicPhase = HideoutCinematicController.HideoutPostCinematicPhase.FinalizeAgents;
			}
			return this._postCinematicPhase == HideoutCinematicController.HideoutPostCinematicPhase.Completed && this._remainingStateDuration <= 0f;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00012D40 File Offset: 0x00010F40
		private void BeginStateTransition(HideoutCinematicController.HideoutCinematicState nextState)
		{
			this.State = nextState;
			this._remainingTransitionDuration = this._transitionDuration;
			this.InStateTransition = true;
			HideoutCinematicController.OnHideoutCinematicTransition transitionCallback = this._transitionCallback;
			if (transitionCallback == null)
			{
				return;
			}
			transitionCallback(this.State, this._remainingTransitionDuration);
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00012D78 File Offset: 0x00010F78
		private bool CheckNavMeshValidity(ref Vec3 initial, ref Vec3 target)
		{
			Scene scene = base.Mission.Scene;
			bool flag = false;
			bool navigationMeshForPosition = scene.GetNavigationMeshForPosition(ref initial);
			bool navigationMeshForPosition2 = scene.GetNavigationMeshForPosition(ref target);
			if (navigationMeshForPosition && navigationMeshForPosition2)
			{
				WorldPosition worldPosition;
				worldPosition..ctor(scene, initial);
				WorldPosition worldPosition2;
				worldPosition2..ctor(scene, target);
				flag = scene.DoesPathExistBetweenPositions(worldPosition, worldPosition2);
			}
			return flag;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00012DD0 File Offset: 0x00010FD0
		private void ComputeAgentFrames(Agent playerAgent, List<Agent> allyAgents, Agent bossAgent, List<Agent> banditAgents, float placementPerturbation, float placementAngle)
		{
			this._hideoutAgentsInfo = new List<HideoutCinematicController.HideoutCinematicAgentInfo>();
			MatrixFrame matrixFrame;
			MatrixFrame matrixFrame2;
			this._hideoutBossFightBehavior.GetPlayerFrames(out matrixFrame, out matrixFrame2, placementPerturbation);
			this._playerAgentInfo = new HideoutCinematicController.HideoutCinematicAgentInfo(playerAgent, HideoutCinematicController.HideoutAgentType.Player, matrixFrame, matrixFrame2);
			this._hideoutAgentsInfo.Add(this._playerAgentInfo);
			List<MatrixFrame> list;
			List<MatrixFrame> list2;
			this._hideoutBossFightBehavior.GetAllyFrames(out list, out list2, allyAgents.Count, placementAngle, placementPerturbation);
			for (int i = 0; i < allyAgents.Count; i++)
			{
				matrixFrame = list[i];
				matrixFrame2 = list2[i];
				this._hideoutAgentsInfo.Add(new HideoutCinematicController.HideoutCinematicAgentInfo(allyAgents[i], HideoutCinematicController.HideoutAgentType.Ally, matrixFrame, matrixFrame2));
			}
			this._hideoutBossFightBehavior.GetBossFrames(out matrixFrame, out matrixFrame2, placementPerturbation);
			this._bossAgentInfo = new HideoutCinematicController.HideoutCinematicAgentInfo(bossAgent, HideoutCinematicController.HideoutAgentType.Boss, matrixFrame, matrixFrame2);
			this._hideoutAgentsInfo.Add(this._bossAgentInfo);
			this._hideoutBossFightBehavior.GetBanditFrames(out list, out list2, banditAgents.Count, placementAngle, placementPerturbation);
			for (int j = 0; j < banditAgents.Count; j++)
			{
				matrixFrame = list[j];
				matrixFrame2 = list2[j];
				this._hideoutAgentsInfo.Add(new HideoutCinematicController.HideoutCinematicAgentInfo(banditAgents[j], HideoutCinematicController.HideoutAgentType.Bandit, matrixFrame, matrixFrame2));
			}
		}

		// Token: 0x0400015D RID: 349
		private const float AgentTargetProximityThreshold = 0.5f;

		// Token: 0x0400015E RID: 350
		private const float AgentMaxSpeedCinematicOverride = 0.65f;

		// Token: 0x0400015F RID: 351
		public const string HideoutSceneEntityTag = "hideout_boss_fight";

		// Token: 0x04000160 RID: 352
		public const float DefaultTransitionDuration = 0.4f;

		// Token: 0x04000161 RID: 353
		public const float DefaultStateDuration = 0.2f;

		// Token: 0x04000162 RID: 354
		public const float DefaultCinematicDuration = 8f;

		// Token: 0x04000163 RID: 355
		public const float DefaultPlacementPerturbation = 0.25f;

		// Token: 0x04000164 RID: 356
		public const float DefaultPlacementAngle = 0.20943952f;

		// Token: 0x04000165 RID: 357
		private HideoutCinematicController.OnHideoutCinematicFinished _cinematicFinishedCallback;

		// Token: 0x04000166 RID: 358
		private HideoutCinematicController.OnHideoutCinematicStateChanged _stateChangedCallback;

		// Token: 0x04000167 RID: 359
		private HideoutCinematicController.OnHideoutCinematicTransition _transitionCallback;

		// Token: 0x04000168 RID: 360
		private float _cinematicDuration = 8f;

		// Token: 0x04000169 RID: 361
		private float _stateDuration = 0.2f;

		// Token: 0x0400016A RID: 362
		private float _transitionDuration = 0.4f;

		// Token: 0x0400016B RID: 363
		private float _remainingCinematicDuration = 8f;

		// Token: 0x0400016C RID: 364
		private float _remainingStateDuration = 0.2f;

		// Token: 0x0400016D RID: 365
		private float _remainingTransitionDuration = 0.4f;

		// Token: 0x0400016E RID: 366
		private List<Formation> _cachedAgentFormations;

		// Token: 0x0400016F RID: 367
		private List<HideoutCinematicController.HideoutCinematicAgentInfo> _hideoutAgentsInfo;

		// Token: 0x04000170 RID: 368
		private HideoutCinematicController.HideoutCinematicAgentInfo _bossAgentInfo;

		// Token: 0x04000171 RID: 369
		private HideoutCinematicController.HideoutCinematicAgentInfo _playerAgentInfo;

		// Token: 0x04000172 RID: 370
		private bool _isBehaviorInit;

		// Token: 0x04000173 RID: 371
		private HideoutCinematicController.HideoutPreCinematicPhase _preCinematicPhase;

		// Token: 0x04000174 RID: 372
		private HideoutCinematicController.HideoutPostCinematicPhase _postCinematicPhase;

		// Token: 0x04000175 RID: 373
		private HideoutBossFightBehavior _hideoutBossFightBehavior;

		// Token: 0x02000110 RID: 272
		// (Invoke) Token: 0x06000CC1 RID: 3265
		public delegate void OnHideoutCinematicFinished();

		// Token: 0x02000111 RID: 273
		// (Invoke) Token: 0x06000CC5 RID: 3269
		public delegate void OnHideoutCinematicStateChanged(HideoutCinematicController.HideoutCinematicState state);

		// Token: 0x02000112 RID: 274
		// (Invoke) Token: 0x06000CC9 RID: 3273
		public delegate void OnHideoutCinematicTransition(HideoutCinematicController.HideoutCinematicState nextState, float duration);

		// Token: 0x02000113 RID: 275
		public readonly struct HideoutCinematicAgentInfo
		{
			// Token: 0x06000CCC RID: 3276 RVA: 0x00061F70 File Offset: 0x00060170
			public HideoutCinematicAgentInfo(Agent agent, HideoutCinematicController.HideoutAgentType type, in MatrixFrame initialFrame, in MatrixFrame targetFrame)
			{
				this.Agent = agent;
				this.InitialFrame = initialFrame;
				this.TargetFrame = targetFrame;
				this.Type = type;
			}

			// Token: 0x06000CCD RID: 3277 RVA: 0x00061F9C File Offset: 0x0006019C
			public bool HasReachedTarget(float proximityThreshold = 0.5f)
			{
				return this.Agent.Position.Distance(this.TargetFrame.origin) <= proximityThreshold;
			}

			// Token: 0x0400054B RID: 1355
			public readonly Agent Agent;

			// Token: 0x0400054C RID: 1356
			public readonly MatrixFrame InitialFrame;

			// Token: 0x0400054D RID: 1357
			public readonly MatrixFrame TargetFrame;

			// Token: 0x0400054E RID: 1358
			public readonly HideoutCinematicController.HideoutAgentType Type;
		}

		// Token: 0x02000114 RID: 276
		public enum HideoutCinematicState
		{
			// Token: 0x04000550 RID: 1360
			None,
			// Token: 0x04000551 RID: 1361
			PreCinematic,
			// Token: 0x04000552 RID: 1362
			Cinematic,
			// Token: 0x04000553 RID: 1363
			PostCinematic,
			// Token: 0x04000554 RID: 1364
			Completed
		}

		// Token: 0x02000115 RID: 277
		public enum HideoutAgentType
		{
			// Token: 0x04000556 RID: 1366
			Player,
			// Token: 0x04000557 RID: 1367
			Boss,
			// Token: 0x04000558 RID: 1368
			Ally,
			// Token: 0x04000559 RID: 1369
			Bandit
		}

		// Token: 0x02000116 RID: 278
		public enum HideoutPreCinematicPhase
		{
			// Token: 0x0400055B RID: 1371
			NotStarted,
			// Token: 0x0400055C RID: 1372
			InitializeFormations,
			// Token: 0x0400055D RID: 1373
			StopFormations,
			// Token: 0x0400055E RID: 1374
			InitializeAgents,
			// Token: 0x0400055F RID: 1375
			MoveAgents,
			// Token: 0x04000560 RID: 1376
			Completed
		}

		// Token: 0x02000117 RID: 279
		public enum HideoutPostCinematicPhase
		{
			// Token: 0x04000562 RID: 1378
			NotStarted,
			// Token: 0x04000563 RID: 1379
			MoveAgents,
			// Token: 0x04000564 RID: 1380
			FinalizeAgents,
			// Token: 0x04000565 RID: 1381
			Completed
		}
	}
}
