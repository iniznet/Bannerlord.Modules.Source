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
	public class SiegeTower : SiegeWeapon, IPathHolder, IPrimarySiegeWeapon, IMoveableSiegeWeapon, ISpawnable
	{
		public MissionObject TargetCastlePosition
		{
			get
			{
				return this._targetWallSegment;
			}
		}

		private GameEntity CleanState
		{
			get
			{
				if (!(this._cleanState == null))
				{
					return this._cleanState;
				}
				return base.GameEntity;
			}
			set
			{
				this._cleanState = value;
			}
		}

		public FormationAI.BehaviorSide WeaponSide { get; private set; }

		public string PathEntity { get; private set; }

		public bool EditorGhostEntityMove
		{
			get
			{
				return this.GhostEntityMove;
			}
		}

		public bool HasCompletedAction()
		{
			return !base.IsDisabled && this.IsDeactivated && this._hasArrivedAtTarget && !base.IsDestroyed;
		}

		public float SiegeWeaponPriority
		{
			get
			{
				return 20f;
			}
		}

		public int OverTheWallNavMeshID
		{
			get
			{
				return this.GetGateNavMeshId();
			}
		}

		public SiegeWeaponMovementComponent MovementComponent { get; private set; }

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

		public int GetGateNavMeshId()
		{
			if (this.GateNavMeshId != 0)
			{
				return this.GateNavMeshId;
			}
			if (this.DynamicNavmeshIdStart == 0)
			{
				return 0;
			}
			return this.DynamicNavmeshIdStart + 3;
		}

		public List<int> CollectGetDifficultNavmeshIDs()
		{
			List<int> list = new List<int>();
			if (!this._hasLadders)
			{
				return list;
			}
			list.Add(this.DynamicNavmeshIdStart + 1);
			list.Add(this.DynamicNavmeshIdStart + 5);
			list.Add(this.DynamicNavmeshIdStart + 6);
			list.Add(this.DynamicNavmeshIdStart + 7);
			return list;
		}

		public List<int> CollectGetDifficultNavmeshIDsForAttackers()
		{
			List<int> list = new List<int>();
			if (!this._hasLadders)
			{
				return list;
			}
			list = this.CollectGetDifficultNavmeshIDs();
			list.Add(this.DynamicNavmeshIdStart + 3);
			return list;
		}

		public List<int> CollectGetDifficultNavmeshIDsForDefenders()
		{
			List<int> list = new List<int>();
			if (!this._hasLadders)
			{
				return list;
			}
			list = this.CollectGetDifficultNavmeshIDs();
			list.Add(this.DynamicNavmeshIdStart + 2);
			return list;
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
					this.MovementComponent.SetDestinationNavMeshIdState(!this.HasArrivedAtTarget);
				}
				if (this._hasArrivedAtTarget != value)
				{
					this._hasArrivedAtTarget = value;
					if (this._hasArrivedAtTarget)
					{
						this.ActiveWaitStandingPoint = base.WaitStandingPoints[1];
						if (GameNetwork.IsClientOrReplay)
						{
							goto IL_C2;
						}
						using (List<LadderQueueManager>.Enumerator enumerator = this._queueManagers.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								LadderQueueManager ladderQueueManager = enumerator.Current;
								this.CleanState.Scene.SetAbilityOfFacesWithId(ladderQueueManager.ManagedNavigationFaceId, true);
								ladderQueueManager.Activate();
							}
							goto IL_C2;
						}
					}
					if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() > 0)
					{
						this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
					}
					IL_C2:
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetSiegeTowerHasArrivedAtTarget(base.Id));
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

		public SiegeTower.GateState State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state != value)
				{
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetSiegeTowerGateState(base.Id, value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					}
					this._state = value;
					this.OnSiegeTowerGateStateChange();
				}
			}
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			if (gameEntity == null || !gameEntity.HasScriptOfType<UsableMissionObject>() || gameEntity.HasTag("move"))
			{
				return new TextObject("{=aXjlMBiE}Siege Tower", null).ToString();
			}
			return new TextObject("{=6wZUG0ev}Gate", null).ToString();
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = (usableGameObject.GameEntity.HasTag("move") ? new TextObject("{=rwZAZSvX}{KEY} Move", null) : new TextObject("{=5oozsaIb}{KEY} Open", null));
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteBoolToPacket(this.HasArrivedAtTarget);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeTowerGateStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this._fallAngularSpeed, CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.MovementComponent.GetTotalDistanceTraveledForPathTracker(), CompressionBasic.PositionCompressionInfo);
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
			if (this.HasCompletedAction())
			{
				return OrderType.Use;
			}
			return OrderType.FollowEntity;
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
			if (this.HasCompletedAction() || base.IsDestroyed || this.IsDeactivated)
			{
				targetFlags |= TargetFlags.NotAThreat;
			}
			if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehavior.DebugDefendState == DebugSiegeBehavior.DebugStateDefender.DebugDefendersToTower)
			{
				targetFlags |= TargetFlags.DebugThreat;
			}
			return targetFlags | TargetFlags.IsSiegeTower;
		}

		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 90f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		public override void Disable()
		{
			base.Disable();
			this.SetAbilityOfFaces(false);
			if (this._queueManagers != null)
			{
				foreach (LadderQueueManager ladderQueueManager in this._queueManagers)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(ladderQueueManager.ManagedNavigationFaceId, false);
					ladderQueueManager.DeactivateImmediate();
				}
			}
		}

		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.SiegeTower;
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new SiegeTowerAI(this);
		}

		public override bool IsDeactivated
		{
			get
			{
				return (this.MovementComponent.HasArrivedAtTarget && this.State == SiegeTower.GateState.Open) || base.IsDeactivated;
			}
		}

		protected internal override void OnDeploymentStateChanged(bool isDeployed)
		{
			base.OnDeploymentStateChanged(isDeployed);
			if (this._ditchFillDebris != null)
			{
				if (!GameNetwork.IsClientOrReplay)
				{
					this._ditchFillDebris.SetVisibleSynched(isDeployed, false);
				}
				if (!GameNetwork.IsClientOrReplay)
				{
					if (isDeployed)
					{
						if (this._soilGenericNavMeshID > 0)
						{
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilGenericNavMeshID, true);
						}
						if (this._soilNavMeshID1 > 0 && this._groundToSoilNavMeshID1 > 0 && this._ditchNavMeshID1 > 0)
						{
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID1, true);
							Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID1, this._ditchNavMeshID1, this._soilNavMeshID1);
						}
						if (this._soilNavMeshID2 > 0 && this._groundToSoilNavMeshID2 > 0 && this._ditchNavMeshID2 > 0)
						{
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID2, true);
							Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID2, this._ditchNavMeshID2, this._soilNavMeshID2);
						}
						if (this._groundGenericNavMeshID > 0)
						{
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._groundGenericNavMeshID, false);
						}
					}
					else
					{
						if (this._groundGenericNavMeshID > 0)
						{
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._groundGenericNavMeshID, true);
						}
						if (this._soilNavMeshID1 > 0 && this._groundToSoilNavMeshID1 > 0 && this._ditchNavMeshID1 > 0)
						{
							Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID1, this._soilNavMeshID1, this._ditchNavMeshID1);
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID1, false);
						}
						if (this._soilNavMeshID2 > 0 && this._groundToSoilNavMeshID2 > 0 && this._ditchNavMeshID2 > 0)
						{
							Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID2, this._soilNavMeshID2, this._ditchNavMeshID2);
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID2, false);
						}
						if (this._soilGenericNavMeshID > 0)
						{
							Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilGenericNavMeshID, false);
						}
					}
				}
			}
			if (this._sameSideSiegeLadders == null)
			{
				this._sameSideSiegeLadders = (from sl in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>()
					where sl.WeaponSide == this.WeaponSide
					select sl).ToList<SiegeLadder>();
			}
			foreach (SiegeLadder siegeLadder in this._sameSideSiegeLadders)
			{
				siegeLadder.GameEntity.SetVisibilityExcludeParents(!isDeployed);
			}
		}

		protected override void AttachDynamicNavmeshToEntity()
		{
			if (this.NavMeshPrefabName.Length > 0)
			{
				this.DynamicNavmeshIdStart = Mission.Current.GetNextDynamicNavMeshIdStart();
				this.CleanState.Scene.ImportNavigationMeshPrefab(this.NavMeshPrefabName, this.DynamicNavmeshIdStart);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 1, false, false, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 2, true, false, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 4, false, true, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 5, false, false, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 6, false, false, false);
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 7, false, false, false);
			}
		}

		protected override GameEntity GetEntityToAttachNavMeshFaces()
		{
			return this.CleanState;
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			SiegeWeaponMovementComponent movementComponent = this.MovementComponent;
			if (movementComponent == null)
			{
				return;
			}
			movementComponent.OnRemoved();
		}

		public override void SetAbilityOfFaces(bool enabled)
		{
			base.SetAbilityOfFaces(enabled);
			if (this._queueManagers != null)
			{
				foreach (LadderQueueManager ladderQueueManager in this._queueManagers)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(ladderQueueManager.ManagedNavigationFaceId, enabled);
					if (ladderQueueManager.IsDeactivated != !enabled)
					{
						if (enabled)
						{
							ladderQueueManager.Activate();
						}
						else
						{
							ladderQueueManager.DeactivateImmediate();
						}
					}
				}
			}
		}

		protected override float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
		{
			float minimumDistanceBetweenPositions = this.GetMinimumDistanceBetweenPositions(weaponPos);
			if (minimumDistanceBetweenPositions < 10f)
			{
				return 1f;
			}
			if (minimumDistanceBetweenPositions < 25f)
			{
				return 0.8f;
			}
			return 0.6f;
		}

		private bool IsNavmeshOnThisTowerAttackerDifficultNavmeshIDs(int testedNavmeshID)
		{
			return this._hasLadders && (testedNavmeshID == this.DynamicNavmeshIdStart + 1 || testedNavmeshID == this.DynamicNavmeshIdStart + 5 || testedNavmeshID == this.DynamicNavmeshIdStart + 6 || testedNavmeshID == this.DynamicNavmeshIdStart + 7 || testedNavmeshID == this.DynamicNavmeshIdStart + 3);
		}

		protected override bool IsAgentOnInconvenientNavmesh(Agent agent, StandingPoint standingPoint)
		{
			if (Mission.Current.MissionTeamAIType != Mission.MissionTeamAITypeEnum.Siege)
			{
				return false;
			}
			int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
			TeamAISiegeComponent teamAISiegeComponent;
			if ((teamAISiegeComponent = agent.Team.TeamAI as TeamAISiegeComponent) != null)
			{
				if (teamAISiegeComponent is TeamAISiegeDefender && currentNavigationFaceId % 10 != 1)
				{
					return true;
				}
				foreach (int num in teamAISiegeComponent.DifficultNavmeshIDs)
				{
					if (currentNavigationFaceId == num)
					{
						return standingPoint != this._gateStandingPoint || !this.IsNavmeshOnThisTowerAttackerDifficultNavmeshIDs(currentNavigationFaceId);
					}
				}
				if (teamAISiegeComponent is TeamAISiegeAttacker && currentNavigationFaceId % 10 == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		protected internal override void OnInit()
		{
			this.CleanState = base.GameEntity.GetFirstChildEntityWithTag("body");
			base.OnInit();
			base.DestructionComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
			base.DestructionComponent.BattleSide = BattleSideEnum.Attacker;
			this._aiBarriers = base.Scene.FindEntitiesWithTag(this.BarrierTagToRemove).ToList<GameEntity>();
			if (!GameNetwork.IsClientOrReplay && this._soilGenericNavMeshID > 0)
			{
				this.CleanState.Scene.SetAbilityOfFacesWithId(this._soilGenericNavMeshID, false);
			}
			List<SynchedMissionObject> list = this.CleanState.CollectObjectsWithTag(this.GateTag);
			if (list.Count > 0)
			{
				this._gateObject = list[0];
			}
			this.AddRegularMovementComponent();
			List<GameEntity> list2 = base.Scene.FindEntitiesWithTag("breakable_wall").ToList<GameEntity>();
			if (!list2.IsEmpty<GameEntity>())
			{
				float num = 10000000f;
				GameEntity gameEntity = null;
				MatrixFrame targetFrame = this.MovementComponent.GetTargetFrame();
				foreach (GameEntity gameEntity2 in list2)
				{
					float lengthSquared = (gameEntity2.GlobalPosition - targetFrame.origin).LengthSquared;
					if (lengthSquared < num)
					{
						num = lengthSquared;
						gameEntity = gameEntity2;
					}
				}
				list2 = gameEntity.CollectChildrenEntitiesWithTag("destroyed");
				if (list2.Count > 0)
				{
					this._destroyedWallEntity = list2[0];
				}
				list2 = gameEntity.CollectChildrenEntitiesWithTag("non_destroyed");
				if (list2.Count > 0)
				{
					this._nonDestroyedWallEntity = list2[0];
				}
				list2 = gameEntity.CollectChildrenEntitiesWithTag("particle_spawnpoint");
				if (list2.Count > 0)
				{
					this._battlementDestroyedParticle = list2[0];
				}
			}
			list = this.CleanState.CollectObjectsWithTag(this.HandleTag);
			this._handleObject = ((list.Count < 1) ? null : list[0]);
			this._gateHandleIdleAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.GateHandleIdleAnimation);
			this._gateTrembleAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.GateTrembleAnimation);
			this._queueManagers = new List<LadderQueueManager>();
			if (!GameNetwork.IsClientOrReplay)
			{
				List<GameEntity> list3 = this.CleanState.CollectChildrenEntitiesWithTag("ladder");
				if (list3.Count > 0)
				{
					this._hasLadders = true;
					GameEntity gameEntity3 = list3.ElementAt(list3.Count / 2);
					foreach (GameEntity gameEntity4 in list3)
					{
						if (gameEntity4.Name.Contains("middle"))
						{
							gameEntity3 = gameEntity4;
						}
						else
						{
							LadderQueueManager ladderQueueManager = gameEntity4.GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
							ladderQueueManager.Initialize(-1, MatrixFrame.Identity, Vec3.Zero, BattleSideEnum.None, int.MaxValue, 1f, 5f, 5f, 5f, 0f, false, 1f, 0f, 0f, false, -1, -1, int.MaxValue, int.MaxValue);
							ladderQueueManager.DeactivateImmediate();
						}
					}
					int num2 = 0;
					int num3 = 1;
					for (int i = base.GameEntity.Name.Length - 1; i >= 0; i--)
					{
						if (char.IsDigit(base.GameEntity.Name[i]))
						{
							num2 += (int)(base.GameEntity.Name[i] - '0') * num3;
							num3 *= 10;
						}
						else if (num2 > 0)
						{
							break;
						}
					}
					LadderQueueManager ladderQueueManager2 = gameEntity3.GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
					if (ladderQueueManager2 != null)
					{
						MatrixFrame identity = MatrixFrame.Identity;
						identity.rotation.RotateAboutSide(1.5707964f);
						identity.rotation.RotateAboutForward(0.3926991f);
						ladderQueueManager2.Initialize(this.DynamicNavmeshIdStart + 5, identity, new Vec3(0f, 0f, 1f, -1f), BattleSideEnum.Attacker, list3.Count * 2, 0.7853982f, 2f, 1f, 4f, 3f, false, 0.8f, (float)num2 * 2f / 3f, 5f, list3.Count > 1, this.DynamicNavmeshIdStart + 6, this.DynamicNavmeshIdStart + 7, num2 * MathF.Round((float)list3.Count * 0.666f), list3.Count + 1);
						this._queueManagers.Add(ladderQueueManager2);
					}
					base.GameEntity.Scene.MarkFacesWithIdAsLadder(5, true);
					base.GameEntity.Scene.MarkFacesWithIdAsLadder(6, true);
					base.GameEntity.Scene.MarkFacesWithIdAsLadder(7, true);
				}
				else
				{
					this._hasLadders = false;
					LadderQueueManager ladderQueueManager3 = this.CleanState.GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
					if (ladderQueueManager3 != null)
					{
						MatrixFrame identity2 = MatrixFrame.Identity;
						identity2.origin.y = identity2.origin.y + 4f;
						identity2.rotation.RotateAboutSide(-1.5707964f);
						identity2.rotation.RotateAboutUp(3.1415927f);
						ladderQueueManager3.Initialize(this.DynamicNavmeshIdStart + 2, identity2, new Vec3(0f, -1f, 0f, -1f), BattleSideEnum.Attacker, 15, 0.7853982f, 2f, 1f, 3f, 1f, false, 0.8f, 4f, 5f, false, -2, -2, int.MaxValue, 15);
						this._queueManagers.Add(ladderQueueManager3);
					}
				}
			}
			this._state = SiegeTower.GateState.Closed;
			this._gateOpenSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/siegetower/dooropen");
			this._closedStateRotation = this._gateObject.GameEntity.GetFrame().rotation;
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none));
				if (!standingPoint.GameEntity.HasTag("move"))
				{
					this._gateStandingPoint = standingPoint;
					standingPoint.IsDeactivated = true;
					this._gateStandingPointLocalIKFrame = standingPoint.GameEntity.GetGlobalFrame().TransformToLocal(this.CleanState.GetGlobalFrame());
					standingPoint.AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
				}
			}
			if (base.WaitStandingPoints[0].GlobalPosition.z > base.WaitStandingPoints[1].GlobalPosition.z)
			{
				GameEntity gameEntity5 = base.WaitStandingPoints[0];
				base.WaitStandingPoints[0] = base.WaitStandingPoints[1];
				base.WaitStandingPoints[1] = gameEntity5;
				this.ActiveWaitStandingPoint = base.WaitStandingPoints[0];
			}
			IEnumerable<GameEntity> enumerable = from ewtwst in base.Scene.FindEntitiesWithTag(this._targetWallSegmentTag).ToList<GameEntity>()
				where ewtwst.HasScriptOfType<WallSegment>()
				select ewtwst;
			if (!enumerable.IsEmpty<GameEntity>())
			{
				this._targetWallSegment = enumerable.First<GameEntity>().GetFirstScriptOfType<WallSegment>();
				this._targetWallSegment.AttackerSiegeWeapon = this;
			}
			string sideTag = this._sideTag;
			if (!(sideTag == "left"))
			{
				if (!(sideTag == "middle"))
				{
					if (!(sideTag == "right"))
					{
						this.WeaponSide = FormationAI.BehaviorSide.Middle;
					}
					else
					{
						this.WeaponSide = FormationAI.BehaviorSide.Right;
					}
				}
				else
				{
					this.WeaponSide = FormationAI.BehaviorSide.Middle;
				}
			}
			else
			{
				this.WeaponSide = FormationAI.BehaviorSide.Left;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				if (this.GetGateNavMeshId() != 0)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
				}
				foreach (LadderQueueManager ladderQueueManager4 in this._queueManagers)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(ladderQueueManager4.ManagedNavigationFaceId, false);
					ladderQueueManager4.DeactivateImmediate();
				}
			}
			GameEntity gameEntity6 = base.Scene.FindEntitiesWithTag("ditch_filler").FirstOrDefault((GameEntity df) => df.HasTag(this._sideTag));
			if (gameEntity6 != null)
			{
				this._ditchFillDebris = gameEntity6.GetFirstScriptOfType<SynchedMissionObject>();
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				this._gateObject.GameEntity.AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 3, true, false, false);
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
			Mission.Current.AddToWeaponListForFriendlyFirePreventing(this);
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!this.CleanState.IsVisibleIncludeParents())
			{
				return;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					if (standingPoint.GameEntity.HasTag("move"))
					{
						standingPoint.SetIsDeactivatedSynched(this.MovementComponent.HasArrivedAtTarget);
					}
					else
					{
						UsableMissionObject usableMissionObject = standingPoint;
						bool flag;
						if (this.MovementComponent.HasArrivedAtTarget && this.State != SiegeTower.GateState.Open)
						{
							if (this.State == SiegeTower.GateState.GateFalling || this.State == SiegeTower.GateState.GateFallingWallDestroyed)
							{
								Agent userAgent = standingPoint.UserAgent;
								flag = userAgent != null && userAgent.IsPlayerControlled;
							}
							else
							{
								flag = false;
							}
						}
						else
						{
							flag = true;
						}
						usableMissionObject.SetIsDeactivatedSynched(flag);
					}
				}
			}
			if (!GameNetwork.IsClientOrReplay && this.MovementComponent.HasArrivedAtTarget && !this.HasArrivedAtTarget)
			{
				this.HasArrivedAtTarget = true;
				this.ActiveWaitStandingPoint = base.WaitStandingPoints[1];
			}
			if (this.HasArrivedAtTarget)
			{
				switch (this.State)
				{
				case SiegeTower.GateState.Closed:
					if (!GameNetwork.IsClientOrReplay && base.UserCountNotInStruckAction > 0)
					{
						this.State = SiegeTower.GateState.GateFalling;
						return;
					}
					break;
				case SiegeTower.GateState.Open:
					break;
				case SiegeTower.GateState.GateFalling:
				{
					MatrixFrame frame = this._gateObject.GameEntity.GetFrame();
					frame.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
					this._gateObject.GameEntity.SetFrame(ref frame);
					if (Vec3.DotProduct(frame.rotation.u, this._openStateRotation.f) < 0.025f)
					{
						this.State = SiegeTower.GateState.GateFallingWallDestroyed;
					}
					this._fallAngularSpeed += dt * 2f * MathF.Max(0.3f, 1f - frame.rotation.u.z);
					return;
				}
				case SiegeTower.GateState.GateFallingWallDestroyed:
				{
					MatrixFrame frame2 = this._gateObject.GameEntity.GetFrame();
					frame2.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
					this._gateObject.GameEntity.SetFrame(ref frame2);
					float num = Vec3.DotProduct(frame2.rotation.u, this._openStateRotation.f);
					if (this._fallAngularSpeed > 0f && num < 0.05f)
					{
						frame2.rotation = this._openStateRotation;
						this._gateObject.GameEntity.SetFrame(ref frame2);
						this._gateObject.GameEntity.Skeleton.SetAnimationAtChannel(this._gateTrembleAnimationIndex, 0, 1f, -1f, 0f);
						SoundEvent gateOpenSound = this._gateOpenSound;
						if (gateOpenSound != null)
						{
							gateOpenSound.Stop();
						}
						if (!GameNetwork.IsClientOrReplay)
						{
							this.State = SiegeTower.GateState.Open;
						}
					}
					this._fallAngularSpeed += dt * 3f * MathF.Max(0.3f, 1f - frame2.rotation.u.z);
					return;
				}
				default:
					Debug.FailedAssert("Invalid gate state.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SiegeTower.cs", "OnTick", 974);
					break;
				}
			}
		}

		protected internal override void OnTickParallel(float dt)
		{
			base.OnTickParallel(dt);
			if (!this.CleanState.IsVisibleIncludeParents())
			{
				return;
			}
			this.MovementComponent.TickParallelManually(dt);
			if (this._gateStandingPoint.HasUser)
			{
				Agent userAgent = this._gateStandingPoint.UserAgent;
				if (userAgent.IsInBeingStruckAction)
				{
					userAgent.ClearHandInverseKinematics();
					return;
				}
				Agent userAgent2 = this._gateStandingPoint.UserAgent;
				MatrixFrame globalFrame = this.CleanState.GetGlobalFrame();
				userAgent2.SetHandInverseKinematicsFrameForMissionObjectUsage(this._gateStandingPointLocalIKFrame, globalFrame, 0f);
			}
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() > 0)
			{
				this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
			}
			this._state = SiegeTower.GateState.Closed;
			this._hasArrivedAtTarget = false;
			MatrixFrame frame = this._gateObject.GameEntity.GetFrame();
			frame.rotation = this._closedStateRotation;
			SynchedMissionObject handleObject = this._handleObject;
			if (handleObject != null)
			{
				handleObject.GameEntity.Skeleton.SetAnimationAtChannel(-1, 0, 1f, -1f, 0f);
			}
			this._gateObject.GameEntity.Skeleton.SetAnimationAtChannel(-1, 0, 1f, -1f, 0f);
			this._gateObject.GameEntity.SetFrame(ref frame);
			if (this._destroyedWallEntity != null && this._nonDestroyedWallEntity != null)
			{
				this._nonDestroyedWallEntity.SetVisibilityExcludeParents(false);
				this._destroyedWallEntity.SetVisibilityExcludeParents(true);
			}
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.IsDeactivated = !standingPoint.GameEntity.HasTag("move");
			}
		}

		public void OnDestroyed(DestructableComponent destroyedComponent, Agent destroyerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			bool flag = false;
			MissionWeapon missionWeapon = weapon;
			if (missionWeapon.CurrentUsageItem != null)
			{
				missionWeapon = weapon;
				bool flag2;
				if (missionWeapon.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.Burning))
				{
					missionWeapon = weapon;
					flag2 = missionWeapon.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.AffectsArea | WeaponFlags.AffectsAreaBig);
				}
				else
				{
					flag2 = false;
				}
				flag = flag2;
			}
			Mission.Current.KillAgentsOnEntity(destroyedComponent.CurrentState, destroyerAgent, flag);
			foreach (GameEntity gameEntity in this._aiBarriers)
			{
				gameEntity.SetVisibilityExcludeParents(true);
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
					this.GhostEntityMove = true;
					this.MovementComponent.GhostEntitySpeedMultiplier *= 3f;
					this.MovementComponent.SetGhostVisibility(true);
				}
				this._isGhostMovementOn = true;
				return;
			}
			if (this._isGhostMovementOn)
			{
				base.RemoveComponent(this.MovementComponent);
				PathLastNodeFixer component = base.GetComponent<PathLastNodeFixer>();
				base.RemoveComponent(component);
				this.AddRegularMovementComponent();
				this.MovementComponent.SetGhostVisibility(false);
			}
			this._isGhostMovementOn = false;
		}

		public MatrixFrame GetInitialFrame()
		{
			SiegeWeaponMovementComponent movementComponent = this.MovementComponent;
			if (movementComponent == null)
			{
				return this.CleanState.GetGlobalFrame();
			}
			return movementComponent.GetInitialFrame();
		}

		private void OnSiegeTowerGateStateChange()
		{
			switch (this.State)
			{
			case SiegeTower.GateState.Closed:
			{
				SynchedMissionObject handleObject = this._handleObject;
				if (handleObject != null)
				{
					handleObject.GameEntity.Skeleton.SetAnimationAtChannel(this._gateHandleIdleAnimationIndex, 0, 1f, -1f, 0f);
				}
				if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() != 0)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
					return;
				}
				break;
			}
			case SiegeTower.GateState.Open:
				if (this._gateObject.GameEntity.Skeleton.GetAnimationIndexAtChannel(0) != this._gateHandleIdleAnimationIndex)
				{
					MatrixFrame frame = this._gateObject.GameEntity.GetFrame();
					frame.rotation = this._openStateRotation;
					this._gateObject.GameEntity.SetFrame(ref frame);
					this._gateObject.GameEntity.Skeleton.SetAnimationAtChannel(this._gateTrembleAnimationIndex, 0, 1f, -1f, 0f);
					SoundEvent gateOpenSound = this._gateOpenSound;
					if (gateOpenSound != null)
					{
						gateOpenSound.Stop();
					}
					if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() != 0)
					{
						this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), true);
					}
				}
				if (!GameNetwork.IsClientOrReplay)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), true);
				}
				foreach (GameEntity gameEntity in this._aiBarriers)
				{
					gameEntity.SetVisibilityExcludeParents(false);
				}
				break;
			case SiegeTower.GateState.GateFalling:
				this._fallAngularSpeed = 0f;
				this._gateOpenSound = SoundEvent.CreateEvent(this._gateOpenSoundIndex, base.Scene);
				this._gateOpenSound.PlayInPosition(this._gateObject.GameEntity.GlobalPosition);
				return;
			case SiegeTower.GateState.GateFallingWallDestroyed:
				if (this._destroyedWallEntity != null && this._nonDestroyedWallEntity != null)
				{
					this._fallAngularSpeed *= 0.1f;
					this._nonDestroyedWallEntity.SetVisibilityExcludeParents(false);
					this._destroyedWallEntity.SetVisibilityExcludeParents(true);
					if (this._battlementDestroyedParticle != null)
					{
						Mission.Current.AddParticleSystemBurstByName(this.BattlementDestroyedParticle, this._battlementDestroyedParticle.GetGlobalFrame(), false);
						return;
					}
				}
				break;
			default:
				return;
			}
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
				MovementSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/siege/siegetower/move"),
				GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier
			};
			base.AddComponent(this.MovementComponent);
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

		private void UpdateGhostEntity()
		{
			List<GameEntity> list = this.CleanState.CollectChildrenEntitiesWithTag("ghost_object");
			if (list.Count > 0)
			{
				GameEntity gameEntity = list[0];
				if (gameEntity.ChildCount > 0)
				{
					this.MovementComponent.GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier;
					GameEntity child = gameEntity.GetChild(0);
					MatrixFrame frame = child.GetFrame();
					child.SetFrame(ref frame);
				}
			}
		}

		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		public void AssignParametersFromSpawner(string pathEntityName, string targetWallSegment, string sideTag, int soilNavMeshID1, int soilNavMeshID2, int ditchNavMeshID1, int ditchNavMeshID2, int groundToSoilNavMeshID1, int groundToSoilNavMeshID2, int soilGenericNavMeshID, int groundGenericNavMeshID, Mat3 openStateRotation, string barrierTagToRemove)
		{
			this.PathEntity = pathEntityName;
			this._targetWallSegmentTag = targetWallSegment;
			this._sideTag = sideTag;
			this._soilNavMeshID1 = soilNavMeshID1;
			this._soilNavMeshID2 = soilNavMeshID2;
			this._ditchNavMeshID1 = ditchNavMeshID1;
			this._ditchNavMeshID2 = ditchNavMeshID2;
			this._groundToSoilNavMeshID1 = groundToSoilNavMeshID1;
			this._groundToSoilNavMeshID2 = groundToSoilNavMeshID2;
			this._soilGenericNavMeshID = soilGenericNavMeshID;
			this._groundGenericNavMeshID = groundGenericNavMeshID;
			this._openStateRotation = openStateRotation;
			this.BarrierTagToRemove = barrierTagToRemove;
		}

		public override void OnAfterReadFromNetwork(ValueTuple<BaseSynchedMissionObjectReadableRecord, ISynchedMissionObjectReadableRecord> synchedMissionObjectReadableRecord)
		{
			base.OnAfterReadFromNetwork(synchedMissionObjectReadableRecord);
			SiegeTower.SiegeTowerRecord siegeTowerRecord = (SiegeTower.SiegeTowerRecord)synchedMissionObjectReadableRecord.Item2;
			this.HasArrivedAtTarget = siegeTowerRecord.HasArrivedAtTarget;
			this._state = (SiegeTower.GateState)siegeTowerRecord.State;
			this._fallAngularSpeed = siegeTowerRecord.FallAngularSpeed;
			if (this._state == SiegeTower.GateState.Open)
			{
				if (this._destroyedWallEntity != null && this._nonDestroyedWallEntity != null)
				{
					this._nonDestroyedWallEntity.SetVisibilityExcludeParents(false);
					this._destroyedWallEntity.SetVisibilityExcludeParents(true);
				}
				MatrixFrame frame = this._gateObject.GameEntity.GetFrame();
				frame.rotation = this._openStateRotation;
				this._gateObject.GameEntity.SetFrame(ref frame);
			}
			float num = siegeTowerRecord.TotalDistanceTraveled;
			num += 0.05f;
			this.MovementComponent.SetTotalDistanceTraveledForPathTracker(num);
			this.MovementComponent.SetTargetFrameForPathTracker();
		}

		public bool GetNavmeshFaceIds(out List<int> navmeshFaceIds)
		{
			navmeshFaceIds = new List<int>
			{
				this.DynamicNavmeshIdStart + 1,
				this.DynamicNavmeshIdStart + 3,
				this.DynamicNavmeshIdStart + 5,
				this.DynamicNavmeshIdStart + 6,
				this.DynamicNavmeshIdStart + 7
			};
			return true;
		}

		private const int LeftLadderNavMeshIdLocal = 5;

		private const int MiddleLadderNavMeshIdLocal = 6;

		private const int RightLadderNavMeshIdLocal = 7;

		private const string BreakableWallTag = "breakable_wall";

		private const string DestroyedWallTag = "destroyed";

		private const string NonDestroyedWallTag = "non_destroyed";

		private const string LadderTag = "ladder";

		private const string BattlementDestroyedParticleTag = "particle_spawnpoint";

		public string GateTag = "gate";

		public string GateOpenTag = "gateOpen";

		public string HandleTag = "handle";

		public string GateHandleIdleAnimation = "siegetower_handle_idle";

		private int _gateHandleIdleAnimationIndex = -1;

		public string GateTrembleAnimation = "siegetower_door_stop";

		private int _gateTrembleAnimationIndex = -1;

		public string BattlementDestroyedParticle = "psys_adobe_battlement_destroyed";

		private string _targetWallSegmentTag;

		public bool GhostEntityMove = true;

		public float GhostEntitySpeedMultiplier = 1f;

		private string _sideTag;

		private bool _hasLadders;

		public float WheelDiameter = 1.3f;

		public float MinSpeed = 0.5f;

		public float MaxSpeed = 1f;

		public int GateNavMeshId;

		public int NavMeshIdToDisableOnDestination = -1;

		private int _soilNavMeshID1;

		private int _soilNavMeshID2;

		private int _ditchNavMeshID1;

		private int _ditchNavMeshID2;

		private int _groundToSoilNavMeshID1;

		private int _groundToSoilNavMeshID2;

		private int _soilGenericNavMeshID;

		private int _groundGenericNavMeshID;

		public string BarrierTagToRemove = "barrier";

		private List<GameEntity> _aiBarriers;

		private bool _isGhostMovementOn;

		private bool _hasArrivedAtTarget;

		private SiegeTower.GateState _state;

		private SynchedMissionObject _gateObject;

		private SynchedMissionObject _handleObject;

		private SoundEvent _gateOpenSound;

		private int _gateOpenSoundIndex = -1;

		private Mat3 _openStateRotation;

		private Mat3 _closedStateRotation;

		private float _fallAngularSpeed;

		private GameEntity _cleanState;

		private GameEntity _destroyedWallEntity;

		private GameEntity _nonDestroyedWallEntity;

		private GameEntity _battlementDestroyedParticle;

		private StandingPoint _gateStandingPoint;

		private MatrixFrame _gateStandingPointLocalIKFrame;

		private SynchedMissionObject _ditchFillDebris;

		private List<LadderQueueManager> _queueManagers;

		private WallSegment _targetWallSegment;

		private List<SiegeLadder> _sameSideSiegeLadders;

		[DefineSynchedMissionObjectType(typeof(SiegeTower))]
		public struct SiegeTowerRecord : ISynchedMissionObjectReadableRecord
		{
			public bool HasArrivedAtTarget { get; private set; }

			public int State { get; private set; }

			public float FallAngularSpeed { get; private set; }

			public float TotalDistanceTraveled { get; private set; }

			public bool ReadFromNetwork(ref bool bufferReadValid)
			{
				this.HasArrivedAtTarget = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
				this.State = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeTowerGateStateCompressionInfo, ref bufferReadValid);
				this.FallAngularSpeed = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo, ref bufferReadValid);
				this.TotalDistanceTraveled = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
				return bufferReadValid;
			}
		}

		public enum GateState
		{
			Closed,
			Open,
			GateFalling,
			GateFallingWallDestroyed,
			NumberOfStates
		}
	}
}
