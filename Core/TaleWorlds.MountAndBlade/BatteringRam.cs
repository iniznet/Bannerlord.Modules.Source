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
	// Token: 0x02000349 RID: 841
	public class BatteringRam : SiegeWeapon, IPathHolder, IPrimarySiegeWeapon, IMoveableSiegeWeapon, ISpawnable
	{
		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x06002CCE RID: 11470 RVA: 0x000ADFC2 File Offset: 0x000AC1C2
		// (set) Token: 0x06002CCF RID: 11471 RVA: 0x000ADFCA File Offset: 0x000AC1CA
		public SiegeWeaponMovementComponent MovementComponent { get; private set; }

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x06002CD0 RID: 11472 RVA: 0x000ADFD3 File Offset: 0x000AC1D3
		public FormationAI.BehaviorSide WeaponSide
		{
			get
			{
				return this._weaponSide;
			}
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x06002CD1 RID: 11473 RVA: 0x000ADFDB File Offset: 0x000AC1DB
		public string PathEntity
		{
			get
			{
				return this._pathEntityName;
			}
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x06002CD2 RID: 11474 RVA: 0x000ADFE3 File Offset: 0x000AC1E3
		public bool EditorGhostEntityMove
		{
			get
			{
				return this.GhostEntityMove;
			}
		}

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x06002CD3 RID: 11475 RVA: 0x000ADFEB File Offset: 0x000AC1EB
		// (set) Token: 0x06002CD4 RID: 11476 RVA: 0x000ADFF3 File Offset: 0x000AC1F3
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

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x06002CD5 RID: 11477 RVA: 0x000AE005 File Offset: 0x000AC205
		public MissionObject TargetCastlePosition
		{
			get
			{
				return this._gate;
			}
		}

		// Token: 0x06002CD6 RID: 11478 RVA: 0x000AE00D File Offset: 0x000AC20D
		public bool HasCompletedAction()
		{
			return this._gate == null || this._gate.IsDestroyed || (this._gate.State == CastleGate.GateState.Open && this.HasArrivedAtTarget);
		}

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x06002CD7 RID: 11479 RVA: 0x000AE03B File Offset: 0x000AC23B
		public float SiegeWeaponPriority
		{
			get
			{
				return 25f;
			}
		}

		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x06002CD8 RID: 11480 RVA: 0x000AE042 File Offset: 0x000AC242
		public int OverTheWallNavMeshID
		{
			get
			{
				return this.GateNavMeshId;
			}
		}

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x06002CD9 RID: 11481 RVA: 0x000AE04A File Offset: 0x000AC24A
		public bool HoldLadders
		{
			get
			{
				return !this.MovementComponent.HasArrivedAtTarget;
			}
		}

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x06002CDA RID: 11482 RVA: 0x000AE05A File Offset: 0x000AC25A
		public bool SendLadders
		{
			get
			{
				return this.MovementComponent.HasArrivedAtTarget;
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x06002CDB RID: 11483 RVA: 0x000AE067 File Offset: 0x000AC267
		// (set) Token: 0x06002CDC RID: 11484 RVA: 0x000AE070 File Offset: 0x000AC270
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
						GameNetwork.WriteMessage(new SetBatteringRamHasArrivedAtTarget(this));
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

		// Token: 0x06002CDD RID: 11485 RVA: 0x000AE0D5 File Offset: 0x000AC2D5
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

		// Token: 0x06002CDE RID: 11486 RVA: 0x000AE112 File Offset: 0x000AC312
		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.Ram;
		}

		// Token: 0x06002CDF RID: 11487 RVA: 0x000AE11C File Offset: 0x000AC31C
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

		// Token: 0x06002CE0 RID: 11488 RVA: 0x000AE384 File Offset: 0x000AC584
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

		// Token: 0x06002CE1 RID: 11489 RVA: 0x000AE408 File Offset: 0x000AC608
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

		// Token: 0x06002CE2 RID: 11490 RVA: 0x000AE57E File Offset: 0x000AC77E
		public MatrixFrame GetInitialFrame()
		{
			if (this.MovementComponent != null)
			{
				return this.MovementComponent.GetInitialFrame();
			}
			return base.GameEntity.GetGlobalFrame();
		}

		// Token: 0x06002CE3 RID: 11491 RVA: 0x000AE59F File Offset: 0x000AC79F
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002CE4 RID: 11492 RVA: 0x000AE5C0 File Offset: 0x000AC7C0
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

		// Token: 0x06002CE5 RID: 11493 RVA: 0x000AE6A0 File Offset: 0x000AC8A0
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

		// Token: 0x06002CE6 RID: 11494 RVA: 0x000AE9D8 File Offset: 0x000ACBD8
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

		// Token: 0x06002CE7 RID: 11495 RVA: 0x000AEAE0 File Offset: 0x000ACCE0
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

		// Token: 0x06002CE8 RID: 11496 RVA: 0x000AEB70 File Offset: 0x000ACD70
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
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\BatteringRam.cs", "GetActionCodeForStandingPoint", 572);
				break;
			}
			return actionIndexCache;
		}

		// Token: 0x06002CE9 RID: 11497 RVA: 0x000AEBF8 File Offset: 0x000ACDF8
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new BatteringRamAI(this);
		}

		// Token: 0x06002CEA RID: 11498 RVA: 0x000AEC00 File Offset: 0x000ACE00
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

		// Token: 0x06002CEB RID: 11499 RVA: 0x000AEC98 File Offset: 0x000ACE98
		public override bool ReadFromNetwork()
		{
			bool flag = true;
			flag = flag && base.ReadFromNetwork();
			if (flag)
			{
				bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
				int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BatteringRamStateCompressionInfo, ref flag);
				if (flag)
				{
					this.HasArrivedAtTarget = flag2;
					this._state = (BatteringRam.RamState)num;
				}
			}
			return flag;
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x000AECDE File Offset: 0x000ACEDE
		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteBoolToPacket(this.HasArrivedAtTarget);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.BatteringRamStateCompressionInfo);
		}

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x06002CED RID: 11501 RVA: 0x000AED01 File Offset: 0x000ACF01
		public override bool IsDeactivated
		{
			get
			{
				return this._gate == null || this._gate.IsDestroyed || (this._gate.State == CastleGate.GateState.Open && this.HasArrivedAtTarget) || base.IsDeactivated;
			}
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x000AED35 File Offset: 0x000ACF35
		public void HighlightPath()
		{
			this.MovementComponent.HighlightPath();
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x000AED44 File Offset: 0x000ACF44
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

		// Token: 0x06002CF0 RID: 11504 RVA: 0x000AEDD8 File Offset: 0x000ACFD8
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

		// Token: 0x06002CF1 RID: 11505 RVA: 0x000AEE3A File Offset: 0x000AD03A
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return new TextObject("{=MaBSSg7I}Battering Ram", null).ToString();
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x000AEE4C File Offset: 0x000AD04C
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = (usableGameObject.GameEntity.HasTag("pull") ? new TextObject("{=1cnJtNTt}{KEY} Pull", null) : new TextObject("{=rwZAZSvX}{KEY} Move", null));
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		// Token: 0x06002CF3 RID: 11507 RVA: 0x000AEEA0 File Offset: 0x000AD0A0
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

		// Token: 0x06002CF4 RID: 11508 RVA: 0x000AEEC0 File Offset: 0x000AD0C0
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

		// Token: 0x06002CF5 RID: 11509 RVA: 0x000AEF1C File Offset: 0x000AD11C
		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 300f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		// Token: 0x06002CF6 RID: 11510 RVA: 0x000AEF40 File Offset: 0x000AD140
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

		// Token: 0x06002CF7 RID: 11511 RVA: 0x000AEF76 File Offset: 0x000AD176
		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		// Token: 0x06002CF8 RID: 11512 RVA: 0x000AEF80 File Offset: 0x000AD180
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

		// Token: 0x06002CF9 RID: 11513 RVA: 0x000AEFD2 File Offset: 0x000AD1D2
		public bool GetNavmeshFaceIds(out List<int> navmeshFaceIds)
		{
			navmeshFaceIds = null;
			return false;
		}

		// Token: 0x04001142 RID: 4418
		private static readonly ActionIndexCache act_usage_batteringram_left = ActionIndexCache.Create("act_usage_batteringram_left");

		// Token: 0x04001143 RID: 4419
		private static readonly ActionIndexCache act_usage_batteringram_left_slower = ActionIndexCache.Create("act_usage_batteringram_left_slower");

		// Token: 0x04001144 RID: 4420
		private static readonly ActionIndexCache act_usage_batteringram_left_slowest = ActionIndexCache.Create("act_usage_batteringram_left_slowest");

		// Token: 0x04001145 RID: 4421
		private static readonly ActionIndexCache act_usage_batteringram_right = ActionIndexCache.Create("act_usage_batteringram_right");

		// Token: 0x04001146 RID: 4422
		private static readonly ActionIndexCache act_usage_batteringram_right_slower = ActionIndexCache.Create("act_usage_batteringram_right_slower");

		// Token: 0x04001147 RID: 4423
		private static readonly ActionIndexCache act_usage_batteringram_right_slowest = ActionIndexCache.Create("act_usage_batteringram_right_slowest");

		// Token: 0x04001149 RID: 4425
		private string _pathEntityName = "Path";

		// Token: 0x0400114A RID: 4426
		private const string PullStandingPointTag = "pull";

		// Token: 0x0400114B RID: 4427
		private const string RightStandingPointTag = "right";

		// Token: 0x0400114C RID: 4428
		private const string LeftStandingPointTag = "left";

		// Token: 0x0400114D RID: 4429
		private const string IdleAnimation = "batteringram_idle";

		// Token: 0x0400114E RID: 4430
		private const string KnockAnimation = "batteringram_fire";

		// Token: 0x0400114F RID: 4431
		private const string KnockSlowerAnimation = "batteringram_fire_weak";

		// Token: 0x04001150 RID: 4432
		private const string KnockSlowestAnimation = "batteringram_fire_weakest";

		// Token: 0x04001151 RID: 4433
		private const float KnockAnimationHitProgress = 0.53f;

		// Token: 0x04001152 RID: 4434
		private const float KnockSlowerAnimationHitProgress = 0.6f;

		// Token: 0x04001153 RID: 4435
		private const float KnockSlowestAnimationHitProgress = 0.61f;

		// Token: 0x04001154 RID: 4436
		private const string RoofTag = "roof";

		// Token: 0x04001155 RID: 4437
		private string _gateTag = "gate";

		// Token: 0x04001156 RID: 4438
		public bool GhostEntityMove = true;

		// Token: 0x04001157 RID: 4439
		public float GhostEntitySpeedMultiplier = 1f;

		// Token: 0x04001158 RID: 4440
		private string _sideTag;

		// Token: 0x04001159 RID: 4441
		private FormationAI.BehaviorSide _weaponSide;

		// Token: 0x0400115A RID: 4442
		public float WheelDiameter = 1.3f;

		// Token: 0x0400115B RID: 4443
		public int GateNavMeshId = 7;

		// Token: 0x0400115C RID: 4444
		public int DisabledNavMeshID = 8;

		// Token: 0x0400115D RID: 4445
		private int _bridgeNavMeshID_1 = 8;

		// Token: 0x0400115E RID: 4446
		private int _bridgeNavMeshID_2 = 8;

		// Token: 0x0400115F RID: 4447
		private int _ditchNavMeshID_1 = 9;

		// Token: 0x04001160 RID: 4448
		private int _ditchNavMeshID_2 = 10;

		// Token: 0x04001161 RID: 4449
		private int _groundToBridgeNavMeshID_1 = 12;

		// Token: 0x04001162 RID: 4450
		private int _groundToBridgeNavMeshID_2 = 13;

		// Token: 0x04001163 RID: 4451
		public int NavMeshIdToDisableOnDestination = -1;

		// Token: 0x04001164 RID: 4452
		public float MinSpeed = 0.5f;

		// Token: 0x04001165 RID: 4453
		public float MaxSpeed = 1f;

		// Token: 0x04001166 RID: 4454
		public float DamageMultiplier = 10f;

		// Token: 0x04001167 RID: 4455
		private int _usedPower;

		// Token: 0x04001168 RID: 4456
		private float _storedPower;

		// Token: 0x04001169 RID: 4457
		private List<StandingPoint> _pullStandingPoints;

		// Token: 0x0400116A RID: 4458
		private List<MatrixFrame> _pullStandingPointLocalIKFrames;

		// Token: 0x0400116B RID: 4459
		private GameEntity _ditchFillDebris;

		// Token: 0x0400116C RID: 4460
		private GameEntity _batteringRamBody;

		// Token: 0x0400116D RID: 4461
		private Skeleton _batteringRamBodySkeleton;

		// Token: 0x0400116E RID: 4462
		private bool _isGhostMovementOn;

		// Token: 0x0400116F RID: 4463
		private bool _isAllStandingPointsDisabled;

		// Token: 0x04001170 RID: 4464
		private BatteringRam.RamState _state;

		// Token: 0x04001171 RID: 4465
		private CastleGate _gate;

		// Token: 0x04001172 RID: 4466
		private bool _hasArrivedAtTarget;

		// Token: 0x0200064D RID: 1613
		public enum RamState
		{
			// Token: 0x0400206C RID: 8300
			Stable,
			// Token: 0x0400206D RID: 8301
			Hitting,
			// Token: 0x0400206E RID: 8302
			AfterHit,
			// Token: 0x0400206F RID: 8303
			NumberOfStates
		}
	}
}
