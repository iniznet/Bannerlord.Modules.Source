using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000360 RID: 864
	public class StandingPoint : UsableMissionObject
	{
		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x06002F2F RID: 12079 RVA: 0x000C08A8 File Offset: 0x000BEAA8
		public virtual Agent.AIScriptedFrameFlags DisableScriptedFrameFlags
		{
			get
			{
				return Agent.AIScriptedFrameFlags.None;
			}
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06002F30 RID: 12080 RVA: 0x000C08AB File Offset: 0x000BEAAB
		public override bool DisableCombatActionsOnUse
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x06002F31 RID: 12081 RVA: 0x000C08AE File Offset: 0x000BEAAE
		// (set) Token: 0x06002F32 RID: 12082 RVA: 0x000C08B6 File Offset: 0x000BEAB6
		[EditableScriptComponentVariable(false)]
		public Agent FavoredUser { get; set; }

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x06002F33 RID: 12083 RVA: 0x000C08BF File Offset: 0x000BEABF
		public virtual bool PlayerStopsUsingWhenInteractsWithOther
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002F34 RID: 12084 RVA: 0x000C08C2 File Offset: 0x000BEAC2
		public StandingPoint()
			: base(false)
		{
			this.AutoSheathWeapons = true;
			this.TranslateUser = true;
			this._autoAttachOnUsingStopped = true;
			this._needsSingleThreadTickOnce = false;
		}

		// Token: 0x06002F35 RID: 12085 RVA: 0x000C08F0 File Offset: 0x000BEAF0
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

		// Token: 0x06002F36 RID: 12086 RVA: 0x000C095A File Offset: 0x000BEB5A
		public void OnParentMachinePhysicsStateChanged()
		{
			base.GameEntityWithWorldPosition.InvalidateWorldPosition();
		}

		// Token: 0x06002F37 RID: 12087 RVA: 0x000C0967 File Offset: 0x000BEB67
		public override bool IsDisabledForAgent(Agent agent)
		{
			return base.IsDisabledForAgent(agent) || (this.StandingPointSide != BattleSideEnum.None && agent.IsAIControlled && agent.Team != null && agent.Team.Side != this.StandingPointSide);
		}

		// Token: 0x06002F38 RID: 12088 RVA: 0x000C09A5 File Offset: 0x000BEBA5
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (!GameNetwork.IsClientOrReplay && base.HasUser)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel2;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002F39 RID: 12089 RVA: 0x000C09C8 File Offset: 0x000BEBC8
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

		// Token: 0x06002F3A RID: 12090 RVA: 0x000C0AF2 File Offset: 0x000BECF2
		protected internal override void OnTickParallel2(float dt)
		{
			base.OnTickParallel2(dt);
			this.TickAux(true);
		}

		// Token: 0x06002F3B RID: 12091 RVA: 0x000C0B02 File Offset: 0x000BED02
		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._needsSingleThreadTickOnce)
			{
				this._needsSingleThreadTickOnce = false;
				this.TickAux(false);
			}
		}

		// Token: 0x06002F3C RID: 12092 RVA: 0x000C0B21 File Offset: 0x000BED21
		protected virtual bool DoesActionTypeStopUsingGameObject(Agent.ActionCodeType actionType)
		{
			return actionType == Agent.ActionCodeType.Jump || actionType == Agent.ActionCodeType.Kick || actionType == Agent.ActionCodeType.WeaponBash;
		}

		// Token: 0x06002F3D RID: 12093 RVA: 0x000C0B34 File Offset: 0x000BED34
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

		// Token: 0x06002F3E RID: 12094 RVA: 0x000C0BCA File Offset: 0x000BEDCA
		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			if (this.LockUserFrames || this.LockUserPositions)
			{
				userAgent.ClearTargetFrame();
			}
		}

		// Token: 0x06002F3F RID: 12095 RVA: 0x000C0BEC File Offset: 0x000BEDEC
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

		// Token: 0x06002F40 RID: 12096 RVA: 0x000C0CB2 File Offset: 0x000BEEB2
		public virtual bool HasAlternative()
		{
			return false;
		}

		// Token: 0x06002F41 RID: 12097 RVA: 0x000C0CB8 File Offset: 0x000BEEB8
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

		// Token: 0x06002F42 RID: 12098 RVA: 0x000C0D0C File Offset: 0x000BEF0C
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

		// Token: 0x06002F43 RID: 12099 RVA: 0x000C0D49 File Offset: 0x000BEF49
		public void SetupOnUsingStoppedBehavior(bool autoAttach, Action<Agent, bool> action)
		{
			this._autoAttachOnUsingStopped = autoAttach;
			this._onUsingStoppedAction = action;
		}

		// Token: 0x06002F44 RID: 12100 RVA: 0x000C0D5C File Offset: 0x000BEF5C
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

		// Token: 0x06002F45 RID: 12101 RVA: 0x000C0E7B File Offset: 0x000BF07B
		public override void OnEndMission()
		{
			base.OnEndMission();
			this.FavoredUser = null;
		}

		// Token: 0x06002F46 RID: 12102 RVA: 0x000C0E8A File Offset: 0x000BF08A
		protected internal virtual bool IsUsableBySide(BattleSideEnum side)
		{
			return !base.IsDeactivated && (base.IsInstantUse || !base.HasUser) && (this.StandingPointSide == BattleSideEnum.None || side == this.StandingPointSide);
		}

		// Token: 0x06002F47 RID: 12103 RVA: 0x000C0EBA File Offset: 0x000BF0BA
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return string.Empty;
		}

		// Token: 0x04001359 RID: 4953
		public bool AutoSheathWeapons;

		// Token: 0x0400135A RID: 4954
		public bool AutoEquipWeaponsOnUseStopped;

		// Token: 0x0400135B RID: 4955
		private bool _autoAttachOnUsingStopped;

		// Token: 0x0400135C RID: 4956
		private Action<Agent, bool> _onUsingStoppedAction;

		// Token: 0x0400135D RID: 4957
		public bool AutoWieldWeapons;

		// Token: 0x0400135E RID: 4958
		public readonly bool TranslateUser;

		// Token: 0x0400135F RID: 4959
		public bool HasRecentlyBeenRechecked;

		// Token: 0x04001361 RID: 4961
		private Dictionary<Agent, StandingPoint.AgentDistanceCache> _cachedAgentDistances;

		// Token: 0x04001362 RID: 4962
		private bool _needsSingleThreadTickOnce;

		// Token: 0x04001363 RID: 4963
		protected BattleSideEnum StandingPointSide = BattleSideEnum.None;

		// Token: 0x02000679 RID: 1657
		public struct StackArray8StandingPoint
		{
			// Token: 0x170009E2 RID: 2530
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

			// Token: 0x040020FD RID: 8445
			private StandingPoint _element0;

			// Token: 0x040020FE RID: 8446
			private StandingPoint _element1;

			// Token: 0x040020FF RID: 8447
			private StandingPoint _element2;

			// Token: 0x04002100 RID: 8448
			private StandingPoint _element3;

			// Token: 0x04002101 RID: 8449
			private StandingPoint _element4;

			// Token: 0x04002102 RID: 8450
			private StandingPoint _element5;

			// Token: 0x04002103 RID: 8451
			private StandingPoint _element6;

			// Token: 0x04002104 RID: 8452
			private StandingPoint _element7;

			// Token: 0x04002105 RID: 8453
			public const int Length = 8;
		}

		// Token: 0x0200067A RID: 1658
		private struct AgentDistanceCache
		{
			// Token: 0x04002106 RID: 8454
			public Vec2 AgentPosition;

			// Token: 0x04002107 RID: 8455
			public Vec2 StandingPointPosition;

			// Token: 0x04002108 RID: 8456
			public float PathDistance;
		}
	}
}
