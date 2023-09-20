using System;
using System.Collections.Generic;
using SandBox.Objects.Cinematics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class HideoutCinematicController : MissionLogic
	{
		public HideoutCinematicController.HideoutCinematicState State { get; private set; }

		public bool InStateTransition { get; private set; }

		public bool IsCinematicActive
		{
			get
			{
				return this.State > HideoutCinematicController.HideoutCinematicState.None;
			}
		}

		public float CinematicDuration
		{
			get
			{
				return this._cinematicDuration;
			}
		}

		public float TransitionDuration
		{
			get
			{
				return this._transitionDuration;
			}
		}

		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return 0;
			}
		}

		public HideoutCinematicController()
		{
			this.State = HideoutCinematicController.HideoutCinematicState.None;
			this._cinematicFinishedCallback = null;
			this._transitionCallback = null;
			this._stateChangedCallback = null;
			this.InStateTransition = false;
			this._isBehaviorInit = false;
		}

		public void SetStateTransitionCallback(HideoutCinematicController.OnHideoutCinematicStateChanged onStateChanged, HideoutCinematicController.OnHideoutCinematicTransition onTransition)
		{
			this._stateChangedCallback = onStateChanged;
			this._transitionCallback = onTransition;
		}

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

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("hideout_boss_fight");
			this._hideoutBossFightBehavior = ((gameEntity != null) ? gameEntity.GetFirstScriptOfType<HideoutBossFightBehavior>() : null);
			this._isBehaviorInit = gameEntity != null && this._hideoutBossFightBehavior != null;
		}

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

		private bool TickCinematic(float dt)
		{
			this._remainingCinematicDuration -= dt;
			this._remainingStateDuration -= dt;
			return this._remainingCinematicDuration <= 0f && this._remainingStateDuration <= 0f;
		}

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

		private const float AgentTargetProximityThreshold = 0.5f;

		private const float AgentMaxSpeedCinematicOverride = 0.65f;

		public const string HideoutSceneEntityTag = "hideout_boss_fight";

		public const float DefaultTransitionDuration = 0.4f;

		public const float DefaultStateDuration = 0.2f;

		public const float DefaultCinematicDuration = 8f;

		public const float DefaultPlacementPerturbation = 0.25f;

		public const float DefaultPlacementAngle = 0.20943952f;

		private HideoutCinematicController.OnHideoutCinematicFinished _cinematicFinishedCallback;

		private HideoutCinematicController.OnHideoutCinematicStateChanged _stateChangedCallback;

		private HideoutCinematicController.OnHideoutCinematicTransition _transitionCallback;

		private float _cinematicDuration = 8f;

		private float _stateDuration = 0.2f;

		private float _transitionDuration = 0.4f;

		private float _remainingCinematicDuration = 8f;

		private float _remainingStateDuration = 0.2f;

		private float _remainingTransitionDuration = 0.4f;

		private List<Formation> _cachedAgentFormations;

		private List<HideoutCinematicController.HideoutCinematicAgentInfo> _hideoutAgentsInfo;

		private HideoutCinematicController.HideoutCinematicAgentInfo _bossAgentInfo;

		private HideoutCinematicController.HideoutCinematicAgentInfo _playerAgentInfo;

		private bool _isBehaviorInit;

		private HideoutCinematicController.HideoutPreCinematicPhase _preCinematicPhase;

		private HideoutCinematicController.HideoutPostCinematicPhase _postCinematicPhase;

		private HideoutBossFightBehavior _hideoutBossFightBehavior;

		public delegate void OnHideoutCinematicFinished();

		public delegate void OnHideoutCinematicStateChanged(HideoutCinematicController.HideoutCinematicState state);

		public delegate void OnHideoutCinematicTransition(HideoutCinematicController.HideoutCinematicState nextState, float duration);

		public readonly struct HideoutCinematicAgentInfo
		{
			public HideoutCinematicAgentInfo(Agent agent, HideoutCinematicController.HideoutAgentType type, in MatrixFrame initialFrame, in MatrixFrame targetFrame)
			{
				this.Agent = agent;
				this.InitialFrame = initialFrame;
				this.TargetFrame = targetFrame;
				this.Type = type;
			}

			public bool HasReachedTarget(float proximityThreshold = 0.5f)
			{
				return this.Agent.Position.Distance(this.TargetFrame.origin) <= proximityThreshold;
			}

			public readonly Agent Agent;

			public readonly MatrixFrame InitialFrame;

			public readonly MatrixFrame TargetFrame;

			public readonly HideoutCinematicController.HideoutAgentType Type;
		}

		public enum HideoutCinematicState
		{
			None,
			PreCinematic,
			Cinematic,
			PostCinematic,
			Completed
		}

		public enum HideoutAgentType
		{
			Player,
			Boss,
			Ally,
			Bandit
		}

		public enum HideoutPreCinematicPhase
		{
			NotStarted,
			InitializeFormations,
			StopFormations,
			InitializeAgents,
			MoveAgents,
			Completed
		}

		public enum HideoutPostCinematicPhase
		{
			NotStarted,
			MoveAgents,
			FinalizeAgents,
			Completed
		}
	}
}
