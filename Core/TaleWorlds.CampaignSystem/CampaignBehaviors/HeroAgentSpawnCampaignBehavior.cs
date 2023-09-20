using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class HeroAgentSpawnCampaignBehavior : CampaignBehaviorBase
	{
		private static Location Prison
		{
			get
			{
				return LocationComplex.Current.GetLocationWithId("prison");
			}
		}

		private static Location LordsHall
		{
			get
			{
				return LocationComplex.Current.GetLocationWithId("lordshall");
			}
		}

		private static Location Tavern
		{
			get
			{
				return LocationComplex.Current.GetLocationWithId("tavern");
			}
		}

		private static Location Alley
		{
			get
			{
				return LocationComplex.Current.GetLocationWithId("alley");
			}
		}

		private static Location Center
		{
			get
			{
				return LocationComplex.Current.GetLocationWithId("center");
			}
		}

		private static Location VillageCenter
		{
			get
			{
				return LocationComplex.Current.GetLocationWithId("village_center");
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.PrisonersChangeInSettlement.AddNonSerializedListener(this, new Action<Settlement, FlattenedTroopRoster, Hero, bool>(this.OnPrisonersChangeInSettlement));
			CampaignEvents.OnGovernorChangedEvent.AddNonSerializedListener(this, new Action<Town, Hero, Hero>(this.OnGovernorChanged));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnGovernorChanged(Town town, Hero oldGovernor, Hero newGovernor)
		{
			if (oldGovernor != null)
			{
				LocationCharacter locationCharacterOfHero = town.Settlement.LocationComplex.GetLocationCharacterOfHero(oldGovernor);
				if (locationCharacterOfHero != null)
				{
					SettlementAccessModel.AccessDetails accessDetails;
					Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterLordsHall(town.Settlement, out accessDetails);
					Location locationOfCharacter = town.Settlement.LocationComplex.GetLocationOfCharacter(oldGovernor);
					Location location = ((accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.FullAccess) ? HeroAgentSpawnCampaignBehavior.LordsHall : (town.IsTown ? HeroAgentSpawnCampaignBehavior.Tavern : HeroAgentSpawnCampaignBehavior.Center));
					if (location != locationOfCharacter)
					{
						town.Settlement.LocationComplex.ChangeLocation(locationCharacterOfHero, locationOfCharacter, location);
					}
				}
			}
			if (newGovernor != null)
			{
				LocationCharacter locationCharacterOfHero2 = town.Settlement.LocationComplex.GetLocationCharacterOfHero(newGovernor);
				if (locationCharacterOfHero2 != null)
				{
					Location locationOfCharacter2 = town.Settlement.LocationComplex.GetLocationOfCharacter(newGovernor);
					if (locationOfCharacter2 != HeroAgentSpawnCampaignBehavior.LordsHall)
					{
						town.Settlement.LocationComplex.ChangeLocation(locationCharacterOfHero2, locationOfCharacter2, HeroAgentSpawnCampaignBehavior.LordsHall);
					}
				}
			}
		}

		private void OnMissionEnded(IMission mission)
		{
			if (LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null && Settlement.CurrentSettlement != null && !Hero.MainHero.IsPrisoner && !Settlement.CurrentSettlement.IsUnderSiege)
			{
				this.AddSettlementLocationCharacters(Settlement.CurrentSettlement);
				this._addNotableHelperCharacters = true;
			}
		}

		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null)
			{
				if (mobileParty != null)
				{
					if (mobileParty == MobileParty.MainParty)
					{
						this.AddSettlementLocationCharacters(settlement);
						this._addNotableHelperCharacters = true;
						return;
					}
					if (MobileParty.MainParty.CurrentSettlement == settlement && (settlement.IsFortification || settlement.IsVillage))
					{
						this.AddPartyHero(mobileParty, settlement);
						return;
					}
				}
				else if (MobileParty.MainParty.CurrentSettlement == settlement && hero != null)
				{
					if (hero.IsNotable)
					{
						this.AddNotableLocationCharacter(hero, settlement);
						return;
					}
					if (hero.IsWanderer)
					{
						this.AddWandererLocationCharacter(hero, settlement);
					}
				}
			}
		}

		public void OnSettlementLeft(MobileParty mobileParty, Settlement settlement)
		{
			if (mobileParty != MobileParty.MainParty && MobileParty.MainParty.CurrentSettlement == settlement && mobileParty.LeaderHero != null && LocationComplex.Current != null)
			{
				Location locationOfCharacter = LocationComplex.Current.GetLocationOfCharacter(mobileParty.LeaderHero);
				if (locationOfCharacter != null)
				{
					locationOfCharacter.RemoveCharacter(mobileParty.LeaderHero);
				}
			}
		}

		private void OnGameLoadFinished()
		{
			if (Settlement.CurrentSettlement != null && !Hero.MainHero.IsPrisoner && LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null && !Settlement.CurrentSettlement.IsUnderSiege)
			{
				this.AddSettlementLocationCharacters(Settlement.CurrentSettlement);
			}
		}

		private void AddSettlementLocationCharacters(Settlement settlement)
		{
			if (settlement.IsFortification || settlement.IsVillage)
			{
				List<MobileParty> list = Settlement.CurrentSettlement.Parties.ToList<MobileParty>();
				if (settlement.IsFortification)
				{
					this.AddLordsHallCharacters(settlement, ref list);
					this.RefreshPrisonCharacters(settlement);
					this.AddCompanionsAndClanMembersToSettlement(settlement);
					if (settlement.IsFortification)
					{
						this.AddNotablesAndWanderers(settlement);
					}
				}
				else if (settlement.IsVillage)
				{
					this.AddHeroesWithoutPartyCharactersToVillage(settlement);
					this.AddCompanionsAndClanMembersToSettlement(settlement);
				}
				foreach (MobileParty mobileParty in list)
				{
					this.AddPartyHero(mobileParty, settlement);
				}
			}
		}

		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			if (this._addNotableHelperCharacters && (CampaignMission.Current.Location == HeroAgentSpawnCampaignBehavior.Center || CampaignMission.Current.Location == HeroAgentSpawnCampaignBehavior.VillageCenter))
			{
				this.SpawnNotableHelperCharacters(settlement);
				this._addNotableHelperCharacters = false;
			}
		}

		private void AddCompanionsAndClanMembersToSettlement(Settlement settlement)
		{
			if (settlement.IsFortification || settlement.IsVillage)
			{
				foreach (Hero hero in Clan.PlayerClan.Lords)
				{
					int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
					if (hero != Hero.MainHero && hero.Age >= (float)heroComesOfAge && !hero.IsPrisoner && hero.CurrentSettlement == settlement && (hero.GovernorOf == null || hero.GovernorOf != settlement.Town))
					{
						Location location;
						if (settlement.IsFortification)
						{
							SettlementAccessModel.AccessDetails accessDetails;
							Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterLordsHall(settlement, out accessDetails);
							location = ((accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.FullAccess) ? HeroAgentSpawnCampaignBehavior.LordsHall : (settlement.IsTown ? HeroAgentSpawnCampaignBehavior.Tavern : HeroAgentSpawnCampaignBehavior.Center));
						}
						else
						{
							location = HeroAgentSpawnCampaignBehavior.VillageCenter;
						}
						IFaction mapFaction = hero.MapFaction;
						uint num = ((mapFaction != null) ? mapFaction.Color : 4291609515U);
						IFaction mapFaction2 = hero.MapFaction;
						uint num2 = ((mapFaction2 != null) ? mapFaction2.Color : 4291609515U);
						Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(hero.CharacterObject.Race);
						AgentData agentData = new AgentData(new PartyAgentOrigin(PartyBase.MainParty, hero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(baseMonsterFromRace).NoHorses(true).ClothingColor1(num)
							.ClothingColor2(num2);
						location.AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddCompanionBehaviors), "sp_notable", true, LocationCharacter.CharacterRelations.Friendly, null, !PlayerEncounter.LocationEncounter.Settlement.IsVillage, false, null, false, true, true));
					}
				}
				using (IEnumerator<Hero> enumerator2 = Hero.MainHero.CompanionsInParty.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Hero companion = enumerator2.Current;
						if (!companion.IsWounded && !PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer.Exists((AccompanyingCharacter x) => x.LocationCharacter.Character.HeroObject == companion))
						{
							IFaction mapFaction3 = companion.MapFaction;
							uint num3 = ((mapFaction3 != null) ? mapFaction3.Color : 4291609515U);
							IFaction mapFaction4 = companion.MapFaction;
							uint num4 = ((mapFaction4 != null) ? mapFaction4.Color : 4291609515U);
							Monster baseMonsterFromRace2 = FaceGen.GetBaseMonsterFromRace(companion.CharacterObject.Race);
							Location location = (settlement.IsFortification ? HeroAgentSpawnCampaignBehavior.Center : HeroAgentSpawnCampaignBehavior.VillageCenter);
							AgentData agentData2 = new AgentData(new PartyAgentOrigin(PartyBase.MainParty, companion.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(baseMonsterFromRace2).NoHorses(true).ClothingColor1(num3)
								.ClothingColor2(num4);
							location.AddCharacter(new LocationCharacter(agentData2, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddCompanionBehaviors), "sp_notable", true, LocationCharacter.CharacterRelations.Friendly, null, !PlayerEncounter.LocationEncounter.Settlement.IsVillage, false, null, false, true, true));
						}
					}
				}
			}
		}

		private void AddPartyHero(MobileParty mobileParty, Settlement settlement)
		{
			Hero leaderHero = mobileParty.LeaderHero;
			if (leaderHero == null || leaderHero == Hero.MainHero)
			{
				return;
			}
			IFaction mapFaction = leaderHero.MapFaction;
			uint num = ((mapFaction != null) ? mapFaction.Color : 4291609515U);
			IFaction mapFaction2 = leaderHero.MapFaction;
			uint num2 = ((mapFaction2 != null) ? mapFaction2.Color : 4291609515U);
			Tuple<string, Monster> actionSetAndMonster = HeroAgentSpawnCampaignBehavior.GetActionSetAndMonster(leaderHero.CharacterObject);
			AgentData agentData = new AgentData(new PartyAgentOrigin(mobileParty.Party, leaderHero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(actionSetAndMonster.Item2).NoHorses(true).ClothingColor1(num)
				.ClothingColor2(num2);
			string text = "sp_notable";
			Location location;
			if (settlement.IsFortification)
			{
				location = HeroAgentSpawnCampaignBehavior.LordsHall;
			}
			else if (settlement.IsVillage)
			{
				location = HeroAgentSpawnCampaignBehavior.VillageCenter;
			}
			else
			{
				location = HeroAgentSpawnCampaignBehavior.Center;
			}
			if (location != null)
			{
				location.AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), text, true, LocationCharacter.CharacterRelations.Neutral, actionSetAndMonster.Item1, !settlement.IsVillage, false, null, false, false, true));
			}
		}

		private void OnHeroPrisonerTaken(PartyBase capturerParty, Hero prisoner)
		{
			if (capturerParty.IsSettlement)
			{
				this.OnPrisonersChangeInSettlement(capturerParty.Settlement, null, prisoner, false);
			}
		}

		public void OnPrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool takenFromDungeon)
		{
			if (settlement != null && settlement.IsFortification && LocationComplex.Current == settlement.LocationComplex)
			{
				if (prisonerHero != null)
				{
					this.SendPrisonerHeroToNextLocation(settlement, prisonerHero, takenFromDungeon);
				}
				if (prisonerRoster != null)
				{
					foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in prisonerRoster)
					{
						if (flattenedTroopRosterElement.Troop.IsHero)
						{
							this.SendPrisonerHeroToNextLocation(settlement, flattenedTroopRosterElement.Troop.HeroObject, takenFromDungeon);
						}
					}
				}
			}
		}

		private void SendPrisonerHeroToNextLocation(Settlement settlement, Hero hero, bool takenFromDungeon)
		{
			Location locationOfCharacter = LocationComplex.Current.GetLocationOfCharacter(hero);
			Location location = this.DecideNewLocationOnPrisonerChange(settlement, hero, takenFromDungeon);
			LocationCharacter locationCharacterOfHero = LocationComplex.Current.GetLocationCharacterOfHero(hero);
			if (locationCharacterOfHero == null)
			{
				if (location != null)
				{
					this.AddHeroToDecidedLocation(location, hero, settlement);
					return;
				}
			}
			else if (locationOfCharacter != location)
			{
				LocationComplex.Current.ChangeLocation(locationCharacterOfHero, locationOfCharacter, location);
			}
		}

		private Location DecideNewLocationOnPrisonerChange(Settlement settlement, Hero hero, bool takenFromDungeon)
		{
			if (hero.IsPrisoner)
			{
				if (!takenFromDungeon)
				{
					return HeroAgentSpawnCampaignBehavior.Prison;
				}
				return null;
			}
			else
			{
				if (!settlement.IsFortification)
				{
					return HeroAgentSpawnCampaignBehavior.VillageCenter;
				}
				if (hero.IsWanderer && settlement.IsTown)
				{
					return HeroAgentSpawnCampaignBehavior.Tavern;
				}
				if (hero.CharacterObject.Occupation == Occupation.Lord)
				{
					return HeroAgentSpawnCampaignBehavior.LordsHall;
				}
				return HeroAgentSpawnCampaignBehavior.Center;
			}
		}

		private void AddHeroToDecidedLocation(Location location, Hero hero, Settlement settlement)
		{
			if (location == HeroAgentSpawnCampaignBehavior.Prison)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(hero.CharacterObject.Race, "_settlement");
				AgentData agentData = new AgentData(new SimpleAgentOrigin(hero.CharacterObject, -1, null, default(UniqueTroopDescriptor))).NoWeapons(true).Monster(monsterWithSuffix).NoHorses(true);
				location.AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_prisoner", true, LocationCharacter.CharacterRelations.Neutral, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_villager"), true, false, null, false, false, true));
				return;
			}
			if (location == HeroAgentSpawnCampaignBehavior.VillageCenter)
			{
				Monster monsterWithSuffix2 = FaceGen.GetMonsterWithSuffix(hero.CharacterObject.Race, "_settlement");
				AgentData agentData2 = new AgentData(new PartyAgentOrigin(null, hero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(monsterWithSuffix2);
				location.AddCharacter(new LocationCharacter(agentData2, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_notable_rural_notable", false, LocationCharacter.CharacterRelations.Neutral, null, true, false, null, false, false, true));
				return;
			}
			if (location == HeroAgentSpawnCampaignBehavior.Tavern)
			{
				Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(hero.CharacterObject.Race);
				AgentData agentData3 = new AgentData(new PartyAgentOrigin(PartyBase.MainParty, hero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(baseMonsterFromRace).NoHorses(true);
				location.AddCharacter(new LocationCharacter(agentData3, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddCompanionBehaviors), null, true, LocationCharacter.CharacterRelations.Friendly, null, !PlayerEncounter.LocationEncounter.Settlement.IsVillage, false, null, false, true, true));
				return;
			}
			if (location == HeroAgentSpawnCampaignBehavior.LordsHall)
			{
				Tuple<string, Monster> actionSetAndMonster = HeroAgentSpawnCampaignBehavior.GetActionSetAndMonster(hero.CharacterObject);
				AgentData agentData4 = new AgentData(new SimpleAgentOrigin(hero.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(actionSetAndMonster.Item2).NoHorses(true);
				location.AddCharacter(new LocationCharacter(agentData4, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), null, true, LocationCharacter.CharacterRelations.Neutral, actionSetAndMonster.Item1, true, false, null, false, false, true));
				return;
			}
			if (location == HeroAgentSpawnCampaignBehavior.Center)
			{
				if (hero.IsNotable)
				{
					this.AddNotableLocationCharacter(hero, settlement);
					return;
				}
				Monster monsterWithSuffix3 = FaceGen.GetMonsterWithSuffix(hero.CharacterObject.Race, "_settlement");
				AgentData agentData5 = new AgentData(new PartyAgentOrigin(null, hero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(monsterWithSuffix3);
				location.AddCharacter(new LocationCharacter(agentData5, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_notable_rural_notable", false, LocationCharacter.CharacterRelations.Neutral, null, true, false, null, false, false, true));
			}
		}

		private void AddLordsHallCharacters(Settlement settlement, ref List<MobileParty> partiesToBeSpawn)
		{
			Hero hero = null;
			Hero hero2 = null;
			if (settlement.MapFaction.IsKingdomFaction)
			{
				Hero leader = ((Kingdom)settlement.MapFaction).Leader;
				if (leader.CurrentSettlement == settlement)
				{
					hero = leader;
				}
				if (leader.Spouse != null && leader.Spouse.CurrentSettlement == settlement)
				{
					hero2 = leader.Spouse;
				}
			}
			if (hero == null && settlement.OwnerClan.Leader.CurrentSettlement == settlement)
			{
				hero = settlement.OwnerClan.Leader;
			}
			if (hero2 == null && settlement.OwnerClan.Leader.Spouse != null && settlement.OwnerClan.Leader.Spouse.CurrentSettlement == settlement)
			{
				hero2 = settlement.OwnerClan.Leader.Spouse;
			}
			bool flag = false;
			if (hero != null && hero != Hero.MainHero)
			{
				IFaction mapFaction = hero.MapFaction;
				uint num = ((mapFaction != null) ? mapFaction.Color : 4291609515U);
				IFaction mapFaction2 = hero.MapFaction;
				uint num2 = ((mapFaction2 != null) ? mapFaction2.Color : 4291609515U);
				flag = true;
				Tuple<string, Monster> actionSetAndMonster = HeroAgentSpawnCampaignBehavior.GetActionSetAndMonster(hero.CharacterObject);
				AgentData agentData = new AgentData(new PartyAgentOrigin(null, hero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(actionSetAndMonster.Item2).NoHorses(true).ClothingColor1(num)
					.ClothingColor2(num2);
				HeroAgentSpawnCampaignBehavior.LordsHall.AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_throne", true, LocationCharacter.CharacterRelations.Neutral, actionSetAndMonster.Item1, true, false, null, false, false, true));
				if (hero.PartyBelongedTo != null && partiesToBeSpawn.Contains(hero.PartyBelongedTo))
				{
					partiesToBeSpawn.Remove(hero.PartyBelongedTo);
				}
			}
			if (hero2 != null && hero2 != Hero.MainHero)
			{
				IFaction mapFaction3 = hero2.MapFaction;
				uint num3 = ((mapFaction3 != null) ? mapFaction3.Color : 4291609515U);
				IFaction mapFaction4 = hero2.MapFaction;
				uint num4 = ((mapFaction4 != null) ? mapFaction4.Color : 4291609515U);
				Tuple<string, Monster> actionSetAndMonster2 = HeroAgentSpawnCampaignBehavior.GetActionSetAndMonster(hero2.CharacterObject);
				AgentData agentData2 = new AgentData(new PartyAgentOrigin(null, hero2.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(actionSetAndMonster2.Item2).NoHorses(true).ClothingColor1(num3)
					.ClothingColor2(num4);
				HeroAgentSpawnCampaignBehavior.LordsHall.AddCharacter(new LocationCharacter(agentData2, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), flag ? "sp_notable" : "sp_throne", true, LocationCharacter.CharacterRelations.Neutral, actionSetAndMonster2.Item1, true, false, null, false, false, true));
				if (hero2.PartyBelongedTo != null && partiesToBeSpawn.Contains(hero2.PartyBelongedTo))
				{
					partiesToBeSpawn.Remove(hero2.PartyBelongedTo);
				}
			}
			int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
			foreach (Hero hero3 in settlement.HeroesWithoutParty)
			{
				if (hero3 != hero && hero3 != hero2 && hero3.Age >= (float)heroComesOfAge && !hero3.IsPrisoner && (hero3.Clan != Clan.PlayerClan || (hero3.GovernorOf != null && hero3.GovernorOf == settlement.Town)))
				{
					Tuple<string, Monster> actionSetAndMonster3 = HeroAgentSpawnCampaignBehavior.GetActionSetAndMonster(hero3.CharacterObject);
					IFaction mapFaction5 = hero3.MapFaction;
					uint num5 = ((mapFaction5 != null) ? mapFaction5.Color : 4291609515U);
					IFaction mapFaction6 = hero3.MapFaction;
					uint num6 = ((mapFaction6 != null) ? mapFaction6.Color : 4291609515U);
					AgentData agentData3 = new AgentData(new SimpleAgentOrigin(hero3.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(actionSetAndMonster3.Item2).NoHorses(true).ClothingColor1(num5)
						.ClothingColor2(num6);
					HeroAgentSpawnCampaignBehavior.LordsHall.AddCharacter(new LocationCharacter(agentData3, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_notable", true, LocationCharacter.CharacterRelations.Neutral, actionSetAndMonster3.Item1, true, false, null, false, false, true));
				}
			}
		}

		private void RefreshPrisonCharacters(Settlement settlement)
		{
			HeroAgentSpawnCampaignBehavior.Prison.RemoveAllHeroCharactersFromPrison();
			List<CharacterObject> prisonerHeroes = settlement.SettlementComponent.GetPrisonerHeroes();
			if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
			{
				for (int i = 0; i < 5; i++)
				{
					prisonerHeroes.Add(Game.Current.ObjectManager.GetObject<CharacterObject>("townsman_empire"));
				}
			}
			foreach (CharacterObject characterObject in prisonerHeroes)
			{
				Hero heroObject = characterObject.HeroObject;
				uint? num;
				if (heroObject == null)
				{
					num = null;
				}
				else
				{
					IFaction mapFaction = heroObject.MapFaction;
					num = ((mapFaction != null) ? new uint?(mapFaction.Color) : null);
				}
				uint num2 = num ?? 4291609515U;
				Hero heroObject2 = characterObject.HeroObject;
				uint? num3;
				if (heroObject2 == null)
				{
					num3 = null;
				}
				else
				{
					IFaction mapFaction2 = heroObject2.MapFaction;
					num3 = ((mapFaction2 != null) ? new uint?(mapFaction2.Color) : null);
				}
				uint num4 = num3 ?? 4291609515U;
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(characterObject.Race, "_settlement");
				AgentData agentData = new AgentData(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).NoWeapons(true).Monster(monsterWithSuffix).NoHorses(true)
					.ClothingColor1(num2)
					.ClothingColor2(num4);
				HeroAgentSpawnCampaignBehavior.Prison.AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_prisoner", true, LocationCharacter.CharacterRelations.Neutral, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_villager"), true, false, null, false, false, true));
			}
		}

		private void AddNotablesAndWanderers(Settlement settlement)
		{
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				foreach (Hero hero in settlement.Notables)
				{
					this.AddNotableLocationCharacter(hero, settlement);
				}
				foreach (Hero hero2 in settlement.HeroesWithoutParty.Where((Hero x) => x.IsWanderer || x.IsPlayerCompanion))
				{
					if (hero2.GovernorOf == null || hero2.GovernorOf != settlement.Town)
					{
						this.AddWandererLocationCharacter(hero2, settlement);
					}
				}
			}
		}

		private void AddWandererLocationCharacter(Hero wanderer, Settlement settlement)
		{
			bool flag = settlement.Culture.StringId.ToLower() == "aserai" || settlement.Culture.StringId.ToLower() == "khuzait";
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(wanderer.CharacterObject.Race, "_settlement");
			string text = (flag ? ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, wanderer.IsFemale, "_warrior_in_aserai_tavern") : ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, wanderer.IsFemale, "_warrior_in_tavern"));
			LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new PartyAgentOrigin(null, wanderer.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(monsterWithSuffix).NoHorses(true), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "npc_common", true, LocationCharacter.CharacterRelations.Neutral, text, true, false, null, false, false, true);
			if (settlement.IsCastle)
			{
				HeroAgentSpawnCampaignBehavior.Center.AddCharacter(locationCharacter);
				return;
			}
			if (settlement.IsTown)
			{
				Location location;
				if (CampaignBehaviorBase.GetCampaignBehavior<IAlleyCampaignBehavior>().IsHeroAlleyLeaderOfAnyPlayerAlley(wanderer))
				{
					location = HeroAgentSpawnCampaignBehavior.Alley;
				}
				else
				{
					location = HeroAgentSpawnCampaignBehavior.Tavern;
				}
				location.AddCharacter(locationCharacter);
				return;
			}
			HeroAgentSpawnCampaignBehavior.VillageCenter.AddCharacter(locationCharacter);
		}

		private void AddNotableLocationCharacter(Hero notable, Settlement settlement)
		{
			string text = (notable.IsArtisan ? "_villager_artisan" : (notable.IsMerchant ? "_villager_merchant" : (notable.IsPreacher ? "_villager_preacher" : (notable.IsGangLeader ? "_villager_gangleader" : (notable.IsRuralNotable ? "_villager_ruralnotable" : (notable.IsFemale ? "_lord" : "_villager_merchant"))))));
			string text2 = (notable.IsArtisan ? "sp_notable_artisan" : (notable.IsMerchant ? "sp_notable_merchant" : (notable.IsPreacher ? "sp_notable_preacher" : (notable.IsGangLeader ? "sp_notable_gangleader" : (notable.IsRuralNotable ? "sp_notable_rural_notable" : ((notable.GovernorOf == notable.CurrentSettlement.Town) ? "sp_governor" : "sp_notable"))))));
			MBReadOnlyList<Workshop> ownedWorkshops = notable.OwnedWorkshops;
			if (ownedWorkshops.Count != 0)
			{
				for (int i = 0; i < ownedWorkshops.Count; i++)
				{
					if (!ownedWorkshops[i].WorkshopType.IsHidden)
					{
						text2 = text2 + "_" + ownedWorkshops[i].Tag;
						break;
					}
				}
			}
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(notable.CharacterObject.Race, "_settlement");
			AgentData agentData = new AgentData(new PartyAgentOrigin(null, notable.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(monsterWithSuffix).NoHorses(true);
			LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), text2, true, LocationCharacter.CharacterRelations.Neutral, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, notable.IsFemale, text), true, false, null, false, false, true);
			if (settlement.IsVillage)
			{
				HeroAgentSpawnCampaignBehavior.VillageCenter.AddCharacter(locationCharacter);
				return;
			}
			HeroAgentSpawnCampaignBehavior.Center.AddCharacter(locationCharacter);
		}

		private void AddHeroesWithoutPartyCharactersToVillage(Settlement settlement)
		{
			foreach (Hero hero in settlement.HeroesWithoutParty)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(hero.CharacterObject.Race, "_settlement");
				AgentData agentData = new AgentData(new PartyAgentOrigin(null, hero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(monsterWithSuffix);
				HeroAgentSpawnCampaignBehavior.VillageCenter.AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_notable_rural_notable", false, LocationCharacter.CharacterRelations.Neutral, null, true, false, null, false, false, true));
			}
		}

		private void SpawnNotableHelperCharacters(Settlement settlement)
		{
			int num = settlement.Notables.Count((Hero x) => x.IsGangLeader);
			int num2 = settlement.Notables.Count((Hero x) => x.IsPreacher);
			int num3 = settlement.Notables.Count((Hero x) => x.IsArtisan);
			int num4 = settlement.Notables.Count((Hero x) => x.IsRuralNotable || x.IsHeadman);
			int num5 = settlement.Notables.Count((Hero x) => x.IsMerchant);
			this.SpawnNotableHelperCharacter(settlement.Culture.GangleaderBodyguard, "_gangleader_bodyguard", "sp_gangleader_bodyguard", num * 2);
			this.SpawnNotableHelperCharacter(settlement.Culture.PreacherNotary, "_merchant_notary", "sp_preacher_notary", num2);
			this.SpawnNotableHelperCharacter(settlement.Culture.ArtisanNotary, "_merchant_notary", "sp_artisan_notary", num3);
			this.SpawnNotableHelperCharacter(settlement.Culture.RuralNotableNotary, "_merchant_notary", "sp_rural_notable_notary", num4);
			this.SpawnNotableHelperCharacter(settlement.Culture.MerchantNotary, "_merchant_notary", "sp_merchant_notary", num5);
		}

		private void SpawnNotableHelperCharacter(CharacterObject character, string actionSetSuffix, string tag, int characterToSpawnCount)
		{
			Location location = HeroAgentSpawnCampaignBehavior.Center ?? HeroAgentSpawnCampaignBehavior.VillageCenter;
			while (characterToSpawnCount > 0)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(character.Race, "_settlement");
				int num;
				int num2;
				Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(character, out num, out num2, "Notary");
				AgentData agentData = new AgentData(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).NoHorses(true).Age(MBRandom.RandomInt(num, num2));
				LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), tag, true, LocationCharacter.CharacterRelations.Neutral, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, actionSetSuffix), true, false, null, false, false, true);
				location.AddCharacter(locationCharacter);
				characterToSpawnCount--;
			}
		}

		private static Tuple<string, Monster> GetActionSetAndMonster(CharacterObject character)
		{
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(character.Race, "_settlement");
			return new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, character.IsFemale, "_lord"), monsterWithSuffix);
		}

		[NonSerialized]
		private bool _addNotableHelperCharacters;
	}
}
