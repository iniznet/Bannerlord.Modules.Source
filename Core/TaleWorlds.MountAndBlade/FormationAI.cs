using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000129 RID: 297
	public class FormationAI
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000DD9 RID: 3545 RVA: 0x00026F48 File Offset: 0x00025148
		// (remove) Token: 0x06000DDA RID: 3546 RVA: 0x00026F80 File Offset: 0x00025180
		public event Action<Formation> OnActiveBehaviorChanged;

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000DDB RID: 3547 RVA: 0x00026FB5 File Offset: 0x000251B5
		// (set) Token: 0x06000DDC RID: 3548 RVA: 0x00026FC0 File Offset: 0x000251C0
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

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000DDD RID: 3549 RVA: 0x00027040 File Offset: 0x00025240
		// (set) Token: 0x06000DDE RID: 3550 RVA: 0x00027048 File Offset: 0x00025248
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

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000DDF RID: 3551 RVA: 0x000270B4 File Offset: 0x000252B4
		// (set) Token: 0x06000DE0 RID: 3552 RVA: 0x000270BC File Offset: 0x000252BC
		public bool IsMainFormation { get; set; }

		// Token: 0x06000DE1 RID: 3553 RVA: 0x000270C8 File Offset: 0x000252C8
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

		// Token: 0x06000DE2 RID: 3554 RVA: 0x00027170 File Offset: 0x00025370
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

		// Token: 0x06000DE3 RID: 3555 RVA: 0x000271EC File Offset: 0x000253EC
		public void AddAiBehavior(BehaviorComponent behaviorComponent)
		{
			this._behaviors.Add(behaviorComponent);
		}

		// Token: 0x06000DE4 RID: 3556 RVA: 0x000271FC File Offset: 0x000253FC
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

		// Token: 0x06000DE5 RID: 3557 RVA: 0x000272C4 File Offset: 0x000254C4
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

		// Token: 0x06000DE6 RID: 3558 RVA: 0x000272EC File Offset: 0x000254EC
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

		// Token: 0x06000DE7 RID: 3559 RVA: 0x0002741C File Offset: 0x0002561C
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

		// Token: 0x06000DE8 RID: 3560 RVA: 0x000274E4 File Offset: 0x000256E4
		public void Tick()
		{
			if (Mission.Current.AllowAiTicking && (Mission.Current.ForceTickOccasionally || this._tickTimer.Check(Mission.Current.CurrentTime)))
			{
				this.TickOccasionally(this._tickTimer.PreviousDeltaTime);
			}
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x00027534 File Offset: 0x00025734
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

		// Token: 0x06000DEA RID: 3562 RVA: 0x000276F8 File Offset: 0x000258F8
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

		// Token: 0x06000DEB RID: 3563 RVA: 0x000277AC File Offset: 0x000259AC
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

		// Token: 0x06000DEC RID: 3564 RVA: 0x000278F4 File Offset: 0x00025AF4
		public void ResetBehaviorWeights()
		{
			foreach (BehaviorComponent behaviorComponent in this._behaviors)
			{
				behaviorComponent.ResetBehavior();
			}
		}

		// Token: 0x04000373 RID: 883
		private const float BehaviorPreserveTime = 5f;

		// Token: 0x04000375 RID: 885
		private readonly Formation _formation;

		// Token: 0x04000376 RID: 886
		private readonly List<FormationAI.BehaviorData> _specialBehaviorData;

		// Token: 0x04000377 RID: 887
		private readonly List<BehaviorComponent> _behaviors = new List<BehaviorComponent>();

		// Token: 0x04000378 RID: 888
		private BehaviorComponent _activeBehavior;

		// Token: 0x04000379 RID: 889
		private FormationAI.BehaviorSide _side = FormationAI.BehaviorSide.Middle;

		// Token: 0x0400037A RID: 890
		private readonly Timer _tickTimer;

		// Token: 0x0200045F RID: 1119
		public class BehaviorData
		{
			// Token: 0x040018B8 RID: 6328
			public BehaviorComponent Behavior;

			// Token: 0x040018B9 RID: 6329
			public float Preference = 1f;

			// Token: 0x040018BA RID: 6330
			public float Weight;

			// Token: 0x040018BB RID: 6331
			public bool IsRemovedOnCancel;

			// Token: 0x040018BC RID: 6332
			public bool IsPreprocessed;
		}

		// Token: 0x02000460 RID: 1120
		public enum BehaviorSide
		{
			// Token: 0x040018BE RID: 6334
			Left,
			// Token: 0x040018BF RID: 6335
			Middle,
			// Token: 0x040018C0 RID: 6336
			Right,
			// Token: 0x040018C1 RID: 6337
			BehaviorSideNotSet,
			// Token: 0x040018C2 RID: 6338
			ValidBehaviorSideCount = 3
		}
	}
}
