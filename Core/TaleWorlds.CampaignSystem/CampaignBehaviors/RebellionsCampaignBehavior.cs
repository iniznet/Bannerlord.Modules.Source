using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class RebellionsCampaignBehavior : CampaignBehaviorBase
	{
		public RebellionsCampaignBehavior()
		{
			this._rebelClansAndDaysPassedAfterCreation = new Dictionary<Clan, int>();
			this._cultureIconIdAndFrequencies = new Dictionary<CultureObject, Dictionary<int, int>>();
		}

		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoaded));
			CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyed));
			CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeStarted));
		}

		private void OnSiegeStarted(SiegeEvent siegeEvent)
		{
			if (siegeEvent.BesiegedSettlement.IsTown)
			{
				this.CheckAndSetTownRebelliousState(siegeEvent.BesiegedSettlement);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Clan, int>>("_rebelClansAndDaysPassedAfterCreation", ref this._rebelClansAndDaysPassedAfterCreation);
			dataStore.SyncData<Dictionary<CultureObject, Dictionary<int, int>>>("_iconIdAndFrequency", ref this._cultureIconIdAndFrequencies);
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			this.InitializeIconIdAndFrequencies();
		}

		private void OnGameLoaded()
		{
			this.InitializeIconIdAndFrequencies();
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("e1.7.3.0", 27066))
			{
				foreach (Settlement settlement in Settlement.All)
				{
					if (!settlement.IsTown && settlement.InRebelliousState)
					{
						settlement.Town.InRebelliousState = false;
						CampaignEventDispatcher.Instance.TownRebelliousStateChanged(settlement.Town, false);
					}
				}
			}
		}

		private void DailyTickSettlement(Settlement settlement)
		{
			if (this._rebellionEnabled && settlement.IsTown && settlement.Party.MapEvent == null && settlement.Party.SiegeEvent == null && !settlement.OwnerClan.IsRebelClan && Settlement.CurrentSettlement != settlement)
			{
				this.CheckAndSetTownRebelliousState(settlement);
				if (MBRandom.RandomFloat < 0.25f && RebellionsCampaignBehavior.CheckRebellionEvent(settlement))
				{
					this.StartRebellionEvent(settlement);
				}
			}
			if (settlement.IsTown && settlement.OwnerClan.IsRebelClan)
			{
				float num = MBMath.Map((float)(this._rebelClansAndDaysPassedAfterCreation[settlement.OwnerClan] - 1), 0f, 30f, (float)Campaign.Current.Models.SettlementLoyaltyModel.LoyaltyBoostAfterRebellionStartValue, 0f);
				settlement.Town.Loyalty += num;
			}
		}

		private void CheckAndSetTownRebelliousState(Settlement settlement)
		{
			bool inRebelliousState = settlement.Town.InRebelliousState;
			settlement.Town.InRebelliousState = settlement.Town.Loyalty <= (float)Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold;
			if (inRebelliousState != settlement.Town.InRebelliousState)
			{
				CampaignEventDispatcher.Instance.TownRebelliousStateChanged(settlement.Town, settlement.Town.InRebelliousState);
			}
		}

		private void OnClanDestroyed(Clan destroyedClan)
		{
			if (this._rebelClansAndDaysPassedAfterCreation.ContainsKey(destroyedClan))
			{
				this._rebelClansAndDaysPassedAfterCreation.Remove(destroyedClan);
			}
			if (destroyedClan.IsRebelClan)
			{
				for (int i = destroyedClan.Heroes.Count - 1; i >= 0; i--)
				{
					Hero hero = destroyedClan.Heroes[i];
					Campaign.Current.CampaignObjectManager.UnregisterDeadHero(hero);
				}
			}
		}

		private void DailyTickClan(Clan clan)
		{
			if (this._rebelClansAndDaysPassedAfterCreation.ContainsKey(clan))
			{
				Dictionary<Clan, int> rebelClansAndDaysPassedAfterCreation = this._rebelClansAndDaysPassedAfterCreation;
				int num = rebelClansAndDaysPassedAfterCreation[clan];
				rebelClansAndDaysPassedAfterCreation[clan] = num + 1;
				if (this._rebelClansAndDaysPassedAfterCreation[clan] >= 30 && clan.Leader != null && clan.Settlements.Count > 0)
				{
					TextObject textObject = new TextObject("{=aKaGaOQx}{CLAN_LEADER.NAME}{.o} Clan", null);
					StringHelpers.SetCharacterProperties("CLAN_LEADER", clan.Leader.CharacterObject, textObject, false);
					clan.ChangeClanName(textObject, textObject);
					clan.IsRebelClan = false;
					this._rebelClansAndDaysPassedAfterCreation.Remove(clan);
					CampaignEventDispatcher.Instance.OnRebelliousClanDisbandedAtSettlement(clan.HomeSettlement, clan);
				}
			}
			if (clan.IsRebelClan && clan.Settlements.Count == 0 && clan.Heroes.Count > 0 && !clan.IsEliminated)
			{
				for (int i = clan.Heroes.Count - 1; i >= 0; i--)
				{
					Hero hero = clan.Heroes[i];
					if (hero.IsAlive)
					{
						if (hero.IsPrisoner && hero.PartyBelongedToAsPrisoner != null && hero.PartyBelongedToAsPrisoner != PartyBase.MainParty && hero.PartyBelongedToAsPrisoner.LeaderHero != null)
						{
							KillCharacterAction.ApplyByExecution(hero, hero.PartyBelongedToAsPrisoner.LeaderHero, true, true);
						}
						else if (hero.PartyBelongedTo == null)
						{
							KillCharacterAction.ApplyByRemove(hero, false, true);
						}
						else if (this._rebelClansAndDaysPassedAfterCreation[clan] > 90 && hero.PartyBelongedTo != null && hero.PartyBelongedTo.MapEvent == null)
						{
							KillCharacterAction.ApplyByRemove(hero, false, true);
						}
					}
				}
			}
		}

		private static bool CheckRebellionEvent(Settlement settlement)
		{
			if (settlement.Town.Loyalty <= (float)Campaign.Current.Models.SettlementLoyaltyModel.RebellionStartLoyaltyThreshold)
			{
				float militia = settlement.Militia;
				MobileParty garrisonParty = settlement.Town.GarrisonParty;
				float num = ((garrisonParty != null) ? garrisonParty.Party.TotalStrength : 0f);
				foreach (MobileParty mobileParty in settlement.Parties)
				{
					if (mobileParty.IsLordParty && FactionManager.IsAlliedWithFaction(mobileParty.MapFaction, settlement.MapFaction))
					{
						num += mobileParty.Party.TotalStrength;
					}
				}
				return militia >= num * 1.4f;
			}
			return false;
		}

		public void StartRebellionEvent(Settlement settlement)
		{
			Clan ownerClan = settlement.OwnerClan;
			this.CreateRebelPartyAndClan(settlement);
			this.ApplyRebellionConsequencesToSettlement(settlement);
			CampaignEventDispatcher.Instance.OnRebellionFinished(settlement, ownerClan);
			settlement.Town.FoodStocks = (float)settlement.Town.FoodStocksUpperLimit();
			settlement.Militia = 100f;
		}

		private void ApplyRebellionConsequencesToSettlement(Settlement settlement)
		{
			Dictionary<TroopRosterElement, int> dictionary = new Dictionary<TroopRosterElement, int>();
			foreach (TroopRosterElement troopRosterElement in settlement.Town.GarrisonParty.MemberRoster.GetTroopRoster())
			{
				for (int i = 0; i < troopRosterElement.Number; i++)
				{
					if (MBRandom.RandomFloat < 0.5f)
					{
						if (dictionary.ContainsKey(troopRosterElement))
						{
							Dictionary<TroopRosterElement, int> dictionary2 = dictionary;
							TroopRosterElement troopRosterElement2 = troopRosterElement;
							int num = dictionary2[troopRosterElement2];
							dictionary2[troopRosterElement2] = num + 1;
						}
						else
						{
							dictionary.Add(troopRosterElement, 1);
						}
					}
				}
			}
			settlement.Town.GarrisonParty.MemberRoster.Clear();
			foreach (KeyValuePair<TroopRosterElement, int> keyValuePair in dictionary)
			{
				settlement.Town.GarrisonParty.AddPrisoner(keyValuePair.Key.Character, keyValuePair.Value);
			}
			settlement.Town.GarrisonParty.AddElementToMemberRoster(settlement.Culture.RangedMilitiaTroop, (int)(settlement.Militia * (MBRandom.RandomFloatRanged(-0.1f, 0.1f) + 0.6f)), false);
			settlement.Militia = 0f;
			if (settlement.MilitiaPartyComponent != null)
			{
				DestroyPartyAction.Apply(null, settlement.MilitiaPartyComponent.MobileParty);
			}
			settlement.Town.GarrisonParty.MemberRoster.AddToCounts(settlement.OwnerClan.Culture.BasicTroop, 50, false, 0, 0, true, -1);
			settlement.Town.GarrisonParty.MemberRoster.AddToCounts((settlement.OwnerClan.Culture.BasicTroop.UpgradeTargets.Length != 0) ? settlement.OwnerClan.Culture.BasicTroop.UpgradeTargets.GetRandomElement<CharacterObject>() : settlement.OwnerClan.Culture.BasicTroop, 25, false, 0, 0, true, -1);
			settlement.Town.Loyalty = 100f;
			settlement.Town.InRebelliousState = false;
		}

		private void CreateRebelPartyAndClan(Settlement settlement)
		{
			MBReadOnlyList<CharacterObject> rebelliousHeroTemplates = settlement.Culture.RebelliousHeroTemplates;
			List<Hero> list = new List<Hero>
			{
				this.CreateRebelLeader(rebelliousHeroTemplates.GetRandomElement<CharacterObject>(), settlement),
				this.CreateRebelGovernor(rebelliousHeroTemplates.GetRandomElement<CharacterObject>(), settlement),
				this.CreateRebelSupporterHero(rebelliousHeroTemplates.GetRandomElement<CharacterObject>(), settlement),
				this.CreateRebelSupporterHero(rebelliousHeroTemplates.GetRandomElement<CharacterObject>(), settlement)
			};
			int clanIdForNewRebelClan = this.GetClanIdForNewRebelClan(settlement.Culture);
			Clan clan = Clan.CreateSettlementRebelClan(settlement, list[0], clanIdForNewRebelClan);
			clan.IsNoble = true;
			clan.AddRenown((float)MBRandom.RandomInt(200, 300), true);
			foreach (Hero hero in list)
			{
				hero.Clan = clan;
			}
			this._rebelClansAndDaysPassedAfterCreation.Add(clan, 1);
			MobileParty mobileParty = MobilePartyHelper.SpawnLordParty(list[0], settlement);
			MobilePartyHelper.SpawnLordParty(list[2], settlement);
			MobilePartyHelper.SpawnLordParty(list[3], settlement);
			IFaction mapFaction = settlement.MapFaction;
			ChangeOwnerOfSettlementAction.ApplyByRebellion(mobileParty.LeaderHero, settlement);
			DeclareWarAction.ApplyByRebellion(clan, mapFaction);
			foreach (Hero hero2 in list)
			{
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(mapFaction.Leader, hero2, MBRandom.RandomInt(-85, -75), true);
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (!kingdom.IsEliminated && kingdom.Culture != mapFaction.Culture)
					{
						int num = 0;
						foreach (Town town in kingdom.Fiefs)
						{
							num += (town.IsTown ? 2 : 1);
						}
						int num2 = (int)(MBRandom.RandomFloat * MBRandom.RandomFloat * 30f - (float)num);
						int num3 = ((kingdom.Culture == clan.Culture) ? (num2 + MBRandom.RandomInt(55, 65)) : num2);
						kingdom.Leader.SetPersonalRelation(hero2, num3);
					}
				}
				foreach (Hero hero3 in list)
				{
					if (hero2 != hero3)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero2, hero3, MBRandom.RandomInt(5, 15), true);
					}
				}
				hero2.ChangeState(Hero.CharacterStates.Active);
			}
			ChangeGovernorAction.Apply(settlement.Town, list[1]);
			EnterSettlementAction.ApplyForParty(mobileParty, settlement);
			mobileParty.Ai.DisableForHours(5);
			list[0].ChangeHeroGold(50000);
		}

		private Hero CreateRebelLeader(CharacterObject templateCharacter, Settlement settlement)
		{
			return this.CreateRebelHeroInternal(templateCharacter, settlement, new Dictionary<SkillObject, int>
			{
				{
					DefaultSkills.Steward,
					MBRandom.RandomInt(100, 175)
				},
				{
					DefaultSkills.Leadership,
					MBRandom.RandomInt(125, 175)
				},
				{
					DefaultSkills.OneHanded,
					MBRandom.RandomInt(125, 175)
				}
			});
		}

		private Hero CreateRebelGovernor(CharacterObject templateCharacter, Settlement settlement)
		{
			return this.CreateRebelHeroInternal(templateCharacter, settlement, new Dictionary<SkillObject, int>
			{
				{
					DefaultSkills.Steward,
					MBRandom.RandomInt(125, 200)
				},
				{
					DefaultSkills.Leadership,
					MBRandom.RandomInt(100, 125)
				},
				{
					DefaultSkills.OneHanded,
					MBRandom.RandomInt(60, 90)
				}
			});
		}

		private Hero CreateRebelSupporterHero(CharacterObject templateCharacter, Settlement settlement)
		{
			return this.CreateRebelHeroInternal(templateCharacter, settlement, new Dictionary<SkillObject, int>
			{
				{
					DefaultSkills.Steward,
					MBRandom.RandomInt(100, 175)
				},
				{
					DefaultSkills.Leadership,
					MBRandom.RandomInt(100, 175)
				},
				{
					DefaultSkills.OneHanded,
					MBRandom.RandomInt(125, 175)
				}
			});
		}

		private Hero CreateRebelHeroInternal(CharacterObject templateCharacter, Settlement settlement, Dictionary<SkillObject, int> startingSkills)
		{
			Hero hero = HeroCreator.CreateSpecialHero(templateCharacter, settlement, null, null, MBRandom.RandomInt(25, 40));
			foreach (KeyValuePair<SkillObject, int> keyValuePair in startingSkills)
			{
				hero.HeroDeveloper.SetInitialSkillLevel(keyValuePair.Key, keyValuePair.Value);
			}
			return hero;
		}

		private int GetClanIdForNewRebelClan(CultureObject culture)
		{
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			Dictionary<int, int> dictionary;
			if (!this._cultureIconIdAndFrequencies.TryGetValue(culture, out dictionary))
			{
				dictionary = new Dictionary<int, int>();
				this._cultureIconIdAndFrequencies.Add(culture, dictionary);
			}
			MBList<int> mblist = culture.PossibleClanBannerIconsIDs.ToMBList<int>();
			mblist.Shuffle<int>();
			foreach (int num3 in mblist)
			{
				int num4;
				if (!dictionary.TryGetValue(num3, out num4))
				{
					num4 = 0;
					dictionary.Add(num3, num4);
				}
				if (num4 < num2)
				{
					num = num3;
					num2 = num4;
				}
			}
			if (num == 2147483647)
			{
				foreach (KeyValuePair<CultureObject, Dictionary<int, int>> keyValuePair in this._cultureIconIdAndFrequencies)
				{
					foreach (KeyValuePair<int, int> keyValuePair2 in keyValuePair.Value)
					{
						if (keyValuePair2.Value < num2)
						{
							num = keyValuePair2.Key;
							num2 = keyValuePair2.Value;
						}
					}
				}
			}
			int num5 = num;
			int num6;
			if (this._cultureIconIdAndFrequencies[culture].TryGetValue(num5, out num6))
			{
				this._cultureIconIdAndFrequencies[culture][num5] = num6 + 1;
			}
			else
			{
				this._cultureIconIdAndFrequencies[culture].Add(num5, 1);
			}
			return num5;
		}

		private void InitializeIconIdAndFrequencies()
		{
			if (this._cultureIconIdAndFrequencies == null)
			{
				this._cultureIconIdAndFrequencies = new Dictionary<CultureObject, Dictionary<int, int>>();
			}
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (!this._cultureIconIdAndFrequencies.ContainsKey(kingdom.Culture))
				{
					this._cultureIconIdAndFrequencies.Add(kingdom.Culture, new Dictionary<int, int>());
				}
			}
			foreach (CultureObject cultureObject in MBObjectManager.Instance.GetObjectTypeList<CultureObject>())
			{
				if (!this._cultureIconIdAndFrequencies.ContainsKey(cultureObject))
				{
					this._cultureIconIdAndFrequencies.Add(cultureObject, new Dictionary<int, int>());
				}
			}
			foreach (CultureObject cultureObject2 in this._cultureIconIdAndFrequencies.Keys)
			{
				foreach (int num in cultureObject2.PossibleClanBannerIconsIDs)
				{
					if (!this._cultureIconIdAndFrequencies[cultureObject2].ContainsKey(num))
					{
						this._cultureIconIdAndFrequencies[cultureObject2].Add(num, 0);
					}
				}
			}
		}

		private const int UpdateClanAfterDays = 30;

		private const int LoyaltyAfterRebellion = 100;

		private const int InitialRelationPenalty = -80;

		private const int InitialRelationBoostWithOtherFactions = 10;

		private const int InitialRelationBoost = 60;

		private const int InitialRelationBetweenRebelHeroes = 10;

		private const int RebelClanStartingRenownMin = 200;

		private const int RebelClanStartingRenownMax = 300;

		private const int RebelHeroAgeMin = 25;

		private const int RebelHeroAgeMax = 40;

		private const float MilitiaGarrisonRatio = 1.4f;

		private const float ThrowGarrisonTroopToPrisonPercentage = 0.5f;

		private const float ThrowMilitiaTroopToGarrisonPercentage = 0.6f;

		private const float DailyRebellionCheckChance = 0.25f;

		private Dictionary<Clan, int> _rebelClansAndDaysPassedAfterCreation;

		private Dictionary<CultureObject, Dictionary<int, int>> _cultureIconIdAndFrequencies;

		private bool _rebellionEnabled = true;
	}
}
