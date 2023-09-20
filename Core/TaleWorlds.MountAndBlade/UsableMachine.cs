using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000382 RID: 898
	public abstract class UsableMachine : SynchedMissionObject, IFocusable, IOrderable, IDetachment
	{
		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x060030B7 RID: 12471 RVA: 0x000CA17C File Offset: 0x000C837C
		// (set) Token: 0x060030B8 RID: 12472 RVA: 0x000CA184 File Offset: 0x000C8384
		public MBList<StandingPoint> StandingPoints { get; private set; }

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x060030B9 RID: 12473 RVA: 0x000CA18D File Offset: 0x000C838D
		// (set) Token: 0x060030BA RID: 12474 RVA: 0x000CA195 File Offset: 0x000C8395
		public StandingPoint PilotStandingPoint { get; private set; }

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x060030BB RID: 12475 RVA: 0x000CA19E File Offset: 0x000C839E
		// (set) Token: 0x060030BC RID: 12476 RVA: 0x000CA1A6 File Offset: 0x000C83A6
		protected internal List<StandingPoint> AmmoPickUpPoints { get; private set; }

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x060030BD RID: 12477 RVA: 0x000CA1AF File Offset: 0x000C83AF
		// (set) Token: 0x060030BE RID: 12478 RVA: 0x000CA1B7 File Offset: 0x000C83B7
		private protected List<GameEntity> WaitStandingPoints { protected get; private set; }

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x060030BF RID: 12479 RVA: 0x000CA1C0 File Offset: 0x000C83C0
		public DestructableComponent DestructionComponent
		{
			get
			{
				return this._destructionComponent;
			}
		}

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x060030C0 RID: 12480 RVA: 0x000CA1C8 File Offset: 0x000C83C8
		public bool IsDestructible
		{
			get
			{
				return this.DestructionComponent != null;
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x060030C1 RID: 12481 RVA: 0x000CA1D3 File Offset: 0x000C83D3
		public bool IsDestroyed
		{
			get
			{
				return this.DestructionComponent != null && this.DestructionComponent.IsDestroyed;
			}
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x060030C2 RID: 12482 RVA: 0x000CA1EA File Offset: 0x000C83EA
		public Agent PilotAgent
		{
			get
			{
				StandingPoint pilotStandingPoint = this.PilotStandingPoint;
				if (pilotStandingPoint == null)
				{
					return null;
				}
				return pilotStandingPoint.UserAgent;
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x060030C3 RID: 12483 RVA: 0x000CA1FD File Offset: 0x000C83FD
		public bool IsLoose
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060030C4 RID: 12484 RVA: 0x000CA200 File Offset: 0x000C8400
		public UsableMachineAIBase Ai
		{
			get
			{
				if (this._ai == null)
				{
					this._ai = this.CreateAIBehaviorObject();
				}
				return this._ai;
			}
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060030C5 RID: 12485 RVA: 0x000CA21C File Offset: 0x000C841C
		public virtual FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.Item;
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060030C6 RID: 12486 RVA: 0x000CA21F File Offset: 0x000C841F
		// (set) Token: 0x060030C7 RID: 12487 RVA: 0x000CA227 File Offset: 0x000C8427
		public StandingPoint CurrentlyUsedAmmoPickUpPoint
		{
			get
			{
				return this._currentlyUsedAmmoPickUpPoint;
			}
			set
			{
				this._currentlyUsedAmmoPickUpPoint = value;
				base.SetScriptComponentToTick(this.GetTickRequirement());
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x060030C8 RID: 12488 RVA: 0x000CA23C File Offset: 0x000C843C
		public bool HasAIPickingUpAmmo
		{
			get
			{
				return this.CurrentlyUsedAmmoPickUpPoint != null;
			}
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x060030C9 RID: 12489 RVA: 0x000CA247 File Offset: 0x000C8447
		public MBReadOnlyList<Formation> UserFormations
		{
			get
			{
				return this._userFormations;
			}
		}

		// Token: 0x060030CA RID: 12490 RVA: 0x000CA250 File Offset: 0x000C8450
		protected UsableMachine()
		{
			this._components = new List<UsableMissionObjectComponent>();
		}

		// Token: 0x060030CB RID: 12491 RVA: 0x000CA2A8 File Offset: 0x000C84A8
		public void AddComponent(UsableMissionObjectComponent component)
		{
			this._components.Add(component);
			component.OnAdded(base.Scene);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x060030CC RID: 12492 RVA: 0x000CA2CE File Offset: 0x000C84CE
		public void RemoveComponent(UsableMissionObjectComponent component)
		{
			component.OnRemoved();
			this._components.Remove(component);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x060030CD RID: 12493 RVA: 0x000CA2EF File Offset: 0x000C84EF
		public T GetComponent<T>() where T : UsableMissionObjectComponent
		{
			return this._components.Find((UsableMissionObjectComponent c) => c is T) as T;
		}

		// Token: 0x060030CE RID: 12494 RVA: 0x000CA325 File Offset: 0x000C8525
		public virtual OrderType GetOrder(BattleSideEnum side)
		{
			return OrderType.Use;
		}

		// Token: 0x060030CF RID: 12495 RVA: 0x000CA329 File Offset: 0x000C8529
		public virtual UsableMachineAIBase CreateAIBehaviorObject()
		{
			return null;
		}

		// Token: 0x060030D0 RID: 12496 RVA: 0x000CA32C File Offset: 0x000C852C
		public GameEntity GetValidStandingPointForAgent(Agent agent)
		{
			float num = float.MaxValue;
			StandingPoint standingPoint = null;
			foreach (StandingPoint standingPoint2 in this.StandingPoints)
			{
				if (!standingPoint2.IsDisabledForAgent(agent) && (!standingPoint2.HasUser || standingPoint2.HasAIUser))
				{
					WorldFrame worldFrame = standingPoint2.GetUserFrameForAgent(agent);
					float num2 = worldFrame.Origin.AsVec2.DistanceSquared(agent.Position.AsVec2);
					if (agent.CanReachAndUseObject(standingPoint2, num2) && num2 < num)
					{
						worldFrame = standingPoint2.GetUserFrameForAgent(agent);
						if (MathF.Abs(worldFrame.Origin.GetGroundVec3().z - agent.Position.z) < 1.5f)
						{
							num = num2;
							standingPoint = standingPoint2;
						}
					}
				}
			}
			if (standingPoint == null)
			{
				return null;
			}
			return standingPoint.GameEntity;
		}

		// Token: 0x060030D1 RID: 12497 RVA: 0x000CA424 File Offset: 0x000C8624
		public GameEntity GetValidStandingPointForAgentWithoutDistanceCheck(Agent agent)
		{
			float num = float.MaxValue;
			StandingPoint standingPoint = null;
			foreach (StandingPoint standingPoint2 in this.StandingPoints)
			{
				if (!standingPoint2.IsDisabledForAgent(agent) && (!standingPoint2.HasUser || standingPoint2.HasAIUser))
				{
					WorldFrame worldFrame = standingPoint2.GetUserFrameForAgent(agent);
					float num2 = worldFrame.Origin.AsVec2.DistanceSquared(agent.Position.AsVec2);
					if (num2 < num)
					{
						worldFrame = standingPoint2.GetUserFrameForAgent(agent);
						if (MathF.Abs(worldFrame.Origin.GetGroundVec3().z - agent.Position.z) < 1.5f)
						{
							num = num2;
							standingPoint = standingPoint2;
						}
					}
				}
			}
			if (standingPoint == null)
			{
				return null;
			}
			return standingPoint.GameEntity;
		}

		// Token: 0x060030D2 RID: 12498 RVA: 0x000CA510 File Offset: 0x000C8710
		public StandingPoint GetVacantStandingPointForAI(Agent agent)
		{
			if (this.PilotStandingPoint != null && !this.PilotStandingPoint.IsDisabledForAgent(agent) && !this.AmmoPickUpPoints.Contains(this.PilotStandingPoint))
			{
				return this.PilotStandingPoint;
			}
			float num = 100000000f;
			StandingPoint standingPoint = null;
			foreach (StandingPoint standingPoint2 in this.StandingPoints)
			{
				bool flag = true;
				if (this.AmmoPickUpPoints.Contains(standingPoint2))
				{
					foreach (StandingPoint standingPoint3 in this.StandingPoints)
					{
						if (standingPoint3 is StandingPointWithWeaponRequirement && !this.AmmoPickUpPoints.Contains(standingPoint3) && (standingPoint3.IsDeactivated || standingPoint3.HasUser || standingPoint3.HasAIMovingTo))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag && !standingPoint2.IsDisabledForAgent(agent))
				{
					float num2 = (agent.Position - standingPoint2.GetUserFrameForAgent(agent).Origin.GetGroundVec3()).LengthSquared;
					if (!standingPoint2.IsDisabledForPlayers)
					{
						num2 -= 100000f;
					}
					if (num2 < num)
					{
						num = num2;
						standingPoint = standingPoint2;
					}
				}
			}
			return standingPoint;
		}

		// Token: 0x060030D3 RID: 12499 RVA: 0x000CA678 File Offset: 0x000C8878
		public StandingPoint GetTargetStandingPointOfAIAgent(Agent agent)
		{
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				if (standingPoint.IsAIMovingTo(agent))
				{
					return standingPoint;
				}
			}
			return null;
		}

		// Token: 0x060030D4 RID: 12500 RVA: 0x000CA6D4 File Offset: 0x000C88D4
		public override void SetVisibleSynched(bool value, bool forceChildrenVisible = false)
		{
			base.SetVisibleSynched(value, forceChildrenVisible);
		}

		// Token: 0x060030D5 RID: 12501 RVA: 0x000CA6E0 File Offset: 0x000C88E0
		public override void SetPhysicsStateSynched(bool value, bool setChildren = true)
		{
			base.SetPhysicsStateSynched(value, setChildren);
			this.SetAbilityOfFaces(value);
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				standingPoint.OnParentMachinePhysicsStateChanged();
			}
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x060030D6 RID: 12502 RVA: 0x000CA740 File Offset: 0x000C8940
		public int UserCountNotInStruckAction
		{
			get
			{
				int num = 0;
				foreach (StandingPoint standingPoint in this.StandingPoints)
				{
					if (standingPoint.HasUser && !standingPoint.UserAgent.IsInBeingStruckAction)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x060030D7 RID: 12503 RVA: 0x000CA7A8 File Offset: 0x000C89A8
		public int UserCountIncludingInStruckAction
		{
			get
			{
				int num = 0;
				using (List<StandingPoint>.Enumerator enumerator = this.StandingPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.HasUser)
						{
							num++;
						}
					}
				}
				return num;
			}
		}

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x060030D8 RID: 12504 RVA: 0x000CA804 File Offset: 0x000C8A04
		public virtual int MaxUserCount
		{
			get
			{
				return this.StandingPoints.Count;
			}
		}

		// Token: 0x060030D9 RID: 12505 RVA: 0x000CA811 File Offset: 0x000C8A11
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.CollectAndSetStandingPoints();
		}

		// Token: 0x060030DA RID: 12506 RVA: 0x000CA820 File Offset: 0x000C8A20
		protected internal override void OnInit()
		{
			base.OnInit();
			this._isDisabledForAttackerAIDueToEnemyInRange = new QueryData<bool>(delegate
			{
				Vec3 globalScale = base.GameEntity.GetGlobalScale();
				Vec3 globalPosition = base.GameEntity.GlobalPosition;
				globalPosition.x += this.MachinePositionOffsetToStopUsing.x * globalScale.x;
				globalPosition.y += this.MachinePositionOffsetToStopUsing.y * globalScale.y;
				Agent closestEnemyAgent = Mission.Current.GetClosestEnemyAgent(Mission.Current.Teams.Attacker, globalPosition, this.EnemyRangeToStopUsing);
				return closestEnemyAgent != null && closestEnemyAgent.Position.z > globalPosition.z - 2f && closestEnemyAgent.Position.z < globalPosition.z + 4f;
			}, 1f);
			this._isDisabledForDefenderAIDueToEnemyInRange = new QueryData<bool>(delegate
			{
				Vec3 globalScale2 = base.GameEntity.GetGlobalScale();
				Vec3 globalPosition2 = base.GameEntity.GlobalPosition;
				globalPosition2.x += this.MachinePositionOffsetToStopUsing.x * globalScale2.x;
				globalPosition2.y += this.MachinePositionOffsetToStopUsing.y * globalScale2.y;
				Agent closestEnemyAgent2 = Mission.Current.GetClosestEnemyAgent(Mission.Current.Teams.Defender, globalPosition2, this.EnemyRangeToStopUsing);
				return closestEnemyAgent2 != null && closestEnemyAgent2.Position.z > globalPosition2.z - 2f && closestEnemyAgent2.Position.z < globalPosition2.z + 4f;
			}, 1f);
			this.CollectAndSetStandingPoints();
			this.AmmoPickUpPoints = new List<StandingPoint>();
			this._destructionComponent = base.GameEntity.GetFirstScriptOfType<DestructableComponent>();
			this.PilotStandingPoint = null;
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				if (standingPoint.GameEntity.HasTag(this.PilotStandingPointTag))
				{
					this.PilotStandingPoint = standingPoint;
				}
				if (standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
				{
					this.AmmoPickUpPoints.Add(standingPoint);
				}
				standingPoint.InitializeDefendingAgents();
			}
			this.WaitStandingPoints = base.GameEntity.CollectChildrenEntitiesWithTag(this.WaitStandingPointTag);
			if (this.WaitStandingPoints.Count > 0)
			{
				this.ActiveWaitStandingPoint = this.WaitStandingPoints[0];
			}
			this._userFormations = new MBList<Formation>();
			this._usableStandingPoints = new List<ValueTuple<int, StandingPoint>>();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x000CA96C File Offset: 0x000C8B6C
		private void CollectAndSetStandingPoints()
		{
			GameEntity parent = base.GameEntity.Parent;
			if (parent != null && parent.HasTag("machine_parent"))
			{
				this.StandingPoints = base.GameEntity.Parent.CollectObjects<StandingPoint>();
				return;
			}
			this.StandingPoints = base.GameEntity.CollectObjects<StandingPoint>();
		}

		// Token: 0x060030DC RID: 12508 RVA: 0x000CA9C0 File Offset: 0x000C8BC0
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			bool flag = false;
			using (List<UsableMissionObjectComponent>.Enumerator enumerator = this._components.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsOnTickRequired())
					{
						flag = true;
						break;
					}
				}
			}
			if (base.GameEntity.IsVisibleIncludeParents() && (flag || (!GameNetwork.IsClientOrReplay && this.HasAIPickingUpAmmo)))
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x060030DD RID: 12509 RVA: 0x000CAA48 File Offset: 0x000C8C48
		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this.MakeVisibilityCheck && !base.GameEntity.IsVisibleIncludeParents())
			{
				return;
			}
			if (!GameNetwork.IsClientOrReplay && this.HasAIPickingUpAmmo && !this.CurrentlyUsedAmmoPickUpPoint.HasAIMovingTo && !this.CurrentlyUsedAmmoPickUpPoint.HasAIUser)
			{
				this.CurrentlyUsedAmmoPickUpPoint = null;
			}
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnTick(dt);
			}
			bool isClientOrReplay = GameNetwork.IsClientOrReplay;
		}

		// Token: 0x060030DE RID: 12510 RVA: 0x000CAAEC File Offset: 0x000C8CEC
		private static string DebugGetMemberNameOf<T>(object instance, T sp) where T : class
		{
			Type type = instance.GetType();
			foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!(propertyInfo.GetMethod == null))
				{
					if (propertyInfo.GetValue(instance) == sp)
					{
						return propertyInfo.Name;
					}
					IReadOnlyList<StandingPoint> readOnlyList;
					if (propertyInfo.GetType().IsGenericType && (propertyInfo.GetType().GetGenericTypeDefinition() == typeof(List<>) || propertyInfo.GetType().GetGenericTypeDefinition() == typeof(MBList<>) || propertyInfo.GetType().GetGenericTypeDefinition() == typeof(MBReadOnlyList<>)) && (readOnlyList = propertyInfo.GetValue(instance) as IReadOnlyList<StandingPoint>) != null)
					{
						for (int j = 0; j < readOnlyList.Count; j++)
						{
							StandingPoint standingPoint = readOnlyList[j];
							if (sp == standingPoint)
							{
								return string.Concat(new object[] { propertyInfo.Name, "[", j, "]" });
							}
						}
					}
				}
			}
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (fieldInfo.GetValue(instance) == sp)
				{
					return fieldInfo.Name;
				}
				IReadOnlyList<StandingPoint> readOnlyList2;
				if (fieldInfo.FieldType.IsGenericType && (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>) || fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(MBList<>) || fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(MBReadOnlyList<>)) && (readOnlyList2 = fieldInfo.GetValue(instance) as IReadOnlyList<StandingPoint>) != null)
				{
					for (int k = 0; k < readOnlyList2.Count; k++)
					{
						StandingPoint standingPoint2 = readOnlyList2[k];
						if (sp == standingPoint2)
						{
							return string.Concat(new object[] { fieldInfo.Name, "[", k, "]" });
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060030DF RID: 12511 RVA: 0x000CAD2C File Offset: 0x000C8F2C
		[Conditional("_RGL_KEEP_ASSERTS")]
		protected virtual void DebugTick(float dt)
		{
			if (MBDebug.IsDisplayingHighLevelAI)
			{
				foreach (StandingPoint standingPoint in this.StandingPoints)
				{
					Vec3 globalPosition = standingPoint.GameEntity.GlobalPosition;
					Vec3.One / 3f;
					bool isDeactivated = standingPoint.IsDeactivated;
				}
			}
		}

		// Token: 0x060030E0 RID: 12512 RVA: 0x000CADA0 File Offset: 0x000C8FA0
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnEditorTick(dt);
			}
		}

		// Token: 0x060030E1 RID: 12513 RVA: 0x000CADF8 File Offset: 0x000C8FF8
		protected internal override void OnEditorValidate()
		{
			base.OnEditorValidate();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnEditorValidate();
			}
		}

		// Token: 0x060030E2 RID: 12514 RVA: 0x000CAE50 File Offset: 0x000C9050
		public virtual void OnFocusGain(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusGain(userAgent);
			}
		}

		// Token: 0x060030E3 RID: 12515 RVA: 0x000CAEA4 File Offset: 0x000C90A4
		public virtual void OnFocusLose(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusLose(userAgent);
			}
		}

		// Token: 0x060030E4 RID: 12516 RVA: 0x000CAEF8 File Offset: 0x000C90F8
		public virtual TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			return TextObject.Empty;
		}

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x060030E5 RID: 12517 RVA: 0x000CAEFF File Offset: 0x000C90FF
		public virtual bool HasWaitFrame
		{
			get
			{
				return this.ActiveWaitStandingPoint != null;
			}
		}

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x060030E6 RID: 12518 RVA: 0x000CAF10 File Offset: 0x000C9110
		public MatrixFrame WaitFrame
		{
			get
			{
				if (this.ActiveWaitStandingPoint != null)
				{
					return this.ActiveWaitStandingPoint.GetGlobalFrame();
				}
				return default(MatrixFrame);
			}
		}

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x060030E7 RID: 12519 RVA: 0x000CAF40 File Offset: 0x000C9140
		public GameEntity WaitEntity
		{
			get
			{
				return this.ActiveWaitStandingPoint;
			}
		}

		// Token: 0x060030E8 RID: 12520 RVA: 0x000CAF48 File Offset: 0x000C9148
		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnMissionReset();
			}
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x060030E9 RID: 12521 RVA: 0x000CAFA0 File Offset: 0x000C91A0
		public virtual bool IsDeactivated
		{
			get
			{
				return this._isMachineDeactivated || this.IsDestroyed;
			}
		}

		// Token: 0x060030EA RID: 12522 RVA: 0x000CAFB4 File Offset: 0x000C91B4
		public void Deactivate()
		{
			this._isMachineDeactivated = true;
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				standingPoint.IsDeactivated = true;
			}
		}

		// Token: 0x060030EB RID: 12523 RVA: 0x000CB00C File Offset: 0x000C920C
		public void Activate()
		{
			this._isMachineDeactivated = false;
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				standingPoint.IsDeactivated = false;
			}
		}

		// Token: 0x060030EC RID: 12524 RVA: 0x000CB064 File Offset: 0x000C9264
		public virtual bool IsDisabledForBattleSide(BattleSideEnum sideEnum)
		{
			return this.IsDeactivated;
		}

		// Token: 0x060030ED RID: 12525 RVA: 0x000CB06C File Offset: 0x000C926C
		public virtual bool IsDisabledForBattleSideAI(BattleSideEnum sideEnum)
		{
			return this._isDisabledForAI || (this.EnemyRangeToStopUsing > 0f && sideEnum != BattleSideEnum.None && this.IsDisabledDueToEnemyInRange(sideEnum));
		}

		// Token: 0x060030EE RID: 12526 RVA: 0x000CB094 File Offset: 0x000C9294
		public virtual bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return true;
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x000CB097 File Offset: 0x000C9297
		protected bool IsDisabledDueToEnemyInRange(BattleSideEnum sideEnum)
		{
			if (sideEnum == BattleSideEnum.Attacker)
			{
				return this._isDisabledForAttackerAIDueToEnemyInRange.Value;
			}
			return this._isDisabledForDefenderAIDueToEnemyInRange.Value;
		}

		// Token: 0x060030F0 RID: 12528 RVA: 0x000CB0B4 File Offset: 0x000C92B4
		public virtual bool AutoAttachUserToFormation(BattleSideEnum sideEnum)
		{
			return true;
		}

		// Token: 0x060030F1 RID: 12529 RVA: 0x000CB0B7 File Offset: 0x000C92B7
		public virtual bool HasToBeDefendedByUser(BattleSideEnum sideEnum)
		{
			return false;
		}

		// Token: 0x060030F2 RID: 12530 RVA: 0x000CB0BC File Offset: 0x000C92BC
		public virtual void Disable()
		{
			foreach (Team team in Mission.Current.Teams.Where((Team t) => t.DetachmentManager.ContainsDetachment(this)))
			{
				team.DetachmentManager.DestroyDetachment(this);
			}
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				if (!standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
				{
					if (standingPoint.HasUser)
					{
						standingPoint.UserAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
					standingPoint.SetIsDeactivatedSynched(true);
				}
			}
		}

		// Token: 0x060030F3 RID: 12531 RVA: 0x000CB18C File Offset: 0x000C938C
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnRemoved();
			}
		}

		// Token: 0x060030F4 RID: 12532 RVA: 0x000CB1E4 File Offset: 0x000C93E4
		public override bool ReadFromNetwork()
		{
			bool flag = true;
			flag = flag && base.ReadFromNetwork();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				flag = flag && usableMissionObjectComponent.ReadFromNetwork();
			}
			return flag;
		}

		// Token: 0x060030F5 RID: 12533 RVA: 0x000CB250 File Offset: 0x000C9450
		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.WriteToNetwork();
			}
		}

		// Token: 0x060030F6 RID: 12534 RVA: 0x000CB2A8 File Offset: 0x000C94A8
		public override string ToString()
		{
			string text = base.GetType() + " with Components:";
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				text = string.Concat(new object[] { text, "[", usableMissionObjectComponent, "]" });
			}
			return text;
		}

		// Token: 0x060030F7 RID: 12535
		public abstract TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject);

		// Token: 0x060030F8 RID: 12536 RVA: 0x000CB32C File Offset: 0x000C952C
		public virtual StandingPoint GetBestPointAlternativeTo(StandingPoint standingPoint, Agent agent)
		{
			return standingPoint;
		}

		// Token: 0x060030F9 RID: 12537 RVA: 0x000CB330 File Offset: 0x000C9530
		public virtual bool IsInRangeToCheckAlternativePoints(Agent agent)
		{
			float num = ((this.StandingPoints.Count > 0) ? (agent.GetInteractionDistanceToUsable(this.StandingPoints[0]) + 1f) : 2f);
			return base.GameEntity.GlobalPosition.DistanceSquared(agent.Position) < num * num;
		}

		// Token: 0x060030FA RID: 12538 RVA: 0x000CB38C File Offset: 0x000C958C
		void IDetachment.OnFormationLeave(Formation formation)
		{
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				Agent userAgent = standingPoint.UserAgent;
				if (userAgent != null && userAgent.Formation == formation && !userAgent.IsPlayerControlled)
				{
					this.OnFormationLeaveHelper(formation, userAgent);
				}
				Agent movingAgent = standingPoint.MovingAgent;
				if (movingAgent != null && movingAgent.Formation == formation)
				{
					this.OnFormationLeaveHelper(formation, movingAgent);
				}
				for (int i = standingPoint.GetDefendingAgentCount() - 1; i >= 0; i--)
				{
					Agent agent = standingPoint.DefendingAgents[i];
					if (agent.Formation == formation)
					{
						this.OnFormationLeaveHelper(formation, agent);
					}
				}
			}
		}

		// Token: 0x060030FB RID: 12539 RVA: 0x000CB454 File Offset: 0x000C9654
		private void OnFormationLeaveHelper(Formation formation, Agent agent)
		{
			((IDetachment)this).RemoveAgent(agent);
			formation.AttachUnit(agent);
		}

		// Token: 0x060030FC RID: 12540 RVA: 0x000CB464 File Offset: 0x000C9664
		bool IDetachment.IsAgentUsingOrInterested(Agent agent)
		{
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				if (agent.CurrentlyUsedGameObject == standingPoint || (agent.IsAIControlled && agent.AIInterestedInGameObject(standingPoint)))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060030FD RID: 12541 RVA: 0x000CB4D4 File Offset: 0x000C96D4
		protected virtual float GetWeightOfStandingPoint(StandingPoint sp)
		{
			if (!sp.HasAIMovingTo)
			{
				return 0.6f;
			}
			return 0.2f;
		}

		// Token: 0x060030FE RID: 12542 RVA: 0x000CB4E9 File Offset: 0x000C96E9
		float IDetachment.GetDetachmentWeight(BattleSideEnum side)
		{
			return this.GetDetachmentWeightAux(side);
		}

		// Token: 0x060030FF RID: 12543 RVA: 0x000CB4F4 File Offset: 0x000C96F4
		protected virtual float GetDetachmentWeightAux(BattleSideEnum side)
		{
			if (this.IsDisabledForBattleSideAI(side))
			{
				return float.MinValue;
			}
			this._usableStandingPoints.Clear();
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < this.StandingPoints.Count; i++)
			{
				StandingPoint standingPoint = this.StandingPoints[i];
				if (standingPoint.IsUsableBySide(side))
				{
					if (!standingPoint.HasAIMovingTo)
					{
						if (!flag2)
						{
							this._usableStandingPoints.Clear();
						}
						flag2 = true;
					}
					else if (flag2 || standingPoint.MovingAgent.Formation.Team.Side != side)
					{
						goto IL_81;
					}
					flag = true;
					this._usableStandingPoints.Add(new ValueTuple<int, StandingPoint>(i, standingPoint));
				}
				IL_81:;
			}
			this._areUsableStandingPointsVacant = flag2;
			if (!flag)
			{
				return float.MinValue;
			}
			if (flag2)
			{
				return 1f;
			}
			if (!this._isDetachmentRecentlyEvaluated)
			{
				return 0.1f;
			}
			return 0.01f;
		}

		// Token: 0x06003100 RID: 12544 RVA: 0x000CB5C0 File Offset: 0x000C97C0
		void IDetachment.GetSlotIndexWeightTuples(List<ValueTuple<int, float>> slotIndexWeightTuples)
		{
			foreach (ValueTuple<int, StandingPoint> valueTuple in this._usableStandingPoints)
			{
				StandingPoint item = valueTuple.Item2;
				slotIndexWeightTuples.Add(new ValueTuple<int, float>(valueTuple.Item1, this.GetWeightOfStandingPoint(item) * ((!this._areUsableStandingPointsVacant && item.HasRecentlyBeenRechecked) ? 0.1f : 1f)));
			}
		}

		// Token: 0x06003101 RID: 12545 RVA: 0x000CB648 File Offset: 0x000C9848
		bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent)
		{
			return agent.CanBeAssignedForScriptedMovement() && !this.StandingPoints[slotIndex].IsDisabledForAgent(agent) && !this.IsAgentOnInconvenientNavmesh(agent, this.StandingPoints[slotIndex]);
		}

		// Token: 0x06003102 RID: 12546 RVA: 0x000CB680 File Offset: 0x000C9880
		protected virtual bool IsAgentOnInconvenientNavmesh(Agent agent, StandingPoint standingPoint)
		{
			if (Mission.Current.MissionTeamAIType != Mission.MissionTeamAITypeEnum.Siege)
			{
				return false;
			}
			int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
			TeamAISiegeComponent teamAISiegeComponent;
			if ((teamAISiegeComponent = agent.Team.TeamAI as TeamAISiegeComponent) != null)
			{
				if (teamAISiegeComponent is TeamAISiegeAttacker && currentNavigationFaceId % 10 == 1)
				{
					return true;
				}
				if (teamAISiegeComponent is TeamAISiegeDefender && currentNavigationFaceId % 10 != 1)
				{
					return true;
				}
				foreach (int num in teamAISiegeComponent.DifficultNavmeshIDs)
				{
					if (currentNavigationFaceId == num)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06003103 RID: 12547 RVA: 0x000CB728 File Offset: 0x000C9928
		bool IDetachment.IsAgentEligible(Agent agent)
		{
			return true;
		}

		// Token: 0x06003104 RID: 12548 RVA: 0x000CB72C File Offset: 0x000C992C
		public void AddAgentAtSlotIndex(Agent agent, int slotIndex)
		{
			StandingPoint standingPoint = this.StandingPoints[slotIndex];
			if (standingPoint.HasAIMovingTo)
			{
				Agent movingAgent = standingPoint.MovingAgent;
				if (movingAgent != null)
				{
					((IDetachment)this).RemoveAgent(movingAgent);
					Formation formation = movingAgent.Formation;
					if (formation != null)
					{
						formation.AttachUnit(movingAgent);
					}
				}
			}
			((IDetachment)this).AddAgent(agent, slotIndex);
			Formation formation2 = agent.Formation;
			if (formation2 != null)
			{
				formation2.DetachUnit(agent, false);
			}
			agent.Detachment = this;
			agent.DetachmentWeight = 1f;
		}

		// Token: 0x06003105 RID: 12549 RVA: 0x000CB79E File Offset: 0x000C999E
		Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex)
		{
			return this.StandingPoints[slotIndex].MovingAgent;
		}

		// Token: 0x06003106 RID: 12550 RVA: 0x000CB7B1 File Offset: 0x000C99B1
		bool IDetachment.IsDetachmentRecentlyEvaluated()
		{
			return this._isDetachmentRecentlyEvaluated;
		}

		// Token: 0x06003107 RID: 12551 RVA: 0x000CB7B9 File Offset: 0x000C99B9
		void IDetachment.UnmarkDetachment()
		{
			this._isDetachmentRecentlyEvaluated = false;
		}

		// Token: 0x06003108 RID: 12552 RVA: 0x000CB7C4 File Offset: 0x000C99C4
		void IDetachment.MarkSlotAtIndex(int slotIndex)
		{
			int count = this._usableStandingPoints.Count;
			int num = this._reevaluatedCount + 1;
			this._reevaluatedCount = num;
			if (num >= count)
			{
				foreach (ValueTuple<int, StandingPoint> valueTuple in this._usableStandingPoints)
				{
					valueTuple.Item2.HasRecentlyBeenRechecked = false;
				}
				this._isDetachmentRecentlyEvaluated = true;
				this._reevaluatedCount = 0;
				return;
			}
			this.StandingPoints[slotIndex].HasRecentlyBeenRechecked = true;
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x000CB85C File Offset: 0x000C9A5C
		float? IDetachment.GetWeightOfNextSlot(BattleSideEnum side)
		{
			if (this.IsDisabledForBattleSideAI(side))
			{
				return null;
			}
			StandingPoint suitableStandingPointFor = this.GetSuitableStandingPointFor(side, null, null, null);
			if (suitableStandingPointFor != null)
			{
				return new float?(this.GetWeightOfStandingPoint(suitableStandingPointFor));
			}
			return null;
		}

		// Token: 0x0600310A RID: 12554 RVA: 0x000CB8A0 File Offset: 0x000C9AA0
		float IDetachment.GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex)
		{
			StandingPoint standingPoint = this.StandingPoints[slotIndex];
			Vec3 globalPosition = standingPoint.GameEntity.GlobalPosition;
			WorldPosition worldPosition = new WorldPosition(candidate.Mission.Scene, globalPosition);
			WorldPosition worldPosition2 = candidate.GetWorldPosition();
			float maxValue;
			if (!standingPoint.Scene.GetPathDistanceBetweenPositions(ref worldPosition, ref worldPosition2, candidate.Monster.BodyCapsuleRadius, out maxValue))
			{
				maxValue = float.MaxValue;
			}
			return maxValue;
		}

		// Token: 0x0600310B RID: 12555 RVA: 0x000CB904 File Offset: 0x000C9B04
		List<float> IDetachment.GetTemplateCostsOfAgent(Agent candidate, List<float> oldValue)
		{
			List<float> list = oldValue ?? new List<float>(this.StandingPoints.Count);
			list.Clear();
			for (int i = 0; i < this.StandingPoints.Count; i++)
			{
				list.Add(float.MaxValue);
			}
			foreach (ValueTuple<int, StandingPoint> valueTuple in this._usableStandingPoints)
			{
				float num = valueTuple.Item2.GameEntity.GlobalPosition.Distance(candidate.Position);
				list[valueTuple.Item1] = num * MissionGameModels.Current.AgentStatCalculateModel.GetDetachmentCostMultiplierOfAgent(candidate, this);
			}
			return list;
		}

		// Token: 0x0600310C RID: 12556 RVA: 0x000CB9D0 File Offset: 0x000C9BD0
		float IDetachment.GetTemplateWeightOfAgent(Agent candidate)
		{
			Scene scene = Mission.Current.Scene;
			Vec3 globalPosition = base.GameEntity.GlobalPosition;
			WorldPosition worldPosition = candidate.GetWorldPosition();
			WorldPosition worldPosition2 = new WorldPosition(scene, UIntPtr.Zero, globalPosition, true);
			float maxValue;
			if (!scene.GetPathDistanceBetweenPositions(ref worldPosition2, ref worldPosition, candidate.Monster.BodyCapsuleRadius, out maxValue))
			{
				maxValue = float.MaxValue;
			}
			return maxValue;
		}

		// Token: 0x0600310D RID: 12557 RVA: 0x000CBA2C File Offset: 0x000C9C2C
		float IDetachment.GetWeightOfOccupiedSlot(Agent agent)
		{
			return this.GetWeightOfStandingPoint(this.StandingPoints.FirstOrDefault((StandingPoint sp) => sp.UserAgent == agent || sp.IsAIMovingTo(agent)));
		}

		// Token: 0x0600310E RID: 12558 RVA: 0x000CBA64 File Offset: 0x000C9C64
		WorldFrame? IDetachment.GetAgentFrame(Agent agent)
		{
			return null;
		}

		// Token: 0x0600310F RID: 12559 RVA: 0x000CBA7A File Offset: 0x000C9C7A
		void IDetachment.RemoveAgent(Agent agent)
		{
			agent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.None);
		}

		// Token: 0x06003110 RID: 12560 RVA: 0x000CBA84 File Offset: 0x000C9C84
		public bool IsStandingPointAvailableForAgent(Agent agent)
		{
			bool flag = false;
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				if (!standingPoint.IsDeactivated && (standingPoint.IsInstantUse || ((!standingPoint.HasUser || standingPoint.UserAgent == agent) && (!standingPoint.HasAIMovingTo || standingPoint.IsAIMovingTo(agent)))) && !standingPoint.IsDisabledForAgent(agent) && !this.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(standingPoint))
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		// Token: 0x06003111 RID: 12561 RVA: 0x000CBB1C File Offset: 0x000C9D1C
		float? IDetachment.GetWeightOfAgentAtNextSlot(List<Agent> candidates, out Agent match)
		{
			BattleSideEnum side = candidates[0].Team.Side;
			StandingPoint suitableStandingPointFor = this.GetSuitableStandingPointFor(side, null, candidates, null);
			if (suitableStandingPointFor == null)
			{
				match = null;
				return null;
			}
			match = UsableMachineAIBase.GetSuitableAgentForStandingPoint(this, suitableStandingPointFor, candidates, new List<Agent>());
			if (match == null)
			{
				return null;
			}
			float? weightOfNextSlot = ((IDetachment)this).GetWeightOfNextSlot(side);
			float num = 1f;
			float? num2 = weightOfNextSlot;
			float num3 = num;
			if (num2 == null)
			{
				return null;
			}
			return new float?(num2.GetValueOrDefault() * num3);
		}

		// Token: 0x06003112 RID: 12562 RVA: 0x000CBBA8 File Offset: 0x000C9DA8
		float? IDetachment.GetWeightOfAgentAtNextSlot(List<ValueTuple<Agent, float>> candidates, out Agent match)
		{
			BattleSideEnum side = candidates[0].Item1.Team.Side;
			StandingPoint suitableStandingPointFor = this.GetSuitableStandingPointFor(side, null, null, candidates);
			if (suitableStandingPointFor == null)
			{
				match = null;
				return null;
			}
			float? weightOfNextSlot = ((IDetachment)this).GetWeightOfNextSlot(side);
			match = UsableMachineAIBase.GetSuitableAgentForStandingPoint(this, suitableStandingPointFor, candidates, new List<Agent>(), weightOfNextSlot.Value);
			if (match == null)
			{
				return null;
			}
			float num = 1f;
			float? num2 = weightOfNextSlot;
			float num3 = num;
			if (num2 == null)
			{
				return null;
			}
			return new float?(num2.GetValueOrDefault() * num3);
		}

		// Token: 0x06003113 RID: 12563 RVA: 0x000CBC40 File Offset: 0x000C9E40
		float? IDetachment.GetWeightOfAgentAtOccupiedSlot(Agent detachedAgent, List<Agent> candidates, out Agent match)
		{
			BattleSideEnum side = candidates[0].Team.Side;
			match = null;
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				if (standingPoint.IsAIMovingTo(detachedAgent) || standingPoint.UserAgent == detachedAgent)
				{
					match = UsableMachineAIBase.GetSuitableAgentForStandingPoint(this, standingPoint, candidates, new List<Agent>());
					break;
				}
			}
			if (match == null)
			{
				return null;
			}
			float? weightOfNextSlot = ((IDetachment)this).GetWeightOfNextSlot(side);
			float num = 1f;
			if (weightOfNextSlot == null)
			{
				return null;
			}
			return new float?(weightOfNextSlot.GetValueOrDefault() * num * 0.5f);
		}

		// Token: 0x06003114 RID: 12564 RVA: 0x000CBD0C File Offset: 0x000C9F0C
		void IDetachment.AddAgent(Agent agent, int slotIndex)
		{
			StandingPoint standingPoint = ((slotIndex == -1) ? this.GetSuitableStandingPointFor(agent.Team.Side, agent, null, null) : this.StandingPoints[slotIndex]);
			if (standingPoint != null)
			{
				if (standingPoint.HasAIMovingTo && !standingPoint.IsInstantUse)
				{
					standingPoint.MovingAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
				agent.AIMoveToGameObjectEnable(standingPoint, this, this.Ai.GetScriptedFrameFlags(agent));
				if (standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
				{
					this.CurrentlyUsedAmmoPickUpPoint = standingPoint;
					return;
				}
			}
			else
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Usables\\UsableMachine.cs", "AddAgent", 1355);
			}
		}

		// Token: 0x06003115 RID: 12565 RVA: 0x000CBDA8 File Offset: 0x000C9FA8
		void IDetachment.FormationStartUsing(Formation formation)
		{
			this._userFormations.Add(formation);
		}

		// Token: 0x06003116 RID: 12566 RVA: 0x000CBDB6 File Offset: 0x000C9FB6
		void IDetachment.FormationStopUsing(Formation formation)
		{
			this._userFormations.Remove(formation);
		}

		// Token: 0x06003117 RID: 12567 RVA: 0x000CBDC5 File Offset: 0x000C9FC5
		public bool IsUsedByFormation(Formation formation)
		{
			return this._userFormations.Contains(formation);
		}

		// Token: 0x06003118 RID: 12568 RVA: 0x000CBDD3 File Offset: 0x000C9FD3
		void IDetachment.ResetEvaluation()
		{
			this._isEvaluated = false;
		}

		// Token: 0x06003119 RID: 12569 RVA: 0x000CBDDC File Offset: 0x000C9FDC
		bool IDetachment.IsEvaluated()
		{
			return this._isEvaluated;
		}

		// Token: 0x0600311A RID: 12570 RVA: 0x000CBDE4 File Offset: 0x000C9FE4
		void IDetachment.SetAsEvaluated()
		{
			this._isEvaluated = true;
		}

		// Token: 0x0600311B RID: 12571 RVA: 0x000CBDED File Offset: 0x000C9FED
		float IDetachment.GetDetachmentWeightFromCache()
		{
			return this._cachedDetachmentWeight;
		}

		// Token: 0x0600311C RID: 12572 RVA: 0x000CBDF5 File Offset: 0x000C9FF5
		float IDetachment.ComputeAndCacheDetachmentWeight(BattleSideEnum side)
		{
			this._cachedDetachmentWeight = this.GetDetachmentWeightAux(side);
			return this._cachedDetachmentWeight;
		}

		// Token: 0x0600311D RID: 12573 RVA: 0x000CBE0A File Offset: 0x000CA00A
		protected internal virtual bool IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(StandingPoint standingPoint)
		{
			return this.AmmoPickUpPoints.Contains(standingPoint) && (this.StandingPoints.Any((StandingPoint standingPoint2) => (standingPoint2.IsDeactivated || standingPoint2.HasUser || standingPoint2.HasAIMovingTo) && !standingPoint2.GameEntity.HasTag(this.AmmoPickUpTag) && standingPoint2 is StandingPointWithWeaponRequirement) || this.HasAIPickingUpAmmo);
		}

		// Token: 0x0600311E RID: 12574 RVA: 0x000CBE44 File Offset: 0x000CA044
		protected virtual StandingPoint GetSuitableStandingPointFor(BattleSideEnum side, Agent agent = null, List<Agent> agents = null, List<ValueTuple<Agent, float>> agentValuePairs = null)
		{
			return this.StandingPoints.FirstOrDefault((StandingPoint sp) => !sp.IsDeactivated && (sp.IsInstantUse || (!sp.HasUser && !sp.HasAIMovingTo)) && (agent == null || !sp.IsDisabledForAgent(agent)) && (agents == null || agents.Any((Agent a) => !sp.IsDisabledForAgent(a))) && (agentValuePairs == null || agentValuePairs.Any((ValueTuple<Agent, float> avp) => !sp.IsDisabledForAgent(avp.Item1))) && !this.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(sp));
		}

		// Token: 0x0600311F RID: 12575
		public abstract string GetDescriptionText(GameEntity gameEntity = null);

		// Token: 0x04001482 RID: 5250
		public const string UsableMachineParentTag = "machine_parent";

		// Token: 0x04001483 RID: 5251
		public string PilotStandingPointTag = "Pilot";

		// Token: 0x04001484 RID: 5252
		public string AmmoPickUpTag = "ammopickup";

		// Token: 0x04001485 RID: 5253
		public string WaitStandingPointTag = "Wait";

		// Token: 0x0400148A RID: 5258
		protected GameEntity ActiveWaitStandingPoint;

		// Token: 0x0400148B RID: 5259
		private readonly List<UsableMissionObjectComponent> _components;

		// Token: 0x0400148C RID: 5260
		private DestructableComponent _destructionComponent;

		// Token: 0x0400148D RID: 5261
		protected bool _areUsableStandingPointsVacant = true;

		// Token: 0x0400148E RID: 5262
		protected List<ValueTuple<int, StandingPoint>> _usableStandingPoints;

		// Token: 0x0400148F RID: 5263
		protected bool _isDetachmentRecentlyEvaluated;

		// Token: 0x04001490 RID: 5264
		private int _reevaluatedCount;

		// Token: 0x04001491 RID: 5265
		private bool _isEvaluated;

		// Token: 0x04001492 RID: 5266
		private float _cachedDetachmentWeight;

		// Token: 0x04001493 RID: 5267
		protected float EnemyRangeToStopUsing;

		// Token: 0x04001494 RID: 5268
		protected Vec2 MachinePositionOffsetToStopUsing = Vec2.Zero;

		// Token: 0x04001495 RID: 5269
		protected bool MakeVisibilityCheck = true;

		// Token: 0x04001496 RID: 5270
		private UsableMachineAIBase _ai;

		// Token: 0x04001497 RID: 5271
		private StandingPoint _currentlyUsedAmmoPickUpPoint;

		// Token: 0x04001498 RID: 5272
		private QueryData<bool> _isDisabledForAttackerAIDueToEnemyInRange;

		// Token: 0x04001499 RID: 5273
		private QueryData<bool> _isDisabledForDefenderAIDueToEnemyInRange;

		// Token: 0x0400149A RID: 5274
		protected bool _isDisabledForAI;

		// Token: 0x0400149B RID: 5275
		protected MBList<Formation> _userFormations;

		// Token: 0x0400149C RID: 5276
		private bool _isMachineDeactivated;
	}
}
