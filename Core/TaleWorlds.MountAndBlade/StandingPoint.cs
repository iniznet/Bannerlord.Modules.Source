using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class StandingPoint : UsableMissionObject
	{
		public virtual Agent.AIScriptedFrameFlags DisableScriptedFrameFlags
		{
			get
			{
				return Agent.AIScriptedFrameFlags.None;
			}
		}

		public override bool DisableCombatActionsOnUse
		{
			get
			{
				return false;
			}
		}

		[EditableScriptComponentVariable(false)]
		public Agent FavoredUser { get; set; }

		public virtual bool PlayerStopsUsingWhenInteractsWithOther
		{
			get
			{
				return true;
			}
		}

		public StandingPoint()
			: base(false)
		{
			this.AutoSheathWeapons = true;
			this.TranslateUser = true;
			this._autoAttachOnUsingStopped = true;
			this._needsSingleThreadTickOnce = false;
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this._cachedAgentDistances = new Dictionary<Agent, StandingPoint.AgentDistanceCache>();
			bool flag = base.GameEntity.HasTag("attacker");
			bool flag2 = base.GameEntity.HasTag("defender");
			if (flag && !flag2)
			{
				this.StandingPointSide = BattleSideEnum.Attacker;
			}
			else if (!flag && flag2)
			{
				this.StandingPointSide = BattleSideEnum.Defender;
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public void OnParentMachinePhysicsStateChanged()
		{
			base.GameEntityWithWorldPosition.InvalidateWorldPosition();
		}

		public override bool IsDisabledForAgent(Agent agent)
		{
			return base.IsDisabledForAgent(agent) || (this.StandingPointSide != BattleSideEnum.None && agent.IsAIControlled && agent.Team != null && agent.Team.Side != this.StandingPointSide);
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (!GameNetwork.IsClientOrReplay && base.HasUser)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel2;
			}
			return base.GetTickRequirement();
		}

		private void TickAux(bool isParallel)
		{
			if (!GameNetwork.IsClientOrReplay && base.HasUser)
			{
				if (!base.UserAgent.IsActive() || this.DoesActionTypeStopUsingGameObject(MBAnimation.GetActionType(base.UserAgent.GetCurrentAction(0))))
				{
					if (isParallel)
					{
						this._needsSingleThreadTickOnce = true;
						return;
					}
					Agent userAgent = base.UserAgent;
					Agent.StopUsingGameObjectFlags stopUsingGameObjectFlags = Agent.StopUsingGameObjectFlags.None;
					if (this._autoAttachOnUsingStopped)
					{
						stopUsingGameObjectFlags |= Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject;
					}
					userAgent.StopUsingGameObject(false, stopUsingGameObjectFlags);
					Action<Agent, bool> onUsingStoppedAction = this._onUsingStoppedAction;
					if (onUsingStoppedAction == null)
					{
						return;
					}
					onUsingStoppedAction(userAgent, true);
					return;
				}
				else if (this.AutoSheathWeapons)
				{
					if (base.UserAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
					{
						if (isParallel)
						{
							this._needsSingleThreadTickOnce = true;
						}
						else
						{
							base.UserAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.Instant);
						}
					}
					if (base.UserAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand) != EquipmentIndex.None)
					{
						if (isParallel)
						{
							this._needsSingleThreadTickOnce = true;
							return;
						}
						base.UserAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.Instant);
						return;
					}
				}
				else if (this.AutoWieldWeapons && base.UserAgent.Equipment.HasAnyWeapon() && base.UserAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) == EquipmentIndex.None && base.UserAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand) == EquipmentIndex.None)
				{
					if (isParallel)
					{
						this._needsSingleThreadTickOnce = true;
						return;
					}
					base.UserAgent.WieldInitialWeapons(Agent.WeaponWieldActionType.Instant);
				}
			}
		}

		protected internal override void OnTickParallel2(float dt)
		{
			base.OnTickParallel2(dt);
			this.TickAux(true);
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._needsSingleThreadTickOnce)
			{
				this._needsSingleThreadTickOnce = false;
				this.TickAux(false);
			}
		}

		protected virtual bool DoesActionTypeStopUsingGameObject(Agent.ActionCodeType actionType)
		{
			return actionType == Agent.ActionCodeType.Jump || actionType == Agent.ActionCodeType.Kick || actionType == Agent.ActionCodeType.WeaponBash;
		}

		public override void OnUse(Agent userAgent)
		{
			if (!this._autoAttachOnUsingStopped && this.MovingAgent != null)
			{
				Agent movingAgent = this.MovingAgent;
				movingAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
				Action<Agent, bool> onUsingStoppedAction = this._onUsingStoppedAction;
				if (onUsingStoppedAction != null)
				{
					onUsingStoppedAction(movingAgent, false);
				}
			}
			base.OnUse(userAgent);
			if (this.LockUserFrames)
			{
				WorldFrame userFrameForAgent = this.GetUserFrameForAgent(userAgent);
				userAgent.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
				return;
			}
			if (this.LockUserPositions)
			{
				userAgent.SetTargetPosition(this.GetUserFrameForAgent(userAgent).Origin.AsVec2);
			}
		}

		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			if (this.LockUserFrames || this.LockUserPositions)
			{
				userAgent.ClearTargetFrame();
			}
		}

		public override WorldFrame GetUserFrameForAgent(Agent agent)
		{
			if (!Mission.Current.IsTeleportingAgents && !this.TranslateUser)
			{
				return agent.GetWorldFrame();
			}
			if (!Mission.Current.IsTeleportingAgents && (this.LockUserFrames || this.LockUserPositions))
			{
				return base.GetUserFrameForAgent(agent);
			}
			WorldFrame userFrameForAgent = base.GetUserFrameForAgent(agent);
			MatrixFrame lookFrame = agent.LookFrame;
			Vec2 vec = (lookFrame.origin.AsVec2 - userFrameForAgent.Origin.AsVec2).Normalized();
			Vec2 vec2 = userFrameForAgent.Origin.AsVec2 + agent.GetInteractionDistanceToUsable(this) * 0.5f * vec;
			Mat3 rotation = lookFrame.rotation;
			userFrameForAgent.Origin.SetVec2(vec2);
			userFrameForAgent.Rotation = rotation;
			return userFrameForAgent;
		}

		public virtual bool HasAlternative()
		{
			return false;
		}

		public virtual float GetUsageScoreForAgent(Agent agent)
		{
			WorldPosition origin = this.GetUserFrameForAgent(agent).Origin;
			WorldPosition worldPosition = agent.GetWorldPosition();
			float pathDistance = this.GetPathDistance(agent, ref origin, ref worldPosition);
			float num = ((pathDistance < 0f) ? float.MinValue : (-pathDistance));
			if (agent == this.FavoredUser)
			{
				num *= 0.5f;
			}
			return num;
		}

		public virtual float GetUsageScoreForAgent(ValueTuple<Agent, float> agentPair)
		{
			float item = agentPair.Item2;
			float num = ((item < 0f) ? float.MinValue : (-item));
			if (agentPair.Item1 == this.FavoredUser)
			{
				num *= 0.5f;
			}
			return num;
		}

		public void SetupOnUsingStoppedBehavior(bool autoAttach, Action<Agent, bool> action)
		{
			this._autoAttachOnUsingStopped = autoAttach;
			this._onUsingStoppedAction = action;
		}

		private float GetPathDistance(Agent agent, ref WorldPosition userPosition, ref WorldPosition agentPosition)
		{
			StandingPoint.AgentDistanceCache agentDistanceCache;
			float num;
			if (this._cachedAgentDistances.TryGetValue(agent, out agentDistanceCache))
			{
				if (agentDistanceCache.AgentPosition.DistanceSquared(agentPosition.AsVec2) < 1f && agentDistanceCache.StandingPointPosition.DistanceSquared(userPosition.AsVec2) < 1f)
				{
					num = agentDistanceCache.PathDistance;
				}
				else
				{
					if (!Mission.Current.Scene.GetPathDistanceBetweenPositions(ref userPosition, ref agentPosition, agent.Monster.BodyCapsuleRadius, out num))
					{
						num = float.MaxValue;
					}
					agentDistanceCache = new StandingPoint.AgentDistanceCache
					{
						AgentPosition = agentPosition.AsVec2,
						StandingPointPosition = userPosition.AsVec2,
						PathDistance = num
					};
					this._cachedAgentDistances[agent] = agentDistanceCache;
				}
			}
			else
			{
				if (!Mission.Current.Scene.GetPathDistanceBetweenPositions(ref userPosition, ref agentPosition, agent.Monster.BodyCapsuleRadius, out num))
				{
					num = float.MaxValue;
				}
				agentDistanceCache = new StandingPoint.AgentDistanceCache
				{
					AgentPosition = agentPosition.AsVec2,
					StandingPointPosition = userPosition.AsVec2,
					PathDistance = num
				};
				this._cachedAgentDistances[agent] = agentDistanceCache;
			}
			return num;
		}

		public override void OnEndMission()
		{
			base.OnEndMission();
			this.FavoredUser = null;
		}

		protected internal virtual bool IsUsableBySide(BattleSideEnum side)
		{
			return !base.IsDeactivated && (base.IsInstantUse || !base.HasUser) && (this.StandingPointSide == BattleSideEnum.None || side == this.StandingPointSide);
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		public bool AutoSheathWeapons;

		public bool AutoEquipWeaponsOnUseStopped;

		private bool _autoAttachOnUsingStopped;

		private Action<Agent, bool> _onUsingStoppedAction;

		public bool AutoWieldWeapons;

		public readonly bool TranslateUser;

		public bool HasRecentlyBeenRechecked;

		private Dictionary<Agent, StandingPoint.AgentDistanceCache> _cachedAgentDistances;

		private bool _needsSingleThreadTickOnce;

		protected BattleSideEnum StandingPointSide = BattleSideEnum.None;

		public struct StackArray8StandingPoint
		{
			public StandingPoint this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					case 5:
						return this._element5;
					case 6:
						return this._element6;
					case 7:
						return this._element7;
					default:
						return null;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					case 5:
						this._element5 = value;
						return;
					case 6:
						this._element6 = value;
						return;
					case 7:
						this._element7 = value;
						return;
					default:
						return;
					}
				}
			}

			private StandingPoint _element0;

			private StandingPoint _element1;

			private StandingPoint _element2;

			private StandingPoint _element3;

			private StandingPoint _element4;

			private StandingPoint _element5;

			private StandingPoint _element6;

			private StandingPoint _element7;

			public const int Length = 8;
		}

		private struct AgentDistanceCache
		{
			public Vec2 AgentPosition;

			public Vec2 StandingPointPosition;

			public float PathDistance;
		}
	}
}
