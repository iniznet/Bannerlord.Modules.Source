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
	// Token: 0x0200000A RID: 10
	public sealed class AgentNavigator
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000042B5 File Offset: 0x000024B5
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000042BD File Offset: 0x000024BD
		public UsableMachine TargetUsableMachine { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000042C6 File Offset: 0x000024C6
		// (set) Token: 0x06000046 RID: 70 RVA: 0x000042CE File Offset: 0x000024CE
		public WorldPosition TargetPosition { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000042D7 File Offset: 0x000024D7
		// (set) Token: 0x06000048 RID: 72 RVA: 0x000042DF File Offset: 0x000024DF
		public Vec2 TargetDirection { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000042E8 File Offset: 0x000024E8
		// (set) Token: 0x0600004A RID: 74 RVA: 0x000042F0 File Offset: 0x000024F0
		public GameEntity TargetEntity { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000042F9 File Offset: 0x000024F9
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00004301 File Offset: 0x00002501
		public Alley MemberOfAlley { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004D RID: 77 RVA: 0x0000430A File Offset: 0x0000250A
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00004314 File Offset: 0x00002514
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00004346 File Offset: 0x00002546
		// (set) Token: 0x06000050 RID: 80 RVA: 0x0000434E File Offset: 0x0000254E
		private Dictionary<KeyValuePair<sbyte, string>, int> _bodyComponents { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00004357 File Offset: 0x00002557
		// (set) Token: 0x06000052 RID: 82 RVA: 0x0000435F File Offset: 0x0000255F
		public AgentNavigator.NavigationState _agentState { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00004368 File Offset: 0x00002568
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00004370 File Offset: 0x00002570
		public bool CharacterHasVisiblePrefabs { get; private set; }

		// Token: 0x06000055 RID: 85 RVA: 0x0000437C File Offset: 0x0000257C
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

		// Token: 0x06000056 RID: 86 RVA: 0x000043D0 File Offset: 0x000025D0
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

		// Token: 0x06000057 RID: 87 RVA: 0x0000445E File Offset: 0x0000265E
		public void OnStopUsingGameObject()
		{
			this._targetBehavior = null;
			this.TargetUsableMachine = null;
			this._agentState = AgentNavigator.NavigationState.NoTarget;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004478 File Offset: 0x00002678
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

		// Token: 0x06000059 RID: 89 RVA: 0x000044D4 File Offset: 0x000026D4
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

		// Token: 0x0600005A RID: 90 RVA: 0x000045BC File Offset: 0x000027BC
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

		// Token: 0x0600005B RID: 91 RVA: 0x00004634 File Offset: 0x00002834
		public void ClearTarget()
		{
			this.SetTarget(null, false);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004640 File Offset: 0x00002840
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

		// Token: 0x0600005D RID: 93 RVA: 0x00004708 File Offset: 0x00002908
		public float GetDistanceToTarget(UsableMachine target)
		{
			float num = 100000f;
			if (target != null && this.OwnerAgent.CurrentlyUsedGameObject != null)
			{
				num = this.OwnerAgent.CurrentlyUsedGameObject.GetUserFrameForAgent(this.OwnerAgent).Origin.GetGroundVec3().Distance(this.OwnerAgent.Position);
			}
			return num;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004764 File Offset: 0x00002964
		public bool IsTargetReached()
		{
			if (this.TargetDirection.IsValid && this.TargetPosition.IsValid)
			{
				float num = Vec2.DotProduct(this.TargetDirection, this.OwnerAgent.GetMovementDirection());
				this._targetReached = (this.OwnerAgent.Position - this.TargetPosition.GetGroundVec3()).LengthSquared < this._rangeThreshold * this._rangeThreshold && num > this._rotationScoreThreshold;
			}
			return this._targetReached;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000047F5 File Offset: 0x000029F5
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

		// Token: 0x06000060 RID: 96 RVA: 0x00004824 File Offset: 0x00002A24
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

		// Token: 0x06000061 RID: 97 RVA: 0x000048AC File Offset: 0x00002AAC
		public void RecoverRecentlyUsedMeshes()
		{
			foreach (int num in this._prevPrefabs)
			{
				this.OwnerAgent.SetSynchedPrefabComponentVisibility(num, true);
			}
			this._prevPrefabs.Clear();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004910 File Offset: 0x00002B10
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

		// Token: 0x06000063 RID: 99 RVA: 0x000049A9 File Offset: 0x00002BA9
		public bool IsCarryingSomething()
		{
			return this.OwnerAgent.GetWieldedItemIndex(0) >= 0 || this.OwnerAgent.GetWieldedItemIndex(1) >= 0 || this._bodyComponents.Any((KeyValuePair<KeyValuePair<sbyte, string>, int> component) => this.OwnerAgent.IsSynchedPrefabComponentVisible(component.Value));
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000049E4 File Offset: 0x00002BE4
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

		// Token: 0x06000065 RID: 101 RVA: 0x00004A70 File Offset: 0x00002C70
		public bool GetPrefabVisibility(sbyte realBoneIndex, string prefabName)
		{
			KeyValuePair<sbyte, string> keyValuePair = new KeyValuePair<sbyte, string>(realBoneIndex, prefabName);
			int num;
			return this._bodyComponents.TryGetValue(keyValuePair, out num) && this.OwnerAgent.IsSynchedPrefabComponentVisible(num);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004AA8 File Offset: 0x00002CA8
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

		// Token: 0x06000067 RID: 103 RVA: 0x00004B70 File Offset: 0x00002D70
		public void SetItemsVisibility(bool isVisible)
		{
			foreach (KeyValuePair<sbyte, string> keyValuePair in this._prefabNamesForBones)
			{
				this.SetPrefabVisibility(keyValuePair.Key, keyValuePair.Value, isVisible);
			}
			this.CharacterHasVisiblePrefabs = this._prefabNamesForBones.Count > 0 && isVisible;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004BE8 File Offset: 0x00002DE8
		public void SetCommonArea(Alley alley)
		{
			if (alley != this.MemberOfAlley)
			{
				this.MemberOfAlley = alley;
				this.SpecialTargetTag = ((alley == null) ? "" : alley.Tag);
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004C10 File Offset: 0x00002E10
		public void ForceThink(float inSeconds)
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				agentBehaviorGroup.ForceThink(inSeconds);
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004C64 File Offset: 0x00002E64
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

		// Token: 0x0600006B RID: 107 RVA: 0x00004CC8 File Offset: 0x00002EC8
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

		// Token: 0x0600006C RID: 108 RVA: 0x00004D30 File Offset: 0x00002F30
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

		// Token: 0x0600006D RID: 109 RVA: 0x00004DD4 File Offset: 0x00002FD4
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

		// Token: 0x0600006E RID: 110 RVA: 0x00004E34 File Offset: 0x00003034
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

		// Token: 0x0600006F RID: 111 RVA: 0x00004E78 File Offset: 0x00003078
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

		// Token: 0x06000070 RID: 112 RVA: 0x00004F04 File Offset: 0x00003104
		private void ActivateGroup(AgentBehaviorGroup behaviorGroup)
		{
			foreach (AgentBehaviorGroup agentBehaviorGroup in this._behaviorGroups)
			{
				agentBehaviorGroup.IsActive = false;
			}
			behaviorGroup.IsActive = true;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004F5C File Offset: 0x0000315C
		private void HandleBehaviorGroups(bool isSimulation)
		{
			if (isSimulation || this._checkBehaviorGroupsTimer.ElapsedTime > 1f)
			{
				this.RefreshBehaviorGroups(isSimulation);
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004F7C File Offset: 0x0000317C
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

		// Token: 0x06000073 RID: 115 RVA: 0x00004FE8 File Offset: 0x000031E8
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

		// Token: 0x06000074 RID: 116 RVA: 0x00005048 File Offset: 0x00003248
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

		// Token: 0x0400002F RID: 47
		private const float SeeingDistance = 30f;

		// Token: 0x04000030 RID: 48
		public readonly Agent OwnerAgent;

		// Token: 0x04000036 RID: 54
		private readonly Mission _mission;

		// Token: 0x04000037 RID: 55
		private readonly List<AgentBehaviorGroup> _behaviorGroups;

		// Token: 0x04000038 RID: 56
		private readonly ItemObject _specialItem;

		// Token: 0x04000039 RID: 57
		private UsableMachineAIBase _targetBehavior;

		// Token: 0x0400003A RID: 58
		private bool _targetReached;

		// Token: 0x0400003B RID: 59
		private float _rangeThreshold;

		// Token: 0x0400003C RID: 60
		private float _rotationScoreThreshold;

		// Token: 0x0400003D RID: 61
		private string _specialTargetTag;

		// Token: 0x0400003E RID: 62
		private bool _disableClearTargetWhenTargetIsReached;

		// Token: 0x04000040 RID: 64
		private readonly Dictionary<sbyte, string> _prefabNamesForBones;

		// Token: 0x04000041 RID: 65
		private readonly List<int> _prevPrefabs;

		// Token: 0x04000044 RID: 68
		private readonly MissionConversationLogic _conversationHandler;

		// Token: 0x04000045 RID: 69
		private readonly BasicMissionTimer _checkBehaviorGroupsTimer;

		// Token: 0x020000CF RID: 207
		public enum NavigationState
		{
			// Token: 0x04000462 RID: 1122
			NoTarget,
			// Token: 0x04000463 RID: 1123
			GoToTarget,
			// Token: 0x04000464 RID: 1124
			AtTargetPosition,
			// Token: 0x04000465 RID: 1125
			UseMachine
		}
	}
}
