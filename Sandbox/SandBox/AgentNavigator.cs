using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Conversation;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public sealed class AgentNavigator
	{
		public UsableMachine TargetUsableMachine { get; private set; }

		public WorldPosition TargetPosition { get; private set; }

		public Vec2 TargetDirection { get; private set; }

		public GameEntity TargetEntity { get; private set; }

		public Alley MemberOfAlley { get; private set; }

		public string SpecialTargetTag
		{
			get
			{
				return this._specialTargetTag;
			}
			set
			{
				if (value != this._specialTargetTag)
				{
					this._specialTargetTag = value;
					AgentBehavior activeBehavior = this.GetActiveBehavior();
					if (activeBehavior != null)
					{
						activeBehavior.OnSpecialTargetChanged();
					}
				}
			}
		}

		private Dictionary<KeyValuePair<sbyte, string>, int> _bodyComponents { get; set; }

		public AgentNavigator.NavigationState _agentState { get; private set; }

		public bool CharacterHasVisiblePrefabs { get; private set; }

		public AgentNavigator(Agent agent, LocationCharacter locationCharacter)
			: this(agent)
		{
			this.SpecialTargetTag = locationCharacter.SpecialTargetTag;
			this._prefabNamesForBones = locationCharacter.PrefabNamesForBones;
			this._specialItem = locationCharacter.SpecialItem;
			this.MemberOfAlley = locationCharacter.MemberOfAlley;
			this.SetItemsVisibility(true);
			this.SetSpecialItem();
		}

		public AgentNavigator(Agent agent)
		{
			this._mission = agent.Mission;
			this._conversationHandler = this._mission.GetMissionBehavior<MissionConversationLogic>();
			this.OwnerAgent = agent;
			this._prefabNamesForBones = new Dictionary<sbyte, string>();
			this._behaviorGroups = new List<AgentBehaviorGroup>();
			this._bodyComponents = new Dictionary<KeyValuePair<sbyte, string>, int>();
			this.SpecialTargetTag = string.Empty;
			this.MemberOfAlley = null;
			this.TargetUsableMachine = null;
			this._checkBehaviorGroupsTimer = new BasicMissionTimer();
			this._prevPrefabs = new List<int>();
			this.CharacterHasVisiblePrefabs = false;
		}

		public void OnStopUsingGameObject()
		{
			this._targetBehavior = null;
			this.TargetUsableMachine = null;
			this._agentState = AgentNavigator.NavigationState.NoTarget;
		}

		public void OnAgentRemoved(Agent agent)
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				if (agentBehaviorGroup.IsActive)
				{
					agentBehaviorGroup.OnAgentRemoved(agent);
				}
			}
		}

		public void SetTarget(UsableMachine usableMachine, bool isInitialTarget = false)
		{
			if (usableMachine == null)
			{
				UsableMachine targetUsableMachine = this.TargetUsableMachine;
				if (targetUsableMachine != null)
				{
					targetUsableMachine.RemoveAgent(this.OwnerAgent);
				}
				this.TargetUsableMachine = null;
				this.OwnerAgent.DisableScriptedMovement();
				this.OwnerAgent.ClearTargetFrame();
				this.TargetPosition = WorldPosition.Invalid;
				this.TargetEntity = null;
				this._agentState = AgentNavigator.NavigationState.NoTarget;
				return;
			}
			if (this.TargetUsableMachine != usableMachine || isInitialTarget)
			{
				this.TargetPosition = WorldPosition.Invalid;
				this._agentState = AgentNavigator.NavigationState.NoTarget;
				UsableMachine targetUsableMachine2 = this.TargetUsableMachine;
				if (targetUsableMachine2 != null)
				{
					targetUsableMachine2.RemoveAgent(this.OwnerAgent);
				}
				if (usableMachine.IsStandingPointAvailableForAgent(this.OwnerAgent))
				{
					this.TargetUsableMachine = usableMachine;
					this.TargetPosition = WorldPosition.Invalid;
					this._agentState = AgentNavigator.NavigationState.UseMachine;
					this._targetBehavior = this.TargetUsableMachine.CreateAIBehaviorObject();
					this.TargetUsableMachine.AddAgent(this.OwnerAgent, -1);
					this._targetReached = false;
				}
			}
		}

		public void SetTargetFrame(WorldPosition position, float rotation, float rangeThreshold = 1f, float rotationThreshold = -10f, Agent.AIScriptedFrameFlags flags = 0, bool disableClearTargetWhenTargetIsReached = false)
		{
			if (this._agentState != AgentNavigator.NavigationState.NoTarget)
			{
				this.ClearTarget();
			}
			this.TargetPosition = position;
			this.TargetDirection = Vec2.FromRotation(rotation);
			this._rangeThreshold = rangeThreshold;
			this._rotationScoreThreshold = rotationThreshold;
			this._disableClearTargetWhenTargetIsReached = disableClearTargetWhenTargetIsReached;
			if (this.IsTargetReached())
			{
				this.TargetPosition = WorldPosition.Invalid;
				this._agentState = AgentNavigator.NavigationState.NoTarget;
				return;
			}
			this.OwnerAgent.SetScriptedPositionAndDirection(ref position, rotation, false, flags);
			this._agentState = AgentNavigator.NavigationState.GoToTarget;
		}

		public void ClearTarget()
		{
			this.SetTarget(null, false);
		}

		public void Tick(float dt, bool isSimulation = false)
		{
			this.HandleBehaviorGroups(isSimulation);
			if (ConversationMission.ConversationAgents.Contains(this.OwnerAgent))
			{
				using (List<AgentBehaviorGroup>.Enumerator enumerator = this._behaviorGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AgentBehaviorGroup agentBehaviorGroup = enumerator.Current;
						if (agentBehaviorGroup.IsActive)
						{
							agentBehaviorGroup.ConversationTick();
						}
					}
					goto IL_5E;
				}
			}
			this.TickActiveGroups(dt, isSimulation);
			IL_5E:
			if (this.TargetUsableMachine != null)
			{
				this._targetBehavior.Tick(this.OwnerAgent, null, null, dt);
			}
			else
			{
				this.HandleMovement();
			}
			if (this.TargetUsableMachine != null && isSimulation)
			{
				this._targetBehavior.TeleportUserAgentsToMachine(new List<Agent> { this.OwnerAgent });
			}
		}

		public float GetDistanceToTarget(UsableMachine target)
		{
			float num = 100000f;
			if (target != null && this.OwnerAgent.CurrentlyUsedGameObject != null)
			{
				num = this.OwnerAgent.CurrentlyUsedGameObject.GetUserFrameForAgent(this.OwnerAgent).Origin.GetGroundVec3().Distance(this.OwnerAgent.Position);
			}
			return num;
		}

		public bool IsTargetReached()
		{
			if (this.TargetDirection.IsValid && this.TargetPosition.IsValid)
			{
				float num = Vec2.DotProduct(this.TargetDirection, this.OwnerAgent.GetMovementDirection());
				this._targetReached = (this.OwnerAgent.Position - this.TargetPosition.GetGroundVec3()).LengthSquared < this._rangeThreshold * this._rangeThreshold && num > this._rotationScoreThreshold;
			}
			return this._targetReached;
		}

		private void HandleMovement()
		{
			if (this._agentState == AgentNavigator.NavigationState.GoToTarget && this.IsTargetReached())
			{
				this._agentState = AgentNavigator.NavigationState.AtTargetPosition;
				if (!this._disableClearTargetWhenTargetIsReached)
				{
					this.OwnerAgent.ClearTargetFrame();
				}
			}
		}

		public void HoldAndHideRecentlyUsedMeshes()
		{
			foreach (KeyValuePair<KeyValuePair<sbyte, string>, int> keyValuePair in this._bodyComponents)
			{
				if (this.OwnerAgent.IsSynchedPrefabComponentVisible(keyValuePair.Value))
				{
					this.OwnerAgent.SetSynchedPrefabComponentVisibility(keyValuePair.Value, false);
					this._prevPrefabs.Add(keyValuePair.Value);
				}
			}
		}

		public void RecoverRecentlyUsedMeshes()
		{
			foreach (int num in this._prevPrefabs)
			{
				this.OwnerAgent.SetSynchedPrefabComponentVisibility(num, true);
			}
			this._prevPrefabs.Clear();
		}

		public bool CanSeeAgent(Agent otherAgent)
		{
			if ((this.OwnerAgent.Position - otherAgent.Position).Length < 30f)
			{
				Vec3 eyeGlobalPosition = otherAgent.GetEyeGlobalPosition();
				Vec3 eyeGlobalPosition2 = this.OwnerAgent.GetEyeGlobalPosition();
				if (MathF.Abs(Vec3.AngleBetweenTwoVectors(otherAgent.Position - this.OwnerAgent.Position, this.OwnerAgent.LookDirection)) < 1.5f)
				{
					float num;
					return !Mission.Current.Scene.RayCastForClosestEntityOrTerrain(eyeGlobalPosition2, eyeGlobalPosition, ref num, 0.01f, 79617);
				}
			}
			return false;
		}

		public bool IsCarryingSomething()
		{
			return this.OwnerAgent.GetWieldedItemIndex(0) >= 0 || this.OwnerAgent.GetWieldedItemIndex(1) >= 0 || this._bodyComponents.Any((KeyValuePair<KeyValuePair<sbyte, string>, int> component) => this.OwnerAgent.IsSynchedPrefabComponentVisible(component.Value));
		}

		public void SetPrefabVisibility(sbyte realBoneIndex, string prefabName, bool isVisible)
		{
			KeyValuePair<sbyte, string> keyValuePair = new KeyValuePair<sbyte, string>(realBoneIndex, prefabName);
			int num2;
			if (isVisible)
			{
				int num;
				if (!this._bodyComponents.TryGetValue(keyValuePair, out num))
				{
					this._bodyComponents.Add(keyValuePair, this.OwnerAgent.AddSynchedPrefabComponentToBone(prefabName, realBoneIndex));
					return;
				}
				if (!this.OwnerAgent.IsSynchedPrefabComponentVisible(num))
				{
					this.OwnerAgent.SetSynchedPrefabComponentVisibility(num, true);
					return;
				}
			}
			else if (this._bodyComponents.TryGetValue(keyValuePair, out num2) && this.OwnerAgent.IsSynchedPrefabComponentVisible(num2))
			{
				this.OwnerAgent.SetSynchedPrefabComponentVisibility(num2, false);
			}
		}

		public bool GetPrefabVisibility(sbyte realBoneIndex, string prefabName)
		{
			KeyValuePair<sbyte, string> keyValuePair = new KeyValuePair<sbyte, string>(realBoneIndex, prefabName);
			int num;
			return this._bodyComponents.TryGetValue(keyValuePair, out num) && this.OwnerAgent.IsSynchedPrefabComponentVisible(num);
		}

		public void SetSpecialItem()
		{
			if (this._specialItem != null)
			{
				bool flag = false;
				EquipmentIndex equipmentIndex = -1;
				for (EquipmentIndex equipmentIndex2 = 0; equipmentIndex2 <= 3; equipmentIndex2++)
				{
					if (this.OwnerAgent.Equipment[equipmentIndex2].IsEmpty)
					{
						equipmentIndex = equipmentIndex2;
					}
					else if (this.OwnerAgent.Equipment[equipmentIndex2].Item == this._specialItem)
					{
						equipmentIndex = equipmentIndex2;
						flag = true;
						break;
					}
				}
				if (equipmentIndex == -1)
				{
					this.OwnerAgent.DropItem(3, 0);
					equipmentIndex = 3;
				}
				if (!flag)
				{
					ItemObject specialItem = this._specialItem;
					ItemModifier itemModifier = null;
					IAgentOriginBase origin = this.OwnerAgent.Origin;
					MissionWeapon missionWeapon;
					missionWeapon..ctor(specialItem, itemModifier, (origin != null) ? origin.Banner : null);
					this.OwnerAgent.EquipWeaponWithNewEntity(equipmentIndex, ref missionWeapon);
				}
				this.OwnerAgent.TryToWieldWeaponInSlot(equipmentIndex, 1, false);
			}
		}

		public void SetItemsVisibility(bool isVisible)
		{
			foreach (KeyValuePair<sbyte, string> keyValuePair in this._prefabNamesForBones)
			{
				this.SetPrefabVisibility(keyValuePair.Key, keyValuePair.Value, isVisible);
			}
			this.CharacterHasVisiblePrefabs = this._prefabNamesForBones.Count > 0 && isVisible;
		}

		public void SetCommonArea(Alley alley)
		{
			if (alley != this.MemberOfAlley)
			{
				this.MemberOfAlley = alley;
				this.SpecialTargetTag = ((alley == null) ? "" : alley.Tag);
			}
		}

		public void ForceThink(float inSeconds)
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				agentBehaviorGroup.ForceThink(inSeconds);
			}
		}

		public T AddBehaviorGroup<T>() where T : AgentBehaviorGroup
		{
			T t = this.GetBehaviorGroup<T>();
			if (t == null)
			{
				t = Activator.CreateInstance(typeof(T), new object[] { this, this._mission }) as T;
				if (t != null)
				{
					this._behaviorGroups.Add(t);
				}
			}
			return t;
		}

		public T GetBehaviorGroup<T>() where T : AgentBehaviorGroup
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				if (agentBehaviorGroup is T)
				{
					return (T)((object)agentBehaviorGroup);
				}
			}
			return default(T);
		}

		public AgentBehavior GetBehavior<T>() where T : AgentBehavior
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				foreach (AgentBehavior agentBehavior in agentBehaviorGroup.Behaviors)
				{
					if (agentBehavior.GetType() == typeof(T))
					{
						return agentBehavior;
					}
				}
			}
			return null;
		}

		public bool HasBehaviorGroup<T>()
		{
			using (List<AgentBehaviorGroup>.Enumerator enumerator = this._behaviorGroups.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType() is T)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void RemoveBehaviorGroup<T>() where T : AgentBehaviorGroup
		{
			for (int i = 0; i < this._behaviorGroups.Count; i++)
			{
				if (this._behaviorGroups[i] is T)
				{
					this._behaviorGroups.RemoveAt(i);
				}
			}
		}

		public void RefreshBehaviorGroups(bool isSimulation)
		{
			this._checkBehaviorGroupsTimer.Reset();
			float num = 0f;
			AgentBehaviorGroup agentBehaviorGroup = null;
			foreach (AgentBehaviorGroup agentBehaviorGroup2 in this._behaviorGroups)
			{
				float score = agentBehaviorGroup2.GetScore(isSimulation);
				if (score > num)
				{
					num = score;
					agentBehaviorGroup = agentBehaviorGroup2;
				}
			}
			if (num > 0f && agentBehaviorGroup != null && !agentBehaviorGroup.IsActive)
			{
				this.ActivateGroup(agentBehaviorGroup);
			}
		}

		private void ActivateGroup(AgentBehaviorGroup behaviorGroup)
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				agentBehaviorGroup.IsActive = false;
			}
			behaviorGroup.IsActive = true;
		}

		private void HandleBehaviorGroups(bool isSimulation)
		{
			if (isSimulation || this._checkBehaviorGroupsTimer.ElapsedTime > 1f)
			{
				this.RefreshBehaviorGroups(isSimulation);
			}
		}

		private void TickActiveGroups(float dt, bool isSimulation)
		{
			if (!this.OwnerAgent.IsActive())
			{
				return;
			}
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				if (agentBehaviorGroup.IsActive)
				{
					agentBehaviorGroup.Tick(dt, isSimulation);
				}
			}
		}

		public AgentBehavior GetActiveBehavior()
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				if (agentBehaviorGroup.IsActive)
				{
					return agentBehaviorGroup.GetActiveBehavior();
				}
			}
			return null;
		}

		public AgentBehaviorGroup GetActiveBehaviorGroup()
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				if (agentBehaviorGroup.IsActive)
				{
					return agentBehaviorGroup;
				}
			}
			return null;
		}

		private const float SeeingDistance = 30f;

		public readonly Agent OwnerAgent;

		private readonly Mission _mission;

		private readonly List<AgentBehaviorGroup> _behaviorGroups;

		private readonly ItemObject _specialItem;

		private UsableMachineAIBase _targetBehavior;

		private bool _targetReached;

		private float _rangeThreshold;

		private float _rotationScoreThreshold;

		private string _specialTargetTag;

		private bool _disableClearTargetWhenTargetIsReached;

		private readonly Dictionary<sbyte, string> _prefabNamesForBones;

		private readonly List<int> _prevPrefabs;

		private readonly MissionConversationLogic _conversationHandler;

		private readonly BasicMissionTimer _checkBehaviorGroupsTimer;

		public enum NavigationState
		{
			NoTarget,
			GoToTarget,
			AtTargetPosition,
			UseMachine
		}
	}
}
