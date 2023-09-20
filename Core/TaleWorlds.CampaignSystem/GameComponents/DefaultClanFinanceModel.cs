using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000FA RID: 250
	public class DefaultClanFinanceModel : ClanFinanceModel
	{
		// Token: 0x060014C5 RID: 5317 RVA: 0x0005D1F4 File Offset: 0x0005B3F4
		public override ExplainedNumber CalculateClanGoldChange(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateClanIncomeInternal(clan, ref explainedNumber, applyWithdrawals, includeDetails);
			this.CalculateClanExpensesInternal(clan, ref explainedNumber, applyWithdrawals, includeDetails);
			return explainedNumber;
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x0005D228 File Offset: 0x0005B428
		public override ExplainedNumber CalculateClanIncome(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateClanIncomeInternal(clan, ref explainedNumber, applyWithdrawals, includeDetails);
			return explainedNumber;
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x0005D250 File Offset: 0x0005B450
		private void CalculateClanIncomeInternal(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals = false, bool includeDetails = false)
		{
			if (clan.IsEliminated)
			{
				return;
			}
			Kingdom kingdom = clan.Kingdom;
			if (((kingdom != null) ? kingdom.RulingClan : null) == clan)
			{
				this.AddRulingClanIncome(clan, ref goldChange, applyWithdrawals, includeDetails);
			}
			if (clan != Clan.PlayerClan && (!clan.MapFaction.IsKingdomFaction || clan.IsUnderMercenaryService) && clan.Fiefs.Count == 0)
			{
				int num = clan.Tier * (80 + (clan.IsUnderMercenaryService ? 40 : 0));
				goldChange.Add((float)num, null, null);
			}
			this.AddMercenaryIncome(clan, ref goldChange, applyWithdrawals);
			this.AddSettlementIncome(clan, ref goldChange, applyWithdrawals, includeDetails);
			this.CalculateHeroIncomeFromWorkshops(clan.Leader, ref goldChange, applyWithdrawals);
			this.AddIncomeFromParties(clan, ref goldChange, applyWithdrawals, includeDetails);
			if (clan == Clan.PlayerClan)
			{
				this.AddPlayerClanIncomeFromOwnedAlleys(ref goldChange);
			}
			if (!clan.IsUnderMercenaryService)
			{
				this.AddIncomeFromTribute(clan, ref goldChange, applyWithdrawals, includeDetails);
			}
			if (clan.Gold < 30000 && clan.Kingdom != null && clan.Leader != Hero.MainHero && !clan.IsUnderMercenaryService)
			{
				this.AddIncomeFromKingdomBudget(clan, ref goldChange, applyWithdrawals);
			}
			Hero leader = clan.Leader;
			if (leader != null && leader.GetPerkValue(DefaultPerks.Trade.SpringOfGold))
			{
				int num2 = MathF.Min(1000, MathF.Round((float)clan.Leader.Gold * DefaultPerks.Trade.SpringOfGold.PrimaryBonus));
				goldChange.Add((float)num2, DefaultPerks.Trade.SpringOfGold.Name, null);
			}
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x0005D3A8 File Offset: 0x0005B5A8
		public void CalculateClanExpensesInternal(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals = false, bool includeDetails = false)
		{
			this.AddExpensesFromPartiesAndGarrisons(clan, ref goldChange, applyWithdrawals, includeDetails);
			if (!clan.IsUnderMercenaryService)
			{
				this.AddExpensesForHiredMercenaries(clan, ref goldChange, applyWithdrawals);
				this.AddExpensesForTributes(clan, ref goldChange, applyWithdrawals);
			}
			this.AddExpensesForAutoRecruitment(clan, ref goldChange, applyWithdrawals);
			if (clan.Gold > 100000 && clan.Kingdom != null && clan.Leader != Hero.MainHero && !clan.IsUnderMercenaryService)
			{
				int num = (int)(((float)clan.Gold - 100000f) * 0.01f);
				if (applyWithdrawals)
				{
					clan.Kingdom.KingdomBudgetWallet += num;
				}
				goldChange.Add((float)(-(float)num), DefaultClanFinanceModel._kingdomBudgetStr, null);
			}
			if (clan.DebtToKingdom > 0)
			{
				this.AddPaymentForDebts(clan, ref goldChange, applyWithdrawals);
			}
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x0005D45C File Offset: 0x0005B65C
		public override ExplainedNumber CalculateClanExpenses(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateClanExpensesInternal(clan, ref explainedNumber, applyWithdrawals, includeDetails);
			return explainedNumber;
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x0005D484 File Offset: 0x0005B684
		private void AddPaymentForDebts(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			if (clan.Kingdom != null && clan.DebtToKingdom > 0)
			{
				int num = clan.DebtToKingdom;
				if (applyWithdrawals)
				{
					num = MathF.Min(num, (int)((float)clan.Gold + goldChange.ResultNumber));
					clan.DebtToKingdom -= num;
				}
				goldChange.Add((float)(-(float)num), DefaultClanFinanceModel._debtStr, null);
			}
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x0005D4E0 File Offset: 0x0005B6E0
		private void AddRulingClanIncome(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals, bool includeDetails)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, goldChange.IncludeDescriptions, null);
			int num = 0;
			int num2 = 0;
			bool flag = clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.LandTax);
			float num3 = 0f;
			foreach (Town town in clan.Fiefs)
			{
				num += (int)Campaign.Current.Models.SettlementTaxModel.CalculateTownTax(town, false).ResultNumber;
				num2++;
				if (flag)
				{
					foreach (Village village in town.Villages)
					{
						if (!village.IsOwnerUnassigned && village.Settlement.OwnerClan != clan)
						{
							int num4 = ((village.VillageState == Village.VillageStates.Looted || village.VillageState == Village.VillageStates.BeingRaided) ? 0 : ((int)((float)village.TradeTaxAccumulated / this.RevenueSmoothenFraction())));
							num3 += (float)num4 * 0.05f;
						}
					}
				}
			}
			if (flag && !num3.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				explainedNumber.Add(num3, DefaultPolicies.LandTax.Name, null);
			}
			Kingdom kingdom = clan.Kingdom;
			if (kingdom.RulingClan == clan)
			{
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.WarTax))
				{
					int num5 = (int)((float)num * 0.05f);
					explainedNumber.Add((float)num5, DefaultPolicies.WarTax.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.DebasementOfTheCurrency))
				{
					explainedNumber.Add((float)(num2 * 100), DefaultPolicies.DebasementOfTheCurrency.Name, null);
				}
			}
			int num6 = 0;
			int num7 = 0;
			foreach (Settlement settlement in clan.Settlements)
			{
				if (settlement.IsTown)
				{
					if (kingdom.ActivePolicies.Contains(DefaultPolicies.RoadTolls))
					{
						int num8 = settlement.Town.TradeTaxAccumulated / 30;
						if (applyWithdrawals)
						{
							settlement.Town.TradeTaxAccumulated -= num8;
						}
						num6 += num8;
					}
					if (kingdom.ActivePolicies.Contains(DefaultPolicies.StateMonopolies))
					{
						num7 += (int)((float)settlement.Town.Workshops.Sum((Workshop t) => t.ProfitMade) * 0.05f);
					}
					if (num6 > 0)
					{
						explainedNumber.Add((float)num6, DefaultPolicies.RoadTolls.Name, null);
					}
					if (num7 > 0)
					{
						explainedNumber.Add((float)num7, DefaultPolicies.StateMonopolies.Name, null);
					}
				}
			}
			if (!explainedNumber.ResultNumber.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				if (!includeDetails)
				{
					goldChange.Add(explainedNumber.ResultNumber, GameTexts.FindText("str_policies", null), null);
					return;
				}
				goldChange.AddFromExplainedNumber(explainedNumber, GameTexts.FindText("str_policies", null));
			}
		}

		// Token: 0x060014CC RID: 5324 RVA: 0x0005D820 File Offset: 0x0005BA20
		private void AddExpensesForHiredMercenaries(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			Kingdom kingdom = clan.Kingdom;
			if (kingdom != null)
			{
				float num = DefaultClanFinanceModel.CalculateShareFactor(clan);
				if (kingdom.MercenaryWallet < 0)
				{
					int num2 = (int)((float)(-(float)kingdom.MercenaryWallet) * num);
					DefaultClanFinanceModel.ApplyShareForExpenses(clan, ref goldChange, applyWithdrawals, num2, DefaultClanFinanceModel._mercenaryExpensesStr);
					if (applyWithdrawals)
					{
						kingdom.MercenaryWallet += num2;
					}
				}
			}
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x0005D874 File Offset: 0x0005BA74
		private void AddExpensesForTributes(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			Kingdom kingdom = clan.Kingdom;
			if (kingdom != null)
			{
				float num = DefaultClanFinanceModel.CalculateShareFactor(clan);
				if (kingdom.TributeWallet < 0)
				{
					int num2 = (int)((float)(-(float)kingdom.TributeWallet) * num);
					DefaultClanFinanceModel.ApplyShareForExpenses(clan, ref goldChange, applyWithdrawals, num2, DefaultClanFinanceModel._tributeExpensesStr);
					if (applyWithdrawals)
					{
						kingdom.TributeWallet += num2;
						if (clan == Clan.PlayerClan)
						{
							CampaignEventDispatcher.Instance.OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType.TributesPaid, num2);
						}
					}
				}
			}
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x0005D8DC File Offset: 0x0005BADC
		private static void ApplyShareForExpenses(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals, int expenseShare, TextObject mercenaryExpensesStr)
		{
			if (applyWithdrawals)
			{
				int num = (int)((float)clan.Gold + goldChange.ResultNumber);
				if (expenseShare > num)
				{
					int num2 = expenseShare - num;
					expenseShare = num;
					clan.DebtToKingdom += num2;
				}
			}
			goldChange.Add((float)(-(float)expenseShare), mercenaryExpensesStr, null);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x0005D924 File Offset: 0x0005BB24
		private void AddSettlementIncome(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals, bool includeDetails)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, goldChange.IncludeDescriptions, null);
			foreach (Town town in clan.Fiefs)
			{
				ExplainedNumber explainedNumber2 = new ExplainedNumber((float)((int)((float)town.TradeTaxAccumulated / this.RevenueSmoothenFraction())), false, null);
				int num = MathF.Round(explainedNumber2.ResultNumber);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Trade.ContentTrades, town, ref explainedNumber2);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Crossbow.Steady, town, ref explainedNumber2);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Roguery.SaltTheEarth, town, ref explainedNumber2);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Steward.GivingHands, town, ref explainedNumber2);
				if (applyWithdrawals)
				{
					town.TradeTaxAccumulated -= num;
					if (clan == Clan.PlayerClan)
					{
						CampaignEventDispatcher.Instance.OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType.Taxes, (int)explainedNumber2.ResultNumber);
					}
				}
				int num2 = (int)Campaign.Current.Models.SettlementTaxModel.CalculateTownTax(town, false).ResultNumber;
				explainedNumber.Add((float)num2, DefaultClanFinanceModel._townTaxStr, town.Name);
				explainedNumber.Add(explainedNumber2.ResultNumber, DefaultClanFinanceModel._tariffTaxStr, town.Name);
				if (town.CurrentDefaultBuilding != null && town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Engineering.ArchitecturalCommisions))
				{
					explainedNumber.Add(DefaultPerks.Engineering.ArchitecturalCommisions.SecondaryBonus, DefaultClanFinanceModel._projectsIncomeStr, null);
				}
				foreach (Village village in town.Villages)
				{
					int num3 = this.CalculateVillageIncome(clan, village, applyWithdrawals);
					explainedNumber.Add((float)num3, village.Name, null);
				}
			}
			if (!includeDetails)
			{
				goldChange.Add(explainedNumber.ResultNumber, DefaultClanFinanceModel._settlementIncome, null);
				return;
			}
			goldChange.AddFromExplainedNumber(explainedNumber, DefaultClanFinanceModel._settlementIncome);
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x0005DB34 File Offset: 0x0005BD34
		private int CalculateVillageIncome(Clan clan, Village village, bool applyWithdrawals)
		{
			int num = ((village.VillageState == Village.VillageStates.Looted || village.VillageState == Village.VillageStates.BeingRaided) ? 0 : ((int)((float)village.TradeTaxAccumulated / this.RevenueSmoothenFraction())));
			int num2 = num;
			if (clan.Kingdom != null && clan.Kingdom.RulingClan != clan && clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.LandTax))
			{
				num -= (int)(0.05f * (float)num);
			}
			if (village.Bound.Town != null && village.Bound.Town.Governor != null && village.Bound.Town.Governor.GetPerkValue(DefaultPerks.Scouting.ForestKin))
			{
				num += MathF.Round((float)num * DefaultPerks.Scouting.ForestKin.SecondaryBonus);
			}
			Settlement bound = village.Bound;
			bool flag;
			if (bound == null)
			{
				flag = null != null;
			}
			else
			{
				Town town = bound.Town;
				flag = ((town != null) ? town.Governor : null) != null;
			}
			if (flag && village.Bound.Town.Governor.GetPerkValue(DefaultPerks.Steward.Logistician))
			{
				num += MathF.Round((float)num * DefaultPerks.Steward.Logistician.SecondaryBonus);
			}
			if (applyWithdrawals)
			{
				village.TradeTaxAccumulated -= num2;
				if (clan == Clan.PlayerClan)
				{
					CampaignEventDispatcher.Instance.OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType.Taxes, num);
				}
			}
			return num;
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0005DC68 File Offset: 0x0005BE68
		private static float CalculateShareFactor(Clan clan)
		{
			Kingdom kingdom = clan.Kingdom;
			int num = kingdom.Fiefs.Sum(delegate(Town x)
			{
				if (!x.IsCastle)
				{
					return 3;
				}
				return 1;
			}) + 1 + kingdom.Clans.Count;
			return (float)(clan.Fiefs.Sum(delegate(Town x)
			{
				if (!x.IsCastle)
				{
					return 3;
				}
				return 1;
			}) + ((clan == kingdom.RulingClan) ? 1 : 0) + 1) / (float)num;
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0005DCF4 File Offset: 0x0005BEF4
		private void AddMercenaryIncome(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			if (clan.IsUnderMercenaryService && clan.Leader != null && clan.Kingdom != null)
			{
				int num = MathF.Ceiling(clan.Influence * (1f / Campaign.Current.Models.ClanFinanceModel.RevenueSmoothenFraction())) * clan.MercenaryAwardMultiplier;
				if (applyWithdrawals)
				{
					clan.Kingdom.MercenaryWallet -= num;
				}
				goldChange.Add((float)num, DefaultClanFinanceModel._mercenaryStr, null);
			}
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x0005DD6C File Offset: 0x0005BF6C
		private void AddIncomeFromKingdomBudget(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			int num = ((clan.Gold < 5000) ? 2000 : ((clan.Gold < 10000) ? 1500 : ((clan.Gold < 20000) ? 1000 : 500)));
			num *= ((clan.Kingdom.KingdomBudgetWallet > 1000000) ? 2 : 1);
			num *= ((clan.Leader == clan.Kingdom.Leader) ? 2 : 1);
			int num2 = MathF.Min(clan.Kingdom.KingdomBudgetWallet, num);
			if (applyWithdrawals)
			{
				clan.Kingdom.KingdomBudgetWallet -= num2;
			}
			goldChange.Add((float)num2, DefaultClanFinanceModel._kingdomSupport, null);
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0005DE24 File Offset: 0x0005C024
		private void AddPlayerClanIncomeFromOwnedAlleys(ref ExplainedNumber goldChange)
		{
			int num = 0;
			foreach (Alley alley in Hero.MainHero.OwnedAlleys)
			{
				num += Campaign.Current.Models.AlleyModel.GetDailyIncomeOfAlley(alley);
			}
			goldChange.Add((float)num, DefaultClanFinanceModel._alley, null);
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x0005DE9C File Offset: 0x0005C09C
		private void AddIncomeFromTribute(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals, bool includeDetails)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, goldChange.IncludeDescriptions, null);
			IFaction mapFaction = clan.MapFaction;
			float num = 1f;
			if (clan.Kingdom != null)
			{
				num = DefaultClanFinanceModel.CalculateShareFactor(clan);
			}
			foreach (StanceLink stanceLink in mapFaction.Stances)
			{
				if (stanceLink.IsNeutral && stanceLink.GetDailyTributePaid(mapFaction) < 0)
				{
					int num2 = (int)((float)stanceLink.GetDailyTributePaid(mapFaction) * num);
					IFaction faction = ((stanceLink.Faction1 == mapFaction) ? stanceLink.Faction2 : stanceLink.Faction1);
					if (applyWithdrawals)
					{
						faction.TributeWallet += num2;
						if (stanceLink.Faction1 == mapFaction)
						{
							stanceLink.TotalTributePaidby2 += -num2;
						}
						if (stanceLink.Faction2 == mapFaction)
						{
							stanceLink.TotalTributePaidby1 += -num2;
						}
						if (clan == Clan.PlayerClan)
						{
							CampaignEventDispatcher.Instance.OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType.TributesEarned, num2);
						}
					}
					explainedNumber.Add((float)(-(float)num2), DefaultClanFinanceModel._tributeIncomeStr, faction.InformalName);
				}
			}
			if (!includeDetails)
			{
				goldChange.Add(explainedNumber.ResultNumber, DefaultClanFinanceModel._tributeIncomes, null);
				return;
			}
			goldChange.AddFromExplainedNumber(explainedNumber, DefaultClanFinanceModel._tributeIncomes);
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0005DFF8 File Offset: 0x0005C1F8
		private void AddIncomeFromParties(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals, bool includeDetails)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, goldChange.IncludeDescriptions, null);
			foreach (Hero hero in clan.Lords)
			{
				foreach (CaravanPartyComponent caravanPartyComponent in hero.OwnedCaravans)
				{
					if (caravanPartyComponent.MobileParty.IsActive && caravanPartyComponent.MobileParty.LeaderHero != clan.Leader && (caravanPartyComponent.MobileParty.IsLordParty || caravanPartyComponent.MobileParty.IsGarrison || caravanPartyComponent.MobileParty.IsCaravan))
					{
						int num = this.AddIncomeFromParty(caravanPartyComponent.MobileParty, clan, ref goldChange, applyWithdrawals);
						explainedNumber.Add((float)num, DefaultClanFinanceModel._caravanIncomeStr, (caravanPartyComponent.Leader != null) ? caravanPartyComponent.Leader.Name : caravanPartyComponent.Name);
					}
				}
			}
			foreach (Hero hero2 in clan.Companions)
			{
				foreach (CaravanPartyComponent caravanPartyComponent2 in hero2.OwnedCaravans)
				{
					if (caravanPartyComponent2.MobileParty.IsActive && caravanPartyComponent2.MobileParty.LeaderHero != clan.Leader && (caravanPartyComponent2.MobileParty.IsLordParty || caravanPartyComponent2.MobileParty.IsGarrison || caravanPartyComponent2.MobileParty.IsCaravan))
					{
						int num2 = this.AddIncomeFromParty(caravanPartyComponent2.MobileParty, clan, ref goldChange, applyWithdrawals);
						explainedNumber.Add((float)num2, DefaultClanFinanceModel._caravanIncomeStr, (caravanPartyComponent2.Leader != null) ? caravanPartyComponent2.Leader.Name : caravanPartyComponent2.Name);
					}
				}
			}
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				if (warPartyComponent.MobileParty.IsActive && warPartyComponent.MobileParty.LeaderHero != clan.Leader && (warPartyComponent.MobileParty.IsLordParty || warPartyComponent.MobileParty.IsGarrison || warPartyComponent.MobileParty.IsCaravan))
				{
					int num3 = this.AddIncomeFromParty(warPartyComponent.MobileParty, clan, ref goldChange, applyWithdrawals);
					explainedNumber.Add((float)num3, DefaultClanFinanceModel._partyIncomeStr, warPartyComponent.MobileParty.Name);
				}
			}
			if (!includeDetails)
			{
				goldChange.Add(explainedNumber.ResultNumber, DefaultClanFinanceModel._caravanAndPartyIncome, null);
				return;
			}
			goldChange.AddFromExplainedNumber(explainedNumber, DefaultClanFinanceModel._caravanAndPartyIncome);
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x0005E30C File Offset: 0x0005C50C
		private int AddIncomeFromParty(MobileParty party, Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			int num = 0;
			if (party.IsActive && party.LeaderHero != clan.Leader && (party.IsLordParty || party.IsGarrison || party.IsCaravan))
			{
				int num2 = ((party.IsLordParty && party.LeaderHero != null) ? party.LeaderHero.Gold : party.PartyTradeGold);
				if (num2 > 10000)
				{
					num = (num2 - 10000) / 10;
					if (applyWithdrawals)
					{
						this.RemovePartyGold(party, num);
						if (party.LeaderHero != null && num > 0)
						{
							SkillLevelingManager.OnTradeProfitMade(party.LeaderHero, num);
						}
						Hero owner = party.Party.Owner;
						bool flag;
						if (owner == null)
						{
							flag = null != null;
						}
						else
						{
							Clan clan2 = owner.Clan;
							flag = ((clan2 != null) ? clan2.Leader : null) != null;
						}
						if (flag && party.IsCaravan && party.Party.Owner.Clan.Leader.GetPerkValue(DefaultPerks.Trade.GreatInvestor))
						{
							party.Party.Owner.Clan.AddRenown(DefaultPerks.Trade.GreatInvestor.PrimaryBonus, true);
						}
						if (clan == Clan.PlayerClan && party.IsCaravan)
						{
							CampaignEventDispatcher.Instance.OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType.Caravan, num);
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x0005E440 File Offset: 0x0005C640
		private void AddExpensesFromPartiesAndGarrisons(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals, bool includeDetails)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, goldChange.IncludeDescriptions, null);
			int num = this.AddExpenseFromLeaderParty(clan, goldChange, applyWithdrawals);
			explainedNumber.Add((float)num, DefaultClanFinanceModel._mainPartywageStr, null);
			foreach (Hero hero in clan.Lords)
			{
				foreach (CaravanPartyComponent caravanPartyComponent in hero.OwnedCaravans)
				{
					if (caravanPartyComponent.MobileParty.IsActive && caravanPartyComponent.MobileParty.LeaderHero != clan.Leader)
					{
						int num2 = this.AddPartyExpense(caravanPartyComponent.MobileParty, clan, goldChange, applyWithdrawals);
						explainedNumber.Add((float)num2, DefaultClanFinanceModel._partyExpensesStr, caravanPartyComponent.Name);
					}
				}
			}
			foreach (Hero hero2 in clan.Companions)
			{
				foreach (CaravanPartyComponent caravanPartyComponent2 in hero2.OwnedCaravans)
				{
					int num3 = this.AddPartyExpense(caravanPartyComponent2.MobileParty, clan, goldChange, applyWithdrawals);
					explainedNumber.Add((float)num3, DefaultClanFinanceModel._partyExpensesStr, caravanPartyComponent2.Name);
				}
			}
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				if (warPartyComponent.MobileParty.IsActive && warPartyComponent.MobileParty.LeaderHero != clan.Leader)
				{
					int num4 = this.AddPartyExpense(warPartyComponent.MobileParty, clan, goldChange, applyWithdrawals);
					explainedNumber.Add((float)num4, DefaultClanFinanceModel._partyExpensesStr, warPartyComponent.Name);
				}
			}
			foreach (Town town in clan.Fiefs)
			{
				if (town.GarrisonParty != null && town.GarrisonParty.IsActive)
				{
					int num5 = this.AddPartyExpense(town.GarrisonParty, clan, goldChange, applyWithdrawals);
					TextObject textObject = new TextObject("{=fsTBcLvA}{SETTLEMENT} Garrison", null);
					textObject.SetTextVariable("SETTLEMENT", town.Name);
					explainedNumber.Add((float)num5, DefaultClanFinanceModel._partyExpensesStr, textObject);
				}
			}
			if (!includeDetails)
			{
				goldChange.Add(explainedNumber.ResultNumber, DefaultClanFinanceModel._garrisonAndPartyExpenses, null);
				return;
			}
			goldChange.AddFromExplainedNumber(explainedNumber, DefaultClanFinanceModel._garrisonAndPartyExpenses);
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x0005E73C File Offset: 0x0005C93C
		private void AddExpensesForAutoRecruitment(Clan clan, ref ExplainedNumber goldChange, bool applyWithdrawals = false)
		{
			int num = clan.AutoRecruitmentExpenses / 5;
			if (applyWithdrawals)
			{
				clan.AutoRecruitmentExpenses -= num;
			}
			goldChange.Add((float)(-(float)num), DefaultClanFinanceModel._autoRecruitmentStr, null);
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x0005E774 File Offset: 0x0005C974
		private int AddExpenseFromLeaderParty(Clan clan, ExplainedNumber goldChange, bool applyWithdrawals)
		{
			Hero leader = clan.Leader;
			MobileParty mobileParty = ((leader != null) ? leader.PartyBelongedTo : null);
			if (mobileParty != null)
			{
				int num = clan.Gold + (int)goldChange.ResultNumber;
				if (num < 2000 && applyWithdrawals && clan != Clan.PlayerClan)
				{
					num = 0;
				}
				return -this.CalculatePartyWage(mobileParty, num, applyWithdrawals);
			}
			return 0;
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0005E7CC File Offset: 0x0005C9CC
		private int AddPartyExpense(MobileParty party, Clan clan, ExplainedNumber goldChange, bool applyWithdrawals)
		{
			int num = clan.Gold + (int)goldChange.ResultNumber;
			int num2 = num;
			if (num < (party.IsGarrison ? 8000 : 4000) && applyWithdrawals && clan != Clan.PlayerClan)
			{
				num2 = ((party.LeaderHero != null && party.LeaderHero.Gold < 500) ? MathF.Min(num, 250) : 0);
			}
			int num3 = this.CalculatePartyWage(party, num2, applyWithdrawals);
			int num4 = ((party.IsLordParty && party.LeaderHero != null) ? party.LeaderHero.Gold : party.PartyTradeGold);
			if (applyWithdrawals)
			{
				if (party.IsLordParty)
				{
					if (party.LeaderHero != null)
					{
						party.LeaderHero.Gold -= num3;
					}
					else
					{
						party.ActualClan.Leader.Gold -= num3;
					}
				}
				else
				{
					party.PartyTradeGold -= num3;
				}
			}
			num4 -= num3;
			if (num4 < 5000)
			{
				int num5 = 5000 - num4;
				if (applyWithdrawals)
				{
					num5 = MathF.Min(num5, num2);
					if (party.IsLordParty && party.LeaderHero != null)
					{
						party.LeaderHero.Gold += num5;
					}
					else
					{
						party.PartyTradeGold += num5;
					}
				}
				return -num5;
			}
			return 0;
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x0005E915 File Offset: 0x0005CB15
		public override int CalculateOwnerIncomeFromCaravan(MobileParty caravan)
		{
			return (int)((float)MathF.Max(0, caravan.PartyTradeGold - 10000) / this.RevenueSmoothenFraction());
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x0005E932 File Offset: 0x0005CB32
		private void RemovePartyGold(MobileParty party, int share)
		{
			if (party.IsLordParty && party.LeaderHero != null)
			{
				party.LeaderHero.Gold -= share;
				return;
			}
			party.PartyTradeGold -= share;
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x0005E966 File Offset: 0x0005CB66
		public override int CalculateOwnerIncomeFromWorkshop(Workshop workshop)
		{
			return (int)((float)MathF.Max(0, workshop.ProfitMade) / this.RevenueSmoothenFraction());
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x0005E97D File Offset: 0x0005CB7D
		public override int CalculateOwnerExpenseFromWorkshop(Workshop workshop)
		{
			return (int)((float)MathF.Max(0, workshop.InitialCapital - workshop.Capital) / this.RevenueSmoothenFraction());
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x0005E99C File Offset: 0x0005CB9C
		private void CalculateHeroIncomeFromAssets(Hero hero, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			int num = 0;
			foreach (CaravanPartyComponent caravanPartyComponent in hero.OwnedCaravans)
			{
				if (caravanPartyComponent.MobileParty.PartyTradeGold > 10000)
				{
					int num2 = Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromCaravan(caravanPartyComponent.MobileParty);
					if (applyWithdrawals)
					{
						caravanPartyComponent.MobileParty.PartyTradeGold -= num2;
						SkillLevelingManager.OnTradeProfitMade(hero, num2);
					}
					if (num2 > 0)
					{
						num += num2;
					}
				}
			}
			goldChange.Add((float)num, DefaultClanFinanceModel._caravanIncomeStr, null);
			this.CalculateHeroIncomeFromWorkshops(hero, ref goldChange, applyWithdrawals);
			if (hero.CurrentSettlement != null)
			{
				foreach (Alley alley in hero.CurrentSettlement.Alleys)
				{
					if (alley.Owner == hero)
					{
						goldChange.Add(30f, alley.Name, null);
					}
				}
			}
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x0005EABC File Offset: 0x0005CCBC
		private void CalculateHeroIncomeFromWorkshops(Hero hero, ref ExplainedNumber goldChange, bool applyWithdrawals)
		{
			int num = 0;
			int num2 = 0;
			foreach (Workshop workshop in hero.OwnedWorkshops)
			{
				int num3 = Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromWorkshop(workshop);
				num += num3;
				if (applyWithdrawals && num3 > 0)
				{
					workshop.ChangeGold(-num3);
					if (hero == Hero.MainHero)
					{
						CampaignEventDispatcher.Instance.OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType.Workshop, num3);
					}
				}
				if (num3 > 0)
				{
					num2++;
				}
			}
			goldChange.Add((float)num, DefaultClanFinanceModel._shopIncomeStr, null);
			bool flag;
			if (hero.Clan != null)
			{
				Hero leader = hero.Clan.Leader;
				flag = leader != null && leader.GetPerkValue(DefaultPerks.Trade.ArtisanCommunity);
			}
			else
			{
				flag = false;
			}
			if (flag && applyWithdrawals)
			{
				hero.Clan.AddRenown((float)num2 * DefaultPerks.Trade.ArtisanCommunity.PrimaryBonus, true);
			}
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x0005EBAC File Offset: 0x0005CDAC
		public override float RevenueSmoothenFraction()
		{
			return 5f;
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0005EBB3 File Offset: 0x0005CDB3
		public override int PartyGoldLowerTreshold()
		{
			return 5000;
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x0005EBBC File Offset: 0x0005CDBC
		private int CalculatePartyWage(MobileParty mobileParty, int budget, bool applyWithdrawals)
		{
			int totalWage = mobileParty.TotalWage;
			int num = totalWage;
			if (applyWithdrawals)
			{
				num = MathF.Min(totalWage, budget);
				DefaultClanFinanceModel.ApplyMoraleEffect(mobileParty, totalWage, num);
			}
			return num;
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x0005EBE8 File Offset: 0x0005CDE8
		public override int CalculateNotableDailyGoldChange(Hero hero, bool applyWithdrawals)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			this.CalculateHeroIncomeFromAssets(hero, ref explainedNumber, applyWithdrawals);
			return (int)explainedNumber.ResultNumber;
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x0005EC18 File Offset: 0x0005CE18
		private static void ApplyMoraleEffect(MobileParty mobileParty, int wage, int paymentAmount)
		{
			if (paymentAmount < wage && wage > 0)
			{
				float num = 1f - (float)paymentAmount / (float)wage;
				float num2 = (float)Campaign.Current.Models.PartyMoraleModel.GetDailyNoWageMoralePenalty(mobileParty) * num;
				if (mobileParty.HasUnpaidWages < num)
				{
					num2 += (float)Campaign.Current.Models.PartyMoraleModel.GetDailyNoWageMoralePenalty(mobileParty) * (num - mobileParty.HasUnpaidWages);
				}
				mobileParty.RecentEventsMorale += num2;
				mobileParty.HasUnpaidWages = num;
				MBTextManager.SetTextVariable("reg1", MathF.Round(MathF.Abs(num2), 1));
				if (mobileParty == MobileParty.MainParty)
				{
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_party_loses_moral_due_to_insufficent_funds", null), 0, null, "");
					return;
				}
			}
			else
			{
				mobileParty.HasUnpaidWages = 0f;
			}
		}

		// Token: 0x04000753 RID: 1875
		private static readonly TextObject _townTaxStr = new TextObject("{=TLuaPAIO}{A0} Taxes", null);

		// Token: 0x04000754 RID: 1876
		private static readonly TextObject _townTradeTaxStr = new TextObject("{=dfwCjiRx}Trade Tax from {A0}", null);

		// Token: 0x04000755 RID: 1877
		private static readonly TextObject _partyIncomeStr = new TextObject("{=uuyso3mg}Income from Parties", null);

		// Token: 0x04000756 RID: 1878
		private static readonly TextObject _financialHelpStr = new TextObject("{=E3BsEDav}Financial Help for Parties", null);

		// Token: 0x04000757 RID: 1879
		private static readonly TextObject _scutageTaxStr = new TextObject("{=RuHaC2Ck}Scutage Tax", null);

		// Token: 0x04000758 RID: 1880
		private static readonly TextObject _caravanIncomeStr = new TextObject("{=qyahMgD3}Caravan ({A0})", null);

		// Token: 0x04000759 RID: 1881
		private static readonly TextObject _projectsIncomeStr = new TextObject("{=uixuohBp}Settlement Projects", null);

		// Token: 0x0400075A RID: 1882
		private static readonly TextObject _partyExpensesStr = new TextObject("{=dZDFxUvU}{A0} Party Expense", null);

		// Token: 0x0400075B RID: 1883
		private static readonly TextObject _shopIncomeStr = new TextObject("{=0g7MZCAK}Workshop Income", null);

		// Token: 0x0400075C RID: 1884
		private static readonly TextObject _mercenaryStr = new TextObject("{=qcaaJLhx}Mercenary Contract", null);

		// Token: 0x0400075D RID: 1885
		private static readonly TextObject _mercenaryExpensesStr = new TextObject("{=5aElrlUt}Payment to Mercenaries", null);

		// Token: 0x0400075E RID: 1886
		private static readonly TextObject _tributeExpensesStr = new TextObject("{=AtFv5RMW}Tribute Payments", null);

		// Token: 0x0400075F RID: 1887
		private static readonly TextObject _tributeIncomeStr = new TextObject("{=rhfgzKtA}Tribute from {A0}", null);

		// Token: 0x04000760 RID: 1888
		private static readonly TextObject _tributeIncomes = new TextObject("{=tributeIncome}Tribute Income", null);

		// Token: 0x04000761 RID: 1889
		private static readonly TextObject _settlementIncome = new TextObject("{=AewK9qME}Settlement Income", null);

		// Token: 0x04000762 RID: 1890
		private static readonly TextObject _mainPartywageStr = new TextObject("{=YkZKXsIn}Main party wages", null);

		// Token: 0x04000763 RID: 1891
		private static readonly TextObject _caravanAndPartyIncome = new TextObject("{=8iLzK3Y4}Caravan and Party Income", null);

		// Token: 0x04000764 RID: 1892
		private static readonly TextObject _garrisonAndPartyExpenses = new TextObject("{=ChUDSiJw}Garrison and Party Expense", null);

		// Token: 0x04000765 RID: 1893
		private static readonly TextObject _debtStr = new TextObject("{=U3LdMEXb}Debts", null);

		// Token: 0x04000766 RID: 1894
		private static readonly TextObject _kingdomSupport = new TextObject("{=essaRvXP}King's support", null);

		// Token: 0x04000767 RID: 1895
		private static readonly TextObject _supportKing = new TextObject("{=WrJSUsBe}Support to king", null);

		// Token: 0x04000768 RID: 1896
		private static readonly TextObject _workshopExpenseStr = new TextObject("{=oNgwQTTV}Workshop Expense", null);

		// Token: 0x04000769 RID: 1897
		private static readonly TextObject _kingdomBudgetStr = new TextObject("{=7uzvI8e8}Kingdom Budget Expense", null);

		// Token: 0x0400076A RID: 1898
		private static readonly TextObject _tariffTaxStr = new TextObject("{=wVMPdc8J}{A0}'s tariff", null);

		// Token: 0x0400076B RID: 1899
		private static readonly TextObject _autoRecruitmentStr = new TextObject("{=6gvDrbe7}Recruitment Expense", null);

		// Token: 0x0400076C RID: 1900
		private static readonly TextObject _alley = new TextObject("{=UQc6zg1Q}Owned Alleys", null);

		// Token: 0x0400076D RID: 1901
		private const int PartyGoldIncomeThreshold = 10000;

		// Token: 0x0400076E RID: 1902
		private const int PartyGoldLowerThreshold = 5000;

		// Token: 0x0400076F RID: 1903
		private const int payGarrisonWagesTreshold = 8000;

		// Token: 0x04000770 RID: 1904
		private const int payClanPartiesTreshold = 4000;

		// Token: 0x04000771 RID: 1905
		private const int payLeaderPartyWageTreshold = 2000;

		// Token: 0x020004FE RID: 1278
		private enum TransactionType
		{
			// Token: 0x04001581 RID: 5505
			Income = 1,
			// Token: 0x04001582 RID: 5506
			Both = 0,
			// Token: 0x04001583 RID: 5507
			Expense = -1
		}

		// Token: 0x020004FF RID: 1279
		public enum AssetIncomeType
		{
			// Token: 0x04001585 RID: 5509
			Workshop,
			// Token: 0x04001586 RID: 5510
			Caravan,
			// Token: 0x04001587 RID: 5511
			Taxes,
			// Token: 0x04001588 RID: 5512
			TributesEarned,
			// Token: 0x04001589 RID: 5513
			TributesPaid
		}
	}
}
