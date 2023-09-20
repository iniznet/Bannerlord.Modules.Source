using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000FE RID: 254
	public abstract class BehaviorComponent
	{
		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06000C80 RID: 3200 RVA: 0x0001AA7B File Offset: 0x00018C7B
		// (set) Token: 0x06000C81 RID: 3201 RVA: 0x0001AA83 File Offset: 0x00018C83
		public Formation Formation { get; private set; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x0001AA8C File Offset: 0x00018C8C
		// (set) Token: 0x06000C83 RID: 3203 RVA: 0x0001AA94 File Offset: 0x00018C94
		public float BehaviorCoherence { get; set; }

		// Token: 0x06000C84 RID: 3204 RVA: 0x0001AAA0 File Offset: 0x00018CA0
		protected BehaviorComponent(Formation formation)
		{
			this.Formation = formation;
			this.PreserveExpireTime = 0f;
			this._navmeshlessTargetPenaltyTime = new Timer(Mission.Current.CurrentTime, 50f, true);
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0001AAF6 File Offset: 0x00018CF6
		protected BehaviorComponent()
		{
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x0001AB14 File Offset: 0x00018D14
		private void InformSergeantPlayer()
		{
			if (Mission.Current.MainAgent != null && this.Formation.Team.GeneralAgent != null && !this.Formation.Team.IsPlayerGeneral && this.Formation.Team.IsPlayerSergeant && this.Formation.PlayerOwner == Agent.Main)
			{
				TextObject behaviorString = this.GetBehaviorString();
				MBTextManager.SetTextVariable("BEHAVIOR", behaviorString, false);
				MBTextManager.SetTextVariable("PLAYER_NAME", Mission.Current.MainAgent.Name, false);
				MBTextManager.SetTextVariable("TEAM_LEADER", this.Formation.Team.GeneralAgent.Name, false);
				MBInformationManager.AddQuickInformation(new TextObject("{=L91XKoMD}{TEAM_LEADER}: {PLAYER_NAME}, {BEHAVIOR}", null), 4000, this.Formation.Team.GeneralAgent.Character, "");
			}
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x0001ABFD File Offset: 0x00018DFD
		protected virtual void OnBehaviorActivatedAux()
		{
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0001AC00 File Offset: 0x00018E00
		internal void OnBehaviorActivated()
		{
			if (!this.Formation.Team.IsPlayerGeneral && !this.Formation.Team.IsPlayerSergeant && this.Formation.IsPlayerTroopInFormation && Mission.Current.MainAgent != null)
			{
				TextObject textObject = new TextObject(this.ToString().Replace("MBModule.Behavior", ""), null);
				MBTextManager.SetTextVariable("BEHAVIOUR_NAME_BEGIN", textObject, false);
				textObject = GameTexts.FindText("str_formation_ai_soldier_instruction_text", null);
				MBInformationManager.AddQuickInformation(textObject, 2000, Mission.Current.MainAgent.Character, "");
			}
			if (Game.Current.GameType != MultiplayerGame.Current)
			{
				this.InformSergeantPlayer();
				this._lastPlayerInformTime = Mission.Current.CurrentTime;
			}
			if (this.Formation.IsAIControlled)
			{
				this.OnBehaviorActivatedAux();
			}
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0001ACD9 File Offset: 0x00018ED9
		public virtual void OnBehaviorCanceled()
		{
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x0001ACDC File Offset: 0x00018EDC
		public void RemindSergeantPlayer()
		{
			float currentTime = Mission.Current.CurrentTime;
			if (this == this.Formation.AI.ActiveBehavior && this._lastPlayerInformTime + 60f < currentTime)
			{
				this.InformSergeantPlayer();
				this._lastPlayerInformTime = currentTime;
			}
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x0001AD23 File Offset: 0x00018F23
		public virtual void TickOccasionally()
		{
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x0001AD28 File Offset: 0x00018F28
		// (set) Token: 0x06000C8D RID: 3213 RVA: 0x0001ADB4 File Offset: 0x00018FB4
		public virtual float NavmeshlessTargetPositionPenalty
		{
			get
			{
				if (this._navmeshlessTargetPositionPenalty == 1f)
				{
					return 1f;
				}
				this._navmeshlessTargetPenaltyTime.Check(Mission.Current.CurrentTime);
				float num = this._navmeshlessTargetPenaltyTime.ElapsedTime();
				if (num >= 10f)
				{
					this._navmeshlessTargetPositionPenalty = 1f;
					return 1f;
				}
				if (num <= 5f)
				{
					return this._navmeshlessTargetPositionPenalty;
				}
				return MBMath.Lerp(this._navmeshlessTargetPositionPenalty, 1f, (num - 5f) / 5f, 1E-05f);
			}
			set
			{
				this._navmeshlessTargetPenaltyTime.Reset(Mission.Current.CurrentTime);
				this._navmeshlessTargetPositionPenalty = value;
			}
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x0001ADD2 File Offset: 0x00018FD2
		public float GetAIWeight()
		{
			return this.GetAiWeight() * this.NavmeshlessTargetPositionPenalty;
		}

		// Token: 0x06000C8F RID: 3215
		protected abstract float GetAiWeight();

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0001ADE1 File Offset: 0x00018FE1
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x0001ADE9 File Offset: 0x00018FE9
		public MovementOrder CurrentOrder
		{
			get
			{
				return this._currentOrder;
			}
			protected set
			{
				this._currentOrder = value;
				this.IsCurrentOrderChanged = true;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x0001ADF9 File Offset: 0x00018FF9
		// (set) Token: 0x06000C93 RID: 3219 RVA: 0x0001AE01 File Offset: 0x00019001
		public float PreserveExpireTime { get; set; }

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06000C94 RID: 3220 RVA: 0x0001AE0A File Offset: 0x0001900A
		// (set) Token: 0x06000C95 RID: 3221 RVA: 0x0001AE12 File Offset: 0x00019012
		public float WeightFactor { get; set; }

		// Token: 0x06000C96 RID: 3222 RVA: 0x0001AE1B File Offset: 0x0001901B
		public virtual void ResetBehavior()
		{
			this.WeightFactor = 0f;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x0001AE28 File Offset: 0x00019028
		public virtual TextObject GetBehaviorString()
		{
			string name = base.GetType().Name;
			return GameTexts.FindText("str_formation_ai_sergeant_instruction_behavior_text", name);
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x0001AE4C File Offset: 0x0001904C
		public virtual void OnValidBehaviorSideChanged()
		{
			this._behaviorSide = this.Formation.AI.Side;
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x0001AE64 File Offset: 0x00019064
		protected virtual void CalculateCurrentOrder()
		{
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0001AE68 File Offset: 0x00019068
		public void PrecalculateMovementOrder()
		{
			this.CalculateCurrentOrder();
			this.CurrentOrder.GetPosition(this.Formation);
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0001AE90 File Offset: 0x00019090
		public override bool Equals(object obj)
		{
			return base.GetType() == obj.GetType();
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0001AEA3 File Offset: 0x000190A3
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x040002EE RID: 750
		protected FormationAI.BehaviorSide _behaviorSide;

		// Token: 0x040002EF RID: 751
		protected const float FormArrangementDistanceToOrderPosition = 10f;

		// Token: 0x040002F0 RID: 752
		private const float _playerInformCooldown = 60f;

		// Token: 0x040002F1 RID: 753
		protected float _lastPlayerInformTime;

		// Token: 0x040002F2 RID: 754
		private Timer _navmeshlessTargetPenaltyTime;

		// Token: 0x040002F3 RID: 755
		private float _navmeshlessTargetPositionPenalty = 1f;

		// Token: 0x040002F4 RID: 756
		public bool IsCurrentOrderChanged;

		// Token: 0x040002F5 RID: 757
		private MovementOrder _currentOrder;

		// Token: 0x040002F6 RID: 758
		protected FacingOrder CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
	}
}
