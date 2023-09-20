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
	// Token: 0x02000359 RID: 857
	public class SiegeTower : SiegeWeapon, IPathHolder, IPrimarySiegeWeapon, IMoveableSiegeWeapon, ISpawnable
	{
		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x06002E80 RID: 11904 RVA: 0x000BB5BF File Offset: 0x000B97BF
		public MissionObject TargetCastlePosition
		{
			get
			{
				return this._targetWallSegment;
			}
		}

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x06002E81 RID: 11905 RVA: 0x000BB5C7 File Offset: 0x000B97C7
		// (set) Token: 0x06002E82 RID: 11906 RVA: 0x000BB5E4 File Offset: 0x000B97E4
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

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x06002E83 RID: 11907 RVA: 0x000BB5ED File Offset: 0x000B97ED
		// (set) Token: 0x06002E84 RID: 11908 RVA: 0x000BB5F5 File Offset: 0x000B97F5
		public FormationAI.BehaviorSide WeaponSide { get; private set; }

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06002E85 RID: 11909 RVA: 0x000BB5FE File Offset: 0x000B97FE
		// (set) Token: 0x06002E86 RID: 11910 RVA: 0x000BB606 File Offset: 0x000B9806
		public string PathEntity { get; private set; }

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x06002E87 RID: 11911 RVA: 0x000BB60F File Offset: 0x000B980F
		public bool EditorGhostEntityMove
		{
			get
			{
				return this.GhostEntityMove;
			}
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x000BB617 File Offset: 0x000B9817
		public bool HasCompletedAction()
		{
			return !base.IsDisabled && this.IsDeactivated && !base.IsDestroyed;
		}

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x06002E89 RID: 11913 RVA: 0x000BB634 File Offset: 0x000B9834
		public float SiegeWeaponPriority
		{
			get
			{
				return 20f;
			}
		}

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x06002E8A RID: 11914 RVA: 0x000BB63B File Offset: 0x000B983B
		public int OverTheWallNavMeshID
		{
			get
			{
				return this.GetGateNavMeshId();
			}
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x06002E8B RID: 11915 RVA: 0x000BB643 File Offset: 0x000B9843
		// (set) Token: 0x06002E8C RID: 11916 RVA: 0x000BB64B File Offset: 0x000B984B
		public SiegeWeaponMovementComponent MovementComponent { get; private set; }

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x06002E8D RID: 11917 RVA: 0x000BB654 File Offset: 0x000B9854
		public bool HoldLadders
		{
			get
			{
				return !this.MovementComponent.HasArrivedAtTarget;
			}
		}

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x06002E8E RID: 11918 RVA: 0x000BB664 File Offset: 0x000B9864
		public bool SendLadders
		{
			get
			{
				return this.MovementComponent.HasArrivedAtTarget;
			}
		}

		// Token: 0x06002E8F RID: 11919 RVA: 0x000BB671 File Offset: 0x000B9871
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

		// Token: 0x06002E90 RID: 11920 RVA: 0x000BB694 File Offset: 0x000B9894
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

		// Token: 0x06002E91 RID: 11921 RVA: 0x000BB6EC File Offset: 0x000B98EC
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

		// Token: 0x06002E92 RID: 11922 RVA: 0x000BB720 File Offset: 0x000B9920
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

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x06002E93 RID: 11923 RVA: 0x000BB753 File Offset: 0x000B9953
		// (set) Token: 0x06002E94 RID: 11924 RVA: 0x000BB75C File Offset: 0x000B995C
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
							goto IL_C3;
						}
						using (List<LadderQueueManager>.Enumerator enumerator = this._queueManagers.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								LadderQueueManager ladderQueueManager = enumerator.Current;
								this.CleanState.Scene.SetAbilityOfFacesWithId(ladderQueueManager.ManagedNavigationFaceId, true);
								ladderQueueManager.IsDeactivated = false;
							}
							goto IL_C3;
						}
					}
					if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() > 0)
					{
						this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
					}
					IL_C3:
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetSiegeTowerHasArrivedAtTarget(this));
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

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x06002E95 RID: 11925 RVA: 0x000BB870 File Offset: 0x000B9A70
		// (set) Token: 0x06002E96 RID: 11926 RVA: 0x000BB878 File Offset: 0x000B9A78
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
						GameNetwork.WriteMessage(new SetSiegeTowerGateState(this, value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					}
					this._state = value;
					this.OnSiegeTowerGateStateChange();
				}
			}
		}

		// Token: 0x06002E97 RID: 11927 RVA: 0x000BB8B0 File Offset: 0x000B9AB0
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			if (gameEntity == null || !gameEntity.HasScriptOfType<UsableMissionObject>() || gameEntity.HasTag("move"))
			{
				return new TextObject("{=aXjlMBiE}Siege Tower", null).ToString();
			}
			return new TextObject("{=6wZUG0ev}Gate", null).ToString();
		}

		// Token: 0x06002E98 RID: 11928 RVA: 0x000BB8FC File Offset: 0x000B9AFC
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = (usableGameObject.GameEntity.HasTag("move") ? new TextObject("{=rwZAZSvX}{KEY} Move", null) : new TextObject("{=5oozsaIb}{KEY} Open", null));
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		// Token: 0x06002E99 RID: 11929 RVA: 0x000BB950 File Offset: 0x000B9B50
		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteBoolToPacket(this.HasArrivedAtTarget);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeTowerGateStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this._fallAngularSpeed, CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo);
		}

		// Token: 0x06002E9A RID: 11930 RVA: 0x000BB984 File Offset: 0x000B9B84
		public override bool ReadFromNetwork()
		{
			bool flag = base.ReadFromNetwork();
			bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeTowerGateStateCompressionInfo, ref flag);
			float num2 = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo, ref flag);
			if (flag)
			{
				this.HasArrivedAtTarget = flag2;
				this._state = (SiegeTower.GateState)num;
				this._fallAngularSpeed = num2;
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
					base.GameEntity.RecomputeBoundingBox();
				}
			}
			return flag;
		}

		// Token: 0x06002E9B RID: 11931 RVA: 0x000BBA4F File Offset: 0x000B9C4F
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

		// Token: 0x06002E9C RID: 11932 RVA: 0x000BBA70 File Offset: 0x000B9C70
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

		// Token: 0x06002E9D RID: 11933 RVA: 0x000BBAD4 File Offset: 0x000B9CD4
		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 90f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		// Token: 0x06002E9E RID: 11934 RVA: 0x000BBAF8 File Offset: 0x000B9CF8
		public override void Disable()
		{
			base.Disable();
			this.SetAbilityOfFaces(false);
			if (this._queueManagers != null)
			{
				foreach (LadderQueueManager ladderQueueManager in this._queueManagers)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(ladderQueueManager.ManagedNavigationFaceId, false);
					ladderQueueManager.IsDeactivated = true;
				}
			}
		}

		// Token: 0x06002E9F RID: 11935 RVA: 0x000BBB78 File Offset: 0x000B9D78
		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.SiegeTower;
		}

		// Token: 0x06002EA0 RID: 11936 RVA: 0x000BBB7F File Offset: 0x000B9D7F
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new SiegeTowerAI(this);
		}

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x06002EA1 RID: 11937 RVA: 0x000BBB87 File Offset: 0x000B9D87
		public override bool IsDeactivated
		{
			get
			{
				return (this.MovementComponent.HasArrivedAtTarget && this.State == SiegeTower.GateState.Open) || base.IsDeactivated;
			}
		}

		// Token: 0x06002EA2 RID: 11938 RVA: 0x000BBBA8 File Offset: 0x000B9DA8
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

		// Token: 0x06002EA3 RID: 11939 RVA: 0x000BBE38 File Offset: 0x000BA038
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

		// Token: 0x06002EA4 RID: 11940 RVA: 0x000BBF06 File Offset: 0x000BA106
		protected override GameEntity GetEntityToAttachNavMeshFaces()
		{
			return this.CleanState;
		}

		// Token: 0x06002EA5 RID: 11941 RVA: 0x000BBF0E File Offset: 0x000BA10E
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

		// Token: 0x06002EA6 RID: 11942 RVA: 0x000BBF28 File Offset: 0x000BA128
		public override void SetAbilityOfFaces(bool enabled)
		{
			base.SetAbilityOfFaces(enabled);
			if (this._queueManagers != null)
			{
				foreach (LadderQueueManager ladderQueueManager in this._queueManagers)
				{
					this.CleanState.Scene.SetAbilityOfFacesWithId(ladderQueueManager.ManagedNavigationFaceId, enabled);
					ladderQueueManager.IsDeactivated = !enabled;
				}
			}
		}

		// Token: 0x06002EA7 RID: 11943 RVA: 0x000BBFA4 File Offset: 0x000BA1A4
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

		// Token: 0x06002EA8 RID: 11944 RVA: 0x000BBFDC File Offset: 0x000BA1DC
		private bool IsNavmeshOnThisTowerAttackerDifficultNavmeshIDs(int testedNavmeshID)
		{
			return this._hasLadders && (testedNavmeshID == this.DynamicNavmeshIdStart + 1 || testedNavmeshID == this.DynamicNavmeshIdStart + 5 || testedNavmeshID == this.DynamicNavmeshIdStart + 6 || testedNavmeshID == this.DynamicNavmeshIdStart + 7 || testedNavmeshID == this.DynamicNavmeshIdStart + 3);
		}

		// Token: 0x06002EA9 RID: 11945 RVA: 0x000BC02C File Offset: 0x000BA22C
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

		// Token: 0x06002EAA RID: 11946 RVA: 0x000BC0E8 File Offset: 0x000BA2E8
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
							ladderQueueManager.IsDeactivated = true;
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
					ladderQueueManager4.IsDeactivated = true;
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

		// Token: 0x06002EAB RID: 11947 RVA: 0x000BC950 File Offset: 0x000BAB50
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002EAC RID: 11948 RVA: 0x000BC970 File Offset: 0x000BAB70
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
					base.GameEntity.RecomputeBoundingBox();
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
					base.GameEntity.RecomputeBoundingBox();
					float num = Vec3.DotProduct(frame2.rotation.u, this._openStateRotation.f);
					if (this._fallAngularSpeed > 0f && num < 0.05f)
					{
						frame2.rotation = this._openStateRotation;
						this._gateObject.GameEntity.SetFrame(ref frame2);
						base.GameEntity.RecomputeBoundingBox();
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
					Debug.FailedAssert("Invalid gate state.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SiegeTower.cs", "OnTick", 980);
					break;
				}
			}
		}

		// Token: 0x06002EAD RID: 11949 RVA: 0x000BCC9C File Offset: 0x000BAE9C
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

		// Token: 0x06002EAE RID: 11950 RVA: 0x000BCD1C File Offset: 0x000BAF1C
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
			base.GameEntity.RecomputeBoundingBox();
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

		// Token: 0x06002EAF RID: 11951 RVA: 0x000BCE7C File Offset: 0x000BB07C
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

		// Token: 0x06002EB0 RID: 11952 RVA: 0x000BCF34 File Offset: 0x000BB134
		public void HighlightPath()
		{
			this.MovementComponent.HighlightPath();
		}

		// Token: 0x06002EB1 RID: 11953 RVA: 0x000BCF44 File Offset: 0x000BB144
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

		// Token: 0x06002EB2 RID: 11954 RVA: 0x000BCFD5 File Offset: 0x000BB1D5
		public MatrixFrame GetInitialFrame()
		{
			SiegeWeaponMovementComponent movementComponent = this.MovementComponent;
			if (movementComponent == null)
			{
				return this.CleanState.GetGlobalFrame();
			}
			return movementComponent.GetInitialFrame();
		}

		// Token: 0x06002EB3 RID: 11955 RVA: 0x000BCFF4 File Offset: 0x000BB1F4
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
					base.GameEntity.RecomputeBoundingBox();
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

		// Token: 0x06002EB4 RID: 11956 RVA: 0x000BD258 File Offset: 0x000BB458
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

		// Token: 0x06002EB5 RID: 11957 RVA: 0x000BD2DC File Offset: 0x000BB4DC
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

		// Token: 0x06002EB6 RID: 11958 RVA: 0x000BD340 File Offset: 0x000BB540
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

		// Token: 0x06002EB7 RID: 11959 RVA: 0x000BD39E File Offset: 0x000BB59E
		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		// Token: 0x06002EB8 RID: 11960 RVA: 0x000BD3A8 File Offset: 0x000BB5A8
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

		// Token: 0x06002EB9 RID: 11961 RVA: 0x000BD41C File Offset: 0x000BB61C
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

		// Token: 0x040012E3 RID: 4835
		private const int LeftLadderNavMeshIdLocal = 5;

		// Token: 0x040012E4 RID: 4836
		private const int MiddleLadderNavMeshIdLocal = 6;

		// Token: 0x040012E5 RID: 4837
		private const int RightLadderNavMeshIdLocal = 7;

		// Token: 0x040012E6 RID: 4838
		private const string BreakableWallTag = "breakable_wall";

		// Token: 0x040012E7 RID: 4839
		private const string DestroyedWallTag = "destroyed";

		// Token: 0x040012E8 RID: 4840
		private const string NonDestroyedWallTag = "non_destroyed";

		// Token: 0x040012E9 RID: 4841
		private const string LadderTag = "ladder";

		// Token: 0x040012EA RID: 4842
		private const string BattlementDestroyedParticleTag = "particle_spawnpoint";

		// Token: 0x040012EB RID: 4843
		public string GateTag = "gate";

		// Token: 0x040012EC RID: 4844
		public string GateOpenTag = "gateOpen";

		// Token: 0x040012ED RID: 4845
		public string HandleTag = "handle";

		// Token: 0x040012EE RID: 4846
		public string GateHandleIdleAnimation = "siegetower_handle_idle";

		// Token: 0x040012EF RID: 4847
		private int _gateHandleIdleAnimationIndex = -1;

		// Token: 0x040012F0 RID: 4848
		public string GateTrembleAnimation = "siegetower_door_stop";

		// Token: 0x040012F1 RID: 4849
		private int _gateTrembleAnimationIndex = -1;

		// Token: 0x040012F2 RID: 4850
		public string BattlementDestroyedParticle = "psys_adobe_battlement_destroyed";

		// Token: 0x040012F3 RID: 4851
		private string _targetWallSegmentTag;

		// Token: 0x040012F4 RID: 4852
		public bool GhostEntityMove = true;

		// Token: 0x040012F5 RID: 4853
		public float GhostEntitySpeedMultiplier = 1f;

		// Token: 0x040012F6 RID: 4854
		private string _sideTag;

		// Token: 0x040012F7 RID: 4855
		private bool _hasLadders;

		// Token: 0x040012F8 RID: 4856
		public float WheelDiameter = 1.3f;

		// Token: 0x040012F9 RID: 4857
		public float MinSpeed = 0.5f;

		// Token: 0x040012FA RID: 4858
		public float MaxSpeed = 1f;

		// Token: 0x040012FB RID: 4859
		public int GateNavMeshId;

		// Token: 0x040012FC RID: 4860
		public int NavMeshIdToDisableOnDestination = -1;

		// Token: 0x040012FD RID: 4861
		private int _soilNavMeshID1;

		// Token: 0x040012FE RID: 4862
		private int _soilNavMeshID2;

		// Token: 0x040012FF RID: 4863
		private int _ditchNavMeshID1;

		// Token: 0x04001300 RID: 4864
		private int _ditchNavMeshID2;

		// Token: 0x04001301 RID: 4865
		private int _groundToSoilNavMeshID1;

		// Token: 0x04001302 RID: 4866
		private int _groundToSoilNavMeshID2;

		// Token: 0x04001303 RID: 4867
		private int _soilGenericNavMeshID;

		// Token: 0x04001304 RID: 4868
		private int _groundGenericNavMeshID;

		// Token: 0x04001305 RID: 4869
		public string BarrierTagToRemove = "barrier";

		// Token: 0x04001306 RID: 4870
		private List<GameEntity> _aiBarriers;

		// Token: 0x04001307 RID: 4871
		private bool _isGhostMovementOn;

		// Token: 0x04001308 RID: 4872
		private bool _hasArrivedAtTarget;

		// Token: 0x04001309 RID: 4873
		private SiegeTower.GateState _state;

		// Token: 0x0400130A RID: 4874
		private SynchedMissionObject _gateObject;

		// Token: 0x0400130B RID: 4875
		private SynchedMissionObject _handleObject;

		// Token: 0x0400130C RID: 4876
		private SoundEvent _gateOpenSound;

		// Token: 0x0400130D RID: 4877
		private int _gateOpenSoundIndex = -1;

		// Token: 0x0400130E RID: 4878
		private Mat3 _openStateRotation;

		// Token: 0x0400130F RID: 4879
		private Mat3 _closedStateRotation;

		// Token: 0x04001310 RID: 4880
		private float _fallAngularSpeed;

		// Token: 0x04001311 RID: 4881
		private GameEntity _cleanState;

		// Token: 0x04001312 RID: 4882
		private GameEntity _destroyedWallEntity;

		// Token: 0x04001313 RID: 4883
		private GameEntity _nonDestroyedWallEntity;

		// Token: 0x04001314 RID: 4884
		private GameEntity _battlementDestroyedParticle;

		// Token: 0x04001315 RID: 4885
		private StandingPoint _gateStandingPoint;

		// Token: 0x04001316 RID: 4886
		private MatrixFrame _gateStandingPointLocalIKFrame;

		// Token: 0x04001317 RID: 4887
		private SynchedMissionObject _ditchFillDebris;

		// Token: 0x04001318 RID: 4888
		private List<LadderQueueManager> _queueManagers;

		// Token: 0x04001319 RID: 4889
		private WallSegment _targetWallSegment;

		// Token: 0x0400131A RID: 4890
		private List<SiegeLadder> _sameSideSiegeLadders;

		// Token: 0x0200066D RID: 1645
		public enum GateState
		{
			// Token: 0x040020E0 RID: 8416
			Closed,
			// Token: 0x040020E1 RID: 8417
			Open,
			// Token: 0x040020E2 RID: 8418
			GateFalling,
			// Token: 0x040020E3 RID: 8419
			GateFallingWallDestroyed,
			// Token: 0x040020E4 RID: 8420
			NumberOfStates
		}
	}
}
