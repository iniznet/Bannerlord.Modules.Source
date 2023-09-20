using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Source.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	public class CastleGate : UsableMachine, IPointDefendable, ICastleKeyPosition, ITargetable
	{
		public TacticalPosition MiddlePosition { get; private set; }

		private static int BatteringRamHitSoundIdCache
		{
			get
			{
				if (CastleGate._batteringRamHitSoundId == -1)
				{
					CastleGate._batteringRamHitSoundId = SoundEvent.GetEventIdFromString("event:/mission/siege/door/hit");
				}
				return CastleGate._batteringRamHitSoundId;
			}
		}

		public TacticalPosition WaitPosition { get; private set; }

		public override FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.Gate;
			}
		}

		public CastleGate.GateState State { get; private set; }

		public bool IsGateOpen
		{
			get
			{
				return this.State == CastleGate.GateState.Open || base.IsDestroyed;
			}
		}

		public IPrimarySiegeWeapon AttackerSiegeWeapon { get; set; }

		public IEnumerable<DefencePoint> DefencePoints { get; protected set; }

		public CastleGate()
		{
			this._attackOnlyDoorColliders = new List<GameEntity>();
		}

		public Vec3 GetPosition()
		{
			return base.GameEntity.GlobalPosition;
		}

		public override OrderType GetOrder(BattleSideEnum side)
		{
			if (base.IsDestroyed)
			{
				return OrderType.None;
			}
			if (side != BattleSideEnum.Attacker)
			{
				return OrderType.Use;
			}
			return OrderType.AttackEntity;
		}

		public FormationAI.BehaviorSide DefenseSide { get; private set; }

		public WorldFrame MiddleFrame
		{
			get
			{
				return this._middleFrame;
			}
		}

		public WorldFrame DefenseWaitFrame
		{
			get
			{
				return this._defenseWaitFrame;
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			DestructableComponent destructableComponent = base.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
			if (destructableComponent != null)
			{
				destructableComponent.OnNextDestructionState += this.OnNextDestructionState;
				this.DestructibleComponentOnMissionReset = new Action(destructableComponent.OnMissionReset);
				if (!GameNetwork.IsClientOrReplay)
				{
					destructableComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
					destructableComponent.OnHitTaken += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnHitTaken);
					DestructableComponent destructableComponent2 = destructableComponent;
					destructableComponent2.OnCalculateDestructionStateIndex = (Func<int, int, int, int>)Delegate.Combine(destructableComponent2.OnCalculateDestructionStateIndex, new Func<int, int, int, int>(this.OnCalculateDestructionStateIndex));
				}
				destructableComponent.BattleSide = BattleSideEnum.Defender;
			}
			this.CollectGameEntities(true);
			base.GameEntity.SetAnimationSoundActivation(true);
			if (GameNetwork.IsClientOrReplay)
			{
				return;
			}
			this._queueManager = base.GameEntity.GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
			if (this._queueManager == null)
			{
				GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity ce) => ce.GetScriptComponents<LadderQueueManager>().Any<LadderQueueManager>());
				if (gameEntity != null)
				{
					this._queueManager = gameEntity.GetFirstScriptOfType<LadderQueueManager>();
				}
			}
			if (this._queueManager != null)
			{
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin.y = identity.origin.y - 2f;
				identity.rotation.RotateAboutSide(-1.5707964f);
				identity.rotation.RotateAboutForward(3.1415927f);
				this._queueManager.Initialize(this._queueManager.ManagedNavigationFaceId, identity, -identity.rotation.u, BattleSideEnum.Defender, 15, 0.62831855f, 3f, 2.2f, 0f, 0f, false, 1f, 2.1474836E+09f, 5f, false, -2, -2, int.MaxValue, 15);
				this._queueManager.Activate();
			}
			string sideTag = this.SideTag;
			if (!(sideTag == "left"))
			{
				if (!(sideTag == "middle"))
				{
					if (!(sideTag == "right"))
					{
						this.DefenseSide = FormationAI.BehaviorSide.BehaviorSideNotSet;
					}
					else
					{
						this.DefenseSide = FormationAI.BehaviorSide.Right;
					}
				}
				else
				{
					this.DefenseSide = FormationAI.BehaviorSide.Middle;
				}
			}
			else
			{
				this.DefenseSide = FormationAI.BehaviorSide.Left;
			}
			List<GameEntity> list = base.GameEntity.CollectChildrenEntitiesWithTag("middle_pos");
			if (list.Count > 0)
			{
				GameEntity gameEntity2 = list.FirstOrDefault<GameEntity>();
				this.MiddlePosition = gameEntity2.GetFirstScriptOfType<TacticalPosition>();
				MatrixFrame globalFrame = gameEntity2.GetGlobalFrame();
				this._middleFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
				this._middleFrame.Origin.GetGroundVec3();
			}
			else
			{
				MatrixFrame globalFrame2 = base.GameEntity.GetGlobalFrame();
				this._middleFrame = new WorldFrame(globalFrame2.rotation, globalFrame2.origin.ToWorldPosition());
			}
			List<GameEntity> list2 = base.GameEntity.CollectChildrenEntitiesWithTag("wait_pos");
			if (list2.Count > 0)
			{
				GameEntity gameEntity3 = list2.FirstOrDefault<GameEntity>();
				this.WaitPosition = gameEntity3.GetFirstScriptOfType<TacticalPosition>();
				MatrixFrame globalFrame3 = gameEntity3.GetGlobalFrame();
				this._defenseWaitFrame = new WorldFrame(globalFrame3.rotation, globalFrame3.origin.ToWorldPosition());
				this._defenseWaitFrame.Origin.GetGroundVec3();
			}
			else
			{
				this._defenseWaitFrame = this._middleFrame;
			}
			this._openingAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.OpeningAnimationName);
			this._closingAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.ClosingAnimationName);
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this.OnCheckForProblems();
		}

		public void SetUsableTeam(Team team)
		{
			using (List<StandingPoint>.Enumerator enumerator = base.StandingPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StandingPointWithTeamLimit standingPointWithTeamLimit;
					if ((standingPointWithTeamLimit = enumerator.Current as StandingPointWithTeamLimit) != null)
					{
						standingPointWithTeamLimit.UsableTeam = team;
					}
				}
			}
		}

		public override void AfterMissionStart()
		{
			this._afterMissionStartTriggered = true;
			base.AfterMissionStart();
			this.SetInitialStateOfGate();
			this.InitializeExtraColliderPositions();
			if (!GameNetwork.IsClientOrReplay)
			{
				this.SetAutoOpenState(Mission.Current.IsSallyOutBattle);
			}
			if (this.OwningTeam == CastleGate.DoorOwnership.Attackers)
			{
				this.SetUsableTeam(Mission.Current.AttackerTeam);
			}
			else if (this.OwningTeam == CastleGate.DoorOwnership.Defenders)
			{
				this.SetUsableTeam(Mission.Current.DefenderTeam);
			}
			this._pathChecker = new AgentPathNavMeshChecker(Mission.Current, base.GameEntity.GetGlobalFrame(), 2f, this.NavigationMeshId, BattleSideEnum.Defender, AgentPathNavMeshChecker.Direction.BothDirections, 14f, 3f);
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			DestructableComponent destructableComponent = base.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
			if (destructableComponent != null)
			{
				destructableComponent.OnNextDestructionState -= this.OnNextDestructionState;
				if (!GameNetwork.IsClientOrReplay)
				{
					destructableComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
					destructableComponent.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnHitTaken);
					DestructableComponent destructableComponent2 = destructableComponent;
					destructableComponent2.OnCalculateDestructionStateIndex = (Func<int, int, int, int>)Delegate.Remove(destructableComponent2.OnCalculateDestructionStateIndex, new Func<int, int, int, int>(this.OnCalculateDestructionStateIndex));
				}
			}
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			if (base.GameEntity.HasTag("outer_gate") && base.GameEntity.HasTag("inner_gate"))
			{
				MBDebug.ShowWarning("Castle gate has both the outer gate tag and the inner gate tag.");
			}
		}

		protected internal override void OnMissionReset()
		{
			Action destructibleComponentOnMissionReset = this.DestructibleComponentOnMissionReset;
			if (destructibleComponentOnMissionReset != null)
			{
				destructibleComponentOnMissionReset();
			}
			this.CollectGameEntities(false);
			base.OnMissionReset();
			this.SetInitialStateOfGate();
			this._previousAnimationProgress = -1f;
		}

		private void SetInitialStateOfGate()
		{
			if (!GameNetwork.IsClientOrReplay && this.NavigationMeshIdToDisableOnOpen != -1)
			{
				this._openNavMeshIdDisabled = false;
				base.Scene.SetAbilityOfFacesWithId(this.NavigationMeshIdToDisableOnOpen, true);
			}
			if (!this._civilianMission)
			{
				this._doorSkeleton.SetAnimationAtChannel(this._closingAnimationIndex, 0, 1f, -1f, 0f);
				this._doorSkeleton.SetAnimationParameterAtChannel(0, 0.99f);
				this._doorSkeleton.Freeze(false);
				this.State = CastleGate.GateState.Closed;
				return;
			}
			this.OpenDoor();
			if (this._doorSkeleton != null)
			{
				this._door.SetAnimationChannelParameterSynched(0, 1f);
			}
			this.SetGateNavMeshState(true);
			base.SetDisabled(true);
			DestructableComponent firstScriptOfType = base.GameEntity.GetFirstScriptOfType<DestructableComponent>();
			if (firstScriptOfType == null)
			{
				return;
			}
			firstScriptOfType.SetDisabled(false);
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return new TextObject("{=6wZUG0ev}Gate", null).ToString();
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject(usableGameObject.GameEntity.HasTag("open") ? "{=5oozsaIb}{KEY} Open" : "{=TJj71hPO}{KEY} Close", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new CastleGateAI(this);
		}

		public void OpenDoorAndDisableGateForCivilianMission()
		{
			this._civilianMission = true;
		}

		public void OpenDoor()
		{
			if (!base.IsDisabled)
			{
				this.State = CastleGate.GateState.Open;
				if (!this.AutoOpen)
				{
					this.SetGateNavMeshState(true);
				}
				else
				{
					this.SetGateNavMeshStateForEnemies(true);
				}
				int animationIndexAtChannel = this._doorSkeleton.GetAnimationIndexAtChannel(0);
				float animationParameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
				this._door.SetAnimationAtChannelSynched(this._openingAnimationIndex, 0, 1f);
				if (animationIndexAtChannel == this._closingAnimationIndex)
				{
					this._door.SetAnimationChannelParameterSynched(0, 1f - animationParameterAtChannel);
				}
				SynchedMissionObject plank = this._plank;
				if (plank == null)
				{
					return;
				}
				plank.SetVisibleSynched(false, false);
			}
		}

		public void CloseDoor()
		{
			if (!base.IsDisabled)
			{
				this.State = CastleGate.GateState.Closed;
				if (!this.AutoOpen)
				{
					this.SetGateNavMeshState(false);
				}
				else
				{
					this.SetGateNavMeshStateForEnemies(false);
				}
				int animationIndexAtChannel = this._doorSkeleton.GetAnimationIndexAtChannel(0);
				float animationParameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
				this._door.SetAnimationAtChannelSynched(this._closingAnimationIndex, 0, 1f);
				if (animationIndexAtChannel == this._openingAnimationIndex)
				{
					this._door.SetAnimationChannelParameterSynched(0, 1f - animationParameterAtChannel);
				}
			}
		}

		private void UpdateDoorBodies(bool updateAnyway)
		{
			if (this._attackOnlyDoorColliders.Count == 2)
			{
				float animationParameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
				if (this._previousAnimationProgress != animationParameterAtChannel || updateAnyway)
				{
					this._previousAnimationProgress = animationParameterAtChannel;
					MatrixFrame matrixFrame = this._doorSkeleton.GetBoneEntitialFrameWithIndex(this._leftDoorBoneIndex);
					MatrixFrame matrixFrame2 = this._doorSkeleton.GetBoneEntitialFrameWithIndex(this._rightDoorBoneIndex);
					this._attackOnlyDoorColliders[0].SetFrame(ref matrixFrame2);
					this._attackOnlyDoorColliders[1].SetFrame(ref matrixFrame);
					GameEntity agentColliderLeft = this._agentColliderLeft;
					if (agentColliderLeft != null)
					{
						agentColliderLeft.SetFrame(ref matrixFrame);
					}
					GameEntity agentColliderRight = this._agentColliderRight;
					if (agentColliderRight != null)
					{
						agentColliderRight.SetFrame(ref matrixFrame2);
					}
					if (this._extraColliderLeft != null && this._extraColliderRight != null)
					{
						if (this.State == CastleGate.GateState.Closed)
						{
							if (!this._leftExtraColliderDisabled)
							{
								this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
								this._leftExtraColliderDisabled = true;
							}
							if (!this._rightExtraColliderDisabled)
							{
								this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
								this._rightExtraColliderDisabled = true;
								return;
							}
						}
						else
						{
							float num = (matrixFrame2.origin - matrixFrame.origin).Length * 0.5f;
							float num2 = Vec3.DotProduct(matrixFrame2.rotation.s, Vec3.Side) / (matrixFrame2.rotation.s.Length * 1f);
							float num3 = MathF.Sqrt(1f - num2 * num2);
							float num4 = num * 1.1f;
							float num5 = MBMath.Map(num2, 0.3f, 1f, 0f, 1f) * (num * 0.2f);
							this._extraColliderLeft.SetLocalPosition(matrixFrame.origin - new Vec3(num4 - num + num5, num * num3, 0f, -1f));
							this._extraColliderRight.SetLocalPosition(matrixFrame2.origin - new Vec3(-(num4 - num) - num5, num * num3, 0f, -1f));
							float num6;
							if (num2 < 0f)
							{
								num6 = num;
								num6 += num * -num2;
							}
							else
							{
								num6 = num - num * num2;
							}
							num6 = (num4 - num6) / num;
							if (num6 <= 0.0001f)
							{
								if (!this._leftExtraColliderDisabled)
								{
									this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
									this._leftExtraColliderDisabled = true;
								}
							}
							else
							{
								if (this._leftExtraColliderDisabled)
								{
									this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag & ~BodyFlags.Disabled);
									this._leftExtraColliderDisabled = false;
								}
								matrixFrame = this._extraColliderLeft.GetFrame();
								matrixFrame.rotation.Orthonormalize();
								matrixFrame.origin -= new Vec3(num4 - num4 * num6, 0f, 0f, -1f);
								this._extraColliderLeft.SetFrame(ref matrixFrame);
							}
							matrixFrame2 = this._extraColliderRight.GetFrame();
							matrixFrame2.rotation.Orthonormalize();
							float num7;
							if (num2 < 0f)
							{
								num7 = num;
								num7 += num * -num2;
							}
							else
							{
								num7 = num - num * num2;
							}
							num7 = (num4 - num7) / num;
							if (num7 > 0.0001f)
							{
								if (this._rightExtraColliderDisabled)
								{
									this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag & ~BodyFlags.Disabled);
									this._rightExtraColliderDisabled = false;
								}
								matrixFrame2.origin += new Vec3(num4 - num4 * num7, 0f, 0f, -1f);
								this._extraColliderRight.SetFrame(ref matrixFrame2);
								return;
							}
							if (!this._rightExtraColliderDisabled)
							{
								this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
								this._rightExtraColliderDisabled = true;
								return;
							}
						}
					}
				}
			}
			else if (this._attackOnlyDoorColliders.Count == 1)
			{
				MatrixFrame boneEntitialFrameWithName = this._doorSkeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName);
				this._attackOnlyDoorColliders[0].SetFrame(ref boneEntitialFrameWithName);
				GameEntity agentColliderRight2 = this._agentColliderRight;
				if (agentColliderRight2 == null)
				{
					return;
				}
				agentColliderRight2.SetFrame(ref boneEntitialFrameWithName);
			}
		}

		private void SetGateNavMeshState(bool isEnabled)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				base.Scene.SetAbilityOfFacesWithId(this.NavigationMeshId, isEnabled);
				if (this._queueManager != null)
				{
					this._queueManager.Activate();
					base.Scene.SetAbilityOfFacesWithId(this._queueManager.ManagedNavigationFaceId, isEnabled);
				}
			}
		}

		private void SetGateNavMeshStateForEnemies(bool isEnabled)
		{
			Team attackerTeam = Mission.Current.AttackerTeam;
			if (attackerTeam != null)
			{
				foreach (Agent agent in attackerTeam.ActiveAgents)
				{
					if (agent.IsAIControlled)
					{
						agent.SetAgentExcludeStateForFaceGroupId(this.NavigationMeshId, !isEnabled);
					}
				}
			}
		}

		public void SetAutoOpenState(bool isEnabled)
		{
			this.AutoOpen = isEnabled;
			if (this.AutoOpen)
			{
				this.SetGateNavMeshState(true);
				this.SetGateNavMeshStateForEnemies(this.State == CastleGate.GateState.Open);
				return;
			}
			if (this.State == CastleGate.GateState.Open)
			{
				this.CloseDoor();
			}
			else
			{
				this.SetGateNavMeshState(false);
			}
			this.SetGateNavMeshStateForEnemies(true);
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!base.GameEntity.IsVisibleIncludeParents())
			{
				return;
			}
			if (!GameNetwork.IsClientOrReplay && this.NavigationMeshIdToDisableOnOpen != -1)
			{
				if (this._openNavMeshIdDisabled)
				{
					if (base.IsDestroyed)
					{
						base.Scene.SetAbilityOfFacesWithId(this.NavigationMeshIdToDisableOnOpen, true);
						this._openNavMeshIdDisabled = false;
					}
					else if (this.State == CastleGate.GateState.Closed)
					{
						int animationIndexAtChannel = this._doorSkeleton.GetAnimationIndexAtChannel(0);
						float animationParameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
						if (animationIndexAtChannel != this._closingAnimationIndex || animationParameterAtChannel > 0.4f)
						{
							base.Scene.SetAbilityOfFacesWithId(this.NavigationMeshIdToDisableOnOpen, true);
							this._openNavMeshIdDisabled = false;
						}
					}
				}
				else if (this.State == CastleGate.GateState.Open && !base.IsDestroyed)
				{
					base.Scene.SetAbilityOfFacesWithId(this.NavigationMeshIdToDisableOnOpen, false);
					this._openNavMeshIdDisabled = true;
				}
			}
			if (this._afterMissionStartTriggered)
			{
				this.UpdateDoorBodies(false);
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				this.ServerTick(dt);
			}
			if (base.Ai.HasActionCompleted)
			{
				bool flag = false;
				for (int i = 0; i < base.StandingPoints.Count; i++)
				{
					if (base.StandingPoints[i].HasUser || base.StandingPoints[i].HasAIMovingTo)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					bool flag2 = false;
					for (int j = 0; j < this._userFormations.Count; j++)
					{
						if (this._userFormations[j].CountOfDetachableNonplayerUnits > 0)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						((CastleGateAI)base.Ai).ResetInitialGateState(this.State);
					}
				}
			}
		}

		protected override bool IsAgentOnInconvenientNavmesh(Agent agent, StandingPoint standingPoint)
		{
			if (Mission.Current.MissionTeamAIType != Mission.MissionTeamAITypeEnum.Siege)
			{
				return false;
			}
			int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
			TeamAISiegeComponent teamAISiegeComponent;
			if ((teamAISiegeComponent = agent.Team.TeamAI as TeamAISiegeComponent) != null && currentNavigationFaceId % 10 != 1)
			{
				if (base.GameEntity.HasTag("inner_gate"))
				{
					return true;
				}
				if (base.GameEntity.HasTag("outer_gate"))
				{
					CastleGate innerGate = teamAISiegeComponent.InnerGate;
					if (innerGate != null)
					{
						Vec3 vec = base.GameEntity.GlobalPosition - agent.Position;
						Vec3 vec2 = innerGate.GameEntity.GlobalPosition - agent.Position;
						if (vec.AsVec2.DotProduct(vec2.AsVec2) > 0f)
						{
							return true;
						}
					}
				}
				foreach (int num in (Mission.Current.DefenderTeam.TeamAI as TeamAISiegeDefender).DifficultNavmeshIDs)
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

		private void ServerTick(float dt)
		{
			if (!this.IsDeactivated)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					if (standingPoint.HasUser)
					{
						if (standingPoint.GameEntity.HasTag("open"))
						{
							this.OpenDoor();
							if (this.AutoOpen)
							{
								this.SetAutoOpenState(false);
							}
						}
						else
						{
							this.CloseDoor();
							if (Mission.Current.IsSallyOutBattle)
							{
								this.SetAutoOpenState(true);
							}
						}
					}
				}
				if (this.AutoOpen && this._pathChecker != null)
				{
					this._pathChecker.Tick(dt);
					if (this._pathChecker.HasAgentsUsingPath())
					{
						if (this.State != CastleGate.GateState.Open)
						{
							this.OpenDoor();
						}
					}
					else if (this.State != CastleGate.GateState.Closed)
					{
						this.CloseDoor();
					}
				}
				if (this._doorSkeleton != null && !base.IsDestroyed)
				{
					float animationParameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
					foreach (StandingPoint standingPoint2 in base.StandingPoints)
					{
						bool flag = animationParameterAtChannel < 1f || standingPoint2.GameEntity.HasTag((this.State == CastleGate.GateState.Open) ? "open" : "close");
						standingPoint2.SetIsDeactivatedSynched(flag);
					}
					if (animationParameterAtChannel >= 1f && this.State == CastleGate.GateState.Open)
					{
						if (this._extraColliderRight != null)
						{
							this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
							this._rightExtraColliderDisabled = true;
						}
						if (this._extraColliderLeft != null)
						{
							this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
							this._leftExtraColliderDisabled = true;
						}
					}
					if (this._plank != null && this.State == CastleGate.GateState.Closed && animationParameterAtChannel > 0.9f)
					{
						this._plank.SetVisibleSynched(true, false);
					}
				}
			}
		}

		public TargetFlags GetTargetFlags()
		{
			TargetFlags targetFlags = TargetFlags.None;
			targetFlags |= TargetFlags.IsStructure;
			if (base.IsDestroyed)
			{
				targetFlags |= TargetFlags.NotAThreat;
			}
			if (DebugSiegeBehavior.DebugAttackState == DebugSiegeBehavior.DebugStateAttacker.DebugAttackersToBattlements)
			{
				targetFlags |= TargetFlags.DebugThreat;
			}
			return targetFlags;
		}

		public float GetTargetValue(List<Vec3> weaponPos)
		{
			return 10f;
		}

		public GameEntity GetTargetEntity()
		{
			return base.GameEntity;
		}

		public BattleSideEnum GetSide()
		{
			return BattleSideEnum.Defender;
		}

		public GameEntity Entity()
		{
			return base.GameEntity;
		}

		protected void CollectGameEntities(bool calledFromOnInit)
		{
			this.CollectDynamicGameEntities(calledFromOnInit);
			if (!GameNetwork.IsClientOrReplay)
			{
				List<GameEntity> list = base.GameEntity.CollectChildrenEntitiesWithTag("plank");
				if (list.Count > 0)
				{
					this._plank = list.FirstOrDefault<GameEntity>().GetFirstScriptOfType<SynchedMissionObject>();
				}
			}
		}

		protected void OnNextDestructionState()
		{
			this.CollectDynamicGameEntities(false);
			this.UpdateDoorBodies(true);
		}

		protected void CollectDynamicGameEntities(bool calledFromOnInit)
		{
			this._attackOnlyDoorColliders.Clear();
			List<GameEntity> list;
			if (calledFromOnInit)
			{
				list = base.GameEntity.CollectChildrenEntitiesWithTag("gate").ToList<GameEntity>();
				this._leftExtraColliderDisabled = false;
				this._rightExtraColliderDisabled = false;
				this._agentColliderLeft = base.GameEntity.GetFirstChildEntityWithTag("collider_agent_l");
				this._agentColliderRight = base.GameEntity.GetFirstChildEntityWithTag("collider_agent_r");
			}
			else
			{
				list = (from x in base.GameEntity.CollectChildrenEntitiesWithTag("gate")
					where x.IsVisibleIncludeParents()
					select x).ToList<GameEntity>();
			}
			if (list.Count == 0)
			{
				return;
			}
			if (list.Count > 1)
			{
				int num = int.MinValue;
				int num2 = int.MaxValue;
				GameEntity gameEntity = null;
				GameEntity gameEntity2 = null;
				foreach (GameEntity gameEntity3 in list)
				{
					int num3 = int.Parse(gameEntity3.Tags.FirstOrDefault((string x) => x.Contains("state_")).Split(new char[] { '_' }).Last<string>());
					if (num3 > num)
					{
						num = num3;
						gameEntity = gameEntity3;
					}
					if (num3 < num2)
					{
						num2 = num3;
						gameEntity2 = gameEntity3;
					}
				}
				this._door = (calledFromOnInit ? gameEntity2.GetFirstScriptOfType<SynchedMissionObject>() : gameEntity.GetFirstScriptOfType<SynchedMissionObject>());
			}
			else
			{
				this._door = list[0].GetFirstScriptOfType<SynchedMissionObject>();
			}
			this._doorSkeleton = this._door.GameEntity.Skeleton;
			GameEntity gameEntity4 = this._door.GameEntity.CollectChildrenEntitiesWithTag("collider_r").FirstOrDefault<GameEntity>();
			if (gameEntity4 != null)
			{
				this._attackOnlyDoorColliders.Add(gameEntity4);
			}
			GameEntity gameEntity5 = this._door.GameEntity.CollectChildrenEntitiesWithTag("collider_l").FirstOrDefault<GameEntity>();
			if (gameEntity5 != null)
			{
				this._attackOnlyDoorColliders.Add(gameEntity5);
			}
			if (gameEntity4 == null || gameEntity5 == null)
			{
				GameEntity agentColliderLeft = this._agentColliderLeft;
				if (agentColliderLeft != null)
				{
					agentColliderLeft.SetVisibilityExcludeParents(false);
				}
				GameEntity agentColliderRight = this._agentColliderRight;
				if (agentColliderRight != null)
				{
					agentColliderRight.SetVisibilityExcludeParents(false);
				}
			}
			GameEntity gameEntity6 = this._door.GameEntity.CollectChildrenEntitiesWithTag(this.ExtraCollisionObjectTagLeft).FirstOrDefault<GameEntity>();
			if (gameEntity6 != null)
			{
				if (!this.ActivateExtraColliders)
				{
					gameEntity6.RemovePhysics(false);
				}
				else
				{
					if (!calledFromOnInit)
					{
						MatrixFrame matrixFrame = ((this._extraColliderLeft != null) ? this._extraColliderLeft.GetFrame() : this._doorSkeleton.GetBoneEntitialFrameWithName(this.LeftDoorBoneName));
						this._extraColliderLeft = gameEntity6;
						this._extraColliderLeft.SetFrame(ref matrixFrame);
					}
					else
					{
						this._extraColliderLeft = gameEntity6;
					}
					if (this._leftExtraColliderDisabled)
					{
						this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
					}
					else
					{
						this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag & ~BodyFlags.Disabled);
					}
				}
			}
			GameEntity gameEntity7 = this._door.GameEntity.CollectChildrenEntitiesWithTag(this.ExtraCollisionObjectTagRight).FirstOrDefault<GameEntity>();
			if (gameEntity7 != null)
			{
				if (!this.ActivateExtraColliders)
				{
					gameEntity7.RemovePhysics(false);
				}
				else
				{
					if (!calledFromOnInit)
					{
						MatrixFrame matrixFrame2 = ((this._extraColliderRight != null) ? this._extraColliderRight.GetFrame() : this._doorSkeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName));
						this._extraColliderRight = gameEntity7;
						this._extraColliderRight.SetFrame(ref matrixFrame2);
					}
					else
					{
						this._extraColliderRight = gameEntity7;
					}
					if (this._rightExtraColliderDisabled)
					{
						this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
					}
					else
					{
						this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag & ~BodyFlags.Disabled);
					}
				}
			}
			if (this._door != null && this._doorSkeleton != null)
			{
				this._leftDoorBoneIndex = Skeleton.GetBoneIndexFromName(this._doorSkeleton.GetName(), this.LeftDoorBoneName);
				this._rightDoorBoneIndex = Skeleton.GetBoneIndexFromName(this._doorSkeleton.GetName(), this.RightDoorBoneName);
			}
		}

		private void InitializeExtraColliderPositions()
		{
			if (this._extraColliderLeft != null)
			{
				MatrixFrame boneEntitialFrameWithName = this._doorSkeleton.GetBoneEntitialFrameWithName(this.LeftDoorBoneName);
				this._extraColliderLeft.SetFrame(ref boneEntitialFrameWithName);
				this._extraColliderLeft.SetVisibilityExcludeParents(true);
			}
			if (this._extraColliderRight != null)
			{
				MatrixFrame boneEntitialFrameWithName2 = this._doorSkeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName);
				this._extraColliderRight.SetFrame(ref boneEntitialFrameWithName2);
				this._extraColliderRight.SetVisibilityExcludeParents(true);
			}
			this.UpdateDoorBodies(true);
			foreach (GameEntity gameEntity in this._attackOnlyDoorColliders)
			{
				gameEntity.SetVisibilityExcludeParents(true);
			}
			if (this._agentColliderLeft != null)
			{
				this._agentColliderLeft.SetVisibilityExcludeParents(true);
			}
			if (this._agentColliderRight != null)
			{
				this._agentColliderRight.SetVisibilityExcludeParents(true);
			}
		}

		private void OnHitTaken(DestructableComponent hitComponent, Agent hitterAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			if (!GameNetwork.IsClientOrReplay && inflictedDamage >= 200 && this.State == CastleGate.GateState.Closed && attackerScriptComponentBehavior is BatteringRam)
			{
				SynchedMissionObject plank = this._plank;
				if (plank != null)
				{
					plank.SetAnimationAtChannelSynched(this.PlankHitAnimationName, 0, 1f);
				}
				this._door.SetAnimationAtChannelSynched(this.HitAnimationName, 0, 1f);
				Mission.Current.MakeSound(CastleGate.BatteringRamHitSoundIdCache, base.GameEntity.GlobalPosition, false, true, -1, -1);
			}
		}

		private void OnDestroyed(DestructableComponent destroyedComponent, Agent destroyerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				SynchedMissionObject plank = this._plank;
				if (plank != null)
				{
					plank.SetVisibleSynched(false, false);
				}
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					standingPoint.SetIsDeactivatedSynched(true);
				}
				if (attackerScriptComponentBehavior is BatteringRam)
				{
					this._door.SetAnimationAtChannelSynched(this.DestroyAnimationName, 0, 1f);
				}
				this.SetGateNavMeshState(true);
			}
		}

		private int OnCalculateDestructionStateIndex(int destructionStateIndex, int inflictedDamage, int destructionStateCount)
		{
			if (inflictedDamage < 200)
			{
				return destructionStateIndex;
			}
			return MathF.Min(destructionStateIndex, destructionStateCount - 1);
		}

		protected internal override bool OnCheckForProblems()
		{
			bool flag = base.OnCheckForProblems();
			if (base.GameEntity.HasTag("outer_gate") && base.GameEntity.HasTag("inner_gate"))
			{
				MBEditor.AddEntityWarning(base.GameEntity, "This castle gate has both outer and inner tag at the same time.");
				flag = true;
			}
			if (base.GameEntity.CollectChildrenEntitiesWithTag("wait_pos").Count != 1)
			{
				MBEditor.AddEntityWarning(base.GameEntity, "There must be one entity with wait position tag under castle gate.");
				flag = true;
			}
			if (base.GameEntity.HasTag("outer_gate"))
			{
				uint visibilityMask = base.GameEntity.GetVisibilityLevelMaskIncludingParents();
				GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.HasTag("middle_pos") && x.GetVisibilityLevelMaskIncludingParents() == visibilityMask);
				if (gameEntity != null)
				{
					GameEntity gameEntity2 = base.Scene.FindEntitiesWithTag("inner_gate").FirstOrDefault((GameEntity x) => x.GetVisibilityLevelMaskIncludingParents() == visibilityMask);
					if (gameEntity2 != null)
					{
						if (gameEntity2.HasScriptOfType<CastleGate>())
						{
							Vec2 vec = gameEntity2.GlobalPosition.AsVec2 - gameEntity.GlobalPosition.AsVec2;
							Vec2 vec2 = base.GameEntity.GlobalPosition.AsVec2 - gameEntity.GlobalPosition.AsVec2;
							if (Vec2.DotProduct(vec, vec2) <= 0f)
							{
								MBEditor.AddEntityWarning(base.GameEntity, "Outer gate's middle position must not be between outer and inner gate.");
								flag = true;
							}
						}
						else
						{
							MBEditor.AddEntityWarning(base.GameEntity, gameEntity2.Name + " this entity has inner gate tag but doesn't have castle gate script.");
							flag = true;
						}
					}
					else
					{
						MBEditor.AddEntityWarning(base.GameEntity, "There is no entity with inner gate tag.");
						flag = true;
					}
				}
				else
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Outer gate doesn't have any middle positions");
					flag = true;
				}
			}
			Vec3 scaleVector = base.GameEntity.GetGlobalFrame().rotation.GetScaleVector();
			if (MathF.Abs(scaleVector.x - scaleVector.y) > 1E-05f || MathF.Abs(scaleVector.x - scaleVector.z) > 1E-05f || MathF.Abs(scaleVector.y - scaleVector.z) > 1E-05f)
			{
				MBEditor.AddEntityWarning(base.GameEntity, "$$$ Non uniform scale on CastleGate at scene " + base.GameEntity.Scene.GetName());
				flag = true;
			}
			return flag;
		}

		public Vec3 GetTargetingOffset()
		{
			return Vec3.Zero;
		}

		public const string OuterGateTag = "outer_gate";

		public const string InnerGateTag = "inner_gate";

		private const float ExtraColliderScaleFactor = 1.1f;

		private const string LeftDoorBodyTag = "collider_l";

		private const string RightDoorBodyTag = "collider_r";

		private const string RightDoorAgentOnlyBodyTag = "collider_agent_r";

		private const string OpenTag = "open";

		private const string CloseTag = "close";

		private const string MiddlePositionTag = "middle_pos";

		private const string WaitPositionTag = "wait_pos";

		private const string LeftDoorAgentOnlyBodyTag = "collider_agent_l";

		private const int HeavyBlowDamageLimit = 200;

		private static int _batteringRamHitSoundId = -1;

		public CastleGate.DoorOwnership OwningTeam;

		public string OpeningAnimationName = "castle_gate_a_opening";

		public string ClosingAnimationName = "castle_gate_a_closing";

		public string HitAnimationName = "castle_gate_a_hit";

		public string PlankHitAnimationName = "castle_gate_a_plank_hit";

		public string HitMeleeAnimationName = "castle_gate_a_hit_melee";

		public string DestroyAnimationName = "castle_gate_a_break";

		public int NavigationMeshId = 1000;

		public int NavigationMeshIdToDisableOnOpen = -1;

		public string LeftDoorBoneName = "bn_bottom_l";

		public string RightDoorBoneName = "bn_bottom_r";

		public string ExtraCollisionObjectTagRight = "extra_collider_r";

		public string ExtraCollisionObjectTagLeft = "extra_collider_l";

		private int _openingAnimationIndex = -1;

		private int _closingAnimationIndex = -1;

		private bool _leftExtraColliderDisabled;

		private bool _rightExtraColliderDisabled;

		private bool _civilianMission;

		public bool ActivateExtraColliders = true;

		public string SideTag;

		private bool _openNavMeshIdDisabled;

		private SynchedMissionObject _door;

		private Skeleton _doorSkeleton;

		private GameEntity _extraColliderRight;

		private GameEntity _extraColliderLeft;

		private readonly List<GameEntity> _attackOnlyDoorColliders;

		private float _previousAnimationProgress = -1f;

		private GameEntity _agentColliderRight;

		private GameEntity _agentColliderLeft;

		private LadderQueueManager _queueManager;

		private bool _afterMissionStartTriggered;

		private sbyte _rightDoorBoneIndex;

		private sbyte _leftDoorBoneIndex;

		private AgentPathNavMeshChecker _pathChecker;

		public bool AutoOpen;

		private SynchedMissionObject _plank;

		private WorldFrame _middleFrame;

		private WorldFrame _defenseWaitFrame;

		private Action DestructibleComponentOnMissionReset;

		public enum DoorOwnership
		{
			Defenders,
			Attackers
		}

		public enum GateState
		{
			Open,
			Closed
		}
	}
}
