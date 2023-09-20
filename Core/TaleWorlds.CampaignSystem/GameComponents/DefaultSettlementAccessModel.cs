using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000134 RID: 308
	public class DefaultSettlementAccessModel : SettlementAccessModel
	{
		// Token: 0x060016EB RID: 5867 RVA: 0x00070A54 File Offset: 0x0006EC54
		public override void CanMainHeroEnterSettlement(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			if (settlement.IsFortification && Hero.MainHero.MapFaction == settlement.MapFaction && (settlement.Town.GarrisonParty == null || settlement.Town.GarrisonParty.Party.NumberOfAllMembers == 0))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.Direct
				};
				return;
			}
			if (settlement.IsTown)
			{
				this.CanMainHeroEnterTown(settlement, out accessDetails);
				return;
			}
			if (settlement.IsCastle)
			{
				this.CanMainHeroEnterCastle(settlement, out accessDetails);
				return;
			}
			if (settlement.IsVillage)
			{
				this.CanMainHeroEnterVillage(settlement, out accessDetails);
				return;
			}
			Debug.FailedAssert("Invalid type of settlement", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameComponents\\DefaultSettlementAccessModel.cs", "CanMainHeroEnterSettlement", 41);
			accessDetails = new SettlementAccessModel.AccessDetails
			{
				AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
				AccessMethod = SettlementAccessModel.AccessMethod.Direct
			};
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x00070B27 File Offset: 0x0006ED27
		public override void CanMainHeroEnterDungeon(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			accessDetails = default(SettlementAccessModel.AccessDetails);
			this.CanMainHeroEnterKeepInternal(settlement, out accessDetails);
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x00070B38 File Offset: 0x0006ED38
		public override void CanMainHeroEnterLordsHall(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			accessDetails = default(SettlementAccessModel.AccessDetails);
			this.CanMainHeroEnterKeepInternal(settlement, out accessDetails);
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x00070B4C File Offset: 0x0006ED4C
		private void CanMainHeroEnterKeepInternal(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			accessDetails = default(SettlementAccessModel.AccessDetails);
			Hero mainHero = Hero.MainHero;
			if (settlement.OwnerClan == mainHero.Clan)
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.Direct
				};
			}
			else if (FactionManager.IsAlliedWithFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.Direct
				};
			}
			else if (FactionManager.IsNeutralWithFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				if (Campaign.Current.IsMainHeroDisguised)
				{
					accessDetails = new SettlementAccessModel.AccessDetails
					{
						AccessLevel = SettlementAccessModel.AccessLevel.LimitedAccess,
						LimitedAccessSolution = SettlementAccessModel.LimitedAccessSolution.Disguise,
						AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.Disguised
					};
				}
				else if (Campaign.Current.Models.CrimeModel.DoesPlayerHaveAnyCrimeRating(settlement.MapFaction))
				{
					accessDetails = new SettlementAccessModel.AccessDetails
					{
						AccessLevel = SettlementAccessModel.AccessLevel.LimitedAccess,
						LimitedAccessSolution = SettlementAccessModel.LimitedAccessSolution.Bribe,
						AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.CrimeRating
					};
				}
				else if (mainHero.Clan.Tier < 3)
				{
					accessDetails = new SettlementAccessModel.AccessDetails
					{
						AccessLevel = SettlementAccessModel.AccessLevel.LimitedAccess,
						LimitedAccessSolution = SettlementAccessModel.LimitedAccessSolution.Bribe,
						AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.ClanTier
					};
				}
				else
				{
					accessDetails = new SettlementAccessModel.AccessDetails
					{
						AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
						AccessMethod = SettlementAccessModel.AccessMethod.Direct
					};
				}
			}
			else if (FactionManager.IsAtWarAgainstFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.LimitedAccess,
					LimitedAccessSolution = SettlementAccessModel.LimitedAccessSolution.Disguise,
					AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.Disguised
				};
			}
			if (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && (accessDetails.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Bribe || accessDetails.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Disguise) && settlement.LocationComplex.GetListOfCharactersInLocation("lordshall").IsEmpty<LocationCharacter>() && settlement.LocationComplex.GetListOfCharactersInLocation("prison").IsEmpty<LocationCharacter>())
			{
				accessDetails.AccessLevel = SettlementAccessModel.AccessLevel.NoAccess;
				accessDetails.AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.LocationEmpty;
			}
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x00070D50 File Offset: 0x0006EF50
		public override bool CanMainHeroAccessLocation(Settlement settlement, string locationId, out bool disableOption, out TextObject disabledText)
		{
			disabledText = TextObject.Empty;
			disableOption = false;
			bool flag = true;
			if (locationId == "center")
			{
				flag = this.CanMainHeroWalkAroundTownCenter(settlement, out disableOption, out disabledText);
			}
			else if (locationId == "arena")
			{
				flag = this.CanMainHeroGoToArena(settlement, out disableOption, out disabledText);
			}
			else if (locationId == "tavern")
			{
				flag = this.CanMainHeroGoToTavern(settlement, out disableOption, out disabledText);
			}
			else if (locationId == "lordshall")
			{
				if (settlement.IsCastle)
				{
					flag = true;
				}
				else
				{
					SettlementAccessModel.AccessDetails accessDetails;
					this.CanMainHeroEnterLordsHall(settlement, out accessDetails);
					if (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && accessDetails.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Bribe)
					{
						flag = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterLordsHall(settlement) == 0;
					}
					else
					{
						flag = accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.FullAccess;
					}
				}
			}
			else if (locationId == "prison")
			{
				SettlementAccessModel.AccessDetails accessDetails2;
				this.CanMainHeroEnterDungeon(settlement, out accessDetails2);
				if (accessDetails2.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && accessDetails2.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Bribe)
				{
					flag = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterDungeon(settlement) == 0;
				}
				else
				{
					flag = accessDetails2.AccessLevel == SettlementAccessModel.AccessLevel.FullAccess;
				}
			}
			else if (locationId == "house_1" || locationId == "house_2" || locationId == "house_3")
			{
				Location locationWithId = settlement.LocationComplex.GetLocationWithId(locationId);
				flag = locationWithId.IsReserved && (locationWithId.SpecialItems.Count > 0 || locationWithId.GetCharacterList().Any<LocationCharacter>());
			}
			else
			{
				Debug.FailedAssert("invalid location which is not supported by DefaultSettlementAccessModel", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameComponents\\DefaultSettlementAccessModel.cs", "CanMainHeroAccessLocation", 207);
			}
			return flag;
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x00070EF0 File Offset: 0x0006F0F0
		public override bool IsRequestMeetingOptionAvailable(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			bool flag = true;
			disableOption = false;
			disabledText = TextObject.Empty;
			SettlementAccessModel.AccessDetails accessDetails;
			this.CanMainHeroEnterSettlement(settlement, out accessDetails);
			if (settlement.OwnerClan == Clan.PlayerClan)
			{
				flag = false;
			}
			else if (FactionManager.IsAlliedWithFaction(settlement.MapFaction, Clan.PlayerClan.MapFaction) && accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.NoAccess)
			{
				flag = TownHelpers.IsThereAnyoneToMeetInTown(settlement);
			}
			else if (settlement.IsTown && FactionManager.IsNeutralWithFaction(Hero.MainHero.MapFaction, settlement.MapFaction) && Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingMild(settlement.MapFaction))
			{
				flag = false;
			}
			else if (Clan.PlayerClan.Tier < 3)
			{
				disableOption = true;
				disabledText = new TextObject("{=bdzZUVxf}Your clan tier is not high enough to request a meeting.", null);
				flag = true;
			}
			else if (TownHelpers.IsThereAnyoneToMeetInTown(settlement))
			{
				flag = true;
			}
			else
			{
				disableOption = true;
				disabledText = new TextObject("{=196tGVIm}There are no nobles to meet.", null);
			}
			return flag;
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x00070FCC File Offset: 0x0006F1CC
		public override bool CanMainHeroDoSettlementAction(Settlement settlement, SettlementAccessModel.SettlementAction settlementAction, out bool disableOption, out TextObject disabledText)
		{
			switch (settlementAction)
			{
			case SettlementAccessModel.SettlementAction.RecruitTroops:
				return this.CanMainHeroRecruitTroops(settlement, out disableOption, out disabledText);
			case SettlementAccessModel.SettlementAction.Craft:
				return this.CanMainHeroCraft(settlement, out disableOption, out disabledText);
			case SettlementAccessModel.SettlementAction.WalkAroundTheArena:
				return this.CanMainHeroEnterArena(settlement, out disableOption, out disabledText);
			case SettlementAccessModel.SettlementAction.JoinTournament:
				return this.CanMainHeroJoinTournament(settlement, out disableOption, out disabledText);
			case SettlementAccessModel.SettlementAction.WatchTournament:
				return this.CanMainHeroWatchTournament(settlement, out disableOption, out disabledText);
			case SettlementAccessModel.SettlementAction.Trade:
				return this.CanMainHeroTrade(settlement, out disableOption, out disabledText);
			case SettlementAccessModel.SettlementAction.WaitInSettlement:
				return this.CanMainHeroWaitInSettlement(settlement, out disableOption, out disabledText);
			case SettlementAccessModel.SettlementAction.ManageTown:
				return this.CanMainHeroManageTown(settlement, out disableOption, out disabledText);
			default:
				Debug.FailedAssert("Invalid Settlement Action", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameComponents\\DefaultSettlementAccessModel.cs", "CanMainHeroDoSettlementAction", 276);
				disableOption = false;
				disabledText = TextObject.Empty;
				return true;
			}
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x0007107E File Offset: 0x0006F27E
		private bool CanMainHeroGoToArena(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			if (Campaign.Current.IsDay)
			{
				disabledText = TextObject.Empty;
				disableOption = false;
				return true;
			}
			disabledText = new TextObject("{=wsbkjJhz}Arena is closed at night.", null);
			disableOption = true;
			return false;
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x000710A9 File Offset: 0x0006F2A9
		private bool CanMainHeroGoToTavern(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			disabledText = TextObject.Empty;
			disableOption = false;
			return true;
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x000710B6 File Offset: 0x0006F2B6
		private bool CanMainHeroEnterArena(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			if (Campaign.Current.IsMainHeroDisguised)
			{
				disableOption = true;
				disabledText = new TextObject("{=brzz79Je}You cannot enter arena while in disguise.", null);
				return false;
			}
			disableOption = false;
			disabledText = TextObject.Empty;
			return true;
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x000710E4 File Offset: 0x0006F2E4
		private void CanMainHeroEnterVillage(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			Hero mainHero = Hero.MainHero;
			accessDetails = new SettlementAccessModel.AccessDetails
			{
				AccessLevel = SettlementAccessModel.AccessLevel.NoAccess,
				AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.None,
				PreliminaryActionObligation = SettlementAccessModel.PreliminaryActionObligation.None,
				PreliminaryActionType = SettlementAccessModel.PreliminaryActionType.None
			};
			MobileParty partyBelongedTo = mainHero.PartyBelongedTo;
			if (partyBelongedTo != null && (partyBelongedTo.Army == null || partyBelongedTo.Army.LeaderParty == partyBelongedTo))
			{
				accessDetails.AccessLevel = SettlementAccessModel.AccessLevel.FullAccess;
				accessDetails.AccessMethod = SettlementAccessModel.AccessMethod.Direct;
			}
			if (settlement.Village.VillageState == Village.VillageStates.Looted)
			{
				accessDetails.AccessLevel = SettlementAccessModel.AccessLevel.NoAccess;
				accessDetails.AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.VillageIsLooted;
			}
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x0007116E File Offset: 0x0006F36E
		private bool CanMainHeroManageTown(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			disabledText = TextObject.Empty;
			disableOption = false;
			return settlement.IsTown && settlement.OwnerClan.Leader == Hero.MainHero;
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x00071198 File Offset: 0x0006F398
		private void CanMainHeroEnterCastle(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			Hero mainHero = Hero.MainHero;
			accessDetails = default(SettlementAccessModel.AccessDetails);
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			if (settlementComponent != null && settlementComponent.IsTaken && settlementComponent.Owner.MapFaction == Hero.MainHero.MapFaction)
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					PreliminaryActionObligation = SettlementAccessModel.PreliminaryActionObligation.Must,
					PreliminaryActionType = SettlementAccessModel.PreliminaryActionType.SettlementIsTaken
				};
				return;
			}
			if (settlement.OwnerClan == mainHero.Clan)
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.Direct
				};
				return;
			}
			if (FactionManager.IsAlliedWithFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.ByRequest
				};
				if (!settlement.Town.IsOwnerUnassigned && settlement.OwnerClan.Leader.GetRelationWithPlayer() < -4f && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
				{
					accessDetails.AccessLevel = SettlementAccessModel.AccessLevel.NoAccess;
					accessDetails.AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.RelationshipWithOwner;
					return;
				}
			}
			else if (FactionManager.IsNeutralWithFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.ByRequest
				};
				if (Campaign.Current.Models.CrimeModel.DoesPlayerHaveAnyCrimeRating(settlement.MapFaction))
				{
					accessDetails.AccessLevel = SettlementAccessModel.AccessLevel.NoAccess;
					accessDetails.AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.CrimeRating;
					return;
				}
				if (settlement.OwnerClan.Leader.GetRelationWithPlayer() < 0f)
				{
					accessDetails.AccessLevel = SettlementAccessModel.AccessLevel.NoAccess;
					accessDetails.AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.RelationshipWithOwner;
					return;
				}
			}
			else if (FactionManager.IsAtWarAgainstFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.NoAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.ByRequest,
					AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.HostileFaction
				};
			}
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x0007136C File Offset: 0x0006F56C
		private void CanMainHeroEnterTown(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			Hero mainHero = Hero.MainHero;
			accessDetails = default(SettlementAccessModel.AccessDetails);
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			if (settlementComponent != null && settlementComponent.IsTaken && settlementComponent.Owner.MapFaction == Hero.MainHero.MapFaction)
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					PreliminaryActionObligation = SettlementAccessModel.PreliminaryActionObligation.Must,
					PreliminaryActionType = SettlementAccessModel.PreliminaryActionType.SettlementIsTaken
				};
				return;
			}
			if (settlement.OwnerClan == mainHero.Clan)
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.Direct
				};
				return;
			}
			if (FactionManager.IsAlliedWithFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.Direct
				};
				if (Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(settlement.MapFaction) || Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(settlement.MapFaction))
				{
					accessDetails.PreliminaryActionType = SettlementAccessModel.PreliminaryActionType.FaceCharges;
					accessDetails.PreliminaryActionObligation = SettlementAccessModel.PreliminaryActionObligation.Optional;
					return;
				}
			}
			else if (FactionManager.IsNeutralWithFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.FullAccess,
					AccessMethod = SettlementAccessModel.AccessMethod.Direct
				};
				if (Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(settlement.MapFaction) || Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(settlement.MapFaction))
				{
					accessDetails.AccessLevel = SettlementAccessModel.AccessLevel.LimitedAccess;
					accessDetails.AccessMethod = SettlementAccessModel.AccessMethod.None;
					accessDetails.LimitedAccessSolution = SettlementAccessModel.LimitedAccessSolution.Disguise;
					accessDetails.AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.CrimeRating;
					return;
				}
			}
			else if (FactionManager.IsAtWarAgainstFaction(mainHero.MapFaction, settlement.MapFaction))
			{
				accessDetails = new SettlementAccessModel.AccessDetails
				{
					AccessLevel = SettlementAccessModel.AccessLevel.LimitedAccess,
					LimitedAccessSolution = SettlementAccessModel.LimitedAccessSolution.Disguise,
					AccessLimitationReason = SettlementAccessModel.AccessLimitationReason.HostileFaction
				};
			}
		}

		// Token: 0x060016F9 RID: 5881 RVA: 0x0007153A File Offset: 0x0006F73A
		private bool CanMainHeroWalkAroundTownCenter(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			disabledText = TextObject.Empty;
			disableOption = false;
			return settlement.IsTown || settlement.IsCastle;
		}

		// Token: 0x060016FA RID: 5882 RVA: 0x00071556 File Offset: 0x0006F756
		private bool CanMainHeroRecruitTroops(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			disabledText = TextObject.Empty;
			disableOption = false;
			return true;
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x00071563 File Offset: 0x0006F763
		private bool CanMainHeroCraft(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			disableOption = false;
			disabledText = TextObject.Empty;
			return Campaign.Current.IsCraftingEnabled;
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x0007157C File Offset: 0x0006F77C
		private bool CanMainHeroJoinTournament(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			bool flag = settlement.Town.HasTournament && Campaign.Current.IsDay;
			disableOption = false;
			disabledText = TextObject.Empty;
			if (!flag)
			{
				return false;
			}
			if (Campaign.Current.IsMainHeroDisguised)
			{
				disableOption = true;
				disabledText = new TextObject("{=mu6Xl4RS}You cannot enter the tournament while disguised.", null);
				return false;
			}
			if (Hero.MainHero.IsWounded)
			{
				disableOption = true;
				disabledText = new TextObject("{=68rmPu7Z}Your health is too low to fight.", null);
				return false;
			}
			return true;
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x000715EE File Offset: 0x0006F7EE
		private bool CanMainHeroWatchTournament(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			disableOption = false;
			disabledText = TextObject.Empty;
			return settlement.Town.HasTournament && Campaign.Current.IsDay;
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x00071613 File Offset: 0x0006F813
		private bool CanMainHeroTrade(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			if (Campaign.Current.IsMainHeroDisguised)
			{
				disableOption = true;
				disabledText = new TextObject("{=shU7OlQT}You cannot trade while in disguise.", null);
				return false;
			}
			disableOption = false;
			disabledText = TextObject.Empty;
			return true;
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x00071640 File Offset: 0x0006F840
		private bool CanMainHeroWaitInSettlement(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			disableOption = false;
			disabledText = TextObject.Empty;
			if (Campaign.Current.IsMainHeroDisguised)
			{
				disableOption = true;
				disabledText = new TextObject("{=dN5Qc9vN}You cannot wait in town while disguised.", null);
				return false;
			}
			if (settlement.IsVillage && settlement.Party.MapEvent != null)
			{
				disableOption = true;
				disabledText = new TextObject("{=dN5Qc7vN}You cannot wait in village while it is being raided.", null);
				return false;
			}
			return MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
		}
	}
}
