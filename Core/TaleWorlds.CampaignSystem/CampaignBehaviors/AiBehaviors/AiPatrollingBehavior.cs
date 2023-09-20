using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	public class AiPatrollingBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private static void CalculatePatrollingScoreForSettlement(Settlement settlement, PartyThinkParams p, float patrollingScoreAdjustment)
		{
			MobileParty mobilePartyOf = p.MobilePartyOf;
			AIBehaviorTuple aibehaviorTuple = new AIBehaviorTuple(settlement, AiBehavior.PatrolAroundPoint, false);
			float num = Campaign.Current.Models.TargetScoreCalculatingModel.CalculatePatrollingScoreForSettlement(settlement, mobilePartyOf);
			num *= patrollingScoreAdjustment;
			float num2;
			if (p.TryGetBehaviorScore(aibehaviorTuple, out num2))
			{
				p.SetBehaviorScore(aibehaviorTuple, num2 + num);
				return;
			}
			ValueTuple<AIBehaviorTuple, float> valueTuple = new ValueTuple<AIBehaviorTuple, float>(aibehaviorTuple, num);
			p.AddBehaviorScore(valueTuple);
		}

		public void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			if (mobileParty.IsMilitia || mobileParty.IsCaravan || mobileParty.IsVillager || mobileParty.IsBandit || mobileParty.IsDisbanding || (!mobileParty.MapFaction.IsMinorFaction && !mobileParty.MapFaction.IsKingdomFaction && !mobileParty.MapFaction.Leader.IsLord))
			{
				return;
			}
			Settlement currentSettlement = mobileParty.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) != null)
			{
				return;
			}
			float num4;
			if (mobileParty.Army != null)
			{
				float num = 0f;
				foreach (MobileParty mobileParty2 in mobileParty.Army.Parties)
				{
					float num2 = PartyBaseHelper.FindPartySizeNormalLimit(mobileParty2);
					float num3 = mobileParty2.PartySizeRatio / num2;
					num += num3;
				}
				num4 = num / (float)mobileParty.Army.Parties.Count;
			}
			else
			{
				float num5 = PartyBaseHelper.FindPartySizeNormalLimit(mobileParty);
				num4 = mobileParty.PartySizeRatio / num5;
			}
			float num6 = MathF.Sqrt(MathF.Min(1f, num4));
			float num7 = ((mobileParty.CurrentSettlement != null && mobileParty.CurrentSettlement.IsFortification && mobileParty.CurrentSettlement.IsUnderSiege) ? 0f : 1f);
			num7 *= num6;
			if (mobileParty.Party.MapFaction.Settlements.Count > 0)
			{
				using (List<Settlement>.Enumerator enumerator2 = mobileParty.Party.MapFaction.Settlements.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Settlement settlement = enumerator2.Current;
						if (settlement.IsTown || settlement.IsVillage || settlement.MapFaction.IsMinorFaction)
						{
							AiPatrollingBehavior.CalculatePatrollingScoreForSettlement(settlement, p, num7);
						}
					}
					return;
				}
			}
			int num8 = -1;
			do
			{
				num8 = SettlementHelper.FindNextSettlementAroundMapPoint(mobileParty, Campaign.AverageDistanceBetweenTwoFortifications * 5f, num8);
				if (num8 >= 0 && Settlement.All[num8].IsTown)
				{
					AiPatrollingBehavior.CalculatePatrollingScoreForSettlement(Settlement.All[num8], p, num7);
				}
			}
			while (num8 >= 0);
		}
	}
}
