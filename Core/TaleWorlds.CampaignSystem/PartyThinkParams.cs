using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000089 RID: 137
	public class PartyThinkParams
	{
		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06001075 RID: 4213 RVA: 0x0004A7D2 File Offset: 0x000489D2
		public MBReadOnlyList<ValueTuple<AIBehaviorTuple, float>> AIBehaviorScores
		{
			get
			{
				return this._aiBehaviorScores;
			}
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x0004A7DA File Offset: 0x000489DA
		public PartyThinkParams(MobileParty mobileParty)
		{
			this._aiBehaviorScores = new MBList<ValueTuple<AIBehaviorTuple, float>>(16);
			this.MobilePartyOf = mobileParty;
			this.WillGatherAnArmy = false;
			this.DoNotChangeBehavior = false;
			this.CurrentObjectiveValue = 0f;
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0004A810 File Offset: 0x00048A10
		public void Reset(MobileParty mobileParty)
		{
			this._aiBehaviorScores.Clear();
			this.MobilePartyOf = mobileParty;
			this.WillGatherAnArmy = false;
			this.DoNotChangeBehavior = false;
			this.CurrentObjectiveValue = 0f;
			this.StrengthOfLordsWithoutArmy = 0f;
			this.StrengthOfLordsWithArmy = 0f;
			this.StrengthOfLordsAtSameClanWithoutArmy = 0f;
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x0004A86C File Offset: 0x00048A6C
		public void Initialization()
		{
			this.StrengthOfLordsWithoutArmy = 0f;
			this.StrengthOfLordsWithArmy = 0f;
			this.StrengthOfLordsAtSameClanWithoutArmy = 0f;
			foreach (Hero hero in this.MobilePartyOf.MapFaction.Heroes)
			{
				if (hero.PartyBelongedTo != null)
				{
					MobileParty partyBelongedTo = hero.PartyBelongedTo;
					if (partyBelongedTo.Army != null)
					{
						this.StrengthOfLordsWithArmy += partyBelongedTo.Party.TotalStrength;
					}
					else
					{
						this.StrengthOfLordsWithoutArmy += partyBelongedTo.Party.TotalStrength;
						Clan clan = hero.Clan;
						Hero leaderHero = this.MobilePartyOf.LeaderHero;
						if (clan == ((leaderHero != null) ? leaderHero.Clan : null))
						{
							this.StrengthOfLordsAtSameClanWithoutArmy += partyBelongedTo.Party.TotalStrength;
						}
					}
				}
			}
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x0004A96C File Offset: 0x00048B6C
		public bool TryGetBehaviorScore(in AIBehaviorTuple aiBehaviorTuple, out float score)
		{
			foreach (ValueTuple<AIBehaviorTuple, float> valueTuple in this._aiBehaviorScores)
			{
				AIBehaviorTuple item = valueTuple.Item1;
				if (item.Equals(aiBehaviorTuple))
				{
					score = valueTuple.Item2;
					return true;
				}
			}
			score = 0f;
			return false;
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x0004A9E4 File Offset: 0x00048BE4
		public void SetBehaviorScore(in AIBehaviorTuple aiBehaviorTuple, float score)
		{
			for (int i = 0; i < this._aiBehaviorScores.Count; i++)
			{
				if (this._aiBehaviorScores[i].Item1.Equals(aiBehaviorTuple))
				{
					this._aiBehaviorScores[i] = new ValueTuple<AIBehaviorTuple, float>(this._aiBehaviorScores[i].Item1, score);
					return;
				}
			}
			Debug.FailedAssert("AIBehaviorScore not found.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\ICampaignBehaviorManager.cs", "SetBehaviorScore", 152);
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0004AA65 File Offset: 0x00048C65
		public void AddBehaviorScore(in ValueTuple<AIBehaviorTuple, float> value)
		{
			this._aiBehaviorScores.Add(value);
		}

		// Token: 0x040005DF RID: 1503
		public MobileParty MobilePartyOf;

		// Token: 0x040005E0 RID: 1504
		private readonly MBList<ValueTuple<AIBehaviorTuple, float>> _aiBehaviorScores;

		// Token: 0x040005E1 RID: 1505
		public float CurrentObjectiveValue;

		// Token: 0x040005E2 RID: 1506
		public bool WillGatherAnArmy;

		// Token: 0x040005E3 RID: 1507
		public bool DoNotChangeBehavior;

		// Token: 0x040005E4 RID: 1508
		public float StrengthOfLordsWithoutArmy;

		// Token: 0x040005E5 RID: 1509
		public float StrengthOfLordsWithArmy;

		// Token: 0x040005E6 RID: 1510
		public float StrengthOfLordsAtSameClanWithoutArmy;
	}
}
