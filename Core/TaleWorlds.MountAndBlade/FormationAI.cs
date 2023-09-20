using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class FormationAI
	{
		public event Action<Formation> OnActiveBehaviorChanged;

		public BehaviorComponent ActiveBehavior
		{
			get
			{
				return this._activeBehavior;
			}
			private set
			{
				if (this._activeBehavior != value)
				{
					BehaviorComponent activeBehavior = this._activeBehavior;
					if (activeBehavior != null)
					{
						activeBehavior.OnBehaviorCanceled();
					}
					BehaviorComponent activeBehavior2 = this._activeBehavior;
					this._activeBehavior = value;
					this._activeBehavior.OnBehaviorActivated();
					this.ActiveBehavior.PreserveExpireTime = Mission.Current.CurrentTime + 10f;
					if (this.OnActiveBehaviorChanged != null && (activeBehavior2 == null || !activeBehavior2.Equals(value)))
					{
						this.OnActiveBehaviorChanged(this._formation);
					}
				}
			}
		}

		public FormationAI.BehaviorSide Side
		{
			get
			{
				return this._side;
			}
			set
			{
				if (this._side != value)
				{
					this._side = value;
					if (this._side != FormationAI.BehaviorSide.BehaviorSideNotSet)
					{
						foreach (BehaviorComponent behaviorComponent in this._behaviors)
						{
							behaviorComponent.OnValidBehaviorSideChanged();
						}
					}
				}
			}
		}

		public bool IsMainFormation { get; set; }

		public FormationAI(Formation formation)
		{
			this._formation = formation;
			float num = 0f;
			if (formation.Team != null)
			{
				float num2 = 0.1f * (float)formation.FormationIndex;
				float num3 = 0f;
				if (formation.Team.TeamIndex >= 0)
				{
					num3 = (float)formation.Team.TeamIndex * 0.5f * 0.1f;
				}
				num = num2 + num3;
			}
			this._tickTimer = new Timer(Mission.Current.CurrentTime + 0.5f * num, 0.5f, true);
			this._specialBehaviorData = new List<FormationAI.BehaviorData>();
		}

		public T SetBehaviorWeight<T>(float w) where T : BehaviorComponent
		{
			using (List<BehaviorComponent>.Enumerator enumerator = this._behaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t;
					if ((t = enumerator.Current as T) != null)
					{
						t.WeightFactor = w;
						return t;
					}
				}
			}
			throw new MBException("Behavior weight could not be set.");
		}

		public void AddAiBehavior(BehaviorComponent behaviorComponent)
		{
			this._behaviors.Add(behaviorComponent);
		}

		public T GetBehavior<T>() where T : BehaviorComponent
		{
			using (List<BehaviorComponent>.Enumerator enumerator = this._behaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t;
					if ((t = enumerator.Current as T) != null)
					{
						return t;
					}
				}
			}
			using (List<FormationAI.BehaviorData>.Enumerator enumerator2 = this._specialBehaviorData.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					T t2;
					if ((t2 = enumerator2.Current.Behavior as T) != null)
					{
						return t2;
					}
				}
			}
			return default(T);
		}

		public void AddSpecialBehavior(BehaviorComponent behavior, bool purgePreviousSpecialBehaviors = false)
		{
			if (purgePreviousSpecialBehaviors)
			{
				this._specialBehaviorData.Clear();
			}
			this._specialBehaviorData.Add(new FormationAI.BehaviorData
			{
				Behavior = behavior
			});
		}

		private bool FindBestBehavior()
		{
			BehaviorComponent behaviorComponent = null;
			float num = float.MinValue;
			foreach (BehaviorComponent behaviorComponent2 in this._behaviors)
			{
				if (behaviorComponent2.WeightFactor > 1E-07f)
				{
					float num2 = behaviorComponent2.GetAIWeight() * behaviorComponent2.WeightFactor;
					if (behaviorComponent2 == this.ActiveBehavior)
					{
						num2 *= MBMath.Lerp(1.2f, 2f, MBMath.ClampFloat((behaviorComponent2.PreserveExpireTime - Mission.Current.CurrentTime) / 5f, 0f, 1f), float.MinValue);
					}
					if (num2 > num)
					{
						if (behaviorComponent2.NavmeshlessTargetPositionPenalty > 0f)
						{
							num2 /= behaviorComponent2.NavmeshlessTargetPositionPenalty;
						}
						behaviorComponent2.PrecalculateMovementOrder();
						num2 *= behaviorComponent2.NavmeshlessTargetPositionPenalty;
						if (num2 > num)
						{
							behaviorComponent = behaviorComponent2;
							num = num2;
						}
					}
				}
			}
			if (behaviorComponent != null)
			{
				this.ActiveBehavior = behaviorComponent;
				if (behaviorComponent != this._behaviors[0])
				{
					this._behaviors.Remove(behaviorComponent);
					this._behaviors.Insert(0, behaviorComponent);
				}
				return true;
			}
			return false;
		}

		private void PreprocessBehaviors()
		{
			if (this._formation.HasAnyEnemyFormationsThatIsNotEmpty())
			{
				FormationAI.BehaviorData behaviorData = this._specialBehaviorData.FirstOrDefault((FormationAI.BehaviorData sd) => !sd.IsPreprocessed);
				if (behaviorData != null)
				{
					behaviorData.Behavior.TickOccasionally();
					float num = behaviorData.Behavior.GetAIWeight();
					if (behaviorData.Behavior == this.ActiveBehavior)
					{
						num *= MBMath.Lerp(1.01f, 1.5f, MBMath.ClampFloat((behaviorData.Behavior.PreserveExpireTime - Mission.Current.CurrentTime) / 5f, 0f, 1f), float.MinValue);
					}
					behaviorData.Weight = num * behaviorData.Preference;
					behaviorData.IsPreprocessed = true;
				}
			}
		}

		public void Tick()
		{
			if (Mission.Current.AllowAiTicking && (Mission.Current.ForceTickOccasionally || this._tickTimer.Check(Mission.Current.CurrentTime)))
			{
				this.TickOccasionally(this._tickTimer.PreviousDeltaTime);
			}
		}

		private void TickOccasionally(float dt)
		{
			this._formation.IsAITickedAfterSplit = true;
			if (this.FindBestBehavior())
			{
				if (!this._formation.IsAIControlled)
				{
					if (MultiplayerGame.Current == null && Mission.Current.MainAgent != null && !this._formation.Team.IsPlayerGeneral && this._formation.Team.IsPlayerSergeant && this._formation.PlayerOwner == Agent.Main)
					{
						this.ActiveBehavior.RemindSergeantPlayer();
						return;
					}
				}
				else
				{
					this.ActiveBehavior.TickOccasionally();
				}
				return;
			}
			BehaviorComponent behaviorComponent = this.ActiveBehavior;
			if (this._formation.HasAnyEnemyFormationsThatIsNotEmpty())
			{
				this.PreprocessBehaviors();
				foreach (FormationAI.BehaviorData behaviorData in this._specialBehaviorData)
				{
					behaviorData.IsPreprocessed = false;
				}
				if (behaviorComponent is BehaviorStop && this._specialBehaviorData.Count > 0)
				{
					IEnumerable<FormationAI.BehaviorData> enumerable = this._specialBehaviorData.Where((FormationAI.BehaviorData sbd) => sbd.Weight > 0f);
					if (enumerable.Any<FormationAI.BehaviorData>())
					{
						behaviorComponent = enumerable.MaxBy((FormationAI.BehaviorData abd) => abd.Weight).Behavior;
					}
				}
				bool isAIControlled = this._formation.IsAIControlled;
				bool flag = false;
				if (this.ActiveBehavior != behaviorComponent)
				{
					BehaviorComponent activeBehavior = this.ActiveBehavior;
					this.ActiveBehavior = behaviorComponent;
					flag = true;
				}
				if (flag || (behaviorComponent != null && behaviorComponent.IsCurrentOrderChanged))
				{
					if (this._formation.IsAIControlled)
					{
						this._formation.SetMovementOrder(behaviorComponent.CurrentOrder);
					}
					behaviorComponent.IsCurrentOrderChanged = false;
				}
			}
		}

		[Conditional("DEBUG")]
		public void DebugMore()
		{
			if (!MBDebug.IsDisplayingHighLevelAI)
			{
				return;
			}
			foreach (FormationAI.BehaviorData behaviorData in this._specialBehaviorData.OrderBy((FormationAI.BehaviorData d) => d.Behavior.GetType().ToString()))
			{
				behaviorData.Behavior.GetType().ToString().Replace("MBModule.Behavior", "");
				behaviorData.Weight.ToString("0.00");
				behaviorData.Preference.ToString("0.00");
			}
		}

		[Conditional("DEBUG")]
		public void DebugScores()
		{
			if (this._formation.IsRanged())
			{
				MBDebug.Print("Ranged", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			else if (this._formation.IsCavalry())
			{
				MBDebug.Print("Cavalry", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			else
			{
				MBDebug.Print("Infantry", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			foreach (FormationAI.BehaviorData behaviorData in this._specialBehaviorData.OrderBy((FormationAI.BehaviorData d) => d.Behavior.GetType().ToString()))
			{
				string text = behaviorData.Behavior.GetType().ToString().Replace("MBModule.Behavior", "");
				string text2 = behaviorData.Weight.ToString("0.00");
				string text3 = behaviorData.Preference.ToString("0.00");
				MBDebug.Print(string.Concat(new string[] { text, " \t\t w:", text2, "\t p:", text3 }), 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		public void ResetBehaviorWeights()
		{
			foreach (BehaviorComponent behaviorComponent in this._behaviors)
			{
				behaviorComponent.ResetBehavior();
			}
		}

		private const float BehaviorPreserveTime = 5f;

		private readonly Formation _formation;

		private readonly List<FormationAI.BehaviorData> _specialBehaviorData;

		private readonly List<BehaviorComponent> _behaviors = new List<BehaviorComponent>();

		private BehaviorComponent _activeBehavior;

		private FormationAI.BehaviorSide _side = FormationAI.BehaviorSide.Middle;

		private readonly Timer _tickTimer;

		public class BehaviorData
		{
			public BehaviorComponent Behavior;

			public float Preference = 1f;

			public float Weight;

			public bool IsRemovedOnCancel;

			public bool IsPreprocessed;
		}

		public enum BehaviorSide
		{
			Left,
			Middle,
			Right,
			BehaviorSideNotSet,
			ValidBehaviorSideCount = 3
		}
	}
}
