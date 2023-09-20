using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class CommonAIComponent : AgentComponent
	{
		public bool IsPanicked { get; private set; }

		public bool IsRetreating { get; private set; }

		public int ReservedRiderAgentIndex { get; private set; }

		public float InitialMorale
		{
			get
			{
				return this._initialMorale;
			}
		}

		public float RecoveryMorale
		{
			get
			{
				return this._recoveryMorale;
			}
		}

		public float Morale
		{
			get
			{
				return this._morale;
			}
			set
			{
				this._morale = MBMath.ClampFloat(value, 0f, 100f);
			}
		}

		public CommonAIComponent(Agent agent)
			: base(agent)
		{
			this._fadeOutTimer = new Timer(Mission.Current.CurrentTime, 0.5f + MBRandom.RandomFloat * 0.1f, true);
			float num = agent.Monster.BodyCapsuleRadius * 2f * 7.5f;
			this._retreatDistanceSquared = num * num;
			this.ReservedRiderAgentIndex = -1;
		}

		public override void Initialize()
		{
			base.Initialize();
			this.InitializeMorale();
		}

		private void InitializeMorale()
		{
			int num = MBRandom.RandomInt(30);
			float num2 = this.Agent.Components.Sum((AgentComponent c) => c.GetMoraleAddition());
			float num3 = 35f + (float)num + num2;
			num3 = MissionGameModels.Current.BattleMoraleModel.GetEffectiveInitialMorale(this.Agent, num3);
			num3 = MBMath.ClampFloat(num3, 15f, 100f);
			this._initialMorale = num3;
			this._recoveryMorale = this._initialMorale * 0.5f;
			this.Morale = this._initialMorale;
		}

		public override void OnTickAsAI(float dt)
		{
			base.OnTickAsAI(dt);
			if (!this.IsRetreating && this._morale < 0.01f)
			{
				if (this.CanPanic())
				{
					this.Panic();
				}
				else
				{
					this.Morale = 0.01f;
				}
			}
			if (!this.IsPanicked && this._morale < this._recoveryMorale)
			{
				this.Morale = Math.Min(this._morale + 0.4f * dt, this._recoveryMorale);
			}
			if (Mission.Current.CanAgentRout(this.Agent) && this._fadeOutTimer.Check(Mission.Current.CurrentTime) && !this.Agent.IsFadingOut())
			{
				Vec3 position = this.Agent.Position;
				WorldPosition retreatPos = this.Agent.GetRetreatPos();
				if ((retreatPos.AsVec2.IsValid && retreatPos.AsVec2.DistanceSquared(position.AsVec2) < this._retreatDistanceSquared && retreatPos.GetGroundVec3().DistanceSquared(position) < this._retreatDistanceSquared) || !this.Agent.Mission.IsPositionInsideBoundaries(position.AsVec2) || position.DistanceSquared(this.Agent.Mission.GetClosestBoundaryPosition(position.AsVec2).ToVec3(0f)) < this._retreatDistanceSquared)
				{
					this.Agent.StartFadingOut();
				}
			}
			if (this.IsPanicked && this.Agent.Mission.MissionEnded)
			{
				MissionResult missionResult = this.Agent.Mission.MissionResult;
				if (this.Agent.Team != null && missionResult != null && ((missionResult.PlayerVictory && (this.Agent.Team.IsPlayerTeam || this.Agent.Team.IsPlayerAlly)) || (missionResult.PlayerDefeated && !this.Agent.Team.IsPlayerTeam && !this.Agent.Team.IsPlayerAlly)) && this.Agent != Agent.Main && this.Agent.IsActive())
				{
					this.StopRetreating();
				}
			}
		}

		public void Panic()
		{
			if (this.IsPanicked)
			{
				return;
			}
			this.IsPanicked = true;
			this.Agent.Mission.OnAgentPanicked(this.Agent);
		}

		public void Retreat()
		{
			if (!this.IsRetreating)
			{
				this.IsRetreating = true;
				this.Agent.Retreat(this.Agent.Mission.GetClosestFleePositionForAgent(this.Agent));
			}
		}

		public void StopRetreating()
		{
			if (!this.IsRetreating)
			{
				return;
			}
			this.IsRetreating = false;
			this.IsPanicked = false;
			float num = MathF.Max(0.02f, this.Morale);
			this.Agent.SetMorale(num);
			this.Agent.StopRetreating();
		}

		public bool CanPanic()
		{
			if (!MissionGameModels.Current.BattleMoraleModel.CanPanicDueToMorale(this.Agent))
			{
				return false;
			}
			TeamAISiegeComponent teamAISiegeComponent;
			if (Mission.Current.IsSiegeBattle && this.Agent.Team.Side == BattleSideEnum.Attacker && (teamAISiegeComponent = this.Agent.Team.TeamAI as TeamAISiegeComponent) != null)
			{
				int currentNavigationFaceId = this.Agent.GetCurrentNavigationFaceId();
				if (currentNavigationFaceId % 10 == 1)
				{
					return false;
				}
				if (teamAISiegeComponent.IsPrimarySiegeWeaponNavmeshFaceId(currentNavigationFaceId))
				{
					return false;
				}
			}
			return true;
		}

		public override void OnHit(Agent affectorAgent, int damage, in MissionWeapon affectorWeapon)
		{
			base.OnHit(affectorAgent, damage, affectorWeapon);
			if (damage >= 1 && this.Agent.IsMount && this.Agent.IsAIControlled && this.Agent.RiderAgent == null)
			{
				this.Panic();
			}
		}

		public override void OnAgentRemoved()
		{
			base.OnAgentRemoved();
			if (this.Agent.IsMount && this.Agent.RiderAgent == null && this.ReservedRiderAgentIndex >= 0)
			{
				foreach (Agent agent in Mission.Current.Agents)
				{
					if (agent.Index == this.ReservedRiderAgentIndex)
					{
						agent.SetSelectedMountIndex(-1);
						break;
					}
				}
				this.ReservedRiderAgentIndex = -1;
			}
		}

		internal int OnMountSelectionForRiderUpdate(int index)
		{
			int reservedRiderAgentIndex = this.ReservedRiderAgentIndex;
			this.ReservedRiderAgentIndex = index;
			return reservedRiderAgentIndex;
		}

		private const float MoraleThresholdForPanicking = 0.01f;

		private const float MaxRecoverableMoraleMultiplier = 0.5f;

		private const float MoraleRecoveryPerSecond = 0.4f;

		private float _recoveryMorale;

		private float _initialMorale;

		private float _morale = 50f;

		private readonly Timer _fadeOutTimer;

		private readonly float _retreatDistanceSquared;
	}
}
