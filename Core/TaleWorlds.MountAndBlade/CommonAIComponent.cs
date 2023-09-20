using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F5 RID: 245
	public class CommonAIComponent : AgentComponent
	{
		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06000C07 RID: 3079 RVA: 0x00016494 File Offset: 0x00014694
		// (set) Token: 0x06000C08 RID: 3080 RVA: 0x0001649C File Offset: 0x0001469C
		public bool IsPanicked { get; private set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06000C09 RID: 3081 RVA: 0x000164A5 File Offset: 0x000146A5
		// (set) Token: 0x06000C0A RID: 3082 RVA: 0x000164AD File Offset: 0x000146AD
		public bool IsRetreating { get; private set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06000C0B RID: 3083 RVA: 0x000164B6 File Offset: 0x000146B6
		// (set) Token: 0x06000C0C RID: 3084 RVA: 0x000164BE File Offset: 0x000146BE
		public int ReservedRiderAgentIndex { get; private set; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06000C0D RID: 3085 RVA: 0x000164C7 File Offset: 0x000146C7
		public float InitialMorale
		{
			get
			{
				return this._initialMorale;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06000C0E RID: 3086 RVA: 0x000164CF File Offset: 0x000146CF
		public float RecoveryMorale
		{
			get
			{
				return this._recoveryMorale;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06000C0F RID: 3087 RVA: 0x000164D7 File Offset: 0x000146D7
		// (set) Token: 0x06000C10 RID: 3088 RVA: 0x000164DF File Offset: 0x000146DF
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

		// Token: 0x06000C11 RID: 3089 RVA: 0x000164F8 File Offset: 0x000146F8
		public CommonAIComponent(Agent agent)
			: base(agent)
		{
			this._fadeOutTimer = new Timer(Mission.Current.CurrentTime, 0.5f + MBRandom.RandomFloat * 0.1f, true);
			float num = agent.Monster.BodyCapsuleRadius * 2f * 7.5f;
			this._retreatDistanceSquared = num * num;
			this.ReservedRiderAgentIndex = -1;
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x00016566 File Offset: 0x00014766
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeMorale();
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x00016574 File Offset: 0x00014774
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

		// Token: 0x06000C14 RID: 3092 RVA: 0x00016614 File Offset: 0x00014814
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

		// Token: 0x06000C15 RID: 3093 RVA: 0x0001683F File Offset: 0x00014A3F
		public void Panic()
		{
			if (this.IsPanicked)
			{
				return;
			}
			this.IsPanicked = true;
			this.Agent.Mission.OnAgentPanicked(this.Agent);
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x00016867 File Offset: 0x00014A67
		public void Retreat()
		{
			if (!this.IsRetreating)
			{
				this.IsRetreating = true;
				this.Agent.Retreat(this.Agent.Mission.GetClosestFleePositionForAgent(this.Agent));
			}
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0001689C File Offset: 0x00014A9C
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

		// Token: 0x06000C18 RID: 3096 RVA: 0x000168E8 File Offset: 0x00014AE8
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

		// Token: 0x06000C19 RID: 3097 RVA: 0x00016967 File Offset: 0x00014B67
		public override void OnHit(Agent affectorAgent, int damage, in MissionWeapon affectorWeapon)
		{
			base.OnHit(affectorAgent, damage, affectorWeapon);
			if (damage >= 1 && this.Agent.IsMount && this.Agent.IsAIControlled && this.Agent.RiderAgent == null)
			{
				this.Panic();
			}
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x000169A4 File Offset: 0x00014BA4
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

		// Token: 0x06000C1B RID: 3099 RVA: 0x00016A3C File Offset: 0x00014C3C
		internal int OnMountSelectionForRiderUpdate(int index)
		{
			int reservedRiderAgentIndex = this.ReservedRiderAgentIndex;
			this.ReservedRiderAgentIndex = index;
			return reservedRiderAgentIndex;
		}

		// Token: 0x040002B0 RID: 688
		private const float MoraleThresholdForPanicking = 0.01f;

		// Token: 0x040002B1 RID: 689
		private const float MaxRecoverableMoraleMultiplier = 0.5f;

		// Token: 0x040002B2 RID: 690
		private const float MoraleRecoveryPerSecond = 0.4f;

		// Token: 0x040002B6 RID: 694
		private float _recoveryMorale;

		// Token: 0x040002B7 RID: 695
		private float _initialMorale;

		// Token: 0x040002B8 RID: 696
		private float _morale = 50f;

		// Token: 0x040002B9 RID: 697
		private readonly Timer _fadeOutTimer;

		// Token: 0x040002BA RID: 698
		private readonly float _retreatDistanceSquared;
	}
}
