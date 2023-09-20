using System;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	// Token: 0x02000400 RID: 1024
	public class AiEngagePartyBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003D25 RID: 15653 RVA: 0x00122F04 File Offset: 0x00121104
		public override void RegisterEvents()
		{
			CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
		}

		// Token: 0x06003D26 RID: 15654 RVA: 0x00122F1D File Offset: 0x0012111D
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003D27 RID: 15655 RVA: 0x00122F20 File Offset: 0x00121120
		public void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			if (mobileParty.CurrentSettlement != null && mobileParty.CurrentSettlement.SiegeEvent != null)
			{
				return;
			}
			float num = 25f;
			if ((mobileParty.MapFaction.IsKingdomFaction || mobileParty.MapFaction == Hero.MainHero.MapFaction) && !mobileParty.IsCaravan && (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty) && mobileParty.LeaderHero != null)
			{
				float num2 = Campaign.MapDiagonalSquared;
				LocatableSearchData<Settlement> locatableSearchData = Settlement.StartFindingLocatablesAroundPosition(mobileParty.Position2D, 50f);
				for (Settlement settlement = Settlement.FindNextLocatable(ref locatableSearchData); settlement != null; settlement = Settlement.FindNextLocatable(ref locatableSearchData))
				{
					if (settlement.MapFaction == mobileParty.MapFaction)
					{
						float num3 = settlement.Position2D.DistanceSquared(mobileParty.Position2D);
						if (num3 < num2)
						{
							num2 = num3;
						}
					}
				}
				float num4 = MathF.Sqrt(num2);
				float num5 = ((num4 < 50f) ? (1f - MathF.Max(0f, num4 - 15f) / 35f) : 0f);
				if (num5 > 0f)
				{
					float num6 = mobileParty.PartySizeRatio;
					foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
					{
						num6 += mobileParty2.PartySizeRatio;
					}
					float num7 = MathF.Min(1f, num6 / ((float)mobileParty.AttachedParties.Count + 1f));
					float num8 = num7 * ((num7 <= 0.5f) ? num7 : (0.5f + 0.707f * MathF.Sqrt(num7 - 0.5f)));
					LocatableSearchData<MobileParty> locatableSearchData2 = MobileParty.StartFindingLocatablesAroundPosition(mobileParty.Position2D, num);
					for (MobileParty mobileParty3 = MobileParty.FindNextLocatable(ref locatableSearchData2); mobileParty3 != null; mobileParty3 = MobileParty.FindNextLocatable(ref locatableSearchData2))
					{
						if (mobileParty3.IsActive)
						{
							IFaction mapFaction = mobileParty3.MapFaction;
							if (mapFaction != null && mapFaction.IsAtWarWith(mobileParty.MapFaction) && (mobileParty3.Army == null || mobileParty3 == mobileParty3.Army.LeaderParty))
							{
								IFaction mapFaction2 = mobileParty3.MapFaction;
								if (((mapFaction2 != null && mapFaction2.IsKingdomFaction) || mobileParty3.MapFaction == Hero.MainHero.MapFaction) && (mobileParty3.CurrentSettlement == null || !mobileParty3.CurrentSettlement.IsFortification) && mobileParty3.Aggressiveness > 0.1f)
								{
									Army army = mobileParty3.Army;
									float num9 = ((army != null) ? army.TotalStrength : mobileParty3.Party.TotalStrength);
									float num10 = 1f - 0.5f * mobileParty3.Position2D.DistanceSquared(mobileParty.Position2D) / (num * num);
									float num11 = 1f;
									if (mobileParty3.LeaderHero != null)
									{
										int relation = mobileParty3.LeaderHero.GetRelation(mobileParty.LeaderHero);
										if (relation < 0)
										{
											num11 = 1f + MathF.Sqrt((float)(-(float)relation)) / 20f;
										}
										else
										{
											num11 = 1f - MathF.Sqrt((float)relation) / 10f;
										}
									}
									float num12 = 0f;
									LocatableSearchData<MobileParty> locatableSearchData3 = MobileParty.StartFindingLocatablesAroundPosition(mobileParty.Position2D, num);
									for (MobileParty mobileParty4 = MobileParty.FindNextLocatable(ref locatableSearchData3); mobileParty4 != null; mobileParty4 = MobileParty.FindNextLocatable(ref locatableSearchData3))
									{
										if (mobileParty4 != mobileParty && mobileParty4.MapFaction == mobileParty.MapFaction && (mobileParty4.Army == null || mobileParty4.Army.LeaderParty == mobileParty4) && ((mobileParty4.DefaultBehavior == AiBehavior.GoAroundParty && mobileParty4.TargetParty == mobileParty3) || (mobileParty4.ShortTermBehavior == AiBehavior.EngageParty && mobileParty4.ShortTermTargetParty == mobileParty3)))
										{
											float num13 = num12;
											Army army2 = mobileParty4.Army;
											num12 = num13 + ((army2 != null) ? army2.TotalStrength : mobileParty4.Party.TotalStrength);
										}
									}
									Army army3 = mobileParty.Army;
									float num14 = ((army3 != null) ? army3.TotalStrength : mobileParty.Party.TotalStrength);
									float num15 = (num12 + num14) / num9;
									float num16 = ((mobileParty3.CurrentSettlement != null && mobileParty3.CurrentSettlement.IsFortification && mobileParty3.CurrentSettlement.MapFaction != mobileParty.MapFaction) ? 0.25f : 1f);
									float num17 = 1f;
									if (num12 + (num14 + 30f) > num9 * 1.5f)
									{
										float num18 = num9 * 1.5f + 10f + ((mobileParty3.MapEvent != null || mobileParty3.SiegeEvent != null) ? 30f : 0f);
										float num19 = num12 + (num14 + 30f);
										num17 = MathF.Pow(num18 / num19, 0.8f);
									}
									float speed = mobileParty.Speed;
									float speed2 = mobileParty3.Speed;
									float num20 = speed / speed2;
									float num21 = num20 * num20 * num20 * num20;
									float num22 = ((speed > speed2 && mobileParty.Army == null) ? 1f : ((num12 + num14 > num9) ? (0.5f + 0.5f * num21 * num17) : (0.5f * num21)));
									float num23 = ((mobileParty.DefaultBehavior == AiBehavior.GoAroundParty && mobileParty3 == mobileParty.TargetParty) ? 1.1f : 1f);
									float num24 = ((mobileParty.Army != null) ? 0.9f : 1f);
									float num25 = ((mobileParty3 == MobileParty.MainParty) ? 1.2f : 1f);
									float num26 = 1f;
									if (mobileParty.Objective == MobileParty.PartyObjective.Defensive)
									{
										num26 = 1.2f;
									}
									float num27 = 1f;
									if (mobileParty.MapFaction != null && mobileParty.MapFaction.IsKingdomFaction && mobileParty.MapFaction.Leader == Hero.MainHero)
									{
										StanceLink stanceWith = Hero.MainHero.MapFaction.GetStanceWith(mobileParty3.MapFaction);
										if (stanceWith != null && stanceWith.BehaviorPriority == 1)
										{
											num27 = 1.2f;
										}
									}
									float num28 = num10 * num5 * num11 * num15 * num25 * num8 * num22 * num17 * num16 * num23 * num24 * num26 * num27 * 2f;
									if (num28 > 0.05f && mobileParty3.CurrentSettlement == null)
									{
										float num29 = Campaign.MapDiagonalSquared;
										LocatableSearchData<Settlement> locatableSearchData4 = Settlement.StartFindingLocatablesAroundPosition(mobileParty3.Position2D, 25f);
										for (Settlement settlement2 = Settlement.FindNextLocatable(ref locatableSearchData4); settlement2 != null; settlement2 = Settlement.FindNextLocatable(ref locatableSearchData4))
										{
											if (settlement2.MapFaction == mobileParty3.MapFaction)
											{
												float num30 = settlement2.Position2D.DistanceSquared(mobileParty.Position2D);
												if (num30 < num29)
												{
													num29 = num30;
												}
											}
										}
										if (num29 < 625f)
										{
											float num31 = MathF.Sqrt(num29);
											num28 *= 0.25f + 0.75f * (MathF.Max(0f, num31 - 5f) / 20f);
										}
									}
									p.CurrentObjectiveValue = num28;
									AiBehavior aiBehavior = AiBehavior.GoAroundParty;
									AIBehaviorTuple aibehaviorTuple = new AIBehaviorTuple(mobileParty3, aiBehavior, false);
									ValueTuple<AIBehaviorTuple, float> valueTuple = new ValueTuple<AIBehaviorTuple, float>(aibehaviorTuple, num28);
									p.AddBehaviorScore(valueTuple);
								}
							}
						}
					}
				}
			}
		}
	}
}
