using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public sealed class Agent : DotNetObject, IAgent, IFocusable, IUsable, IFormationUnit, ITrackableBase
	{
		public static Agent Main
		{
			get
			{
				Mission mission = Mission.Current;
				if (mission == null)
				{
					return null;
				}
				return mission.MainAgent;
			}
		}

		public event Agent.OnAgentHealthChangedDelegate OnAgentHealthChanged;

		public event Agent.OnMountHealthChangedDelegate OnMountHealthChanged;

		public bool IsPlayerControlled
		{
			get
			{
				return this.IsMine || this.MissionPeer != null;
			}
		}

		public bool IsMine
		{
			get
			{
				return this.Controller == Agent.ControllerType.Player;
			}
		}

		public bool IsMainAgent
		{
			get
			{
				return this == Agent.Main;
			}
		}

		public bool IsHuman
		{
			get
			{
				return (this.GetAgentFlags() & AgentFlag.IsHumanoid) > AgentFlag.None;
			}
		}

		public bool IsMount
		{
			get
			{
				return (this.GetAgentFlags() & AgentFlag.Mountable) > AgentFlag.None;
			}
		}

		public bool IsAIControlled
		{
			get
			{
				return this.Controller == Agent.ControllerType.AI && !GameNetwork.IsClientOrReplay;
			}
		}

		public bool IsPlayerTroop
		{
			get
			{
				return !GameNetwork.IsMultiplayer && this.Origin != null && this.Origin.Troop == Game.Current.PlayerTroop;
			}
		}

		public bool IsUsingGameObject
		{
			get
			{
				return this.CurrentlyUsedGameObject != null;
			}
		}

		public bool CanLeadFormationsRemotely
		{
			get
			{
				return this._canLeadFormationsRemotely;
			}
		}

		public bool IsDetachableFromFormation
		{
			get
			{
				return this._isDetachableFromFormation;
			}
		}

		public float AgentScale
		{
			get
			{
				return MBAPI.IMBAgent.GetAgentScale(this.GetPtr());
			}
		}

		public bool CrouchMode
		{
			get
			{
				return MBAPI.IMBAgent.GetCrouchMode(this.GetPtr());
			}
		}

		public bool WalkMode
		{
			get
			{
				return MBAPI.IMBAgent.GetWalkMode(this.GetPtr());
			}
		}

		public Vec3 Position
		{
			get
			{
				return AgentHelper.GetAgentPosition(this.PositionPointer);
			}
		}

		public Vec3 VisualPosition
		{
			get
			{
				return MBAPI.IMBAgent.GetVisualPosition(this.GetPtr());
			}
		}

		public Vec2 MovementVelocity
		{
			get
			{
				return MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr());
			}
		}

		public Vec3 AverageVelocity
		{
			get
			{
				return MBAPI.IMBAgent.GetAverageVelocity(this.GetPtr());
			}
		}

		public float MaximumForwardUnlimitedSpeed
		{
			get
			{
				return MBAPI.IMBAgent.GetMaximumForwardUnlimitedSpeed(this.GetPtr());
			}
		}

		public float MovementDirectionAsAngle
		{
			get
			{
				return MBAPI.IMBAgent.GetMovementDirectionAsAngle(this.GetPtr());
			}
		}

		public bool IsLookRotationInSlowMotion
		{
			get
			{
				return MBAPI.IMBAgent.IsLookRotationInSlowMotion(this.GetPtr());
			}
		}

		public Agent.AgentPropertiesModifiers PropertyModifiers
		{
			get
			{
				return this._propertyModifiers;
			}
		}

		public MBActionSet ActionSet
		{
			get
			{
				return new MBActionSet(MBAPI.IMBAgent.GetActionSetNo(this.GetPtr()));
			}
		}

		public MBReadOnlyList<AgentComponent> Components
		{
			get
			{
				return this._components;
			}
		}

		public MBReadOnlyList<Agent.Hitter> HitterList
		{
			get
			{
				return this._hitterList;
			}
		}

		public Agent.GuardMode CurrentGuardMode
		{
			get
			{
				return MBAPI.IMBAgent.GetCurrentGuardMode(this.GetPtr());
			}
		}

		public Agent ImmediateEnemy
		{
			get
			{
				return MBAPI.IMBAgent.GetImmediateEnemy(this.GetPtr());
			}
		}

		public bool IsDoingPassiveAttack
		{
			get
			{
				return MBAPI.IMBAgent.GetIsDoingPassiveAttack(this.GetPtr());
			}
		}

		public bool IsPassiveUsageConditionsAreMet
		{
			get
			{
				return MBAPI.IMBAgent.GetIsPassiveUsageConditionsAreMet(this.GetPtr());
			}
		}

		public float CurrentAimingError
		{
			get
			{
				return MBAPI.IMBAgent.GetCurrentAimingError(this.GetPtr());
			}
		}

		public float CurrentAimingTurbulance
		{
			get
			{
				return MBAPI.IMBAgent.GetCurrentAimingTurbulance(this.GetPtr());
			}
		}

		public Agent.UsageDirection AttackDirection
		{
			get
			{
				return MBAPI.IMBAgent.GetAttackDirectionUsage(this.GetPtr());
			}
		}

		public float WalkingSpeedLimitOfMountable
		{
			get
			{
				return MBAPI.IMBAgent.GetWalkSpeedLimitOfMountable(this.GetPtr());
			}
		}

		public Agent RiderAgent
		{
			get
			{
				return this.GetRiderAgentAux();
			}
		}

		public bool HasMount
		{
			get
			{
				return this.MountAgent != null;
			}
		}

		public bool CanLogCombatFor
		{
			get
			{
				return (this.RiderAgent != null && !this.RiderAgent.IsAIControlled) || (!this.IsMount && !this.IsAIControlled);
			}
		}

		public float MissileRangeAdjusted
		{
			get
			{
				return this.GetMissileRangeWithHeightDifference();
			}
		}

		public float MaximumMissileRange
		{
			get
			{
				return this.GetMissileRange();
			}
		}

		FocusableObjectType IFocusable.FocusableObjectType
		{
			get
			{
				if (!this.IsMount)
				{
					return FocusableObjectType.Agent;
				}
				return FocusableObjectType.Mount;
			}
		}

		public string Name
		{
			get
			{
				if (this.MissionPeer == null)
				{
					return this._name.ToString();
				}
				return this.MissionPeer.Name;
			}
		}

		public AgentMovementLockedState MovementLockedState
		{
			get
			{
				return this.GetMovementLockedState();
			}
		}

		public Monster Monster { get; }

		public bool IsRunningAway { get; private set; }

		public BodyProperties BodyPropertiesValue { get; private set; }

		public CommonAIComponent CommonAIComponent { get; private set; }

		public HumanAIComponent HumanAIComponent { get; private set; }

		public int BodyPropertiesSeed { get; internal set; }

		public float LastRangedHitTime { get; private set; } = float.MinValue;

		public float LastMeleeHitTime { get; private set; } = float.MinValue;

		public float LastRangedAttackTime { get; private set; } = float.MinValue;

		public float LastMeleeAttackTime { get; private set; } = float.MinValue;

		public bool IsFemale { get; set; }

		public ItemObject Banner
		{
			get
			{
				MissionEquipment equipment = this.Equipment;
				if (equipment == null)
				{
					return null;
				}
				return equipment.GetBanner();
			}
		}

		public ItemObject FormationBanner
		{
			get
			{
				return this._formationBanner;
			}
		}

		public MissionWeapon WieldedWeapon
		{
			get
			{
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				if (wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot)
				{
					return MissionWeapon.Invalid;
				}
				return this.Equipment[wieldedItemIndex];
			}
		}

		public bool IsItemUseDisabled { get; set; }

		public bool SyncHealthToAllClients { get; private set; }

		public UsableMissionObject CurrentlyUsedGameObject { get; private set; }

		public bool CombatActionsEnabled
		{
			get
			{
				return this.CurrentlyUsedGameObject == null || !this.CurrentlyUsedGameObject.DisableCombatActionsOnUse;
			}
		}

		public Mission Mission { get; private set; }

		public bool IsHero
		{
			get
			{
				return this.Character != null && this.Character.IsHero;
			}
		}

		public int Index { get; }

		public MissionEquipment Equipment { get; private set; }

		public TextObject AgentRole { get; set; }

		public bool HasBeenBuilt { get; private set; }

		public Agent.MortalityState CurrentMortalityState { get; private set; }

		public Equipment SpawnEquipment { get; private set; }

		public FormationPositionPreference FormationPositionPreference { get; set; }

		public bool RandomizeColors { get; private set; }

		public float CharacterPowerCached { get; private set; }

		public float WalkSpeedCached { get; private set; }

		public float RunSpeedCached { get; private set; }

		public IAgentOriginBase Origin { get; set; }

		public Team Team { get; private set; }

		public int KillCount { get; set; }

		public AgentDrivenProperties AgentDrivenProperties { get; private set; }

		public float BaseHealthLimit { get; set; }

		public string HorseCreationKey { get; private set; }

		public float HealthLimit { get; set; }

		public bool IsRangedCached
		{
			get
			{
				return this.Equipment.ContainsNonConsumableRangedWeaponWithAmmo();
			}
		}

		public bool HasMeleeWeaponCached
		{
			get
			{
				return this.Equipment.ContainsMeleeWeapon();
			}
		}

		public bool HasShieldCached
		{
			get
			{
				return this.Equipment.ContainsShield();
			}
		}

		public bool HasSpearCached
		{
			get
			{
				return this.Equipment.ContainsSpear();
			}
		}

		public bool HasThrownCached
		{
			get
			{
				return this.Equipment.ContainsThrownWeapon();
			}
		}

		public Agent.AIStateFlag AIStateFlags
		{
			get
			{
				return MBAPI.IMBAgent.GetAIStateFlags(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetAIStateFlags(this.GetPtr(), value);
			}
		}

		public MatrixFrame Frame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				MBAPI.IMBAgent.GetRotationFrame(this.GetPtr(), ref matrixFrame);
				return matrixFrame;
			}
		}

		public Agent.MovementControlFlag MovementFlags
		{
			get
			{
				return (Agent.MovementControlFlag)MBAPI.IMBAgent.GetMovementFlags(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetMovementFlags(this.GetPtr(), value);
			}
		}

		public Vec2 MovementInputVector
		{
			get
			{
				return MBAPI.IMBAgent.GetMovementInputVector(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetMovementInputVector(this.GetPtr(), value);
			}
		}

		public CapsuleData CollisionCapsule
		{
			get
			{
				CapsuleData capsuleData = default(CapsuleData);
				MBAPI.IMBAgent.GetCollisionCapsule(this.GetPtr(), ref capsuleData);
				return capsuleData;
			}
		}

		public Vec3 CollisionCapsuleCenter
		{
			get
			{
				CapsuleData collisionCapsule = this.CollisionCapsule;
				return (collisionCapsule.GetBoxMax() + collisionCapsule.GetBoxMin()) * 0.5f;
			}
		}

		public MBAgentVisuals AgentVisuals
		{
			get
			{
				MBAgentVisuals agentVisuals;
				if (!this._visualsWeakRef.TryGetTarget(out agentVisuals))
				{
					agentVisuals = MBAPI.IMBAgent.GetAgentVisuals(this.GetPtr());
					this._visualsWeakRef.SetTarget(agentVisuals);
				}
				return agentVisuals;
			}
		}

		public bool HeadCameraMode
		{
			get
			{
				return MBAPI.IMBAgent.GetHeadCameraMode(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetHeadCameraMode(this.GetPtr(), value);
			}
		}

		public Agent MountAgent
		{
			get
			{
				return this.GetMountAgentAux();
			}
			private set
			{
				this.SetMountAgent(value);
				this.UpdateAgentStats();
			}
		}

		public IDetachment Detachment
		{
			get
			{
				return this._detachment;
			}
			set
			{
				this._detachment = value;
				if (this._detachment != null)
				{
					Formation formation = this.Formation;
					if (formation == null)
					{
						return;
					}
					formation.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
				}
			}
		}

		public bool IsPaused
		{
			get
			{
				return this.AIStateFlags.HasAnyFlag(Agent.AIStateFlag.Paused);
			}
			set
			{
				if (value)
				{
					this.AIStateFlags |= Agent.AIStateFlag.Paused;
					return;
				}
				this.AIStateFlags &= ~Agent.AIStateFlag.Paused;
			}
		}

		public bool IsDetachedFromFormation
		{
			get
			{
				return this._detachment != null;
			}
		}

		public Agent.WatchState CurrentWatchState
		{
			get
			{
				Agent.AIStateFlag aistateFlags = this.AIStateFlags;
				if ((aistateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.Alarmed)
				{
					return Agent.WatchState.Alarmed;
				}
				if ((aistateFlags & Agent.AIStateFlag.Cautious) == Agent.AIStateFlag.Cautious)
				{
					return Agent.WatchState.Cautious;
				}
				return Agent.WatchState.Patrolling;
			}
			private set
			{
				Agent.AIStateFlag aistateFlag = this.AIStateFlags;
				switch (value)
				{
				case Agent.WatchState.Patrolling:
					aistateFlag &= ~(Agent.AIStateFlag.Cautious | Agent.AIStateFlag.Alarmed);
					break;
				case Agent.WatchState.Cautious:
					aistateFlag |= Agent.AIStateFlag.Cautious;
					aistateFlag &= ~Agent.AIStateFlag.Alarmed;
					break;
				case Agent.WatchState.Alarmed:
					aistateFlag |= Agent.AIStateFlag.Alarmed;
					aistateFlag &= ~Agent.AIStateFlag.Cautious;
					break;
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", "CurrentWatchState", 900);
					break;
				}
				this.AIStateFlags = aistateFlag;
			}
		}

		public float Defensiveness
		{
			get
			{
				return this._defensiveness;
			}
			set
			{
				if (MathF.Abs(value - this._defensiveness) > 0.0001f)
				{
					this._defensiveness = value;
					this.UpdateAgentProperties();
				}
			}
		}

		public Formation Formation
		{
			get
			{
				return this._formation;
			}
			set
			{
				if (this._formation != value)
				{
					if (GameNetwork.IsServer && this.HasBeenBuilt && this.Mission.GetMissionBehavior<MissionNetworkComponent>() != null)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new AgentSetFormation(this.Index, (value != null) ? value.Index : (-1)));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
					this.SetNativeFormationNo((value != null) ? value.Index : (-1));
					IDetachment detachment = null;
					float num = 0f;
					if (this._formation != null)
					{
						if (this.IsDetachedFromFormation)
						{
							detachment = this.Detachment;
							num = this.DetachmentWeight;
						}
						this._formation.RemoveUnit(this);
					}
					this._formation = value;
					if (this._formation != null)
					{
						if (!this._formation.HasBeenPositioned)
						{
							this._formation.SetPositioning(new WorldPosition?(this.GetWorldPosition()), new Vec2?(this.LookDirection.AsVec2), null);
						}
						this._formation.AddUnit(this);
						if (detachment != null && this._formation.Detachments.IndexOf(detachment) >= 0 && detachment.IsStandingPointAvailableForAgent(this))
						{
							detachment.AddAgent(this, -1);
							this._formation.DetachUnit(this, detachment.IsLoose);
							this.Detachment = detachment;
							this.DetachmentWeight = num;
						}
					}
					this.UpdateCachedAndFormationValues(this._formation != null && this._formation.PostponeCostlyOperations, false);
				}
			}
		}

		IFormationUnit IFormationUnit.FollowedUnit
		{
			get
			{
				if (!this.IsActive())
				{
					return null;
				}
				if (this.IsAIControlled)
				{
					return this.GetFollowedUnit();
				}
				return null;
			}
		}

		public bool IsShieldUsageEncouraged
		{
			get
			{
				return this.Formation.FiringOrder.OrderEnum == FiringOrder.RangedWeaponUsageOrderEnum.HoldYourFire || !this.Equipment.HasAnyWeaponWithFlags(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand);
			}
		}

		public bool IsPlayerUnit
		{
			get
			{
				return this.IsPlayerControlled || this.IsPlayerTroop;
			}
		}

		public Agent.ControllerType Controller
		{
			get
			{
				return this.GetController();
			}
			set
			{
				Agent.ControllerType controller = this.Controller;
				if (value != controller)
				{
					this.SetController(value);
					if (value == Agent.ControllerType.Player)
					{
						this.Mission.MainAgent = this;
						this.SetAgentFlags(this.GetAgentFlags() | AgentFlag.CanRide);
					}
					if (this.Formation != null)
					{
						this.Formation.OnAgentControllerChanged(this, controller);
					}
					if (value != Agent.ControllerType.AI && this.GetAgentFlags().HasAnyFlag(AgentFlag.IsHumanoid))
					{
						this.SetMaximumSpeedLimit(-1f, false);
						if (this.WalkMode)
						{
							this.EventControlFlags |= Agent.EventControlFlag.Run;
						}
					}
					foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
					{
						missionBehavior.OnAgentControllerChanged(this, controller);
					}
					if (GameNetwork.IsServer)
					{
						MissionPeer missionPeer = this.MissionPeer;
						NetworkCommunicator networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
						if (networkCommunicator != null && !networkCommunicator.IsServerPeer)
						{
							GameNetwork.BeginModuleEventAsServer(networkCommunicator);
							GameNetwork.WriteMessage(new SetAgentIsPlayer(this.Index, this.Controller != Agent.ControllerType.AI));
							GameNetwork.EndModuleEventAsServer();
						}
					}
				}
			}
		}

		public uint ClothingColor1
		{
			get
			{
				if (this._clothingColor1 != null)
				{
					return this._clothingColor1.Value;
				}
				if (this.Team != null)
				{
					return this.Team.Color;
				}
				Debug.FailedAssert("Clothing color is not set.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", "ClothingColor1", 1098);
				return uint.MaxValue;
			}
		}

		public uint ClothingColor2
		{
			get
			{
				if (this._clothingColor2 != null)
				{
					return this._clothingColor2.Value;
				}
				return this.ClothingColor1;
			}
		}

		public MatrixFrame LookFrame
		{
			get
			{
				return new MatrixFrame
				{
					origin = this.Position,
					rotation = this.LookRotation
				};
			}
		}

		public float LookDirectionAsAngle
		{
			get
			{
				return MBAPI.IMBAgent.GetLookDirectionAsAngle(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetLookDirectionAsAngle(this.GetPtr(), value);
			}
		}

		public Mat3 LookRotation
		{
			get
			{
				Mat3 mat;
				mat.f = this.LookDirection;
				mat.u = Vec3.Up;
				mat.s = Vec3.CrossProduct(mat.f, mat.u);
				mat.s.Normalize();
				mat.u = Vec3.CrossProduct(mat.s, mat.f);
				return mat;
			}
		}

		public bool IsLookDirectionLocked
		{
			get
			{
				return MBAPI.IMBAgent.GetIsLookDirectionLocked(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetIsLookDirectionLocked(this.GetPtr(), value);
			}
		}

		public bool IsCheering
		{
			get
			{
				ActionIndexValueCache currentActionValue = this.GetCurrentActionValue(1);
				for (int i = 0; i < Agent.DefaultTauntActions.Length; i++)
				{
					if (Agent.DefaultTauntActions[i] != null && Agent.DefaultTauntActions[i] == currentActionValue)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsInBeingStruckAction
		{
			get
			{
				return MBMath.IsBetween((int)this.GetCurrentActionType(1), 47, 51) || MBMath.IsBetween((int)this.GetCurrentActionType(0), 47, 51);
			}
		}

		public MissionPeer MissionPeer
		{
			get
			{
				return this._missionPeer;
			}
			set
			{
				if (this._missionPeer != value)
				{
					MissionPeer missionPeer = this._missionPeer;
					this._missionPeer = value;
					if (missionPeer != null && missionPeer.ControlledAgent == this)
					{
						missionPeer.ControlledAgent = null;
					}
					if (this._missionPeer != null && this._missionPeer.ControlledAgent != this)
					{
						this._missionPeer.ControlledAgent = this;
						if (GameNetwork.IsServerOrRecorder)
						{
							this.SyncHealthToClients();
							Agent.OnAgentHealthChangedDelegate onAgentHealthChanged = this.OnAgentHealthChanged;
							if (onAgentHealthChanged != null)
							{
								onAgentHealthChanged(this, this.Health, this.Health);
							}
						}
					}
					if (value != null)
					{
						this.Controller = (value.IsMine ? Agent.ControllerType.Player : Agent.ControllerType.None);
					}
					if (GameNetwork.IsServer && this.IsHuman && !this._isDeleted)
					{
						NetworkCommunicator networkCommunicator = ((value != null) ? value.GetNetworkPeer() : null);
						this.SetNetworkPeer(networkCommunicator);
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetAgentPeer(this.Index, networkCommunicator));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
			}
		}

		public BasicCharacterObject Character
		{
			get
			{
				return this._character;
			}
			set
			{
				this._character = value;
				if (value != null)
				{
					this.Health = (float)this._character.HitPoints;
					this.BaseHealthLimit = (float)this._character.MaxHitPoints();
					this.HealthLimit = this.BaseHealthLimit;
					this.CharacterPowerCached = value.GetPower();
					this._name = value.Name;
					this.IsFemale = value.IsFemale;
				}
			}
		}

		IMissionTeam IAgent.Team
		{
			get
			{
				return this.Team;
			}
		}

		IFormationArrangement IFormationUnit.Formation
		{
			get
			{
				return this._formation.Arrangement;
			}
		}

		int IFormationUnit.FormationFileIndex { get; set; }

		int IFormationUnit.FormationRankIndex { get; set; }

		private UIntPtr Pointer
		{
			get
			{
				return this._pointer;
			}
		}

		private UIntPtr FlagsPointer
		{
			get
			{
				return this._flagsPointer;
			}
		}

		private UIntPtr PositionPointer
		{
			get
			{
				return this._positionPointer;
			}
		}

		internal Agent(Mission mission, Mission.AgentCreationResult creationResult, Agent.CreationType creationType, Monster monster)
		{
			this.AgentRole = TextObject.Empty;
			this.Mission = mission;
			this.Index = creationResult.Index;
			this._pointer = creationResult.AgentPtr;
			this._positionPointer = creationResult.PositionPtr;
			this._flagsPointer = creationResult.FlagsPtr;
			this._indexPointer = creationResult.IndexPtr;
			this._statePointer = creationResult.StatePtr;
			this._lastHitInfo = default(Agent.AgentLastHitInfo);
			this._lastHitInfo.Initialize();
			MBAPI.IMBAgent.SetMonoObject(this.GetPtr(), this);
			this.Monster = monster;
			this.KillCount = 0;
			this.HasBeenBuilt = false;
			this._creationType = creationType;
			this._agentControllers = new List<AgentController>();
			this._components = new MBList<AgentComponent>();
			this._hitterList = new MBList<Agent.Hitter>();
			((IFormationUnit)this).FormationFileIndex = -1;
			((IFormationUnit)this).FormationRankIndex = -1;
			this._synchedBodyComponents = null;
			this._cachedAndFormationValuesUpdateTimer = new Timer(this.Mission.CurrentTime, 0.45f + MBRandom.RandomFloat * 0.1f, true);
		}

		public Vec3 LookDirection
		{
			get
			{
				return MBAPI.IMBAgent.GetLookDirection(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetLookDirection(this.GetPtr(), value);
			}
		}

		bool IAgent.IsEnemyOf(IAgent agent)
		{
			return this.IsEnemyOf((Agent)agent);
		}

		bool IAgent.IsFriendOf(IAgent agent)
		{
			return this.IsFriendOf((Agent)agent);
		}

		public float Health
		{
			get
			{
				return this._health;
			}
			set
			{
				float num = (float)(value.ApproximatelyEqualsTo(0f, 1E-05f) ? 0 : MathF.Ceiling(value));
				if (!this._health.ApproximatelyEqualsTo(num, 1E-05f))
				{
					float health = this._health;
					this._health = num;
					if (GameNetwork.IsServerOrRecorder)
					{
						this.SyncHealthToClients();
					}
					Agent.OnAgentHealthChangedDelegate onAgentHealthChanged = this.OnAgentHealthChanged;
					if (onAgentHealthChanged != null)
					{
						onAgentHealthChanged(this, health, this._health);
					}
					if (this.RiderAgent != null)
					{
						Agent.OnMountHealthChangedDelegate onMountHealthChanged = this.RiderAgent.OnMountHealthChanged;
						if (onMountHealthChanged == null)
						{
							return;
						}
						onMountHealthChanged(this.RiderAgent, this, health, this._health);
					}
				}
			}
		}

		public float Age
		{
			get
			{
				return this.BodyPropertiesValue.Age;
			}
			set
			{
				this.BodyPropertiesValue = new BodyProperties(new DynamicBodyProperties(value, this.BodyPropertiesValue.Weight, this.BodyPropertiesValue.Build), this.BodyPropertiesValue.StaticProperties);
				BodyProperties bodyPropertiesValue = this.BodyPropertiesValue;
				this.BodyPropertiesValue = bodyPropertiesValue;
			}
		}

		Vec3 ITrackableBase.GetPosition()
		{
			return this.Position;
		}

		public Vec3 Velocity
		{
			get
			{
				Vec2 movementVelocity = MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr());
				Vec3 vec = new Vec3(movementVelocity, 0f, -1f);
				return this.Frame.rotation.TransformToParent(vec);
			}
		}

		TextObject ITrackableBase.GetName()
		{
			if (this.Character != null)
			{
				return new TextObject(this.Character.Name.ToString(), null);
			}
			return TextObject.Empty;
		}

		[MBCallback]
		internal void SetAgentAIPerformingRetreatBehavior(bool isAgentAIPerformingRetreatBehavior)
		{
			if (!GameNetwork.IsClientOrReplay && this.Mission != null)
			{
				this.IsRunningAway = isAgentAIPerformingRetreatBehavior;
			}
		}

		public Agent.EventControlFlag EventControlFlags
		{
			get
			{
				return (Agent.EventControlFlag)MBAPI.IMBAgent.GetEventControlFlags(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetEventControlFlags(this.GetPtr(), value);
			}
		}

		[MBCallback]
		public float GetMissileRangeWithHeightDifferenceAux(float targetZ)
		{
			return MBAPI.IMBAgent.GetMissileRangeWithHeightDifference(this.GetPtr(), targetZ);
		}

		[MBCallback]
		internal int GetFormationUnitSpacing()
		{
			return this.Formation.UnitSpacing;
		}

		[MBCallback]
		public string GetSoundAndCollisionInfoClassName()
		{
			return this.Monster.SoundAndCollisionInfoClassName;
		}

		[MBCallback]
		internal bool IsInSameFormationWith(Agent otherAgent)
		{
			Formation formation = otherAgent.Formation;
			return this.Formation != null && formation != null && this.Formation == formation;
		}

		[MBCallback]
		internal void OnWeaponSwitchingToAlternativeStart(EquipmentIndex slotIndex, int usageIndex)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new StartSwitchingWeaponUsageIndex(this.Index, slotIndex, usageIndex, Agent.MovementFlagToDirection(this.MovementFlags)));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		[MBCallback]
		internal void OnWeaponReloadPhaseChange(EquipmentIndex slotIndex, short reloadPhase)
		{
			this.Equipment.SetReloadPhaseOfSlot(slotIndex, reloadPhase);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWeaponReloadPhase(this.Index, slotIndex, reloadPhase));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		[MBCallback]
		internal void OnWeaponAmmoReload(EquipmentIndex slotIndex, EquipmentIndex ammoSlotIndex, short totalAmmo)
		{
			if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
			{
				this.Equipment.SetReloadedAmmoOfSlot(slotIndex, ammoSlotIndex, totalAmmo);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetWeaponAmmoData(this.Index, slotIndex, ammoSlotIndex, totalAmmo));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			this.UpdateAgentProperties();
		}

		[MBCallback]
		internal void OnWeaponAmmoConsume(EquipmentIndex slotIndex, short totalAmmo)
		{
			if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
			{
				this.Equipment.SetConsumedAmmoOfSlot(slotIndex, totalAmmo);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetWeaponAmmoData(this.Index, slotIndex, EquipmentIndex.None, totalAmmo));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			this.UpdateAgentProperties();
		}

		public AgentState State
		{
			get
			{
				return AgentHelper.GetAgentState(this._statePointer);
			}
			set
			{
				if (this.State != value)
				{
					MBAPI.IMBAgent.SetStateFlags(this.GetPtr(), value);
				}
			}
		}

		[MBCallback]
		internal void OnShieldDamaged(EquipmentIndex slotIndex, int inflictedDamage)
		{
			int num = MathF.Max(0, (int)this.Equipment[slotIndex].HitPoints - inflictedDamage);
			this.ChangeWeaponHitPoints(slotIndex, (short)num);
			if (num == 0)
			{
				this.RemoveEquippedWeapon(slotIndex);
			}
		}

		public MissionWeapon WieldedOffhandWeapon
		{
			get
			{
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				if (wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot)
				{
					return MissionWeapon.Invalid;
				}
				return this.Equipment[wieldedItemIndex];
			}
		}

		[MBCallback]
		internal void OnWeaponAmmoRemoved(EquipmentIndex slotIndex)
		{
			if (!this.Equipment[slotIndex].AmmoWeapon.IsEmpty)
			{
				this.Equipment.SetConsumedAmmoOfSlot(slotIndex, 0);
			}
		}

		[MBCallback]
		internal void OnMount(Agent mount)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				if (mount.IsAIControlled && mount.IsRetreating(false))
				{
					mount.StopRetreatingMoraleComponent();
				}
				this.CheckToDropFlaggedItem();
			}
			if (this.HasBeenBuilt)
			{
				foreach (AgentComponent agentComponent in this._components)
				{
					agentComponent.OnMount(mount);
				}
				this.Mission.OnAgentMount(this);
			}
			this.UpdateAgentStats();
			Action onAgentMountedStateChanged = this.OnAgentMountedStateChanged;
			if (onAgentMountedStateChanged != null)
			{
				onAgentMountedStateChanged();
			}
			if (GameNetwork.IsServerOrRecorder)
			{
				mount.SyncHealthToClients();
			}
		}

		[MBCallback]
		internal void OnDismount(Agent mount)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				Formation formation = this.Formation;
				if (formation != null)
				{
					formation.OnAgentLostMount(this);
				}
				this.CheckToDropFlaggedItem();
			}
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnDismount(mount);
			}
			this.Mission.OnAgentDismount(this);
			if (this.IsActive())
			{
				this.UpdateAgentStats();
				Action onAgentMountedStateChanged = this.OnAgentMountedStateChanged;
				if (onAgentMountedStateChanged == null)
				{
					return;
				}
				onAgentMountedStateChanged();
			}
		}

		[MBCallback]
		internal void OnAgentAlarmedStateChanged(Agent.AIStateFlag flag)
		{
			foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
			{
				missionBehavior.OnAgentAlarmedStateChanged(this, flag);
			}
		}

		[MBCallback]
		internal void OnRetreating()
		{
			if (!GameNetwork.IsClientOrReplay && this.Mission != null && !this.Mission.MissionEnded)
			{
				if (this.IsUsingGameObject)
				{
					this.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
				foreach (AgentComponent agentComponent in this._components)
				{
					agentComponent.OnRetreating();
				}
			}
		}

		[MBCallback]
		internal void UpdateMountAgentCache(Agent newMountAgent)
		{
			this._cachedMountAgent = newMountAgent;
		}

		[MBCallback]
		internal void UpdateRiderAgentCache(Agent newRiderAgent)
		{
			this._cachedRiderAgent = newRiderAgent;
			if (newRiderAgent == null)
			{
				Mission.Current.AddMountWithoutRider(this);
				return;
			}
			Mission.Current.RemoveMountWithoutRider(this);
		}

		[MBCallback]
		public void UpdateAgentStats()
		{
			if (this.IsActive())
			{
				this.UpdateAgentProperties();
			}
		}

		[MBCallback]
		public float GetWeaponInaccuracy(EquipmentIndex weaponSlotIndex, int weaponUsageIndex)
		{
			WeaponComponentData weaponComponentDataForUsage = this.Equipment[weaponSlotIndex].GetWeaponComponentDataForUsage(weaponUsageIndex);
			int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(this, weaponComponentDataForUsage.RelevantSkill);
			return MissionGameModels.Current.AgentStatCalculateModel.GetWeaponInaccuracy(this, weaponComponentDataForUsage, effectiveSkill);
		}

		[MBCallback]
		public float DebugGetHealth()
		{
			return this.Health;
		}

		public void SetTargetPosition(Vec2 value)
		{
			MBAPI.IMBAgent.SetTargetPosition(this.GetPtr(), ref value);
		}

		public void SetGuardState(Agent guardedAgent, bool isGuarding)
		{
			if (isGuarding)
			{
				this.AIStateFlags |= Agent.AIStateFlag.Guard;
			}
			else
			{
				this.AIStateFlags &= ~Agent.AIStateFlag.Guard;
			}
			this.SetGuardedAgent(guardedAgent);
		}

		public void SetCanLeadFormationsRemotely(bool value)
		{
			this._canLeadFormationsRemotely = value;
		}

		public void SetAveragePingInMilliseconds(double averagePingInMilliseconds)
		{
			MBAPI.IMBAgent.SetAveragePingInMilliseconds(this.GetPtr(), averagePingInMilliseconds);
		}

		public void SetTargetPositionAndDirection(Vec2 targetPosition, Vec3 targetDirection)
		{
			MBAPI.IMBAgent.SetTargetPositionAndDirection(this.GetPtr(), ref targetPosition, ref targetDirection);
		}

		public void SetWatchState(Agent.WatchState watchState)
		{
			this.CurrentWatchState = watchState;
		}

		[MBCallback]
		internal void OnWieldedItemIndexChange(bool isOffHand, bool isWieldedInstantly, bool isWieldedOnSpawn)
		{
			if (this.IsMainAgent)
			{
				Agent.OnMainAgentWieldedItemChangeDelegate onMainAgentWieldedItemChange = this.OnMainAgentWieldedItemChange;
				if (onMainAgentWieldedItemChange != null)
				{
					onMainAgentWieldedItemChange();
				}
			}
			Action onAgentWieldedItemChange = this.OnAgentWieldedItemChange;
			if (onAgentWieldedItemChange != null)
			{
				onAgentWieldedItemChange();
			}
			if (GameNetwork.IsServerOrRecorder)
			{
				int num = 0;
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				if (wieldedItemIndex != EquipmentIndex.None)
				{
					num = this.Equipment[wieldedItemIndex].CurrentUsageIndex;
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWieldedItemIndex(this.Index, isOffHand, isWieldedInstantly, isWieldedOnSpawn, this.GetWieldedItemIndex(isOffHand ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand), num));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.CheckEquipmentForCapeClothSimulationStateChange();
		}

		public void SetFormationBanner(ItemObject banner)
		{
			this._formationBanner = banner;
		}

		public void SetIsAIPaused(bool isPaused)
		{
			this.IsPaused = isPaused;
		}

		public void ResetEnemyCaches()
		{
			MBAPI.IMBAgent.ResetEnemyCaches(this.GetPtr());
		}

		public void SetTargetPositionSynched(ref Vec2 targetPosition)
		{
			if (this.MovementLockedState == AgentMovementLockedState.None || this.GetTargetPosition() != targetPosition)
			{
				if (GameNetwork.IsClientOrReplay)
				{
					this._lastSynchedTargetPosition = targetPosition;
					this._checkIfTargetFrameIsChanged = true;
					return;
				}
				this.SetTargetPosition(targetPosition);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetAgentTargetPosition(this.Index, ref targetPosition));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		public void SetTargetPositionAndDirectionSynched(ref Vec2 targetPosition, ref Vec3 targetDirection)
		{
			if (this.MovementLockedState == AgentMovementLockedState.None || this.GetTargetDirection() != targetDirection)
			{
				if (GameNetwork.IsClientOrReplay)
				{
					this._lastSynchedTargetDirection = targetDirection;
					this._checkIfTargetFrameIsChanged = true;
					return;
				}
				this.SetTargetPositionAndDirection(targetPosition, targetDirection);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetAgentTargetPositionAndDirection(this.Index, ref targetPosition, ref targetDirection));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		public void SetBodyArmorMaterialType(ArmorComponent.ArmorMaterialTypes bodyArmorMaterialType)
		{
			MBAPI.IMBAgent.SetBodyArmorMaterialType(this.GetPtr(), bodyArmorMaterialType);
		}

		public void SetUsedGameObjectForClient(UsableMissionObject usedObject)
		{
			this.CurrentlyUsedGameObject = usedObject;
			usedObject.OnUse(this);
			this.Mission.OnObjectUsed(this, usedObject);
		}

		public void SetTeam(Team team, bool sync)
		{
			if (this.Team != team)
			{
				Team team2 = this.Team;
				Team team3 = this.Team;
				if (team3 != null)
				{
					team3.RemoveAgentFromTeam(this);
				}
				this.Team = team;
				Team team4 = this.Team;
				if (team4 != null)
				{
					team4.AddAgentToTeam(this);
				}
				this.SetTeamInternal((team != null) ? team.MBTeam : MBTeam.InvalidTeam);
				if (sync && GameNetwork.IsServer && this.Mission.HasMissionBehavior<MissionNetworkComponent>())
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new AgentSetTeam(this.Index, (team != null) ? team.TeamIndex : (-1)));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
				{
					missionBehavior.OnAgentTeamChanged(team2, team, this);
				}
			}
		}

		public void SetClothingColor1(uint color)
		{
			this._clothingColor1 = new uint?(color);
		}

		public void SetClothingColor2(uint color)
		{
			this._clothingColor2 = new uint?(color);
		}

		public void SetWieldedItemIndexAsClient(Agent.HandIndex handIndex, EquipmentIndex equipmentIndex, bool isWieldedInstantly, bool isWieldedOnSpawn, int mainHandCurrentUsageIndex)
		{
			MBAPI.IMBAgent.SetWieldedItemIndexAsClient(this.GetPtr(), (int)handIndex, (int)equipmentIndex, isWieldedInstantly, isWieldedOnSpawn, mainHandCurrentUsageIndex);
		}

		public void SetPreciseRangedAimingEnabled(bool set)
		{
			if (set)
			{
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.RangerCanMoveForClearTarget);
				return;
			}
			this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.RangerCanMoveForClearTarget);
		}

		public void SetAsConversationAgent(bool set)
		{
			if (set)
			{
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.InConversation);
				this.DisableLookToPointOfInterest();
				return;
			}
			this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.InConversation);
		}

		public void SetCrouchMode(bool set)
		{
			if (set)
			{
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.Crouch);
				return;
			}
			this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.Crouch);
		}

		public void SetWeaponAmountInSlot(EquipmentIndex equipmentSlot, short amount, bool enforcePrimaryItem)
		{
			MBAPI.IMBAgent.SetWeaponAmountInSlot(this.GetPtr(), (int)equipmentSlot, amount, enforcePrimaryItem);
		}

		public void SetWeaponAmmoAsClient(EquipmentIndex equipmentIndex, EquipmentIndex ammoEquipmentIndex, short ammo)
		{
			MBAPI.IMBAgent.SetWeaponAmmoAsClient(this.GetPtr(), (int)equipmentIndex, (int)ammoEquipmentIndex, ammo);
		}

		public void SetWeaponReloadPhaseAsClient(EquipmentIndex equipmentIndex, short reloadState)
		{
			MBAPI.IMBAgent.SetWeaponReloadPhaseAsClient(this.GetPtr(), (int)equipmentIndex, reloadState);
		}

		public void SetReloadAmmoInSlot(EquipmentIndex equipmentIndex, EquipmentIndex ammoSlotIndex, short reloadedAmmo)
		{
			MBAPI.IMBAgent.SetReloadAmmoInSlot(this.GetPtr(), (int)equipmentIndex, (int)ammoSlotIndex, reloadedAmmo);
		}

		public void SetUsageIndexOfWeaponInSlotAsClient(EquipmentIndex slotIndex, int usageIndex)
		{
			MBAPI.IMBAgent.SetUsageIndexOfWeaponInSlotAsClient(this.GetPtr(), (int)slotIndex, usageIndex);
		}

		public void SetRandomizeColors(bool shouldRandomize)
		{
			this.RandomizeColors = shouldRandomize;
		}

		[MBCallback]
		internal void OnRemoveWeapon(EquipmentIndex slotIndex)
		{
			this.RemoveEquippedWeapon(slotIndex);
		}

		public void SetFormationFrameDisabled()
		{
			MBAPI.IMBAgent.SetFormationFrameDisabled(this.GetPtr());
		}

		public void SetFormationFrameEnabled(WorldPosition position, Vec2 direction, Vec2 positionVelocity, float formationDirectionEnforcingFactor)
		{
			MBAPI.IMBAgent.SetFormationFrameEnabled(this.GetPtr(), position, direction, positionVelocity, formationDirectionEnforcingFactor);
			if (this.Mission.IsTeleportingAgents)
			{
				this.TeleportToPosition(position.GetGroundVec3());
			}
		}

		public void SetShouldCatchUpWithFormation(bool value)
		{
			MBAPI.IMBAgent.SetShouldCatchUpWithFormation(this.GetPtr(), value);
		}

		public void SetFormationIntegrityData(Vec2 position, Vec2 currentFormationDirection, Vec2 averageVelocityOfCloseAgents, float averageMaxUnlimitedSpeedOfCloseAgents, float deviationOfPositions)
		{
			MBAPI.IMBAgent.SetFormationIntegrityData(this.GetPtr(), position, currentFormationDirection, averageVelocityOfCloseAgents, averageMaxUnlimitedSpeedOfCloseAgents, deviationOfPositions);
		}

		public void SetGuardedAgent(Agent guardedAgent)
		{
			int num = ((guardedAgent != null) ? guardedAgent.Index : (-1));
			MBAPI.IMBAgent.SetGuardedAgentIndex(this.GetPtr(), num);
		}

		[MBCallback]
		internal void OnWeaponUsageIndexChange(EquipmentIndex slotIndex, int usageIndex)
		{
			this.Equipment.SetUsageIndexOfSlot(slotIndex, usageIndex);
			this.UpdateAgentProperties();
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new WeaponUsageIndexChangeMessage(this.Index, slotIndex, usageIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		public void SetCurrentActionProgress(int channelNo, float progress)
		{
			MBAPI.IMBAgent.SetCurrentActionProgress(this.GetPtr(), channelNo, progress);
		}

		public void SetCurrentActionSpeed(int channelNo, float speed)
		{
			MBAPI.IMBAgent.SetCurrentActionSpeed(this.GetPtr(), channelNo, speed);
		}

		public bool SetActionChannel(int channelNo, ActionIndexCache actionIndexCache, bool ignorePriority = false, ulong additionalFlags = 0UL, float blendWithNextActionFactor = 0f, float actionSpeed = 1f, float blendInPeriod = -0.2f, float blendOutPeriodToNoAnim = 0.4f, float startProgress = 0f, bool useLinearSmoothing = false, float blendOutPeriod = -0.2f, int actionShift = 0, bool forceFaceMorphRestart = true)
		{
			int index = actionIndexCache.Index;
			return MBAPI.IMBAgent.SetActionChannel(this.GetPtr(), channelNo, index + actionShift, additionalFlags, ignorePriority, blendWithNextActionFactor, actionSpeed, blendInPeriod, blendOutPeriodToNoAnim, startProgress, useLinearSmoothing, blendOutPeriod, forceFaceMorphRestart);
		}

		public bool SetActionChannel(int channelNo, ActionIndexValueCache actionIndexCache, bool ignorePriority = false, ulong additionalFlags = 0UL, float blendWithNextActionFactor = 0f, float actionSpeed = 1f, float blendInPeriod = -0.2f, float blendOutPeriodToNoAnim = 0.4f, float startProgress = 0f, bool useLinearSmoothing = false, float blendOutPeriod = -0.2f, int actionShift = 0, bool forceFaceMorphRestart = true)
		{
			int index = actionIndexCache.Index;
			return MBAPI.IMBAgent.SetActionChannel(this.GetPtr(), channelNo, index + actionShift, additionalFlags, ignorePriority, blendWithNextActionFactor, actionSpeed, blendInPeriod, blendOutPeriodToNoAnim, startProgress, useLinearSmoothing, blendOutPeriod, forceFaceMorphRestart);
		}

		[MBCallback]
		internal void OnWeaponAmountChange(EquipmentIndex slotIndex, short amount)
		{
			this.Equipment.SetAmountOfSlot(slotIndex, amount, false);
			this.UpdateAgentProperties();
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWeaponNetworkData(this.Index, slotIndex, amount));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		public void SetAttackState(int attackState)
		{
			MBAPI.IMBAgent.SetAttackState(this.GetPtr(), attackState);
		}

		public void SetAIBehaviorParams(HumanAIComponent.AISimpleBehaviorKind behavior, float y1, float x2, float y2, float x3, float y3)
		{
			MBAPI.IMBAgent.SetAIBehaviorParams(this.GetPtr(), (int)behavior, y1, x2, y2, x3, y3);
		}

		public void SetAllBehaviorParams(HumanAIComponent.BehaviorValues[] behaviorParams)
		{
			MBAPI.IMBAgent.SetAllAIBehaviorParams(this.GetPtr(), behaviorParams);
		}

		public void SetMovementDirection(in Vec2 direction)
		{
			MBAPI.IMBAgent.SetMovementDirection(this.GetPtr(), direction);
		}

		public void SetScriptedFlags(Agent.AIScriptedFrameFlags flags)
		{
			MBAPI.IMBAgent.SetScriptedFlags(this.GetPtr(), (int)flags);
		}

		public void SetScriptedCombatFlags(Agent.AISpecialCombatModeFlags flags)
		{
			MBAPI.IMBAgent.SetScriptedCombatFlags(this.GetPtr(), (int)flags);
		}

		public void SetScriptedPositionAndDirection(ref WorldPosition scriptedPosition, float scriptedDirection, bool addHumanLikeDelay, Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None)
		{
			MBAPI.IMBAgent.SetScriptedPositionAndDirection(this.GetPtr(), ref scriptedPosition, scriptedDirection, addHumanLikeDelay, (int)additionalFlags);
			if (this.Mission.IsTeleportingAgents && scriptedPosition.AsVec2 != this.Position.AsVec2)
			{
				this.TeleportToPosition(scriptedPosition.GetGroundVec3());
			}
		}

		public void SetScriptedPosition(ref WorldPosition position, bool addHumanLikeDelay, Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None)
		{
			MBAPI.IMBAgent.SetScriptedPosition(this.GetPtr(), ref position, addHumanLikeDelay, (int)additionalFlags);
			if (this.Mission.IsTeleportingAgents && position.AsVec2 != this.Position.AsVec2)
			{
				this.TeleportToPosition(position.GetGroundVec3());
			}
		}

		public void SetScriptedTargetEntityAndPosition(GameEntity target, WorldPosition position, Agent.AISpecialCombatModeFlags additionalFlags = Agent.AISpecialCombatModeFlags.None, bool ignoreIfAlreadyAttacking = false)
		{
			MBAPI.IMBAgent.SetScriptedTargetEntity(this.GetPtr(), target.Pointer, ref position, (int)additionalFlags, ignoreIfAlreadyAttacking);
		}

		public void SetAgentExcludeStateForFaceGroupId(int faceGroupId, bool isExcluded)
		{
			MBAPI.IMBAgent.SetAgentExcludeStateForFaceGroupId(this.GetPtr(), faceGroupId, isExcluded);
		}

		public void SetLookAgent(Agent agent)
		{
			this._lookAgentCache = agent;
			MBAPI.IMBAgent.SetLookAgent(this.GetPtr(), (agent != null) ? agent.GetPtr() : UIntPtr.Zero);
		}

		public void SetInteractionAgent(Agent agent)
		{
			MBAPI.IMBAgent.SetInteractionAgent(this.GetPtr(), (agent != null) ? agent.GetPtr() : UIntPtr.Zero);
		}

		public void SetLookToPointOfInterest(Vec3 point)
		{
			MBAPI.IMBAgent.SetLookToPointOfInterest(this.GetPtr(), point);
		}

		public void SetAgentFlags(AgentFlag agentFlags)
		{
			MBAPI.IMBAgent.SetAgentFlags(this.GetPtr(), (uint)agentFlags);
		}

		public void SetSelectedMountIndex(int mountIndex)
		{
			MBAPI.IMBAgent.SetSelectedMountIndex(this.GetPtr(), mountIndex);
		}

		public int GetSelectedMountIndex()
		{
			return MBAPI.IMBAgent.GetSelectedMountIndex(this.GetPtr());
		}

		public int GetFiringOrder()
		{
			return MBAPI.IMBAgent.GetFiringOrder(this.GetPtr());
		}

		public void SetFiringOrder(FiringOrder.RangedWeaponUsageOrderEnum order)
		{
			MBAPI.IMBAgent.SetFiringOrder(this.GetPtr(), (int)order);
		}

		public int GetRidingOrder()
		{
			return MBAPI.IMBAgent.GetRidingOrder(this.GetPtr());
		}

		public void SetRidingOrder(RidingOrder.RidingOrderEnum order)
		{
			MBAPI.IMBAgent.SetRidingOrder(this.GetPtr(), (int)order);
		}

		public int GetTargetFormationIndex()
		{
			return MBAPI.IMBAgent.GetTargetFormationIndex(this.GetPtr());
		}

		public void SetTargetFormationIndex(int targetFormationIndex)
		{
			MBAPI.IMBAgent.SetTargetFormationIndex(this.GetPtr(), targetFormationIndex);
		}

		public void SetAgentFacialAnimation(Agent.FacialAnimChannel channel, string animationName, bool loop)
		{
			MBAPI.IMBAgent.SetAgentFacialAnimation(this.GetPtr(), (int)channel, animationName, loop);
		}

		public bool SetHandInverseKinematicsFrame(ref MatrixFrame leftGlobalFrame, ref MatrixFrame rightGlobalFrame)
		{
			return MBAPI.IMBAgent.SetHandInverseKinematicsFrame(this.GetPtr(), ref leftGlobalFrame, ref rightGlobalFrame);
		}

		public void SetNativeFormationNo(int formationNo)
		{
			MBAPI.IMBAgent.SetFormationNo(this.GetPtr(), formationNo);
		}

		public void SetDirectionChangeTendency(float tendency)
		{
			MBAPI.IMBAgent.SetDirectionChangeTendency(this.GetPtr(), tendency);
		}

		public float GetBattleImportance()
		{
			BasicCharacterObject character = this.Character;
			float num = ((character != null) ? character.GetBattlePower() : 1f);
			if (this.Team != null && this == this.Team.GeneralAgent)
			{
				num *= 2f;
			}
			else if (this.Formation != null && this == this.Formation.Captain)
			{
				num *= 1.2f;
			}
			return num;
		}

		public void SetSynchedPrefabComponentVisibility(int componentIndex, bool visibility)
		{
			this._synchedBodyComponents[componentIndex].SetVisible(visibility);
			this.AgentVisuals.LazyUpdateAgentRendererData();
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetAgentPrefabComponentVisibility(this.Index, componentIndex, visibility));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		public void SetActionSet(ref AnimationSystemData animationSystemData)
		{
			MBAPI.IMBAgent.SetActionSet(this.GetPtr(), ref animationSystemData);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetAgentActionSet(this.Index, animationSystemData));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		public void SetColumnwiseFollowAgent(Agent followAgent, ref Vec2 followPosition)
		{
			if (!this.IsAIControlled)
			{
				return;
			}
			int num = ((followAgent != null) ? followAgent.Index : (-1));
			MBAPI.IMBAgent.SetColumnwiseFollowAgent(this.GetPtr(), num, ref followPosition);
			this.SetFollowedUnit(followAgent);
		}

		public void SetHandInverseKinematicsFrameForMissionObjectUsage(in MatrixFrame localIKFrame, in MatrixFrame boundEntityGlobalFrame, float animationHeightDifference = 0f)
		{
			if (this.GetCurrentActionValue(1) != ActionIndexValueCache.act_none && this.GetActionChannelWeight(1) > 0f)
			{
				MBAPI.IMBAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this.GetPtr(), localIKFrame, boundEntityGlobalFrame, animationHeightDifference);
				return;
			}
			this.ClearHandInverseKinematics();
		}

		public void SetWantsToYell()
		{
			this._wantsToYell = true;
			this._yellTimer = MBRandom.RandomFloat * 0.3f + 0.1f;
		}

		public void SetCapeClothSimulator(GameEntityComponent clothSimulatorComponent)
		{
			ClothSimulatorComponent clothSimulatorComponent2 = clothSimulatorComponent as ClothSimulatorComponent;
			this._capeClothSimulator = clothSimulatorComponent2;
		}

		public Vec2 GetTargetPosition()
		{
			return MBAPI.IMBAgent.GetTargetPosition(this.GetPtr());
		}

		public Vec3 GetTargetDirection()
		{
			return MBAPI.IMBAgent.GetTargetDirection(this.GetPtr());
		}

		public float GetAimingTimer()
		{
			return MBAPI.IMBAgent.GetAimingTimer(this.GetPtr());
		}

		public float GetInteractionDistanceToUsable(IUsable usable)
		{
			Agent agent;
			if ((agent = usable as Agent) != null)
			{
				if (!agent.IsMount)
				{
					return 3f;
				}
				return 1.75f;
			}
			else
			{
				SpawnedItemEntity spawnedItemEntity;
				if ((spawnedItemEntity = usable as SpawnedItemEntity) != null && spawnedItemEntity.IsBanner())
				{
					return 3f;
				}
				float interactionDistance = MissionGameModels.Current.AgentStatCalculateModel.GetInteractionDistance(this);
				if (!(usable is StandingPoint))
				{
					return interactionDistance;
				}
				if (!this.IsAIControlled || !this.WalkMode)
				{
					return 1f;
				}
				return 0.5f;
			}
		}

		public TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			if (this.IsMount && !userAgent.CheckSkillForMounting(this))
			{
				return GameTexts.FindText("str_ui_riding_skill_not_adequate_to_mount", null);
			}
			return TextObject.Empty;
		}

		public T GetController<T>() where T : AgentController
		{
			for (int i = 0; i < this._agentControllers.Count; i++)
			{
				if (this._agentControllers[i] is T)
				{
					return (T)((object)this._agentControllers[i]);
				}
			}
			return default(T);
		}

		public EquipmentIndex GetWieldedItemIndex(Agent.HandIndex index)
		{
			return MBAPI.IMBAgent.GetWieldedItemIndex(this.GetPtr(), (int)index);
		}

		public float GetTrackDistanceToMainAgent()
		{
			float num = -1f;
			if (Agent.Main != null)
			{
				num = Agent.Main.Position.Distance(this.Position);
			}
			return num;
		}

		public string GetDescriptionText(GameEntity gameEntity = null)
		{
			return this.Name;
		}

		public GameEntity GetWeaponEntityFromEquipmentSlot(EquipmentIndex slotIndex)
		{
			return new GameEntity(MBAPI.IMBAgent.GetWeaponEntityFromEquipmentSlot(this.GetPtr(), (int)slotIndex));
		}

		public WorldPosition GetRetreatPos()
		{
			return MBAPI.IMBAgent.GetRetreatPos(this.GetPtr());
		}

		public Agent.AIScriptedFrameFlags GetScriptedFlags()
		{
			return (Agent.AIScriptedFrameFlags)MBAPI.IMBAgent.GetScriptedFlags(this.GetPtr());
		}

		public Agent.AISpecialCombatModeFlags GetScriptedCombatFlags()
		{
			return (Agent.AISpecialCombatModeFlags)MBAPI.IMBAgent.GetScriptedCombatFlags(this.GetPtr());
		}

		public GameEntity GetSteppedEntity()
		{
			UIntPtr steppedEntityId = MBAPI.IMBAgent.GetSteppedEntityId(this.GetPtr());
			if (!(steppedEntityId != UIntPtr.Zero))
			{
				return null;
			}
			return new GameEntity(steppedEntityId);
		}

		public AnimFlags GetCurrentAnimationFlag(int channelNo)
		{
			return (AnimFlags)MBAPI.IMBAgent.GetCurrentAnimationFlags(this.GetPtr(), channelNo);
		}

		public ActionIndexCache GetCurrentAction(int channelNo)
		{
			return new ActionIndexCache(MBAPI.IMBAgent.GetCurrentAction(this.GetPtr(), channelNo));
		}

		public ActionIndexValueCache GetCurrentActionValue(int channelNo)
		{
			return new ActionIndexValueCache(MBAPI.IMBAgent.GetCurrentAction(this.GetPtr(), channelNo));
		}

		public Agent.ActionCodeType GetCurrentActionType(int channelNo)
		{
			return (Agent.ActionCodeType)MBAPI.IMBAgent.GetCurrentActionType(this.GetPtr(), channelNo);
		}

		public Agent.ActionStage GetCurrentActionStage(int channelNo)
		{
			return (Agent.ActionStage)MBAPI.IMBAgent.GetCurrentActionStage(this.GetPtr(), channelNo);
		}

		public Agent.UsageDirection GetCurrentActionDirection(int channelNo)
		{
			return (Agent.UsageDirection)MBAPI.IMBAgent.GetCurrentActionDirection(this.GetPtr(), channelNo);
		}

		public int GetCurrentActionPriority(int channelNo)
		{
			return MBAPI.IMBAgent.GetCurrentActionPriority(this.GetPtr(), channelNo);
		}

		public float GetCurrentActionProgress(int channelNo)
		{
			return MBAPI.IMBAgent.GetCurrentActionProgress(this.GetPtr(), channelNo);
		}

		public float GetActionChannelWeight(int channelNo)
		{
			return MBAPI.IMBAgent.GetActionChannelWeight(this.GetPtr(), channelNo);
		}

		public float GetActionChannelCurrentActionWeight(int channelNo)
		{
			return MBAPI.IMBAgent.GetActionChannelCurrentActionWeight(this.GetPtr(), channelNo);
		}

		public WorldFrame GetWorldFrame()
		{
			return new WorldFrame(this.LookRotation, this.GetWorldPosition());
		}

		public float GetLookDownLimit()
		{
			return MBAPI.IMBAgent.GetLookDownLimit(this.GetPtr());
		}

		public float GetEyeGlobalHeight()
		{
			return MBAPI.IMBAgent.GetEyeGlobalHeight(this.GetPtr());
		}

		public float GetMaximumSpeedLimit()
		{
			return MBAPI.IMBAgent.GetMaximumSpeedLimit(this.GetPtr());
		}

		public Vec2 GetCurrentVelocity()
		{
			return MBAPI.IMBAgent.GetCurrentVelocity(this.GetPtr());
		}

		public float GetTurnSpeed()
		{
			return MBAPI.IMBAgent.GetTurnSpeed(this.GetPtr());
		}

		public float GetCurrentSpeedLimit()
		{
			return MBAPI.IMBAgent.GetCurrentSpeedLimit(this.GetPtr());
		}

		public Vec2 GetMovementDirection()
		{
			return MBAPI.IMBAgent.GetMovementDirection(this.GetPtr());
		}

		public Vec3 GetCurWeaponOffset()
		{
			return MBAPI.IMBAgent.GetCurWeaponOffset(this.GetPtr());
		}

		public bool GetIsLeftStance()
		{
			return MBAPI.IMBAgent.GetIsLeftStance(this.GetPtr());
		}

		public float GetPathDistanceToPoint(ref Vec3 point)
		{
			return MBAPI.IMBAgent.GetPathDistanceToPoint(this.GetPtr(), ref point);
		}

		public int GetCurrentNavigationFaceId()
		{
			return MBAPI.IMBAgent.GetCurrentNavigationFaceId(this.GetPtr());
		}

		public WorldPosition GetWorldPosition()
		{
			return MBAPI.IMBAgent.GetWorldPosition(this.GetPtr());
		}

		public Agent GetLookAgent()
		{
			return this._lookAgentCache;
		}

		public Agent GetTargetAgent()
		{
			return MBAPI.IMBAgent.GetTargetAgent(this.GetPtr());
		}

		public void SetTargetAgent(Agent agent)
		{
			MBAPI.IMBAgent.SetTargetAgent(this.GetPtr(), (agent != null) ? agent.Index : (-1));
		}

		public void SetAutomaticTargetSelection(bool enable)
		{
			MBAPI.IMBAgent.SetAutomaticTargetSelection(this.GetPtr(), enable);
		}

		public AgentFlag GetAgentFlags()
		{
			return AgentHelper.GetAgentFlags(this.FlagsPointer);
		}

		public string GetAgentFacialAnimation()
		{
			return MBAPI.IMBAgent.GetAgentFacialAnimation(this.GetPtr());
		}

		public string GetAgentVoiceDefinition()
		{
			return MBAPI.IMBAgent.GetAgentVoiceDefinition(this.GetPtr());
		}

		public Vec3 GetEyeGlobalPosition()
		{
			return MBAPI.IMBAgent.GetEyeGlobalPosition(this.GetPtr());
		}

		public Vec3 GetChestGlobalPosition()
		{
			return MBAPI.IMBAgent.GetChestGlobalPosition(this.GetPtr());
		}

		public Agent.MovementControlFlag GetDefendMovementFlag()
		{
			return MBAPI.IMBAgent.GetDefendMovementFlag(this.GetPtr());
		}

		public Agent.UsageDirection GetAttackDirection()
		{
			return MBAPI.IMBAgent.GetAttackDirection(this.GetPtr());
		}

		public WeaponInfo GetWieldedWeaponInfo(Agent.HandIndex handIndex)
		{
			bool flag = false;
			bool flag2 = false;
			if (MBAPI.IMBAgent.GetWieldedWeaponInfo(this.GetPtr(), (int)handIndex, ref flag, ref flag2))
			{
				return new WeaponInfo(true, flag, flag2);
			}
			return new WeaponInfo(false, false, false);
		}

		public Vec2 GetBodyRotationConstraint(int channelIndex = 1)
		{
			return MBAPI.IMBAgent.GetBodyRotationConstraint(this.GetPtr(), channelIndex).AsVec2;
		}

		public float GetTotalEncumbrance()
		{
			return this.AgentDrivenProperties.ArmorEncumbrance + this.AgentDrivenProperties.WeaponsEncumbrance;
		}

		public T GetComponent<T>() where T : AgentComponent
		{
			for (int i = 0; i < this._components.Count; i++)
			{
				if (this._components[i] is T)
				{
					return (T)((object)this._components[i]);
				}
			}
			return default(T);
		}

		public float GetAgentDrivenPropertyValue(DrivenProperty type)
		{
			return this.AgentDrivenProperties.GetStat(type);
		}

		public UsableMachine GetSteppedMachine()
		{
			GameEntity gameEntity = this.GetSteppedEntity();
			while (gameEntity != null && !gameEntity.HasScriptOfType<UsableMachine>())
			{
				gameEntity = gameEntity.Parent;
			}
			if (gameEntity != null)
			{
				return gameEntity.GetFirstScriptOfType<UsableMachine>();
			}
			return null;
		}

		public int GetAttachedWeaponsCount()
		{
			List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>> attachedWeapons = this._attachedWeapons;
			if (attachedWeapons == null)
			{
				return 0;
			}
			return attachedWeapons.Count;
		}

		public MissionWeapon GetAttachedWeapon(int index)
		{
			return this._attachedWeapons[index].Item1;
		}

		public MatrixFrame GetAttachedWeaponFrame(int index)
		{
			return this._attachedWeapons[index].Item2;
		}

		public sbyte GetAttachedWeaponBoneIndex(int index)
		{
			return this._attachedWeapons[index].Item3;
		}

		public void DeleteAttachedWeapon(int index)
		{
			this._attachedWeapons.RemoveAt(index);
			MBAPI.IMBAgent.DeleteAttachedWeaponFromBone(this.Pointer, index);
		}

		public bool HasRangedWeapon(bool checkHasAmmo = false)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				int num;
				bool flag;
				bool flag2;
				if (!this.Equipment[equipmentIndex].IsEmpty && this.Equipment[equipmentIndex].GetRangedUsageIndex() >= 0 && (!checkHasAmmo || this.Equipment.HasAmmo(equipmentIndex, out num, out flag, out flag2)))
				{
					return true;
				}
			}
			return false;
		}

		public void GetFormationFileAndRankInfo(out int fileIndex, out int rankIndex)
		{
			fileIndex = ((IFormationUnit)this).FormationFileIndex;
			rankIndex = ((IFormationUnit)this).FormationRankIndex;
		}

		public void GetFormationFileAndRankInfo(out int fileIndex, out int rankIndex, out int fileCount, out int rankCount)
		{
			fileIndex = ((IFormationUnit)this).FormationFileIndex;
			rankIndex = ((IFormationUnit)this).FormationRankIndex;
			LineFormation lineFormation;
			if ((lineFormation = ((IFormationUnit)this).Formation as LineFormation) != null)
			{
				lineFormation.GetFormationInfo(out fileCount, out rankCount);
				return;
			}
			fileCount = -1;
			rankCount = -1;
		}

		internal Vec2 GetWallDirectionOfRelativeFormationLocation()
		{
			return this.Formation.GetWallDirectionOfRelativeFormationLocation(this);
		}

		public void SetMortalityState(Agent.MortalityState newState)
		{
			this.CurrentMortalityState = newState;
		}

		public void ToggleInvulnerable()
		{
			if (this.CurrentMortalityState == Agent.MortalityState.Invulnerable)
			{
				this.CurrentMortalityState = Agent.MortalityState.Mortal;
				return;
			}
			this.CurrentMortalityState = Agent.MortalityState.Invulnerable;
		}

		public float GetArmLength()
		{
			return this.Monster.ArmLength * this.AgentScale;
		}

		public float GetArmWeight()
		{
			return this.Monster.ArmWeight * this.AgentScale;
		}

		public void GetRunningSimulationDataUntilMaximumSpeedReached(ref float combatAccelerationTime, ref float maxSpeed, float[] speedValues)
		{
			MBAPI.IMBAgent.GetRunningSimulationDataUntilMaximumSpeedReached(this.GetPtr(), ref combatAccelerationTime, ref maxSpeed, speedValues);
		}

		public void SetMaximumSpeedLimit(float maximumSpeedLimit, bool isMultiplier)
		{
			MBAPI.IMBAgent.SetMaximumSpeedLimit(this.GetPtr(), maximumSpeedLimit, isMultiplier);
		}

		public float GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType bodyPart)
		{
			if (!this.IsHuman)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
			}
			if (bodyPart == BoneBodyPartType.None)
			{
				return 0f;
			}
			if (bodyPart == BoneBodyPartType.Head || bodyPart == BoneBodyPartType.Neck)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorHead);
			}
			if (bodyPart == BoneBodyPartType.Legs)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorLegs);
			}
			if (bodyPart == BoneBodyPartType.ArmLeft || bodyPart == BoneBodyPartType.ArmRight)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorArms);
			}
			if (bodyPart == BoneBodyPartType.ShoulderLeft || bodyPart == BoneBodyPartType.ShoulderRight || bodyPart == BoneBodyPartType.Chest || bodyPart == BoneBodyPartType.Abdomen)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", "GetBaseArmorEffectivenessForBodyPart", 2809);
			return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
		}

		public AITargetVisibilityState GetLastTargetVisibilityState()
		{
			return (AITargetVisibilityState)MBAPI.IMBAgent.GetLastTargetVisibilityState(this.GetPtr());
		}

		public float GetMissileRange()
		{
			return MBAPI.IMBAgent.GetMissileRange(this.GetPtr());
		}

		public ItemObject GetWeaponToReplaceOnQuickAction(SpawnedItemEntity spawnedItem, out EquipmentIndex possibleSlotIndex)
		{
			EquipmentIndex equipmentIndex = MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile());
			possibleSlotIndex = equipmentIndex;
			if (equipmentIndex != EquipmentIndex.None && !this.Equipment[equipmentIndex].IsEmpty && ((!spawnedItem.IsStuckMissile() && !spawnedItem.WeaponCopy.IsAnyConsumable()) || this.Equipment[equipmentIndex].Item.PrimaryWeapon.WeaponClass != spawnedItem.WeaponCopy.Item.PrimaryWeapon.WeaponClass || !this.Equipment[equipmentIndex].IsAnyConsumable() || this.Equipment[equipmentIndex].Amount == this.Equipment[equipmentIndex].ModifiedMaxAmount))
			{
				return this.Equipment[equipmentIndex].Item;
			}
			return null;
		}

		public Agent.Hitter GetAssistingHitter(MissionPeer killerPeer)
		{
			Agent.Hitter hitter = null;
			foreach (Agent.Hitter hitter2 in this.HitterList)
			{
				if (hitter2.HitterPeer != killerPeer && (hitter == null || hitter2.Damage > hitter.Damage))
				{
					hitter = hitter2;
				}
			}
			if (hitter != null && hitter.Damage >= 35f)
			{
				return hitter;
			}
			return null;
		}

		public bool CanReachAgent(Agent otherAgent)
		{
			float interactionDistanceToUsable = this.GetInteractionDistanceToUsable(otherAgent);
			return this.Position.DistanceSquared(otherAgent.Position) < interactionDistanceToUsable * interactionDistanceToUsable;
		}

		public bool CanInteractWithAgent(Agent otherAgent, float userAgentCameraElevation)
		{
			bool flag = false;
			foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
			{
				flag = flag || missionBehavior.IsThereAgentAction(this, otherAgent);
			}
			if (!flag)
			{
				return false;
			}
			bool flag2 = this.CanReachAgent(otherAgent);
			if (!otherAgent.IsMount)
			{
				return this.IsOnLand() && flag2;
			}
			if ((this.MountAgent == null && this.GetCurrentActionValue(0) != ActionIndexValueCache.act_none) || (this.MountAgent != null && !this.IsOnLand()))
			{
				return false;
			}
			if (otherAgent.RiderAgent == null)
			{
				return this.MountAgent == null && flag2 && this.CheckSkillForMounting(otherAgent) && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear;
			}
			return otherAgent == this.MountAgent && (flag2 && userAgentCameraElevation < this.GetLookDownLimit() + 0.4f && this.GetCurrentVelocity().LengthSquared < 0.25f) && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear;
		}

		public bool CanBeAssignedForScriptedMovement()
		{
			return this.IsActive() && this.IsAIControlled && !this.IsDetachedFromFormation && !this.IsRunningAway && (this.GetScriptedFlags() & Agent.AIScriptedFrameFlags.GoToPosition) == Agent.AIScriptedFrameFlags.None && !this.InteractingWithAnyGameObject();
		}

		public bool CanReachAndUseObject(UsableMissionObject gameObject, float distanceSq)
		{
			return this.CanReachObject(gameObject, distanceSq) && this.CanUseObject(gameObject);
		}

		public bool CanReachObject(UsableMissionObject gameObject, float distanceSq)
		{
			if (this.IsItemUseDisabled || this.IsUsingGameObject)
			{
				return false;
			}
			float interactionDistanceToUsable = this.GetInteractionDistanceToUsable(gameObject);
			return distanceSq <= interactionDistanceToUsable * interactionDistanceToUsable && MathF.Abs(gameObject.InteractionEntity.GlobalPosition.z - this.Position.z) <= interactionDistanceToUsable * 2f;
		}

		public bool CanUseObject(UsableMissionObject gameObject)
		{
			return !gameObject.IsDisabledForAgent(this) && gameObject.IsUsableByAgent(this);
		}

		public bool CanMoveDirectlyToPosition(in Vec2 position)
		{
			return MBAPI.IMBAgent.CanMoveDirectlyToPosition(this.GetPtr(), position);
		}

		public bool CanInteractableWeaponBePickedUp(SpawnedItemEntity spawnedItem)
		{
			EquipmentIndex equipmentIndex;
			return (!spawnedItem.IsBanner() || MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(spawnedItem, this)) && (this.GetWeaponToReplaceOnQuickAction(spawnedItem, out equipmentIndex) != null || equipmentIndex == EquipmentIndex.None);
		}

		public bool CanQuickPickUp(SpawnedItemEntity spawnedItem)
		{
			return (!spawnedItem.IsBanner() || MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(spawnedItem, this)) && MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile()) != EquipmentIndex.None;
		}

		public unsafe bool CanTeleport()
		{
			return this.Mission.IsTeleportingAgents && (this.Formation == null || this.Mission.Mode != MissionMode.Deployment || this.Formation.GetReadonlyMovementOrderReference()->OrderEnum == MovementOrder.MovementOrderEnum.Move);
		}

		public bool IsActive()
		{
			return this.State == AgentState.Active;
		}

		public bool IsRetreating()
		{
			return MBAPI.IMBAgent.IsRetreating(this.GetPtr());
		}

		public bool IsFadingOut()
		{
			return MBAPI.IMBAgent.IsFadingOut(this.GetPtr());
		}

		public void SetAgentDrivenPropertyValueFromConsole(DrivenProperty type, float val)
		{
			this.AgentDrivenProperties.SetStat(type, val);
		}

		public bool IsOnLand()
		{
			return MBAPI.IMBAgent.IsOnLand(this.GetPtr());
		}

		public bool IsSliding()
		{
			return MBAPI.IMBAgent.IsSliding(this.GetPtr());
		}

		public bool IsSitting()
		{
			Agent.ActionCodeType currentActionType = this.GetCurrentActionType(0);
			return currentActionType == Agent.ActionCodeType.Sit || currentActionType == Agent.ActionCodeType.SitOnTheFloor || currentActionType == Agent.ActionCodeType.SitOnAThrone;
		}

		public bool IsReleasingChainAttack()
		{
			bool flag = false;
			if (Mission.Current.CurrentTime - this._lastQuickReadyDetectedTime < 0.75f && this.GetCurrentActionStage(1) == Agent.ActionStage.AttackRelease)
			{
				flag = true;
			}
			return flag;
		}

		public bool IsCameraAttachable()
		{
			return !this._isDeleted && (!this._isRemoved || this._removalTime + 2.1f > this.Mission.CurrentTime) && this.IsHuman && this.AgentVisuals != null && this.AgentVisuals.IsValid() && (GameNetwork.IsSessionActive || this._agentControllerType > Agent.ControllerType.None);
		}

		public bool IsSynchedPrefabComponentVisible(int componentIndex)
		{
			return this._synchedBodyComponents[componentIndex].GetVisible();
		}

		public bool IsEnemyOf(Agent otherAgent)
		{
			return MBAPI.IMBAgent.IsEnemy(this.GetPtr(), otherAgent.GetPtr());
		}

		public bool IsFriendOf(Agent otherAgent)
		{
			return MBAPI.IMBAgent.IsFriend(this.GetPtr(), otherAgent.GetPtr());
		}

		public void OnFocusGain(Agent userAgent)
		{
		}

		public void OnFocusLose(Agent userAgent)
		{
		}

		public void OnItemRemovedFromScene()
		{
			this.StopUsingGameObjectMT(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
		}

		public void OnUse(Agent userAgent)
		{
			this.Mission.OnAgentInteraction(userAgent, this);
		}

		public void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
		}

		public void OnWeaponDrop(EquipmentIndex equipmentSlot)
		{
			MissionWeapon missionWeapon = this.Equipment[equipmentSlot];
			this.Equipment[equipmentSlot] = MissionWeapon.Invalid;
			this.WeaponEquipped(equipmentSlot, WeaponData.InvalidWeaponData, null, WeaponData.InvalidWeaponData, null, null, false, false);
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnWeaponDrop(missionWeapon);
			}
		}

		public void OnItemPickup(SpawnedItemEntity spawnedItemEntity, EquipmentIndex weaponPickUpSlotIndex, out bool removeWeapon)
		{
			removeWeapon = true;
			bool flag = true;
			MissionWeapon weaponCopy = spawnedItemEntity.WeaponCopy;
			if (weaponPickUpSlotIndex == EquipmentIndex.None)
			{
				weaponPickUpSlotIndex = MissionEquipment.SelectWeaponPickUpSlot(this, weaponCopy, spawnedItemEntity.IsStuckMissile());
			}
			bool flag2 = false;
			if (weaponPickUpSlotIndex == EquipmentIndex.ExtraWeaponSlot)
			{
				if (!GameNetwork.IsClientOrReplay)
				{
					flag2 = true;
					if (!this.Equipment[weaponPickUpSlotIndex].IsEmpty)
					{
						this.DropItem(weaponPickUpSlotIndex, this.Equipment[weaponPickUpSlotIndex].Item.PrimaryWeapon.WeaponClass);
					}
				}
			}
			else if (weaponPickUpSlotIndex != EquipmentIndex.None)
			{
				int num = 0;
				if ((spawnedItemEntity.IsStuckMissile() || spawnedItemEntity.WeaponCopy.IsAnyConsumable()) && !this.Equipment[weaponPickUpSlotIndex].IsEmpty && this.Equipment[weaponPickUpSlotIndex].IsSameType(weaponCopy) && this.Equipment[weaponPickUpSlotIndex].IsAnyConsumable())
				{
					num = (int)(this.Equipment[weaponPickUpSlotIndex].ModifiedMaxAmount - this.Equipment[weaponPickUpSlotIndex].Amount);
				}
				if (num > 0)
				{
					short num2 = (short)MathF.Min(num, (int)weaponCopy.Amount);
					if (num2 != weaponCopy.Amount)
					{
						removeWeapon = false;
						if (!GameNetwork.IsClientOrReplay)
						{
							spawnedItemEntity.ConsumeWeaponAmount(num2);
							if (GameNetwork.IsServer)
							{
								GameNetwork.BeginBroadcastModuleEvent();
								GameNetwork.WriteMessage(new ConsumeWeaponAmount(spawnedItemEntity.Id, num2));
								GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
							}
						}
					}
					if (!GameNetwork.IsClientOrReplay)
					{
						this.SetWeaponAmountInSlot(weaponPickUpSlotIndex, this.Equipment[weaponPickUpSlotIndex].Amount + num2, true);
						if (this.GetWieldedItemIndex(Agent.HandIndex.MainHand) == EquipmentIndex.None && (weaponCopy.Item.PrimaryWeapon.IsRangedWeapon || weaponCopy.Item.PrimaryWeapon.IsMeleeWeapon))
						{
							flag2 = true;
						}
					}
				}
				else if (!GameNetwork.IsClientOrReplay)
				{
					flag2 = true;
					if (!this.Equipment[weaponPickUpSlotIndex].IsEmpty)
					{
						this.DropItem(weaponPickUpSlotIndex, weaponCopy.Item.PrimaryWeapon.WeaponClass);
					}
				}
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				flag = MissionEquipment.DoesWeaponFitToSlot(weaponPickUpSlotIndex, weaponCopy);
				if (flag)
				{
					this.EquipWeaponFromSpawnedItemEntity(weaponPickUpSlotIndex, spawnedItemEntity, removeWeapon);
					if (flag2)
					{
						EquipmentIndex equipmentIndex = weaponPickUpSlotIndex;
						if (weaponCopy.Item.PrimaryWeapon.AmmoClass == weaponCopy.Item.PrimaryWeapon.WeaponClass)
						{
							for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < weaponPickUpSlotIndex; equipmentIndex2++)
							{
								if (!this.Equipment[equipmentIndex2].IsEmpty && weaponCopy.IsEqualTo(this.Equipment[equipmentIndex2]))
								{
									equipmentIndex = equipmentIndex2;
									break;
								}
							}
						}
						this.TryToWieldWeaponInSlot(equipmentIndex, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
					}
					for (int i = 0; i < this._components.Count; i++)
					{
						this._components[i].OnItemPickup(spawnedItemEntity);
					}
					if (this.Controller == Agent.ControllerType.AI)
					{
						this.HumanAIComponent.ItemPickupDone(spawnedItemEntity);
					}
				}
			}
			if (flag)
			{
				this.Mission.TriggerOnItemPickUpEvent(this, spawnedItemEntity);
			}
		}

		public bool CheckTracked(BasicCharacterObject basicCharacter)
		{
			return this.Character == basicCharacter;
		}

		public bool CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(int navigationFaceId, Vec3 direction, float overridenCostForFaceId)
		{
			return MBAPI.IMBAgent.CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction, overridenCostForFaceId);
		}

		public void CheckEquipmentForCapeClothSimulationStateChange()
		{
			if (this._capeClothSimulator != null)
			{
				bool flag = false;
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
				{
					MissionWeapon missionWeapon = this.Equipment[equipmentIndex];
					if (!missionWeapon.IsEmpty && missionWeapon.IsShield() && equipmentIndex != wieldedItemIndex)
					{
						flag = true;
						break;
					}
				}
				this._capeClothSimulator.SetMaxDistanceMultiplier(flag ? 0f : 1f);
			}
		}

		public void CheckToDropFlaggedItem()
		{
			if (this.GetAgentFlags().HasAnyFlag(AgentFlag.CanWieldWeapon))
			{
				for (int i = 0; i < 2; i++)
				{
					EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex((Agent.HandIndex)i);
					if (wieldedItemIndex != EquipmentIndex.None && this.Equipment[wieldedItemIndex].Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction))
					{
						this.DropItem(wieldedItemIndex, WeaponClass.Undefined);
					}
				}
			}
		}

		public bool CheckSkillForMounting(Agent mountAgent)
		{
			int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(this, DefaultSkills.Riding);
			return (this.GetAgentFlags() & AgentFlag.CanRide) > AgentFlag.None && (float)effectiveSkill >= mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.MountDifficulty);
		}

		public void InitializeSpawnEquipment(Equipment spawnEquipment)
		{
			this.SpawnEquipment = spawnEquipment;
		}

		public void InitializeMissionEquipment(MissionEquipment missionEquipment, Banner banner)
		{
			this.Equipment = missionEquipment ?? new MissionEquipment(this.SpawnEquipment, banner);
		}

		public void InitializeAgentProperties(Equipment spawnEquipment, AgentBuildData agentBuildData)
		{
			this._propertyModifiers = default(Agent.AgentPropertiesModifiers);
			this.AgentDrivenProperties = new AgentDrivenProperties();
			float[] array = this.AgentDrivenProperties.InitializeDrivenProperties(this, spawnEquipment, agentBuildData);
			this.UpdateDrivenProperties(array);
			if (this.IsMount && this.RiderAgent == null)
			{
				Mission.Current.AddMountWithoutRider(this);
			}
		}

		public void UpdateFormationOrders()
		{
			if (this.Formation != null && !this.IsRetreating())
			{
				this.EnforceShieldUsage(ArrangementOrder.GetShieldDirectionOfUnit(this.Formation, this, this.Formation.ArrangementOrder.OrderEnum));
			}
		}

		public void UpdateWeapons()
		{
			MBAPI.IMBAgent.UpdateWeapons(this.GetPtr());
		}

		public void UpdateAgentProperties()
		{
			if (this.AgentDrivenProperties != null)
			{
				float[] array = this.AgentDrivenProperties.UpdateDrivenProperties(this);
				this.UpdateDrivenProperties(array);
			}
		}

		public void UpdateCustomDrivenProperties()
		{
			if (this.AgentDrivenProperties != null)
			{
				this.UpdateDrivenProperties(this.AgentDrivenProperties.Values);
			}
		}

		public void UpdateBodyProperties(BodyProperties bodyProperties)
		{
			this.BodyPropertiesValue = bodyProperties;
		}

		public void UpdateSyncHealthToAllClients(bool value)
		{
			this.SyncHealthToAllClients = value;
		}

		public void UpdateSpawnEquipmentAndRefreshVisuals(Equipment newSpawnEquipment)
		{
			this.SpawnEquipment = newSpawnEquipment;
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SynchronizeAgentSpawnEquipment(this.Index, this.SpawnEquipment));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.AgentVisuals.ClearVisualComponents(false);
			this.Mission.OnEquipItemsFromSpawnEquipment(this, Agent.CreationType.FromCharacterObj);
			this.AgentVisuals.ClearAllWeaponMeshes();
			MissionEquipment equipment = this.Equipment;
			Equipment spawnEquipment = this.SpawnEquipment;
			IAgentOriginBase origin = this.Origin;
			equipment.FillFrom(spawnEquipment, (origin != null) ? origin.Banner : null);
			this.CheckEquipmentForCapeClothSimulationStateChange();
			this.EquipItemsFromSpawnEquipment(true);
			this.UpdateAgentProperties();
			if (!Mission.Current.DoesMissionRequireCivilianEquipment && !GameNetwork.IsClientOrReplay)
			{
				this.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp, TaleWorlds.Core.Equipment.InitialWeaponEquipPreference.Any);
			}
			this.PreloadForRendering();
		}

		public void UpdateCachedAndFormationValues(bool updateOnlyMovement, bool arrangementChangeAllowed)
		{
			if (!this.IsActive())
			{
				return;
			}
			if (!updateOnlyMovement)
			{
				Agent mountAgent = this.MountAgent;
				this.WalkSpeedCached = ((mountAgent != null) ? mountAgent.WalkingSpeedLimitOfMountable : this.Monster.WalkingSpeedLimit);
				this.RunSpeedCached = this.MaximumForwardUnlimitedSpeed;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				if (!updateOnlyMovement && !this.IsDetachedFromFormation)
				{
					Formation formation = this.Formation;
					if (formation != null)
					{
						formation.Arrangement.OnTickOccasionallyOfUnit(this, arrangementChangeAllowed);
					}
				}
				if (this.IsAIControlled)
				{
					this.HumanAIComponent.UpdateFormationMovement();
				}
				if (!updateOnlyMovement)
				{
					Formation formation2 = this.Formation;
					if (formation2 != null)
					{
						formation2.Team.DetachmentManager.TickAgent(this);
					}
				}
				if (!updateOnlyMovement && this.IsAIControlled)
				{
					this.UpdateFormationOrders();
					if (this.Formation != null)
					{
						int num;
						int num2;
						int num3;
						int num4;
						this.GetFormationFileAndRankInfo(out num, out num2, out num3, out num4);
						Vec2 wallDirectionOfRelativeFormationLocation = this.GetWallDirectionOfRelativeFormationLocation();
						MBAPI.IMBAgent.SetFormationInfo(this.GetPtr(), num, num2, num3, num4, wallDirectionOfRelativeFormationLocation, this.Formation.UnitSpacing);
					}
				}
			}
		}

		public void UpdateLastRangedAttackTimeDueToAnAttack(float newTime)
		{
			this.LastRangedAttackTime = newTime;
		}

		public void InvalidateTargetAgent()
		{
			MBAPI.IMBAgent.InvalidateTargetAgent(this.GetPtr());
		}

		public void InvalidateAIWeaponSelections()
		{
			MBAPI.IMBAgent.InvalidateAIWeaponSelections(this.GetPtr());
		}

		public void ResetLookAgent()
		{
			this.SetLookAgent(null);
		}

		public void ResetGuard()
		{
			MBAPI.IMBAgent.ResetGuard(this.GetPtr());
		}

		public void ResetAgentProperties()
		{
			this.AgentDrivenProperties = null;
		}

		public void ResetAiWaitBeforeShootFactor()
		{
			this._propertyModifiers.resetAiWaitBeforeShootFactor = true;
		}

		public void ClearTargetFrame()
		{
			this._checkIfTargetFrameIsChanged = false;
			if (this.MovementLockedState != AgentMovementLockedState.None)
			{
				this.ClearTargetFrameAux();
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new ClearAgentTargetFrame(this.Index));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		public void ClearEquipment()
		{
			MBAPI.IMBAgent.ClearEquipment(this.GetPtr());
		}

		public void ClearHandInverseKinematics()
		{
			MBAPI.IMBAgent.ClearHandInverseKinematics(this.GetPtr());
		}

		public void ClearAttachedWeapons()
		{
			List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>> attachedWeapons = this._attachedWeapons;
			if (attachedWeapons == null)
			{
				return;
			}
			attachedWeapons.Clear();
		}

		public void SetDetachableFromFormation(bool value)
		{
			bool isDetachableFromFormation = this._isDetachableFromFormation;
			if (isDetachableFromFormation != value)
			{
				if (isDetachableFromFormation)
				{
					if (this.IsDetachedFromFormation)
					{
						this._detachment.RemoveAgent(this);
						Formation formation = this._formation;
						if (formation != null)
						{
							formation.AttachUnit(this);
						}
					}
					Formation formation2 = this._formation;
					if (formation2 != null)
					{
						Team team = formation2.Team;
						if (team != null)
						{
							team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
						}
					}
				}
				this._isDetachableFromFormation = value;
				if (!this.IsPlayerControlled)
				{
					if (isDetachableFromFormation)
					{
						Formation formation3 = this._formation;
						if (formation3 == null)
						{
							return;
						}
						formation3.OnUndetachableNonPlayerUnitAdded(this);
						return;
					}
					else
					{
						Formation formation4 = this._formation;
						if (formation4 == null)
						{
							return;
						}
						formation4.OnUndetachableNonPlayerUnitRemoved(this);
					}
				}
			}
		}

		public void EnforceShieldUsage(Agent.UsageDirection shieldDirection)
		{
			MBAPI.IMBAgent.EnforceShieldUsage(this.GetPtr(), shieldDirection);
		}

		public bool ObjectHasVacantPosition(UsableMissionObject gameObject)
		{
			return !gameObject.HasUser || gameObject.HasAIUser;
		}

		public bool InteractingWithAnyGameObject()
		{
			return this.IsUsingGameObject || (this.IsAIControlled && this.AIInterestedInAnyGameObject());
		}

		private void StopUsingGameObjectAux(bool isSuccessful, Agent.StopUsingGameObjectFlags flags)
		{
			UsableMachine usableMachine = ((this.Controller != Agent.ControllerType.AI || this.Formation == null) ? null : (this.Formation.GetDetachmentOrDefault(this) as UsableMachine));
			if (usableMachine == null)
			{
				flags &= ~Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject;
			}
			UsableMissionObject currentlyUsedGameObject = this.CurrentlyUsedGameObject;
			UsableMissionObject usableMissionObject = null;
			if (!this.IsUsingGameObject && this.IsAIControlled)
			{
				if (this.AIMoveToGameObjectIsEnabled())
				{
					usableMissionObject = this.HumanAIComponent.GetCurrentlyMovingGameObject();
				}
				else
				{
					usableMissionObject = this.HumanAIComponent.GetCurrentlyDefendingGameObject();
				}
			}
			if (this.IsUsingGameObject)
			{
				bool flag = this.CurrentlyUsedGameObject.LockUserFrames || this.CurrentlyUsedGameObject.LockUserPositions;
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new StopUsingObject(this.Index, isSuccessful));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.CurrentlyUsedGameObject.OnUseStopped(this, isSuccessful, this._usedObjectPreferenceIndex);
				this.CurrentlyUsedGameObject = null;
				if (this.IsAIControlled)
				{
					this.AIUseGameObjectDisable();
				}
				this._usedObjectPreferenceIndex = -1;
				if (flag)
				{
					this.ClearTargetFrame();
				}
			}
			else if (this.IsAIControlled)
			{
				if (this.AIDefendGameObjectIsEnabled())
				{
					this.AIDefendGameObjectDisable();
				}
				else
				{
					this.AIMoveToGameObjectDisable();
				}
			}
			if (this.State == AgentState.Active)
			{
				if (this.IsAIControlled)
				{
					this.DisableScriptedMovement();
					if (usableMachine != null)
					{
						foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
						{
							standingPoint.FavoredUser = this;
						}
					}
				}
				this.AfterStoppedUsingMissionObject(usableMachine, currentlyUsedGameObject, usableMissionObject, isSuccessful, flags);
			}
			this.Mission.OnObjectStoppedBeingUsed(this, this.CurrentlyUsedGameObject);
			this._components.ForEach(delegate(AgentComponent ac)
			{
				ac.OnStopUsingGameObject();
			});
		}

		public void StopUsingGameObjectMT(bool isSuccessful = true, Agent.StopUsingGameObjectFlags flags = Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject)
		{
			object stopUsingGameObjectLock = Agent._stopUsingGameObjectLock;
			lock (stopUsingGameObjectLock)
			{
				this.StopUsingGameObjectAux(isSuccessful, flags);
			}
		}

		public void StopUsingGameObject(bool isSuccessful = true, Agent.StopUsingGameObjectFlags flags = Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject)
		{
			using (new TWParallel.RecursiveSingleThreadTestBlock(TWParallel.RecursiveSingleThreadTestData.GlobalData))
			{
				this.StopUsingGameObjectAux(isSuccessful, flags);
			}
		}

		public void HandleStopUsingAction()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestStopUsingObject());
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			this.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
		}

		public void HandleStartUsingAction(UsableMissionObject targetObject, int preferenceIndex)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestUseObject(targetObject.Id, preferenceIndex));
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			this.UseGameObject(targetObject, preferenceIndex);
		}

		public AgentController AddController(Type type)
		{
			AgentController agentController = null;
			if (type.IsSubclassOf(typeof(AgentController)))
			{
				agentController = Activator.CreateInstance(type) as AgentController;
			}
			if (agentController != null)
			{
				agentController.Owner = this;
				agentController.Mission = this.Mission;
				this._agentControllers.Add(agentController);
				agentController.OnInitialize();
			}
			return agentController;
		}

		public AgentController RemoveController(Type type)
		{
			for (int i = 0; i < this._agentControllers.Count; i++)
			{
				if (type.IsInstanceOfType(this._agentControllers[i]))
				{
					AgentController agentController = this._agentControllers[i];
					this._agentControllers.RemoveAt(i);
					return agentController;
				}
			}
			return null;
		}

		public bool CanThrustAttackStickToBone(BoneBodyPartType bodyPart)
		{
			if (this.IsHuman)
			{
				BoneBodyPartType[] array = new BoneBodyPartType[]
				{
					BoneBodyPartType.Abdomen,
					BoneBodyPartType.Legs,
					BoneBodyPartType.Chest,
					BoneBodyPartType.Neck,
					BoneBodyPartType.ShoulderLeft,
					BoneBodyPartType.ShoulderRight,
					BoneBodyPartType.ArmLeft,
					BoneBodyPartType.ArmRight
				};
				for (int i = 0; i < array.Length; i++)
				{
					if (bodyPart == array[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public void StartSwitchingWeaponUsageIndexAsClient(EquipmentIndex equipmentIndex, int usageIndex, Agent.UsageDirection currentMovementFlagUsageDirection)
		{
			MBAPI.IMBAgent.StartSwitchingWeaponUsageIndexAsClient(this.GetPtr(), (int)equipmentIndex, usageIndex, currentMovementFlagUsageDirection);
		}

		public void TryToWieldWeaponInSlot(EquipmentIndex slotIndex, Agent.WeaponWieldActionType type, bool isWieldedOnSpawn)
		{
			MBAPI.IMBAgent.TryToWieldWeaponInSlot(this.GetPtr(), (int)slotIndex, (int)type, isWieldedOnSpawn);
		}

		public void PrepareWeaponForDropInEquipmentSlot(EquipmentIndex slotIndex, bool dropWithHolster)
		{
			MBAPI.IMBAgent.PrepareWeaponForDropInEquipmentSlot(this.GetPtr(), (int)slotIndex, dropWithHolster);
		}

		public void AddHitter(MissionPeer peer, float damage, bool isFriendlyHit)
		{
			Agent.Hitter hitter = this._hitterList.Find((Agent.Hitter h) => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit);
			if (hitter == null)
			{
				hitter = new Agent.Hitter(peer, damage, isFriendlyHit);
				this._hitterList.Add(hitter);
				return;
			}
			hitter.IncreaseDamage(damage);
		}

		public void TryToSheathWeaponInHand(Agent.HandIndex handIndex, Agent.WeaponWieldActionType type)
		{
			MBAPI.IMBAgent.TryToSheathWeaponInHand(this.GetPtr(), (int)handIndex, (int)type);
		}

		public void RemoveHitter(MissionPeer peer, bool isFriendlyHit)
		{
			Agent.Hitter hitter = this._hitterList.Find((Agent.Hitter h) => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit);
			if (hitter != null)
			{
				this._hitterList.Remove(hitter);
			}
		}

		public void Retreat(WorldPosition retreatPos)
		{
			MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), retreatPos, true);
		}

		public void StopRetreating()
		{
			MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), WorldPosition.Invalid, false);
			this.IsRunningAway = false;
		}

		public void UseGameObject(UsableMissionObject usedObject, int preferenceIndex = -1)
		{
			if (usedObject.LockUserFrames)
			{
				WorldFrame userFrameForAgent = usedObject.GetUserFrameForAgent(this);
				this.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
			}
			else if (usedObject.LockUserPositions)
			{
				this.SetTargetPosition(usedObject.GetUserFrameForAgent(this).Origin.AsVec2);
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
			}
			if (this.IsActive() && this.IsAIControlled && this.AIMoveToGameObjectIsEnabled())
			{
				this.AIMoveToGameObjectDisable();
				Formation formation = this.Formation;
				if (formation != null)
				{
					formation.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
				}
			}
			this.CurrentlyUsedGameObject = usedObject;
			this._usedObjectPreferenceIndex = preferenceIndex;
			if (this.IsAIControlled)
			{
				this.AIUseGameObjectEnable();
			}
			this._equipmentOnMainHandBeforeUsingObject = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			this._equipmentOnOffHandBeforeUsingObject = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			usedObject.OnUse(this);
			this.Mission.OnObjectUsed(this, usedObject);
			if (usedObject.IsInstantUse && !GameNetwork.IsClientOrReplay && this.IsActive() && this.InteractingWithAnyGameObject())
			{
				this.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
		}

		public void StartFadingOut()
		{
			MBAPI.IMBAgent.StartFadingOut(this.GetPtr());
		}

		public void SetRenderCheckEnabled(bool value)
		{
			MBAPI.IMBAgent.SetRenderCheckEnabled(this.GetPtr(), value);
		}

		public bool GetRenderCheckEnabled()
		{
			return MBAPI.IMBAgent.GetRenderCheckEnabled(this.GetPtr());
		}

		public Vec3 ComputeAnimationDisplacement(float dt)
		{
			return MBAPI.IMBAgent.ComputeAnimationDisplacement(this.GetPtr(), dt);
		}

		public void TickActionChannels(float dt)
		{
			MBAPI.IMBAgent.TickActionChannels(this.GetPtr(), dt);
		}

		public void LockAgentReplicationTableDataWithCurrentReliableSequenceNo(NetworkCommunicator peer)
		{
			MBDebug.Print(string.Concat(new object[] { "peer: ", peer.UserName, " index: ", this.Index, " name: ", this.Name }), 0, Debug.DebugColor.White, 17592186044416UL);
			MBAPI.IMBAgent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(this.GetPtr(), peer.Index);
		}

		public void TeleportToPosition(Vec3 position)
		{
			if (this.MountAgent != null)
			{
				MBAPI.IMBAgent.SetPosition(this.MountAgent.GetPtr(), ref position);
			}
			MBAPI.IMBAgent.SetPosition(this.GetPtr(), ref position);
			if (this.RiderAgent != null)
			{
				MBAPI.IMBAgent.SetPosition(this.RiderAgent.GetPtr(), ref position);
			}
		}

		public void FadeOut(bool hideInstantly, bool hideMount)
		{
			MBAPI.IMBAgent.FadeOut(this.GetPtr(), hideInstantly);
			if (hideMount && this.HasMount)
			{
				this.MountAgent.FadeOut(hideMount, false);
			}
		}

		public void FadeIn()
		{
			MBAPI.IMBAgent.FadeIn(this.GetPtr());
		}

		public void DisableScriptedMovement()
		{
			MBAPI.IMBAgent.DisableScriptedMovement(this.GetPtr());
		}

		public void DisableScriptedCombatMovement()
		{
			MBAPI.IMBAgent.DisableScriptedCombatMovement(this.GetPtr());
		}

		public void ForceAiBehaviorSelection()
		{
			MBAPI.IMBAgent.ForceAiBehaviorSelection(this.GetPtr());
		}

		public bool HasPathThroughNavigationFaceIdFromDirectionMT(int navigationFaceId, Vec2 direction)
		{
			object pathCheckObjectLock = Agent._pathCheckObjectLock;
			bool flag2;
			lock (pathCheckObjectLock)
			{
				flag2 = MBAPI.IMBAgent.HasPathThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction);
			}
			return flag2;
		}

		public bool HasPathThroughNavigationFaceIdFromDirection(int navigationFaceId, Vec2 direction)
		{
			return MBAPI.IMBAgent.HasPathThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction);
		}

		public void DisableLookToPointOfInterest()
		{
			MBAPI.IMBAgent.DisableLookToPointOfInterest(this.GetPtr());
		}

		public CompositeComponent AddPrefabComponentToBone(string prefabName, sbyte boneIndex)
		{
			return MBAPI.IMBAgent.AddPrefabToAgentBone(this.GetPtr(), prefabName, boneIndex);
		}

		public void MakeVoice(SkinVoiceManager.SkinVoiceType voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType predictionType)
		{
			MBAPI.IMBAgent.MakeVoice(this.GetPtr(), voiceType.Index, (int)predictionType);
		}

		public void WieldNextWeapon(Agent.HandIndex weaponIndex, Agent.WeaponWieldActionType wieldActionType = Agent.WeaponWieldActionType.WithAnimation)
		{
			MBAPI.IMBAgent.WieldNextWeapon(this.GetPtr(), (int)weaponIndex, (int)wieldActionType);
		}

		public Agent.MovementControlFlag AttackDirectionToMovementFlag(Agent.UsageDirection direction)
		{
			return MBAPI.IMBAgent.AttackDirectionToMovementFlag(this.GetPtr(), direction);
		}

		public Agent.MovementControlFlag DefendDirectionToMovementFlag(Agent.UsageDirection direction)
		{
			return MBAPI.IMBAgent.DefendDirectionToMovementFlag(this.GetPtr(), direction);
		}

		public bool KickClear()
		{
			return MBAPI.IMBAgent.KickClear(this.GetPtr());
		}

		public Agent.UsageDirection PlayerAttackDirection()
		{
			return MBAPI.IMBAgent.PlayerAttackDirection(this.GetPtr());
		}

		public ValueTuple<sbyte, sbyte> GetRandomPairOfRealBloodBurstBoneIndices()
		{
			sbyte b = -1;
			sbyte b2 = -1;
			if (this.Monster.BloodBurstBoneIndices.Length != 0)
			{
				int num = MBRandom.RandomInt(this.Monster.BloodBurstBoneIndices.Length / 2);
				b = this.Monster.BloodBurstBoneIndices[num * 2];
				b2 = this.Monster.BloodBurstBoneIndices[num * 2 + 1];
			}
			return new ValueTuple<sbyte, sbyte>(b, b2);
		}

		public void CreateBloodBurstAtLimb(sbyte realBoneIndex, float scale)
		{
			MBAPI.IMBAgent.CreateBloodBurstAtLimb(this.GetPtr(), realBoneIndex, scale);
		}

		public void AddComponent(AgentComponent agentComponent)
		{
			this._components.Add(agentComponent);
			CommonAIComponent commonAIComponent;
			if ((commonAIComponent = agentComponent as CommonAIComponent) != null)
			{
				this.CommonAIComponent = commonAIComponent;
				return;
			}
			HumanAIComponent humanAIComponent;
			if ((humanAIComponent = agentComponent as HumanAIComponent) != null)
			{
				this.HumanAIComponent = humanAIComponent;
			}
		}

		public bool RemoveComponent(AgentComponent agentComponent)
		{
			bool flag = this._components.Remove(agentComponent);
			if (flag)
			{
				agentComponent.OnComponentRemoved();
				if (this.CommonAIComponent == agentComponent)
				{
					this.CommonAIComponent = null;
					return flag;
				}
				if (this.HumanAIComponent == agentComponent)
				{
					this.HumanAIComponent = null;
				}
			}
			return flag;
		}

		public void HandleTaunt(int tauntIndex, bool isDefaultTaunt)
		{
			if (tauntIndex < 0)
			{
				return;
			}
			if (isDefaultTaunt)
			{
				ActionIndexCache actionIndexCache = Agent.DefaultTauntActions[tauntIndex];
				this.SetActionChannel(1, actionIndexCache, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				this.MakeVoice(SkinVoiceManager.VoiceType.Victory, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				return;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				ActionIndexCache suitableTauntAction = CosmeticsManagerHelper.GetSuitableTauntAction(this, tauntIndex);
				if (suitableTauntAction.Index >= 0)
				{
					this.SetActionChannel(1, suitableTauntAction, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				}
			}
		}

		public void HandleBark(int indexOfBark)
		{
			if (indexOfBark < SkinVoiceManager.VoiceType.MpBarks.Length && !GameNetwork.IsClientOrReplay)
			{
				this.MakeVoice(SkinVoiceManager.VoiceType.MpBarks[indexOfBark], SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				if (GameNetwork.IsMultiplayer)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new BarkAgent(this.Index, indexOfBark));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, this.MissionPeer.GetNetworkPeer());
				}
			}
		}

		public void HandleDropWeapon(bool isDefendPressed, EquipmentIndex forcedSlotIndexToDropWeaponFrom)
		{
			Agent.ActionCodeType currentActionType = this.GetCurrentActionType(1);
			if (this.State == AgentState.Active && currentActionType != Agent.ActionCodeType.ReleaseMelee && currentActionType != Agent.ActionCodeType.ReleaseRanged && currentActionType != Agent.ActionCodeType.ReleaseThrowing && currentActionType != Agent.ActionCodeType.WeaponBash)
			{
				EquipmentIndex equipmentIndex = forcedSlotIndexToDropWeaponFrom;
				if (equipmentIndex == EquipmentIndex.None)
				{
					EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
					EquipmentIndex wieldedItemIndex2 = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
					if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot && isDefendPressed)
					{
						equipmentIndex = wieldedItemIndex2;
					}
					else if (wieldedItemIndex >= EquipmentIndex.WeaponItemBeginSlot)
					{
						equipmentIndex = wieldedItemIndex;
					}
					else if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot)
					{
						equipmentIndex = wieldedItemIndex2;
					}
					else
					{
						for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex2++)
						{
							if (!this.Equipment[equipmentIndex2].IsEmpty && this.Equipment[equipmentIndex2].Item.PrimaryWeapon.IsConsumable)
							{
								if (this.Equipment[equipmentIndex2].Item.PrimaryWeapon.IsRangedWeapon)
								{
									if (this.Equipment[equipmentIndex2].Amount == 0)
									{
										equipmentIndex = equipmentIndex2;
										break;
									}
								}
								else
								{
									bool flag = false;
									for (EquipmentIndex equipmentIndex3 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex3 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex3++)
									{
										if (!this.Equipment[equipmentIndex3].IsEmpty && this.Equipment[equipmentIndex3].HasAnyUsageWithAmmoClass(this.Equipment[equipmentIndex2].Item.PrimaryWeapon.WeaponClass) && this.Equipment[equipmentIndex2].Amount > 0)
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										equipmentIndex = equipmentIndex2;
										break;
									}
								}
							}
						}
					}
				}
				if (equipmentIndex != EquipmentIndex.None && !this.Equipment[equipmentIndex].IsEmpty)
				{
					this.DropItem(equipmentIndex, WeaponClass.Undefined);
					this.UpdateAgentProperties();
				}
			}
		}

		public void DropItem(EquipmentIndex itemIndex, WeaponClass pickedUpItemType = WeaponClass.Undefined)
		{
			if (this.Equipment[itemIndex].CurrentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.AffectsArea | WeaponFlags.Burning))
			{
				MatrixFrame boneEntitialFrameWithIndex = this.AgentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(this.Monster.MainHandItemBoneIndex);
				MatrixFrame globalFrame = this.AgentVisuals.GetGlobalFrame();
				MatrixFrame matrixFrame = globalFrame.TransformToParent(boneEntitialFrameWithIndex);
				Vec3 vec = globalFrame.origin + globalFrame.rotation.f - matrixFrame.origin;
				vec.Normalize();
				Mat3 identity = Mat3.Identity;
				identity.f = vec;
				identity.Orthonormalize();
				Mission.Current.OnAgentShootMissile(this, itemIndex, matrixFrame.origin, vec, identity, false, false, -1);
				this.RemoveEquippedWeapon(itemIndex);
				return;
			}
			MBAPI.IMBAgent.DropItem(this.GetPtr(), (int)itemIndex, (int)pickedUpItemType);
		}

		public void EquipItemsFromSpawnEquipment(bool neededBatchedItems)
		{
			this.Mission.OnEquipItemsFromSpawnEquipmentBegin(this, this._creationType);
			switch (this._creationType)
			{
			case Agent.CreationType.FromRoster:
			case Agent.CreationType.FromCharacterObj:
			{
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
				{
					WeaponData weaponData = WeaponData.InvalidWeaponData;
					WeaponStatsData[] array = null;
					WeaponData weaponData2 = WeaponData.InvalidWeaponData;
					WeaponStatsData[] array2 = null;
					if (!this.Equipment[equipmentIndex].IsEmpty)
					{
						weaponData = this.Equipment[equipmentIndex].GetWeaponData(neededBatchedItems);
						array = this.Equipment[equipmentIndex].GetWeaponStatsData();
						weaponData2 = this.Equipment[equipmentIndex].GetAmmoWeaponData(neededBatchedItems);
						array2 = this.Equipment[equipmentIndex].GetAmmoWeaponStatsData();
					}
					this.WeaponEquipped(equipmentIndex, weaponData, array, weaponData2, array2, null, true, true);
					weaponData.DeinitializeManagedPointers();
					weaponData2.DeinitializeManagedPointers();
					for (int i = 0; i < this.Equipment[equipmentIndex].GetAttachedWeaponsCount(); i++)
					{
						MatrixFrame attachedWeaponFrame = this.Equipment[equipmentIndex].GetAttachedWeaponFrame(i);
						MissionWeapon attachedWeapon = this.Equipment[equipmentIndex].GetAttachedWeapon(i);
						this.AttachWeaponToWeaponAux(equipmentIndex, ref attachedWeapon, null, ref attachedWeaponFrame);
					}
				}
				this.AddSkinMeshes(!neededBatchedItems);
				break;
			}
			}
			this.UpdateAgentProperties();
			this.Mission.OnEquipItemsFromSpawnEquipment(this, this._creationType);
			this.CheckEquipmentForCapeClothSimulationStateChange();
		}

		public void WieldInitialWeapons(Agent.WeaponWieldActionType wieldActionType = Agent.WeaponWieldActionType.InstantAfterPickUp, Equipment.InitialWeaponEquipPreference initialWeaponEquipPreference = TaleWorlds.Core.Equipment.InitialWeaponEquipPreference.Any)
		{
			EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			EquipmentIndex wieldedItemIndex2 = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			bool flag;
			this.SpawnEquipment.GetInitialWeaponIndicesToEquip(out wieldedItemIndex, out wieldedItemIndex2, out flag, initialWeaponEquipPreference);
			if (wieldedItemIndex2 != EquipmentIndex.None)
			{
				this.TryToWieldWeaponInSlot(wieldedItemIndex2, wieldActionType, true);
			}
			if (wieldedItemIndex != EquipmentIndex.None)
			{
				this.TryToWieldWeaponInSlot(wieldedItemIndex, wieldActionType, true);
				if (this.GetWieldedItemIndex(Agent.HandIndex.MainHand) == EquipmentIndex.None)
				{
					this.WieldNextWeapon(Agent.HandIndex.MainHand, wieldActionType);
				}
			}
		}

		public void ChangeWeaponHitPoints(EquipmentIndex slotIndex, short hitPoints)
		{
			this.Equipment.SetHitPointsOfSlot(slotIndex, hitPoints, false);
			this.SetWeaponHitPointsInSlot(slotIndex, hitPoints);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWeaponNetworkData(this.Index, slotIndex, hitPoints));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnWeaponHPChanged(this.Equipment[slotIndex].Item, (int)hitPoints);
			}
		}

		public bool HasWeapon()
		{
			for (int i = 0; i < 5; i++)
			{
				WeaponComponentData currentUsageItem = this.Equipment[i].CurrentUsageItem;
				if (currentUsageItem != null && currentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask))
				{
					return true;
				}
			}
			return false;
		}

		public void AttachWeaponToWeapon(EquipmentIndex slotIndex, MissionWeapon weapon, GameEntity weaponEntity, ref MatrixFrame attachLocalFrame)
		{
			this.Equipment.AttachWeaponToWeaponInSlot(slotIndex, ref weapon, ref attachLocalFrame);
			this.AttachWeaponToWeaponAux(slotIndex, ref weapon, weaponEntity, ref attachLocalFrame);
		}

		public void AttachWeaponToBone(MissionWeapon weapon, GameEntity weaponEntity, sbyte boneIndex, ref MatrixFrame attachLocalFrame)
		{
			if (this._attachedWeapons == null)
			{
				this._attachedWeapons = new List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>>();
			}
			this._attachedWeapons.Add(new ValueTuple<MissionWeapon, MatrixFrame, sbyte>(weapon, attachLocalFrame, boneIndex));
			this.AttachWeaponToBoneAux(ref weapon, weaponEntity, boneIndex, ref attachLocalFrame);
		}

		public void RestoreShieldHitPoints()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!this.Equipment[equipmentIndex].IsEmpty && this.Equipment[equipmentIndex].CurrentUsageItem.IsShield)
				{
					this.ChangeWeaponHitPoints(equipmentIndex, this.Equipment[equipmentIndex].ModifiedMaxHitPoints);
				}
			}
		}

		public void Die(Blow b, Agent.KillInfo overrideKillInfo = Agent.KillInfo.Invalid)
		{
			if (this.Formation != null)
			{
				this.Formation.Team.QuerySystem.RegisterDeath();
				if (b.IsMissile)
				{
					this.Formation.Team.QuerySystem.RegisterDeathByRanged();
				}
			}
			this.Health = 0f;
			if (overrideKillInfo != Agent.KillInfo.TeamSwitch && (b.OwnerId == -1 || b.OwnerId == this.Index) && this.IsHuman && this._lastHitInfo.CanOverrideBlow)
			{
				b.OwnerId = this._lastHitInfo.LastBlowOwnerId;
				b.AttackType = this._lastHitInfo.LastBlowAttackType;
			}
			MBAPI.IMBAgent.Die(this.GetPtr(), ref b, (sbyte)overrideKillInfo);
		}

		public void MakeDead(bool isKilled, ActionIndexValueCache actionIndex)
		{
			MBAPI.IMBAgent.MakeDead(this.GetPtr(), isKilled, actionIndex.Index);
		}

		public void RegisterBlow(Blow blow, in AttackCollisionData collisionData)
		{
			this.HandleBlow(ref blow, collisionData);
		}

		public void CreateBlowFromBlowAsReflection(in Blow blow, in AttackCollisionData collisionData, out Blow outBlow, out AttackCollisionData outCollisionData)
		{
			outBlow = blow;
			outBlow.InflictedDamage = blow.SelfInflictedDamage;
			outBlow.GlobalPosition = this.Position;
			outBlow.BoneIndex = 0;
			outBlow.BlowFlag = BlowFlags.None;
			outCollisionData = collisionData;
			outCollisionData.UpdateCollisionPositionAndBoneForReflect(collisionData.InflictedDamage, this.Position, 0);
		}

		public void Tick(float dt)
		{
			if (this.IsActive())
			{
				if (this.GetCurrentActionStage(1) == Agent.ActionStage.AttackQuickReady)
				{
					this._lastQuickReadyDetectedTime = Mission.Current.CurrentTime;
				}
				if (this._checkIfTargetFrameIsChanged)
				{
					Vec2 vec = ((this.MovementLockedState != AgentMovementLockedState.None) ? this.GetTargetPosition() : this.LookFrame.origin.AsVec2);
					Vec3 vec2 = ((this.MovementLockedState != AgentMovementLockedState.None) ? this.GetTargetDirection() : this.LookFrame.rotation.f);
					AgentMovementLockedState movementLockedState = this.MovementLockedState;
					if (movementLockedState != AgentMovementLockedState.PositionLocked)
					{
						if (movementLockedState == AgentMovementLockedState.FrameLocked)
						{
							this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vec || this._lastSynchedTargetDirection != vec2;
						}
					}
					else
					{
						this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vec;
					}
					if (this._checkIfTargetFrameIsChanged)
					{
						if (this.MovementLockedState == AgentMovementLockedState.FrameLocked)
						{
							this.SetTargetPositionAndDirection(MBMath.Lerp(vec, this._lastSynchedTargetPosition, 5f * dt, 0.005f), MBMath.Lerp(vec2, this._lastSynchedTargetDirection, 5f * dt, 0.005f));
						}
						else
						{
							this.SetTargetPosition(MBMath.Lerp(vec, this._lastSynchedTargetPosition, 5f * dt, 0.005f));
						}
					}
				}
				if (this.Mission.AllowAiTicking && this.IsAIControlled)
				{
					this.TickAsAI(dt);
				}
				if (this._wantsToYell)
				{
					if (this._yellTimer > 0f)
					{
						this._yellTimer -= dt;
					}
					else
					{
						this.MakeVoice((this.MountAgent != null) ? SkinVoiceManager.VoiceType.HorseRally : SkinVoiceManager.VoiceType.Yell, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
						this._wantsToYell = false;
					}
				}
				if (this.IsPlayerControlled && this.IsCheering && this.MovementInputVector != Vec2.Zero)
				{
					this.SetActionChannel(1, ActionIndexCache.act_none, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					return;
				}
			}
			else
			{
				MissionPeer missionPeer = this.MissionPeer;
				if (((missionPeer != null) ? missionPeer.ControlledAgent : null) == this && !this.IsCameraAttachable())
				{
					this.MissionPeer.ControlledAgent = null;
				}
			}
		}

		[Conditional("DEBUG")]
		public void DebugMore()
		{
			MBAPI.IMBAgent.DebugMore(this.GetPtr());
		}

		public void Mount(Agent mountAgent)
		{
			bool flag = mountAgent.GetCurrentActionType(0) == Agent.ActionCodeType.Rear;
			if (this.MountAgent == null && mountAgent.RiderAgent == null)
			{
				if (this.CheckSkillForMounting(mountAgent) && !flag && this.GetCurrentActionValue(0) == ActionIndexValueCache.act_none)
				{
					this.EventControlFlags |= Agent.EventControlFlag.Mount;
					this.SetInteractionAgent(mountAgent);
					return;
				}
			}
			else if (this.MountAgent == mountAgent && !flag)
			{
				this.EventControlFlags |= Agent.EventControlFlag.Dismount;
			}
		}

		public void EquipWeaponToExtraSlotAndWield(ref MissionWeapon weapon)
		{
			if (!this.Equipment[EquipmentIndex.ExtraWeaponSlot].IsEmpty)
			{
				this.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
			}
			this.EquipWeaponWithNewEntity(EquipmentIndex.ExtraWeaponSlot, ref weapon);
			this.TryToWieldWeaponInSlot(EquipmentIndex.ExtraWeaponSlot, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
		}

		public void RemoveEquippedWeapon(EquipmentIndex slotIndex)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new RemoveEquippedWeapon(this.Index, slotIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.Equipment[slotIndex] = MissionWeapon.Invalid;
			this.WeaponEquipped(slotIndex, WeaponData.InvalidWeaponData, null, WeaponData.InvalidWeaponData, null, null, true, false);
			this.UpdateAgentProperties();
		}

		public void EquipWeaponWithNewEntity(EquipmentIndex slotIndex, ref MissionWeapon weapon)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new EquipWeaponWithNewEntity(this.Index, slotIndex, weapon));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.Equipment[slotIndex] = weapon;
			WeaponData weaponData = WeaponData.InvalidWeaponData;
			WeaponStatsData[] array = null;
			WeaponData weaponData2 = WeaponData.InvalidWeaponData;
			WeaponStatsData[] array2 = null;
			if (!weapon.IsEmpty)
			{
				weaponData = weapon.GetWeaponData(true);
				array = weapon.GetWeaponStatsData();
				weaponData2 = weapon.GetAmmoWeaponData(true);
				array2 = weapon.GetAmmoWeaponStatsData();
			}
			this.WeaponEquipped(slotIndex, weaponData, array, weaponData2, array2, null, true, true);
			weaponData.DeinitializeManagedPointers();
			weaponData2.DeinitializeManagedPointers();
			for (int i = 0; i < weapon.GetAttachedWeaponsCount(); i++)
			{
				MissionWeapon attachedWeapon = weapon.GetAttachedWeapon(i);
				MatrixFrame attachedWeaponFrame = weapon.GetAttachedWeaponFrame(i);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new AttachWeaponToWeaponInAgentEquipmentSlot(attachedWeapon, this.Index, slotIndex, attachedWeaponFrame));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, null, ref attachedWeaponFrame);
			}
			this.UpdateAgentProperties();
		}

		public void EquipWeaponFromSpawnedItemEntity(EquipmentIndex slotIndex, SpawnedItemEntity spawnedItemEntity, bool removeWeapon)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new EquipWeaponFromSpawnedItemEntity(this.Index, slotIndex, spawnedItemEntity.Id, removeWeapon));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			if (spawnedItemEntity.GameEntity.Parent != null && spawnedItemEntity.GameEntity.Parent.HasScriptOfType<SpawnedItemEntity>())
			{
				SpawnedItemEntity firstScriptOfType = spawnedItemEntity.GameEntity.Parent.GetFirstScriptOfType<SpawnedItemEntity>();
				int num = -1;
				for (int i = 0; i < firstScriptOfType.GameEntity.ChildCount; i++)
				{
					if (firstScriptOfType.GameEntity.GetChild(i) == spawnedItemEntity.GameEntity)
					{
						num = i;
						break;
					}
				}
				firstScriptOfType.WeaponCopy.RemoveAttachedWeapon(num);
			}
			if (removeWeapon)
			{
				if (!this.Equipment[slotIndex].IsEmpty)
				{
					using (new TWSharedMutexWriteLock(Scene.PhysicsAndRayCastLock))
					{
						spawnedItemEntity.GameEntity.Remove(73);
						return;
					}
				}
				GameEntity gameEntity = spawnedItemEntity.GameEntity;
				using (new TWSharedMutexWriteLock(Scene.PhysicsAndRayCastLock))
				{
					gameEntity.RemovePhysicsMT(false);
				}
				gameEntity.RemoveScriptComponent(spawnedItemEntity.ScriptComponent.Pointer, 10);
				gameEntity.SetVisibilityExcludeParents(true);
				MissionWeapon weaponCopy = spawnedItemEntity.WeaponCopy;
				this.Equipment[slotIndex] = weaponCopy;
				WeaponData weaponData = weaponCopy.GetWeaponData(true);
				WeaponStatsData[] weaponStatsData = weaponCopy.GetWeaponStatsData();
				WeaponData ammoWeaponData = weaponCopy.GetAmmoWeaponData(true);
				WeaponStatsData[] ammoWeaponStatsData = weaponCopy.GetAmmoWeaponStatsData();
				this.WeaponEquipped(slotIndex, weaponData, weaponStatsData, ammoWeaponData, ammoWeaponStatsData, gameEntity, true, false);
				weaponData.DeinitializeManagedPointers();
				for (int j = 0; j < weaponCopy.GetAttachedWeaponsCount(); j++)
				{
					MatrixFrame attachedWeaponFrame = weaponCopy.GetAttachedWeaponFrame(j);
					MissionWeapon attachedWeapon = weaponCopy.GetAttachedWeapon(j);
					this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, null, ref attachedWeaponFrame);
				}
				this.UpdateAgentProperties();
			}
		}

		public void PreloadForRendering()
		{
			this.PreloadForRenderingAux();
		}

		public int AddSynchedPrefabComponentToBone(string prefabName, sbyte boneIndex)
		{
			if (this._synchedBodyComponents == null)
			{
				this._synchedBodyComponents = new List<CompositeComponent>();
			}
			if (!GameEntity.PrefabExists(prefabName))
			{
				MBDebug.ShowWarning("Missing prefab for agent logic :" + prefabName);
				prefabName = "rock_001";
			}
			CompositeComponent compositeComponent = this.AddPrefabComponentToBone(prefabName, boneIndex);
			int count = this._synchedBodyComponents.Count;
			this._synchedBodyComponents.Add(compositeComponent);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new AddPrefabComponentToAgentBone(this.Index, prefabName, boneIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			return count;
		}

		public bool WillDropWieldedShield(SpawnedItemEntity spawnedItem)
		{
			EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			if (wieldedItemIndex != EquipmentIndex.None && spawnedItem.WeaponCopy.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.NotUsableWithOneHand) && spawnedItem.WeaponCopy.HasAllUsagesWithAnyWeaponFlag(WeaponFlags.NotUsableWithOneHand))
			{
				bool flag = false;
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
				{
					if (equipmentIndex != wieldedItemIndex && !this.Equipment[equipmentIndex].IsEmpty && this.Equipment[equipmentIndex].IsShield())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public bool HadSameTypeOfConsumableOrShieldOnSpawn(WeaponClass weaponClass)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!this.SpawnEquipment[equipmentIndex].IsEmpty)
				{
					foreach (WeaponComponentData weaponComponentData in this.SpawnEquipment[equipmentIndex].Item.Weapons)
					{
						if ((weaponComponentData.IsConsumable || weaponComponentData.IsShield) && weaponComponentData.WeaponClass == weaponClass)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool CanAIWieldAsRangedWeapon(MissionWeapon weapon)
		{
			ItemObject item = weapon.Item;
			return !this.IsAIControlled || item == null || !item.ItemFlags.HasAnyFlag(ItemFlags.NotStackable);
		}

		public override int GetHashCode()
		{
			return this._creationIndex;
		}

		public bool TryGetImmediateEnemyAgentMovementData(out float maximumForwardUnlimitedSpeed, out Vec3 position)
		{
			return MBAPI.IMBAgent.TryGetImmediateEnemyAgentMovementData(this.GetPtr(), out maximumForwardUnlimitedSpeed, out position);
		}

		public bool HasLostShield()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (this.Equipment[equipmentIndex].IsEmpty && this.SpawnEquipment[equipmentIndex].Item != null && this.SpawnEquipment[equipmentIndex].Item.PrimaryWeapon.IsShield)
				{
					return true;
				}
			}
			return false;
		}

		internal void SetMountAgentBeforeBuild(Agent mount)
		{
			this.MountAgent = mount;
		}

		internal void SetMountInitialValues(TextObject name, string horseCreationKey)
		{
			this._name = name;
			this.HorseCreationKey = horseCreationKey;
		}

		internal void SetInitialAgentScale(float initialScale)
		{
			MBAPI.IMBAgent.SetAgentScale(this.GetPtr(), initialScale);
		}

		internal void InitializeAgentRecord()
		{
			MBAPI.IMBAgent.InitializeAgentRecord(this.GetPtr());
		}

		internal void OnDelete()
		{
			this._isDeleted = true;
			this.MissionPeer = null;
		}

		internal void OnFleeing()
		{
			this.RelieveFromCaptaincy();
			if (this.Formation != null)
			{
				this.Formation.Team.DetachmentManager.OnAgentRemoved(this);
				this.Formation = null;
			}
		}

		internal void OnRemove()
		{
			this._isRemoved = true;
			this._removalTime = this.Mission.CurrentTime;
			IAgentOriginBase origin = this.Origin;
			if (origin != null)
			{
				origin.OnAgentRemoved(this.Health);
			}
			this.RelieveFromCaptaincy();
			Team team = this.Team;
			if (team != null)
			{
				team.OnAgentRemoved(this);
			}
			if (this.Formation != null)
			{
				this.Formation.Team.DetachmentManager.OnAgentRemoved(this);
				this.Formation = null;
			}
			if (this.IsUsingGameObject && !GameNetwork.IsClientOrReplay && this.Mission != null && !this.Mission.MissionEnded)
			{
				this.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnAgentRemoved();
			}
		}

		internal void InitializeComponents()
		{
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.Initialize();
			}
		}

		internal void Build(AgentBuildData agentBuildData, int creationIndex)
		{
			this.BuildAux();
			this.HasBeenBuilt = true;
			this.Controller = (this.GetAgentFlags().HasAnyFlag(AgentFlag.IsHumanoid) ? agentBuildData.AgentController : Agent.ControllerType.AI);
			this.Formation = ((!this.IsMount) ? ((agentBuildData != null) ? agentBuildData.AgentFormation : null) : null);
			MissionGameModels missionGameModels = MissionGameModels.Current;
			if (missionGameModels != null)
			{
				missionGameModels.AgentStatCalculateModel.InitializeMissionEquipment(this);
			}
			this.InitializeAgentProperties(this.SpawnEquipment, agentBuildData);
			this._creationIndex = creationIndex;
			if (GameNetwork.IsServerOrRecorder)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					if (!networkCommunicator.IsMine && networkCommunicator.IsSynchronized)
					{
						this.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkCommunicator);
					}
				}
			}
		}

		private void PreloadForRenderingAux()
		{
			MBAPI.IMBAgent.PreloadForRendering(this.GetPtr());
		}

		internal void Clear()
		{
			this.Mission = null;
			this._pointer = UIntPtr.Zero;
			this._positionPointer = UIntPtr.Zero;
			this._flagsPointer = UIntPtr.Zero;
			this._indexPointer = UIntPtr.Zero;
			this._statePointer = UIntPtr.Zero;
		}

		public bool HasPathThroughNavigationFacesIDFromDirection(int navigationFaceID_1, int navigationFaceID_2, int navigationFaceID_3, Vec2 direction)
		{
			return MBAPI.IMBAgent.HasPathThroughNavigationFacesIDFromDirection(this.GetPtr(), navigationFaceID_1, navigationFaceID_2, navigationFaceID_3, ref direction);
		}

		public bool HasPathThroughNavigationFacesIDFromDirectionMT(int navigationFaceID_1, int navigationFaceID_2, int navigationFaceID_3, Vec2 direction)
		{
			object pathCheckObjectLock = Agent._pathCheckObjectLock;
			bool flag2;
			lock (pathCheckObjectLock)
			{
				flag2 = MBAPI.IMBAgent.HasPathThroughNavigationFacesIDFromDirection(this.GetPtr(), navigationFaceID_1, navigationFaceID_2, navigationFaceID_3, ref direction);
			}
			return flag2;
		}

		private void AfterStoppedUsingMissionObject(UsableMachine usableMachine, UsableMissionObject usedObject, UsableMissionObject movingToOrDefendingObject, bool isSuccessful, Agent.StopUsingGameObjectFlags flags)
		{
			if (this.IsAIControlled)
			{
				if (flags.HasAnyFlag(Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject))
				{
					Formation formation = this.Formation;
					if (formation != null)
					{
						formation.AttachUnit(this);
					}
				}
				if (flags.HasAnyFlag(Agent.StopUsingGameObjectFlags.DefendAfterStoppingUsingGameObject))
				{
					UsableMissionObject usableMissionObject = usedObject ?? movingToOrDefendingObject;
					this.AIDefendGameObjectEnable(usableMissionObject, usableMachine);
				}
			}
			StandingPoint standingPoint;
			if ((standingPoint = usedObject as StandingPoint) != null && standingPoint.AutoEquipWeaponsOnUseStopped && !flags.HasAnyFlag(Agent.StopUsingGameObjectFlags.DoNotWieldWeaponAfterStoppingUsingGameObject))
			{
				bool flag = !isSuccessful;
				bool flag2 = this._equipmentOnMainHandBeforeUsingObject != EquipmentIndex.None;
				if (this._equipmentOnOffHandBeforeUsingObject != EquipmentIndex.None)
				{
					Agent.WeaponWieldActionType weaponWieldActionType = ((flag && !flag2) ? Agent.WeaponWieldActionType.WithAnimation : Agent.WeaponWieldActionType.Instant);
					this.TryToWieldWeaponInSlot(this._equipmentOnOffHandBeforeUsingObject, weaponWieldActionType, false);
				}
				if (flag2)
				{
					Agent.WeaponWieldActionType weaponWieldActionType2 = (flag ? Agent.WeaponWieldActionType.WithAnimation : Agent.WeaponWieldActionType.Instant);
					this.TryToWieldWeaponInSlot(this._equipmentOnMainHandBeforeUsingObject, weaponWieldActionType2, false);
				}
			}
		}

		private UIntPtr GetPtr()
		{
			return this.Pointer;
		}

		private void SetWeaponHitPointsInSlot(EquipmentIndex equipmentIndex, short hitPoints)
		{
			MBAPI.IMBAgent.SetWeaponHitPointsInSlot(this.GetPtr(), (int)equipmentIndex, hitPoints);
		}

		private AgentMovementLockedState GetMovementLockedState()
		{
			return MBAPI.IMBAgent.GetMovementLockedState(this.GetPtr());
		}

		private void AttachWeaponToBoneAux(ref MissionWeapon weapon, GameEntity weaponEntity, sbyte boneIndex, ref MatrixFrame attachLocalFrame)
		{
			WeaponData weaponData = weapon.GetWeaponData(true);
			MBAPI.IMBAgent.AttachWeaponToBone(this.Pointer, weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, (weaponEntity != null) ? weaponEntity.Pointer : UIntPtr.Zero, boneIndex, ref attachLocalFrame);
			weaponData.DeinitializeManagedPointers();
		}

		private Agent GetRiderAgentAux()
		{
			return this._cachedRiderAgent;
		}

		private void AttachWeaponToWeaponAux(EquipmentIndex slotIndex, ref MissionWeapon weapon, GameEntity weaponEntity, ref MatrixFrame attachLocalFrame)
		{
			WeaponData weaponData = weapon.GetWeaponData(true);
			MBAPI.IMBAgent.AttachWeaponToWeaponInSlot(this.Pointer, weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, (weaponEntity != null) ? weaponEntity.Pointer : UIntPtr.Zero, (int)slotIndex, ref attachLocalFrame);
			weaponData.DeinitializeManagedPointers();
		}

		private Agent GetMountAgentAux()
		{
			return this._cachedMountAgent;
		}

		private void SetMountAgent(Agent mountAgent)
		{
			int num = ((mountAgent == null) ? (-1) : mountAgent.Index);
			MBAPI.IMBAgent.SetMountAgent(this.GetPtr(), num);
		}

		private void RelieveFromCaptaincy()
		{
			if (this._canLeadFormationsRemotely && this.Team != null)
			{
				using (List<Formation>.Enumerator enumerator = this.Team.FormationsIncludingSpecialAndEmpty.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation = enumerator.Current;
						if (formation.Captain == this)
						{
							formation.Captain = null;
						}
					}
					return;
				}
			}
			if (this.Formation != null && this.Formation.Captain == this)
			{
				this.Formation.Captain = null;
			}
		}

		private void SetTeamInternal(MBTeam team)
		{
			MBAPI.IMBAgent.SetTeam(this.GetPtr(), team.Index);
		}

		private Agent.ControllerType GetController()
		{
			return this._agentControllerType;
		}

		private void SetController(Agent.ControllerType controllerType)
		{
			if (controllerType != this._agentControllerType)
			{
				if (controllerType == Agent.ControllerType.Player && this.IsDetachedFromFormation)
				{
					this._detachment.RemoveAgent(this);
					Formation formation = this._formation;
					if (formation != null)
					{
						formation.AttachUnit(this);
					}
				}
				this._agentControllerType = controllerType;
				MBAPI.IMBAgent.SetController(this.GetPtr(), controllerType);
			}
		}

		private void WeaponEquipped(EquipmentIndex equipmentSlot, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, GameEntity weaponEntity, bool removeOldWeaponFromScene, bool isWieldedOnSpawn)
		{
			MBAPI.IMBAgent.WeaponEquipped(this.GetPtr(), (int)equipmentSlot, weaponData, weaponStatsData, (weaponStatsData != null) ? weaponStatsData.Length : 0, ammoWeaponData, ammoWeaponStatsData, (ammoWeaponStatsData != null) ? ammoWeaponStatsData.Length : 0, (weaponEntity != null) ? weaponEntity.Pointer : UIntPtr.Zero, removeOldWeaponFromScene, isWieldedOnSpawn);
			this.CheckEquipmentForCapeClothSimulationStateChange();
		}

		private Agent GetRiderAgent()
		{
			return MBAPI.IMBAgent.GetRiderAgent(this.GetPtr());
		}

		public void SetInitialFrame(in Vec3 initialPosition, in Vec2 initialDirection, bool canSpawnOutsideOfMissionBoundary = false)
		{
			MBAPI.IMBAgent.SetInitialFrame(this.GetPtr(), initialPosition, initialDirection, canSpawnOutsideOfMissionBoundary);
		}

		private void UpdateDrivenProperties(float[] values)
		{
			MBAPI.IMBAgent.UpdateDrivenProperties(this.GetPtr(), values);
		}

		private void UpdateLastAttackAndHitTimes(Agent attackerAgent, bool isMissile)
		{
			float currentTime = this.Mission.CurrentTime;
			if (isMissile)
			{
				this.LastRangedHitTime = currentTime;
			}
			else
			{
				this.LastMeleeHitTime = currentTime;
			}
			if (attackerAgent != this && attackerAgent != null)
			{
				if (isMissile)
				{
					attackerAgent.LastRangedAttackTime = currentTime;
					return;
				}
				attackerAgent.LastMeleeAttackTime = currentTime;
			}
		}

		private void SetNetworkPeer(NetworkCommunicator newPeer)
		{
			MBAPI.IMBAgent.SetNetworkPeer(this.GetPtr(), (newPeer != null) ? newPeer.Index : (-1));
		}

		private void ClearTargetFrameAux()
		{
			MBAPI.IMBAgent.ClearTargetFrame(this.GetPtr());
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		private void CheckUnmanagedAgentValid()
		{
			AgentHelper.GetAgentIndex(this._indexPointer);
		}

		private void BuildAux()
		{
			MBAPI.IMBAgent.Build(this.GetPtr(), this.Monster.EyeOffsetWrtHead);
		}

		private float GetMissileRangeWithHeightDifference()
		{
			if (this.IsMount || (!this.IsRangedCached && !this.HasThrownCached) || this.Formation == null || this.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				return 0f;
			}
			return this.GetMissileRangeWithHeightDifferenceAux(this.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.GetNavMeshZ());
		}

		private void AddSkinMeshes(bool useGPUMorph)
		{
			bool flag = this == Agent.Main;
			SkinMask skinMeshesMask = this.SpawnEquipment.GetSkinMeshesMask();
			bool flag2 = this.IsFemale && this.BodyPropertiesValue.Age >= 14f;
			SkinGenerationParams skinGenerationParams = new SkinGenerationParams((int)skinMeshesMask, this.SpawnEquipment.GetUnderwearType(flag2), (int)this.SpawnEquipment.BodyMeshType, (int)this.SpawnEquipment.HairCoverType, (int)this.SpawnEquipment.BeardCoverType, (int)this.SpawnEquipment.BodyDeformType, flag, this.Character.FaceDirtAmount, this.IsFemale ? 1 : 0, this.Character.Race, false, false);
			bool flag3 = this.Character != null && this.Character.FaceMeshCache;
			this.AgentVisuals.AddSkinMeshes(skinGenerationParams, this.BodyPropertiesValue, useGPUMorph, flag3);
		}

		private void HandleBlow(ref Blow b, in AttackCollisionData collisionData)
		{
			b.BaseMagnitude = MathF.Min(b.BaseMagnitude, 1000f);
			b.DamagedPercentage = (float)b.InflictedDamage / this.HealthLimit;
			Agent agent = ((b.OwnerId != -1) ? this.Mission.FindAgentWithIndex(b.OwnerId) : null);
			if (!b.BlowFlag.HasAnyFlag(BlowFlags.NoSound))
			{
				bool flag = b.IsBlowCrit(this.Monster.HitPoints * 4);
				bool flag2 = b.IsBlowLow(this.Monster.HitPoints);
				bool flag3 = agent == null || agent.IsHuman;
				bool flag4 = b.BlowFlag.HasAnyFlag(BlowFlags.NonTipThrust);
				int hitSound = b.WeaponRecord.GetHitSound(flag3, flag, flag2, flag4, b.AttackType, b.DamageType);
				float soundParameterForArmorType = Agent.GetSoundParameterForArmorType(this.GetProtectorArmorMaterialOfBone(b.BoneIndex));
				SoundEventParameter soundEventParameter = new SoundEventParameter("Armor Type", soundParameterForArmorType);
				this.Mission.MakeSound(hitSound, b.GlobalPosition, false, true, b.OwnerId, this.Index, ref soundEventParameter);
				if (b.IsMissile && agent != null)
				{
					int soundCodeMissionCombatPlayerhit = CombatSoundContainer.SoundCodeMissionCombatPlayerhit;
					this.Mission.MakeSoundOnlyOnRelatedPeer(soundCodeMissionCombatPlayerhit, b.GlobalPosition, agent.Index);
				}
				this.Mission.AddSoundAlarmFactorToAgents(b.OwnerId, b.GlobalPosition, 15f);
			}
			if (b.InflictedDamage <= 0)
			{
				return;
			}
			this.UpdateLastAttackAndHitTimes(agent, b.IsMissile);
			float health = this.Health;
			float num = (((float)b.InflictedDamage > health) ? health : ((float)b.InflictedDamage));
			float num2 = health - num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (this.CurrentMortalityState != Agent.MortalityState.Immortal && !this.Mission.DisableDying)
			{
				this.Health = num2;
			}
			if (agent != null && agent != this && this.IsHuman)
			{
				if (agent.IsMount && agent.RiderAgent != null)
				{
					this._lastHitInfo.RegisterLastBlow(agent.RiderAgent.Index, b.AttackType);
				}
				else if (agent.IsHuman)
				{
					this._lastHitInfo.RegisterLastBlow(b.OwnerId, b.AttackType);
				}
			}
			this.Mission.OnAgentHit(this, agent, b, collisionData, false, num);
			if (this.Health < 1f)
			{
				Agent.KillInfo killInfo = (b.IsFallDamage ? Agent.KillInfo.Gravity : Agent.KillInfo.Invalid);
				this.Die(b, killInfo);
			}
			this.HandleBlowAux(ref b);
		}

		private void HandleBlowAux(ref Blow b)
		{
			MBAPI.IMBAgent.HandleBlowAux(this.GetPtr(), ref b);
		}

		private ArmorComponent.ArmorMaterialTypes GetProtectorArmorMaterialOfBone(sbyte boneIndex)
		{
			if (boneIndex >= 0)
			{
				EquipmentIndex equipmentIndex = EquipmentIndex.None;
				switch (this.AgentVisuals.GetBoneTypeData(boneIndex).BodyPartType)
				{
				case BoneBodyPartType.Head:
				case BoneBodyPartType.Neck:
					equipmentIndex = EquipmentIndex.NumAllWeaponSlots;
					break;
				case BoneBodyPartType.Chest:
				case BoneBodyPartType.Abdomen:
				case BoneBodyPartType.ShoulderLeft:
				case BoneBodyPartType.ShoulderRight:
					equipmentIndex = EquipmentIndex.Body;
					break;
				case BoneBodyPartType.ArmLeft:
				case BoneBodyPartType.ArmRight:
					equipmentIndex = EquipmentIndex.Gloves;
					break;
				case BoneBodyPartType.Legs:
					equipmentIndex = EquipmentIndex.Leg;
					break;
				}
				if (equipmentIndex != EquipmentIndex.None && this.SpawnEquipment[equipmentIndex].Item != null)
				{
					return this.SpawnEquipment[equipmentIndex].Item.ArmorComponent.MaterialType;
				}
			}
			return ArmorComponent.ArmorMaterialTypes.None;
		}

		private void TickAsAI(float dt)
		{
			for (int i = 0; i < this._components.Count; i++)
			{
				this._components[i].OnTickAsAI(dt);
			}
			if (this.Formation != null && this._cachedAndFormationValuesUpdateTimer.Check(this.Mission.CurrentTime))
			{
				this.UpdateCachedAndFormationValues(false, true);
			}
		}

		private void SyncHealthToClients()
		{
			if (this.SyncHealthToAllClients && (!this.IsMount || this.RiderAgent != null))
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetAgentHealth(this.Index, (int)this.Health));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				return;
			}
			NetworkCommunicator networkCommunicator;
			if (!this.IsMount)
			{
				MissionPeer missionPeer = this.MissionPeer;
				networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
			}
			else
			{
				Agent riderAgent = this.RiderAgent;
				if (riderAgent == null)
				{
					networkCommunicator = null;
				}
				else
				{
					MissionPeer missionPeer2 = riderAgent.MissionPeer;
					networkCommunicator = ((missionPeer2 != null) ? missionPeer2.GetNetworkPeer() : null);
				}
			}
			NetworkCommunicator networkCommunicator2 = networkCommunicator;
			if (networkCommunicator2 != null && !networkCommunicator2.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkCommunicator2);
				GameNetwork.WriteMessage(new SetAgentHealth(this.Index, (int)this.Health));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		public static Agent.UsageDirection MovementFlagToDirection(Agent.MovementControlFlag flag)
		{
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackDown))
			{
				return Agent.UsageDirection.AttackDown;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackUp))
			{
				return Agent.UsageDirection.AttackUp;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackLeft))
			{
				return Agent.UsageDirection.AttackLeft;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackRight))
			{
				return Agent.UsageDirection.AttackRight;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendDown))
			{
				return Agent.UsageDirection.DefendDown;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendUp))
			{
				return Agent.UsageDirection.AttackEnd;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendLeft))
			{
				return Agent.UsageDirection.DefendLeft;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendRight))
			{
				return Agent.UsageDirection.DefendRight;
			}
			return Agent.UsageDirection.None;
		}

		public static Agent.UsageDirection GetActionDirection(int actionIndex)
		{
			return MBAPI.IMBAgent.GetActionDirection(actionIndex);
		}

		public static int GetMonsterUsageIndex(string monsterUsage)
		{
			return MBAPI.IMBAgent.GetMonsterUsageIndex(monsterUsage);
		}

		private static float GetSoundParameterForArmorType(ArmorComponent.ArmorMaterialTypes armorMaterialType)
		{
			return (float)armorMaterialType * 0.1f;
		}

		public const float BecomeTeenagerAge = 14f;

		public const float MaxMountInteractionDistance = 1.75f;

		public const float DismountVelocityLimit = 0.5f;

		public const float HealthDyingThreshold = 1f;

		public const float CachedAndFormationValuesUpdateTime = 0.5f;

		public const float MaxInteractionDistance = 3f;

		public const float MaxFocusDistance = 10f;

		private const float ChainAttackDetectionTimeout = 0.75f;

		public static readonly ActionIndexCache[] DefaultTauntActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_taunt_cheer_1"),
			ActionIndexCache.Create("act_taunt_cheer_2"),
			ActionIndexCache.Create("act_taunt_cheer_3"),
			ActionIndexCache.Create("act_taunt_cheer_4")
		};

		private static readonly object _stopUsingGameObjectLock = new object();

		private static readonly object _pathCheckObjectLock = new object();

		public Agent.OnMainAgentWieldedItemChangeDelegate OnMainAgentWieldedItemChange;

		public Action OnAgentMountedStateChanged;

		public Action OnAgentWieldedItemChange;

		public float LastDetachmentTickAgentTime;

		public MissionPeer OwningAgentMissionPeer;

		public MissionRepresentativeBase MissionRepresentative;

		private readonly MBList<AgentComponent> _components;

		private readonly Agent.CreationType _creationType;

		private readonly List<AgentController> _agentControllers;

		private readonly Timer _cachedAndFormationValuesUpdateTimer;

		private Agent.ControllerType _agentControllerType = Agent.ControllerType.AI;

		private Agent _cachedMountAgent;

		private Agent _cachedRiderAgent;

		private BasicCharacterObject _character;

		private uint? _clothingColor1;

		private uint? _clothingColor2;

		private EquipmentIndex _equipmentOnMainHandBeforeUsingObject;

		private EquipmentIndex _equipmentOnOffHandBeforeUsingObject;

		private float _defensiveness;

		private UIntPtr _positionPointer;

		private UIntPtr _pointer;

		private UIntPtr _flagsPointer;

		private UIntPtr _indexPointer;

		private UIntPtr _statePointer;

		private float _lastQuickReadyDetectedTime;

		private Agent _lookAgentCache;

		private IDetachment _detachment;

		private readonly MBList<Agent.Hitter> _hitterList;

		private List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>> _attachedWeapons;

		private float _health;

		private MissionPeer _missionPeer;

		private TextObject _name;

		private float _removalTime;

		private List<CompositeComponent> _synchedBodyComponents;

		private Formation _formation;

		private bool _checkIfTargetFrameIsChanged;

		private Agent.AgentPropertiesModifiers _propertyModifiers;

		private int _usedObjectPreferenceIndex = -1;

		private bool _isDeleted;

		private bool _wantsToYell;

		private float _yellTimer;

		private Vec3 _lastSynchedTargetDirection;

		private Vec2 _lastSynchedTargetPosition;

		private Agent.AgentLastHitInfo _lastHitInfo;

		private ClothSimulatorComponent _capeClothSimulator;

		private bool _isRemoved;

		private WeakReference<MBAgentVisuals> _visualsWeakRef = new WeakReference<MBAgentVisuals>(null);

		private int _creationIndex;

		private bool _canLeadFormationsRemotely;

		private bool _isDetachableFromFormation = true;

		private ItemObject _formationBanner;

		public float DetachmentWeight;

		public class Hitter
		{
			public float Damage { get; private set; }

			public Hitter(MissionPeer peer, float damage, bool isFriendlyHit)
			{
				this.HitterPeer = peer;
				this.Damage = damage;
				this.IsFriendlyHit = isFriendlyHit;
			}

			public void IncreaseDamage(float amount)
			{
				this.Damage += amount;
			}

			public const float AssistMinDamage = 35f;

			public readonly MissionPeer HitterPeer;

			public readonly bool IsFriendlyHit;
		}

		public struct AgentLastHitInfo
		{
			public int LastBlowOwnerId { get; private set; }

			public AgentAttackType LastBlowAttackType { get; private set; }

			public bool CanOverrideBlow
			{
				get
				{
					return this.LastBlowOwnerId >= 0 && this._lastBlowTimer.ElapsedTime <= 5f;
				}
			}

			public void Initialize()
			{
				this.LastBlowOwnerId = -1;
				this.LastBlowAttackType = AgentAttackType.Standard;
				this._lastBlowTimer = new BasicMissionTimer();
			}

			public void RegisterLastBlow(int ownerId, AgentAttackType attackType)
			{
				this._lastBlowTimer.Reset();
				this.LastBlowOwnerId = ownerId;
				this.LastBlowAttackType = attackType;
			}

			private BasicMissionTimer _lastBlowTimer;
		}

		public struct AgentPropertiesModifiers
		{
			public bool resetAiWaitBeforeShootFactor;
		}

		public struct StackArray8Agent
		{
			public Agent this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					case 5:
						return this._element5;
					case 6:
						return this._element6;
					case 7:
						return this._element7;
					default:
						return null;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					case 5:
						this._element5 = value;
						return;
					case 6:
						this._element6 = value;
						return;
					case 7:
						this._element7 = value;
						return;
					default:
						return;
					}
				}
			}

			private Agent _element0;

			private Agent _element1;

			private Agent _element2;

			private Agent _element3;

			private Agent _element4;

			private Agent _element5;

			private Agent _element6;

			private Agent _element7;

			public const int Length = 8;
		}

		public enum ActionStage
		{
			None = -1,
			AttackReady,
			AttackQuickReady,
			AttackRelease,
			ReloadMidPhase,
			ReloadLastPhase,
			Defend,
			DefendParry,
			NumActionStages
		}

		[Flags]
		public enum AIScriptedFrameFlags
		{
			None = 0,
			GoToPosition = 1,
			NoAttack = 2,
			ConsiderRotation = 4,
			NeverSlowDown = 8,
			DoNotRun = 16,
			GoWithoutMount = 32,
			RangerCanMoveForClearTarget = 128,
			InConversation = 256,
			Crouch = 512
		}

		[Flags]
		public enum AISpecialCombatModeFlags
		{
			None = 0,
			AttackEntity = 1,
			SurroundAttackEntity = 2,
			IgnoreAmmoLimitForRangeCalculation = 1024
		}

		[Flags]
		[EngineStruct("Ai_state_flag", false)]
		public enum AIStateFlag : uint
		{
			None = 0U,
			Cautious = 1U,
			Alarmed = 2U,
			Paused = 4U,
			UseObjectMoving = 8U,
			UseObjectUsing = 16U,
			UseObjectWaiting = 32U,
			Guard = 64U,
			ColumnwiseFollow = 128U
		}

		public enum WatchState
		{
			Patrolling,
			Cautious,
			Alarmed
		}

		public enum MortalityState
		{
			Mortal,
			Invulnerable,
			Immortal
		}

		[EngineStruct("Agent_controller_type", false)]
		public enum ControllerType
		{
			None,
			AI,
			Player,
			Count
		}

		public enum CreationType
		{
			Invalid,
			FromRoster,
			FromHorseObj,
			FromCharacterObj
		}

		[Flags]
		public enum EventControlFlag : uint
		{
			Dismount = 1U,
			Mount = 2U,
			Rear = 4U,
			Jump = 8U,
			Wield0 = 16U,
			Wield1 = 32U,
			Wield2 = 64U,
			Wield3 = 128U,
			Sheath0 = 256U,
			Sheath1 = 512U,
			ToggleAlternativeWeapon = 1024U,
			Walk = 2048U,
			Run = 4096U,
			Crouch = 8192U,
			Stand = 16384U,
			Kick = 32768U,
			DoubleTapToDirectionUp = 65536U,
			DoubleTapToDirectionDown = 131072U,
			DoubleTapToDirectionLeft = 196608U,
			DoubleTapToDirectionRight = 262144U,
			DoubleTapToDirectionMask = 458752U
		}

		public enum FacialAnimChannel
		{
			High,
			Mid,
			Low,
			num_facial_anim_channels
		}

		[EngineStruct("Action_code_type", false)]
		public enum ActionCodeType
		{
			Other,
			DefendFist,
			DefendShield,
			DefendForward2h,
			DefendUp2h,
			DefendRight2h,
			DefendLeft2h,
			DefendForward1h,
			DefendUp1h,
			DefendRight1h,
			DefendLeft1h,
			DefendForwardStaff,
			DefendUpStaff,
			DefendRightStaff,
			DefendLeftStaff,
			ReadyRanged,
			ReleaseRanged,
			ReleaseThrowing,
			Reload,
			ReadyMelee,
			ReleaseMelee,
			ParriedMelee,
			BlockedMelee,
			Fall,
			JumpStart,
			Jump,
			JumpEnd,
			JumpEndHard,
			Kick,
			KickContinue,
			KickHit,
			WeaponBash,
			PassiveUsage,
			EquipUnequip,
			Idle,
			Guard,
			Mount,
			Dismount,
			Dash,
			MountQuickStop,
			HitObject,
			Sit,
			SitOnTheFloor,
			SitOnAThrone,
			LadderRaise,
			LadderRaiseEnd,
			Rear,
			StrikeLight,
			StrikeMedium,
			StrikeHeavy,
			StrikeKnockBack,
			MountStrike,
			Count,
			StrikeBegin = 47,
			StrikeEnd = 51,
			DefendAllBegin = 1,
			DefendAllEnd = 15,
			AttackMeleeAllBegin = 19,
			AttackMeleeAllEnd = 23,
			CombatAllBegin = 1,
			CombatAllEnd = 23,
			JumpAllBegin,
			JumpAllEnd = 28
		}

		[EngineStruct("Agent_guard_mode", false)]
		public enum GuardMode
		{
			None = -1,
			Up,
			Down,
			Left,
			Right
		}

		public enum HandIndex
		{
			MainHand,
			OffHand
		}

		[EngineStruct("rglInt8", false)]
		public enum KillInfo : sbyte
		{
			Invalid = -1,
			Headshot,
			CouchedLance,
			Punch,
			MountHit,
			Bow,
			Crossbow,
			ThrowingAxe,
			ThrowingKnife,
			Javelin,
			Stone,
			Pistol,
			Musket,
			OneHandedSword,
			TwoHandedSword,
			OneHandedAxe,
			TwoHandedAxe,
			Mace,
			Spear,
			Morningstar,
			Maul,
			Backstabbed,
			Gravity,
			ShieldBash,
			WeaponBash,
			Kick,
			TeamSwitch
		}

		public enum MovementBehaviorType
		{
			Engaged,
			Idle,
			Flee
		}

		[Flags]
		public enum MovementControlFlag : uint
		{
			Forward = 1U,
			Backward = 2U,
			StrafeRight = 4U,
			StrafeLeft = 8U,
			TurnRight = 16U,
			TurnLeft = 32U,
			AttackLeft = 64U,
			AttackRight = 128U,
			AttackUp = 256U,
			AttackDown = 512U,
			DefendLeft = 1024U,
			DefendRight = 2048U,
			DefendUp = 4096U,
			DefendDown = 8192U,
			DefendAuto = 16384U,
			DefendBlock = 32768U,
			Action = 65536U,
			AttackMask = 960U,
			DefendMask = 31744U,
			DefendDirMask = 15360U,
			MoveMask = 63U
		}

		public enum UnderAttackType
		{
			NotUnderAttack,
			UnderMeleeAttack,
			UnderRangedAttack
		}

		[EngineStruct("Usage_direction", false)]
		public enum UsageDirection
		{
			None = -1,
			AttackUp,
			AttackDown,
			AttackLeft,
			AttackRight,
			AttackBegin = 0,
			AttackEnd = 4,
			DefendUp = 4,
			DefendDown,
			DefendLeft,
			DefendRight,
			DefendBegin = 4,
			DefendAny = 8,
			DefendEnd,
			AttackAny = 9
		}

		[EngineStruct("Weapon_wield_action_type", false)]
		public enum WeaponWieldActionType
		{
			WithAnimation,
			Instant,
			InstantAfterPickUp,
			WithAnimationUninterruptible
		}

		[Flags]
		public enum StopUsingGameObjectFlags : byte
		{
			None = 0,
			AutoAttachAfterStoppingUsingGameObject = 1,
			DoNotWieldWeaponAfterStoppingUsingGameObject = 2,
			DefendAfterStoppingUsingGameObject = 4
		}

		public delegate void OnAgentHealthChangedDelegate(Agent agent, float oldHealth, float newHealth);

		public delegate void OnMountHealthChangedDelegate(Agent agent, Agent mount, float oldHealth, float newHealth);

		public delegate void OnMainAgentWieldedItemChangeDelegate();
	}
}
