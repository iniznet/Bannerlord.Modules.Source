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
	public abstract class UsableMachine : SynchedMissionObject, IFocusable, IOrderable, IDetachment
	{
		public MBList<StandingPoint> StandingPoints { get; private set; }

		public StandingPoint PilotStandingPoint { get; private set; }

		protected internal List<StandingPoint> AmmoPickUpPoints { get; private set; }

		private protected List<GameEntity> WaitStandingPoints { protected get; private set; }

		public DestructableComponent DestructionComponent
		{
			get
			{
				return this._destructionComponent;
			}
		}

		public bool IsDestructible
		{
			get
			{
				return this.DestructionComponent != null;
			}
		}

		public bool IsDestroyed
		{
			get
			{
				return this.DestructionComponent != null && this.DestructionComponent.IsDestroyed;
			}
		}

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

		public bool IsLoose
		{
			get
			{
				return false;
			}
		}

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

		public virtual FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.Item;
			}
		}

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

		public bool HasAIPickingUpAmmo
		{
			get
			{
				return this.CurrentlyUsedAmmoPickUpPoint != null;
			}
		}

		public MBReadOnlyList<Formation> UserFormations
		{
			get
			{
				return this._userFormations;
			}
		}

		protected UsableMachine()
		{
			this._components = new List<UsableMissionObjectComponent>();
		}

		public void AddComponent(UsableMissionObjectComponent component)
		{
			this._components.Add(component);
			component.OnAdded(base.Scene);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public void RemoveComponent(UsableMissionObjectComponent component)
		{
			component.OnRemoved();
			this._components.Remove(component);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public T GetComponent<T>() where T : UsableMissionObjectComponent
		{
			return this._components.Find((UsableMissionObjectComponent c) => c is T) as T;
		}

		public virtual OrderType GetOrder(BattleSideEnum side)
		{
			return OrderType.Use;
		}

		public virtual UsableMachineAIBase CreateAIBehaviorObject()
		{
			return null;
		}

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

		public override void OnMissionEnded()
		{
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				Agent userAgent = standingPoint.UserAgent;
				if (userAgent != null)
				{
					userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
				standingPoint.IsDeactivated = true;
			}
		}

		public override void SetVisibleSynched(bool value, bool forceChildrenVisible = false)
		{
			base.SetVisibleSynched(value, forceChildrenVisible);
		}

		public override void SetPhysicsStateSynched(bool value, bool setChildren = true)
		{
			base.SetPhysicsStateSynched(value, setChildren);
			this.SetAbilityOfFaces(value);
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				standingPoint.OnParentMachinePhysicsStateChanged();
			}
		}

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

		public virtual int MaxUserCount
		{
			get
			{
				return this.StandingPoints.Count;
			}
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.CollectAndSetStandingPoints();
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this._isDisabledForAttackerAIDueToEnemyInRange = new QueryData<bool>(delegate
			{
				bool flag = false;
				if (this.EnemyRangeToStopUsing > 0f)
				{
					Vec3 vec = base.GameEntity.GetGlobalFrame().rotation.TransformToParent(new Vec3(this.MachinePositionOffsetToStopUsingLocal, 0f, -1f));
					Vec3 vec2 = base.GameEntity.GlobalPosition + vec;
					Agent closestEnemyAgent = Mission.Current.GetClosestEnemyAgent(Mission.Current.Teams.Attacker, vec2, this.EnemyRangeToStopUsing);
					flag = closestEnemyAgent != null && closestEnemyAgent.Position.z > vec2.z - 2f && closestEnemyAgent.Position.z < vec2.z + 4f;
				}
				return flag;
			}, 1f);
			this._isDisabledForDefenderAIDueToEnemyInRange = new QueryData<bool>(delegate
			{
				bool flag2 = false;
				if (this.EnemyRangeToStopUsing > 0f)
				{
					Vec3 vec3 = base.GameEntity.GetGlobalFrame().rotation.TransformToParent(new Vec3(this.MachinePositionOffsetToStopUsingLocal, 0f, -1f));
					Vec3 vec4 = base.GameEntity.GlobalPosition + vec3;
					Agent closestEnemyAgent2 = Mission.Current.GetClosestEnemyAgent(Mission.Current.Teams.Defender, vec4, this.EnemyRangeToStopUsing);
					flag2 = closestEnemyAgent2 != null && closestEnemyAgent2.Position.z > vec4.z - 2f && closestEnemyAgent2.Position.z < vec4.z + 4f;
				}
				return flag2;
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

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnEditorTick(dt);
			}
		}

		protected internal override void OnEditorValidate()
		{
			base.OnEditorValidate();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnEditorValidate();
			}
		}

		public virtual void OnFocusGain(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusGain(userAgent);
			}
		}

		public virtual void OnFocusLose(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusLose(userAgent);
			}
		}

		public virtual TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			return TextObject.Empty;
		}

		public virtual bool HasWaitFrame
		{
			get
			{
				return this.ActiveWaitStandingPoint != null;
			}
		}

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

		public GameEntity WaitEntity
		{
			get
			{
				return this.ActiveWaitStandingPoint;
			}
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnMissionReset();
			}
		}

		public virtual bool IsDeactivated
		{
			get
			{
				return this._isMachineDeactivated || this.IsDestroyed;
			}
		}

		public void Deactivate()
		{
			this._isMachineDeactivated = true;
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				standingPoint.IsDeactivated = true;
			}
		}

		public void Activate()
		{
			this._isMachineDeactivated = false;
			foreach (StandingPoint standingPoint in this.StandingPoints)
			{
				standingPoint.IsDeactivated = false;
			}
		}

		public virtual bool IsDisabledForBattleSide(BattleSideEnum sideEnum)
		{
			return this.IsDeactivated;
		}

		public virtual bool IsDisabledForBattleSideAI(BattleSideEnum sideEnum)
		{
			return this._isDisabledForAI || (this.EnemyRangeToStopUsing > 0f && sideEnum != BattleSideEnum.None && this.IsDisabledDueToEnemyInRange(sideEnum));
		}

		public virtual bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return true;
		}

		protected bool IsDisabledDueToEnemyInRange(BattleSideEnum sideEnum)
		{
			if (sideEnum == BattleSideEnum.Attacker)
			{
				return this._isDisabledForAttackerAIDueToEnemyInRange.Value;
			}
			return this._isDisabledForDefenderAIDueToEnemyInRange.Value;
		}

		public virtual bool AutoAttachUserToFormation(BattleSideEnum sideEnum)
		{
			return true;
		}

		public virtual bool HasToBeDefendedByUser(BattleSideEnum sideEnum)
		{
			return false;
		}

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

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnRemoved();
			}
		}

		public override string ToString()
		{
			string text = base.GetType() + " with Components:";
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				text = string.Concat(new object[] { text, "[", usableMissionObjectComponent, "]" });
			}
			return text;
		}

		public abstract TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject);

		public virtual StandingPoint GetBestPointAlternativeTo(StandingPoint standingPoint, Agent agent)
		{
			return standingPoint;
		}

		public virtual bool IsInRangeToCheckAlternativePoints(Agent agent)
		{
			float num = ((this.StandingPoints.Count > 0) ? (agent.GetInteractionDistanceToUsable(this.StandingPoints[0]) + 1f) : 2f);
			return base.GameEntity.GlobalPosition.DistanceSquared(agent.Position) < num * num;
		}

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

		private void OnFormationLeaveHelper(Formation formation, Agent agent)
		{
			((IDetachment)this).RemoveAgent(agent);
			formation.AttachUnit(agent);
		}

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

		protected virtual float GetWeightOfStandingPoint(StandingPoint sp)
		{
			if (!sp.HasAIMovingTo)
			{
				return 0.6f;
			}
			return 0.2f;
		}

		float IDetachment.GetDetachmentWeight(BattleSideEnum side)
		{
			return this.GetDetachmentWeightAux(side);
		}

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

		void IDetachment.GetSlotIndexWeightTuples(List<ValueTuple<int, float>> slotIndexWeightTuples)
		{
			foreach (ValueTuple<int, StandingPoint> valueTuple in this._usableStandingPoints)
			{
				StandingPoint item = valueTuple.Item2;
				slotIndexWeightTuples.Add(new ValueTuple<int, float>(valueTuple.Item1, this.GetWeightOfStandingPoint(item) * ((!this._areUsableStandingPointsVacant && item.HasRecentlyBeenRechecked) ? 0.1f : 1f)));
			}
		}

		bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent)
		{
			return agent.CanBeAssignedForScriptedMovement() && !this.StandingPoints[slotIndex].IsDisabledForAgent(agent) && !this.IsAgentOnInconvenientNavmesh(agent, this.StandingPoints[slotIndex]);
		}

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

		bool IDetachment.IsAgentEligible(Agent agent)
		{
			return true;
		}

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
			if (standingPoint.HasDefendingAgent)
			{
				for (int i = standingPoint.DefendingAgents.Count - 1; i >= 0; i--)
				{
					Agent agent2 = standingPoint.DefendingAgents[i];
					if (agent2 != null)
					{
						((IDetachment)this).RemoveAgent(agent2);
						Formation formation2 = agent2.Formation;
						if (formation2 != null)
						{
							formation2.AttachUnit(agent2);
						}
					}
				}
			}
			((IDetachment)this).AddAgent(agent, slotIndex);
			Formation formation3 = agent.Formation;
			if (formation3 != null)
			{
				formation3.DetachUnit(agent, false);
			}
			agent.Detachment = this;
			agent.DetachmentWeight = 1f;
		}

		Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex)
		{
			return this.StandingPoints[slotIndex].MovingAgent;
		}

		bool IDetachment.IsDetachmentRecentlyEvaluated()
		{
			return this._isDetachmentRecentlyEvaluated;
		}

		void IDetachment.UnmarkDetachment()
		{
			this._isDetachmentRecentlyEvaluated = false;
		}

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

		float IDetachment.GetWeightOfOccupiedSlot(Agent agent)
		{
			return this.GetWeightOfStandingPoint(this.StandingPoints.FirstOrDefault((StandingPoint sp) => sp.UserAgent == agent || sp.IsAIMovingTo(agent)));
		}

		WorldFrame? IDetachment.GetAgentFrame(Agent agent)
		{
			return null;
		}

		void IDetachment.RemoveAgent(Agent agent)
		{
			agent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.None);
		}

		public int GetNumberOfUsableSlots()
		{
			return this._usableStandingPoints.Count;
		}

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
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Usables\\UsableMachine.cs", "AddAgent", 1367);
			}
		}

		void IDetachment.FormationStartUsing(Formation formation)
		{
			this._userFormations.Add(formation);
		}

		void IDetachment.FormationStopUsing(Formation formation)
		{
			this._userFormations.Remove(formation);
		}

		public bool IsUsedByFormation(Formation formation)
		{
			return this._userFormations.Contains(formation);
		}

		void IDetachment.ResetEvaluation()
		{
			this._isEvaluated = false;
		}

		bool IDetachment.IsEvaluated()
		{
			return this._isEvaluated;
		}

		void IDetachment.SetAsEvaluated()
		{
			this._isEvaluated = true;
		}

		float IDetachment.GetDetachmentWeightFromCache()
		{
			return this._cachedDetachmentWeight;
		}

		float IDetachment.ComputeAndCacheDetachmentWeight(BattleSideEnum side)
		{
			this._cachedDetachmentWeight = this.GetDetachmentWeightAux(side);
			return this._cachedDetachmentWeight;
		}

		protected internal virtual bool IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(StandingPoint standingPoint)
		{
			return this.AmmoPickUpPoints.Contains(standingPoint) && (this.StandingPoints.Any((StandingPoint standingPoint2) => (standingPoint2.IsDeactivated || standingPoint2.HasUser || standingPoint2.HasAIMovingTo) && !standingPoint2.GameEntity.HasTag(this.AmmoPickUpTag) && standingPoint2 is StandingPointWithWeaponRequirement) || this.HasAIPickingUpAmmo);
		}

		protected virtual StandingPoint GetSuitableStandingPointFor(BattleSideEnum side, Agent agent = null, List<Agent> agents = null, List<ValueTuple<Agent, float>> agentValuePairs = null)
		{
			return this.StandingPoints.FirstOrDefault((StandingPoint sp) => !sp.IsDeactivated && (sp.IsInstantUse || (!sp.HasUser && !sp.HasAIMovingTo)) && (agent == null || !sp.IsDisabledForAgent(agent)) && (agents == null || agents.Any((Agent a) => !sp.IsDisabledForAgent(a))) && (agentValuePairs == null || agentValuePairs.Any((ValueTuple<Agent, float> avp) => !sp.IsDisabledForAgent(avp.Item1))) && !this.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(sp));
		}

		public abstract string GetDescriptionText(GameEntity gameEntity = null);

		public const string UsableMachineParentTag = "machine_parent";

		public string PilotStandingPointTag = "Pilot";

		public string AmmoPickUpTag = "ammopickup";

		public string WaitStandingPointTag = "Wait";

		protected GameEntity ActiveWaitStandingPoint;

		private readonly List<UsableMissionObjectComponent> _components;

		private DestructableComponent _destructionComponent;

		protected bool _areUsableStandingPointsVacant = true;

		protected List<ValueTuple<int, StandingPoint>> _usableStandingPoints;

		protected bool _isDetachmentRecentlyEvaluated;

		private int _reevaluatedCount;

		private bool _isEvaluated;

		private float _cachedDetachmentWeight;

		protected float EnemyRangeToStopUsing;

		protected Vec2 MachinePositionOffsetToStopUsingLocal = Vec2.Zero;

		protected bool MakeVisibilityCheck = true;

		private UsableMachineAIBase _ai;

		private StandingPoint _currentlyUsedAmmoPickUpPoint;

		private QueryData<bool> _isDisabledForAttackerAIDueToEnemyInRange;

		private QueryData<bool> _isDisabledForDefenderAIDueToEnemyInRange;

		protected bool _isDisabledForAI;

		protected MBList<Formation> _userFormations;

		private bool _isMachineDeactivated;
	}
}
