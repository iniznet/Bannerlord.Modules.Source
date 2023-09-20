using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public abstract class BehaviorComponent
	{
		public Formation Formation { get; private set; }

		public float BehaviorCoherence { get; set; }

		protected BehaviorComponent(Formation formation)
		{
			this.Formation = formation;
			this.PreserveExpireTime = 0f;
			this._navmeshlessTargetPenaltyTime = new Timer(Mission.Current.CurrentTime, 50f, true);
		}

		protected BehaviorComponent()
		{
		}

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

		protected virtual void OnBehaviorActivatedAux()
		{
		}

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

		public virtual void OnBehaviorCanceled()
		{
		}

		public void RemindSergeantPlayer()
		{
			float currentTime = Mission.Current.CurrentTime;
			if (this == this.Formation.AI.ActiveBehavior && this._lastPlayerInformTime + 60f < currentTime)
			{
				this.InformSergeantPlayer();
				this._lastPlayerInformTime = currentTime;
			}
		}

		public virtual void TickOccasionally()
		{
		}

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

		public float GetAIWeight()
		{
			return this.GetAiWeight() * this.NavmeshlessTargetPositionPenalty;
		}

		protected abstract float GetAiWeight();

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

		public float PreserveExpireTime { get; set; }

		public float WeightFactor { get; set; }

		public virtual void ResetBehavior()
		{
			this.WeightFactor = 0f;
		}

		public virtual TextObject GetBehaviorString()
		{
			string name = base.GetType().Name;
			return GameTexts.FindText("str_formation_ai_sergeant_instruction_behavior_text", name);
		}

		public virtual void OnValidBehaviorSideChanged()
		{
			this._behaviorSide = this.Formation.AI.Side;
		}

		protected virtual void CalculateCurrentOrder()
		{
		}

		public void PrecalculateMovementOrder()
		{
			this.CalculateCurrentOrder();
			this.CurrentOrder.GetPosition(this.Formation);
		}

		public override bool Equals(object obj)
		{
			return base.GetType() == obj.GetType();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected FormationAI.BehaviorSide _behaviorSide;

		protected const float FormArrangementDistanceToOrderPosition = 10f;

		private const float _playerInformCooldown = 60f;

		protected float _lastPlayerInformTime;

		private Timer _navmeshlessTargetPenaltyTime;

		private float _navmeshlessTargetPositionPenalty = 1f;

		public bool IsCurrentOrderChanged;

		private MovementOrder _currentOrder;

		protected FacingOrder CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
	}
}
