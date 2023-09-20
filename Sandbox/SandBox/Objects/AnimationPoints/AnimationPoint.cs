using System;
using System.Collections.Generic;
using SandBox.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.Objects.AnimationPoints
{
	public class AnimationPoint : StandingPoint
	{
		public override bool PlayerStopsUsingWhenInteractsWithOther
		{
			get
			{
				return false;
			}
		}

		public bool IsArriveActionFinished { get; private set; }

		protected string SelectedRightHandItem
		{
			get
			{
				return this._selectedRightHandItem;
			}
			set
			{
				if (value != this._selectedRightHandItem)
				{
					AnimationPoint.ItemForBone itemForBone = new AnimationPoint.ItemForBone(this.RightHandItemBone, value, false);
					this.AssignItemToBone(itemForBone);
					this._selectedRightHandItem = value;
				}
			}
		}

		protected string SelectedLeftHandItem
		{
			get
			{
				return this._selectedLeftHandItem;
			}
			set
			{
				if (value != this._selectedLeftHandItem)
				{
					AnimationPoint.ItemForBone itemForBone = new AnimationPoint.ItemForBone(this.LeftHandItemBone, value, false);
					this.AssignItemToBone(itemForBone);
					this._selectedLeftHandItem = value;
				}
			}
		}

		public bool IsActive { get; private set; } = true;

		public AnimationPoint()
		{
			this._greetingTimer = null;
		}

		public override bool DisableCombatActionsOnUse
		{
			get
			{
				return !base.IsInstantUse;
			}
		}

		private void CreateVisualizer()
		{
			if (this.PairLoopStartActionCode != ActionIndexCache.act_none || this.LoopStartActionCode != ActionIndexCache.act_none)
			{
				this._animatedEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, false);
				this._animatedEntity.EntityFlags = this._animatedEntity.EntityFlags | 131072;
				this._animatedEntity.Name = "ap_visual_entity";
				MBActionSet mbactionSet = MBActionSet.GetActionSetWithIndex(0);
				ActionIndexCache actionIndexCache = ActionIndexCache.act_none;
				int numberOfActionSets = MBActionSet.GetNumberOfActionSets();
				for (int i = 0; i < numberOfActionSets; i++)
				{
					MBActionSet actionSetWithIndex = MBActionSet.GetActionSetWithIndex(i);
					if (this.ArriveActionCode == ActionIndexCache.act_none || MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, this.ArriveActionCode))
					{
						if (this.PairLoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, this.PairLoopStartActionCode))
						{
							mbactionSet = actionSetWithIndex;
							actionIndexCache = this.PairLoopStartActionCode;
							break;
						}
						if (this.LoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, this.LoopStartActionCode))
						{
							mbactionSet = actionSetWithIndex;
							actionIndexCache = this.LoopStartActionCode;
							break;
						}
					}
				}
				if (actionIndexCache == null || actionIndexCache == ActionIndexCache.act_none)
				{
					actionIndexCache = ActionIndexCache.Create("act_jump_loop");
				}
				GameEntityExtensions.CreateAgentSkeleton(this._animatedEntity, "human_skeleton", true, mbactionSet, "human", MBObjectManager.Instance.GetObject<Monster>("human"));
				MBSkeletonExtensions.SetAgentActionChannel(this._animatedEntity.Skeleton, 0, actionIndexCache, 0f, -0.2f, true);
				this._animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("roman_cloth_tunic_a", true, false));
				this._animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("casual_02_boots", true, false));
				this._animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("hands_male_a", true, false));
				this._animatedEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("head_male_a", true, false));
				this._animatedEntityDisplacement = Vec3.Zero;
				if (this.ArriveActionCode != ActionIndexCache.act_none && (MBActionSet.GetActionAnimationFlags(mbactionSet, this.ArriveActionCode) & 70368744177664L) != null)
				{
					this._animatedEntityDisplacement = MBActionSet.GetActionDisplacementVector(mbactionSet, this.ArriveActionCode);
				}
				this.UpdateAnimatedEntityFrame();
			}
		}

		private void UpdateAnimatedEntityFrame()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			MatrixFrame matrixFrame = default(MatrixFrame);
			matrixFrame.rotation = Mat3.Identity;
			matrixFrame.origin = this._animatedEntityDisplacement;
			matrixFrame = globalFrame.TransformToParent(matrixFrame);
			globalFrame.origin = matrixFrame.origin;
			this._animatedEntity.SetFrame(ref globalFrame);
		}

		protected override void OnEditModeVisibilityChanged(bool currentVisibility)
		{
			if (this._animatedEntity != null)
			{
				this._animatedEntity.SetVisibilityExcludeParents(currentVisibility);
				if (!base.GameEntity.IsGhostObject())
				{
					this._resyncAnimations = true;
				}
			}
		}

		protected override void OnEditorTick(float dt)
		{
			if (this._animatedEntity != null)
			{
				if (this._resyncAnimations)
				{
					this.ResetAnimations();
					this._resyncAnimations = false;
				}
				bool flag = this._animatedEntity.IsVisibleIncludeParents();
				if (flag && !MBEditor.HelpersEnabled())
				{
					this._animatedEntity.SetVisibilityExcludeParents(false);
					flag = false;
				}
				if (flag)
				{
					this.UpdateAnimatedEntityFrame();
				}
			}
		}

		protected override void OnEditorInit()
		{
			this._itemsForBones = new List<AnimationPoint.ItemForBone>();
			this.SetActionCodes();
			this.InitParameters();
			if (!base.GameEntity.IsGhostObject())
			{
				this.CreateVisualizer();
			}
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			if (this._animatedEntity != null && this._animatedEntity.Scene == base.GameEntity.Scene)
			{
				this._animatedEntity.Remove(removeReason);
				this._animatedEntity = null;
			}
		}

		protected void ResetAnimations()
		{
			ActionIndexCache actionIndexCache = ActionIndexCache.act_none;
			int numberOfActionSets = MBActionSet.GetNumberOfActionSets();
			for (int i = 0; i < numberOfActionSets; i++)
			{
				MBActionSet actionSetWithIndex = MBActionSet.GetActionSetWithIndex(i);
				if (this.ArriveActionCode == ActionIndexCache.act_none || MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, this.ArriveActionCode))
				{
					if (this.PairLoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, this.PairLoopStartActionCode))
					{
						actionIndexCache = this.PairLoopStartActionCode;
						break;
					}
					if (this.LoopStartActionCode != ActionIndexCache.act_none && MBActionSet.CheckActionAnimationClipExists(actionSetWithIndex, this.LoopStartActionCode))
					{
						actionIndexCache = this.LoopStartActionCode;
						break;
					}
				}
			}
			if (actionIndexCache != null && actionIndexCache != ActionIndexCache.act_none)
			{
				ActionIndexCache actionIndexCache2 = ActionIndexCache.Create("act_jump_loop");
				MBSkeletonExtensions.SetAgentActionChannel(this._animatedEntity.Skeleton, 0, actionIndexCache2, 0f, -0.2f, true);
				MBSkeletonExtensions.SetAgentActionChannel(this._animatedEntity.Skeleton, 0, actionIndexCache, 0f, -0.2f, true);
			}
		}

		protected override void OnEditorVariableChanged(string variableName)
		{
			if (this.ShouldUpdateOnEditorVariableChanged(variableName))
			{
				if (this._animatedEntity != null)
				{
					this._animatedEntity.Remove(91);
				}
				this.SetActionCodes();
				this.CreateVisualizer();
			}
		}

		public void RequestResync()
		{
			this._resyncAnimations = true;
		}

		public override void AfterMissionStart()
		{
			if (Agent.Main != null && this.LoopStartActionCode != ActionIndexCache.act_none && !MBActionSet.CheckActionAnimationClipExists(Agent.Main.ActionSet, this.LoopStartActionCode))
			{
				base.IsDisabledForPlayers = true;
			}
		}

		protected virtual bool ShouldUpdateOnEditorVariableChanged(string variableName)
		{
			return variableName == "ArriveAction" || variableName == "LoopStartAction" || variableName == "PairLoopStartAction";
		}

		protected void ClearAssignedItems()
		{
			this.SetAgentItemsVisibility(false);
			this._itemsForBones.Clear();
		}

		protected void AssignItemToBone(AnimationPoint.ItemForBone newItem)
		{
			if (!string.IsNullOrEmpty(newItem.ItemPrefabName) && !this._itemsForBones.Contains(newItem))
			{
				this._itemsForBones.Add(newItem);
			}
		}

		public override bool IsDisabledForAgent(Agent agent)
		{
			if (base.HasUser && base.UserAgent == agent)
			{
				return !this.IsActive || base.IsDeactivated;
			}
			if (!this.IsActive || agent.MountAgent != null || base.IsDeactivated || !agent.IsOnLand() || (!agent.IsAIControlled && (base.IsDisabledForPlayers || base.HasUser)))
			{
				return true;
			}
			GameEntity parent = base.GameEntity.Parent;
			if (parent == null || !parent.HasScriptOfType<UsableMachine>() || !base.GameEntity.HasTag("alternative"))
			{
				return base.IsDisabledForAgent(agent);
			}
			if (agent.IsAIControlled && parent.HasTag("reserved"))
			{
				return true;
			}
			CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
			string text = ((((component != null) ? component.AgentNavigator : null) != null) ? agent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag : string.Empty);
			if (!string.IsNullOrEmpty(text) && !parent.HasTag(text))
			{
				return true;
			}
			using (List<StandingPoint>.Enumerator enumerator = parent.GetFirstScriptOfType<UsableMachine>().StandingPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AnimationPoint animationPoint;
					if ((animationPoint = enumerator.Current as AnimationPoint) != null && this.GroupId == animationPoint.GroupId && !animationPoint.IsDeactivated && (animationPoint.HasUser || (animationPoint.HasAIMovingTo && !animationPoint.IsAIMovingTo(agent))) && animationPoint.GameEntity.HasTag("alternative"))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected override void OnInit()
		{
			base.OnInit();
			this._itemsForBones = new List<AnimationPoint.ItemForBone>();
			this.SetActionCodes();
			this.InitParameters();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		private void InitParameters()
		{
			this._greetingTimer = null;
			this._pointRotation = Vec3.Zero;
			this._state = AnimationPoint.State.NotUsing;
			this._pairPoints = this.GetPairs(this.PairEntity);
			if (this.ActivatePairs)
			{
				this.SetPairsActivity(false);
			}
			this.LockUserPositions = true;
		}

		protected virtual void SetActionCodes()
		{
			this.ArriveActionCode = ActionIndexCache.Create(this.ArriveAction);
			this.LoopStartActionCode = ActionIndexCache.Create(this.LoopStartAction);
			this.PairLoopStartActionCode = ActionIndexCache.Create(this.PairLoopStartAction);
			this.LeaveActionCode = ActionIndexCache.Create(this.LeaveAction);
			this.SelectedRightHandItem = this.RightHandItem;
			this.SelectedLeftHandItem = this.LeftHandItem;
		}

		protected override bool DoesActionTypeStopUsingGameObject(Agent.ActionCodeType actionType)
		{
			return false;
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.HasUser)
			{
				return base.GetTickRequirement() | 2;
			}
			return base.GetTickRequirement();
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			this.Tick(dt, false);
		}

		private List<AnimationPoint> GetPairs(GameEntity entity)
		{
			List<AnimationPoint> list = new List<AnimationPoint>();
			if (entity != null)
			{
				if (entity.HasScriptOfType<AnimationPoint>())
				{
					AnimationPoint firstScriptOfType = entity.GetFirstScriptOfType<AnimationPoint>();
					list.Add(firstScriptOfType);
				}
				else
				{
					foreach (GameEntity gameEntity in entity.GetChildren())
					{
						list.AddRange(this.GetPairs(gameEntity));
					}
				}
			}
			if (list.Contains(this))
			{
				list.Remove(this);
			}
			return list;
		}

		public override WorldFrame GetUserFrameForAgent(Agent agent)
		{
			WorldFrame userFrameForAgent = base.GetUserFrameForAgent(agent);
			float agentScale = agent.AgentScale;
			userFrameForAgent.Origin.SetVec2(userFrameForAgent.Origin.AsVec2 + (userFrameForAgent.Rotation.f.AsVec2 * -this.ForwardDistanceToPivotPoint + userFrameForAgent.Rotation.s.AsVec2 * this.SideDistanceToPivotPoint) * (1f - agentScale));
			return userFrameForAgent;
		}

		private void Tick(float dt, bool isSimulation = false)
		{
			if (base.HasUser)
			{
				if (Game.Current != null && Game.Current.IsDevelopmentMode)
				{
					base.UserAgent.GetTargetPosition().IsNonZero();
				}
				ActionIndexValueCache currentActionValue = base.UserAgent.GetCurrentActionValue(0);
				switch (this._state)
				{
				case AnimationPoint.State.NotUsing:
					if (this.IsTargetReached() && base.UserAgent.MovementVelocity.LengthSquared < 0.1f && base.UserAgent.IsOnLand())
					{
						if (this.ArriveActionCode != ActionIndexCache.act_none)
						{
							Agent userAgent = base.UserAgent;
							int num = 0;
							ActionIndexCache arriveActionCode = this.ArriveActionCode;
							bool flag = false;
							ulong num2 = 0UL;
							float num3 = 0f;
							float num4 = (float)(isSimulation ? 0 : 0);
							userAgent.SetActionChannel(num, arriveActionCode, flag, num2, num3, MBRandom.RandomFloatRanged(0.8f, 1f), num4, 0.4f, 0f, false, -0.2f, 0, true);
						}
						this._state = AnimationPoint.State.StartToUse;
						return;
					}
					break;
				case AnimationPoint.State.StartToUse:
					if (this.ArriveActionCode != ActionIndexCache.act_none && isSimulation)
					{
						this.SimulateAnimations(0.1f);
					}
					if (this.ArriveActionCode == ActionIndexCache.act_none || currentActionValue == this.ArriveActionCode || base.UserAgent.ActionSet.AreActionsAlternatives(currentActionValue, this.ArriveActionCode))
					{
						base.UserAgent.ClearTargetFrame();
						WorldFrame userFrameForAgent = this.GetUserFrameForAgent(base.UserAgent);
						this._pointRotation = userFrameForAgent.Rotation.f;
						this._pointRotation.Normalize();
						if (base.UserAgent != Agent.Main)
						{
							base.UserAgent.SetScriptedPositionAndDirection(ref userFrameForAgent.Origin, userFrameForAgent.Rotation.f.AsVec2.RotationInRadians, false, 16);
						}
						this._state = AnimationPoint.State.Using;
						return;
					}
					break;
				case AnimationPoint.State.Using:
					if (isSimulation)
					{
						float num5 = 0.1f;
						if (currentActionValue != this.ArriveActionCode)
						{
							num5 = 0.01f + MBRandom.RandomFloat * 0.09f;
						}
						this.SimulateAnimations(num5);
					}
					if (!this.IsArriveActionFinished && (this.ArriveActionCode == ActionIndexCache.act_none || base.UserAgent.GetCurrentActionValue(0) != this.ArriveActionCode))
					{
						this.IsArriveActionFinished = true;
						this.AddItemsToAgent();
					}
					if (this.IsRotationCorrectDuringUsage())
					{
						base.UserAgent.SetActionChannel(0, this.LoopStartActionCode, false, 0UL, 0f, (this.ActionSpeed < 0.8f) ? this.ActionSpeed : MBRandom.RandomFloatRanged(0.8f, this.ActionSpeed), (float)(isSimulation ? 0 : 0), 0.4f, isSimulation ? MBRandom.RandomFloatRanged(0f, 0.5f) : 0f, false, -0.2f, 0, true);
					}
					if (this.IsArriveActionFinished && base.UserAgent != Agent.Main)
					{
						this.PairTick(isSimulation);
					}
					break;
				default:
					return;
				}
			}
		}

		private void PairTick(bool isSimulation)
		{
			MBList<Agent> pairEntityUsers = this.GetPairEntityUsers();
			if (this.PairEntity != null)
			{
				bool flag = base.UserAgent != ConversationMission.OneToOneConversationAgent && pairEntityUsers.Count + 1 >= this.MinUserToStartInteraction;
				this.SetAgentItemsVisibility(flag);
			}
			if (this._pairState != AnimationPoint.PairState.NoPair && pairEntityUsers.Count < this.MinUserToStartInteraction)
			{
				this._pairState = AnimationPoint.PairState.NoPair;
				if (base.UserAgent != ConversationMission.OneToOneConversationAgent)
				{
					base.UserAgent.SetActionChannel(0, this._lastAction, false, (ulong)((long)base.UserAgent.GetCurrentActionPriority(0)), 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					base.UserAgent.ResetLookAgent();
				}
				this._greetingTimer = null;
			}
			else if (this._pairState == AnimationPoint.PairState.NoPair && pairEntityUsers.Count >= this.MinUserToStartInteraction && this.IsRotationCorrectDuringUsage())
			{
				this._lastAction = base.UserAgent.GetCurrentActionValue(0);
				if (this._startPairAnimationWithGreeting)
				{
					this._pairState = AnimationPoint.PairState.BecomePair;
					this._greetingTimer = new Timer(Mission.Current.CurrentTime, (float)MBRandom.RandomInt(5) * 0.3f, true);
				}
				else
				{
					this._pairState = AnimationPoint.PairState.StartPairAnimation;
				}
			}
			else if (this._pairState == AnimationPoint.PairState.BecomePair && this._greetingTimer.Check(Mission.Current.CurrentTime))
			{
				this._greetingTimer = null;
				this._pairState = AnimationPoint.PairState.Greeting;
				Vec3 eyeGlobalPosition = Extensions.GetRandomElement<Agent>(pairEntityUsers).GetEyeGlobalPosition();
				Vec3 eyeGlobalPosition2 = base.UserAgent.GetEyeGlobalPosition();
				Vec3 vec = eyeGlobalPosition - eyeGlobalPosition2;
				vec.Normalize();
				Mat3 rotation = base.UserAgent.Frame.rotation;
				if (Vec3.DotProduct(rotation.f, vec) > 0f)
				{
					ActionIndexCache greetingActionId = this.GetGreetingActionId(eyeGlobalPosition2, eyeGlobalPosition, rotation);
					base.UserAgent.SetActionChannel(1, greetingActionId, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				}
			}
			else if (this._pairState == AnimationPoint.PairState.Greeting && base.UserAgent.GetCurrentActionValue(1) == ActionIndexCache.act_none)
			{
				this._pairState = AnimationPoint.PairState.StartPairAnimation;
			}
			if (this._pairState == AnimationPoint.PairState.StartPairAnimation)
			{
				this._pairState = AnimationPoint.PairState.Pair;
				base.UserAgent.SetActionChannel(0, this.PairLoopStartActionCode, false, 0UL, 0f, MBRandom.RandomFloatRanged(0.8f, this.ActionSpeed), (float)(isSimulation ? 0 : 0), 0.4f, isSimulation ? MBRandom.RandomFloatRanged(0f, 0.5f) : 0f, false, -0.2f, 0, true);
			}
			if (this._pairState == AnimationPoint.PairState.Pair && this.IsRotationCorrectDuringUsage())
			{
				base.UserAgent.SetActionChannel(0, this.PairLoopStartActionCode, false, 0UL, 0f, MBRandom.RandomFloatRanged(0.8f, this.ActionSpeed), (float)(isSimulation ? 0 : 0), 0.4f, isSimulation ? MBRandom.RandomFloatRanged(0f, 0.5f) : 0f, false, -0.2f, 0, true);
			}
		}

		private ActionIndexCache GetGreetingActionId(Vec3 userAgentGlobalEyePoint, Vec3 lookTarget, Mat3 userAgentRot)
		{
			Vec3 vec = lookTarget - userAgentGlobalEyePoint;
			vec.Normalize();
			float num = Vec3.DotProduct(userAgentRot.f, vec);
			if (num > 0.8f)
			{
				return this._greetingFrontActions[MBRandom.RandomInt(this._greetingFrontActions.Length)];
			}
			if (num <= 0f)
			{
				return ActionIndexCache.act_none;
			}
			if (Vec3.DotProduct(Vec3.CrossProduct(vec, userAgentRot.f), userAgentRot.u) > 0f)
			{
				return this._greetingRightActions[MBRandom.RandomInt(this._greetingRightActions.Length)];
			}
			return this._greetingLeftActions[MBRandom.RandomInt(this._greetingLeftActions.Length)];
		}

		private MBList<Agent> GetPairEntityUsers()
		{
			MBList<Agent> mblist = new MBList<Agent>();
			if (base.UserAgent != ConversationMission.OneToOneConversationAgent)
			{
				foreach (AnimationPoint animationPoint in this._pairPoints)
				{
					if (animationPoint.HasUser && animationPoint._state == AnimationPoint.State.Using && animationPoint.UserAgent != ConversationMission.OneToOneConversationAgent)
					{
						mblist.Add(animationPoint.UserAgent);
					}
				}
			}
			return mblist;
		}

		private void SetPairsActivity(bool isActive)
		{
			foreach (AnimationPoint animationPoint in this._pairPoints)
			{
				animationPoint.IsActive = isActive;
				if (!isActive)
				{
					if (animationPoint.HasAIUser)
					{
						animationPoint.UserAgent.StopUsingGameObject(true, 1);
					}
					Agent movingAgent = animationPoint.MovingAgent;
					if (movingAgent != null)
					{
						movingAgent.StopUsingGameObject(true, 1);
					}
				}
			}
		}

		public override bool IsUsableByAgent(Agent userAgent)
		{
			return this.IsActive && base.IsUsableByAgent(userAgent);
		}

		public override void OnUse(Agent userAgent)
		{
			base.OnUse(userAgent);
			this._equipmentIndexMainHand = base.UserAgent.GetWieldedItemIndex(0);
			this._equipmentIndexOffHand = base.UserAgent.GetWieldedItemIndex(1);
			this._state = AnimationPoint.State.NotUsing;
			if (this.ActivatePairs)
			{
				this.SetPairsActivity(true);
			}
		}

		private void RevertWeaponWieldSheathState()
		{
			if (this._equipmentIndexMainHand != -1 && this.AutoSheathWeapons)
			{
				base.UserAgent.TryToWieldWeaponInSlot(this._equipmentIndexMainHand, 0, false);
			}
			else if (this._equipmentIndexMainHand == -1 && this.AutoWieldWeapons)
			{
				base.UserAgent.TryToSheathWeaponInHand(0, 0);
			}
			if (this._equipmentIndexOffHand != -1 && this.AutoSheathWeapons)
			{
				base.UserAgent.TryToWieldWeaponInSlot(this._equipmentIndexOffHand, 0, false);
				return;
			}
			if (this._equipmentIndexOffHand == -1 && this.AutoWieldWeapons)
			{
				base.UserAgent.TryToSheathWeaponInHand(1, 0);
			}
		}

		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			this.SetAgentItemsVisibility(false);
			this.RevertWeaponWieldSheathState();
			if (base.UserAgent.IsActive())
			{
				if (this.LeaveActionCode == ActionIndexCache.act_none)
				{
					base.UserAgent.SetActionChannel(0, this.LeaveActionCode, false, (ulong)((long)base.UserAgent.GetCurrentActionPriority(0)), 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				}
				else if (this.IsArriveActionFinished)
				{
					ActionIndexValueCache currentActionValue = base.UserAgent.GetCurrentActionValue(0);
					if (currentActionValue != this.LeaveActionCode && !base.UserAgent.ActionSet.AreActionsAlternatives(currentActionValue, this.LeaveActionCode))
					{
						base.UserAgent.SetActionChannel(0, this.LeaveActionCode, false, (ulong)((long)base.UserAgent.GetCurrentActionPriority(0)), 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					}
				}
				else
				{
					ActionIndexValueCache currentActionValue2 = userAgent.GetCurrentActionValue(0);
					if (currentActionValue2 == this.ArriveActionCode && this.ArriveActionCode != ActionIndexCache.act_none)
					{
						MBActionSet actionSet = userAgent.ActionSet;
						float currentActionProgress = userAgent.GetCurrentActionProgress(0);
						float actionBlendOutStartProgress = MBActionSet.GetActionBlendOutStartProgress(actionSet, currentActionValue2);
						if (currentActionProgress < actionBlendOutStartProgress)
						{
							float num = (actionBlendOutStartProgress - currentActionProgress) / actionBlendOutStartProgress;
							MBActionSet.GetActionBlendOutStartProgress(actionSet, this.LeaveActionCode);
						}
					}
				}
			}
			this._pairState = AnimationPoint.PairState.NoPair;
			this._lastAction = ActionIndexValueCache.act_none;
			if (base.UserAgent.GetLookAgent() != null)
			{
				base.UserAgent.ResetLookAgent();
			}
			this.IsArriveActionFinished = false;
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			if (this.ActivatePairs)
			{
				this.SetPairsActivity(false);
			}
		}

		public override void SimulateTick(float dt)
		{
			this.Tick(dt, true);
		}

		public override bool HasAlternative()
		{
			return this.GroupId >= 0;
		}

		public float GetRandomWaitInSeconds()
		{
			if (this.MinWaitinSeconds < 0f || this.MaxWaitInSeconds < 0f)
			{
				return -1f;
			}
			if (MathF.Abs(this.MinWaitinSeconds - this.MaxWaitInSeconds) >= 1E-45f)
			{
				return this.MinWaitinSeconds + MBRandom.RandomFloat * (this.MaxWaitInSeconds - this.MinWaitinSeconds);
			}
			return this.MinWaitinSeconds;
		}

		public List<AnimationPoint> GetAlternatives()
		{
			List<AnimationPoint> list = new List<AnimationPoint>();
			IEnumerable<GameEntity> children = base.GameEntity.Parent.GetChildren();
			if (children != null)
			{
				foreach (GameEntity gameEntity in children)
				{
					AnimationPoint firstScriptOfType = gameEntity.GetFirstScriptOfType<AnimationPoint>();
					if (firstScriptOfType != null && firstScriptOfType.HasAlternative() && this.GroupId == firstScriptOfType.GroupId)
					{
						list.Add(firstScriptOfType);
					}
				}
			}
			return list;
		}

		private void SimulateAnimations(float dt)
		{
			base.UserAgent.TickActionChannels(dt);
			Vec3 vec = base.UserAgent.ComputeAnimationDisplacement(dt);
			if (vec.LengthSquared > 0f)
			{
				base.UserAgent.TeleportToPosition(base.UserAgent.Position + vec);
			}
			base.UserAgent.AgentVisuals.GetSkeleton().TickAnimations(dt, base.UserAgent.AgentVisuals.GetGlobalFrame(), true);
		}

		private bool IsTargetReached()
		{
			float num = Vec2.DotProduct(base.UserAgent.GetTargetDirection().AsVec2, base.UserAgent.GetMovementDirection());
			return (base.UserAgent.Position.AsVec2 - base.UserAgent.GetTargetPosition()).LengthSquared < 0.040000003f && num > 0.99f;
		}

		public bool IsRotationCorrectDuringUsage()
		{
			return this._pointRotation.IsNonZero && Vec2.DotProduct(this._pointRotation.AsVec2, base.UserAgent.GetMovementDirection()) > 0.99f;
		}

		protected bool CanAgentUseItem(Agent agent)
		{
			return agent.GetComponent<CampaignAgentComponent>() != null && agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null;
		}

		protected void AddItemsToAgent()
		{
			if (this.CanAgentUseItem(base.UserAgent) && this.IsArriveActionFinished)
			{
				if (this._itemsForBones.Count != 0)
				{
					base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.HoldAndHideRecentlyUsedMeshes();
				}
				foreach (AnimationPoint.ItemForBone itemForBone in this._itemsForBones)
				{
					ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(itemForBone.ItemPrefabName);
					if (@object != null)
					{
						EquipmentIndex equipmentIndex = this.FindProperSlot(@object);
						if (!base.UserAgent.Equipment[equipmentIndex].IsEmpty)
						{
							base.UserAgent.DropItem(equipmentIndex, 0);
						}
						ItemObject itemObject = @object;
						ItemModifier itemModifier = null;
						IAgentOriginBase origin = base.UserAgent.Origin;
						MissionWeapon missionWeapon;
						missionWeapon..ctor(itemObject, itemModifier, (origin != null) ? origin.Banner : null);
						base.UserAgent.EquipWeaponWithNewEntity(equipmentIndex, ref missionWeapon);
						base.UserAgent.TryToWieldWeaponInSlot(equipmentIndex, 1, false);
					}
					else
					{
						sbyte realBoneIndex = base.UserAgent.AgentVisuals.GetRealBoneIndex(itemForBone.HumanBone);
						base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(realBoneIndex, itemForBone.ItemPrefabName, true);
					}
				}
			}
		}

		public override void OnUserConversationStart()
		{
			this._pointRotation = base.UserAgent.Frame.rotation.f;
			this._pointRotation.Normalize();
			if (!this.KeepOldVisibility)
			{
				foreach (AnimationPoint.ItemForBone itemForBone in this._itemsForBones)
				{
					itemForBone.OldVisibility = itemForBone.IsVisible;
				}
				this.SetAgentItemsVisibility(false);
			}
		}

		public override void OnUserConversationEnd()
		{
			base.UserAgent.ResetLookAgent();
			base.UserAgent.LookDirection = this._pointRotation;
			base.UserAgent.SetActionChannel(0, this.LoopStartActionCode, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			foreach (AnimationPoint.ItemForBone itemForBone in this._itemsForBones)
			{
				if (itemForBone.OldVisibility)
				{
					this.SetAgentItemVisibility(itemForBone, true);
				}
			}
		}

		public void SetAgentItemsVisibility(bool isVisible)
		{
			if (!base.UserAgent.IsMainAgent)
			{
				foreach (AnimationPoint.ItemForBone itemForBone in this._itemsForBones)
				{
					sbyte realBoneIndex = base.UserAgent.AgentVisuals.GetRealBoneIndex(itemForBone.HumanBone);
					base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(realBoneIndex, itemForBone.ItemPrefabName, isVisible);
					AnimationPoint.ItemForBone itemForBone2 = itemForBone;
					itemForBone2.IsVisible = isVisible;
				}
			}
		}

		private void SetAgentItemVisibility(AnimationPoint.ItemForBone item, bool isVisible)
		{
			sbyte realBoneIndex = base.UserAgent.AgentVisuals.GetRealBoneIndex(item.HumanBone);
			base.UserAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetPrefabVisibility(realBoneIndex, item.ItemPrefabName, isVisible);
			item.IsVisible = isVisible;
		}

		private EquipmentIndex FindProperSlot(ItemObject item)
		{
			EquipmentIndex equipmentIndex = 3;
			for (EquipmentIndex equipmentIndex2 = 0; equipmentIndex2 <= 3; equipmentIndex2++)
			{
				if (base.UserAgent.Equipment[equipmentIndex2].IsEmpty)
				{
					equipmentIndex = equipmentIndex2;
				}
				else if (base.UserAgent.Equipment[equipmentIndex2].Item == item)
				{
					return equipmentIndex2;
				}
			}
			return equipmentIndex;
		}

		private const string AlternativeTag = "alternative";

		private const float RangeThreshold = 0.2f;

		private const float RotationScoreThreshold = 0.99f;

		private const float ActionSpeedRandomMinValue = 0.8f;

		private const float AnimationRandomProgressMaxValue = 0.5f;

		public string ArriveAction = "";

		public string LoopStartAction = "";

		public string PairLoopStartAction = "";

		public string LeaveAction = "";

		public int GroupId = -1;

		public string RightHandItem = "";

		public HumanBone RightHandItemBone = 27;

		public string LeftHandItem = "";

		public HumanBone LeftHandItemBone = 20;

		public GameEntity PairEntity;

		public int MinUserToStartInteraction = 1;

		public bool ActivatePairs;

		public float MinWaitinSeconds = 30f;

		public float MaxWaitInSeconds = 120f;

		public float ForwardDistanceToPivotPoint;

		public float SideDistanceToPivotPoint;

		private bool _startPairAnimationWithGreeting;

		protected float ActionSpeed = 1f;

		public bool KeepOldVisibility;

		private ActionIndexCache ArriveActionCode;

		protected ActionIndexCache LoopStartActionCode;

		protected ActionIndexCache PairLoopStartActionCode;

		private ActionIndexCache LeaveActionCode;

		protected ActionIndexCache DefaultActionCode;

		private bool _resyncAnimations;

		private string _selectedRightHandItem;

		private string _selectedLeftHandItem;

		private AnimationPoint.State _state;

		private AnimationPoint.PairState _pairState;

		private Vec3 _pointRotation;

		private List<AnimationPoint> _pairPoints;

		private List<AnimationPoint.ItemForBone> _itemsForBones;

		private ActionIndexValueCache _lastAction;

		private Timer _greetingTimer;

		private GameEntity _animatedEntity;

		private Vec3 _animatedEntityDisplacement = Vec3.Zero;

		private EquipmentIndex _equipmentIndexMainHand;

		private EquipmentIndex _equipmentIndexOffHand;

		private readonly ActionIndexCache[] _greetingFrontActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_greeting_front_1"),
			ActionIndexCache.Create("act_greeting_front_2"),
			ActionIndexCache.Create("act_greeting_front_3"),
			ActionIndexCache.Create("act_greeting_front_4")
		};

		private readonly ActionIndexCache[] _greetingRightActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_greeting_right_1"),
			ActionIndexCache.Create("act_greeting_right_2"),
			ActionIndexCache.Create("act_greeting_right_3"),
			ActionIndexCache.Create("act_greeting_right_4")
		};

		private readonly ActionIndexCache[] _greetingLeftActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_greeting_left_1"),
			ActionIndexCache.Create("act_greeting_left_2"),
			ActionIndexCache.Create("act_greeting_left_3"),
			ActionIndexCache.Create("act_greeting_left_4")
		};

		protected struct ItemForBone
		{
			public ItemForBone(HumanBone bone, string name, bool isVisible)
			{
				this.HumanBone = bone;
				this.ItemPrefabName = name;
				this.IsVisible = isVisible;
				this.OldVisibility = isVisible;
			}

			public HumanBone HumanBone;

			public string ItemPrefabName;

			public bool IsVisible;

			public bool OldVisibility;
		}

		private enum State
		{
			NotUsing,
			StartToUse,
			Using
		}

		private enum PairState
		{
			NoPair,
			BecomePair,
			Greeting,
			StartPairAnimation,
			Pair
		}
	}
}
