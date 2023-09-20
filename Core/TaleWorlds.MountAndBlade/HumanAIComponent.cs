using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class HumanAIComponent : AgentComponent
	{
		public Agent FollowedAgent { get; private set; }

		public bool ShouldCatchUpWithFormation
		{
			get
			{
				return this._shouldCatchUpWithFormation;
			}
			private set
			{
				if (this._shouldCatchUpWithFormation != value)
				{
					this._shouldCatchUpWithFormation = value;
					this.Agent.SetShouldCatchUpWithFormation(value);
				}
			}
		}

		public bool IsDefending
		{
			get
			{
				return this._objectInterestKind == HumanAIComponent.UsableObjectInterestKind.Defending;
			}
		}

		public HumanAIComponent(Agent agent)
			: base(agent)
		{
			this._behaviorValues = new HumanAIComponent.BehaviorValues[7];
			this.SetDefaultBehaviorParams();
			this.Agent.SetAllBehaviorParams(this._behaviorValues);
			this._hasNewBehaviorValues = false;
			Agent agent2 = this.Agent;
			agent2.OnAgentWieldedItemChange = (Action)Delegate.Combine(agent2.OnAgentWieldedItemChange, new Action(this.DisablePickUpForAgentIfNeeded));
			Agent agent3 = this.Agent;
			agent3.OnAgentMountedStateChanged = (Action)Delegate.Combine(agent3.OnAgentMountedStateChanged, new Action(this.DisablePickUpForAgentIfNeeded));
			this._itemPickUpTickTimer = new MissionTimer(2.5f + MBRandom.RandomFloat);
			this._mountSearchTimer = new MissionTimer(2f + MBRandom.RandomFloat);
		}

		public void SetDefaultBehaviorParams()
		{
			this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
			this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Melee, 8f, 7f, 4f, 20f, 1f);
			this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Ranged, 2f, 7f, 4f, 20f, 5f);
			this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 2f, 25f, 5f, 30f, 5f);
			this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 2f, 15f, 6.5f, 30f, 5.5f);
			this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
			this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 5.5f, 12f, 8f, 30f, 4.5f);
		}

		public void SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind behavior, float y1, float x2, float y2, float x3, float y3)
		{
			this._behaviorValues[(int)behavior].y1 = y1;
			this._behaviorValues[(int)behavior].x2 = x2;
			this._behaviorValues[(int)behavior].y2 = y2;
			this._behaviorValues[(int)behavior].x3 = x3;
			this._behaviorValues[(int)behavior].y3 = y3;
			this._hasNewBehaviorValues = true;
		}

		public void SyncBehaviorParamsIfNecessary()
		{
			if (this._hasNewBehaviorValues)
			{
				this.Agent.SetAllBehaviorParams(this._behaviorValues);
				this._hasNewBehaviorValues = false;
			}
		}

		public void DisablePickUpForAgentIfNeeded()
		{
			this._disablePickUpForAgent = true;
			if (this.Agent.MountAgent == null)
			{
				if (this.Agent.HasLostShield())
				{
					this._disablePickUpForAgent = false;
				}
				else
				{
					for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
					{
						MissionWeapon missionWeapon = this.Agent.Equipment[equipmentIndex];
						if (!missionWeapon.IsEmpty && missionWeapon.IsAnyConsumable())
						{
							this._disablePickUpForAgent = false;
							break;
						}
					}
				}
			}
			if (this._disablePickUpForAgent && this.Agent.Formation != null && MissionGameModels.Current.BattleBannerBearersModel.IsBannerSearchingAgent(this.Agent))
			{
				this._disablePickUpForAgent = false;
			}
		}

		public override void OnTickAsAI(float dt)
		{
			this.SyncBehaviorParamsIfNecessary();
			if (this._itemToPickUp != null)
			{
				if (!this._itemToPickUp.IsAIMovingTo(this.Agent) || this.Agent.Mission.MissionEnded)
				{
					this._itemToPickUp = null;
				}
				else if (this._itemToPickUp.GameEntity == null)
				{
					this.Agent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
			}
			if (this._itemPickUpTickTimer.Check(true) && !this.Agent.Mission.MissionEnded)
			{
				EquipmentIndex wieldedItemIndex = this.Agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				WeaponComponentData weaponComponentData = ((wieldedItemIndex == EquipmentIndex.None) ? null : this.Agent.Equipment[wieldedItemIndex].CurrentUsageItem);
				bool flag = weaponComponentData != null && weaponComponentData.IsRangedWeapon;
				if (!this._disablePickUpForAgent && MissionGameModels.Current.ItemPickupModel.IsAgentEquipmentSuitableForPickUpAvailability(this.Agent) && this.Agent.CanBeAssignedForScriptedMovement() && this.Agent.CurrentWatchState == Agent.WatchState.Alarmed && (this.Agent.GetAgentFlags() & AgentFlag.CanAttack) != AgentFlag.None && !this.IsInImportantCombatAction())
				{
					Agent targetAgent = this.Agent.GetTargetAgent();
					if (targetAgent != null)
					{
						Vec3 vec = targetAgent.Position;
						if (vec.DistanceSquared(this.Agent.Position) <= 400f)
						{
							goto IL_223;
						}
						if (flag && !this.IsAnyConsumableDepleted())
						{
							vec = targetAgent.Position;
							if (vec.DistanceSquared(this.Agent.Position) < this.Agent.GetMissileRange() * 1.2f && this.Agent.GetLastTargetVisibilityState() == AITargetVisibilityState.TargetIsClear)
							{
								goto IL_223;
							}
						}
					}
					float maximumForwardUnlimitedSpeed = this.Agent.MaximumForwardUnlimitedSpeed;
					if (this._itemToPickUp == null)
					{
						Vec3 vec2 = this.Agent.Position - new Vec3(maximumForwardUnlimitedSpeed, maximumForwardUnlimitedSpeed, 1f, -1f);
						Vec3 vec3 = this.Agent.Position + new Vec3(maximumForwardUnlimitedSpeed, maximumForwardUnlimitedSpeed, 1.8f, -1f);
						this._itemToPickUp = this.SelectPickableItem(vec2, vec3);
						if (this._itemToPickUp != null)
						{
							this.RequestMoveToItem(this._itemToPickUp);
						}
					}
				}
			}
			IL_223:
			if (this._itemToPickUp != null && !this.Agent.IsRunningAway && this.Agent.AIMoveToGameObjectIsEnabled())
			{
				float num = (this._itemToPickUp.IsBanner() ? MissionGameModels.Current.BattleBannerBearersModel.GetBannerInteractionDistance(this.Agent) : MissionGameModels.Current.AgentStatCalculateModel.GetInteractionDistance(this.Agent));
				num *= 3f;
				ref WorldFrame ptr = ref this._itemToPickUp.GetUserFrameForAgent(this.Agent);
				Vec3 vec = this.Agent.Position;
				float num2 = ptr.Origin.DistanceSquaredWithLimit(vec, num * num + 1E-05f);
				if (this.Agent.CanReachAndUseObject(this._itemToPickUp, num2))
				{
					this.Agent.UseGameObject(this._itemToPickUp, -1);
				}
			}
			if (this.Agent.CommonAIComponent != null && this.Agent.MountAgent == null && !this.Agent.CommonAIComponent.IsRetreating && this._mountSearchTimer.Check(true) && this.Agent.GetRidingOrder() == 1)
			{
				int selectedMountIndex = this.Agent.GetSelectedMountIndex();
				bool flag2 = true;
				if (selectedMountIndex >= 0)
				{
					foreach (Agent agent in Mission.Current.MountsWithoutRiders)
					{
						if (agent.Index == selectedMountIndex)
						{
							flag2 = agent.State != AgentState.Active || agent.RiderAgent != null;
							if (flag2)
							{
								agent.CommonAIComponent.OnMountSelectionForRiderUpdate(-1);
								break;
							}
							break;
						}
					}
				}
				if (flag2)
				{
					this.Agent.SetSelectedMountIndex(-1);
					this.FindMount();
				}
			}
		}

		private void FindMount()
		{
			float num = float.MaxValue;
			Agent agent = null;
			foreach (Agent agent2 in Mission.Current.MountsWithoutRiders)
			{
				if (agent2.IsActive() && agent2.CommonAIComponent.ReservedRiderAgentIndex < 0 && agent2.RiderAgent == null && !agent2.IsRunningAway && MissionGameModels.Current.AgentStatCalculateModel.CanAgentRideMount(this.Agent, agent2))
				{
					float num2 = this.Agent.Position.DistanceSquared(agent2.Position);
					if (6400f > num2 && num > num2)
					{
						agent = agent2;
						num = num2;
					}
				}
			}
			if (agent != null)
			{
				agent.CommonAIComponent.OnMountSelectionForRiderUpdate(this.Agent.Index);
				this.Agent.SetSelectedMountIndex(agent.Index);
			}
		}

		public override void OnAgentRemoved()
		{
			int selectedMountIndex = this.Agent.GetSelectedMountIndex();
			foreach (Agent agent in Mission.Current.MountsWithoutRiders)
			{
				if (agent.Index == selectedMountIndex && agent.CommonAIComponent.ReservedRiderAgentIndex == this.Agent.Index)
				{
					agent.CommonAIComponent.OnMountSelectionForRiderUpdate(-1);
				}
			}
		}

		public bool IsInImportantCombatAction()
		{
			Agent.ActionCodeType currentActionType = this.Agent.GetCurrentActionType(1);
			return currentActionType == Agent.ActionCodeType.ReadyMelee || currentActionType == Agent.ActionCodeType.ReadyRanged || currentActionType == Agent.ActionCodeType.ReleaseMelee || currentActionType == Agent.ActionCodeType.ReleaseRanged || currentActionType == Agent.ActionCodeType.ReleaseThrowing || currentActionType == Agent.ActionCodeType.DefendShield;
		}

		private bool IsAnyConsumableDepleted()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				MissionWeapon missionWeapon = this.Agent.Equipment[equipmentIndex];
				if (!missionWeapon.IsEmpty && missionWeapon.IsAnyConsumable() && missionWeapon.Amount == 0)
				{
					return true;
				}
			}
			return false;
		}

		private SpawnedItemEntity SelectPickableItem(Vec3 bMin, Vec3 bMax)
		{
			Agent targetAgent = this.Agent.GetTargetAgent();
			Vec3 vec = ((targetAgent == null) ? Vec3.Invalid : (targetAgent.Position - this.Agent.Position));
			int num = this.Agent.Mission.Scene.SelectEntitiesInBoxWithScriptComponent<SpawnedItemEntity>(ref bMin, ref bMax, this._tempPickableEntities, this._pickableItemsId);
			float num2 = 0f;
			SpawnedItemEntity spawnedItemEntity = null;
			for (int i = 0; i < num; i++)
			{
				SpawnedItemEntity firstScriptOfType = this._tempPickableEntities[i].GetFirstScriptOfType<SpawnedItemEntity>();
				bool flag = false;
				if (firstScriptOfType != null)
				{
					MissionWeapon weaponCopy = firstScriptOfType.WeaponCopy;
					flag = !weaponCopy.IsEmpty && (weaponCopy.IsShield() || weaponCopy.IsBanner() || firstScriptOfType.IsStuckMissile() || firstScriptOfType.IsQuiverAndNotEmpty());
				}
				if (flag && !firstScriptOfType.HasUser && (!firstScriptOfType.HasAIMovingTo || firstScriptOfType.IsAIMovingTo(this.Agent)) && firstScriptOfType.GameEntityWithWorldPosition.WorldPosition.GetNavMesh() != UIntPtr.Zero)
				{
					Vec3 vec2 = firstScriptOfType.GetUserFrameForAgent(this.Agent).Origin.GetGroundVec3() - this.Agent.Position;
					vec2.Normalize();
					if (targetAgent == null || vec.Length - Vec3.DotProduct(vec, vec2) > targetAgent.MaximumForwardUnlimitedSpeed * 3f)
					{
						EquipmentIndex equipmentIndex = MissionEquipment.SelectWeaponPickUpSlot(this.Agent, firstScriptOfType.WeaponCopy, firstScriptOfType.IsStuckMissile());
						WorldPosition worldPosition = firstScriptOfType.GameEntityWithWorldPosition.WorldPosition;
						if (equipmentIndex != EquipmentIndex.None && worldPosition.GetNavMesh() != UIntPtr.Zero && MissionGameModels.Current.ItemPickupModel.IsItemAvailableForAgent(firstScriptOfType, this.Agent, equipmentIndex) && this.Agent.CanMoveDirectlyToPosition(worldPosition))
						{
							float itemScoreForAgent = MissionGameModels.Current.ItemPickupModel.GetItemScoreForAgent(firstScriptOfType, this.Agent);
							if (itemScoreForAgent > num2)
							{
								spawnedItemEntity = firstScriptOfType;
								num2 = itemScoreForAgent;
							}
						}
					}
				}
			}
			return spawnedItemEntity;
		}

		internal void ItemPickupDone(SpawnedItemEntity spawnedItemEntity)
		{
			this._itemToPickUp = null;
		}

		private void RequestMoveToItem(SpawnedItemEntity item)
		{
			Agent movingAgent = item.MovingAgent;
			if (movingAgent != null)
			{
				movingAgent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
			this.MoveToUsableGameObject(item, null, Agent.AIScriptedFrameFlags.NoAttack);
		}

		public UsableMissionObject GetCurrentlyMovingGameObject()
		{
			return this._objectOfInterest;
		}

		private void SetCurrentlyMovingGameObject(UsableMissionObject objectOfInterest)
		{
			this._objectOfInterest = objectOfInterest;
			this._objectInterestKind = ((this._objectOfInterest != null) ? HumanAIComponent.UsableObjectInterestKind.MovingTo : HumanAIComponent.UsableObjectInterestKind.None);
		}

		public UsableMissionObject GetCurrentlyDefendingGameObject()
		{
			return this._objectOfInterest;
		}

		private void SetCurrentlyDefendingGameObject(UsableMissionObject objectOfInterest)
		{
			this._objectOfInterest = objectOfInterest;
			this._objectInterestKind = ((this._objectOfInterest != null) ? HumanAIComponent.UsableObjectInterestKind.Defending : HumanAIComponent.UsableObjectInterestKind.None);
		}

		public void MoveToUsableGameObject(UsableMissionObject usedObject, IDetachment detachment, Agent.AIScriptedFrameFlags scriptedFrameFlags = Agent.AIScriptedFrameFlags.NoAttack)
		{
			this.Agent.AIStateFlags |= Agent.AIStateFlag.UseObjectMoving;
			this.SetCurrentlyMovingGameObject(usedObject);
			usedObject.OnAIMoveToUse(this.Agent, detachment);
			WorldFrame userFrameForAgent = usedObject.GetUserFrameForAgent(this.Agent);
			this.Agent.SetScriptedPositionAndDirection(ref userFrameForAgent.Origin, userFrameForAgent.Rotation.f.AsVec2.RotationInRadians, false, scriptedFrameFlags);
		}

		public void MoveToClear()
		{
			UsableMissionObject currentlyMovingGameObject = this.GetCurrentlyMovingGameObject();
			if (currentlyMovingGameObject != null)
			{
				currentlyMovingGameObject.OnMoveToStopped(this.Agent);
			}
			this.SetCurrentlyMovingGameObject(null);
			this.Agent.AIStateFlags &= ~Agent.AIStateFlag.UseObjectMoving;
		}

		public void StartDefendingGameObject(UsableMissionObject usedObject, IDetachment detachment)
		{
			this.SetCurrentlyDefendingGameObject(usedObject);
			usedObject.OnAIDefendBegin(this.Agent, detachment);
		}

		public void StopDefendingGameObject()
		{
			this.GetCurrentlyDefendingGameObject().OnAIDefendEnd(this.Agent);
			this.SetCurrentlyDefendingGameObject(null);
		}

		public bool IsInterestedInAnyGameObject()
		{
			return this._objectInterestKind > HumanAIComponent.UsableObjectInterestKind.None;
		}

		public bool IsInterestedInGameObject(UsableMissionObject usableMissionObject)
		{
			bool flag = false;
			switch (this._objectInterestKind)
			{
			case HumanAIComponent.UsableObjectInterestKind.None:
				break;
			case HumanAIComponent.UsableObjectInterestKind.MovingTo:
				flag = usableMissionObject == this.GetCurrentlyMovingGameObject();
				break;
			case HumanAIComponent.UsableObjectInterestKind.Defending:
				flag = usableMissionObject == this.GetCurrentlyDefendingGameObject();
				break;
			default:
				Debug.FailedAssert("Unexpected object interest kind: " + this._objectInterestKind, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\AgentComponents\\HumanAIComponent.cs", "IsInterestedInGameObject", 541);
				break;
			}
			return flag;
		}

		public void FollowAgent(Agent agent)
		{
			this.FollowedAgent = agent;
		}

		public float GetDesiredSpeedInFormation(bool isCharging)
		{
			if (this.ShouldCatchUpWithFormation && (!isCharging || !Mission.Current.IsMissionEnding))
			{
				Agent mountAgent = this.Agent.MountAgent;
				float num = ((mountAgent != null) ? mountAgent.MaximumForwardUnlimitedSpeed : this.Agent.MaximumForwardUnlimitedSpeed);
				bool flag = !isCharging;
				if (isCharging)
				{
					FormationQuerySystem closestEnemyFormation = this.Agent.Formation.QuerySystem.ClosestEnemyFormation;
					float num2 = float.MaxValue;
					float num3 = 4f * num * num;
					if (closestEnemyFormation != null)
					{
						WorldPosition medianPosition = this.Agent.Formation.QuerySystem.MedianPosition;
						WorldPosition medianPosition2 = closestEnemyFormation.MedianPosition;
						num2 = medianPosition.AsVec2.DistanceSquared(medianPosition2.AsVec2);
						if (num2 <= num3)
						{
							num2 = this.Agent.Formation.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(closestEnemyFormation.MedianPosition.GetNavMeshVec3());
						}
					}
					flag = num2 > num3;
				}
				if (flag)
				{
					Vec2 vec = this.Agent.Formation.GetCurrentGlobalPositionOfUnit(this.Agent, true) - this.Agent.Position.AsVec2;
					float num4 = -this.Agent.GetMovementDirection().DotProduct(vec);
					num4 = MathF.Clamp(num4, 0f, 100f);
					float num5 = ((this.Agent.MountAgent != null) ? 4f : 2f);
					float num6 = (isCharging ? this.Agent.Formation.QuerySystem.FormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents : this.Agent.Formation.QuerySystem.MovementSpeed) / num;
					return MathF.Clamp((0.7f + 0.4f * ((num - num4 * num5) / (num + num4 * num5))) * num6, 0.3f, 1f);
				}
			}
			return 1f;
		}

		private unsafe bool GetFormationFrame(out WorldPosition formationPosition, out Vec2 formationDirection, out float speedLimit, out bool isSettingDestinationSpeed, out bool limitIsMultiplier, bool finalDestination = false)
		{
			Formation formation = this.Agent.Formation;
			isSettingDestinationSpeed = false;
			limitIsMultiplier = false;
			bool flag = false;
			if (formation != null)
			{
				formationPosition = formation.GetOrderPositionOfUnit(this.Agent);
				formationDirection = formation.GetDirectionOfUnit(this.Agent);
			}
			else
			{
				formationPosition = WorldPosition.Invalid;
				formationDirection = Vec2.Invalid;
			}
			if (HumanAIComponent.FormationSpeedAdjustmentEnabled && this.Agent.IsMount)
			{
				formationPosition = WorldPosition.Invalid;
				formationDirection = Vec2.Invalid;
				if (this.Agent.RiderAgent == null || (this.Agent.RiderAgent != null && (!this.Agent.RiderAgent.IsActive() || this.Agent.RiderAgent.Formation == null)))
				{
					speedLimit = -1f;
				}
				else
				{
					limitIsMultiplier = true;
					HumanAIComponent humanAIComponent = this.Agent.RiderAgent.HumanAIComponent;
					MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
					speedLimit = humanAIComponent.GetDesiredSpeedInFormation(movementOrder.MovementState == MovementOrder.MovementStateEnum.Charge);
				}
			}
			else if (formation == null)
			{
				speedLimit = -1f;
			}
			else if (this.Agent.IsDetachedFromFormation)
			{
				speedLimit = -1f;
				WorldFrame? worldFrame = null;
				MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
				if (movementOrder.MovementState != MovementOrder.MovementStateEnum.Charge || (this.Agent.Detachment != null && (!this.Agent.Detachment.IsLoose || formationPosition.IsValid)))
				{
					worldFrame = formation.GetDetachmentFrame(this.Agent);
				}
				if (worldFrame != null)
				{
					formationDirection = worldFrame.Value.Rotation.f.AsVec2.Normalized();
					flag = true;
				}
				else
				{
					formationDirection = Vec2.Invalid;
				}
			}
			else
			{
				MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
				switch (movementOrder.MovementState)
				{
				case MovementOrder.MovementStateEnum.Charge:
					limitIsMultiplier = true;
					speedLimit = (HumanAIComponent.FormationSpeedAdjustmentEnabled ? this.GetDesiredSpeedInFormation(true) : (-1f));
					flag = formationPosition.IsValid;
					break;
				case MovementOrder.MovementStateEnum.Hold:
					isSettingDestinationSpeed = true;
					if (HumanAIComponent.FormationSpeedAdjustmentEnabled && this.ShouldCatchUpWithFormation)
					{
						limitIsMultiplier = true;
						speedLimit = this.GetDesiredSpeedInFormation(false);
					}
					else
					{
						speedLimit = -1f;
					}
					flag = true;
					break;
				case MovementOrder.MovementStateEnum.Retreat:
					speedLimit = -1f;
					break;
				case MovementOrder.MovementStateEnum.StandGround:
					formationDirection = this.Agent.Frame.rotation.f.AsVec2;
					speedLimit = -1f;
					flag = true;
					break;
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\AgentComponents\\HumanAIComponent.cs", "GetFormationFrame", 728);
					speedLimit = -1f;
					break;
				}
			}
			return flag;
		}

		public void AdjustSpeedLimit(Agent agent, float desiredSpeed, bool limitIsMultiplier)
		{
			if (agent.MissionPeer != null)
			{
				desiredSpeed = -1f;
			}
			this.Agent.SetMaximumSpeedLimit(desiredSpeed, limitIsMultiplier);
			Agent mountAgent = agent.MountAgent;
			if (mountAgent == null)
			{
				return;
			}
			mountAgent.SetMaximumSpeedLimit(desiredSpeed, limitIsMultiplier);
		}

		public void UpdateFormationMovement()
		{
			WorldPosition worldPosition;
			Vec2 vec;
			float num;
			bool flag;
			bool flag2;
			bool formationFrame = this.GetFormationFrame(out worldPosition, out vec, out num, out flag, out flag2, false);
			this.AdjustSpeedLimit(this.Agent, num, flag2);
			if (this.Agent.Controller == Agent.ControllerType.AI && this.Agent.Formation != null && this.Agent.Formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Stop && this.Agent.Formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Retreat && !this.Agent.IsRetreating())
			{
				FormationQuerySystem.FormationIntegrityDataGroup formationIntegrityData = this.Agent.Formation.QuerySystem.FormationIntegrityData;
				float num2 = formationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents * 3f;
				if (formationIntegrityData.DeviationOfPositionsExcludeFarAgents > num2 * 100f)
				{
					this.ShouldCatchUpWithFormation = false;
					this.Agent.SetFormationIntegrityData(Vec2.Zero, Vec2.Zero, Vec2.Zero, 0f, 0f);
				}
				else
				{
					Vec2 currentGlobalPositionOfUnit = this.Agent.Formation.GetCurrentGlobalPositionOfUnit(this.Agent, true);
					float num3 = this.Agent.Position.AsVec2.Distance(currentGlobalPositionOfUnit);
					this.ShouldCatchUpWithFormation = num3 < num2 * 2f;
					this.Agent.SetFormationIntegrityData(this.ShouldCatchUpWithFormation ? currentGlobalPositionOfUnit : Vec2.Zero, this.Agent.Formation.CurrentDirection, formationIntegrityData.AverageVelocityExcludeFarAgents, formationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents, formationIntegrityData.DeviationOfPositionsExcludeFarAgents);
				}
			}
			else
			{
				this.ShouldCatchUpWithFormation = false;
			}
			if (!formationFrame || !worldPosition.IsValid)
			{
				this.Agent.SetFormationFrameDisabled();
				return;
			}
			if (!GameNetwork.IsMultiplayer && this.Agent.Mission.Mode == MissionMode.Deployment)
			{
				MBSceneUtilities.ProjectPositionToDeploymentBoundaries(this.Agent.Formation.Team.Side, ref worldPosition);
			}
			this.Agent.SetFormationFrameEnabled(worldPosition, vec, this.Agent.Formation.CalculateFormationDirectionEnforcingFactorForRank(((IFormationUnit)this.Agent).FormationRankIndex));
			float num4 = 1f;
			if (this.Agent.Formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall && !this.Agent.IsDetachedFromFormation)
			{
				num4 = this.Agent.Formation.Arrangement.GetDirectionChangeTendencyOfUnit(this.Agent);
			}
			this.Agent.SetDirectionChangeTendency(num4);
		}

		public override void OnRetreating()
		{
			base.OnRetreating();
			this.AdjustSpeedLimit(this.Agent, -1f, false);
		}

		public override void OnDismount(Agent mount)
		{
			base.OnDismount(mount);
			mount.SetMaximumSpeedLimit(-1f, false);
		}

		public override void OnMount(Agent mount)
		{
			base.OnMount(mount);
			int num = mount.CommonAIComponent.OnMountSelectionForRiderUpdate(-1);
			if (num != this.Agent.Index)
			{
				Agent agent = Mission.Current.FindAgentWithIndex(num);
				if (agent != null)
				{
					agent.SetSelectedMountIndex(-1);
				}
				int selectedMountIndex = this.Agent.GetSelectedMountIndex();
				Agent agent2 = Mission.Current.FindAgentWithIndex(selectedMountIndex);
				if (agent2 != null)
				{
					agent2.CommonAIComponent.OnMountSelectionForRiderUpdate(-1);
				}
			}
		}

		private const float AvoidPickUpIfLookAgentIsCloseDistance = 20f;

		private const float AvoidPickUpIfLookAgentIsCloseDistanceSquared = 400f;

		public static bool FormationSpeedAdjustmentEnabled = true;

		private readonly HumanAIComponent.BehaviorValues[] _behaviorValues;

		private bool _hasNewBehaviorValues;

		private readonly GameEntity[] _tempPickableEntities = new GameEntity[16];

		private readonly UIntPtr[] _pickableItemsId = new UIntPtr[16];

		private SpawnedItemEntity _itemToPickUp;

		private readonly MissionTimer _itemPickUpTickTimer;

		private bool _disablePickUpForAgent;

		private readonly MissionTimer _mountSearchTimer;

		private UsableMissionObject _objectOfInterest;

		private HumanAIComponent.UsableObjectInterestKind _objectInterestKind;

		private bool _shouldCatchUpWithFormation;

		[EngineStruct("behavior_values_struct")]
		public struct BehaviorValues
		{
			public float GetValueAt(float x)
			{
				if (x <= this.x2)
				{
					return (this.y2 - this.y1) * x / this.x2 + this.y1;
				}
				if (x <= this.x3)
				{
					return (this.y3 - this.y2) * (x - this.x2) / (this.x3 - this.x2) + this.y2;
				}
				return this.y3;
			}

			public float y1;

			public float x2;

			public float y2;

			public float x3;

			public float y3;
		}

		public enum AISimpleBehaviorKind
		{
			GoToPos,
			Melee,
			Ranged,
			ChargeHorseback,
			RangedHorseback,
			AttackEntityMelee,
			AttackEntityRanged,
			Count
		}

		public enum UsableObjectInterestKind
		{
			None,
			MovingTo,
			Defending,
			Count
		}
	}
}
