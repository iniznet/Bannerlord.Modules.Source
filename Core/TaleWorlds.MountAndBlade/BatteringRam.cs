using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	public class BatteringRam : SiegeWeapon, IPathHolder, IPrimarySiegeWeapon, IMoveableSiegeWeapon, ISpawnable
	{
		public SiegeWeaponMovementComponent MovementComponent { get; private set; }

		public FormationAI.BehaviorSide WeaponSide
		{
			get
			{
				return this._weaponSide;
			}
		}

		public string PathEntity
		{
			get
			{
				return this._pathEntityName;
			}
		}

		public bool EditorGhostEntityMove
		{
			get
			{
				return this.GhostEntityMove;
			}
		}

		public BatteringRam.RamState State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state != value)
				{
					this._state = value;
				}
			}
		}

		public MissionObject TargetCastlePosition
		{
			get
			{
				return this._gate;
			}
		}

		public bool HasCompletedAction()
		{
			return this._gate == null || this._gate.IsDestroyed || (this._gate.State == CastleGate.GateState.Open && this.HasArrivedAtTarget);
		}

		public float SiegeWeaponPriority
		{
			get
			{
				return 25f;
			}
		}

		public int OverTheWallNavMeshID
		{
			get
			{
				return this.GateNavMeshId;
			}
		}

		public bool HoldLadders
		{
			get
			{
				return !this.MovementComponent.HasArrivedAtTarget;
			}
		}

		public bool SendLadders
		{
			get
			{
				return this.MovementComponent.HasArrivedAtTarget;
			}
		}

		public bool HasArrivedAtTarget
		{
			get
			{
				return this._hasArrivedAtTarget;
			}
			set
			{
				if (!GameNetwork.IsClientOrReplay)
				{
					this.MovementComponent.SetDestinationNavMeshIdState(!value);
				}
				if (this._hasArrivedAtTarget != value)
				{
					this._hasArrivedAtTarget = value;
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetBatteringRamHasArrivedAtTarget(base.Id));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
						return;
					}
					if (GameNetwork.IsClientOrReplay)
					{
						this.MovementComponent.MoveToTargetAsClient();
					}
				}
			}
		}

		public override void Disable()
		{
			base.Disable();
			if (!GameNetwork.IsClientOrReplay)
			{
				if (this.DisabledNavMeshID != 0)
				{
					base.Scene.SetAbilityOfFacesWithId(this.DisabledNavMeshID, true);
				}
				base.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 4, false);
			}
		}

		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.Ram;
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			DestructableComponent destructableComponent = base.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
			if (destructableComponent != null)
			{
				destructableComponent.BattleSide = BattleSideEnum.Attacker;
			}
			this._state = BatteringRam.RamState.Stable;
			IEnumerable<GameEntity> enumerable = from ewgt in base.Scene.FindEntitiesWithTag(this._gateTag).ToList<GameEntity>()
				where ewgt.HasScriptOfType<CastleGate>()
				select ewgt;
			if (!enumerable.IsEmpty<GameEntity>())
			{
				this._gate = enumerable.First<GameEntity>().GetFirstScriptOfType<CastleGate>();
				this._gate.AttackerSiegeWeapon = this;
			}
			this.AddRegularMovementComponent();
			this._batteringRamBody = base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.HasTag("body"));
			this._batteringRamBodySkeleton = this._batteringRamBody.Skeleton;
			this._batteringRamBodySkeleton.SetAnimationAtChannel("batteringram_idle", 0, 1f, 0f, 0f);
			this._pullStandingPoints = new List<StandingPoint>();
			this._pullStandingPointLocalIKFrames = new List<MatrixFrame>();
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			if (base.StandingPoints != null)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					standingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none));
					if (standingPoint.GameEntity.HasTag("pull"))
					{
						standingPoint.IsDeactivated = true;
						this._pullStandingPoints.Add(standingPoint);
						this._pullStandingPointLocalIKFrames.Add(standingPoint.GameEntity.GetGlobalFrame().TransformToLocal(globalFrame));
						standingPoint.AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
					}
				}
			}
			string sideTag = this._sideTag;
			if (!(sideTag == "left"))
			{
				if (!(sideTag == "middle"))
				{
					if (!(sideTag == "right"))
					{
						this._weaponSide = FormationAI.BehaviorSide.Middle;
					}
					else
					{
						this._weaponSide = FormationAI.BehaviorSide.Right;
					}
				}
				else
				{
					this._weaponSide = FormationAI.BehaviorSide.Middle;
				}
			}
			else
			{
				this._weaponSide = FormationAI.BehaviorSide.Left;
			}
			this._ditchFillDebris = base.Scene.FindEntitiesWithTag("ditch_filler").FirstOrDefault((GameEntity df) => df.HasTag(this._sideTag));
			base.SetScriptComponentToTick(this.GetTickRequirement());
			Mission.Current.AddToWeaponListForFriendlyFirePreventing(this);
		}

		private void AddRegularMovementComponent()
		{
			this.MovementComponent = new SiegeWeaponMovementComponent
			{
				PathEntityName = this.PathEntity,
				MinSpeed = this.MinSpeed,
				MaxSpeed = this.MaxSpeed,
				MainObject = this,
				WheelDiameter = this.WheelDiameter,
				NavMeshIdToDisableOnDestination = this.NavMeshIdToDisableOnDestination,
				MovementSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/siege/batteringram/move"),
				GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier
			};
			base.AddComponent(this.MovementComponent);
		}

		protected internal override void OnDeploymentStateChanged(bool isDeployed)
		{
			base.OnDeploymentStateChanged(isDeployed);
			if (this._ditchFillDebris != null)
			{
				this._ditchFillDebris.SetVisibilityExcludeParents(isDeployed);
				if (!GameNetwork.IsClientOrReplay)
				{
					if (isDeployed)
					{
						Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_1, true);
						Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_2, true);
						Mission.Current.Scene.SeparateFacesWithId(this._ditchNavMeshID_1, this._groundToBridgeNavMeshID_1);
						Mission.Current.Scene.SeparateFacesWithId(this._ditchNavMeshID_2, this._groundToBridgeNavMeshID_2);
						Mission.Current.Scene.MergeFacesWithId(this._bridgeNavMeshID_1, this._groundToBridgeNavMeshID_1, 0);
						Mission.Current.Scene.MergeFacesWithId(this._bridgeNavMeshID_2, this._groundToBridgeNavMeshID_2, 0);
						return;
					}
					Mission.Current.Scene.SeparateFacesWithId(this._bridgeNavMeshID_1, this._groundToBridgeNavMeshID_1);
					Mission.Current.Scene.SeparateFacesWithId(this._bridgeNavMeshID_2, this._groundToBridgeNavMeshID_2);
					Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_1, false);
					Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_2, false);
					Mission.Current.Scene.MergeFacesWithId(this._ditchNavMeshID_1, this._groundToBridgeNavMeshID_1, 0);
					Mission.Current.Scene.MergeFacesWithId(this._ditchNavMeshID_2, this._groundToBridgeNavMeshID_2, 0);
				}
			}
		}

		public MatrixFrame GetInitialFrame()
		{
			if (this.MovementComponent != null)
			{
				return this.MovementComponent.GetInitialFrame();
			}
			return base.GameEntity.GetGlobalFrame();
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTickParallel(float dt)
		{
			base.OnTickParallel(dt);
			if (!base.GameEntity.IsVisibleIncludeParents())
			{
				return;
			}
			this.MovementComponent.TickParallelManually(dt);
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			for (int i = 0; i < this._pullStandingPoints.Count; i++)
			{
				StandingPoint standingPoint = this._pullStandingPoints[i];
				if (standingPoint.HasUser)
				{
					if (standingPoint.UserAgent.IsInBeingStruckAction)
					{
						standingPoint.UserAgent.ClearHandInverseKinematics();
					}
					else
					{
						Agent userAgent = standingPoint.UserAgent;
						MatrixFrame matrixFrame = this._pullStandingPointLocalIKFrames[i];
						userAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(matrixFrame, globalFrame, 0f);
					}
				}
			}
			if (this.MovementComponent.HasArrivedAtTarget && !this.IsDeactivated)
			{
				int userCountNotInStruckAction = base.UserCountNotInStruckAction;
				if (userCountNotInStruckAction > 0)
				{
					float animationParameterAtChannel = this._batteringRamBodySkeleton.GetAnimationParameterAtChannel(0);
					this.UpdateHitAnimationWithProgress((userCountNotInStruckAction - 1) / 2, animationParameterAtChannel);
				}
			}
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!base.GameEntity.IsVisibleIncludeParents())
			{
				return;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				if (this.MovementComponent.HasArrivedAtTarget && !this.HasArrivedAtTarget)
				{
					this.HasArrivedAtTarget = true;
					foreach (StandingPoint standingPoint in base.StandingPoints)
					{
						standingPoint.SetIsDeactivatedSynched(standingPoint.GameEntity.HasTag("move"));
					}
					if (this.DisabledNavMeshID != 0)
					{
						base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DisabledNavMeshID, false);
					}
				}
				if (this.MovementComponent.HasArrivedAtTarget)
				{
					if (this._gate == null || this._gate.IsDestroyed || this._gate.IsGateOpen)
					{
						if (!this._isAllStandingPointsDisabled)
						{
							foreach (StandingPoint standingPoint2 in base.StandingPoints)
							{
								standingPoint2.SetIsDeactivatedSynched(true);
							}
							this._isAllStandingPointsDisabled = true;
							return;
						}
					}
					else
					{
						if (this._isAllStandingPointsDisabled && !this.IsDeactivated)
						{
							foreach (StandingPoint standingPoint3 in base.StandingPoints)
							{
								standingPoint3.SetIsDeactivatedSynched(false);
							}
							this._isAllStandingPointsDisabled = false;
						}
						int userCountNotInStruckAction = base.UserCountNotInStruckAction;
						switch (this.State)
						{
						case BatteringRam.RamState.Stable:
							if (userCountNotInStruckAction > 0)
							{
								this.State = BatteringRam.RamState.Hitting;
								this._usedPower = userCountNotInStruckAction;
								this._storedPower = 0f;
								this.StartHitAnimationWithProgress((userCountNotInStruckAction - 1) / 2, 0f);
								return;
							}
							break;
						case BatteringRam.RamState.Hitting:
						{
							if (userCountNotInStruckAction <= 0 || this._gate == null || this._gate.IsGateOpen)
							{
								this._batteringRamBody.GetFirstScriptOfType<SynchedMissionObject>().SetAnimationAtChannelSynched("batteringram_idle", 0, 1f);
								this.State = BatteringRam.RamState.Stable;
								return;
							}
							int num = (userCountNotInStruckAction - 1) / 2;
							float animationParameterAtChannel = this._batteringRamBodySkeleton.GetAnimationParameterAtChannel(0);
							if ((this._usedPower - 1) / 2 != num)
							{
								this.StartHitAnimationWithProgress(num, animationParameterAtChannel);
							}
							this._usedPower = userCountNotInStruckAction;
							this._storedPower += (float)this._usedPower * dt;
							float num2 = ((num == 3) ? 0.53f : ((num == 2) ? 0.6f : 0.61f));
							string text = ((num == 3) ? "batteringram_fire" : ((num == 2) ? "batteringram_fire_weak" : "batteringram_fire_weakest"));
							if (animationParameterAtChannel >= num2)
							{
								MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
								float num3 = this._storedPower * this.DamageMultiplier;
								num3 /= animationParameterAtChannel * MBAnimation.GetAnimationDuration(text);
								this._gate.DestructionComponent.TriggerOnHit(base.PilotAgent, (int)num3, globalFrame.origin, globalFrame.rotation.f, MissionWeapon.Invalid, this);
								this.State = BatteringRam.RamState.AfterHit;
								return;
							}
							break;
						}
						case BatteringRam.RamState.AfterHit:
							if (this._batteringRamBodySkeleton.GetAnimationParameterAtChannel(0) > 0.999f)
							{
								this.State = BatteringRam.RamState.Stable;
							}
							break;
						default:
							return;
						}
					}
				}
			}
		}

		private void StartHitAnimationWithProgress(int powerStage, float progress)
		{
			string text = ((powerStage == 2) ? "batteringram_fire" : ((powerStage == 1) ? "batteringram_fire_weak" : "batteringram_fire_weakest"));
			this._batteringRamBody.GetFirstScriptOfType<SynchedMissionObject>().SetAnimationAtChannelSynched(text, 0, 1f);
			if (progress > 0f)
			{
				this._batteringRamBody.GetFirstScriptOfType<SynchedMissionObject>().SetAnimationChannelParameterSynched(0, progress);
			}
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				if (standingPoint.HasUser && standingPoint.GameEntity.HasTag("pull"))
				{
					ActionIndexCache actionCodeForStandingPoint = this.GetActionCodeForStandingPoint(standingPoint, powerStage);
					if (!standingPoint.UserAgent.SetActionChannel(1, actionCodeForStandingPoint, false, 0UL, 0f, 1f, -0.2f, 0.4f, progress, false, -0.2f, 0, true) && standingPoint.UserAgent.Controller == Agent.ControllerType.AI)
					{
						standingPoint.UserAgent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
			}
		}

		private void UpdateHitAnimationWithProgress(int powerStage, float progress)
		{
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				if (standingPoint.HasUser && standingPoint.GameEntity.HasTag("pull"))
				{
					ActionIndexCache actionCodeForStandingPoint = this.GetActionCodeForStandingPoint(standingPoint, powerStage);
					if (standingPoint.UserAgent.GetCurrentActionValue(1) == actionCodeForStandingPoint)
					{
						standingPoint.UserAgent.SetCurrentActionProgress(1, progress);
					}
				}
			}
		}

		private ActionIndexCache GetActionCodeForStandingPoint(StandingPoint standingPoint, int powerStage)
		{
			bool flag = standingPoint.GameEntity.HasTag("right");
			ActionIndexCache actionIndexCache = ActionIndexCache.act_none;
			switch (powerStage)
			{
			case 0:
				actionIndexCache = (flag ? BatteringRam.act_usage_batteringram_left_slowest : BatteringRam.act_usage_batteringram_right_slowest);
				break;
			case 1:
				actionIndexCache = (flag ? BatteringRam.act_usage_batteringram_left_slower : BatteringRam.act_usage_batteringram_right_slower);
				break;
			case 2:
				actionIndexCache = (flag ? BatteringRam.act_usage_batteringram_left : BatteringRam.act_usage_batteringram_right);
				break;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\BatteringRam.cs", "GetActionCodeForStandingPoint", 590);
				break;
			}
			return actionIndexCache;
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new BatteringRamAI(this);
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			this._state = BatteringRam.RamState.Stable;
			this._hasArrivedAtTarget = false;
			this._batteringRamBodySkeleton.SetAnimationAtChannel("batteringram_idle", 0, 1f, 0f, 0f);
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.IsDeactivated = !standingPoint.GameEntity.HasTag("move");
			}
		}

		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteBoolToPacket(this.HasArrivedAtTarget);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.BatteringRamStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.MovementComponent.GetTotalDistanceTraveledForPathTracker(), CompressionBasic.PositionCompressionInfo);
		}

		public override bool IsDeactivated
		{
			get
			{
				return this._gate == null || this._gate.IsDestroyed || (this._gate.State == CastleGate.GateState.Open && this.HasArrivedAtTarget) || base.IsDeactivated;
			}
		}

		public void HighlightPath()
		{
			this.MovementComponent.HighlightPath();
		}

		public void SwitchGhostEntityMovementMode(bool isGhostEnabled)
		{
			if (isGhostEnabled)
			{
				if (!this._isGhostMovementOn)
				{
					base.RemoveComponent(this.MovementComponent);
					this.SetUpGhostEntity();
					this.GhostEntityMove = true;
					SiegeWeaponMovementComponent component = base.GetComponent<SiegeWeaponMovementComponent>();
					component.GhostEntitySpeedMultiplier *= 3f;
					component.SetGhostVisibility(true);
				}
				this._isGhostMovementOn = true;
				return;
			}
			if (this._isGhostMovementOn)
			{
				base.RemoveComponent(this.MovementComponent);
				PathLastNodeFixer component2 = base.GetComponent<PathLastNodeFixer>();
				base.RemoveComponent(component2);
				this.AddRegularMovementComponent();
				this.MovementComponent.SetGhostVisibility(false);
			}
			this._isGhostMovementOn = false;
		}

		private void SetUpGhostEntity()
		{
			PathLastNodeFixer pathLastNodeFixer = new PathLastNodeFixer
			{
				PathHolder = this
			};
			base.AddComponent(pathLastNodeFixer);
			this.MovementComponent = new SiegeWeaponMovementComponent
			{
				PathEntityName = this.PathEntity,
				MainObject = this,
				GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier
			};
			base.AddComponent(this.MovementComponent);
			this.MovementComponent.SetupGhostEntity();
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return new TextObject("{=MaBSSg7I}Battering Ram", null).ToString();
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = (usableGameObject.GameEntity.HasTag("pull") ? new TextObject("{=1cnJtNTt}{KEY} Pull", null) : new TextObject("{=rwZAZSvX}{KEY} Move", null));
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override OrderType GetOrder(BattleSideEnum side)
		{
			if (base.IsDestroyed)
			{
				return OrderType.None;
			}
			if (side != BattleSideEnum.Attacker)
			{
				return OrderType.AttackEntity;
			}
			if (!this.HasCompletedAction())
			{
				return OrderType.FollowEntity;
			}
			return OrderType.Use;
		}

		public override TargetFlags GetTargetFlags()
		{
			TargetFlags targetFlags = TargetFlags.None;
			if (base.UserCountNotInStruckAction > 0)
			{
				targetFlags |= TargetFlags.IsMoving;
			}
			targetFlags |= TargetFlags.IsSiegeEngine;
			targetFlags |= TargetFlags.IsAttacker;
			if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehavior.DebugDefendState == DebugSiegeBehavior.DebugStateDefender.DebugDefendersToRam)
			{
				targetFlags |= TargetFlags.DebugThreat;
			}
			if (this.HasCompletedAction() || base.IsDestroyed || this.IsDeactivated)
			{
				targetFlags |= TargetFlags.NotAThreat;
			}
			return targetFlags;
		}

		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 300f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		protected override float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
		{
			float minimumDistanceBetweenPositions = this.GetMinimumDistanceBetweenPositions(weaponPos);
			if (minimumDistanceBetweenPositions < 100f)
			{
				return 1f;
			}
			if (minimumDistanceBetweenPositions < 625f)
			{
				return 0.8f;
			}
			return 0.6f;
		}

		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		public void AssignParametersFromSpawner(string gateTag, string sideTag, int bridgeNavMeshID_1, int bridgeNavMeshID_2, int ditchNavMeshID_1, int ditchNavMeshID_2, int groundToBridgeNavMeshID_1, int groundToBridgeNavMeshID_2, string pathEntityName)
		{
			this._gateTag = gateTag;
			this._sideTag = sideTag;
			this._bridgeNavMeshID_1 = bridgeNavMeshID_1;
			this._bridgeNavMeshID_2 = bridgeNavMeshID_2;
			this._ditchNavMeshID_1 = ditchNavMeshID_1;
			this._ditchNavMeshID_2 = ditchNavMeshID_2;
			this._groundToBridgeNavMeshID_1 = groundToBridgeNavMeshID_1;
			this._groundToBridgeNavMeshID_2 = groundToBridgeNavMeshID_2;
			this._pathEntityName = pathEntityName;
		}

		public bool GetNavmeshFaceIds(out List<int> navmeshFaceIds)
		{
			navmeshFaceIds = null;
			return false;
		}

		public override void OnAfterReadFromNetwork(ValueTuple<BaseSynchedMissionObjectReadableRecord, ISynchedMissionObjectReadableRecord> synchedMissionObjectReadableRecord)
		{
			base.OnAfterReadFromNetwork(synchedMissionObjectReadableRecord);
			BatteringRam.BatteringRamRecord batteringRamRecord = (BatteringRam.BatteringRamRecord)synchedMissionObjectReadableRecord.Item2;
			this.HasArrivedAtTarget = batteringRamRecord.HasArrivedAtTarget;
			this._state = (BatteringRam.RamState)batteringRamRecord.State;
			float num = batteringRamRecord.TotalDistanceTraveled;
			num += 0.05f;
			this.MovementComponent.SetTotalDistanceTraveledForPathTracker(num);
			this.MovementComponent.SetTargetFrameForPathTracker();
		}

		private static readonly ActionIndexCache act_usage_batteringram_left = ActionIndexCache.Create("act_usage_batteringram_left");

		private static readonly ActionIndexCache act_usage_batteringram_left_slower = ActionIndexCache.Create("act_usage_batteringram_left_slower");

		private static readonly ActionIndexCache act_usage_batteringram_left_slowest = ActionIndexCache.Create("act_usage_batteringram_left_slowest");

		private static readonly ActionIndexCache act_usage_batteringram_right = ActionIndexCache.Create("act_usage_batteringram_right");

		private static readonly ActionIndexCache act_usage_batteringram_right_slower = ActionIndexCache.Create("act_usage_batteringram_right_slower");

		private static readonly ActionIndexCache act_usage_batteringram_right_slowest = ActionIndexCache.Create("act_usage_batteringram_right_slowest");

		private string _pathEntityName = "Path";

		private const string PullStandingPointTag = "pull";

		private const string RightStandingPointTag = "right";

		private const string LeftStandingPointTag = "left";

		private const string IdleAnimation = "batteringram_idle";

		private const string KnockAnimation = "batteringram_fire";

		private const string KnockSlowerAnimation = "batteringram_fire_weak";

		private const string KnockSlowestAnimation = "batteringram_fire_weakest";

		private const float KnockAnimationHitProgress = 0.53f;

		private const float KnockSlowerAnimationHitProgress = 0.6f;

		private const float KnockSlowestAnimationHitProgress = 0.61f;

		private const string RoofTag = "roof";

		private string _gateTag = "gate";

		public bool GhostEntityMove = true;

		public float GhostEntitySpeedMultiplier = 1f;

		private string _sideTag;

		private FormationAI.BehaviorSide _weaponSide;

		public float WheelDiameter = 1.3f;

		public int GateNavMeshId = 7;

		public int DisabledNavMeshID = 8;

		private int _bridgeNavMeshID_1 = 8;

		private int _bridgeNavMeshID_2 = 8;

		private int _ditchNavMeshID_1 = 9;

		private int _ditchNavMeshID_2 = 10;

		private int _groundToBridgeNavMeshID_1 = 12;

		private int _groundToBridgeNavMeshID_2 = 13;

		public int NavMeshIdToDisableOnDestination = -1;

		public float MinSpeed = 0.5f;

		public float MaxSpeed = 1f;

		public float DamageMultiplier = 10f;

		private int _usedPower;

		private float _storedPower;

		private List<StandingPoint> _pullStandingPoints;

		private List<MatrixFrame> _pullStandingPointLocalIKFrames;

		private GameEntity _ditchFillDebris;

		private GameEntity _batteringRamBody;

		private Skeleton _batteringRamBodySkeleton;

		private bool _isGhostMovementOn;

		private bool _isAllStandingPointsDisabled;

		private BatteringRam.RamState _state;

		private CastleGate _gate;

		private bool _hasArrivedAtTarget;

		[DefineSynchedMissionObjectType(typeof(BatteringRam))]
		public struct BatteringRamRecord : ISynchedMissionObjectReadableRecord
		{
			public bool HasArrivedAtTarget { get; private set; }

			public int State { get; private set; }

			public float TotalDistanceTraveled { get; private set; }

			public bool ReadFromNetwork(ref bool bufferReadValid)
			{
				this.HasArrivedAtTarget = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
				this.State = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BatteringRamStateCompressionInfo, ref bufferReadValid);
				this.TotalDistanceTraveled = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
				return bufferReadValid;
			}
		}

		public enum RamState
		{
			Stable,
			Hitting,
			AfterHit,
			NumberOfStates
		}
	}
}
