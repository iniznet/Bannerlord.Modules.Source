using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class NotablesCampaignBehavior : CampaignBehaviorBase
	{
		public NotablesCampaignBehavior()
		{
			this._settlementPassedDaysForWeeklyTick = new Dictionary<Settlement, int>();
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyTickHero));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
		}

		private void WeeklyTick()
		{
			foreach (Hero hero in Hero.DeadOrDisabledHeroes.ToList<Hero>())
			{
				if (hero.IsDead && hero.IsNotable && hero.DeathDay.ElapsedDaysUntilNow >= 7f)
				{
					Campaign.Current.CampaignObjectManager.UnregisterDeadHero(hero);
				}
			}
		}

		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.WeeklyTick();
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Settlement, int>>("_settlementPassedDaysForWeeklyTick", ref this._settlementPassedDaysForWeeklyTick);
		}

		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.SpawnNotablesAtGameStart();
		}

		private void DetermineRelation(Hero hero1, Hero hero2, float randomValue, float chanceOfConflict)
		{
			float num = 0.3f;
			if (randomValue < num)
			{
				int num2 = (int)((num - randomValue) * (num - randomValue) / (num * num) * 100f);
				if (num2 > 0)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, num2, true);
					return;
				}
			}
			else if (randomValue > 1f - chanceOfConflict)
			{
				int num3 = -(int)((randomValue - (1f - chanceOfConflict)) * (randomValue - (1f - chanceOfConflict)) / (chanceOfConflict * chanceOfConflict) * 100f);
				if (num3 < 0)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, num3, true);
				}
			}
		}

		public void SetInitialRelationsBetweenNotablesAndLords()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				for (int i = 0; i < settlement.Notables.Count; i++)
				{
					Hero hero = settlement.Notables[i];
					foreach (Hero hero2 in settlement.MapFaction.Lords)
					{
						if (hero2 != hero && hero2 == hero2.Clan.Leader && hero2.MapFaction == settlement.MapFaction)
						{
							float num = (float)HeroHelper.NPCPersonalityClashWithNPC(hero, hero2) * 0.01f * 2.5f;
							float num2 = MBRandom.RandomFloat;
							float num3 = Campaign.MapDiagonal;
							foreach (Settlement settlement2 in hero2.Clan.Settlements)
							{
								float num4 = ((settlement == settlement2) ? 0f : settlement2.Position2D.Distance(settlement.Position2D));
								if (num4 < num3)
								{
									num3 = num4;
								}
							}
							float num5 = ((num3 < 100f) ? (1f - num3 / 100f) : 0f);
							float num6 = num5 * MBRandom.RandomFloat + (1f - num5);
							if (MBRandom.RandomFloat < 0.2f)
							{
								num6 = 1f / (0.5f + 0.5f * num6);
							}
							num2 *= num6;
							if (num2 > 1f)
							{
								num2 = 1f;
							}
							this.DetermineRelation(hero, hero2, num2, num);
						}
						for (int j = i + 1; j < settlement.Notables.Count; j++)
						{
							Hero hero3 = settlement.Notables[j];
							float num7 = (float)HeroHelper.NPCPersonalityClashWithNPC(hero, hero3) * 0.01f * 2.5f;
							float num8 = MBRandom.RandomFloat;
							if (hero.CharacterObject.Occupation == hero3.CharacterObject.Occupation)
							{
								num8 = 1f - 0.25f * MBRandom.RandomFloat;
							}
							this.DetermineRelation(hero, hero3, num8, num7);
						}
					}
				}
			}
		}

		public void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int i)
		{
			if (i == 1)
			{
				this.SetInitialRelationsBetweenNotablesAndLords();
				int num = 50;
				for (int j = 0; j < num; j++)
				{
					foreach (Hero hero in Hero.AllAliveHeroes)
					{
						if (hero.IsNotable)
						{
							this.UpdateNotableSupport(hero);
						}
					}
				}
			}
		}

		private void DailyTickSettlement(Settlement settlement)
		{
			if (this._settlementPassedDaysForWeeklyTick.ContainsKey(settlement))
			{
				Dictionary<Settlement, int> settlementPassedDaysForWeeklyTick = this._settlementPassedDaysForWeeklyTick;
				int num = settlementPassedDaysForWeeklyTick[settlement];
				settlementPassedDaysForWeeklyTick[settlement] = num + 1;
				if (this._settlementPassedDaysForWeeklyTick[settlement] == 7)
				{
					SettlementHelper.SpawnNotablesIfNeeded(settlement);
					this._settlementPassedDaysForWeeklyTick[settlement] = 0;
					return;
				}
			}
			else
			{
				this._settlementPassedDaysForWeeklyTick.Add(settlement, 0);
			}
		}

		private void UpdateNotableRelations(Hero notable)
		{
			foreach (Clan clan in Clan.All)
			{
				if (clan != Clan.PlayerClan && clan.Leader != null && !clan.IsEliminated)
				{
					int relation = notable.GetRelation(clan.Leader);
					if (relation > 0)
					{
						float num = (float)relation / 1000f;
						if (MBRandom.RandomFloat < num)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(notable, clan.Leader, -20, true);
						}
					}
					else if (relation < 0)
					{
						float num2 = (float)(-(float)relation) / 1000f;
						if (MBRandom.RandomFloat < num2)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(notable, clan.Leader, 20, true);
						}
					}
				}
			}
		}

		private void UpdateNotableSupport(Hero notable)
		{
			if (notable.SupporterOf == null)
			{
				using (IEnumerator<Clan> enumerator = Clan.NonBanditFactions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Clan clan = enumerator.Current;
						if (clan.Leader != null)
						{
							int relation = notable.GetRelation(clan.Leader);
							if (relation > 50)
							{
								float num = (float)(relation - 50) / 2000f;
								if (MBRandom.RandomFloat < num)
								{
									notable.SupporterOf = clan;
								}
							}
						}
					}
					return;
				}
			}
			int relation2 = notable.GetRelation(notable.SupporterOf.Leader);
			if (relation2 < 0)
			{
				notable.SupporterOf = null;
				return;
			}
			if (relation2 < 50)
			{
				float num2 = (float)(50 - relation2) / 500f;
				if (MBRandom.RandomFloat < num2)
				{
					notable.SupporterOf = null;
				}
			}
		}

		private void BalanceGoldAndPowerOfNotable(Hero notable)
		{
			if (notable.Gold > 10500)
			{
				int num = (notable.Gold - 10000) / 500;
				GiveGoldAction.ApplyBetweenCharacters(notable, null, num * 500, true);
				notable.AddPower((float)num);
				return;
			}
			if (notable.Gold < 4500 && notable.Power > 0f)
			{
				int num2 = (5000 - notable.Gold) / 500;
				GiveGoldAction.ApplyBetweenCharacters(null, notable, num2 * 500, true);
				notable.AddPower((float)(-(float)num2));
			}
		}

		private void DailyTickHero(Hero hero)
		{
			if (hero.IsNotable && hero.CurrentSettlement != null)
			{
				if (MBRandom.RandomFloat < 0.01f)
				{
					this.UpdateNotableRelations(hero);
				}
				this.UpdateNotableSupport(hero);
				this.BalanceGoldAndPowerOfNotable(hero);
				this.ManageCaravanExpensesOfNotable(hero);
				this.CheckAndMakeNotableDisappear(hero);
			}
		}

		private void CheckAndMakeNotableDisappear(Hero notable)
		{
			if (notable.OwnedWorkshops.IsEmpty<Workshop>() && notable.OwnedCaravans.IsEmpty<CaravanPartyComponent>() && notable.OwnedAlleys.IsEmpty<Alley>() && notable.CanDie(KillCharacterAction.KillCharacterActionDetail.Lost) && notable.CanHaveQuestsOrIssues() && notable.Power < (float)Campaign.Current.Models.NotablePowerModel.NotableDisappearPowerLimit)
			{
				float randomFloat = MBRandom.RandomFloat;
				float notableDisappearProbability = this.GetNotableDisappearProbability(notable);
				if (randomFloat < notableDisappearProbability)
				{
					KillCharacterAction.ApplyByRemove(notable, false, true);
					IssueBase issue = notable.Issue;
					if (issue == null)
					{
						return;
					}
					issue.CompleteIssueWithAiLord(notable.CurrentSettlement.OwnerClan.Leader);
				}
			}
		}

		private void ManageCaravanExpensesOfNotable(Hero notable)
		{
			for (int i = notable.OwnedCaravans.Count - 1; i >= 0; i--)
			{
				CaravanPartyComponent caravanPartyComponent = notable.OwnedCaravans[i];
				int totalWage = caravanPartyComponent.MobileParty.TotalWage;
				if (caravanPartyComponent.MobileParty.PartyTradeGold >= totalWage)
				{
					caravanPartyComponent.MobileParty.PartyTradeGold -= totalWage;
				}
				else
				{
					int num = MathF.Min(totalWage, notable.Gold);
					notable.Gold -= num;
				}
				if (caravanPartyComponent.MobileParty.PartyTradeGold < 5000)
				{
					int num2 = MathF.Min(5000 - caravanPartyComponent.MobileParty.PartyTradeGold, notable.Gold);
					caravanPartyComponent.MobileParty.PartyTradeGold += num2;
					notable.Gold -= num2;
				}
			}
		}

		private float GetNotableDisappearProbability(Hero hero)
		{
			return ((float)Campaign.Current.Models.NotablePowerModel.NotableDisappearPowerLimit - hero.Power) / (float)Campaign.Current.Models.NotablePowerModel.NotableDisappearPowerLimit * 0.02f;
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (victim.IsNotable)
			{
				if (victim.Power >= (float)Campaign.Current.Models.NotablePowerModel.NotableDisappearPowerLimit)
				{
					Hero hero = HeroCreator.CreateRelativeNotableHero(victim);
					if (victim.CurrentSettlement != null)
					{
						this.ChangeDeadNotable(victim, hero, victim.CurrentSettlement);
					}
					using (List<CaravanPartyComponent>.Enumerator enumerator = victim.OwnedCaravans.ToList<CaravanPartyComponent>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CaravanPartyComponent caravanPartyComponent = enumerator.Current;
							CaravanPartyComponent.TransferCaravanOwnership(caravanPartyComponent.MobileParty, hero, hero.CurrentSettlement);
						}
						return;
					}
				}
				foreach (CaravanPartyComponent caravanPartyComponent2 in victim.OwnedCaravans.ToList<CaravanPartyComponent>())
				{
					DestroyPartyAction.Apply(null, caravanPartyComponent2.MobileParty);
				}
			}
		}

		private void ChangeDeadNotable(Hero deadNotable, Hero newNotable, Settlement notableSettlement)
		{
			EnterSettlementAction.ApplyForCharacterOnly(newNotable, notableSettlement);
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (newNotable != hero)
				{
					int relation = deadNotable.GetRelation(hero);
					if (Math.Abs(relation) >= 20 || (relation != 0 && hero.CurrentSettlement == notableSettlement))
					{
						newNotable.SetPersonalRelation(hero, relation);
					}
				}
			}
			if (deadNotable.Issue != null)
			{
				Campaign.Current.IssueManager.ChangeIssueOwner(deadNotable.Issue, newNotable);
			}
		}

		private void SpawnNotablesAtGameStart()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsTown)
				{
					int targetNotableCountForSettlement = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(settlement, Occupation.Artisan);
					for (int i = 0; i < targetNotableCountForSettlement; i++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.Artisan, settlement);
					}
					int targetNotableCountForSettlement2 = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(settlement, Occupation.Merchant);
					for (int j = 0; j < targetNotableCountForSettlement2; j++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.Merchant, settlement);
					}
					int targetNotableCountForSettlement3 = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(settlement, Occupation.GangLeader);
					for (int k = 0; k < targetNotableCountForSettlement3; k++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.GangLeader, settlement);
					}
				}
				else if (settlement.IsVillage)
				{
					int targetNotableCountForSettlement4 = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(settlement, Occupation.RuralNotable);
					for (int l = 0; l < targetNotableCountForSettlement4; l++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.RuralNotable, settlement);
					}
					int targetNotableCountForSettlement5 = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(settlement, Occupation.Headman);
					for (int m = 0; m < targetNotableCountForSettlement5; m++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.Headman, settlement);
					}
				}
			}
		}

		private const int GoldLimitForNotablesToStartGainingPower = 10000;

		private const int GoldLimitForNotablesToStartLosingPower = 5000;

		private const int GoldNeededToGainOnePower = 500;

		private const int CaravanGoldLowLimit = 5000;

		private const int RemoveNotableCharacterAfterDays = 7;

		private Dictionary<Settlement, int> _settlementPassedDaysForWeeklyTick;
	}
}
