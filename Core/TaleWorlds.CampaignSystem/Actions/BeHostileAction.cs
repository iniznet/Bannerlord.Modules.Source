using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000426 RID: 1062
	public static class BeHostileAction
	{
		// Token: 0x06003E8A RID: 16010 RVA: 0x0012A780 File Offset: 0x00128980
		private static void ApplyInternal(PartyBase attackerParty, PartyBase defenderParty, float value)
		{
			if (defenderParty.IsMobile && defenderParty.MobileParty.ActualClan == CampaignData.NeutralFaction)
			{
				return;
			}
			int num = (int)(-1f * value);
			int num2 = (int)(-5f * value);
			int num3 = (int)(-1f * value);
			int num4 = (int)(-4f * value);
			int num5 = (int)(-4f * value);
			int num6 = (int)(-10f * value);
			int num7 = (int)(-2f * value);
			bool flag = attackerParty.MapFaction.IsAtWarWith(defenderParty.MapFaction);
			Hero leaderHero = attackerParty.LeaderHero;
			if (defenderParty.IsSettlement)
			{
				if (defenderParty.Settlement.IsVillage && !flag)
				{
					if (num4 < 0)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, defenderParty.Settlement.OwnerClan.Leader, num4, true);
						foreach (Hero hero in defenderParty.Settlement.Notables)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, hero, num5, true);
						}
					}
					BeHostileAction.ApplyGeneralConsequencesOnPeace(attackerParty, defenderParty, value);
					return;
				}
			}
			else if (defenderParty.MobileParty != null)
			{
				if (defenderParty.MobileParty.IsVillager)
				{
					if (flag)
					{
						using (List<Hero>.Enumerator enumerator = defenderParty.MobileParty.HomeSettlement.Notables.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Hero hero2 = enumerator.Current;
								ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, hero2, num3, true);
							}
							return;
						}
					}
					if (num < 0)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, defenderParty.MobileParty.HomeSettlement.OwnerClan.Leader, num, true);
						foreach (Hero hero3 in defenderParty.MobileParty.HomeSettlement.Notables)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, hero3, num2, true);
						}
					}
					BeHostileAction.ApplyGeneralConsequencesOnPeace(attackerParty, defenderParty, value);
					return;
				}
				if (defenderParty.MobileParty.IsCaravan)
				{
					if (flag)
					{
						if (num7 < 0 && defenderParty.Owner != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, defenderParty.Owner, num7, true);
							return;
						}
					}
					else
					{
						if (num6 < 0 && defenderParty.Owner != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, defenderParty.Owner, num6, true);
						}
						BeHostileAction.ApplyGeneralConsequencesOnPeace(attackerParty, defenderParty, value);
					}
				}
			}
		}

		// Token: 0x06003E8B RID: 16011 RVA: 0x0012A9E4 File Offset: 0x00128BE4
		private static void ApplyGeneralConsequencesOnPeace(PartyBase attackerParty, PartyBase defenderParty, float value)
		{
			float num = -25f * value;
			float num2 = 10f * value;
			int num3 = (int)(-2f * value);
			float num4 = -50f * value;
			bool isClan = attackerParty.MapFaction.IsClan;
			bool isFactionLeader = attackerParty.LeaderHero.IsFactionLeader;
			bool isUnderMercenaryService = attackerParty.LeaderHero.Clan.IsUnderMercenaryService;
			Hero leaderHero = attackerParty.LeaderHero;
			if (leaderHero.Equals(Hero.MainHero))
			{
				if (num < 0f)
				{
					TraitLevelingHelper.OnHostileAction((int)num);
				}
				if (num2 > 0f)
				{
					ChangeCrimeRatingAction.Apply(defenderParty.MapFaction, num2, true);
				}
			}
			if (!isClan)
			{
				if (isFactionLeader)
				{
					if (num4 < 0f)
					{
						GainKingdomInfluenceAction.ApplyForDefault(attackerParty.MobileParty.LeaderHero, num4);
						return;
					}
				}
				else if (isUnderMercenaryService)
				{
					if (num3 < 0)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, leaderHero.MapFaction.Leader, num3, true);
					}
					if (value == 6f)
					{
						ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(leaderHero.Clan, true);
						return;
					}
				}
				else
				{
					if (num3 < 0 && attackerParty.MapFaction != CampaignData.NeutralFaction && defenderParty.MapFaction != CampaignData.NeutralFaction)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leaderHero, defenderParty.MapFaction.Leader, num3, true);
					}
					if (num4 < 0f)
					{
						GainKingdomInfluenceAction.ApplyForDefault(attackerParty.MobileParty.LeaderHero, num4);
					}
				}
			}
		}

		// Token: 0x06003E8C RID: 16012 RVA: 0x0012AB1A File Offset: 0x00128D1A
		public static void ApplyHostileAction(PartyBase attackerParty, PartyBase defenderParty, float value)
		{
			if (attackerParty == null || defenderParty == null || value == 0f)
			{
				Debug.FailedAssert("BeHostileAction, attackerParty and/or defenderParty is null or value is 0.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\BeHostileAction.cs", "ApplyHostileAction", 199);
				return;
			}
			BeHostileAction.ApplyInternal(attackerParty, defenderParty, value);
		}

		// Token: 0x06003E8D RID: 16013 RVA: 0x0012AB4C File Offset: 0x00128D4C
		public static void ApplyMinorCoercionHostileAction(PartyBase attackerParty, PartyBase defenderParty)
		{
			if (attackerParty == null || defenderParty == null)
			{
				Debug.FailedAssert("BeHostileAction, attackerParty and/or defenderParty is null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\BeHostileAction.cs", "ApplyMinorCoercionHostileAction", 211);
				return;
			}
			BeHostileAction.ApplyInternal(attackerParty, defenderParty, 1f);
		}

		// Token: 0x06003E8E RID: 16014 RVA: 0x0012AB7A File Offset: 0x00128D7A
		public static void ApplyMajorCoercionHostileAction(PartyBase attackerParty, PartyBase defenderParty)
		{
			if (attackerParty == null || defenderParty == null)
			{
				Debug.FailedAssert("BeHostileAction, attackerParty and/or defenderParty is null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\BeHostileAction.cs", "ApplyMajorCoercionHostileAction", 223);
				return;
			}
			BeHostileAction.ApplyInternal(attackerParty, defenderParty, 2f);
		}

		// Token: 0x06003E8F RID: 16015 RVA: 0x0012ABA8 File Offset: 0x00128DA8
		public static void ApplyEncounterHostileAction(PartyBase attackerParty, PartyBase defenderParty)
		{
			if (Campaign.Current.Models.EncounterModel.IsEncounterExemptFromHostileActions(attackerParty, defenderParty))
			{
				return;
			}
			BeHostileAction.ApplyInternal(attackerParty, defenderParty, 6f);
			if (attackerParty == PartyBase.MainParty && attackerParty.MapFaction != defenderParty.MapFaction && !FactionManager.IsAtWarAgainstFaction(attackerParty.MapFaction, defenderParty.MapFaction))
			{
				ChangeRelationAction.ApplyPlayerRelation(defenderParty.MapFaction.Leader, -10, true, true);
				DeclareWarAction.ApplyByPlayerHostility(attackerParty.MapFaction, defenderParty.MapFaction);
			}
		}

		// Token: 0x040012BB RID: 4795
		private const float MinorCoercionValue = 1f;

		// Token: 0x040012BC RID: 4796
		private const float MajorCoercionValue = 2f;

		// Token: 0x040012BD RID: 4797
		private const float EncounterValue = 6f;
	}
}
