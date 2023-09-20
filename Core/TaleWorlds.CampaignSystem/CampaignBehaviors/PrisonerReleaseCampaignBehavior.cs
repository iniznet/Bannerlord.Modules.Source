using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003C9 RID: 969
	public class PrisonerReleaseCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003A14 RID: 14868 RVA: 0x0010B36C File Offset: 0x0010956C
		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyHeroTick));
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.HourlyPartyTick));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeaceEvent));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.ClanChangedKingdom));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
		}

		// Token: 0x06003A15 RID: 14869 RVA: 0x0010B404 File Offset: 0x00109604
		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.1.0", 17949))
			{
				foreach (Hero hero in Hero.AllAliveHeroes)
				{
					if (hero != Hero.MainHero && hero.IsPrisoner)
					{
						if (hero.PartyBelongedToAsPrisoner == null)
						{
							if (hero.CurrentSettlement == null)
							{
								MakeHeroFugitiveAction.Apply(hero);
							}
						}
						else if (hero.PartyBelongedToAsPrisoner.IsMobile && hero.PartyBelongedToAsPrisoner.MobileParty.IsMilitia)
						{
							EndCaptivityAction.ApplyByEscape(hero, null);
							MakeHeroFugitiveAction.Apply(hero);
						}
					}
					if (hero != Hero.MainHero && !hero.IsPrisoner && hero.PartyBelongedToAsPrisoner != null)
					{
						hero.PartyBelongedToAsPrisoner.PrisonRoster.RemoveTroop(hero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
						MakeHeroFugitiveAction.Apply(hero);
					}
				}
			}
		}

		// Token: 0x06003A16 RID: 14870 RVA: 0x0010B510 File Offset: 0x00109710
		private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (detail != ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom)
			{
				PrisonerReleaseCampaignBehavior.ReleasePrisonersInternal(clan);
				if (oldKingdom != null)
				{
					PrisonerReleaseCampaignBehavior.ReleasePrisonersInternal(oldKingdom);
				}
				if (newKingdom != null)
				{
					this.OnAfterClanJoinedKingdom(clan, newKingdom);
					PrisonerReleaseCampaignBehavior.ReleasePrisonersInternal(newKingdom);
				}
			}
		}

		// Token: 0x06003A17 RID: 14871 RVA: 0x0010B538 File Offset: 0x00109738
		private void OnAfterClanJoinedKingdom(Clan clan, Kingdom newKingdom)
		{
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (kingdom != newKingdom && kingdom.IsAtWarWith(clan) && !kingdom.IsAtWarWith(newKingdom))
				{
					this.OnMakePeace(clan, kingdom);
				}
			}
		}

		// Token: 0x06003A18 RID: 14872 RVA: 0x0010B5A4 File Offset: 0x001097A4
		private void OnMakePeaceEvent(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			this.OnMakePeace(side1Faction, side2Faction);
		}

		// Token: 0x06003A19 RID: 14873 RVA: 0x0010B5AE File Offset: 0x001097AE
		private void OnMakePeace(IFaction side1Faction, IFaction side2Faction)
		{
			PrisonerReleaseCampaignBehavior.ReleasePrisonersInternal(side1Faction);
			PrisonerReleaseCampaignBehavior.ReleasePrisonersInternal(side2Faction);
		}

		// Token: 0x06003A1A RID: 14874 RVA: 0x0010B5BC File Offset: 0x001097BC
		private static void ReleasePrisonersInternal(IFaction faction)
		{
			foreach (Settlement settlement in faction.Settlements)
			{
				for (int i = settlement.Party.PrisonRoster.Count - 1; i >= 0; i--)
				{
					if (settlement.Party.PrisonRoster.GetElementNumber(i) > 0)
					{
						TroopRosterElement elementCopyAtIndex = settlement.Party.PrisonRoster.GetElementCopyAtIndex(i);
						if (elementCopyAtIndex.Character.IsHero && elementCopyAtIndex.Character.HeroObject != Hero.MainHero && !elementCopyAtIndex.Character.HeroObject.MapFaction.IsAtWarWith(faction.MapFaction))
						{
							EndCaptivityAction.ApplyByPeace(elementCopyAtIndex.Character.HeroObject, null);
							CampaignEventDispatcher.Instance.OnPrisonersChangeInSettlement(settlement, null, elementCopyAtIndex.Character.HeroObject, true);
						}
					}
				}
			}
			Clan clan = ((faction.IsClan || faction.IsMinorFaction) ? ((Clan)faction) : null);
			Kingdom kingdom = (faction.IsKingdomFaction ? ((Kingdom)faction) : null);
			if (clan != null)
			{
				PrisonerReleaseCampaignBehavior.ReleasePrisonersForClan(clan, faction);
				return;
			}
			if (kingdom != null)
			{
				foreach (Clan clan2 in kingdom.Clans)
				{
					PrisonerReleaseCampaignBehavior.ReleasePrisonersForClan(clan2, faction);
				}
			}
		}

		// Token: 0x06003A1B RID: 14875 RVA: 0x0010B744 File Offset: 0x00109944
		private static void ReleasePrisonersForClan(Clan clan, IFaction faction)
		{
			foreach (Hero hero in clan.Lords)
			{
				foreach (CaravanPartyComponent caravanPartyComponent in hero.OwnedCaravans)
				{
					PrisonerReleaseCampaignBehavior.ReleasePartyPrisoners(caravanPartyComponent.MobileParty, faction);
				}
			}
			foreach (Hero hero2 in clan.Companions)
			{
				foreach (CaravanPartyComponent caravanPartyComponent2 in hero2.OwnedCaravans)
				{
					PrisonerReleaseCampaignBehavior.ReleasePartyPrisoners(caravanPartyComponent2.MobileParty, faction);
				}
			}
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				PrisonerReleaseCampaignBehavior.ReleasePartyPrisoners(warPartyComponent.MobileParty, faction);
			}
			foreach (Settlement settlement in clan.Settlements)
			{
				if (settlement.IsVillage && settlement.Village.VillagerPartyComponent != null)
				{
					PrisonerReleaseCampaignBehavior.ReleasePartyPrisoners(settlement.Village.VillagerPartyComponent.MobileParty, faction);
				}
				else if ((settlement.IsCastle || settlement.IsTown) && settlement.Town.GarrisonParty != null)
				{
					PrisonerReleaseCampaignBehavior.ReleasePartyPrisoners(settlement.Town.GarrisonParty, faction);
				}
			}
		}

		// Token: 0x06003A1C RID: 14876 RVA: 0x0010B934 File Offset: 0x00109B34
		private static void ReleasePartyPrisoners(MobileParty mobileParty, IFaction faction)
		{
			for (int i = mobileParty.PrisonRoster.Count - 1; i >= 0; i--)
			{
				if (mobileParty.Party.PrisonRoster.GetElementNumber(i) > 0)
				{
					TroopRosterElement elementCopyAtIndex = mobileParty.Party.PrisonRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.Character.IsHero && elementCopyAtIndex.Character.HeroObject != Hero.MainHero && !elementCopyAtIndex.Character.HeroObject.MapFaction.IsAtWarWith(faction.MapFaction))
					{
						if (elementCopyAtIndex.Character.HeroObject.PartyBelongedToAsPrisoner == mobileParty.Party)
						{
							EndCaptivityAction.ApplyByPeace(elementCopyAtIndex.Character.HeroObject, null);
						}
						else
						{
							mobileParty.Party.PrisonRoster.RemoveTroop(elementCopyAtIndex.Character, 1, default(UniqueTroopDescriptor), 0);
						}
					}
				}
			}
		}

		// Token: 0x06003A1D RID: 14877 RVA: 0x0010BA10 File Offset: 0x00109C10
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003A1E RID: 14878 RVA: 0x0010BA14 File Offset: 0x00109C14
		private void DailyHeroTick(Hero hero)
		{
			if (hero.IsPrisoner && hero.PartyBelongedToAsPrisoner != null && hero != Hero.MainHero)
			{
				float num = 0.04f;
				if (hero.PartyBelongedToAsPrisoner.IsMobile && hero.PartyBelongedToAsPrisoner.MobileParty.CurrentSettlement == null)
				{
					num *= 5f - MathF.Pow((float)MathF.Min(81, hero.PartyBelongedToAsPrisoner.NumberOfHealthyMembers), 0.25f);
				}
				if (hero.PartyBelongedToAsPrisoner == PartyBase.MainParty || (hero.PartyBelongedToAsPrisoner.IsSettlement && hero.PartyBelongedToAsPrisoner.Settlement.OwnerClan == Clan.PlayerClan) || (hero.PartyBelongedToAsPrisoner.IsMobile && hero.PartyBelongedToAsPrisoner.MobileParty.CurrentSettlement != null && hero.PartyBelongedToAsPrisoner.MobileParty.CurrentSettlement.OwnerClan == Clan.PlayerClan))
				{
					num *= 0.5f;
				}
				ExplainedNumber explainedNumber = new ExplainedNumber(num, false, null);
				if (hero.PartyBelongedToAsPrisoner.IsSettlement && hero.PartyBelongedToAsPrisoner.Settlement.Town != null && hero.PartyBelongedToAsPrisoner.Settlement.Town.Governor != null)
				{
					Town town = hero.PartyBelongedToAsPrisoner.Settlement.Town;
					if (hero.PartyBelongedToAsPrisoner.Settlement.IsTown)
					{
						if (town.Governor.GetPerkValue(DefaultPerks.Riding.MountedPatrols))
						{
							explainedNumber.AddFactor(DefaultPerks.Riding.MountedPatrols.SecondaryBonus, DefaultPerks.Riding.MountedPatrols.Description);
						}
						if (town.Governor.GetPerkValue(DefaultPerks.Roguery.SweetTalker))
						{
							explainedNumber.AddFactor(DefaultPerks.Roguery.SweetTalker.SecondaryBonus, DefaultPerks.Roguery.SweetTalker.Description);
						}
					}
					if ((hero.PartyBelongedToAsPrisoner.Settlement.IsTown || hero.PartyBelongedToAsPrisoner.Settlement.IsCastle) && town.Governor.GetPerkValue(DefaultPerks.Engineering.DungeonArchitect))
					{
						explainedNumber.AddFactor(DefaultPerks.Engineering.DungeonArchitect.SecondaryBonus, DefaultPerks.Engineering.DungeonArchitect.Description);
					}
				}
				if (hero.PartyBelongedToAsPrisoner.IsMobile)
				{
					if (hero.GetPerkValue(DefaultPerks.Roguery.FleetFooted))
					{
						explainedNumber.AddFactor(DefaultPerks.Roguery.FleetFooted.SecondaryBonus, null);
					}
					if (hero.PartyBelongedToAsPrisoner.MobileParty.HasPerk(DefaultPerks.Riding.MountedPatrols, false))
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Riding.MountedPatrols, hero.PartyBelongedToAsPrisoner.MobileParty, true, ref explainedNumber);
					}
					if (hero.PartyBelongedToAsPrisoner.MobileParty.HasPerk(DefaultPerks.Roguery.RansomBroker, false))
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Roguery.RansomBroker, hero.PartyBelongedToAsPrisoner.MobileParty, false, ref explainedNumber);
					}
				}
				if (hero.PartyBelongedToAsPrisoner.IsMobile && hero.PartyBelongedToAsPrisoner.MobileParty.HasPerk(DefaultPerks.Scouting.KeenSight, true))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.KeenSight, hero.PartyBelongedToAsPrisoner.MobileParty, false, ref explainedNumber);
				}
				if (MBRandom.RandomFloat < explainedNumber.ResultNumber)
				{
					EndCaptivityAction.ApplyByEscape(hero, null);
				}
			}
		}

		// Token: 0x06003A1F RID: 14879 RVA: 0x0010BCF8 File Offset: 0x00109EF8
		private void HourlyPartyTick(MobileParty mobileParty)
		{
			int prisonerSizeLimit = mobileParty.Party.PrisonerSizeLimit;
			if (mobileParty.MapEvent == null && mobileParty.SiegeEvent == null && mobileParty.PrisonRoster.TotalManCount > prisonerSizeLimit)
			{
				int num = mobileParty.PrisonRoster.TotalManCount - prisonerSizeLimit;
				for (int i = 0; i < num; i++)
				{
					bool flag = mobileParty.PrisonRoster.TotalRegulars > 0;
					float randomFloat = MBRandom.RandomFloat;
					int num2 = (flag ? ((int)((float)mobileParty.PrisonRoster.TotalRegulars * randomFloat)) : ((int)((float)mobileParty.PrisonRoster.TotalManCount * randomFloat)));
					CharacterObject characterObject = null;
					foreach (TroopRosterElement troopRosterElement in mobileParty.PrisonRoster.GetTroopRoster())
					{
						if (!troopRosterElement.Character.IsHero || !flag)
						{
							num2 -= troopRosterElement.Number;
							if (num2 <= 0)
							{
								characterObject = troopRosterElement.Character;
								break;
							}
						}
					}
					this.ApplyEscapeChanceToExceededPrisoners(characterObject, mobileParty);
				}
			}
		}

		// Token: 0x06003A20 RID: 14880 RVA: 0x0010BE18 File Offset: 0x0010A018
		private void ApplyEscapeChanceToExceededPrisoners(CharacterObject character, MobileParty capturerParty)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0.1f, false, null);
			if (capturerParty.HasPerk(DefaultPerks.Athletics.Stamina, true))
			{
				explainedNumber.AddFactor(-0.1f, DefaultPerks.Athletics.Stamina.Name);
			}
			if (capturerParty.IsGarrison || capturerParty.IsMilitia || character.IsPlayerCharacter)
			{
				return;
			}
			if (MBRandom.RandomFloat < explainedNumber.ResultNumber)
			{
				if (character.IsHero)
				{
					EndCaptivityAction.ApplyByEscape(character.HeroObject, null);
					return;
				}
				capturerParty.PrisonRoster.AddToCounts(character, -1, false, 0, 0, true, -1);
			}
		}

		// Token: 0x06003A21 RID: 14881 RVA: 0x0010BEA8 File Offset: 0x0010A0A8
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			foreach (TroopRosterElement troopRosterElement in settlement.Party.PrisonRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero && troopRosterElement.Character.HeroObject != Hero.MainHero && !troopRosterElement.Character.HeroObject.MapFaction.IsAtWarWith(settlement.MapFaction))
				{
					if (troopRosterElement.Character.HeroObject.PartyBelongedToAsPrisoner == settlement.Party && troopRosterElement.Character.HeroObject.IsPrisoner)
					{
						EndCaptivityAction.ApplyByReleasedAfterBattle(troopRosterElement.Character.HeroObject);
					}
					else
					{
						settlement.Party.PrisonRoster.RemoveTroop(troopRosterElement.Character, troopRosterElement.Number, default(UniqueTroopDescriptor), 0);
					}
				}
			}
		}
	}
}
