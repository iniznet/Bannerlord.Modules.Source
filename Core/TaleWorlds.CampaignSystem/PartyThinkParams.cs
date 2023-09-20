using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public class PartyThinkParams
	{
		public MBReadOnlyList<ValueTuple<AIBehaviorTuple, float>> AIBehaviorScores
		{
			get
			{
				return this._aiBehaviorScores;
			}
		}

		public PartyThinkParams(MobileParty mobileParty)
		{
			this._aiBehaviorScores = new MBList<ValueTuple<AIBehaviorTuple, float>>(16);
			this.MobilePartyOf = mobileParty;
			this.WillGatherAnArmy = false;
			this.DoNotChangeBehavior = false;
			this.CurrentObjectiveValue = 0f;
		}

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

		public void AddBehaviorScore(in ValueTuple<AIBehaviorTuple, float> value)
		{
			this._aiBehaviorScores.Add(value);
		}

		public MobileParty MobilePartyOf;

		private readonly MBList<ValueTuple<AIBehaviorTuple, float>> _aiBehaviorScores;

		public float CurrentObjectiveValue;

		public bool WillGatherAnArmy;

		public bool DoNotChangeBehavior;

		public float StrengthOfLordsWithoutArmy;

		public float StrengthOfLordsWithArmy;

		public float StrengthOfLordsAtSameClanWithoutArmy;
	}
}
