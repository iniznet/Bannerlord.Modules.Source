using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Objects;

namespace SandBox.Missions.AgentBehaviors
{
	public class FollowAgentBehavior : AgentBehavior
	{
		public FollowAgentBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._selectedAgent = null;
			this._deactivatedAgent = null;
			this._myLastStateWasRunning = false;
		}

		public void SetTargetAgent(Agent agent)
		{
			this._selectedAgent = agent;
			this._state = FollowAgentBehavior.State.Idle;
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("navigation_mesh_deactivator");
			if (gameEntity != null)
			{
				int disableFaceWithId = gameEntity.GetFirstScriptOfType<NavigationMeshDeactivator>().DisableFaceWithId;
				if (disableFaceWithId != -1)
				{
					base.OwnerAgent.SetAgentExcludeStateForFaceGroupId(disableFaceWithId, false);
				}
			}
			this.TryMoveStateTransition(true);
		}

		public override void Tick(float dt, bool isSimulation)
		{
			if (this._selectedAgent != null)
			{
				this.ControlMovement();
			}
		}

		private void ControlMovement()
		{
			if (base.Navigator.TargetPosition.IsValid && base.Navigator.IsTargetReached())
			{
				base.OwnerAgent.DisableScriptedMovement();
				base.OwnerAgent.SetMaximumSpeedLimit(-1f, false);
				if (this._state == FollowAgentBehavior.State.OnMove)
				{
					this._idleDistance = base.OwnerAgent.Position.AsVec2.Distance(this._selectedAgent.Position.AsVec2);
				}
				this._state = FollowAgentBehavior.State.Idle;
			}
			int nearbyEnemyAgentCount = base.Mission.GetNearbyEnemyAgentCount(base.OwnerAgent.Team, base.OwnerAgent.Position.AsVec2, 5f);
			if (this._state != FollowAgentBehavior.State.Fight && nearbyEnemyAgentCount > 0)
			{
				base.OwnerAgent.SetWatchState(2);
				base.OwnerAgent.ResetLookAgent();
				base.Navigator.ClearTarget();
				base.OwnerAgent.DisableScriptedMovement();
				this._state = FollowAgentBehavior.State.Fight;
				Debug.Print("[Follow agent behavior] Fight!", 0, 12, 17592186044416UL);
			}
			switch (this._state)
			{
			case FollowAgentBehavior.State.Idle:
				this.TryMoveStateTransition(false);
				return;
			case FollowAgentBehavior.State.OnMove:
				this.MoveToFollowingAgent(false);
				break;
			case FollowAgentBehavior.State.Fight:
				if (nearbyEnemyAgentCount == 0)
				{
					base.OwnerAgent.SetWatchState(0);
					base.OwnerAgent.SetLookAgent(this._selectedAgent);
					this._state = FollowAgentBehavior.State.Idle;
					Debug.Print("[Follow agent behavior] Stop fighting!", 0, 12, 17592186044416UL);
					return;
				}
				break;
			default:
				return;
			}
		}

		private void TryMoveStateTransition(bool forceMove)
		{
			if (this._selectedAgent != null && base.OwnerAgent.Position.AsVec2.Distance(this._selectedAgent.Position.AsVec2) > 4f + this._idleDistance)
			{
				this._state = FollowAgentBehavior.State.OnMove;
				this.MoveToFollowingAgent(forceMove);
			}
		}

		private void MoveToFollowingAgent(bool forcedMove)
		{
			Vec2 asVec = this._selectedAgent.Velocity.AsVec2;
			if (this._updatePositionThisFrame || forcedMove || asVec.IsNonZero())
			{
				this._updatePositionThisFrame = false;
				WorldPosition worldPosition = this._selectedAgent.GetWorldPosition();
				Vec2 vec = (asVec.IsNonZero() ? asVec.Normalized() : this._selectedAgent.GetMovementDirection());
				Vec2 vec2 = vec.LeftVec();
				Vec2 vec3 = this._selectedAgent.Position.AsVec2 - base.OwnerAgent.Position.AsVec2;
				float lengthSquared = vec3.LengthSquared;
				int num = ((Vec2.DotProduct(vec3, vec2) > 0f) ? 1 : (-1));
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				foreach (Agent agent in base.Mission.Agents)
				{
					CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
					if (((component != null) ? component.AgentNavigator : null) != null)
					{
						DailyBehaviorGroup behaviorGroup = component.AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
						FollowAgentBehavior followAgentBehavior = ((behaviorGroup != null) ? behaviorGroup.GetBehavior<FollowAgentBehavior>() : null);
						if (followAgentBehavior != null && followAgentBehavior._selectedAgent != null && followAgentBehavior._selectedAgent == this._selectedAgent)
						{
							Vec2 vec4 = this._selectedAgent.Position.AsVec2 - agent.Position.AsVec2;
							int num6 = ((Vec2.DotProduct(vec4, vec2) > 0f) ? 1 : (-1));
							if (vec4.LengthSquared < lengthSquared)
							{
								if (num6 == num)
								{
									if (agent.HasMount)
									{
										num3++;
									}
									else
									{
										num2++;
									}
								}
								if (Vec2.DotProduct(vec4, vec) > 0.3f)
								{
									if (agent.HasMount)
									{
										num5++;
									}
									else
									{
										num4++;
									}
								}
							}
						}
					}
				}
				float num7 = (this._selectedAgent.HasMount ? 1.25f : 0.6f);
				float num8 = (base.OwnerAgent.HasMount ? 1.25f : 0.6f);
				float num9 = (this._selectedAgent.HasMount ? 1.5f : 1f);
				float num10 = (base.OwnerAgent.HasMount ? 1.5f : 1f);
				Vec2 vec5 = vec * (2f + 0.5f * (num8 + num7) + (float)num2 * 0.6f + (float)num3 * 1.25f);
				Vec2 vec6 = (float)num * vec2 * (0.5f * (num10 + num9) + (float)num2 * 1f + (float)num3 * 1.5f);
				Vec2 vec7 = this._selectedAgent.Position.AsVec2 - vec5 - vec6;
				bool flag = false;
				AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, vec7, 0.5f, false);
				while (proximityMapSearchStruct.LastFoundAgent != null)
				{
					Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
					if (lastFoundAgent.Index != base.OwnerAgent.Index && lastFoundAgent.Index != this._selectedAgent.Index)
					{
						flag = true;
						break;
					}
					AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
				}
				float num11 = (base.OwnerAgent.HasMount ? 2.2f : 1.2f);
				if (!flag)
				{
					WorldPosition worldPosition2 = worldPosition;
					worldPosition2..ctor(base.Mission.Scene, UIntPtr.Zero, worldPosition2.GetGroundVec3(), false);
					worldPosition2.SetVec2(vec7);
					if (worldPosition2.GetNavMesh() != UIntPtr.Zero && base.Mission.Scene.IsLineToPointClear(ref worldPosition2, ref worldPosition, base.OwnerAgent.Monster.BodyCapsuleRadius))
					{
						WorldPosition worldPosition3 = worldPosition2;
						worldPosition3.SetVec2(worldPosition3.AsVec2 + vec * 1.5f);
						if (worldPosition3.GetNavMesh() != UIntPtr.Zero && base.Mission.Scene.IsLineToPointClear(ref worldPosition3, ref worldPosition2, base.OwnerAgent.Monster.BodyCapsuleRadius))
						{
							this.SetMovePos(worldPosition3, this._selectedAgent.MovementDirectionAsAngle, num11, 2);
						}
						else
						{
							this.SetMovePos(worldPosition2, this._selectedAgent.MovementDirectionAsAngle, num11, 2);
						}
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					float num12 = num11 + (float)num4 * 0.6f + (float)num5 * 1.25f;
					this.SetMovePos(worldPosition, this._selectedAgent.MovementDirectionAsAngle, num12, 2);
				}
			}
		}

		private void SetMovePos(WorldPosition pos, float rotationInRadians, float rangeThreshold, Agent.AIScriptedFrameFlags flags)
		{
			bool flag = base.Mission.Mode == 4;
			if (base.Navigator.CharacterHasVisiblePrefabs)
			{
				this._myLastStateWasRunning = false;
			}
			else
			{
				if (flag && this._selectedAgent.CrouchMode)
				{
					flags |= 512;
				}
				if (flag && this._selectedAgent.WalkMode)
				{
					base.OwnerAgent.SetMaximumSpeedLimit(this._selectedAgent.CrouchMode ? this._selectedAgent.Monster.CrouchWalkingSpeedLimit : this._selectedAgent.Monster.WalkingSpeedLimit, false);
					this._myLastStateWasRunning = false;
				}
				else
				{
					float num = base.OwnerAgent.Position.AsVec2.Distance(pos.AsVec2);
					if (num - rangeThreshold <= 0.5f * (this._myLastStateWasRunning ? 1f : 1.2f) && this._selectedAgent.Velocity.AsVec2.Length <= base.OwnerAgent.Monster.WalkingSpeedLimit * (this._myLastStateWasRunning ? 1f : 1.2f))
					{
						this._myLastStateWasRunning = false;
					}
					else
					{
						base.OwnerAgent.SetMaximumSpeedLimit(num - rangeThreshold + this._selectedAgent.Velocity.AsVec2.Length, false);
						this._myLastStateWasRunning = true;
					}
				}
			}
			if (!this._myLastStateWasRunning)
			{
				flags |= 16;
			}
			base.Navigator.SetTargetFrame(pos, rotationInRadians, rangeThreshold, -10f, flags, flag);
		}

		public override void OnAgentRemoved(Agent agent)
		{
			if (agent == this._selectedAgent)
			{
				base.OwnerAgent.ResetLookAgent();
				this._selectedAgent = null;
			}
		}

		protected override void OnActivate()
		{
			if (this._deactivatedAgent != null)
			{
				this.SetTargetAgent(this._deactivatedAgent);
				this._deactivatedAgent = null;
			}
		}

		protected override void OnDeactivate()
		{
			this._state = FollowAgentBehavior.State.Idle;
			this._deactivatedAgent = this._selectedAgent;
			this._selectedAgent = null;
			base.OwnerAgent.DisableScriptedMovement();
			base.OwnerAgent.ResetLookAgent();
			base.Navigator.ClearTarget();
		}

		public override string GetDebugInfo()
		{
			return string.Concat(new object[]
			{
				"Follow ",
				this._selectedAgent.Name,
				" (id:",
				this._selectedAgent.Index,
				")"
			});
		}

		public override float GetAvailability(bool isSimulation)
		{
			return (float)((this._selectedAgent == null) ? 0 : 100);
		}

		private const float _moveReactionProximityThreshold = 4f;

		private const float _longitudinalClearanceOffset = 2f;

		private const float _onFootMoveProximityThreshold = 1.2f;

		private const float _mountedMoveProximityThreshold = 2.2f;

		private const float _onFootAgentLongitudinalOffset = 0.6f;

		private const float _onFootAgentLateralOffset = 1f;

		private const float _mountedAgentLongitudinalOffset = 1.25f;

		private const float _mountedAgentLateralOffset = 1.5f;

		private float _idleDistance;

		private Agent _selectedAgent;

		private FollowAgentBehavior.State _state;

		private Agent _deactivatedAgent;

		private bool _myLastStateWasRunning;

		private bool _updatePositionThisFrame;

		private enum State
		{
			Idle,
			OnMove,
			Fight
		}
	}
}
