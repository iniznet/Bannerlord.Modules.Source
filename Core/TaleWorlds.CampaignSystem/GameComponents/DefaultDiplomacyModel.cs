using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000105 RID: 261
	public class DefaultDiplomacyModel : DiplomacyModel
	{
		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06001532 RID: 5426 RVA: 0x00061316 File Offset: 0x0005F516
		public override int MinimumRelationWithConversationCharacterToJoinKingdom
		{
			get
			{
				return -10;
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06001533 RID: 5427 RVA: 0x0006131A File Offset: 0x0005F51A
		public override int GiftingTownRelationshipBonus
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06001534 RID: 5428 RVA: 0x0006131E File Offset: 0x0005F51E
		public override int GiftingCastleRelationshipBonus
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06001535 RID: 5429 RVA: 0x00061322 File Offset: 0x0005F522
		public override int MaxRelationLimit
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06001536 RID: 5430 RVA: 0x00061326 File Offset: 0x0005F526
		public override int MinRelationLimit
		{
			get
			{
				return -100;
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06001537 RID: 5431 RVA: 0x0006132A File Offset: 0x0005F52A
		public override int MaxNeutralRelationLimit
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06001538 RID: 5432 RVA: 0x0006132E File Offset: 0x0005F52E
		public override int MinNeutralRelationLimit
		{
			get
			{
				return -10;
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x00061332 File Offset: 0x0005F532
		public override float GetStrengthThresholdForNonMutualWarsToBeIgnoredToJoinKingdom(Kingdom kingdomToJoin)
		{
			return kingdomToJoin.TotalStrength * 0.05f;
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x00061340 File Offset: 0x0005F540
		public override float GetClanStrength(Clan clan)
		{
			float num = 0f;
			foreach (Hero hero in clan.Heroes)
			{
				num += this.GetHeroCommandingStrengthForClan(hero);
			}
			float num2 = 1.2f;
			float num3 = clan.Influence * num2;
			float num4 = 4f;
			float num5 = (float)clan.Settlements.Count * num4;
			return num + num3 + num5;
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x000613CC File Offset: 0x0005F5CC
		public override float GetHeroCommandingStrengthForClan(Hero hero)
		{
			if (!hero.IsAlive)
			{
				return 0f;
			}
			float num = 1f;
			float num2 = 1f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 0.1f;
			float num6 = 5f;
			float num7 = (float)hero.GetSkillValue(DefaultSkills.Tactics) * num;
			float num8 = (float)hero.GetSkillValue(DefaultSkills.Steward) * num2;
			float num9 = (float)hero.GetSkillValue(DefaultSkills.Trade) * num3;
			float num10 = (float)hero.GetSkillValue(DefaultSkills.Leadership) * num4;
			float num11 = (float)((hero.GetTraitLevel(DefaultTraits.Commander) > 0) ? 300 : 0);
			float num12 = (float)hero.Gold * num5;
			float num13 = ((hero.PartyBelongedTo != null) ? (num6 * hero.PartyBelongedTo.Party.TotalStrength) : 0f);
			float num14 = 0f;
			if (hero.Clan.Leader == hero)
			{
				num14 += 500f;
			}
			float num15 = 0f;
			if (hero.Father == hero.Clan.Leader || hero.Clan.Leader.Father == hero || hero.Mother == hero.Clan.Leader || hero.Clan.Leader.Mother == hero)
			{
				num15 += 100f;
			}
			float num16 = 0f;
			if (hero.IsNoncombatant)
			{
				num16 -= 250f;
			}
			float num17 = 0f;
			if (hero.GovernorOf != null)
			{
				num17 -= 250f;
			}
			float num18 = num11 + num7 + num8 + num9 + num10 + num12 + num13 + num14 + num15 + num16 + num17;
			if (num18 <= 0f)
			{
				return 0f;
			}
			return num18;
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x00061580 File Offset: 0x0005F780
		public override float GetHeroGoverningStrengthForClan(Hero hero)
		{
			if (hero.IsAlive)
			{
				float num = 0.3f;
				float num2 = 0.9f;
				float num3 = 0.8f;
				float num4 = 1.2f;
				float num5 = 1f;
				float num6 = 0.005f;
				float num7 = 2f;
				float num8 = (float)hero.GetSkillValue(DefaultSkills.Tactics) * num;
				float num9 = (float)hero.GetSkillValue(DefaultSkills.Charm) * num2;
				float num10 = (float)hero.GetSkillValue(DefaultSkills.Engineering) * num3;
				float num11 = (float)hero.GetSkillValue(DefaultSkills.Steward) * num7;
				float num12 = (float)hero.GetSkillValue(DefaultSkills.Trade) * num4;
				float num13 = (float)hero.GetSkillValue(DefaultSkills.Leadership) * num5;
				float num14 = (float)((hero.GetTraitLevel(DefaultTraits.Honor) > 0) ? 100 : 0);
				float num15 = (float)MathF.Min(100000, hero.Gold) * num6;
				float num16 = 0f;
				if (hero.Spouse == hero.Clan.Leader)
				{
					num16 += 1000f;
				}
				if (hero.Father == hero.Clan.Leader || hero.Clan.Leader.Father == hero || hero.Mother == hero.Clan.Leader || hero.Clan.Leader.Mother == hero)
				{
					num16 += 750f;
				}
				if (hero.Siblings.Contains(hero.Clan.Leader))
				{
					num16 += 500f;
				}
				return num14 + num8 + num11 + num12 + num13 + num15 + num16 + num9 + num10;
			}
			return 0f;
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x0006170C File Offset: 0x0005F90C
		public override float GetRelationIncreaseFactor(Hero hero1, Hero hero2, float relationChange)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(relationChange, false, null);
			Hero hero3;
			if (hero1.IsHumanPlayerCharacter || hero2.IsHumanPlayerCharacter)
			{
				hero3 = (hero1.IsHumanPlayerCharacter ? hero1 : hero2);
			}
			else
			{
				hero3 = ((MBRandom.RandomFloat < 0.5f) ? hero1 : hero2);
			}
			SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Charm, DefaultSkillEffects.CharmRelationBonus, hero3.CharacterObject, ref explainedNumber, -1, true, 0);
			if (hero1.IsFemale != hero2.IsFemale)
			{
				if (hero3.GetPerkValue(DefaultPerks.Charm.InBloom))
				{
					explainedNumber.AddFactor(DefaultPerks.Charm.InBloom.PrimaryBonus, null);
				}
			}
			else if (hero3.GetPerkValue(DefaultPerks.Charm.YoungAndRespectful))
			{
				explainedNumber.AddFactor(DefaultPerks.Charm.YoungAndRespectful.PrimaryBonus, null);
			}
			if (hero3.GetPerkValue(DefaultPerks.Charm.GoodNatured) && hero2.GetTraitLevel(DefaultTraits.Mercy) > 0)
			{
				explainedNumber.Add(DefaultPerks.Charm.GoodNatured.SecondaryBonus, DefaultPerks.Charm.GoodNatured.Name, null);
			}
			if (hero3.GetPerkValue(DefaultPerks.Charm.Tribute) && hero2.GetTraitLevel(DefaultTraits.Mercy) < 0)
			{
				explainedNumber.Add(DefaultPerks.Charm.Tribute.SecondaryBonus, DefaultPerks.Charm.Tribute.Name, null);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x00061830 File Offset: 0x0005FA30
		public override int GetInfluenceAwardForSettlementCapturer(Settlement settlement)
		{
			if (settlement.IsTown || settlement.IsCastle)
			{
				int num = (settlement.IsTown ? 30 : 10);
				int num2 = 0;
				foreach (Village village in settlement.BoundVillages)
				{
					num2 += this.GetInfluenceAwardForSettlementCapturer(village.Settlement);
				}
				return num + num2;
			}
			return 10;
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x000618B4 File Offset: 0x0005FAB4
		public override float GetHourlyInfluenceAwardForBeingArmyMember(MobileParty mobileParty)
		{
			float totalStrength = mobileParty.Party.TotalStrength;
			float num = 0.0001f * (20f + totalStrength);
			if (mobileParty.BesiegedSettlement != null || mobileParty.MapEvent != null)
			{
				num *= 2f;
			}
			return num;
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x000618F4 File Offset: 0x0005FAF4
		public override float GetHourlyInfluenceAwardForRaidingEnemyVillage(MobileParty mobileParty)
		{
			int num = 0;
			foreach (MapEventParty mapEventParty in mobileParty.MapEvent.AttackerSide.Parties)
			{
				if (mapEventParty.Party.MobileParty != mobileParty)
				{
					MobileParty mobileParty2 = mapEventParty.Party.MobileParty;
					if (((mobileParty2 != null) ? mobileParty2.Army : null) == null || mapEventParty.Party.MobileParty.Army.LeaderParty != mobileParty)
					{
						continue;
					}
				}
				num += mapEventParty.Party.MemberRoster.TotalManCount;
			}
			return (MathF.Sqrt((float)num) + 2f) / 240f;
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x000619B4 File Offset: 0x0005FBB4
		public override float GetHourlyInfluenceAwardForBesiegingEnemyFortification(MobileParty mobileParty)
		{
			int num = 0;
			foreach (PartyBase partyBase in mobileParty.BesiegedSettlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
			{
				if (partyBase.MobileParty == mobileParty || (partyBase.MobileParty.Army != null && partyBase.MobileParty.Army.LeaderParty == mobileParty))
				{
					num += partyBase.MemberRoster.TotalManCount;
				}
			}
			return (MathF.Sqrt((float)num) + 2f) / 240f;
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x00061A58 File Offset: 0x0005FC58
		public override float GetScoreOfClanToJoinKingdom(Clan clan, Kingdom kingdom)
		{
			if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan)
			{
				return -100000000f;
			}
			int relationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
			int num = 0;
			int num2 = 0;
			foreach (Clan clan2 in kingdom.Clans)
			{
				int relationBetweenClans2 = FactionManager.GetRelationBetweenClans(clan, clan2);
				num += relationBetweenClans2;
				num2++;
			}
			float num3 = ((num2 > 0) ? ((float)num / (float)num2) : 0f);
			float num4 = MathF.Max(-100f, MathF.Min(100f, (float)relationBetweenClans + num3));
			float num5 = MathF.Min(2f, MathF.Max(0.33f, 1f + MathF.Sqrt(MathF.Abs(num4)) * ((num4 < 0f) ? (-0.067f) : 0.1f)));
			float num6 = 1f + ((kingdom.Culture == clan.Culture) ? 0.15f : ((kingdom.Leader == Hero.MainHero) ? 0f : (-0.15f)));
			float num7 = clan.CalculateTotalSettlementBaseValue();
			float num8 = clan.CalculateTotalSettlementValueForFaction(kingdom);
			int commanderLimit = clan.CommanderLimit;
			float num9 = 0f;
			float num10 = 0f;
			if (!clan.IsMinorFaction)
			{
				float num11 = 0f;
				foreach (Town town in kingdom.Fiefs)
				{
					num11 += town.Settlement.GetSettlementValueForFaction(kingdom);
				}
				int num12 = 0;
				foreach (Clan clan3 in kingdom.Clans)
				{
					if (!clan3.IsUnderMercenaryService && clan3 != clan)
					{
						num12 += clan3.CommanderLimit;
					}
				}
				num9 = num11 / (float)(num12 + commanderLimit);
				num10 = -((float)(num12 * num12) * 100f) + 10000f;
			}
			int gold = clan.Leader.Gold;
			float num13 = 0.5f * MathF.Min(1000000f, (clan.Kingdom != null) ? ((float)clan.Kingdom.KingdomBudgetWallet) : 0f) / ((clan.Kingdom != null) ? ((float)clan.Kingdom.Clans.Count + 1f) : 2f);
			float num14 = 0.15f;
			float num15 = num9 * MathF.Sqrt((float)commanderLimit) * num14 * 0.2f;
			num15 *= num5 * num6;
			num15 += (clan.MapFaction.IsAtWarWith(kingdom) ? (num8 - num7) : 0f);
			num15 += num10;
			if (clan.Kingdom != null && clan.Kingdom.Leader == Hero.MainHero && num15 > 0f)
			{
				num15 *= 0.2f;
			}
			return num15;
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00061D64 File Offset: 0x0005FF64
		public override float GetScoreOfClanToLeaveKingdom(Clan clan, Kingdom kingdom)
		{
			int relationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan);
			int num = 0;
			int num2 = 0;
			foreach (Clan clan2 in kingdom.Clans)
			{
				int relationBetweenClans2 = FactionManager.GetRelationBetweenClans(clan, clan2);
				num += relationBetweenClans2;
				num2++;
			}
			float num3 = ((num2 > 0) ? ((float)num / (float)num2) : 0f);
			float num4 = MathF.Max(-100f, MathF.Min(100f, (float)relationBetweenClans + num3));
			float num5 = MathF.Min(2f, MathF.Max(0.33f, 1f + MathF.Sqrt(MathF.Abs(num4)) * ((num4 < 0f) ? (-0.067f) : 0.1f)));
			float num6 = 1f + ((kingdom.Culture == clan.Culture) ? 0.15f : ((kingdom.Leader == Hero.MainHero) ? 0f : (-0.15f)));
			float num7 = clan.CalculateTotalSettlementBaseValue();
			float num8 = clan.CalculateTotalSettlementValueForFaction(kingdom);
			int commanderLimit = clan.CommanderLimit;
			float num9 = 0f;
			if (!clan.IsMinorFaction)
			{
				float num10 = 0f;
				foreach (Town town in kingdom.Fiefs)
				{
					num10 += town.Settlement.GetSettlementValueForFaction(kingdom);
				}
				int num11 = 0;
				foreach (Clan clan3 in kingdom.Clans)
				{
					if (!clan3.IsUnderMercenaryService && clan3 != clan)
					{
						num11 += clan3.CommanderLimit;
					}
				}
				num9 = num10 / (float)(num11 + commanderLimit);
			}
			float num12 = HeroHelper.CalculateReliabilityConstant(clan.Leader, 1f);
			float num13 = (float)(CampaignTime.Now - clan.LastFactionChangeTime).ToDays;
			float num14 = 4000f * (15f - MathF.Sqrt(MathF.Min(225f, num13)));
			int num15 = 0;
			int num16 = 0;
			using (List<Town>.Enumerator enumerator2 = clan.Fiefs.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.IsCastle)
					{
						num16++;
					}
					else
					{
						num15++;
					}
				}
			}
			float num17 = -70000f - (float)num16 * 10000f - (float)num15 * 30000f;
			int gold = clan.Leader.Gold;
			float num18 = 0.5f * (float)MathF.Min(1000000, (clan.Kingdom != null) ? clan.Kingdom.KingdomBudgetWallet : 0) / ((float)clan.Kingdom.Clans.Count + 1f);
			float num19 = 0.15f;
			num17 /= num19;
			float num20 = -num9 * MathF.Sqrt((float)commanderLimit) * num19 * 0.2f + num17 * num12 + -num14;
			num20 *= num5 * num6;
			if (num5 < 1f && num7 - num8 < 0f)
			{
				num20 += num5 * (num7 - num8);
			}
			else
			{
				num20 += num7 - num8;
			}
			if (num5 < 1f)
			{
				num20 += (1f - num5) * 200000f;
			}
			if (kingdom.Leader == Hero.MainHero)
			{
				if (num20 > 0f)
				{
					num20 *= 0.2f;
				}
				else
				{
					num20 *= 5f;
				}
			}
			return num20 + ((kingdom.Leader == Hero.MainHero) ? (-(1000000f * num5)) : 0f);
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00062138 File Offset: 0x00060338
		public override float GetScoreOfKingdomToGetClan(Kingdom kingdom, Clan clan)
		{
			float num = MathF.Min(2f, MathF.Max(0.33f, 1f + 0.02f * (float)FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan)));
			float num2 = 1f + ((kingdom.Culture == clan.Culture) ? 1f : 0f);
			int commanderLimit = clan.CommanderLimit;
			float num3 = (clan.TotalStrength + 150f * (float)commanderLimit) * 20f;
			float powerRatioToEnemies = FactionHelper.GetPowerRatioToEnemies(kingdom);
			float num4 = HeroHelper.CalculateReliabilityConstant(clan.Leader, 1f);
			float num5 = 1f / MathF.Max(0.4f, MathF.Min(2.5f, MathF.Sqrt(powerRatioToEnemies)));
			num3 *= num5;
			return (clan.CalculateTotalSettlementValueForFaction(kingdom) * 0.1f + num3) * num * num2 * num4;
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x0006220C File Offset: 0x0006040C
		public override float GetScoreOfKingdomToSackClan(Kingdom kingdom, Clan clan)
		{
			float num = MathF.Min(2f, MathF.Max(0.33f, 1f + 0.02f * (float)FactionManager.GetRelationBetweenClans(kingdom.RulingClan, clan)));
			float num2 = 1f + ((kingdom.Culture == clan.Culture) ? 1f : 0.5f);
			int commanderLimit = clan.CommanderLimit;
			float num3 = (clan.TotalStrength + 150f * (float)commanderLimit) * 20f;
			float num4 = clan.CalculateTotalSettlementValueForFaction(kingdom);
			return 10f - 1f * num3 * num2 * num - num4;
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x000622A4 File Offset: 0x000604A4
		public override float GetScoreOfMercenaryToJoinKingdom(Clan mercenaryClan, Kingdom kingdom)
		{
			int num = ((mercenaryClan.Kingdom == kingdom) ? mercenaryClan.MercenaryAwardMultiplier : Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(mercenaryClan, kingdom, false));
			float num2 = mercenaryClan.TotalStrength + (float)mercenaryClan.CommanderLimit * 50f;
			int mercenaryAwardFactorToJoinKingdom = Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(mercenaryClan, kingdom, true);
			if (kingdom.Leader == Hero.MainHero)
			{
				return 0f;
			}
			return (float)(num - mercenaryAwardFactorToJoinKingdom) * num2 * 0.5f;
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x00062328 File Offset: 0x00060528
		public override float GetScoreOfMercenaryToLeaveKingdom(Clan mercenaryClan, Kingdom kingdom)
		{
			float num = 0.005f * MathF.Min(200f, mercenaryClan.LastFactionChangeTime.ElapsedDaysUntilNow);
			return 10000f * num - 5000f - this.GetScoreOfMercenaryToJoinKingdom(mercenaryClan, kingdom);
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x0006236C File Offset: 0x0006056C
		public override float GetScoreOfKingdomToHireMercenary(Kingdom kingdom, Clan mercenaryClan)
		{
			int num = 0;
			foreach (Clan clan in kingdom.Clans)
			{
				num += clan.CommanderLimit;
			}
			float num2 = (float)((num < 12) ? ((12 - num) * 100) : 0);
			int count = kingdom.Settlements.Count;
			int num3 = ((count < 40) ? ((40 - count) * 30) : 0);
			return num2 + (float)num3;
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x000623F4 File Offset: 0x000605F4
		public override float GetScoreOfKingdomToSackMercenary(Kingdom kingdom, Clan mercenaryClan)
		{
			float num = (((float)kingdom.Leader.Gold > 20000f) ? (MathF.Sqrt((float)kingdom.Leader.Gold / 20000f) - 1f) : (-1f));
			int relationBetweenClans = FactionManager.GetRelationBetweenClans(kingdom.RulingClan, mercenaryClan);
			float num2 = MathF.Min(5f, FactionHelper.GetPowerRatioToEnemies(kingdom));
			return (MathF.Min(2f + (float)relationBetweenClans / 100f - num2, num) * -1f - 0.1f) * 50f * mercenaryClan.TotalStrength * 5f;
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x0006248C File Offset: 0x0006068C
		public override float GetScoreOfDeclaringPeace(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace, IFaction evaluatingClan, out TextObject peaceReason)
		{
			float num = -this.GetScoreOfWarInternal(factionDeclaresPeace, factionDeclaredPeace, evaluatingClan, true, out peaceReason);
			float num2 = 1f;
			if (num > 0f)
			{
				float num3 = ((factionDeclaredPeace.Leader == Hero.MainHero) ? 0.12f : ((Hero.MainHero.MapFaction == factionDeclaredPeace) ? 0.24f : 0.36f));
				num2 *= num3 + (0.84f - num3) * (100f - factionDeclaredPeace.Aggressiveness) * 0.01f;
			}
			int num4 = ((factionDeclaredPeace.Leader == Hero.MainHero || factionDeclaresPeace.Leader == Hero.MainHero) ? (MathF.Min(Hero.MainHero.Level + 1, 31) * 20) : 0);
			int num5 = -(int)MathF.Min(180000f, (MathF.Min(10000f, factionDeclaresPeace.TotalStrength) + 2000f + (float)num4) * (MathF.Min(10000f, factionDeclaredPeace.TotalStrength) + 2000f + (float)num4) * 0.00018f);
			return (float)((int)(num2 * num) + num5);
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x00062588 File Offset: 0x00060788
		private float GetWarFatiqueScoreNew(IFaction factionDeclaresWar, IFaction factionDeclaredWar, IFaction evaluatingClan)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			foreach (Town town in factionDeclaresWar.Fiefs)
			{
				int num8 = 1;
				if (town.OwnerClan == evaluatingClan || (evaluatingClan.IsKingdomFaction && town.OwnerClan.Leader == evaluatingClan.Leader))
				{
					num8 = 3;
				}
				int num9 = (town.Settlement.IsTown ? 2 : 1);
				num += ((town.Loyalty < 50f) ? ((50f - town.Loyalty) * MathF.Min(6000f, town.Prosperity) * (float)num8 * (float)num9 * 0.00166f) : 0f);
				num2 += (float)num9 * ((town.FoodStocks < 100f) ? ((100f - town.FoodStocks) * (float)num8) : 0f);
				num6 += num8 * num9;
				if (town.GarrisonParty == null)
				{
					num3 += 100f * (float)num8 * (float)num9;
				}
				else
				{
					float totalStrength = town.GarrisonParty.Party.TotalStrength;
					MilitiaPartyComponent militiaPartyComponent = town.Settlement.MilitiaPartyComponent;
					float num10 = totalStrength + ((militiaPartyComponent != null) ? militiaPartyComponent.MobileParty.Party.TotalStrength : 0f);
					num3 += ((num10 < (float)(200 * num9)) ? (0.25f * ((float)(200 * num9) - num10) * (float)num8 * (float)num9) : 0f);
				}
				if (town.IsUnderSiege && town.Settlement.SiegeEvent != null && town.Settlement.SiegeEvent.BesiegerCamp.BesiegerParty.MapFaction == factionDeclaredWar && (MobileParty.MainParty.SiegeEvent == null || MobileParty.MainParty.SiegeEvent.BesiegedSettlement != town.Settlement))
				{
					num7 += 100 * num8 * num9;
				}
				foreach (Village village in town.Villages)
				{
					float num11 = MathF.Max(0f, 400f - village.Hearth) * 0.2f;
					num11 += (float)((village.VillageState == Village.VillageStates.Looted) ? 20 : 0);
					num4 += num11 * (float)num8;
					num5 += num8;
				}
			}
			float num12 = 0f;
			float num13 = 0f;
			int num14 = 0;
			if (factionDeclaresWar.IsKingdomFaction)
			{
				foreach (Clan clan in ((Kingdom)factionDeclaresWar).Clans)
				{
					int num15 = 1;
					if (clan == evaluatingClan || (evaluatingClan.IsKingdomFaction && clan.Leader == evaluatingClan.Leader))
					{
						num15 = 3;
					}
					int partyLimitForTier = Campaign.Current.Models.ClanTierModel.GetPartyLimitForTier(clan, clan.Tier);
					if (partyLimitForTier > clan.WarPartyComponents.Count)
					{
						num12 += 100f * (float)(partyLimitForTier - clan.WarPartyComponents.Count * num15);
					}
					foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
					{
						if (warPartyComponent.MobileParty.PartySizeRatio < 0.9f)
						{
							num12 += 100f * (0.9f - warPartyComponent.MobileParty.PartySizeRatio) * (float)num15;
						}
						if (warPartyComponent.Party.TotalStrength > (float)warPartyComponent.Party.PartySizeLimit)
						{
							num13 += (warPartyComponent.Party.TotalStrength - (float)warPartyComponent.Party.PartySizeLimit) * (float)num15;
						}
					}
					num14 += partyLimitForTier * num15;
				}
			}
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int num19 = 0;
			foreach (StanceLink stanceLink in factionDeclaresWar.Stances)
			{
				if (stanceLink.Faction1 == factionDeclaresWar && stanceLink.Faction2 == factionDeclaredWar)
				{
					num16 = stanceLink.SuccessfulSieges2;
					num17 = stanceLink.SuccessfulRaids2;
					num18 = stanceLink.SuccessfulSieges1;
					num19 = stanceLink.SuccessfulRaids1;
				}
				else if (stanceLink.Faction1 == factionDeclaredWar && stanceLink.Faction2 == factionDeclaresWar)
				{
					num16 = stanceLink.SuccessfulSieges1;
					num17 = stanceLink.SuccessfulRaids1;
					num18 = stanceLink.SuccessfulSieges2;
					num19 = stanceLink.SuccessfulRaids2;
				}
			}
			int num20 = (evaluatingClan.IsKingdomFaction ? 0 : evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Calculating));
			float num21 = 1f + 0.2f * (float)MathF.Min(2, MathF.Max(-2, num20));
			int count = factionDeclaresWar.Fiefs.Count;
			int num22 = factionDeclaresWar.Fiefs.Count * 3;
			float num23 = MathF.Max(0f, (float)num16 - (float)num18 * 0.5f) / ((float)count + 5f) * DefaultDiplomacyModel.HappenedSiegesDifFactor * 100f * num21;
			float num24 = MathF.Max(0f, (float)num17 - (float)num19 * 0.5f) / ((float)num22 + 5f) * DefaultDiplomacyModel.HappenedRaidsDifFactor * 100f * num21;
			float num25 = num12 / (float)(num14 + 2) * DefaultDiplomacyModel.LordRiskValueFactor;
			float num26 = num13 / (float)(num14 + 2) * 0.5f * DefaultDiplomacyModel.LordRiskValueFactor;
			float num27 = num / (float)(num6 + 2) * DefaultDiplomacyModel.LoyalityRiskValueFactor;
			float num28 = num4 / (float)(num5 + 2) * DefaultDiplomacyModel.HearthRiskValueFactor;
			float num29 = num2 / (float)(num6 + 2) * DefaultDiplomacyModel.FoodRiskValueFactor;
			float num30 = num3 / (float)(num6 + 2) * DefaultDiplomacyModel.GarrisonRiskValueFactor;
			float num31 = (float)(num7 / (num6 + 2)) * DefaultDiplomacyModel.SiegeRiskValueFactor;
			return MathF.Min(300000f, num31 + num25 - num26 + num27 + num28 + num29 + num30 + num23 + num24);
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x00062C00 File Offset: 0x00060E00
		private DefaultDiplomacyModel.WarStats CalculateWarStats(IFaction faction, IFaction targetFaction)
		{
			float num = faction.TotalStrength * 0.85f;
			float num2 = 0f;
			int num3 = 0;
			foreach (Town town in faction.Fiefs)
			{
				num3 += (town.IsCastle ? 1 : 2);
			}
			if (faction.IsKingdomFaction)
			{
				foreach (Clan clan in ((Kingdom)faction).Clans)
				{
					if (!clan.IsUnderMercenaryService)
					{
						int partyLimitForTier = Campaign.Current.Models.ClanTierModel.GetPartyLimitForTier(clan, clan.Tier);
						num2 += (float)partyLimitForTier * 80f * ((clan.Leader == clan.MapFaction.Leader) ? 1.25f : 1f);
					}
				}
			}
			num += num2;
			Clan clan2 = (faction.IsClan ? (faction as Clan) : (faction as Kingdom).RulingClan);
			float num4 = faction.Fiefs.Sum((Town f) => (float)(f.IsTown ? 2000 : 1000) + f.Prosperity * 0.33f) * DefaultDiplomacyModel.ProsperityValueFactor;
			float num5 = 0f;
			float num6 = 0f;
			foreach (StanceLink stanceLink in faction.Stances)
			{
				if (stanceLink.IsAtWar && stanceLink.Faction1 != targetFaction && stanceLink.Faction2 != targetFaction && (!stanceLink.Faction2.IsMinorFaction || stanceLink.Faction2.Leader == Hero.MainHero))
				{
					IFaction faction2 = ((stanceLink.Faction1 == faction) ? stanceLink.Faction2 : stanceLink.Faction1);
					if (faction2.IsKingdomFaction)
					{
						foreach (Clan clan3 in ((Kingdom)faction2).Clans)
						{
							if (!clan3.IsUnderMercenaryService)
							{
								num5 += (float)clan3.Tier * 80f * ((clan3.Leader == clan3.MapFaction.Leader) ? 1.5f : 1f);
							}
						}
					}
					num6 += faction2.TotalStrength;
				}
			}
			num6 += num5;
			num *= MathF.Sqrt(MathF.Sqrt((float)MathF.Min(num3 + 4, 40))) / 2.5f;
			return new DefaultDiplomacyModel.WarStats
			{
				RulingClan = clan2,
				Strength = num,
				ValueOfSettlements = num4,
				TotalStrengthOfEnemies = num6
			};
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x00062F34 File Offset: 0x00061134
		[return: TupleElementNames(new string[] { "kingdom1", "kingdom1Score", "kingdom2", "kingdom2Score" })]
		private ValueTuple<Kingdom, float, Kingdom, float> GetTopDogs()
		{
			ValueTuple<Kingdom, Kingdom, Kingdom> valueTuple = MBMath.MaxElements3<Kingdom>(Kingdom.All, (Kingdom k) => k.TotalStrength);
			Kingdom item = valueTuple.Item1;
			Kingdom item2 = valueTuple.Item2;
			Kingdom item3 = valueTuple.Item3;
			float num = ((item != null) ? item.TotalStrength : 400f);
			float num2 = ((item2 != null) ? item2.TotalStrength : 300f);
			float num3 = ((item3 != null) ? item3.TotalStrength : ((item2 != null) ? item2.TotalStrength : 200f));
			float num4 = num / (num3 + 1f);
			float num5 = num2 / (num3 + 1f);
			return new ValueTuple<Kingdom, float, Kingdom, float>(item, num4, item2, num5);
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x00062FE0 File Offset: 0x000611E0
		private float GetTopDogScore(IFaction factionDeclaresWar, IFaction factionDeclaredWar)
		{
			float num = 0f;
			ValueTuple<Kingdom, float, Kingdom, float> topDogs = this.GetTopDogs();
			Kingdom item = topDogs.Item1;
			float item2 = topDogs.Item2;
			Kingdom item3 = topDogs.Item3;
			float item4 = topDogs.Item4;
			if (item == factionDeclaresWar)
			{
				return 0f;
			}
			if (factionDeclaredWar == item)
			{
				num = DefaultDiplomacyModel.StrengthValueFactor * 2f * (factionDeclaresWar.TotalStrength + 1f) * (0.3f * (item2 - 0.9f));
			}
			else if (factionDeclaredWar.IsAtWarWith(item))
			{
				num = -DefaultDiplomacyModel.StrengthValueFactor * 2f * (factionDeclaresWar.TotalStrength + 1f) * (0.2f * (item2 - 0.9f));
			}
			if (factionDeclaredWar == item3)
			{
				num = DefaultDiplomacyModel.StrengthValueFactor * 2f * (factionDeclaresWar.TotalStrength + 1f) * (0.3f * (item4 - 0.9f));
			}
			else if (factionDeclaredWar.IsAtWarWith(item))
			{
				num = -DefaultDiplomacyModel.StrengthValueFactor * 2f * (factionDeclaresWar.TotalStrength + 1f) * (0.2f * (item4 - 0.9f));
			}
			return num;
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x000630E0 File Offset: 0x000612E0
		private float GetBottomScore(IFaction factionDeclaresWar, IFaction factionDeclaredWar)
		{
			float num = 0f;
			ValueTuple<Kingdom, float, Kingdom, float> topDogs = this.GetTopDogs();
			Kingdom item = topDogs.Item1;
			float item2 = topDogs.Item2;
			if (factionDeclaredWar == item)
			{
				num = DefaultDiplomacyModel.StrengthValueFactor * factionDeclaresWar.TotalStrength * (0.2f * item2);
			}
			return num;
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x00063124 File Offset: 0x00061324
		private float CalculateClanRiskScoreOfWar(float squareRootOfPowerRatio, IFaction factionDeclaredWar, IFaction evaluatingClan)
		{
			float num = 0f;
			if (squareRootOfPowerRatio > 0.5f)
			{
				foreach (Town town in evaluatingClan.Fiefs)
				{
					float num2 = Campaign.MapDiagonal * 2f;
					float num3 = Campaign.MapDiagonal * 2f;
					foreach (Town town2 in factionDeclaredWar.Fiefs)
					{
						if (town2.IsTown)
						{
							float length = (town.Settlement.GetPosition2D - town2.Settlement.GetPosition2D).Length;
							if (length < num2)
							{
								num3 = num2;
								num2 = length;
							}
							else if (length < num3)
							{
								num3 = length;
							}
						}
					}
					float num4 = (num2 + num3) / 2f;
					if (num4 < Campaign.AverageDistanceBetweenTwoFortifications * 3f)
					{
						float num5 = MathF.Min(Campaign.AverageDistanceBetweenTwoFortifications * 3f - num4, Campaign.AverageDistanceBetweenTwoFortifications * 2f) / (Campaign.AverageDistanceBetweenTwoFortifications * 2f);
						float num6 = MathF.Min(7.5f, (squareRootOfPowerRatio - 0.5f) * 5f);
						num6 += 0.5f;
						int num7 = (town.IsTown ? 2 : 1);
						num += num5 * num6 * (float)num7 * (2000f + MathF.Min(8000f, town.Prosperity));
					}
				}
			}
			return num;
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x000632DC File Offset: 0x000614DC
		private float GetScoreOfWarInternal(IFaction factionDeclaresWar, IFaction factionDeclaredWar, IFaction evaluatingClan, bool evaluatingPeace, out TextObject reason)
		{
			reason = TextObject.Empty;
			if (factionDeclaresWar.MapFaction == factionDeclaredWar.MapFaction)
			{
				return 0f;
			}
			DefaultDiplomacyModel.WarStats warStats = this.CalculateWarStats(factionDeclaresWar, factionDeclaredWar);
			DefaultDiplomacyModel.WarStats warStats2 = this.CalculateWarStats(factionDeclaredWar, factionDeclaresWar);
			float distance = this.GetDistance(factionDeclaresWar, factionDeclaredWar);
			float num = (483f + 8.63f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			float num2 = ((factionDeclaresWar.Leader == Hero.MainHero || factionDeclaredWar.Leader == Hero.MainHero) ? (-300000f) : (-400000f));
			float num3;
			if (distance - Campaign.AverageDistanceBetweenTwoFortifications * 1.5f > num)
			{
				num3 = num2;
			}
			else if (distance - Campaign.AverageDistanceBetweenTwoFortifications * 1.5f < 0f)
			{
				num3 = 0f;
			}
			else
			{
				float num4 = num - Campaign.AverageDistanceBetweenTwoFortifications * 1.5f;
				float num5 = -num2 / MathF.Pow(num4, 1.6f);
				float num6 = distance - Campaign.AverageDistanceBetweenTwoFortifications * 1.5f;
				num3 = num2 + num5 * MathF.Pow(MathF.Pow(num4 - num6, 8f), 0.2f);
				if (num3 > 0f)
				{
					num3 = 0f;
				}
			}
			float num7 = 1f - MathF.Pow(num3 / num2, 0.55f);
			num7 = 0.1f + num7 * 0.9f;
			float num8 = (evaluatingClan.IsKingdomFaction ? 0f : evaluatingClan.Leader.RandomFloat(-20000f, 20000f));
			int num9 = MathF.Max(-2, MathF.Min(2, evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Valor)));
			float num10 = this.CalculateBenefitScore(ref warStats, ref warStats2, num9, evaluatingPeace, distance, false);
			float num11 = this.CalculateBenefitScore(ref warStats2, ref warStats, num9, evaluatingPeace, distance, true);
			float num12 = 0f;
			float num13 = MathF.Min(2f, MathF.Sqrt((warStats2.Strength + 1000f) / (warStats.Strength + 1000f)));
			if (evaluatingClan.IsKingdomFaction)
			{
				int num14 = 0;
				foreach (Clan clan in ((Kingdom)evaluatingClan).Clans)
				{
					num12 += this.CalculateClanRiskScoreOfWar(num13, factionDeclaredWar, clan);
					num14++;
				}
				if (num14 > 0)
				{
					num12 /= (float)num14;
				}
			}
			else
			{
				num12 = this.CalculateClanRiskScoreOfWar(num13, factionDeclaredWar, evaluatingClan);
			}
			num12 = MathF.Min(200000f, num12);
			float warFatiqueScoreNew = this.GetWarFatiqueScoreNew(factionDeclaresWar, factionDeclaredWar, evaluatingClan);
			float topDogScore = this.GetTopDogScore(factionDeclaresWar, factionDeclaredWar);
			float relationWithClan = (float)warStats.RulingClan.GetRelationWithClan(warStats2.RulingClan);
			int relationWithClan2 = evaluatingClan.Leader.Clan.GetRelationWithClan(warStats2.RulingClan);
			float num15 = (relationWithClan + (float)relationWithClan2) / 2f;
			num10 *= 0.7f + 0.3f * (100f - num15) * 0.01f;
			float num16 = ((num15 < 0f) ? ((factionDeclaresWar.TotalStrength > factionDeclaredWar.TotalStrength * 2f) ? (-500f * num15) : (-500f * (factionDeclaresWar.TotalStrength / (2f * factionDeclaredWar.TotalStrength)) * (factionDeclaresWar.TotalStrength / (2f * factionDeclaredWar.TotalStrength)) * num15)) : 0f);
			num16 *= ((factionDeclaredWar.Leader == Hero.MainHero) ? 1.5f : 1f);
			float num17 = 1f + 0.002f * factionDeclaredWar.Aggressiveness * ((factionDeclaredWar.Leader == Hero.MainHero) ? 1.5f : 1f);
			num10 *= num17;
			if (factionDeclaredWar.Leader == Hero.MainHero && evaluatingPeace)
			{
				num11 /= num17;
			}
			float num18 = 0.3f * MathF.Min(100000f, factionDeclaredWar.Settlements.Sum(delegate(Settlement s)
			{
				if (s.Culture != factionDeclaresWar.Culture)
				{
					return 0f;
				}
				return s.Prosperity * 0.5f * DefaultDiplomacyModel.ProsperityValueFactor;
			}));
			int num19 = 0;
			foreach (Town town in factionDeclaresWar.Fiefs)
			{
				num19 += (town.IsTown ? 2 : 1);
			}
			if (num19 > 0)
			{
			}
			float num20 = 0.1f + 0.9f * MathF.Min(MathF.Min(num10, num11), 100000f) / 100000f;
			float num21 = num10 - num11;
			if (!evaluatingClan.IsKingdomFaction && evaluatingClan.Leader != evaluatingClan.MapFaction.Leader)
			{
				if (num21 > 0f && evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Mercy) != 0)
				{
					num21 *= 1f - 0.1f * (float)MathF.Min(2, MathF.Max(-2, evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Mercy)));
				}
				if (num21 < 0f && evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Valor) != 0)
				{
					num21 *= 1f - 0.1f * (float)MathF.Min(2, MathF.Max(-2, evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Valor)));
				}
			}
			float num22 = 0f;
			if (!evaluatingClan.IsKingdomFaction && warStats.Strength > warStats2.Strength && evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Mercy) > 0)
			{
				num22 -= MathF.Min((warStats.Strength + 500f) / (warStats2.Strength + 500f) - 1f, 2f) * 5000f * (float)evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Mercy);
			}
			if (!evaluatingClan.IsKingdomFaction && warStats.Strength < warStats2.Strength && evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Valor) > 0)
			{
				num22 += MathF.Min((warStats2.Strength + 500f) / (warStats.Strength + 500f) - 1f, 2f) * 5000f * (float)evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Valor);
			}
			float num23 = 0f;
			float num24 = 0f;
			StanceLink stanceWith = factionDeclaresWar.GetStanceWith(factionDeclaredWar);
			int num25 = 0;
			int num26 = 0;
			if (stanceWith.IsAtWar)
			{
				float elapsedDaysUntilNow = stanceWith.WarStartDate.ElapsedDaysUntilNow;
				int num27 = 60;
				float num28 = 5f;
				num23 = ((elapsedDaysUntilNow > (float)num27) ? ((elapsedDaysUntilNow - (float)num27) * -400f) : (((float)num27 - elapsedDaysUntilNow) * num28 * (400f + 0.2f * MathF.Min(6000f, MathF.Min(warStats.Strength, warStats2.Strength)))));
				if (num23 < 0f && !evaluatingClan.IsKingdomFaction)
				{
					int traitLevel = evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Valor);
					if (traitLevel < 0)
					{
						num23 *= 1f - (float)MathF.Max(traitLevel, -2) * 0.25f;
					}
					else if (traitLevel > 0)
					{
						num23 *= 1f - (float)MathF.Min(traitLevel, 2) * 0.175f;
					}
				}
				foreach (Hero hero in factionDeclaresWar.Heroes)
				{
					int num29 = ((hero.Clan == evaluatingClan) ? 3 : 1);
					float num30 = ((hero.Clan.Leader == hero) ? 1.5f : 1f);
					double num31 = ((hero == hero.MapFaction.Leader) ? 1.5 : 1.0);
					if (hero.IsPrisoner && hero.IsLord && hero.PartyBelongedToAsPrisoner != null && hero.PartyBelongedToAsPrisoner.MapFaction == factionDeclaredWar)
					{
						num25 += (int)(num31 * (double)num30 * (double)num29 * 3000.0);
					}
				}
				num26 = num25;
				using (List<Hero>.Enumerator enumerator3 = factionDeclaredWar.Heroes.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Hero hero2 = enumerator3.Current;
						double num32 = ((hero2 == hero2.MapFaction.Leader) ? 1.5 : 1.0);
						float num33 = ((hero2.Clan.Leader == hero2) ? 1.5f : 1f);
						if (hero2.IsPrisoner && hero2.IsLord && hero2.PartyBelongedToAsPrisoner != null && hero2.PartyBelongedToAsPrisoner.MapFaction == factionDeclaresWar)
						{
							num25 -= (int)(num32 * (double)num33 * 2500.0);
						}
					}
					goto IL_9C0;
				}
			}
			float elapsedDaysUntilNow2 = stanceWith.PeaceDeclarationDate.ElapsedDaysUntilNow;
			num24 = ((elapsedDaysUntilNow2 > 60f) ? 0f : ((60f - elapsedDaysUntilNow2) * -(400f + 0.2f * MathF.Min(6000f, MathF.Min(warStats.Strength, warStats2.Strength)))));
			if (num24 < 0f && !evaluatingClan.IsKingdomFaction)
			{
				int traitLevel2 = evaluatingClan.Leader.GetTraitLevel(DefaultTraits.Honor);
				if (traitLevel2 > 0)
				{
					num24 *= 1f + (float)MathF.Min(traitLevel2, 2) * 0.25f;
				}
				else if (traitLevel2 < 0)
				{
					num24 *= 1f + (float)MathF.Max(traitLevel2, -2) * 0.175f;
				}
			}
			IL_9C0:
			int num34 = (factionDeclaresWar.IsKingdomFaction ? (((Kingdom)factionDeclaresWar).PoliticalStagnation * 1000) : 0);
			float num35 = num18 + num8 * num20 + num3 * num20 + num21 + num23 + num24 + (float)num34 * num20 - num12 * num20 + num16 - (float)num25 + num22 * num20;
			float num36 = DefaultDiplomacyModel.StrengthValueFactor * 0.5f * Kingdom.All.Sum(delegate(Kingdom k)
			{
				if (!k.IsAtWarWith(factionDeclaresWar) || !k.IsAtWarWith(factionDeclaredWar) || k.IsMinorFaction)
				{
					return 0f;
				}
				return MathF.Min(k.TotalStrength, factionDeclaredWar.TotalStrength);
			});
			float num37 = topDogScore - num36 - warFatiqueScoreNew;
			num37 *= num20;
			if (evaluatingPeace)
			{
				num35 += (float)((factionDeclaredWar.Leader == Hero.MainHero) ? 10000 : 0);
				if (num35 > 0f)
				{
					num35 += num37;
					if (num35 < 0f)
					{
						num35 *= 0.5f;
					}
				}
				else
				{
					num35 += num37 * 0.75f;
				}
			}
			else
			{
				num35 += num37;
			}
			if (evaluatingPeace)
			{
				float num38 = warFatiqueScoreNew;
				float num39 = -num21;
				float num40 = -num23 * 2f;
				int num41 = num25 * 3 + num26;
				float num42 = -num12 * 3f;
				float num43 = num35 * 0.5f;
				float num44 = -num3 * 0.5f;
				if (num43 > num42 && num43 > (float)num25 && num43 > warFatiqueScoreNew && num43 > num39 && num43 > num40 && num43 > num44)
				{
					reason = new TextObject("{=3JGFdaT7}The {ENEMY_KINGDOM_INFORMAL_NAME} are willing to pay considerable tribute.", null);
				}
				else if (num42 > (float)num41 && num42 > warFatiqueScoreNew && num42 > num39 && num42 > num40 && num42 > num44)
				{
					reason = new TextObject("{=eH0roDGM}Our clan's lands are vulnerable. I owe it to those under my protection to seek peace.", null);
				}
				else if ((float)num41 > num38 && (float)num41 > num39 && (float)num41 > num40 && (float)num25 > num44)
				{
					reason = new TextObject("{=TQmPcVRZ}Too many of our nobles are in captivity. We should make peace to free them.", null);
				}
				else if (num38 >= num39 && num38 >= num40 && warFatiqueScoreNew > num44)
				{
					reason = new TextObject("{=QQtJobYP}We need time to recover from the hardships of war.", null);
				}
				else if (num40 >= num39 && num40 > num44)
				{
					reason = new TextObject("{=lV0VOn99}This war has gone on too long.", null);
				}
				else if (num39 > num44)
				{
					if (warStats.TotalStrengthOfEnemies > 0f && warStats.Strength < warStats.TotalStrengthOfEnemies + warStats2.Strength)
					{
						reason = new TextObject("{=nuqv4GAA}We have too many enemies. We need to make peace with at least some of them.", null);
					}
					else if (warStats.Strength < warStats2.Strength)
					{
						reason = new TextObject("{=JOe3BC41}The {ENEMY_KINGDOM_INFORMAL_NAME} is currently more powerful than us. We need time to build up our strength.", null);
					}
					else if (warStats.ValueOfSettlements > warStats2.ValueOfSettlements)
					{
						reason = new TextObject("{=HqJSNG3M}Our realm is currently doing well, but we stand to lose this wealth if we go on fighting.", null);
					}
					else
					{
						reason = new TextObject("{=vwjs6EjJ}On balance, the gains we stand to make are not worth the costs and risks. ", null);
					}
				}
				else
				{
					reason = new TextObject("{=i0h0LKa0}Our borders are far from those of the enemy. It is too arduous to pursue this war.", null);
				}
				reason.SetTextVariable("ENEMY_KINGDOM_INFORMAL_NAME", factionDeclaredWar.InformalName);
			}
			else
			{
				float num45 = num21;
				int num46 = ((relationWithClan2 < 0) ? (-relationWithClan2 * 1000) : 0);
				int num47 = ((stanceWith.Faction1 == evaluatingClan.MapFaction) ? ((stanceWith.TotalTributePaidby1 != 0) ? stanceWith.TotalTributePaidby1 : (-stanceWith.TotalTributePaidby2)) : ((stanceWith.TotalTributePaidby1 != 0) ? stanceWith.TotalTributePaidby2 : (-stanceWith.TotalTributePaidby1)));
				int num48 = num47 * 70;
				int num49 = num34;
				float num50 = topDogScore;
				float num51 = (100f - factionDeclaredWar.Aggressiveness) * 1000f;
				float num52 = 0f;
				if (factionDeclaredWar.Culture != factionDeclaresWar.Culture)
				{
					int num53 = 0;
					using (List<Town>.Enumerator enumerator2 = factionDeclaredWar.Fiefs.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.Culture == factionDeclaresWar.Culture)
							{
								num53++;
							}
						}
					}
					num52 = MathF.Pow((float)num53, 0.7f) * 30000f;
				}
				if ((float)num48 > num45 && num48 > num46 && num48 > num49 && (float)num48 > num52 && (float)num48 > num50 && (float)num48 > num51)
				{
					if (num47 > 1000)
					{
						reason = new TextObject("{=Kt8tBtBG}We are paying too much tribute to the {ENEMY_KINGDOM_INFORMAL_NAME}.", null);
					}
					else
					{
						reason = new TextObject("{=qI4cicQz}It is a disgrace to keep paying tribute to the  {ENEMY_KINGDOM_INFORMAL_NAME}.", null);
					}
				}
				else if ((float)num46 > num45 && num46 > num49 && (float)num46 > num52 && (float)num46 > num50 && (float)num46 > num51)
				{
					reason = new TextObject("{=dov3iRlt}{ENEMY_RULER.NAME} of the {ENEMY_KINGDOM_INFORMAL_NAME} is vile and dangerous. We must deal with {?ENEMY_RULER.GENDER}her{?}him{\\?} before it is too late.", null);
				}
				else if (num45 > (float)num49 && num45 > num52 && num45 > num50)
				{
					if (warStats.TotalStrengthOfEnemies == 0f && warStats.Strength < warStats2.Strength)
					{
						reason = new TextObject("{=1aQAmENB}The  {ENEMY_KINGDOM_INFORMAL_NAME} may be strong but their lands are rich and ripe for the taking.", null);
					}
					else if (warStats.Strength > warStats2.Strength)
					{
						reason = new TextObject("{=az3K3j4C}Right now we are stronger than the {ENEMY_KINGDOM_INFORMAL_NAME}. We should strike while we can.", null);
					}
				}
				else if ((float)num49 > num52 && (float)num49 > num50 && (float)num49 > num51)
				{
					reason = new TextObject("{=pmg9KCqf}We have been at peace too long. Our men grow restless.", null);
				}
				else if (num52 > num50 && num52 > num51)
				{
					reason = new TextObject("{=79lEPn1u}The {ENEMY_KINGDOM_INFORMAL_NAME} have occupied our ancestral lands and they oppress our kinfolk.", null);
				}
				else if (num51 > num50)
				{
					reason = new TextObject("{=bHf8aMtt}The {ENEMY_KINGDOM_INFORMAL_NAME} have been acting aggressively. We should teach them a lesson.", null);
				}
				else
				{
					reason = new TextObject("{=gsmmoKNd}The {ENEMY_KINGDOM_INFORMAL_NAME} will devour all of Calradia if we do not stop them.", null);
				}
				reason.SetTextVariable("ENEMY_KINGDOM_INFORMAL_NAME", factionDeclaredWar.InformalName);
				reason.SetCharacterProperties("ENEMY_RULER", factionDeclaredWar.Leader.CharacterObject, false);
			}
			return num7 * num35;
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x00064240 File Offset: 0x00062440
		private float CalculateBenefitScore(ref DefaultDiplomacyModel.WarStats faction1Stats, ref DefaultDiplomacyModel.WarStats faction2Stats, int valorLevelOfEvaluatingClan, bool evaluatingPeace, float distanceToClosestEnemyFief, bool calculatingRisk = false)
		{
			float valueOfSettlements = faction2Stats.ValueOfSettlements;
			float num = MathF.Clamp((valueOfSettlements > DefaultDiplomacyModel._MeaningfulValue) ? ((valueOfSettlements - DefaultDiplomacyModel._MeaningfulValue) * 0.5f + DefaultDiplomacyModel._MinValue + DefaultDiplomacyModel._MeaningfulValue) : (valueOfSettlements + DefaultDiplomacyModel._MinValue), DefaultDiplomacyModel._MinValue, DefaultDiplomacyModel._MaxValue);
			float num2 = 100f;
			float num3 = (faction2Stats.Strength + num2) / (faction1Stats.Strength + num2);
			float num4 = 0f;
			float num5 = ((num3 > 1f) ? num3 : (1f / num3));
			if (num5 > 3f)
			{
				num4 = MathF.Min(0.4f, (num5 / 3f - 1f) * 0.1f);
			}
			float num6 = MathF.Pow(num3, 1.1f + num4);
			if (!calculatingRisk)
			{
				float num7 = MathF.Min(1f, (MathF.Min(MathF.Max(faction2Stats.Strength, 10000f), faction1Stats.Strength) + num2) / (faction2Stats.Strength + faction1Stats.TotalStrengthOfEnemies + num2));
				num6 /= MathF.Pow(num7, (0.5f - (float)valorLevelOfEvaluatingClan * 0.1f) * (evaluatingPeace ? 1.1f : 1f));
			}
			else
			{
				float num8 = MathF.Min(1f, (MathF.Min(MathF.Max(faction1Stats.Strength, 10000f), faction2Stats.Strength) + num2) / (faction1Stats.Strength + faction2Stats.TotalStrengthOfEnemies + num2));
				num6 *= MathF.Pow(num8, (0.4f - (float)valorLevelOfEvaluatingClan * 0.1f) * (evaluatingPeace ? 1.1f : 1f));
			}
			float num9 = 1f / (1f + num6);
			num9 = MathF.Clamp(num9, 0.01f, 0.99f);
			float num10 = num * num9;
			float num11 = Campaign.AverageDistanceBetweenTwoFortifications * 3f / (Campaign.AverageDistanceBetweenTwoFortifications * 3f + distanceToClosestEnemyFief + distanceToClosestEnemyFief * 0.25f);
			return num10 * MathF.Max(0.25f, num11);
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0006441C File Offset: 0x0006261C
		private ValueTuple<Settlement, float>[] GetClosestSettlementsToOtherFactionsNearestSettlementToMidPoint(IFaction faction1, IFaction faction2)
		{
			Settlement settlement = null;
			float num = float.MaxValue;
			foreach (Town town in faction1.Fiefs)
			{
				float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(town.Settlement, faction1.FactionMidSettlement);
				if (num > distance)
				{
					settlement = town.Settlement;
					num = distance;
				}
			}
			ValueTuple<Settlement, float>[] array = new ValueTuple<Settlement, float>[]
			{
				new ValueTuple<Settlement, float>(null, float.MaxValue),
				new ValueTuple<Settlement, float>(null, float.MaxValue),
				new ValueTuple<Settlement, float>(null, float.MaxValue)
			};
			foreach (Town town2 in faction2.Fiefs)
			{
				float distance2 = Campaign.Current.Models.MapDistanceModel.GetDistance(town2.Settlement, settlement);
				if (distance2 < array[2].Item2)
				{
					if (distance2 < array[1].Item2)
					{
						if (distance2 < array[0].Item2)
						{
							array[2] = array[1];
							array[1] = array[0];
							array[0].Item1 = town2.Settlement;
							array[0].Item2 = distance2;
						}
						else
						{
							array[2] = array[1];
							array[1].Item1 = town2.Settlement;
							array[1].Item2 = distance2;
						}
					}
					else
					{
						array[2].Item1 = town2.Settlement;
						array[2].Item2 = distance2;
					}
				}
			}
			return array;
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x00064624 File Offset: 0x00062824
		private float GetDistance(IFaction factionDeclaresWar, IFaction factionDeclaredWar)
		{
			if (factionDeclaresWar.Fiefs.Count != 0 && factionDeclaredWar.Fiefs.Count != 0)
			{
				ValueTuple<Settlement, float>[] closestSettlementsToOtherFactionsNearestSettlementToMidPoint = this.GetClosestSettlementsToOtherFactionsNearestSettlementToMidPoint(factionDeclaredWar, factionDeclaresWar);
				ValueTuple<Settlement, float>[] closestSettlementsToOtherFactionsNearestSettlementToMidPoint2 = this.GetClosestSettlementsToOtherFactionsNearestSettlementToMidPoint(factionDeclaresWar, factionDeclaredWar);
				float[] array = new float[] { float.MaxValue, float.MaxValue, float.MaxValue };
				foreach (ValueTuple<Settlement, float> valueTuple in closestSettlementsToOtherFactionsNearestSettlementToMidPoint)
				{
					if (valueTuple.Item1 != null)
					{
						foreach (ValueTuple<Settlement, float> valueTuple2 in closestSettlementsToOtherFactionsNearestSettlementToMidPoint2)
						{
							if (valueTuple2.Item1 != null)
							{
								float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(valueTuple.Item1, valueTuple2.Item1);
								if (distance < array[2])
								{
									if (distance < array[1])
									{
										if (distance < array[0])
										{
											array[2] = array[1];
											array[1] = array[0];
											array[0] = distance;
										}
										else
										{
											array[2] = array[1];
											array[1] = distance;
										}
									}
									else
									{
										array[2] = distance;
									}
								}
							}
						}
					}
				}
				float num = array[0];
				float num2 = ((array[1] < float.MaxValue) ? array[1] : num) * 0.67f;
				float num3 = ((array[2] < float.MaxValue) ? array[2] : ((num2 < float.MaxValue) ? num2 : num)) * 0.33f;
				return (num + num2 + num3) / 2f;
			}
			if (factionDeclaresWar.Leader == Hero.MainHero || factionDeclaredWar.Leader == Hero.MainHero)
			{
				return 100f;
			}
			return 0.4f * (factionDeclaresWar.InitialPosition - factionDeclaredWar.InitialPosition).Length;
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x000647BC File Offset: 0x000629BC
		public override float GetScoreOfDeclaringWar(IFaction factionDeclaresWar, IFaction factionDeclaredWar, IFaction evaluatingClan, out TextObject warReason)
		{
			float scoreOfWarInternal = this.GetScoreOfWarInternal(factionDeclaresWar, factionDeclaredWar, evaluatingClan, false, out warReason);
			StanceLink stanceWith = factionDeclaresWar.GetStanceWith(factionDeclaredWar);
			int num = 0;
			if (stanceWith.IsNeutral)
			{
				int dailyTributePaid = stanceWith.GetDailyTributePaid(factionDeclaredWar);
				float num2 = (float)evaluatingClan.Leader.Gold + (evaluatingClan.MapFaction.IsKingdomFaction ? (0.5f * ((float)((Kingdom)evaluatingClan.MapFaction).KingdomBudgetWallet / ((float)((Kingdom)evaluatingClan.MapFaction).Clans.Count + 1f))) : 0f);
				float num3 = ((!evaluatingClan.IsKingdomFaction && evaluatingClan.Leader != null) ? ((num2 < 50000f) ? (1f + 0.5f * ((50000f - num2) / 50000f)) : ((num2 > 200000f) ? MathF.Max(0.5f, MathF.Sqrt(200000f / num2)) : 1f)) : 1f);
				num = this.GetValueOfDailyTribute(dailyTributePaid);
				num = (int)((float)num * num3);
			}
			int num4 = -(int)MathF.Min(120000f, (MathF.Min(10000f, factionDeclaresWar.TotalStrength) * 0.8f + 2000f) * (MathF.Min(10000f, factionDeclaredWar.TotalStrength) * 0.8f + 2000f) * 0.0012f);
			return scoreOfWarInternal + (float)num4 - (float)num;
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x00064910 File Offset: 0x00062B10
		private static int GetWarFatiqueScore(IFaction factionDeclaresWar, IFaction factionDeclaredWar)
		{
			int num = 0;
			StanceLink stanceWith = factionDeclaresWar.GetStanceWith(factionDeclaredWar);
			float num2 = (float)(CampaignTime.Now - stanceWith.WarStartDate).ToDays;
			float num3 = ((factionDeclaresWar.IsMinorFaction && factionDeclaresWar != MobileParty.MainParty.MapFaction) ? 40f : 60f);
			if (num2 < num3)
			{
				int num4 = (((factionDeclaredWar == MobileParty.MainParty.MapFaction && !factionDeclaresWar.IsMinorFaction) || (factionDeclaresWar == MobileParty.MainParty.MapFaction && !factionDeclaredWar.IsMinorFaction)) ? 2 : 1);
				float num5 = ((factionDeclaresWar.IsMinorFaction && factionDeclaresWar != MobileParty.MainParty.MapFaction) ? 1000f : 2000f);
				num = (int)(MathF.Pow(num3 - num2, 1.3f) * num5 * (float)num4);
			}
			return num + 60000;
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x000649DC File Offset: 0x00062BDC
		public override float GetScoreOfLettingPartyGo(MobileParty party, MobileParty partyToLetGo)
		{
			float num = 0f;
			BattleSideEnum battleSideEnum = BattleSideEnum.Attacker;
			if (battleSideEnum == BattleSideEnum.Attacker)
			{
				num = 0.98f;
			}
			float num2 = 0f;
			for (int i = 0; i < partyToLetGo.ItemRoster.Count; i++)
			{
				ItemRosterElement elementCopyAtIndex = partyToLetGo.ItemRoster.GetElementCopyAtIndex(i);
				num2 += (float)(elementCopyAtIndex.Amount * elementCopyAtIndex.EquipmentElement.GetBaseValue());
			}
			float num3 = 0f;
			for (int j = 0; j < party.ItemRoster.Count; j++)
			{
				ItemRosterElement elementCopyAtIndex2 = party.ItemRoster.GetElementCopyAtIndex(j);
				num3 += (float)(elementCopyAtIndex2.Amount * elementCopyAtIndex2.EquipmentElement.GetBaseValue());
			}
			float num4 = 0f;
			foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
			{
				num4 += MathF.Min(1000f, 10f * (float)troopRosterElement.Character.Level * MathF.Sqrt((float)troopRosterElement.Character.Level));
			}
			float num5 = 0f;
			foreach (TroopRosterElement troopRosterElement2 in partyToLetGo.MemberRoster.GetTroopRoster())
			{
				num5 += MathF.Min(1000f, 10f * (float)troopRosterElement2.Character.Level * MathF.Sqrt((float)troopRosterElement2.Character.Level));
			}
			float num6 = 0f;
			foreach (TroopRosterElement troopRosterElement3 in ((battleSideEnum == BattleSideEnum.Attacker) ? partyToLetGo : party).MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement3.Character.IsHero)
				{
					num6 += 500f;
				}
				num6 += (float)Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(troopRosterElement3.Character, (battleSideEnum == BattleSideEnum.Attacker) ? partyToLetGo.LeaderHero : party.LeaderHero) * 0.3f;
			}
			float num7 = (party.IsPartyTradeActive ? ((float)party.PartyTradeGold) : 0f);
			num7 += ((party.LeaderHero != null) ? ((float)party.LeaderHero.Gold * 0.15f) : 0f);
			float num8 = (partyToLetGo.IsPartyTradeActive ? ((float)partyToLetGo.PartyTradeGold) : 0f);
			num7 += ((partyToLetGo.LeaderHero != null) ? ((float)partyToLetGo.LeaderHero.Gold * 0.15f) : 0f);
			float num9 = num5 + 10000f;
			if (partyToLetGo.BesiegedSettlement != null)
			{
				num9 += 20000f;
			}
			return -1000f + (1f - num) * num4 - num * num9 - num * num8 + (1f - num) * num7 + num * num6 + (num3 * (1f - num) - num * num2);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x00064D04 File Offset: 0x00062F04
		public override float GetValueOfHeroForFaction(Hero examinedHero, IFaction targetFaction, bool forMarriage = false)
		{
			return this.GetHeroCommandingStrengthForClan(examinedHero) * 10f;
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x00064D13 File Offset: 0x00062F13
		public override int GetRelationCostOfExpellingClanFromKingdom()
		{
			return -20;
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x00064D17 File Offset: 0x00062F17
		public override int GetInfluenceCostOfSupportingClan()
		{
			return 50;
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x00064D1B File Offset: 0x00062F1B
		public override int GetInfluenceCostOfExpellingClan()
		{
			return 200;
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00064D24 File Offset: 0x00062F24
		public override int GetInfluenceCostOfProposingPeace()
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(100f, false, null);
			this.GetPerkEffectsOnKingdomDecisionInfluenceCost(ref explainedNumber);
			return MathF.Round(explainedNumber.ResultNumber);
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00064D54 File Offset: 0x00062F54
		public override int GetInfluenceCostOfProposingWar(Kingdom proposingKingdom)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(100f, false, null);
			float num = 1f;
			if (proposingKingdom.ActivePolicies.Contains(DefaultPolicies.WarTax))
			{
				num = 2f;
			}
			explainedNumber.AddFactor(num, null);
			this.GetPerkEffectsOnKingdomDecisionInfluenceCost(ref explainedNumber);
			return MathF.Round(explainedNumber.ResultNumber);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x00064DAA File Offset: 0x00062FAA
		public override int GetInfluenceValueOfSupportingClan()
		{
			return this.GetInfluenceCostOfSupportingClan() / 4;
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x00064DB4 File Offset: 0x00062FB4
		public override int GetRelationValueOfSupportingClan()
		{
			return 1;
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x00064DB8 File Offset: 0x00062FB8
		public override int GetInfluenceCostOfAnnexation(Kingdom proposingKingdom)
		{
			float num = 1f;
			if (proposingKingdom != null)
			{
				if (proposingKingdom.ActivePolicies.Contains(DefaultPolicies.FeudalInheritance))
				{
					num *= 2f;
				}
				if (proposingKingdom.ActivePolicies.Contains(DefaultPolicies.PrecarialLandTenure))
				{
					num *= 0.5f;
				}
			}
			return (int)(200f * num);
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x00064E0A File Offset: 0x0006300A
		public override int GetInfluenceCostOfChangingLeaderOfArmy()
		{
			return 30;
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x00064E10 File Offset: 0x00063010
		public override int GetInfluenceCostOfDisbandingArmy()
		{
			int num = 30;
			if (Clan.PlayerClan.Kingdom != null && Clan.PlayerClan == Clan.PlayerClan.Kingdom.RulingClan)
			{
				num /= 2;
			}
			return num;
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x00064E47 File Offset: 0x00063047
		public override int GetRelationCostOfDisbandingArmy(bool isLeaderParty)
		{
			if (!isLeaderParty)
			{
				return -1;
			}
			return -4;
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x00064E50 File Offset: 0x00063050
		public override int GetInfluenceCostOfPolicyProposalAndDisavowal()
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(100f, false, null);
			this.GetPerkEffectsOnKingdomDecisionInfluenceCost(ref explainedNumber);
			return MathF.Round(explainedNumber.ResultNumber);
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x00064E7F File Offset: 0x0006307F
		public override int GetInfluenceCostOfAbandoningArmy()
		{
			return 2;
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00064E82 File Offset: 0x00063082
		private void GetPerkEffectsOnKingdomDecisionInfluenceCost(ref ExplainedNumber cost)
		{
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Charm.Firebrand))
			{
				cost.AddFactor(DefaultPerks.Charm.Firebrand.PrimaryBonus, DefaultPerks.Charm.Firebrand.Name);
			}
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00064EAF File Offset: 0x000630AF
		private int GetBaseRelationBetweenHeroes(Hero hero1, Hero hero2)
		{
			return CharacterRelationManager.GetHeroRelation(hero1, hero2);
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x00064EB8 File Offset: 0x000630B8
		public override int GetBaseRelation(Hero hero1, Hero hero2)
		{
			return this.GetBaseRelationBetweenHeroes(hero1, hero2);
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x00064EC4 File Offset: 0x000630C4
		public override int GetEffectiveRelation(Hero hero1, Hero hero2)
		{
			Hero hero3;
			Hero hero4;
			this.GetHeroesForEffectiveRelation(hero1, hero2, out hero3, out hero4);
			if (hero3 == null || hero4 == null)
			{
				return 0;
			}
			int baseRelationBetweenHeroes = this.GetBaseRelationBetweenHeroes(hero3, hero4);
			this.GetPersonalityEffects(ref baseRelationBetweenHeroes, hero1, hero4);
			return MBMath.ClampInt(baseRelationBetweenHeroes, this.MinRelationLimit, this.MaxRelationLimit);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x00064F0C File Offset: 0x0006310C
		public override void GetHeroesForEffectiveRelation(Hero hero1, Hero hero2, out Hero effectiveHero1, out Hero effectiveHero2)
		{
			Clan neutralFaction = CampaignData.NeutralFaction;
			effectiveHero1 = ((hero1.Clan != null && hero1.Clan != neutralFaction) ? hero1.Clan.Leader : hero1);
			effectiveHero2 = ((hero2.Clan != null && hero2.Clan != neutralFaction) ? hero2.Clan.Leader : hero2);
			if (effectiveHero1 == effectiveHero2 || (hero1.IsPlayerCompanion && hero2.IsHumanPlayerCharacter) || (hero1.IsPlayerCompanion && hero2.IsHumanPlayerCharacter))
			{
				effectiveHero1 = hero1;
				effectiveHero2 = hero2;
			}
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x00064F90 File Offset: 0x00063190
		public override int GetRelationChangeAfterClanLeaderIsDead(Hero deadLeader, Hero relationHero)
		{
			return (int)((float)CharacterRelationManager.GetHeroRelation(deadLeader, relationHero) * 0.7f);
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x00064FA4 File Offset: 0x000631A4
		public override int GetRelationChangeAfterVotingInSettlementOwnerPreliminaryDecision(Hero supporter, bool hasHeroVotedAgainstOwner)
		{
			int num;
			if (hasHeroVotedAgainstOwner)
			{
				num = -20;
				if (supporter.Culture.HasFeat(DefaultCulturalFeats.SturgianDecisionPenaltyFeat))
				{
					num += (int)((float)num * DefaultCulturalFeats.SturgianDecisionPenaltyFeat.EffectBonus);
				}
			}
			else
			{
				num = 5;
			}
			return num;
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x00064FDF File Offset: 0x000631DF
		private void GetPersonalityEffects(ref int effectiveRelation, Hero hero1, Hero effectiveHero2)
		{
			this.GetTraitEffect(ref effectiveRelation, hero1, effectiveHero2, DefaultTraits.Honor, 2);
			this.GetTraitEffect(ref effectiveRelation, hero1, effectiveHero2, DefaultTraits.Valor, 1);
			this.GetTraitEffect(ref effectiveRelation, hero1, effectiveHero2, DefaultTraits.Mercy, 1);
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x00065010 File Offset: 0x00063210
		private void GetTraitEffect(ref int effectiveRelation, Hero hero1, Hero effectiveHero2, TraitObject trait, int effectMagnitude)
		{
			int traitLevel = hero1.GetTraitLevel(trait);
			int traitLevel2 = effectiveHero2.GetTraitLevel(trait);
			int num = traitLevel * traitLevel2;
			if (num > 0)
			{
				effectiveRelation += effectMagnitude;
				return;
			}
			if (num < 0)
			{
				effectiveRelation -= effectMagnitude;
			}
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x00065048 File Offset: 0x00063248
		public override int GetCharmExperienceFromRelationGain(Hero hero, float relationChange, ChangeRelationAction.ChangeRelationDetail detail)
		{
			float num = 20f;
			if (detail != ChangeRelationAction.ChangeRelationDetail.Emissary)
			{
				if (!hero.IsNotable)
				{
					if (hero.MapFaction != null && hero.MapFaction.Leader == hero)
					{
						num *= 30f;
					}
					else if (hero.Clan != null && hero.Clan.Leader == hero)
					{
						num *= 20f;
					}
				}
			}
			else if (!hero.IsNotable)
			{
				if (hero.MapFaction != null && hero.MapFaction.Leader == hero)
				{
					num *= 30f;
				}
				else if (hero.Clan != null && hero.Clan.Leader == hero)
				{
					num *= 20f;
				}
				else
				{
					num *= 10f;
				}
			}
			else
			{
				num *= 20f;
			}
			return MathF.Round(num * relationChange);
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x0006510C File Offset: 0x0006330C
		public override uint GetNotificationColor(ChatNotificationType notificationType)
		{
			switch (notificationType)
			{
			case ChatNotificationType.Default:
				return 10066329U;
			case ChatNotificationType.PlayerFactionPositive:
				return 2284902U;
			case ChatNotificationType.PlayerClanPositive:
				return 3407803U;
			case ChatNotificationType.PlayerFactionNegative:
				return 14509602U;
			case ChatNotificationType.PlayerClanNegative:
				return 16750899U;
			case ChatNotificationType.Civilian:
				return 10053324U;
			case ChatNotificationType.PlayerClanCivilian:
				return 15623935U;
			case ChatNotificationType.PlayerFactionCivilian:
				return 11163101U;
			case ChatNotificationType.Neutral:
				return 12303291U;
			case ChatNotificationType.PlayerFactionIndirectPositive:
				return 12298820U;
			case ChatNotificationType.PlayerFactionIndirectNegative:
				return 13382502U;
			case ChatNotificationType.PlayerClanPolitical:
				return 6745855U;
			case ChatNotificationType.PlayerFactionPolitical:
				return 5614301U;
			case ChatNotificationType.Political:
				return 6724044U;
			default:
				return 13369548U;
			}
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x000651B2 File Offset: 0x000633B2
		public override float DenarsToInfluence()
		{
			return 0.002f;
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x000651B9 File Offset: 0x000633B9
		public override bool CanSettlementBeGifted(Settlement settlementToGift)
		{
			return settlementToGift.Town != null && !settlementToGift.Town.IsOwnerUnassigned;
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x000651D3 File Offset: 0x000633D3
		public override IEnumerable<BarterGroup> GetBarterGroups()
		{
			return new BarterGroup[]
			{
				new GoldBarterGroup(),
				new ItemBarterGroup(),
				new PrisonerBarterGroup(),
				new FiefBarterGroup(),
				new OtherBarterGroup(),
				new DefaultsBarterGroup()
			};
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x0006520B File Offset: 0x0006340B
		public override int GetValueOfDailyTribute(int dailyTributeAmount)
		{
			return dailyTributeAmount * 70;
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x00065211 File Offset: 0x00063411
		public override int GetDailyTributeForValue(int value)
		{
			return value / 70 / 10 * 10;
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x0006521D File Offset: 0x0006341D
		public override bool IsClanEligibleToBecomeRuler(Clan clan)
		{
			return !clan.IsEliminated && clan.Leader.IsAlive && !clan.IsUnderMercenaryService;
		}

		// Token: 0x04000784 RID: 1924
		private const int DailyValueFactorForTributes = 70;

		// Token: 0x04000785 RID: 1925
		private static float HearthRiskValueFactor = 500f;

		// Token: 0x04000786 RID: 1926
		private static float LordRiskValueFactor = 1000f;

		// Token: 0x04000787 RID: 1927
		private static float FoodRiskValueFactor = 750f;

		// Token: 0x04000788 RID: 1928
		private static float GarrisonRiskValueFactor = 2000f;

		// Token: 0x04000789 RID: 1929
		private static float SiegeRiskValueFactor = 3000f;

		// Token: 0x0400078A RID: 1930
		private static float LoyalityRiskValueFactor = 500f;

		// Token: 0x0400078B RID: 1931
		private static float ProsperityValueFactor = 50f;

		// Token: 0x0400078C RID: 1932
		private static float HappenedSiegesDifFactor = 1500f;

		// Token: 0x0400078D RID: 1933
		private static float HappenedRaidsDifFactor = 500f;

		// Token: 0x0400078E RID: 1934
		private static float StrengthValueFactor = 100f;

		// Token: 0x0400078F RID: 1935
		private static TextObject _personalityEffectText = new TextObject("{=HDBryERe}Personalities", null);

		// Token: 0x04000790 RID: 1936
		private const float strengthFactor = 50f;

		// Token: 0x04000791 RID: 1937
		private static float _MaxValue = 10000000f;

		// Token: 0x04000792 RID: 1938
		private static float _MeaningfulValue = 2000000f;

		// Token: 0x04000793 RID: 1939
		private static float _MinValue = 10000f;

		// Token: 0x02000504 RID: 1284
		private struct WarStats
		{
			// Token: 0x04001596 RID: 5526
			public Clan RulingClan;

			// Token: 0x04001597 RID: 5527
			public float Strength;

			// Token: 0x04001598 RID: 5528
			public float ValueOfSettlements;

			// Token: 0x04001599 RID: 5529
			public float TotalStrengthOfEnemies;
		}
	}
}
